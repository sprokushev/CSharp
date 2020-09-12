using SQLGen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SQLGen
{

    public class TableDB
    {
        // целевая БД
        TargetDBType _targetdb;
        public TargetDBType TargetDB
        {
            get
            {
                return this._targetdb;
            }
            set
            {
                this._targetdb = value;
                if (TableOrig != null) TableOrig.TargetDB = this._targetdb;
                if (TableEdit != null) TableEdit.TargetDB = this._targetdb;
            }
        }

        // Тип скрипта
        public ScriptType ScriptType { get; set; }

        // Тип таблицы
        public TableType TableType { get; set; }

        // Оригинальная таблица
        public TableInfo TableOrig;

        // Измененная таблица
        public TableInfo TableEdit;

        // Флаг добавления Drop
        public Boolean isAddDrop { get; set; }

        // текст итогового SQL-скрипта
        public string SQLScript { get; set; }


        public TableDB()
        {
            this.TargetDB = TargetDBType.None;
            TableOrig = new TableInfo(this.TargetDB);
            TableEdit = new TableInfo(this.TargetDB);
        }

        public string RenameTableToScript()
        { 
            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    return "EXEC sp_rename '" + TableOrig.FullTableNameToScript + "', '" + TableOrig.FullTableNameToScript + "';\nGO";
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    return "ALTER TABLE " + TableOrig.FullTableNameToScript + " RENAME TO " + TableOrig.FullTableNameToScript + ";";
                default:
                    return "";
            }   
        }

        public string RenameFieldToScript(RowDB oldRow, RowDB newRow)
        {
            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    return "EXEC sp_rename '" + TableOrig.FullTableNameToScript + "."+oldRow.FieldNameToScript+"', '" + newRow.FieldNameToScript + "', 'COLUMN';\nGO";
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    return "ALTER TABLE " + TableOrig.FullTableNameToScript + " RENAME COLUMN " + oldRow.FieldNameToScript + " TO " + newRow.FieldNameToScript + ";";
                default:
                    return "";
            }
        }


        internal string DelSchemaFromTableName (string name)
        {
            var arr = name.Split('.');
            if (arr.Length > 1) name = arr[1];
            return name;
        }


        public string GenerateScript ()
        {
            string ScriptTable = "";
            string ScriptRow = "";
            string ScriptPK = "";
            string ScriptFK = "";
            string ScriptTableDesc = "";
            string ScriptFieldDesc = "";
            string ScriptIdent = "";
            string ScriptDropField = "";

            string ScriptDrop = "";
            string ScriptProc = "";

            // DROP
            if (this.isAddDrop == true) // Добавляем DROP
            {

                if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                ScriptDrop = ScriptDrop + "-- Удалить объекты (ОСТРОЖНО!!!)";

                if ( (this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE) ) // MSSQL
                {
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + @"IF OBJECT_ID('" + this.TableEdit.FullViewNameToScript + @"') IS NOT NULL
BEGIN
    DROP VIEW " + this.TableEdit.FullViewNameToScript + @";
END
GO";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + @"IF OBJECT_ID('" + this.TableEdit.FullProcINSToScript + @"') IS NOT NULL
BEGIN
    DROP PROCEDURE " + this.TableEdit.FullProcINSToScript + @";
END
GO";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + @"IF OBJECT_ID('" + this.TableEdit.FullProcUPDToScript + @"') IS NOT NULL
BEGIN
    DROP PROCEDURE " + this.TableEdit.FullProcUPDToScript + @";
END
GO";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + @"IF OBJECT_ID('" + this.TableEdit.FullProcDELToScript + @"') IS NOT NULL
BEGIN
    DROP PROCEDURE " + this.TableEdit.FullProcDELToScript + @";
END
GO";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + @"IF OBJECT_ID('" + this.TableEdit.FullTableNameToScript + @"') IS NOT NULL
BEGIN
    DROP TABLE " + this.TableEdit.FullTableNameToScript + @";
END
GO";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n\n";
                }
                else if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) // PGSQL
                {
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + "DROP VIEW IF EXISTS " + this.TableEdit.FullViewNameToScript + ";";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + "DROP FUNCTION IF EXISTS " + this.TableEdit.FullProcINSToScript + ";";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + "DROP FUNCTION IF EXISTS " + this.TableEdit.FullProcUPDToScript + ";";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + "DROP FUNCTION IF EXISTS " + this.TableEdit.FullProcDELToScript + ";";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n";
                    ScriptDrop = ScriptDrop + "DROP TABLE IF EXISTS " + this.TableEdit.FullTableNameToScript + ";";
                    if (ScriptDrop != "") ScriptDrop = ScriptDrop + "\n\n";
                }
            }

            if (TableOrig.SchemaNameToScript != TableEdit.SchemaNameToScript)
            {
                // изменилась схема, создаем все заново
                this.ScriptType = ScriptType.CREATE;
            }

            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    ScriptTable = "-------------------- MSSQL ------------------------";
                    break;
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    ScriptTable = "-------------------- PGSQL ------------------------";
                    break;
                default:
                    return "";
            }

            if ((this.ScriptType == ScriptType.ALTER) && (TableOrig.TableNameToScript != TableEdit.TableNameToScript) )
            {
                // изменилось имя таблицы, переименовываем
                if (ScriptTable != "") ScriptTable = ScriptTable + "\n";
                ScriptTable = ScriptTable + "-- Переименовать таблицу " + TableOrig.FullTableNameToScript;
                if (ScriptTable != "") ScriptTable = ScriptTable + "\n";
                ScriptTable = ScriptTable + RenameTableToScript();
            }

            if (this.ScriptType == ScriptType.CREATE)
            {
                if (ScriptTable != "") ScriptTable = ScriptTable + "\n";
                ScriptTable = ScriptTable + "-- Создать таблицу " + TableEdit.FullTableNameToScript;
            }
            else
            {
                if (ScriptTable != "") ScriptTable = ScriptTable + "\n";
                ScriptTable = ScriptTable + "-- Изменить таблицу " + TableEdit.FullTableNameToScript;
            }

            // перебираем поля обновленной таблицы
            foreach (RowDB row in this.TableEdit.ListField.OrderBy(x => x.FieldOrder).Where(x => ( (x.FieldName != "") && (x.FieldType != "") )))
            {

                RowDB oldRow = TableOrig.FindFieldById(row.FieldId);

                if (this.ScriptType == ScriptType.ALTER)
                {
                    if (oldRow != null)
                    {
                        if (TableOrig.AddFieldToScript(oldRow) != TableEdit.AddFieldToScript(row)) // что-то изменилось
                        {
                            if (oldRow.FieldNameToScript != row.FieldNameToScript)
                            {
                                // переименование поля
                                if (ScriptRow != "") ScriptRow = ScriptRow + "\n"; else ScriptRow = ScriptRow + "\n";
                                ScriptRow = ScriptRow + RenameFieldToScript(oldRow, row);
                            }

                            // изменение ревизитов поля
                            if (ScriptRow != "") ScriptRow = ScriptRow + "\n"; else ScriptRow = ScriptRow + "\n";
                            ScriptRow = ScriptRow + TableEdit.AlterFieldToScript(oldRow,row);
                        }
                    }
                    else
                    {
                        // добавление нового поля
                        if (ScriptRow != "") ScriptRow = ScriptRow + "\n"; else ScriptRow = ScriptRow + "\n";
                        ScriptRow = ScriptRow + TableEdit.AddFieldToScript(row);
                    }
                }
                else
                {
                    // список полей для создания таблицы
                    if (ScriptRow != "") ScriptRow = ScriptRow + ",\n"; else ScriptRow = ScriptRow + "\n";
                    ScriptRow = ScriptRow + "\t" + TableEdit.AddFieldToCreateScript(row);
                }

                if (row.FKTable != "")
                {
                    if (row.FKName == "") row.FKName = "fk_" + TableEdit.TableName + "_" + row.FieldName;
                    if (row.FKField == "") row.FKField = DelSchemaFromTableName(row.FKTable) + "_id";
                }

                if ((oldRow != null) && (this.ScriptType == ScriptType.ALTER))

                    {
                        if (TableOrig.AddFKToScript(oldRow) != TableEdit.AddFKToScript(row))
                    {
                        if (ScriptFK != "") ScriptFK = ScriptFK + "\n"; else ScriptFK = "\n\n";
                        ScriptFK = ScriptFK + TableOrig.DropFKToScript(oldRow);
                        if (TableEdit.AddFKToScript(row) != "")
                        {
                            if (ScriptFK != "") ScriptFK = ScriptFK + "\n"; else ScriptFK = "\n\n";
                            ScriptFK = ScriptFK + TableEdit.AddFKToScript(row);
                        }
                    }
                }
                else if (TableEdit.AddFKToScript(row) != "")
                {
                        if (ScriptFK != "") ScriptFK = ScriptFK + "\n"; else ScriptFK = "\n\n";
                        ScriptFK = ScriptFK + TableEdit.AddFKToScript(row);
                }

                if ((oldRow != null) && (this.ScriptType == ScriptType.ALTER))
                {
                    if (oldRow.FieldDesc != row.FieldDesc)
                    {
                        if (oldRow.FieldDesc != "")
                        {
                            if (ScriptFieldDesc != "") ScriptFieldDesc = ScriptFieldDesc + "\n"; else ScriptFieldDesc = "\n\n";
                            ScriptFieldDesc = ScriptFieldDesc + TableEdit.ChangeFieldDescToScript(row);
                        }
                        else
                        {
                            if (ScriptFieldDesc != "") ScriptFieldDesc = ScriptFieldDesc + "\n"; else ScriptFieldDesc = "\n\n";
                            ScriptFieldDesc = ScriptFieldDesc + TableEdit.AddFieldDescToScript(row);
                        }
                    }
                }
                else 
                { 
                    if (row.FieldDesc != "")
                    { 
                        if (ScriptFieldDesc != "") ScriptFieldDesc = ScriptFieldDesc + "\n"; else ScriptFieldDesc = "\n\n";
                        ScriptFieldDesc = ScriptFieldDesc + TableEdit.AddFieldDescToScript(row);
                    }
                }
            }

            // таблица и поля
            if (ScriptRow != "")
            {
                if (this.ScriptType == ScriptType.CREATE)
                {
                    if (ScriptTable != "") ScriptTable = ScriptTable + "\n";
                    ScriptTable = ScriptTable + TableEdit.CreateTableToScript(ScriptRow);
                }
                else
                {
                    if (ScriptTable != "") ScriptTable = ScriptTable + "\n";
                    ScriptTable = ScriptTable + ScriptRow;
                }
            }

            // перебираем поля оригинальной таблицы, ищем удаленные поля
            if (this.ScriptType == ScriptType.ALTER)
            foreach (RowDB oldRow in this.TableOrig.ListField.OrderBy(x => x.FieldOrder).Where(x => (x.FieldName != "")))
            {
                RowDB row = this.TableEdit.FindFieldById(oldRow.FieldId);

                if (row == null)
                {
                   /* // удаляем PK
                    if (oldRow.IsPK == true)
                    {
                        if (ScriptDropField != "") ScriptDropField = ScriptDropField + "\n"; else ScriptDropField = ScriptDropField + "\n\n";
                        ScriptDropField = ScriptDropField + this.TableOrig.DropPKToScript();
                    }*/

                    // удаляем констрайн
                    if (oldRow.FKTableToScript != "")
                    {
                        if (ScriptDropField != "") ScriptDropField = ScriptDropField + "\n"; else ScriptDropField = ScriptDropField + "\n\n";
                        ScriptDropField = ScriptDropField + this.TableOrig.DropFKToScript(oldRow);
                    }

                    // удаляем поле
                    if (ScriptDropField != "") ScriptDropField = ScriptDropField + "\n"; else ScriptDropField = ScriptDropField + "\n\n";
                    ScriptDropField = ScriptDropField + this.TableOrig.DropFieldToScript(oldRow);
                }
            }


            // Primary Key
            if ( (this.TableOrig.AddPKToScript() != this.TableEdit.AddPKToScript()) || (this.ScriptType == ScriptType.CREATE) )
            {
                if ((this.TableOrig.PKNameToScript != "") && (this.ScriptType == ScriptType.ALTER) )
                {
                    if (ScriptPK != "") ScriptPK = ScriptPK + "\n"; else ScriptPK = ScriptPK + "\n\n";
                    ScriptPK = ScriptPK + this.TableOrig.DropPKToScript();
                }
                if (this.TableEdit.AddPKToScript() != "")
                {
                    if (ScriptPK != "") ScriptPK = ScriptPK + "\n"; else ScriptPK = ScriptPK + "\n\n";
                    ScriptPK = ScriptPK + this.TableEdit.AddPKToScript();
                }
            }

            if ( (this.TableOrig.TableDesc != this.TableEdit.TableDesc) || (this.ScriptType == ScriptType.CREATE))
            {
                if ( (this.TableOrig.TableDesc != "") && (this.ScriptType == ScriptType.ALTER) )
                {
                    ScriptTableDesc = "\n\n" + this.TableEdit.ChangeTableDescToScript();
                }
                else
                {
                    ScriptTableDesc = "\n\n" + this.TableEdit.AddTableDescToScript();
                }
            }

            if (this.TableEdit.AddIdentToScript() != "")
            {
                ScriptIdent = "\n\n" + this.TableEdit.AddIdentToScript();
            }


            // Пересоздание хранимок
            if ((this.TargetDB == TargetDBType.MSSQL) || (this.TargetDB == TargetDBType.MSSQL_LIQUIBASE))
            {
                switch (this.TableType)
                {
                    case TableType.EVN:
                        ScriptProc = "\n\n--EXEC dbo.xp_genEvn_suball '" + this.TableEdit.FullTableNameToScript + "'";
                        break;
                    case TableType.PERSONEVN:
                        ScriptProc = "\n\n--EXEC dbo.xp_genPersonEvn_one '" + this.TableEdit.FullTableNameToScript + "'";
                        break;
                    case TableType.MORBUS:
                        ScriptProc = "\n\n--EXEC dbo.xp_genMorbus_one '" + this.TableEdit.FullTableNameToScript + "'";
                        break;
                    case TableType.DICT:
                    default:
                        ScriptProc = "\n\n--EXEC dbo.xp_genDict_one '" + this.TableEdit.FullTableNameToScript + "'";
                        break;
                }
            }

            // Пересоздание хранимок
            if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE))
            {
                switch (this.TableType)
                {
                    case TableType.EVN:
                        ScriptProc = "\n\nSELECT * FROM dbo.xp_genevn_one('" + this.TableEdit.FullTableNameToScript + "');";
                        if (this.TableEdit.AddGrantsToScript() != "") ScriptProc = ScriptProc + this.TableEdit.AddGrantsToScript();
                        break;
                    case TableType.PERSONEVN:
                        ScriptProc = "\n\nSELECT * FROM dbo.xp_genpersonevn_one('" + this.TableEdit.FullTableNameToScript + "');";
                        if (this.TableEdit.AddGrantsToScript() != "") ScriptProc = ScriptProc + this.TableEdit.AddGrantsToScript();
                        break;
                    case TableType.MORBUS:
                        ScriptProc = "\n\nSELECT * FROM dbo.xp_genmorbus_one('" + this.TableEdit.FullTableNameToScript + "');";
                        if (this.TableEdit.AddGrantsToScript() != "") ScriptProc = ScriptProc + this.TableEdit.AddGrantsToScript();
                        break;
                    case TableType.DICT:
                    default:
                        ScriptProc = "\n\nSELECT * FROM dbo.xp_gendict_one('" + this.TableEdit.FullTableNameToScript + "');";
                        if (this.TableEdit.AddGrantsToScript() != "") ScriptProc = ScriptProc + this.TableEdit.AddGrantsToScript();
                        break;
                }

            }

            return ScriptDrop + ScriptTable + ScriptPK + ScriptFK + ScriptTableDesc + ScriptFieldDesc + ScriptDropField + ScriptIdent + ScriptProc + "\n";

        }

    }


    // Версия таблицы
    public class TableInfo
    {

        // целевая БД
        TargetDBType _targetdb;
        public TargetDBType TargetDB
        {
            get
            {
                return this._targetdb;
            }
            set
            {
                this._targetdb = value;
                if (ListField != null) foreach (var row in ListField) row.TargetDB = this._targetdb;
                //if (ListIndex != null) foreach (var idx in ListIndex) idx.TargetDB = this._targetdb;
            }
        }

        public TableInfo(TargetDBType target)
        {
            this.TargetDB = target;
            this.ListField = new List<RowDB>();
        }


        // Имя схемы
        string _schema_name;

        public string SchemaName { 
            get {
                if ( (_schema_name == "") || (_schema_name == null) ) return "dbo"; 
                else return _schema_name.Trim(); 
            } 
            set { 
                _schema_name = value.Trim(); 
            } }

        public string SchemaNameToScript
        {
            get
            {
                switch (this.TargetDB)
                {
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                        return this.SchemaName;
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        return this.SchemaName.ToLower();
                    default:
                        return "";
                }
            }
        }

        // Имя таблицы
        string _table_name;

        public string TableName { 
            get {
                if (_table_name == null) return "";
                else return _table_name.Trim();
            } 
            set { 
                _table_name = value.Trim(); 
            } }

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

        public string FullTableNameToScript { get { return SchemaNameToScript + "." + TableNameToScript; } }
        public string FullViewNameToScript { get { return SchemaNameToScript + ".v_" + TableNameToScript; } }
        public string FullProcINSToScript { get { return SchemaNameToScript + ".p_" + TableNameToScript + "_ins"; } }
        public string FullProcUPDToScript { get { return SchemaNameToScript + ".p_" + TableNameToScript + "_upd"; } }
        public string FullProcDELToScript { get { return SchemaNameToScript + ".p_" + TableNameToScript + "_del"; } }


        // описание таблицы
        string _table_desc;

        public string TableDesc {
            get {
                if (_table_desc == null) return "";
                else return _table_desc.Trim();
            }
            set { 
                _table_desc = value.Trim(); 
            } }

        // название PrimaryKey
        string _pkname;

        public string PKName { 
            get {
                if (_pkname == null) return "";
                else return _pkname.Trim();
            }
            set {
                _pkname = value.Trim(); 
            } }

        public string PKNameToScript { 
            get {
                switch (this.TargetDB)
                {
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                        return this.PKName;
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        return this.PKName.ToLower();
                    default:
                        return this.PKName;
                }
            }
        }

        // список полей
        public List<RowDB> ListField { get; set; }

        // список индексов
        //public List<IndexDB> ListIndex { get; set; }

        // скрипт на переименование таблицы
        public void RenameTable (string newName)
        {
            string oldName = this.TableName.Trim();
            newName = newName.Trim();
            this.TableName = newName;
            string find_s = "";

            int pos = -1;
            int len = 0;

            find_s = "_" + oldName.ToLower() + "_";
            len = find_s.Length;
            pos = this.PKName.ToLower().IndexOf(find_s);
            if (pos>-1) this.PKName = this.PKName.Remove(pos, len).Insert(pos, "_" + newName + "_");

            foreach (var row in this.ListField)
            {
                find_s = oldName.ToLower() + "_";
                len = find_s.Length;
                if ( row.FieldName.ToLower().StartsWith(find_s) )
                    row.FieldName = row.FieldName.Remove(0, len).Insert(0, newName + "_");

                find_s = "_" + oldName.ToLower() + "_";
                len = find_s.Length;
                pos = row.FKName.ToLower().IndexOf(find_s);
                if (pos > -1) row.FKName = row.FKName.Remove(pos, len).Insert(pos, "_" + newName + "_");
                pos = row.FKName.ToLower().IndexOf(find_s);
                if (pos > -1) row.FKName = row.FKName.Remove(pos, len).Insert(pos, "_" + newName + "_");
            }

        }

        // Найти поле по имени
        public RowDB FindFieldByName (string name)
        {
            return this.ListField.Find(x => x.FieldName.ToLower() == name.ToLower());
        }

        // Найти поле по Id
        public RowDB FindFieldById(int Id)
        {
            return this.ListField.Find(x => x.FieldId == Id);
        }

        // Добавить поле в список
        public void AddField(string FieldOrder, string FieldName, string FieldType, string FieldSize="", string FieldDec="", string FieldDesc="", 
                                string IsNotNull="false", string IsIdentity="false", string IsPK="false", string FieldDefault="", string FKName="", string FKTable="", string FKField="")
        {
            RowDB newField = new RowDB(this.TargetDB);

            if (FieldOrder == "") FieldOrder = "0";
            if (IsNotNull == "") IsNotNull = "false";
            if (IsIdentity == "") IsIdentity = "false";
            if (IsPK == "") IsPK = "false";

            newField.FieldOrder = int.Parse(FieldOrder);
            newField.FieldId = int.Parse(FieldOrder);
            newField.FieldName = FieldName;
            newField.FieldDesc = FieldDesc;
            newField.FieldType = FieldType.ToUpper();
            newField.FieldSize = FieldSize;
            newField.FieldDec = FieldDec;
            newField.IsNotNull = bool.Parse(IsNotNull);
            newField.IsIdentity = bool.Parse(IsIdentity);
            newField.IsPK = bool.Parse(IsPK);
            newField.FieldDefault = FieldDefault;
            newField.FKName = FKName;
            newField.FKTable = FKTable;
            newField.FKField = FKField;

            RowDB existField = FindFieldByName(newField.FieldName);

            if (existField == null)
            {
                if (newField.FieldOrder == 0)
                {
                    int order = 0;
                    foreach (RowDB row in this.ListField) if (order < row.FieldOrder) order = row.FieldOrder;
                    newField.FieldOrder = order + 1;
                }
                this.ListField.Add(newField);
            }
            else
            {
                if (existField.FieldDesc == "") existField.FieldDesc = newField.FieldDesc;
                if (existField.FieldType == "") existField.FieldType = newField.FieldType;
                if (existField.FieldSize == "") existField.FieldSize = newField.FieldSize;
                if (existField.FieldDec == "") existField.FieldDec = newField.FieldDec;
                if (existField.IsNotNull == false) existField.IsNotNull = newField.IsNotNull;
                if (existField.IsIdentity == false) existField.IsIdentity = newField.IsIdentity;
                if (existField.IsPK == false) existField.IsPK = newField.IsPK;
                if (existField.FieldDefault == "") existField.FieldDefault = newField.FieldDefault;
                if (existField.FKName == "") existField.FKName = newField.FKName;
                if (existField.FKTable == "") existField.FKTable = newField.FKTable;
                if (existField.FKField == "") existField.FKField = newField.FKField;
            }
        }






        // Скрипт создания таблицы
        public string CreateTableToScript(string fields)
        {
            if (fields == "") return "";

            string res = "CREATE TABLE " + this.FullTableNameToScript + " (" + fields + "\n)";

            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        return res + " ON [PRIMARY]\nGO";
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        res = res + " WITH (oids = false);"; 
                        /*+"\n\nALTER TABLE " + this.FullTableNameToScript + " OWNER TO developer;" +
                        "\nGRANT SELECT, INSERT, UPDATE, DELETE, REFERENCES, TRIGGER, TRUNCATE ON " + this.FullTableNameToScript + " TO developer;" +
                        "\nGRANT SELECT, INSERT, UPDATE, DELETE, REFERENCES, TRIGGER, TRUNCATE ON " + this.FullTableNameToScript + " TO developer_rep;";*/
                        return res;
                    }
                default:
                    return res + ";";
            }
        }


        // Скрипт изменеения поля в таблице (таблица уже существует)
        public string AlterFieldToScript(RowDB oldrow, RowDB newrow)
        {
            if (oldrow == null) return "";
            if (newrow == null) return "";

            string ScriptRow = "";

            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:

                    ScriptRow = ScriptRow + "ALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + newrow.FieldNameToScript;

                    if ((oldrow.FullFieldTypeToScript != newrow.FullFieldTypeToScript) || (oldrow.IsNotNull != newrow.IsNotNull))
                    {
                        ScriptRow = ScriptRow + " " + newrow.FullFieldTypeToScript;
                        if (newrow.IsNotNull == true)
                            ScriptRow = ScriptRow + " " + newrow.IsNotNullToScript;
                        else
                            ScriptRow = ScriptRow + " " + newrow.IsNullToScript;
                    }

                    if (oldrow.IsIdentity != newrow.IsIdentity) 
                        if (newrow.IsIdentity == true) 
                            ScriptRow = ScriptRow + " " + newrow.IsIdentityToScript;

                    if (oldrow.FieldDefault != newrow.FieldDefault)
                        ScriptRow = ScriptRow + " " + newrow.FieldDefaultToScript;

                    ScriptRow = ScriptRow + "\nGO";
                    break;
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    if (oldrow.FullFieldTypeToScript != newrow.FullFieldTypeToScript)
                        ScriptRow = ScriptRow + "ALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + newrow.FieldNameToScript + " TYPE " + newrow.FullFieldTypeToScript + ";";

                    if (oldrow.IsIdentity != newrow.IsIdentity)
                        if (newrow.IsIdentity == true) 
                            ScriptRow = ScriptRow + "\nALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + newrow.FieldNameToScript+ " ADD "+newrow.IsIdentityToScript+";";
                        else
                            ScriptRow = ScriptRow + "\nALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + newrow.FieldNameToScript + " DROP IDENTITY IF EXISTS;";

                    if (oldrow.IsNotNull != newrow.IsNotNull)
                        if (newrow.IsNotNull == true) 
                            ScriptRow = ScriptRow + "\nALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + newrow.FieldNameToScript + " SET NOT NULL;";
                        else
                            ScriptRow = ScriptRow + "\nALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + newrow.FieldNameToScript + " DROP NOT NULL;";

                    if (oldrow.FieldDefault != newrow.FieldDefault)
                        if (newrow.FieldDefault != "")
                            ScriptRow = ScriptRow + "\nALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + newrow.FieldNameToScript + " SET "+newrow.FieldDefaultToScript+";";
                        else
                            ScriptRow = ScriptRow + "\nALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + newrow.FieldNameToScript + " DROP " + newrow.FieldDefaultToScript + ";";
                    break;
                default:
                    break;
            }
            return ScriptRow;
        }


        // Скрипт добавления поля в таблицу (таблица уже существует)
        public string AddFieldToScript(RowDB row)
        {
            if (row == null) return "";
            
            string ScriptRow = "";

            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    ScriptRow = "ALTER TABLE " + this.FullTableNameToScript + " ADD " + row.FieldNameToScript + " " + row.FullFieldTypeToScript;
                    if (row.IsIdentity == true) ScriptRow = ScriptRow + " " + row.IsIdentityToScript;
                    if (row.IsNotNull == true) ScriptRow = ScriptRow + " " + row.IsNotNullToScript;
                    if (row.FieldDefault != "") ScriptRow = ScriptRow + " " + row.FieldDefaultToScript;
                    ScriptRow = ScriptRow + "\nGO";
                    break;
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    ScriptRow = "ALTER TABLE " + this.FullTableNameToScript + " ADD COLUMN " + row.FieldNameToScript + " " + row.FullFieldTypeToScript + ";";
                    if (row.IsIdentity == true) ScriptRow = ScriptRow + " " + row.IsIdentityToScript;
                    if (row.IsNotNull == true) ScriptRow = ScriptRow + " " + row.IsNotNullToScript;
                    if (row.FieldDefault != "") ScriptRow = ScriptRow + " " + row.FieldDefaultToScript;
                    ScriptRow = ScriptRow + ";";
                    break;
                default:
                    break;
            }
            return ScriptRow;
        }



        // Скрипт добавления поля в скрипт создания таблицы
        public string AddFieldToCreateScript (RowDB row)
        {
            if (row == null) return "";
            
            string ScriptRow = row.FieldNameToScript + " " + row.FullFieldTypeToScript;

            if (row.IsIdentity == true) ScriptRow = ScriptRow + " " + row.IsIdentityToScript;
            if (row.IsNotNull == true) ScriptRow = ScriptRow + " " + row.IsNotNullToScript;
            if (row.FieldDefault != "") ScriptRow = ScriptRow + " " + row.FieldDefaultToScript;
            return ScriptRow;
        }


        // Скрипт добавления генерации сиквенса
        public string AddIdentToScript()
        {
            string fields = "";
            string drop = "";

            foreach (RowDB row in this.ListField.OrderBy(x => x.FieldOrder).Where(x => (x.IsIdentity == true) && (x.FieldName != "") && (x.FieldType != "")))
            {
                if (fields != "") fields = fields + ", ";
                fields = fields + row.FieldNameToScript;

                /*if (drop != "") drop = drop + "\n";
                drop = drop + "ALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + row.FieldNameToScript + " DROP IDENTITY IF EXISTS;";*/
            }

            if (fields == "") return "";

            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        return "";
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        return "\n\n" + drop + "\nSELECT * FROM dbo.xp_GenIdentity('" + this.FullTableNameToScript + "', '" + fields + "');" +
                            "\nALTER SEQUENCE " + this.FullTableNameToScript + "_" + fields + "_seq OWNER TO developer;" +
                            "\nGRANT SELECT, UPDATE, USAGE ON SEQUENCE " + this.FullTableNameToScript + "_" + fields + "_seq TO developer;" +
                            "\nGRANT SELECT, UPDATE, USAGE ON SEQUENCE " + this.FullTableNameToScript + "_" + fields + "_seq TO developer_rep;";

//                        "\n --ALTER TABLE " + this.FullTableNameToScript + " ALTER COLUMN " + fields + " ADD GENERATED BY DEFAULT AS IDENTITY;" +

                    }
                default:
                    return "";
            }
        }



        // Скрипт добавления разрешений
        public string AddGrantsToScript()
        {

            if (this.TargetDB != TargetDBType.PGSQL) return "";

            string grants = "";

            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "ALTER TABLE " + this.FullTableNameToScript + " OWNER TO developer;";
            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "GRANT SELECT, INSERT, UPDATE, DELETE, REFERENCES, TRIGGER, TRUNCATE ON " + this.FullTableNameToScript + " TO developer;";
            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "GRANT SELECT, INSERT, UPDATE, DELETE, REFERENCES, TRIGGER, TRUNCATE ON " + this.FullTableNameToScript + " TO developer_rep;";

            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "ALTER VIEW " + this.FullViewNameToScript + " OWNER TO developer;";
            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "GRANT SELECT, INSERT, UPDATE, DELETE, REFERENCES, TRIGGER, TRUNCATE ON " + this.FullViewNameToScript + " TO developer;";
            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "GRANT SELECT, INSERT, UPDATE, DELETE, REFERENCES, TRIGGER, TRUNCATE ON " + this.FullViewNameToScript + " TO developer_rep;";

            Boolean IsDel = false;
            string fields = "";

            foreach (RowDB row in this.ListField.OrderBy(x => x.FieldOrder).Where(x => (x.FieldName != "") && (x.FieldType != "") ))
            {
                if ( !IsDel) IsDel = row.FieldName.ToLower().Contains("_deleted");

                if ((row.FieldName.ToLower() != "pmuser_insid") && (row.FieldName.ToLower() != "pmuser_updid") && (row.FieldName.ToLower() != "pmuser_delid")
                && (row.FieldName.ToLower() != (this.TableName.ToLower() + "_insdt")) && (row.FieldName.ToLower() != (this.TableName.ToLower() + "_upddt"))
                && (row.FieldName.ToLower() != (this.TableName.ToLower() + "_deldt")) && (row.FieldName.ToLower() != (this.TableName.ToLower() + "_deleted"))
                && (row.FieldName.ToLower() != (this.TableName.ToLower() + "_rowersion")))
                {
                    if (fields != "") fields = fields + ", ";
                    fields = fields + row.FieldTypeToScript;
                }
            }

            if (IsDel)
            {
                if (grants != "") grants = grants + "\n"; else grants = "\n\n";
                grants = grants + "ALTER FUNCTION " + this.FullProcDELToScript + "(BIGINT, BIGINT, BIGINT, VARCHAR, VARCHAR) OWNER TO developer;";
                if (grants != "") grants = grants + "\n"; else grants = "\n\n";
                grants = grants + "GRANT EXECUTE ON FUNCTION " + this.FullProcDELToScript + "(BIGINT, BIGINT, BIGINT, VARCHAR, VARCHAR) TO developer;";
                if (grants != "") grants = grants + "\n"; else grants = "\n\n";
                grants = grants + "GRANT EXECUTE ON FUNCTION " + this.FullProcDELToScript + "(BIGINT, BIGINT, BIGINT, VARCHAR, VARCHAR) TO developer_rep;";
            }
            else
            {
                if (grants != "") grants = grants + "\n"; else grants = "\n\n";
                grants = grants + "ALTER FUNCTION " + this.FullProcDELToScript + "(BIGINT, VARCHAR, VARCHAR) OWNER TO developer;";
                if (grants != "") grants = grants + "\n"; else grants = "\n\n";
                grants = grants + "GRANT EXECUTE ON FUNCTION " + this.FullProcDELToScript + "(BIGINT, VARCHAR, VARCHAR) TO developer;";
                if (grants != "") grants = grants + "\n"; else grants = "\n\n";
                grants = grants + "GRANT EXECUTE ON FUNCTION " + this.FullProcDELToScript + "(BIGINT, VARCHAR, VARCHAR) TO developer_rep;";
            }

            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "ALTER FUNCTION " + this.FullProcINSToScript + "(" + fields + ", BIGINT, VARCHAR, VARCHAR) OWNER TO developer;";
            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "GRANT EXECUTE ON FUNCTION " + this.FullProcINSToScript + "(" + fields + ", BIGINT, VARCHAR, VARCHAR) TO developer;";
            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "GRANT EXECUTE ON FUNCTION " + this.FullProcINSToScript + "(" + fields + ", BIGINT, VARCHAR, VARCHAR) TO developer_rep;";

            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "ALTER FUNCTION " + this.FullProcUPDToScript + "(" + fields + ", BIGINT, VARCHAR, VARCHAR) OWNER TO developer;";
            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "GRANT EXECUTE ON FUNCTION " + this.FullProcUPDToScript + "(" + fields + ", BIGINT, VARCHAR, VARCHAR) TO developer;";
            if (grants != "") grants = grants + "\n"; else grants = "\n\n";
            grants = grants + "GRANT EXECUTE ON FUNCTION " + this.FullProcUPDToScript + "(" + fields + ", BIGINT, VARCHAR, VARCHAR) TO developer_rep;";

            return grants;
        }


        // скрипт удаления поля
        public string DropFieldToScript(RowDB row)
        {
            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        return "ALTER TABLE " + this.FullTableNameToScript + " DROP COLUMN " + row.FieldNameToScript + "\nGO";
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        return "ALTER TABLE " + this.FullTableNameToScript + " DROP COLUMN IF EXISTS " + row.FieldNameToScript + ";";
                    }
                default:
                    return "";
            }
        }



        // Скрипт добавления foreign key
        public string AddFKToScript (RowDB row)
        {
            if ((row.FKNameToScript == "") || (row.FKTableToScript == "") || (row.FKFieldToScript == "")) return "";

            string FK = "ALTER TABLE " + this.FullTableNameToScript + " ADD CONSTRAINT " + row.FKNameToScript + " FOREIGN KEY(" + row.FieldNameToScript + ") " +
                "\nREFERENCES " + row.FKTableToScript + " (" + row.FKFieldToScript + ")";

            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        FK = FK + "\nGO";
                        FK = FK + "\nALTER TABLE " + this.FullTableNameToScript + " CHECK CONSTRAINT " + row.FKNameToScript;
                        FK = FK + "\nGO";

                        if (row.FieldName.ToLower() == "region_id")
                        {
                            FK = FK + "\n\nEXEC sp_addextendedproperty N'SWAN_RegionalTable', N'Региональный справочник', 'SCHEMA', N'" + this.SchemaNameToScript +
                                "', 'TABLE', N'" + this.TableNameToScript + "', NULL, NULL; ";
                            FK = FK + "\nGO";
                        }
                        return FK;
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        FK = FK + "\nON DELETE NO ACTION ON UPDATE NO ACTION NOT DEFERRABLE;";
                        return FK;
                    }
                default:
                    return FK+";";
            }
        }

        // Скрипт удаления foreign key
        public string DropFKToScript(RowDB row)
        {
            if (row.FKNameToScript == "") return "";

            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        return "ALTER TABLE " + this.FullTableNameToScript + " DROP CONSTRAINT " + row.FKNameToScript + "\nGO";
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        return "ALTER TABLE " + this.FullTableNameToScript + " DROP CONSTRAINT IF EXISTS " + row.FKNameToScript + ";";
                    }
                default:
                    return "";
            }
        }

        // скрипт добавления primary key
        public string AddPKToScript()
        {
            if (this.PKNameToScript == "") this.PKName = "pk_" + this.TableName + "_id";

            string cons = "ALTER TABLE " + this.FullTableNameToScript + " ADD CONSTRAINT " + this.PKNameToScript;
            string pk = "";

            foreach (RowDB row in this.ListField.OrderBy(x => x.FieldOrder).Where(x => (x.IsPK==true) && (x.FieldName != "") && (x.FieldType != ""))) 
            {
                if (pk != "") pk = pk + ", ";
                pk = pk + row.FieldNameToScript;
            }

            if (pk == "") return "";

            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        cons = cons + " PRIMARY KEY CLUSTERED (" + pk + ") WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 75) ON [PRIMARY]";
                        cons = cons + "\nGO";
                        return cons;
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        cons = cons + " PRIMARY KEY (" + pk + ");";
                        return cons;
                    }
                default:
                    return "";
            }
        }

        // скрипт удаления primary key
        public string DropPKToScript()
        {
            if (this.PKNameToScript == "") return "";

            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        return "ALTER TABLE " + this.FullTableNameToScript + " DROP CONSTRAINT " + this.PKNameToScript + "\nGO";
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        return "ALTER TABLE " + this.FullTableNameToScript + " DROP CONSTRAINT IF EXISTS " + this.PKNameToScript;
                    }
                default:
                    return "";
            }
        }


        // скрипт изменения описания таблицы
        public string ChangeTableDescToScript()
        {
            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        string desc = @"IF NOT EXISTS(SELECT top(1) 1 FROM sys.fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'" + this.SchemaNameToScript + @"', 'TABLE', N'" +
                            this.TableNameToScript + @"',NULL,NULL) )
BEGIN
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + this.TableDesc + @"' , @level0type=N'SCHEMA',@level0name=N'" + this.SchemaNameToScript + @"', @level1type=N'TABLE',@level1name=N'" + this.TableNameToScript + @"'
END
ELSE
BEGIN
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'" + this.TableDesc + @"' , @level0type=N'SCHEMA',@level0name=N'" + this.SchemaNameToScript + @"', @level1type=N'TABLE',@level1name=N'" + this.TableNameToScript + @"'
END
GO";
                        return desc;
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        string desc = "COMMENT ON TABLE " + this.FullTableNameToScript + " IS '" + this.TableDesc + "';";
                        return desc;
                    }
                default:
                    return "";
            }
        }

        // скрипт добавления описания поля
        public string AddTableDescToScript()
        {
            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        string desc = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + this.TableDesc +
                                                "' , @level0type=N'SCHEMA',@level0name=N'" + this.SchemaNameToScript +
                                                "', @level1type=N'TABLE',@level1name=N'" + this.TableNameToScript + "';";
                        desc = desc + "\nGO";
                        return desc;
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        string desc = "COMMENT ON TABLE " + this.FullTableNameToScript + " IS '" + this.TableDesc + "';";
                        return desc;
                    }
                default:
                    return "";
            }
        }


        // скрипт изменения описания поля
        public string ChangeFieldDescToScript(RowDB row)
        {
            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        string desc = @"IF NOT EXISTS(SELECT top(1) 1 FROM sys.fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'" + this.SchemaNameToScript + @"', 'TABLE', N'" + 
                            this.TableNameToScript + @"', 'COLUMN', '" + row.FieldNameToScript + @"') )
BEGIN
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + row.FieldDesc + @"' , @level0type=N'SCHEMA',@level0name=N'" + this.SchemaNameToScript + @"', @level1type=N'TABLE',@level1name=N'" + this.TableNameToScript + @"', @level2type=N'COLUMN',@level2name=N'" + row.FieldNameToScript + @"'
END
ELSE
BEGIN
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'" + row.FieldDesc + @"' , @level0type=N'SCHEMA',@level0name=N'" + this.SchemaNameToScript + @"', @level1type=N'TABLE',@level1name=N'" + this.TableNameToScript + @"', @level2type=N'COLUMN',@level2name=N'" + row.FieldNameToScript + @"'
END
GO";
                        return desc;
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        string desc = "COMMENT ON COLUMN " + this.FullTableNameToScript + "." + row.FieldNameToScript + " IS '" + row.FieldDesc + "';";
                        return desc;
                    }
                default:
                    return "";
            }
        }

        // скрипт добавления описания поля
        public string AddFieldDescToScript(RowDB row)
        {
            switch (this.TargetDB)
            {
                case TargetDBType.MSSQL:
                case TargetDBType.MSSQL_LIQUIBASE:
                    {
                        string desc = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + row.FieldDesc +
                                                "' , @level0type=N'SCHEMA',@level0name=N'" + this.SchemaNameToScript +
                                                "', @level1type=N'TABLE',@level1name=N'" + this.TableNameToScript + "', @level2type=N'COLUMN',@level2name=N'" + row.FieldNameToScript + "';";
                        desc = desc + "\nGO";
                        return desc;
                    }
                case TargetDBType.PGSQL:
                case TargetDBType.PGSQL_LIQUIBASE:
                    {
                        string desc = "COMMENT ON COLUMN " + this.FullTableNameToScript + "." + row.FieldNameToScript + " IS '" + row.FieldDesc + "';";
                        return desc;
                    }
                default:
                   return "";
            }
        }

    }






    public class RowDB : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // целевая БД
        public TargetDBType TargetDB;

        public RowDB(TargetDBType target)
        {
            this.TargetDB = target;
        }

        public RowDB(RowDB field)
        {
            this.TargetDB = field.TargetDB;
            this.FieldOrder = field.FieldOrder;
            this.FieldId = field.FieldId;
            this.FieldName = field.FieldName;
            this.FieldDesc = field.FieldDesc;
            this.FieldType = field.FieldType;
            this.FieldSize = field.FieldSize;
            this.FieldDec = field.FieldDec;
            this.IsNotNull = field.IsNotNull;
            this.IsIdentity = field.IsIdentity;
            this.IsPK = field.IsPK;
            this.FieldDefault = field.FieldDefault;
            this.FKName = field.FKName;
            this.FKTable = field.FKTable;
            this.FKField = field.FKField;
        }


        public int FieldOrder { get; set; }  // Порядок
        public int FieldId { get; set; }  // ключ для связки полей

        // Имя поля
        string _field_name;
        public string FieldName
        {
            get
            {
                if (_field_name == null) return "";
                else return _field_name.Trim();
            }
            set
            {
                _field_name = value;
            }
        }

        public string FieldNameToScript
        {
            get
            {
                switch (this.TargetDB)
                {
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                        return this.FieldName;
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        return this.FieldName.ToLower();
                    default:
                        return this.FieldName;
                }
            }
        }

        // Описание поля
        string _field_desc;
        public string FieldDesc
        {
            get
            {
                if (_field_desc == null) return "";
                else return _field_desc.Trim();
            }
            set
            {
                _field_desc = value;
            }
        }


        // Тип поля
        string _field_type;
        public string FieldType
        {
            get
            {
                if (_field_type == null) return "";
                else return _field_type.Trim().ToUpper();
            }
            set
            {
                _field_type = value;
            }
        }

        // Размер поля
        string _field_size;
        public string FieldSize
        {
            get
            {
                if (_field_size == null) return "";
                else return _field_size.Trim().ToLower();
            }
            set
            {
                _field_size = value;
            }
        }

        // Кол-во знаков после запятой
        string _field_dec;
        public string FieldDec
        {
            get
            {
                if (_field_dec == null) return "";
                else return _field_dec.Trim().ToLower();
            }
            set
            {
                _field_dec = value;
            }
        }

        // тип поля для скрипта
        public string FieldTypeToScript
        {
            get
            {
                switch (this.FieldType)
                {
                    case "DOUBLE PRECISION":
                        if ( (this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE) ) return this.FieldType;
                        else return "FLOAT";

                    case "MONEY":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "DOUBLE PRECISION";
                        else return this.FieldType;

                    case "FLOAT":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "DOUBLE PRECISION";
                        else return this.FieldType;

                    case "TIMESTAMP WITHOUT TIME ZONE":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return this.FieldType;
                        else return "DATETIME";

                    case "DATETIME":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "TIMESTAMP WITHOUT TIME ZONE";
                        else return this.FieldType;

                    case "TIME WITHOUT TIME ZONE":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return this.FieldType;
                        else return "TIME";

                    case "TIME":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "TIME WITHOUT TIME ZONE";
                        else return this.FieldType;

                    case "INTEGER":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return this.FieldType;
                        else return "INT";

                    case "INT":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "INTEGER";
                        else return this.FieldType;

                    case "BYTEA":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return this.FieldType;
                        else return "?????";

                    case "TIMESTAMP":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "BYTEA";
                        else return this.FieldType;

                    case "UUID":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return this.FieldType;
                        else return "UNIQUEIDENTIFIER";

                    case "UNIQUEIDENTIFIER":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "UUID";
                        else return this.FieldType;

                    case "BOOLEAN":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return this.FieldType;
                        else return "BIT";

                    case "TINYINT":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "SMALLINT";
                        else return this.FieldType;

                    case "TEXT":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "VARCHAR";
                        else return this.FieldType;

                    case "NCHAR":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "CHAR";
                        else return this.FieldType;
                    case "NVARCHAR":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "VARCHAR";
                        else return this.FieldType;

                    case "BIT VARYING":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return this.FieldType;
                        else return "?????";

                    case "VARBINARY":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "BYTEA";
                        else return this.FieldType;

                    case "IMAGE":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "BYTEA";
                        else return this.FieldType;

                    case "DATETIME2":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "?????";
                        else return this.FieldType;

                    case "DECIMAL":
                    case "NUMERIC":
                        if ((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) return "NUMERIC";
                        else return this.FieldType;

                    default:
                        return this.FieldType;
                }

            }
        }


        // тип поля для скрипта
        public string FullFieldTypeToScript
        {
            get
            {
                switch (this.FieldTypeToScript)
                {
                    case "BIT":
                        if (((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) && (this.FieldSize != "") ) return this.FieldTypeToScript + "(" + this.FieldSize + ")";
                        else return this.FieldTypeToScript;

                    case "FLOAT":
                    case "CHAR":
                    case "VARCHAR":
                    case "NCHAR":
                    case "NVARCHAR":
                    case "VARBINARY":
                    case "DATETIME2":
                        {
                            if (((this.TargetDB == TargetDBType.PGSQL) || (this.TargetDB == TargetDBType.PGSQL_LIQUIBASE)) && (this.FieldSize == "max")) return this.FieldTypeToScript;
                            if (this.FieldSize != "") return this.FieldTypeToScript + "(" + this.FieldSize + ")";
                            else return this.FieldTypeToScript;
                        }

                    case "DECIMAL":
                    case "NUMERIC":
                        {
                            if ((this.FieldSize != "") && (this.FieldDec != "")) return this.FieldTypeToScript + "(" + this.FieldSize + ", " + this.FieldDec + ")";
                            if (this.FieldSize != "") return this.FieldTypeToScript + "(" + this.FieldSize + ")";
                            return this.FieldTypeToScript;
                        }

                    default:
                        return this.FieldTypeToScript;
                }

            }
        }

        // глобальный тип (строка, число, логическое)
        public GeneralType FieldGeneralType
        {
            get
            {
                switch (this.FieldType)
                {
                    case "DOUBLE PRECISION":
                    case "FLOAT":
                    case "MONEY":
                    case "INTEGER":
                    case "INT":
                    case "BIT":
                    case "TINYINT":
                    case "SMALLINT":
                    case "DECIMAL":
                    case "NUMERIC":
                        return GeneralType.NUMBER;


                    case "TIMESTAMP WITHOUT TIME ZONE":
                    case "DATETIME":
                    case "TIME WITHOUT TIME ZONE":
                    case "TIME":
                        return GeneralType.DATETIME;

                    case "CHAR":
                    case "VARCHAR":
                    case "TEXT":
                    case "NCHAR":
                    case "NVARCHAR":
                    case "BIT VARYING":
                        return GeneralType.STRING;

                    case "BOOLEAN":
                        return GeneralType.BOOLEAN;


                    default:
                        return GeneralType.UNKNOWN;
                }

            }
        }


        // NOT NULL
        public bool IsNotNull { get; set; } 

        public string IsNotNullToScript
        {
            get
            {
                if (this.IsNotNull == true) return "NOT NULL";
                else return "";
            }
        }

        public string IsNullToScript
        {
            get
            {
                if (this.IsNotNull == false) return "NULL";
                else return "";
            }
        }

        // identity
        public bool IsIdentity { get; set; } 
        public string IsIdentityToScript
        {
            get
            {
                switch (this.TargetDB)
                {
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                        if (this.IsIdentity == true) return "IDENTITY(1,1)";
                        else return "";
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        if (this.IsIdentity == true) return "GENERATED BY DEFAULT AS IDENTITY";
                        else return "";
                    default:
                        return "";
                }
            }
        }

        // primary key
        public bool IsPK { get; set; }
        public string IsPKToScript
        {
            get
            {
                switch (this.TargetDB)
                {
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                        return this.FieldNameToScript + " ASC";
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        return this.FieldNameToScript;
                    default:
                        return this.FieldNameToScript;
                }
            }
        }

        // значение по умолчанию
        string _field_default;
        public string FieldDefault
        {
            get
            {
                if (_field_default == null) return "";
                else return _field_default.Trim();
            }
            set
            {
                _field_default = value;
            }
        }
        public string FieldDefaultToScript
        {
            get
            {
                switch (FieldGeneralType)
                {
                    case GeneralType.STRING:
                    case GeneralType.DATETIME:
                    case GeneralType.BOOLEAN:
                        return "DEFAULT '" + this.FieldDefault + "'";
                    case GeneralType.NUMBER:
                        return "DEFAULT " + this.FieldDefault;
                    default:
                        return "";
                }
            }
        }

        // Наименование внешнего ключа
        string _fkname;
        public string FKName
        {
            get
            {
                if (_fkname == null) return "";
                else return _fkname.Trim();
            }
            set
            {
                _fkname = value;
            }
        }

        public string FKNameToScript
        {
            get
            {
                switch (this.TargetDB)
                {
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                        return this.FKName;
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        return this.FKName.ToLower();
                    default:
                        return this.FKName;
                }
            }
        }

        // Таблица внешнего ключа
        string _fktable;
        public string FKTable
        {
            get
            {
                if (_fktable == null) return "";
                else return _fktable.Trim();
            }
            set
            {
                _fktable = value; NotifyPropertyChanged(); NotifyPropertyChanged("FieldDesc"); NotifyPropertyChanged("FKName"); NotifyPropertyChanged("FKField");
            }
        }

        public string FKTableToScript
        {
            get
            {
                switch (this.TargetDB)
                {
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                        return this.FKTable;
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        return this.FKTable.ToLower();
                    default:
                        return this.FKTable;
                }
            }
        }


        // PK таблицы внешнего ключа
        string _fkfield;
        public string FKField
        {
            get
            {
                if (_fkfield == null) return "";
                else return _fkfield.Trim();
            }
            set
            {
                _fkfield = value;
            }
        }
        public string FKFieldToScript
        {
            get
            {
                switch (this.TargetDB)
                {
                    case TargetDBType.MSSQL:
                    case TargetDBType.MSSQL_LIQUIBASE:
                        return this.FKField;
                    case TargetDBType.PGSQL:
                    case TargetDBType.PGSQL_LIQUIBASE:
                        return this.FKField.ToLower();
                    default:
                        return this.FKField;
                }
            }
        }


    }


}



