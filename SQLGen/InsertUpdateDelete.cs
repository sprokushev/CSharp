using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace SQLGen
{
    public partial class MainWindow : Window
    {

        public QueryDB Query = new QueryDB();


        public void SetQuery (QueryDB _query)
        {

            if (Query == null) Query = new QueryDB();

            // по умолчанию
            tbTableNameSQL.Text = "";
            tbSQL.Text = "";
            cbScriptDB.SelectedIndex = 0;
            cbScriptType.SelectedIndex = 0;
            tbPrimaryKey.Text = "";
            tbScriptIUD.Text = "";
            isUpdateDT.IsChecked = false;
            //tabData.Header = Query.ScriptFilename;


            // новые значения
            if (_query != null)
            {

                Query.Fill(_query);


                tbTableNameSQL.Text = _query.TableName;
                tbSQL.Text = _query.SQLQuery;
                tbPrimaryKey.Text = _query.PrimaryKey;
                tbScriptIUD.Text = _query.SQLScript;
                if (_query.IsUpdateDT == true) isUpdateDT.IsChecked = true; else isUpdateDT.IsChecked = false;

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

                //tabData.Header = Query.ScriptFilename;
            }

            Query.DataTable = new DataTable();
            dgData.ItemsSource = null;

            tabData.Visibility = Visibility.Visible;
            tabAlter.Visibility = Visibility.Collapsed;
            tabData.Focus();
            tabSQL.Focus();
            tbSQL.Focus();
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
            {
                if ((tbPrimaryKey.Text.Trim() == "") && (cbScriptType.SelectedIndex != 0))
                {
                    tbPrimaryKey.Focus();
                    throw new ArgumentException($"Необходимо заполнить Primary Key (через запятую) !");
                }

                try
                {
                    //tabData.Header = Query.ScriptFilename;
                    this.Cursor = Cursors.Wait;
                    tbScriptIUD.Clear();
                    string script = "";
                    string title = "";
                    if (Task != null) title = Task.TaskInfoToScript;
                    Query.GenerateScript(title, null, out script);
                    tbScriptIUD.Text = script;
                    tabScriptIUD.IsSelected = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            this.Cursor = oldCursor;
        }


        private void btGenerateIUDFile_Click(object sender, RoutedEventArgs e)
        {
            if ((Connect.IsConnected != ConnType.None) && (tbTableNameSQL.Text.Trim() != ""))
            {

                if ((tbPrimaryKey.Text.Trim() == "") && (cbScriptType.SelectedIndex != 0))
                {
                    tbPrimaryKey.Focus();
                    throw new ArgumentException($"Необходимо заполнить Primary Key (через запятую) !");
                }

                FileStream fs = null;
                Cursor oldCursor = this.Cursor;
                Encoding encoding = Encoding.GetEncoding(1251);
                if (isUnicodeIUD.IsChecked == true) encoding = Encoding.UTF8;

                try
                {
                    //tabData.Header = Query.ScriptFilename;
                    string filename = Dlg.SaveFileDialog (Query.ScriptFilename, out fs);
                    if (fs != null)
                    {
                        using (StreamWriter file = new StreamWriter(fs, encoding))
                        {

                            try
                            {
                                this.Cursor = Cursors.Wait;
                                string script = "";
                                string title = "";
                                if (Task != null) title = Task.TaskInfoToScript;
                                Query.GenerateScript(title, file, out script);
                                dgFilesInTaskRefresh();

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                        }
/*                        if (CurrentScript != null)
                        {
                            CurrentScript.ScriptFilename = Query.ScriptFilename;
                            CurrentScript.Query.Fill(Query);
                            tabTask.Focus();
                            dgScripts.Focus();
                        }*/
                    }

                }
                finally
                {
                    if (fs != null) fs.Dispose();
                }
                this.Cursor = oldCursor;
            }
        }

        private void dgData_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            string header = e.Column.Header.ToString();

            // Replace all underscores with two underscores, to prevent AccessKey handling
            e.Column.Header = header.Replace("_", "__");
        }


        private void tbSQL_TextChanged(object sender, TextChangedEventArgs e)
        {
            var s_orig = tbSQL.Text.Replace(System.Environment.NewLine, " ").Replace("  "," ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
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

            FileStream fs = null;
            Cursor oldCursor = this.Cursor;
            Encoding encoding = Encoding.GetEncoding(1251);
            if (isUnicodeIUD.IsChecked == true) encoding = Encoding.UTF8;

            try
            {
                //tabData.Header = Query.ScriptFilename;
                string filename = Dlg.SaveFileDialog(Query.ScriptFilename, out fs);
                if (fs != null)
                {
                    using (StreamWriter file = new StreamWriter(fs, encoding))
                    {

                        try
                        {
                            this.Cursor = Cursors.Wait;
                            string script = "";
                            string title = "";
                            if (Task != null) title = Task.TaskInfoToScript;
                            Query.GenerateScript(title, null, out script);
                            tbScriptIUD.Text = script;
                            file.WriteLine(tbScriptIUD.Text);
                            dgFilesInTaskRefresh();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                    /*                if (CurrentScript != null)
                                    {
                                        CurrentScript.ScriptFilename = Query.ScriptFilename;
                                        CurrentScript.Query.Fill(Query);
                                        tabTask.Focus();
                                        dgScripts.Focus();
                                    }*/
                }
            }
            finally
            {
                if (fs != null) fs.Dispose();
            }
            this.Cursor = oldCursor;

        }

        private void btClipboardIUD_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(tbScriptIUD.Text);
/*            if (CurrentScript != null)
            {
                CurrentScript.ScriptFilename = Query.ScriptFilename;
                CurrentScript.Query.Fill(Query);
                tabTask.Focus();
                dgScripts.Focus();
            }*/
        }


        private void tbTableNameSQL_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = tbTableNameSQL.Text.Replace("[", "").Replace("]", "").Trim();
            if (name == "") name="dbo.";
            var arr = name.Split('.');
            if (arr.Length <= 1) name = "dbo." + name;

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
