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
        // номер задачи
        public string TaskNumber { get; set; }

        public string TaskNumberToFilename
        {
            get
            {
                return this.TaskNumber.Replace("-", String.Empty).Replace(" ", String.Empty).ToLower();
            }
        }

        // номер скрипта
        string _script_num;

        [JsonIgnore]
        public string ScriptNumber 
        {
            get
            {
                if (_script_num == null) _script_num = "";
                if (_script_num == "") _script_num = "0";
                return _script_num;
            }
            set
            {
                _script_num = value.Trim();
            }
        }

        public string ScriptNumberToFilename
        {
            get
            {
                return this.ScriptNumber.Replace("-", String.Empty).Replace(" ", String.Empty).ToLower();
            }
        }

        // целевая БД
        public TargetDBType TargetDB { get; set; }
        public string TargetDBTypeToFilename
        {
            get
            {
                switch (this.TargetDB)
                {
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        return "pg";
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                    case TargetDBType.None:
                    default:
                        return "ms";
                }
            }
        }

        // Тип скрипта
        public ScriptType ScriptType { get; set; }
        public string ScriptTypeToFilename
        {
            get
            {
                switch (this.ScriptType)
                {
                    case ScriptType.UPDATE:
                        return "update";
                    case ScriptType.DELETE:
                        return "delete";
                    case ScriptType.INSERT_UPDATE:
                        if ( (this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE) )
                            return "upsert";
                        else
                            return "merge";
                    case ScriptType.INSERT:
                    default:
                        return "insert";
                }
            }
        }

        public QueryDB()
        {
            this.TargetDB = TargetDBType.MSSQL;
            this.ScriptType = ScriptType.INSERT;
            this.DataTable = new DataTable();
            this.IsUpdateDT = true;
        }

        // Имя таблицы
        string _table_name;
        public string TableName
        {
            get
            {
                if (_table_name == null) _table_name= "";
                return _table_name;
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

        public string TableNameToFilename
        {
            get
            {
                return this.TableName.Replace(" ", String.Empty).Replace(".", " ").ToLower();
            }
        }

        // Обновлять insDT/UpdDT
        bool _isupdatedt;
        public bool IsUpdateDT
        {
            get { return _isupdatedt; }
            set { _isupdatedt = value; }
        }

        [JsonIgnore]
        public string IsUpdateDT_string
        {
            get { if (_isupdatedt == true) return "true"; else return "false"; }
            set
            {
                if ((value == null) || (value.Trim().ToLower() == "") || (value.Trim().ToLower() != "true")) _isupdatedt = false;
                else _isupdatedt = true;
            }
        }

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

        // имя файла со скриптом
        //string _filename;
        public string ScriptFilename
        {
            get
            {
//                if ((_filename == null) || (_filename == ""))
                {
                    string s = this.TargetDBTypeToFilename + " " + this.TaskNumberToFilename + " " + this.ScriptNumberToFilename + " " + this.ScriptTypeToFilename;
                    if (this.TableName != "") s = s + " " + this.TableNameToFilename;
                    s = s + ".sql";
                    return s;
                }
//                else return _filename.Trim();
            }
/*            set
            {
                _filename = value.Trim();
            }*/
        }


        public void Fill(QueryDB _query)
        {
            if (_query != null)
            {
                this.TaskNumber = _query.TaskNumber;
                this.TargetDB = _query.TargetDB;
                this.ScriptType = _query.ScriptType;
                this.TableName = _query.TableName;
                this.IsUpdateDT = _query.IsUpdateDT;
                this.PrimaryKey = _query.PrimaryKey;
                this.SQLQuery = _query.SQLQuery;
                this.SQLScript = _query.SQLScript;
                //this.ScriptFilename = _query.ScriptFilename;
            }
        }


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
                return "" + row[column].ToString().Replace(",",".");
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


        public void GenerateScript(string TitleScript, System.IO.StreamWriter file, out string Script)
        {
            // заголовок скрипта (информация о задаче)
            if (TitleScript == null) TitleScript = "";
            if (TitleScript != "") Script = TitleScript + Environment.NewLine;
            else Script = "";

            // Текст запроса в комментарии
            Script = Script + Environment.NewLine + "/* запрос к базе";
            Script = Script + Environment.NewLine + this.SQLQuery;
            Script = Script + Environment.NewLine + "*/";
            Script = Script + Environment.NewLine;

            if (file != null)
            {
                file.Write(Script);
                Script = "";
            }

            // Перед INSERT или INSERT/UPDATE
            if ((ScriptType == ScriptType.INSERT) || (ScriptType == ScriptType.INSERT_UPDATE))
            {
                if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE))
                {
                    Script = Script + Environment.NewLine + "SET IDENTITY_INSERT " + this.TableNameToScript + " ON";
                    Script = Script + Environment.NewLine + "GO";
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

            if (file != null)
            {
                file.WriteLine(Script);
                Script = "";
            }

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
                    addLine = "INSERT INTO " + TableNameToScript + " " + mshint_change + "(" + fields + ") "+
                    Environment.NewLine+"VALUES (" + insvalues + pgend + ";";
                }

                // INSERT/UPDATE
                if (ScriptType == ScriptType.INSERT_UPDATE)
                {
                    if (where == "") throw new ArgumentException($"В таблице " + TableName + " не найдены поля Primary Key: " + keys + " !");

                    if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE)) // для MS SQL
                    {
                        addLine = "MERGE " + TableNameToScript + " " + mshint_change + "AS target "+
                        Environment.NewLine+"USING (SELECT " + keyvalues + ") AS source (" + keys + ")" +
                        Environment.NewLine+"ON " + where + " " +
                        Environment.NewLine+"WHEN MATCHED THEN UPDATE SET " + updvalues +
                        Environment.NewLine+"WHEN NOT MATCHED THEN INSERT (" + fields + ")" +
                        Environment.NewLine+"VALUES (" + insvalues + ");";
                    }

                    if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) // для PG SQL
                    {
                        addLine = "INSERT INTO " + TableNameToScript + " (" + fields + ") "+
                        Environment.NewLine+"VALUES (" + insvalues + ") " +
                        Environment.NewLine+"ON CONFLICT (" + keys + ") DO UPDATE SET " + updvalues + ";";
                    }
                }

                // UPDATE
                if (ScriptType == ScriptType.UPDATE)
                {
                    if (where == "") throw new ArgumentException($"В таблице " + TableName + " не найдены поля Primary Key: " + keys + " !");
                    addLine = "UPDATE " + TableNameToScript + " " + mshint_change + 
                    Environment.NewLine+"SET " + updvalues + " "+
                    Environment.NewLine+"WHERE " + where + ";";
                }

                // DELETE
                if (ScriptType == ScriptType.DELETE)
                {
                    if (where == "") throw new ArgumentException($"В таблице " + TableName + " не найдены поля Primary Key: " + keys + " !");
                    addLine = "DELETE FROM " + TableNameToScript + " " + mshint_change + "WHERE " + where + ";";
                }


                if (addLine != "")
                {
                    Script = Script + Environment.NewLine + addLine;
                    if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE))
                    {
                        Script = Script + Environment.NewLine + "GO";
                    }

                    if (file != null)
                    {
                        file.Write(Script);
                        Script = "";
                    }
                }
            }

            // После INSERT или INSERT/UPDATE
            if ((ScriptType == ScriptType.INSERT) || (ScriptType == ScriptType.INSERT_UPDATE))
            {
                if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE))
                {
                    Script = Script + Environment.NewLine + "SET IDENTITY_INSERT " + TableName + " OFF";
                    Script = Script + Environment.NewLine + "GO";
                }
            }

            // Проверка
            Script = Script + Environment.NewLine+Environment.NewLine+"-- Проверка";
            Script = Script + Environment.NewLine+ "-- SELECT * FROM " + TableNameToScript + " " + mshint_sel + ";";

            if (file != null)
            {
                file.WriteLine(Script);
                Script = "";
            }
        }

    }

}
