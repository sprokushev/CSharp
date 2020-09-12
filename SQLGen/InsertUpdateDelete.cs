using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SQLGen
{
    public partial class MainWindow : Window
    {

        public QueryDB Query = new QueryDB();


        public void SetQuery (QueryDB _query)
        {
            tbTableNameSQL.Text = _query.TableName;
            
            tbSQL.Text = _query.SQLQuery;
            
            switch (_query.TargetDB)
            {
                case TargetDBType.MSSQL:
                    cbScriptDB.SelectedIndex = 0;
                    break;
                case TargetDBType.PGSQL:
                    cbScriptDB.SelectedIndex = 1;
                    break;
                case TargetDBType.MSSQL_LIQUIBASE:
                    cbScriptDB.SelectedIndex = 2;
                    break;
                case TargetDBType.PGSQL_LIQUIBASE:
                    cbScriptDB.SelectedIndex = 3;
                    break;
                case TargetDBType.None:
                default:
                    break;
            }

            switch (_query.ScriptType)
            {
                case ScriptType.INSERT:
                    cbScriptType.SelectedIndex = 0;
                    break;
                case ScriptType.INSERT_UPDATE:
                    cbScriptType.SelectedIndex = 1;
                    break;
                case ScriptType.UPDATE:
                    cbScriptType.SelectedIndex = 2;
                    break;
                case ScriptType.DELETE:
                    cbScriptType.SelectedIndex = 3;
                    break;
                case ScriptType.ALTER:
                case ScriptType.CREATE:
                default:
                    break;
            }

            tbPrimaryKey.Text = _query.PrimaryKey;

            tbScriptIUD.Text = _query.SQLScript;

            if (_query.IsUpdateDT == true) isUpdateDT.IsChecked = true; else isUpdateDT.IsChecked = false;

            Query.DataTable = new DataTable();
        }



        private void btSelectIUD_Click(object sender, RoutedEventArgs e)
        {

            if (tbTableNameSQL.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо заполнить Имя таблицы !");
                tbTableNameSQL.Focus();
            }
            else
            {
                string sql = tbSQL.Text;

                cbScriptType.SelectedIndex = 0;

                var arr = sql.Split(' ');
                int pos;

                pos = Array.IndexOf(arr, "delete");
                if (pos != -1)
                {
                    MessageBox.Show("Только оператор SELECT !");
                    return;
                };

                pos = Array.IndexOf(arr, "update");
                if (pos != -1)
                {
                    MessageBox.Show("Только оператор SELECT !");
                    return;
                };

                pos = Array.IndexOf(arr, "insert");
                if (pos != -1)
                {
                    MessageBox.Show("Только оператор SELECT !");
                    return;
                };

                Cursor oldCursor = this.Cursor;
                lbCount.Content = "Строк: ";

                try
                {
                    if (Connect.IsConnected == ConnType.MSSQL)
                    {
                        this.Cursor = Cursors.Wait;
                        SqlDataAdapter sda = new SqlDataAdapter(new SqlCommand(sql, (SqlConnection)Connect.DbConn));
                        Query.DataTable = new DataTable();
                        sda.Fill(Query.DataTable);
                        dgData.ItemsSource = Query.DataTable.DefaultView;
                        lbCount.Content = "Строк: " + Query.DataTable.Rows.Count;
                    }

                    if (Connect.IsConnected == ConnType.PGSQL)
                    {
                        this.Cursor = Cursors.Wait;
                        NpgsqlDataAdapter sda = new NpgsqlDataAdapter(new NpgsqlCommand(sql, (NpgsqlConnection)Connect.DbConn));
                        Query.DataTable = new DataTable();
                        sda.Fill(Query.DataTable);
                        dgData.ItemsSource = Query.DataTable.DefaultView;
                        lbCount.Content = "Строк: " + Query.DataTable.Rows.Count;
                    }

                    tbPrimaryKey.Text = GetTablePK(tbTableNameSQL.Text);
                    tabGrid.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                this.Cursor = oldCursor;
            }
        }




        private void btGenerateIUD_Click(object sender, RoutedEventArgs e)
        {
            Cursor oldCursor = this.Cursor;

            if ((Connect.IsConnected != ConnType.None) && (tbTableNameSQL.Text.Trim() != ""))
                try
                {
                    if ((tbPrimaryKey.Text.Trim() == "") && (cbScriptType.SelectedIndex != 0))
                    {
                        tbPrimaryKey.Focus();
                        throw new ArgumentException($"Необходимо заполнить Primary Key (через запятую) !");
                    }

                    this.Cursor = Cursors.Wait;
                    tbScriptIUD.Clear();
                    tbScriptIUD.Text = Query.GenerateScript();
                    tabScriptIUD.IsSelected = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            this.Cursor = oldCursor;
        }


        private void dgData_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            string header = e.Column.Header.ToString();

            // Replace all underscores with two underscores, to prevent AccessKey handling
            e.Column.Header = header.Replace("_", "__");
        }


        private void tbSQL_TextChanged(object sender, TextChangedEventArgs e)
        {
            var s_orig = tbSQL.Text.Replace(System.Environment.NewLine, " ");
            var s_lower = s_orig.ToLower();
            var arr = s_orig.Split(' ');
            var pos = Array.IndexOf(s_lower.Split(' '), "from");

            if ((pos != -1) && ((pos + 1) < arr.Count()))
            {
                tbTableNameSQL.Text = arr[pos + 1].Trim();
            }

            Query.SQLQuery = tbSQL.Text;
        }

        private void btSaveIUD_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".sql"; // Default file extension
            dlg.Filter = "(*.sql)|*.sql|Все файлы (*.*)|*.*"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, false))
                {
                    file.WriteLine(tbScriptIUD.Text);
                }
            }
        }

        private void btClipboardIUD_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(tbScriptIUD.Text);
        }


        private void tbTableNameSQL_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = tbTableNameSQL.Text.Replace("[", "").Replace("]", "");

            if (Query.TableName != name)
            {
                if (name == null) name = "";
                Query.TableName = name;
                tbTableNameSQL.Text = name;
            }

        }

        private void cbScriptDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbScriptDB.SelectedIndex)
            {
                case 0:
                    Query.TargetDB = TargetDBType.MSSQL;
                    break;
                case 1:
                    Query.TargetDB = TargetDBType.PGSQL;
                    break;
                case 2:
                    Query.TargetDB = TargetDBType.MSSQL_LIQUIBASE;
                    break;
                case 3:
                    Query.TargetDB = TargetDBType.PGSQL_LIQUIBASE;
                    break;
                default:
                    Query.TargetDB = TargetDBType.None;
                    break;
            }

        }

        private void cbScriptType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbScriptType.SelectedIndex)
            {
                case 0:
                    Query.ScriptType = ScriptType.INSERT;
                    break;
                case 1:
                    Query.ScriptType = ScriptType.INSERT_UPDATE;
                    break;
                case 2:
                    Query.ScriptType = ScriptType.UPDATE;
                    break;
                case 3:
                    Query.ScriptType = ScriptType.DELETE;
                    break;
                default:
                    Query.ScriptType = ScriptType.INSERT;
                    break;
            }
        }

        private void isUpdateDT_Unchecked(object sender, RoutedEventArgs e)
        {
            Query.IsUpdateDT = false;
        }

        private void isUpdateDT_Checked(object sender, RoutedEventArgs e)
        {
            Query.IsUpdateDT = true;
        }

        private void tbPrimaryKey_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Query.PrimaryKey = tbPrimaryKey.Text;
        }

        private void tbScriptIUD_TextChanged(object sender, TextChangedEventArgs e)
        {
            Query.SQLScript = tbScriptIUD.Text;
        }

    }

}
