using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SQLGen
{

    public partial class MainWindow : Window
    {

        public TableDB Table = new TableDB();

        public List<string> ListTypes = new List<string> {
            "BIGINT",
            "INT",
            "INTEGER",
            "BIT",
            "BOOLEAN",
            "SMALLINT",
            "CHAR",
            "VARCHAR",
            "DATETIME",
            "TIMESTAMP WITHOUT TIME ZONE",
            "DATE",
            "TIME",
            "TIME WITHOUT TIME ZONE",
            "NUMERIC",
            "FLOAT",
            "DOUBLE PRECISION",
            "MONEY",
            "TIMESTAMP",
            "UNIQUEIDENTIFIER",
            "UUID",
            "XML",
            "VARBINARY",
            "JSONB",
            "BYTEA",
        };


        public void SetTable(TableDB _table)
        {

            if (Table == null) Table = new TableDB();

            // по умолчанию
            tbSchemaName.Text = "";
            tbTableName.Text = "";
            tbPKName.Text = "";
            tbTableDesc.Text = "";
            cbScriptCreateDB.SelectedIndex = 0;
            cbTableType.SelectedIndex = 0;
            isAddDrop.IsChecked = false;
            cbScriptCreateType.SelectedIndex = 0;
            tbScriptCreate.Text = "";
            //tabAlter.Header = Table.ScriptFilename;

            Table.TableOrig.ListField.Clear();
            Table.TableEdit.ListField.Clear();
            tbTableName.IsReadOnly = false;
            tbSchemaName.IsReadOnly = false;


            // новые значения
            if (_table != null)
            {
                Table.Fill(_table);

                tbSchemaName.Text = _table.TableEdit.SchemaName;
                tbTableName.Text = _table.TableEdit.TableName;
                tbPKName.Text = _table.TableEdit.PKName;
                tbTableDesc.Text = _table.TableEdit.TableDesc;

                switch (_table.TargetDB)
                {
                    case TargetDBType.PGSQL:
                        cbScriptCreateDB.SelectedIndex = 1;
                        break;
                    case TargetDBType.MSSQL_LIQUIBASE:
                        cbScriptCreateDB.SelectedIndex = 2;
                        break;
                    case TargetDBType.PGSQL_LIQUIBASE:
                        cbScriptCreateDB.SelectedIndex = 3;
                        break;
                    case TargetDBType.MSSQL:
                    case TargetDBType.None:
                    default:
                        cbScriptCreateDB.SelectedIndex = 0;
                        break;
                }

                switch (_table.TableType)
                {
                    case TableType.EVN:
                        cbTableType.SelectedIndex = 1;
                        break;
                    case TableType.PERSONEVN:
                        cbTableType.SelectedIndex = 2;
                        break;
                    case TableType.MORBUS:
                        cbTableType.SelectedIndex = 3;
                        break;
                    case TableType.DICT:
                    default:
                        cbTableType.SelectedIndex = 0;
                        break;
                }

                if (_table.isAddDrop == true) isAddDrop.IsChecked = true; else isAddDrop.IsChecked = false;

                switch (_table.ScriptType)
                {
                    case ScriptType.INSERT_UPDATE:
                        cbScriptCreateType.SelectedIndex = 1;
                        break;
                    case ScriptType.UPDATE:
                        cbScriptCreateType.SelectedIndex = 2;
                        break;
                    case ScriptType.DELETE:
                        cbScriptCreateType.SelectedIndex = 3;
                        break;
                    case ScriptType.INSERT:
                    case ScriptType.ALTER:
                    case ScriptType.CREATE:
                    default:
                        cbScriptCreateType.SelectedIndex = 0;
                        break;
                }

                tbScriptCreate.Text = _table.SQLScript;
                //tabAlter.Header = Table.ScriptFilename;

            }

            FieldType.ItemsSource = ListTypes;
            dgFields.ItemsSource = Table.TableEdit.ListField;

            tabData.Visibility = Visibility.Collapsed;
            tabAlter.Visibility = Visibility.Visible;
            tabAlter.Focus();
            tabStructure.Focus();
            dgFieldsRefresh();
            tbTableName.Focus();

        }

        private void btLockSessions_Click(object sender, RoutedEventArgs e)
        {
            // Блокирующие сессии

            if (tbTableName.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо заполнить Имя таблицы !");
                tbTableName.Focus();
                return;
            }

            string queryString = "";
            if (Connect.IsConnected == ConnType.MSSQL)
                queryString = @"select 'TABLE' as object_type, s.name as schema_name, t.name as object_name, s.name + '.' + v.name as view_name, c.name as column_name, ep.value as table_descr, fk.name as definition
from sys.foreign_keys fk
inner join sys.foreign_key_columns fkc on fkc.constraint_object_id=fk.object_id
inner join sys.schemas s on s.schema_id=fk.schema_id
inner join sys.tables t on t.object_id=fk.parent_object_id
inner join sys.columns c on c.object_id=fkc.parent_object_id and c.column_id=fkc.parent_column_id
left join sys.views v on v.name = 'v_' + t.name and v.schema_id = t.schema_id
left outer join sys.extended_properties ep on ep.major_id = t.object_id and ep.minor_id = 0 
where fk.referenced_object_id = object_id('" + Table.TableEdit.FullTableNameToScript + @"')
UNION 
select sv.type_desc as object_type, s.name as schema_name, sv.name as object_name, s.name + '.' + sv.name, '', '', sm.definition
from sys.views sv
join sys.sql_modules sm on sv.object_id = sm.object_id
join sys.schemas s on s.schema_id = sv.schema_id
where sm.definition like '%" + Table.TableEdit.FullTableNameToScript + @"%' and s.name<>'tmp'
UNION 
select sp.type_desc as object_type, s.name as schema_name, sp.name as object_name, '', '', '', sm.definition
from sys.procedures sp
join sys.sql_modules sm on sp.object_id = sm.object_id
join sys.schemas s on s.schema_id = sp.schema_id
where sm.definition like '%" + Table.TableEdit.FullTableNameToScript + @"%' and s.name<>'tmp'";
            else if (Connect.IsConnected == ConnType.PGSQL)
                queryString = @"select 'TABLE' as object_type, t.table_schema as schema_name, t.table_name as object_name, v.table_schema || '.' || v.table_name as view_name,
ccu.column_name, obj_description((t.table_schema || '.' || t.table_name)::regclass, 'pg_class') as table_descr, tc.constraint_name as definition
from information_schema.tables t
inner
join information_schema.columns c on c.table_schema = t.table_schema and c.table_name = t.table_name
inner join information_schema.key_column_usage kcu on kcu.table_schema = c.table_schema and kcu.table_name = c.table_name and kcu.column_name = c.column_name
inner join information_schema.table_constraints tc on kcu.constraint_name = tc.constraint_name and kcu.constraint_schema = tc.constraint_schema and tc.constraint_type = 'FOREIGN KEY'
inner join information_schema.constraint_column_usage ccu on ccu.constraint_name = tc.constraint_name and ccu.table_schema = tc.table_schema and ccu.column_name = c.column_name
left join information_schema.views v on v.table_schema = t.table_schema and v.table_name = 'v_' || t.table_name
where ccu.table_schema || '.' || ccu.table_name = '" + Table.TableEdit.FullTableNameToScript.ToLower() + @"'
UNION
select 'VIEW' as object_type, schemaname as schema_name, viewname as object_name, schemaname || '.' || viewname as view_name, '', '', pg_views.definition
    FROM pg_depend
    JOIN pg_rewrite ON pg_depend.objid = pg_rewrite.oid
JOIN pg_class as dependent_view ON pg_rewrite.ev_class = dependent_view.oid
JOIN pg_class as source_table ON pg_depend.refobjid = source_table.oid
JOIN pg_namespace dependent_ns ON dependent_ns.oid = dependent_view.relnamespace
JOIN pg_namespace source_ns ON source_ns.oid = source_table.relnamespace
join pg_views on pg_views.schemaname = dependent_ns.nspname and pg_views.viewname = dependent_view.relname
WHERE source_ns.nspname || '.' || source_table.relname = '" + Table.TableEdit.FullTableNameToScript.ToLower() + @"'
UNION
select 'PROC' as object_type, pg_namespace.nspname as schema_name, p.proname as object_name, '', '', '', p.prosrc
from pg_proc p
JOIN pg_namespace ON pg_namespace.oid = p.pronamespace
where lower(prosrc) like '%" + Table.TableEdit.FullTableNameToScript.ToLower() + @"%'
ORDER BY schema_name, definition";

            try
            {
                Cursor oldCursor = this.Cursor;
                dgDopInfoGrid.ItemsSource = Connect.FillDataTable(queryString).DefaultView; ;
                this.Cursor = oldCursor;
                tabDopInfoGrid.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
              
        private void btDependOn_Click(object sender, RoutedEventArgs e)
        {
            // Кто зависит от таблицы

            if (tbTableName.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо заполнить Имя таблицы !");
                tbTableName.Focus();
                return;
            }

            string queryString = "";
            if (Connect.IsConnected == ConnType.MSSQL)
                queryString = @"select 'TABLE' as object_type, s.name as schema_name, t.name as object_name, s.name + '.' + v.name as view_name, c.name as column_name, ep.value as table_descr, fk.name as definition
from sys.foreign_keys fk
inner join sys.foreign_key_columns fkc on fkc.constraint_object_id=fk.object_id
inner join sys.schemas s on s.schema_id=fk.schema_id
inner join sys.tables t on t.object_id=fk.parent_object_id
inner join sys.columns c on c.object_id=fkc.parent_object_id and c.column_id=fkc.parent_column_id
left join sys.views v on v.name = 'v_' + t.name and v.schema_id = t.schema_id
left outer join sys.extended_properties ep on ep.major_id = t.object_id and ep.minor_id = 0 
where fk.referenced_object_id = object_id('" + Table.TableEdit.FullTableNameToScript + @"')
UNION 
select sv.type_desc as object_type, s.name as schema_name, sv.name as object_name, s.name + '.' + sv.name, '', '', sm.definition
from sys.views sv
join sys.sql_modules sm on sv.object_id = sm.object_id
join sys.schemas s on s.schema_id = sv.schema_id
where sm.definition like '%" + Table.TableEdit.FullTableNameToScript + @"%' and s.name<>'tmp'
UNION 
select sp.type_desc as object_type, s.name as schema_name, sp.name as object_name, '', '', '', sm.definition
from sys.procedures sp
join sys.sql_modules sm on sp.object_id = sm.object_id
join sys.schemas s on s.schema_id = sp.schema_id
where sm.definition like '%" + Table.TableEdit.FullTableNameToScript + @"%' and s.name<>'tmp'";
            else if (Connect.IsConnected == ConnType.PGSQL)
                queryString = @"select 'TABLE' as object_type, t.table_schema as schema_name, t.table_name as object_name, v.table_schema || '.' || v.table_name as view_name,
ccu.column_name, obj_description((t.table_schema || '.' || t.table_name)::regclass, 'pg_class') as table_descr, tc.constraint_name as definition
from information_schema.tables t
inner
join information_schema.columns c on c.table_schema = t.table_schema and c.table_name = t.table_name
inner join information_schema.key_column_usage kcu on kcu.table_schema = c.table_schema and kcu.table_name = c.table_name and kcu.column_name = c.column_name
inner join information_schema.table_constraints tc on kcu.constraint_name = tc.constraint_name and kcu.constraint_schema = tc.constraint_schema and tc.constraint_type = 'FOREIGN KEY'
inner join information_schema.constraint_column_usage ccu on ccu.constraint_name = tc.constraint_name and ccu.table_schema = tc.table_schema and ccu.column_name = c.column_name
left join information_schema.views v on v.table_schema = t.table_schema and v.table_name = 'v_' || t.table_name
where ccu.table_schema || '.' || ccu.table_name = '" + Table.TableEdit.FullTableNameToScript.ToLower() + @"'
UNION
select 'VIEW' as object_type, schemaname as schema_name, viewname as object_name, schemaname || '.' || viewname as view_name, '', '', pg_views.definition
    FROM pg_depend
    JOIN pg_rewrite ON pg_depend.objid = pg_rewrite.oid
JOIN pg_class as dependent_view ON pg_rewrite.ev_class = dependent_view.oid
JOIN pg_class as source_table ON pg_depend.refobjid = source_table.oid
JOIN pg_namespace dependent_ns ON dependent_ns.oid = dependent_view.relnamespace
JOIN pg_namespace source_ns ON source_ns.oid = source_table.relnamespace
join pg_views on pg_views.schemaname = dependent_ns.nspname and pg_views.viewname = dependent_view.relname
WHERE source_ns.nspname || '.' || source_table.relname = '" + Table.TableEdit.FullTableNameToScript.ToLower() + @"'
UNION
select 'PROC' as object_type, pg_namespace.nspname as schema_name, p.proname as object_name, '', '', '', p.prosrc
from pg_proc p
JOIN pg_namespace ON pg_namespace.oid = p.pronamespace
where lower(prosrc) like '%" + Table.TableEdit.FullTableNameToScript.ToLower() + @"%'
ORDER BY schema_name, definition";

            try
            {
                Cursor oldCursor = this.Cursor;
                dgDopInfoGrid.ItemsSource = Connect.FillDataTable(queryString).DefaultView; ;
                this.Cursor = oldCursor;
                tabDopInfoGrid.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btOnDepends_Click(object sender, RoutedEventArgs e)
        {
            // от кого зависит таблица
            if (tbTableName.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо заполнить Имя таблицы !");
                tbTableName.Focus();
                return;
            }

            string queryString = "";
            if (Connect.IsConnected == ConnType.MSSQL)
                queryString = @"select 'TABLE' as object_type, s.name as schema_name, t.name as object_name, v.name as view_name, c.name as column_name, ep.value as table_descr, fk.name as fk_name
from sys.foreign_keys fk
inner join sys.foreign_key_columns fkc on fkc.constraint_object_id=fk.object_id
inner join sys.columns c on c.object_id=fkc.parent_object_id and c.column_id=fkc.parent_column_id
inner join sys.tables t on t.object_id=fk.referenced_object_id
inner join sys.schemas s on s.schema_id=t.schema_id
left join sys.views v on v.name = 'v_' + t.name and v.schema_id = t.schema_id
left outer join sys.extended_properties ep on ep.major_id = t.object_id and ep.minor_id = 0 
where fk.parent_object_id = object_id('" + Table.TableEdit.FullTableNameToScript + "')";
            else if (Connect.IsConnected == ConnType.PGSQL)
                queryString = @"select 'TABLE' as object_type, ccu.table_schema as schema_name, ccu.table_name as object_name, v.table_schema || '.' || v.table_name as view_name, c.column_name,
obj_description((ccu.table_schema || '.' || ccu.table_name)::regclass, 'pg_class') as table_descr, tc.constraint_name as fk_name
from information_schema.tables t
inner join information_schema.columns c on c.table_schema = t.table_schema and c.table_name = t.table_name
inner join information_schema.key_column_usage kcu on kcu.table_schema = c.table_schema and kcu.table_name = c.table_name and kcu.column_name = c.column_name
inner join information_schema.table_constraints tc on kcu.constraint_name = tc.constraint_name and kcu.constraint_schema = tc.constraint_schema and tc.constraint_type = 'FOREIGN KEY'
inner join information_schema.constraint_column_usage ccu on ccu.constraint_name = tc.constraint_name and ccu.table_schema = tc.table_schema and ccu.column_name = c.column_name
left join information_schema.views v on v.table_schema = ccu.table_schema and v.table_name = 'v_' || ccu.table_name
where t.table_schema || '.' || t.table_name = '" + Table.TableEdit.FullTableNameToScript.ToLower() + "'";

            try
            {
                Cursor oldCursor = this.Cursor;
                dgDopInfoGrid.ItemsSource = Connect.FillDataTable(queryString).DefaultView; ;
                this.Cursor = oldCursor;
                tabDopInfoGrid.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btLastChange_Click(object sender, RoutedEventArgs e)
        {
            // кто последний менял таблицу

            if (tbTableName.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо заполнить Имя таблицы !");
                tbTableName.Focus();
                return;
            }

            string queryString = "";
            if (Connect.IsConnected == ConnType.MSSQL)
                queryString = @"SELECT TOP 10 *
FROM AlterObjectLog
WHERE alterobjectlog_schemaname='" + Table.TableEdit.SchemaName + @"'
AND alterobjectlog_objectname = '" + Table.TableEdit.TableName + @"'
ORDER BY alterobjectlog_insdt desc";
            else if (Connect.IsConnected == ConnType.PGSQL)
                queryString = @"SELECT *
FROM AlterObjectLog
WHERE alterobjectlog_schemaname='" + Table.TableEdit.SchemaName.ToLower() + @"'
AND alterobjectlog_objectname = '" + Table.TableEdit.FullTableNameToScript.ToLower() + @"'
ORDER BY alterobjectlog_insdt desc limit 10";
            try
            {
                Cursor oldCursor = this.Cursor;
                dgDopInfoGrid.ItemsSource = Connect.FillDataTable(queryString).DefaultView; ;
                this.Cursor = oldCursor;
                tabDopInfoGrid.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
   
        private void dgDopInfo_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string header = e.Column.Header.ToString();

            // Replace all underscores with two underscores, to prevent AccessKey handling
            e.Column.Header = header.Replace("_", "__");
        }

        private void btSaveCreate_Click(object sender, RoutedEventArgs e)
        {

            FileStream fs = null;
            Encoding encoding = Encoding.GetEncoding(1251);
            if (isUnicodeCreate.IsChecked == true) encoding = Encoding.UTF8;

            try
            {
                string filename = SaveFileDialog(Table.ScriptFilename, out fs);
                //tabAlter.Header = Table.ScriptFilename;
                if (fs != null)
                {
                    using (StreamWriter file = new StreamWriter(fs, encoding))
                    {
                        file.WriteLine(tbScriptCreate.Text);
                        dgFilesInTaskRefresh();
                    }
                    /*                if (CurrentScript != null)
                                    {
                                        CurrentScript.ScriptFilename = Table.ScriptFilename;
                                        CurrentScript.Table.Fill(Table);
                                        tabTask.Focus();
                                        dgScripts.Focus();
                                    }*/
                }
            }
            finally
            {
                if (fs != null) fs.Dispose();
            }
        }

        private void btClipboardCreate_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(tbScriptCreate.Text);
/*            if (CurrentScript != null)
            {
                CurrentScript.ScriptFilename = Table.ScriptFilename;
                CurrentScript.Table.Fill(Table);
                tabTask.Focus();
                dgScripts.Focus();
            }*/
        }

        private void dgFieldsRefresh()
        {
            ListCollectionView cvTasks = (ListCollectionView)CollectionViewSource.GetDefaultView(dgFields.ItemsSource);

            if (cvTasks.IsAddingNew) cvTasks.CommitNew();
            if (cvTasks.IsEditingItem) cvTasks.CommitEdit();

            if (cvTasks != null && cvTasks.CanSort == true)
            {
                cvTasks.SortDescriptions.Clear();
                cvTasks.SortDescriptions.Add(new SortDescription("FieldOrder", ListSortDirection.Ascending));
            }

            dgFields.Items.Refresh();
        }

        private void tbTableName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = tbTableName.Text.Replace("[","").Replace("]","");
            string schema = tbSchemaName.Text;

            if (Table.TableEdit.TableName != name)
            {
                if (name == null) name = "";
                if (schema == null) schema = "";

                var arr = name.Split('.');

                if (arr.Length > 1)
                {
                    schema = arr[0];
                    name = arr[1];
                }

                Table.TableEdit.SchemaName = schema;
                Table.TableEdit.TableName = name;
                tbSchemaName.Text = schema;
                tbTableName.Text = name;
            }
        }

        private void tbSchemaName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Table.TableEdit.SchemaName = tbSchemaName.Text;
        }

        private void tbTableDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            Table.TableEdit.TableDesc = tbTableDesc.Text;
        }

        private void tbPKName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Table.TableEdit.PKName = tbPKName.Text;
        }

        private void cbScriptCreateDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbScriptCreateDB.SelectedIndex)
            {
                case 0:
                    Table.TargetDB = TargetDBType.MSSQL;
                    break;
                case 1:
                    Table.TargetDB = TargetDBType.PGSQL;
                    break;
                case 2:
                    Table.TargetDB = TargetDBType.MSSQL_LIQUIBASE;
                    break;
                case 3:
                    Table.TargetDB = TargetDBType.PGSQL_LIQUIBASE;
                    break;
                default:
                    Table.TargetDB = TargetDBType.None;
                    break;
            }
        }

        private void btChangeTableName_Click(object sender, RoutedEventArgs e)
        {
            FormNewTableName dlg1 = new FormNewTableName();

            dlg1.tbOldTableName.Text = Table.TableEdit.TableName;

            if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Table.TableEdit.RenameTable(dlg1.tbNewTableName.Text.Trim());

                tbTableName.Text = Table.TableEdit.TableName;
                tbPKName.Text = Table.TableEdit.PKName;
            };

            dlg1.Dispose();

            dgFieldsRefresh();
        }

        private void btAutoPKName_Click(object sender, RoutedEventArgs e)
        {
            tbPKName.Text = "pk_" + Table.TableEdit.TableName + "_id";
        }

        private void DeleteField_Click(object sender, RoutedEventArgs e)
        {

            if (dgFields.SelectedIndex >= 0)
            {
                RowDB field = dgFields.SelectedItem as RowDB;
                Table.TableEdit.ListField.Remove(field);
                dgFieldsRefresh();

            }
        }

        private void AddField_Click(object sender, RoutedEventArgs e)
        {
            Table.TableEdit.AddField("", Table.TableEdit.TableName+"_", "BIGINT");
            dgFieldsRefresh();
        }

        private void btClearFields_Click(object sender, RoutedEventArgs e)
        {

            FieldType.ItemsSource = ListTypes;

            Table.TableOrig.ListField.Clear();
            Table.TableEdit.ListField.Clear();

            dgFields.ItemsSource = Table.TableEdit.ListField;

            cbScriptCreateType.SelectedIndex = 0;
            tbTableName.IsReadOnly = false;
            tbSchemaName.IsReadOnly = false;

            dgFieldsRefresh();
        }


        private void dgFields_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string fieldname = e.Column.SortMemberPath;
            RowDB Row = (RowDB)e.Row.Item;
            if (fieldname.Equals("FKTable"))
            {
                TextBox t = e.EditingElement as TextBox;
                string tablename = t.Text.ToString();
                if (Row.FieldDesc == "") 
                {
                    string desc = GetTableDecription(tablename);
                    Row.FieldDesc = desc;
                }
                if (Row.FKName == "") 
                {
                    string fkname = "fk_" + Table.TableEdit.TableName + "_" + Row.FieldName;
                    Row.FKName = fkname;
                }
                if (Row.FKField == "")
                {
                    string fkfield = GetTablePK(tablename);
                    Row.FKField = fkfield;
                }
            }
        }


        private void AddFieldIdCodeName_Click(object sender, RoutedEventArgs e)
        {
            // добавляю id, Code, Name
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_id", "BIGINT", "", "", "Уникальный идентификатор", "true", "true", "true");
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_Code", "BIGINT", "", "", "Код");
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_Name", "VARCHAR", "100", "", "Наименование");
            dgFieldsRefresh();
        }

        private void AddFieldInsUpdID_Click(object sender, RoutedEventArgs e)
        {
            // добавляю кто добавил, кто изменил
            Table.TableEdit.AddField("", "pmUser_insID", "BIGINT", "", "", "Кто создал запись", "true");
            Table.TableEdit.AddField("", "pmUser_updID", "BIGINT", "", "", "Кто редактировал запись", "true");
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_insDT", "DATETIME", "", "", "Дата создания", "true");
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_updDT", "DATETIME", "", "", "Дата редактирования", "true");
            dgFieldsRefresh();
        }

        private void AddFieldSysNick_Click(object sender, RoutedEventArgs e)
        {
            // добавляю SysNick
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_SysNick", "VARCHAR", "20", "", "Системное наименование");
            dgFieldsRefresh();
        }

        private void AddFieldDescr_Click(object sender, RoutedEventArgs e)
        {
            // добавляю Descr
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_Descr", "VARCHAR", "200", "", "Описание");
            dgFieldsRefresh();
        }

        private void AddFieldBegEndDate_Click(object sender, RoutedEventArgs e)
        {
            // добавляю Период действия (begDate, endDate)
            Table.TableEdit.AddField("0", Table.TableEdit.TableName + "_begDate", "DATETIME", "", "", "Дата начала действия");
            Table.TableEdit.AddField("0", Table.TableEdit.TableName + "_endDate", "DATETIME", "", "", "Дата окончания действия");
            dgFieldsRefresh();
        }

        private void AddFieldBegEndDT_Click(object sender, RoutedEventArgs e)
        {
            // добавляю Период действия (begDate, endDate)
            Table.TableEdit.AddField("0", Table.TableEdit.TableName + "_begDT", "DATETIME", "", "", "Дата начала действия");
            Table.TableEdit.AddField("0", Table.TableEdit.TableName + "_endDT", "DATETIME", "", "", "Дата окончания действия");
            dgFieldsRefresh();
        }

        private void AddFieldSetDate_Click(object sender, RoutedEventArgs e)
        {
            // добавляю Дата (setDate)
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_setDate", "DATETIME", "", "", "Дата документа");
            dgFieldsRefresh();
        }

        private void AddFieldSetDT_Click(object sender, RoutedEventArgs e)
        {
            // добавляю Дата (setDT)
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_setDT", "DATETIME", "", "", "Дата документа");
            dgFieldsRefresh();
        }

        private void AddFieldDelID_Click(object sender, RoutedEventArgs e)
        {
            // добавляю Признак удаления (deleted)
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_deleted", "BIGINT", "", "", "Признак удаления", "false", "false", "false", "1", "fk_" + Table.TableEdit.TableName + "_deleted", "dbo.YesNo", "YesNo_id");
            Table.TableEdit.AddField("", "pmUser_delID", "BIGINT", "", "", "Пользователь, удаливший запись");
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_delDT", "DATETIME", "", "", "Дата удаления");
            dgFieldsRefresh();
        }

        private void AddFieldIs_Click(object sender, RoutedEventArgs e)
        {
            // добавляю Да/Нет (Is)
            Table.TableEdit.AddField("", Table.TableEdit.TableName + "_Is", "BIGINT", "", "", "", "false", "false", "false", "", "", "dbo.YesNo", "YesNo_id");
            dgFieldsRefresh();
        }

        private void AddFieldRegion_Click(object sender, RoutedEventArgs e)
        {
            // добавляю Регион (Region_id)
            Table.TableEdit.AddField("", "Region_id", "BIGINT", "", "", "Идентификатор региона", "false", "false", "false", "", "fk_" + Table.TableEdit.TableName + "_Region_id", "dbo.KLArea", "KLArea_id");
            dgFieldsRefresh();
        }


        private void btFillFromDB_Click(object sender, RoutedEventArgs e)
        {
            btClearFields_Click(sender, e);

            if (tbTableName.Text.Trim() != "")
                try
                {

                    Table.TableOrig.TableName = tbTableName.Text;
                    Table.TableOrig.SchemaName = tbSchemaName.Text;

                    string queryString = "";

                    // считать структуру таблицы
                    // 1. Информация о таблице
                    if (Connect.IsConnected == ConnType.MSSQL)
                        queryString = @"Select schema_name(t.schema_id) as schema_name, t.name as table_name, ep.value as table_descr, pk.name as pk_name 
from sys.tables t left outer join sys.extended_properties ep on ep.major_id = t.object_id and ep.minor_id = 0 
left join sys.indexes pk on pk.object_id = t.object_id and pk.is_primary_key = 1 where t.object_id = object_id('" + Table.TableOrig.FullTableNameToScript + "');";
                    else if (Connect.IsConnected == ConnType.PGSQL)
                        queryString = @"select 
t.table_schema as schema_name,
t.table_name,
obj_description((t.table_schema || '.' || t.table_name)::regclass, 'pg_class') as table_descr,
tco.constraint_name as pk_name
from information_schema.tables t
left join information_schema.table_constraints tco on tco.table_schema = t.table_schema and tco.table_name = t.table_name and tco.constraint_type = 'PRIMARY KEY' 
where t.table_schema || '.' || t.table_name = '" + Table.TableOrig.FullTableNameToScript.ToLower() + "';";

                    if (Connect.IsConnected != ConnType.None)
                    {
                        using (DbDataReader reader = Connect.OpenQuery(queryString))
                        {
                            while (reader.Read())
                            {
                                tbSchemaName.Text = reader[0].ToString();
                                tbTableName.Text = reader[1].ToString();
                                tbTableDesc.Text = reader[2].ToString();
                                tbPKName.Text = reader[3].ToString();

                                Table.TableOrig.TableName = tbTableName.Text;
                                Table.TableOrig.SchemaName = tbSchemaName.Text;
                                Table.TableOrig.TableDesc = tbTableDesc.Text;
                                Table.TableOrig.PKName = tbPKName.Text;

                                break;
                            }
                        }
                    }

                    // 2. Поля
                    if (Connect.IsConnected == ConnType.MSSQL)
                        queryString = @"Select
c.column_id as field_order,
c.name as field_name,
ep.value as field_desc,
Upper(typ.name) as field_type,
case 
	when typ.name = 'binary' or typ.name = 'char' then cast(c.max_length as varchar)
	when typ.name = 'datetime2' or typ.name = 'time' then cast(c.scale as varchar)
	when typ.name = 'decimal' or typ.name = 'numeric' then cast(c.precision as varchar)
	when c.max_length = -1 and (typ.name = 'nvarchar' or typ.name = 'varchar' or typ.name = 'varbinary') then 'max'
    when c.max_length <> -1 and (typ.name = 'varchar'or typ.name = 'varbinary') then cast(c.max_length as varchar)
	when typ.name = 'nchar' then cast(c.max_length/2 as varchar)
    when c.max_length <> -1 and (typ.name = 'nvarchar') then cast(c.max_length/2 as varchar)
    else ''
end as field_size,
case 
	when typ.name = 'decimal' or typ.name = 'numeric' then cast(c.scale as varchar)
    else ''
end as field_dec,
case when c.is_nullable = 1 then 'false' else 'true' end as IsNotNull,
case when c.is_identity = 1 then 'true' else 'false' end as IsIdentity,
case when pk.object_id is not null then 'true' else 'false' end as IsPK,
replace(replace(def.definition, '((', ''), '))', '') as field_default,
fk.name as fk_name,
schema_name(rt.schema_id) + '.' + rt.name as fk_table,
rc.name as fk_field
from sys.tables t
inner join sys.columns c on c.object_id = t.object_id
left outer join sys.extended_properties ep on ep.major_id = t.object_id and ep.minor_id = c.column_id
inner join sys.types typ on typ.system_type_id = c.system_type_id and typ.user_type_id = c.user_type_id
left join sys.default_constraints def on def.parent_object_id = t.object_id and def.parent_column_id = c.column_id
left join(
select i.object_id, ic.column_id
from sys.indexes i
inner join sys.index_columns ic on i.object_id = ic.object_id and i.index_id = ic.index_id
where i.is_primary_key = 1
) pk on pk.object_id = c.object_id and pk.column_id = c.column_id
left join sys.foreign_key_columns fkc on fkc.parent_object_id = c.object_id and fkc.parent_column_id = c.column_id
left join sys.foreign_keys fk on fk.object_id = fkc.constraint_object_id
left join sys.tables rt on rt.object_id = fkc.referenced_object_id
left join sys.columns rc on rc.object_id = fkc.referenced_object_id and rc.column_id = fkc.referenced_column_id
where t.object_id = object_id('" + Table.TableOrig.FullTableNameToScript + "') order by c.column_id;";
                    else if (Connect.IsConnected == ConnType.PGSQL)
                        queryString = @"Select
c.ordinal_position as field_order,
c.column_name as field_name,
col_description((t.table_schema || '.' || t.table_name)::regclass, c.ordinal_position) as field_desc,
Upper(
	case 
    	when c.data_type = 'character' or c.data_type = '""char""' then 'char'
        when c.data_type = 'character varying' then 'varchar'
        else c.data_type
    end) as field_type,
case 
	when c.data_type = 'bit' or c.data_type = 'character' or c.data_type = 'character varying' or c.data_type = '""char""' then cast(c.character_maximum_length as varchar)
	when c.data_type = 'numeric' then cast(c.numeric_precision as varchar)
    else ''
end as field_size,
case 
	when c.data_type = 'numeric' then cast(c.numeric_scale as varchar)
    else ''
end as field_dec,
case when c.is_nullable = 'YES' then 'false' else 'true' end as IsNotNull,
case when c.is_identity = 'YES' then 'true' else 'false' end as IsIdentity,
case when pk.constraint_type = 'PRIMARY KEY' then 'true' else 'false' end as IsPK,
c.column_default as field_default,
fk.constraint_name as fk_name,
fk.table_schema || '.' || fk.table_name as fk_table,
fk.column_name as fk_field
from information_schema.tables t
inner join information_schema.columns c on c.table_schema = t.table_schema and c.table_name = t.table_name
left join lateral(
select tc.constraint_type
from information_schema.key_column_usage kcu
inner join information_schema.table_constraints tc on kcu.constraint_name = tc.constraint_name and kcu.constraint_schema = tc.constraint_schema and tc.constraint_type = 'PRIMARY KEY'
where kcu.table_schema = c.table_schema and kcu.table_name = c.table_name and kcu.column_name = c.column_name
) pk on true
left join lateral(
select tc.constraint_name, ccu.table_schema, ccu.table_name, ccu.column_name
from information_schema.key_column_usage kcu
inner join information_schema.table_constraints tc on kcu.constraint_name = tc.constraint_name and kcu.constraint_schema = tc.constraint_schema and tc.constraint_type = 'FOREIGN KEY'
inner join information_schema.constraint_column_usage ccu on ccu.constraint_name = tc.constraint_name and ccu.table_schema = tc.table_schema and ccu.column_name = c.column_name
where kcu.table_schema = c.table_schema and kcu.table_name = c.table_name and kcu.column_name = c.column_name
) fk on true
where t.table_schema || '.' || t.table_name = '" + Table.TableOrig.FullTableNameToScript.ToLower() + @"'
order by c.ordinal_position";

                    if (Connect.IsConnected != ConnType.None)
                    {
                        using (DbDataReader reader = Connect.OpenQuery(queryString))
                        {

                            Table.TableOrig.ListField.Clear();

                            while (reader.Read())
                            {
                                Table.TableOrig.AddField(reader["field_order"].ToString(),
                                    reader["field_name"].ToString(),
                                    reader["field_type"].ToString(),
                                    reader["field_size"].ToString(),
                                    reader["field_dec"].ToString(),
                                    reader["field_desc"].ToString(),
                                    reader["IsNotNull"].ToString(),
                                    reader["IsIdentity"].ToString(),
                                    reader["IsPK"].ToString(),
                                    reader["field_default"].ToString(),
                                    reader["fk_name"].ToString(),
                                    reader["fk_table"].ToString(),
                                    reader["fk_field"].ToString()
                                    );

                                string type = reader["field_type"].ToString().ToUpper();
                                if (!ListTypes.Contains(type)) { ListTypes.Add(type); }

                                cbScriptCreateType.SelectedIndex = 1;
                                tbTableName.IsReadOnly = true;
                                tbSchemaName.IsReadOnly = true;
                            }

                            Table.TableEdit.ListField.Clear();
                            Table.TableEdit.ListField = Table.TableOrig.ListField.Select(item => new RowDB(item)).ToList();

                            dgFields.ItemsSource = Table.TableEdit.ListField;

                            dgFieldsRefresh();
                        }
                    }

                    // 3. Информация о типе таблицы
                    cbTableType.SelectedIndex = 0;

                    queryString = @"Select table_type from (
select 
'Evn' as table_type,
'dbo' as schema_name,
EvnClass_SysNick as table_name
from dbo.EvnClass
union all
select 
'PersonEvn' as table_type,
'dbo' as schema_name,
PersonEvnClass_SysNick as table_name
from dbo.PersonEvnClass
union all
select 
'Morbus' as table_type,
'dbo' as schema_name,
MorbusClass_SysNick as table_name
from dbo.MorbusClass
) t
where lower(t.schema_name + '.' + t.table_name) = '" + Table.TableOrig.FullTableNameToScript.ToLower() + "';";

                    if (Connect.IsConnected != ConnType.None)
                        try
                        {
                            using (DbDataReader reader = Connect.OpenQuery(queryString))
                            {

                                while (reader.Read())
                                {
                                    string s = reader[0].ToString();
                                    switch (s)
                                    {
                                        case "Evn": cbTableType.SelectedIndex = 1; break;
                                        case "PersonEvn": cbTableType.SelectedIndex = 2; break;
                                        case "Morbus": cbTableType.SelectedIndex = 3; break;
                                        default: cbTableType.SelectedIndex = 0; break;
                                    }
                                    break;
                                }
                            }
                        }
                        catch { }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            else
            {
                MessageBox.Show($"Заполнить имя таблицы!");
                tbTableName.Focus();
            }
        }


        private void btGenerateCreate_Click(object sender, RoutedEventArgs e)
        {
            Cursor oldCursor = this.Cursor;

            if ((Connect.IsConnected != ConnType.None) && (Table.TableEdit.FullTableNameToScript != ""))
                try
                {

                    this.Cursor = Cursors.Wait;
                    tbScriptCreate.Clear();
                    string title = "";
                    if (Task != null) title = Task.TaskInfoToScript;
                    tbScriptCreate.Text = Table.GenerateScript(title);
                    tabScriptCreate.IsSelected = true;
                    //tabAlter.Header = Table.ScriptFilename;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            this.Cursor = oldCursor;
        }

        private void cbTableType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbTableType.SelectedIndex)
            {
                case 0:
                    Table.TableType = TableType.DICT;
                    break;
                case 1:
                    Table.TableType = TableType.EVN;
                    break;
                case 2:
                    Table.TableType = TableType.PERSONEVN;
                    break;
                case 3:
                    Table.TableType = TableType.MORBUS;
                    break;
                default:
                    Table.TableType = TableType.DICT;
                    break;
            }

        }

        private void isAddDrop_Checked(object sender, RoutedEventArgs e)
        {
            Table.isAddDrop = true;
        }

        private void isAddDrop_Unchecked(object sender, RoutedEventArgs e)
        {
            Table.isAddDrop = false;
        }

        private void cbScriptCreateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbScriptCreateType.SelectedIndex)
            {
                case 0:
                    Table.ScriptType = ScriptType.CREATE;
                    break;
                case 1:
                    Table.ScriptType = ScriptType.ALTER;
                    break;
                default:
                    Table.ScriptType = ScriptType.CREATE;
                    break;
            }

        }

        private void tbScriptCreate_TextChanged(object sender, TextChangedEventArgs e)
        {
            Table.SQLScript = tbScriptCreate.Text;
        }



        public string GetTableDecription(string name)
        {
            string queryString = "";
            string result = "";

            if (Connect.IsConnected == ConnType.MSSQL)
                queryString = @"Select distinct ep.value as table_descr 
from sys.tables t left outer join sys.extended_properties ep on ep.major_id = t.object_id and ep.minor_id = 0 
where t.object_id = object_id('" + name + "');";
            else if (Connect.IsConnected == ConnType.PGSQL)
                queryString = @"select 
obj_description((t.table_schema || '.' || t.table_name)::regclass, 'pg_class') as table_descr
from information_schema.tables t
where t.table_schema || '.' || t.table_name = '" + name.ToLower() + "';";

            if (Connect.IsConnected != ConnType.None)
            {
                using (DbDataReader reader = Connect.OpenQuery(queryString))
                {
                    while (reader.Read())
                    {
                        result = reader[0].ToString();
                        break;
                    }
                }
            }

            return result;
        }


        public string GetTablePK(string name)
        {
            string queryString = "";
            string result = "";

            if (Connect.IsConnected == ConnType.MSSQL)
                queryString = @"DECLARE @PK varchar(max); select @PK = ISNULL(@PK + ',', '') + c.name 
from sys.indexes i 
inner join sys.index_columns ic on i.object_id = ic.object_id and i.index_id = ic.index_id 
inner join sys.columns c on i.object_id = c.object_id and ic.column_id = c.column_id 
where i.object_id = object_id('" + name + @"') and i.is_primary_key = 1 order by ic.index_column_id; 
SELECT @PK AS PKColumnList; ";
            else if (Connect.IsConnected == ConnType.PGSQL)
                queryString = @"SELECT array_to_string(array( 
select kcu.column_name from information_schema.table_constraints tco 
join information_schema.key_column_usage kcu on kcu.constraint_name = tco.constraint_name and kcu.constraint_schema = tco.constraint_schema  
where tco.constraint_type = 'PRIMARY KEY' 
and kcu.table_schema || '.' || kcu.table_name = '" + name.ToLower() + "'order by kcu.ordinal_position),',');";

            if (Connect.IsConnected != ConnType.None)
            {
                using (DbDataReader reader = Connect.OpenQuery(queryString))
                {
                    while (reader.Read())
                    {
                        result = reader[0].ToString();
                        break;
                    }
                }
            }

            return result;
        }



    }




}
