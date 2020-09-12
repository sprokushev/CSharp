using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SQLGen
{
    public class QueryDB
    {
        // целевая БД
        public TargetDBType TargetDB { get; set; }

        // Тип скрипта
        public ScriptType ScriptType { get; set; }

        public QueryDB()
        {
            this.TargetDB = TargetDBType.None;
            this.DataTable = new DataTable();
            this.IsUpdateDT = true;
        }

        // Имя таблицы
        string _table_name;
        public string TableName
        {
            get
            {
                if (_table_name == null) return "";
                else return _table_name.Trim();
            }
            set
            {
                _table_name = value.Trim();
            }
        }

        public string TableNameToScript
        {
            get
            {
                switch (this.TargetDB)
                {
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                        return this.TableName;
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        return this.TableName.ToLower();
                    default:
                        return this.TableName;
                }
            }
        }

        // Обновлять insDT/UpdDT
        public bool IsUpdateDT { get; set; }

        // PrimaryKey
        string _pk;
        public string PrimaryKey
        {
            get
            {
                if (_pk == null) return "";
                else return _pk.Trim();
            }
            set
            {
                _pk = value.Trim();
            }
        }


        // текст SQL-запроса
        public string SQLQuery { get; set; }

        // таблица с результатами SQL-запроса
        internal DataTable DataTable;

        // текст итогового SQL-скрипта
        public string SQLScript { get; set; }



        string ColumnValue(DataRow row, DataColumn column)
        {
            // пропускаем rowversion
            if (column.ColumnName.ToLower().IndexOf("rowversion") != -1) throw new ArgumentException($"Необрабатываемый тип данных: " + column.ColumnName + " " + column.DataType.FullName);
            if (column.ColumnName.ToLower().IndexOf("timestamp") != -1) throw new ArgumentException($"Необрабатываемый тип данных: " + column.ColumnName + " " + column.DataType.FullName);

            if (row.IsNull(column))
            {
                return "NULL";
            }
            else if (IsUpdateDT && (
                (column.ColumnName.ToLower().IndexOf("_insdt") != -1) ||
                (column.ColumnName.ToLower().IndexOf("_upddt") != -1)
                ))
            {
                return "getdate()";
            }
            else if (IsUpdateDT && (
                (column.ColumnName.ToLower().IndexOf("_insid") != -1) ||
                (column.ColumnName.ToLower().IndexOf("_updid") != -1)
                ))
            {
                return "1";
            }
            else if (
                Object.ReferenceEquals(column.DataType, typeof(Byte)) ||
                Object.ReferenceEquals(column.DataType, typeof(SByte)) ||
                Object.ReferenceEquals(column.DataType, typeof(Single)) ||
                Object.ReferenceEquals(column.DataType, typeof(Int16)) ||
                Object.ReferenceEquals(column.DataType, typeof(UInt16)) ||
                Object.ReferenceEquals(column.DataType, typeof(Int32)) ||
                Object.ReferenceEquals(column.DataType, typeof(UInt32)) ||
                Object.ReferenceEquals(column.DataType, typeof(Decimal)) ||
                Object.ReferenceEquals(column.DataType, typeof(Double)) ||
                Object.ReferenceEquals(column.DataType, typeof(UInt64)) ||
                Object.ReferenceEquals(column.DataType, typeof(Int64))
                )
            {
                return "" + row[column];
            }
            else if (
                Object.ReferenceEquals(column.DataType, typeof(Char)) ||
                Object.ReferenceEquals(column.DataType, typeof(Guid)) ||
                Object.ReferenceEquals(column.DataType, typeof(String))
                )
            {
                if ( (this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE) )
                    return "N'" + row[column] + "'";
                else
                    return "'" + row[column] + "'";
            }
            else if (
                Object.ReferenceEquals(column.DataType, typeof(DateTime))
                )
            {
                DateTime d = (DateTime)(row[column]);
                return "'" + d.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else throw new ArgumentException($"Необрабатываемый тип данных: " + column.ColumnName + " " + column.DataType.FullName);
        }


        public string GenerateScript()
        {
                string Script = "";

                // Перед INSERT или INSERT/UPDATE
                if ((ScriptType == ScriptType.INSERT) || (ScriptType == ScriptType.INSERT_UPDATE))
                {
                    if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE))
                    {
                        if (Script != "") Script = Script + "\n";
                        Script = Script + "SET IDENTITY_INSERT " + this.TableNameToScript + " ON";
                        if (Script != "") Script = Script + "\n";
                        Script = Script + "GO";
                    }
                }

                // хинт для MS SQL
                string mshint_change = "";
                if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE)) mshint_change = "WITH (rowlock) ";
                string mshint_sel = "";
                if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE)) mshint_sel = "WITH (nolock) ";

                // концовка оператора INSERT для PG SQL
                string pgend = ")";
                if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) pgend = ") ON CONFLICT DO NOTHING";


                // Перебираем строки
                foreach (DataRow row in this.DataTable.Rows)
                {
                    string addLine = "";
                    string fields = "";
                    string insvalues = "";
                    string updvalues = "";
                    string where = "";
                    string keys = this.PrimaryKey;
                    string[] PK = keys.ToLower().Split(',');
                    for (int i = 0; i < PK.Count(); i++) { PK[i] = PK[i].Trim(); }
                    string keyvalues = "";

                    // для PG SQL имя колонки в нижний регистр
                    if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) keys = keys.ToLower();

                    foreach (DataColumn column in this.DataTable.Columns)
                    {

                        string ColumnName = column.ColumnName;

                        // для PG SQL имя колонки в нижний регистр
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) ColumnName = ColumnName.ToLower();

                        // пропускаем rowversion
                        if (ColumnName.ToLower().IndexOf("rowversion") != -1) continue;
                        if (ColumnName.ToLower().IndexOf("timestamp") != -1) continue;


                        if (fields != "") fields = fields + ", ";
                        fields = fields + ColumnName;

                        if (insvalues != "") insvalues = insvalues + ", ";
                        insvalues = insvalues + ColumnValue(row, column);

                        var pos = Array.IndexOf(PK, ColumnName.ToLower());
                        if ((pos != -1) && (ScriptType != ScriptType.INSERT)) // условие where НЕ нужно только для "чистого" INSERT
                        {
                            if (where != "") where = where + " AND ";
                            if (ScriptType == ScriptType.INSERT_UPDATE) // для INSERT/UPDATE
                            {
                                where = where + "target." + ColumnName + " = source." + ColumnName;
                            }
                            else // для UPDATE или DELETE
                            {
                                where = where + ColumnName + " = " + ColumnValue(row, column);
                            }

                            if (keyvalues != "") keyvalues = keyvalues + ", ";
                            keyvalues = keyvalues + ColumnValue(row, column);
                        }
                        else
                        {

                            if ((ColumnName.ToLower().IndexOf("_insdt") == -1) &&
                                (ColumnName.ToLower().IndexOf("_insid") == -1)) // поля _insdt и _insid не обновляем
                            {
                                if (updvalues != "") updvalues = updvalues + ", ";
                                updvalues = updvalues + ColumnName + " = " + ColumnValue(row, column);
                            }
                        }
                    }

                    // INSERT
                    if (ScriptType == ScriptType.INSERT)
                    {
                        addLine = "INSERT INTO " + TableNameToScript + " " + mshint_change + "(" + fields + ") \nVALUES (" + insvalues + pgend + ";";
                    }

                    // INSERT/UPDATE
                    if (ScriptType == ScriptType.INSERT_UPDATE)
                    {
                        if (where == "") throw new ArgumentException($"В таблице " + TableName + " не найдены поля Primary Key: " + keys + " !");

                        if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE)) // для MS SQL
                        {
                            addLine = "MERGE " + TableNameToScript + " " + mshint_change + "AS target \nUSING (SELECT " + keyvalues + ") AS source (" + keys + ")" +
                            "\nON " + where + " " +
                            "\nWHEN MATCHED THEN UPDATE SET " + updvalues +
                            "\nWHEN NOT MATCHED THEN INSERT (" + fields + ")" +
                            "\nVALUES (" + insvalues + ");";
                        }

                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) // для PG SQL
                        {
                            addLine = "INSERT INTO " + TableNameToScript + " (" + fields + ") \nVALUES (" + insvalues + ") " +
                            "\nON CONFLICT (" + keys + ") DO UPDATE SET " + updvalues + ";";
                        }
                    }

                    // UPDATE
                    if (ScriptType == ScriptType.UPDATE)
                    {
                        if (where == "") throw new ArgumentException($"В таблице " + TableName + " не найдены поля Primary Key: " + keys + " !");
                        addLine = "UPDATE " + TableNameToScript + " " + mshint_change + "\nSET " + updvalues + " \nWHERE " + where + ";";
                    }

                    // DELETE
                    if (ScriptType == ScriptType.DELETE)
                    {
                        if (where == "") throw new ArgumentException($"В таблице " + TableName + " не найдены поля Primary Key: " + keys + " !");
                        addLine = "DELETE FROM " + TableNameToScript + " " + mshint_change + "WHERE " + where + ";";
                    }


                    if (addLine != "")
                    {
                        if (Script != "") Script = Script + "\n";
                        Script = Script + addLine;
                        if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE))
                        {
                            if (Script != "") Script = Script + "\n";
                            Script = Script + "GO";
                        }
                    }
                }

                // После INSERT или INSERT/UPDATE
                if ((ScriptType == ScriptType.INSERT) || (ScriptType == ScriptType.INSERT_UPDATE))
                {
                    if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE))
                    {
                        if (Script != "") Script = Script + "\n";
                        Script = Script + "SET IDENTITY_INSERT " + TableName + " OFF";
                        if (Script != "") Script = Script + "\n";
                        Script = Script + "GO";
                    }
                }

                // Проверка
                if (Script != "") Script = Script + "\n\n-- Проверка\n";
                Script = Script + "-- SELECT * FROM " + TableNameToScript + " " + mshint_sel + ";";

                return Script;
        }

    }

}
