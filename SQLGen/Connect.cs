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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace SQLGen
{
    public partial class MainWindow : Window
    {

        public ConnectDB Connect = new ConnectDB();
        public List<ConnectDB> ListConnects = new List<ConnectDB>();

        public Boolean isStartup = true;

        private void LoadConnects()
        {
            string filename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ListConnects.json";
            if (File.Exists(filename))
                try
                {
                    string jsonString = File.ReadAllText(filename);
                    ListConnects = JsonSerializer.Deserialize<List<ConnectDB>>(jsonString);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        private void SaveConnects()
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            };

            string filename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ListConnects.json";
            try
            {
                string jsonString = JsonSerializer.Serialize<List<ConnectDB>>(ListConnects, options);
                File.WriteAllText(filename, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenLogin(object sender, RoutedEventArgs e)
        {
            Connect.IsConnected = ConnType.None;

            FormLogin dlg1 = new FormLogin();

            switch (Connect.ConnType)
            {
                case ConnType.PGSQL:
                    dlg1.cbTypeDB.SelectedIndex = 1;
                    break;
                case ConnType.MSSQL:
                default:
                    dlg1.cbTypeDB.SelectedIndex = 0;
                    break;
            };

            dlg1.tbServerName.Text = Connect.ServerName;
            dlg1.tbDatabaseName.Text = Connect.DBName;

            switch (Connect.AuthType)
            {
                case AuthType.DATABASE:
                    dlg1.cbAuthentication.SelectedIndex = 1;
                    break;
                case AuthType.WINDOWS:
                default:
                    dlg1.cbAuthentication.SelectedIndex = 0;
                    break;
            };

            dlg1.tbUsername.Text = Connect.Username;

            dlg1.cbConnectionHistory.Items.Clear();
            foreach (var item in ListConnects) dlg1.cbConnectionHistory.Items.Add(item.DBConnectionName);

            dlg1.ListConnects = this.ListConnects;

            dlg1.connetionString = "";

            dlg1.currentDBConnectionName = (string)Microsoft.Win32.Registry.GetValue(keyName, "LastDBConnectionName", "");

            if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Connect = new ConnectDB();

                Connect.IsConnected = ConnType.None;
                if (Connect.DbConn != null) { Connect.DbConn.Close(); }
                Connect.DbConn = null;

                switch (dlg1.cbTypeDB.SelectedIndex)
                {
                    case 0:
                        Connect.ConnType = ConnType.MSSQL;
                        break;
                    case 1:
                        Connect.ConnType = ConnType.PGSQL;
                        break;
                    default:
                        Connect.ConnType = ConnType.None;
                        break;
                }

                Connect.ServerName = dlg1.tbServerName.Text;
                Connect.DBName = dlg1.tbDatabaseName.Text;

                switch (dlg1.cbAuthentication.SelectedIndex)
                {
                    case 1:
                        Connect.AuthType = AuthType.DATABASE;
                        break;
                    case 0:
                    default:
                        Connect.AuthType = AuthType.WINDOWS;
                        break;
                };

                Connect.Username = dlg1.tbUsername.Text;


                if (Connect.ConnType == ConnType.MSSQL) // MSSQL
                {
                    try
                    {
                        Connect.DbConn = new SqlConnection(dlg1.connetionString);
                        Connect.DbConn.Open();
                        if (Connect.DbConn.State == System.Data.ConnectionState.Open) Connect.IsConnected = ConnType.MSSQL;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    };
                }

                if (Connect.ConnType == ConnType.PGSQL) // PGSQL
                {

                    try
                    {
                        Connect.DbConn = new NpgsqlConnection(dlg1.connetionString);
                        Connect.DbConn.Open();
                        if (Connect.DbConn.State == System.Data.ConnectionState.Open) Connect.IsConnected = ConnType.PGSQL;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    };

                }

            };

            Connect.DBConnectionName = dlg1.tbConnectionName.Text;

            dlg1.Dispose();


            if (Connect.IsConnected == ConnType.None)
            {
                tabAlls.Visibility = Visibility.Hidden;
                tabTask.Visibility = Visibility.Collapsed;
                tabData.Visibility = Visibility.Collapsed;
                tabAlter.Visibility = Visibility.Collapsed;
                miNewTask.IsEnabled = false;
                miOpenTask.IsEnabled = false;
                miSaveTask.IsEnabled = false;

                this.Title = "Нет подключения к БД !";
            }
            else
            {
                this.Title = Connect.DBConnectionName;
                Microsoft.Win32.Registry.SetValue(keyName, "LastDBConnectionName", Connect.DBConnectionName);

                tabAlls.Visibility = Visibility.Visible;
                tabTask.Visibility = Visibility.Visible;
                tabData.Visibility = Visibility.Visible;
                tabAlter.Visibility = Visibility.Visible;
                miNewTask.IsEnabled = true;
                miOpenTask.IsEnabled = true;
                miSaveTask.IsEnabled = true;

                // очистка по умолчанию
                btClearFields_Click(sender, e);
                cbScriptCreateType.SelectedIndex = 0;
                cbScriptType.SelectedIndex = 0;

                // обработка загрузки задачи из коммандной строки
                String[] args = App.Args;
                if ((args != null) && (args.Length>0) && (args[0] != "") && (File.Exists(args[0])))
                    try
                    {
                        string jsonString = File.ReadAllText(args[0]);
                        Task loadTask = JsonSerializer.Deserialize<Task>(jsonString);
                        SetTask(loadTask);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        SetTask(null);
                    }
                else SetTask(null);

                var item = ListConnects.Find(x => (x.DBConnectionName == Connect.DBConnectionName));
                if ( item == null )
                {
                    ListConnects.Add(Connect);
                }

                
            }

        }

        private void winMain_Activated(object sender, EventArgs e)
        {
            if (isStartup && (Connect.IsConnected == ConnType.None) )
            {
                LoadConnects();
                OpenLogin(sender, e as RoutedEventArgs);
                isStartup = false;
            }
        }

        private void miConnect_Click(object sender, RoutedEventArgs e)
        {
            OpenLogin(sender, e);
        }

        private void winMain_Closed(object sender, EventArgs e)
        {
            SaveConnects();
            SaveTask(Task);
        }

    }


    public class ConnectDB
    {

        public ConnectDB()
        {
            this.ConnType = ConnType.None;
            this.IsConnected = ConnType.None;
            this.AuthType = AuthType.WINDOWS;
        }

        // БД-источник
        public ConnType ConnType { get; set; }

        // БД-источник - если успешный коннект
        public ConnType IsConnected;

        // Connection для использования в приложении
        public DbConnection DbConn;

        // Название соединения
        public string DBConnectionName {get; set; }

        // Сервер
        string _server;
        public string ServerName
        {
            get
            {
                if ( (_server == null) || (_server == "") ) return "192.168.36.30";
                else return _server.Trim();
            }
            set
            {
                _server = value.Trim();
            }
        }

        public string ServerNameToConnect
        {
            get
            {
                if (this.ConnType == ConnType.PGSQL) return this.ServerName.ToLower();
                else return this.ServerName;
            }
        }

        // База данных
        string _db;
        public string DBName
        {
            get
            {
                if ( (_db == null) || (_db == "") ) return "ProMedTest";
                else return _db.Trim();
            }
            set
            {
                _db = value.Trim();
            }
        }
        public string DBNameToConnect
        {
            get
            {
                if (this.ConnType == ConnType.PGSQL) return this.DBName.ToLower();
                else return this.DBName;
            }
        }

        // Тип авторизации
        public AuthType AuthType { get; set; }

        // Пользователь
        string _user;
        public string Username
        {
            get
            {
                if (_user == null) return "";
                else return _user.Trim();
            }
            set
            {
                _user = value.Trim();
            }
        }
        public string UsernameToConnect
        {
            get
            {
                if (this.ConnType == ConnType.PGSQL) return this.Username.ToLower();
                else return this.Username;
            }
        }

        public DbDataReader OpenQuery(string queryString)
        {
            if (IsConnected == ConnType.MSSQL)
            {
                return new SqlCommand(queryString, (SqlConnection)DbConn).ExecuteReader();
            }
            else if (IsConnected == ConnType.PGSQL)
            {
                return new NpgsqlCommand(queryString, (NpgsqlConnection)DbConn).ExecuteReader();
            }
            else return null;
        }


        public DataTable FillDataTable(string query)
        {
            DataTable dt = new DataTable();

            if (query == "") return dt;

            if (IsConnected == ConnType.MSSQL)
            {
                var command = new SqlCommand(query, (SqlConnection)DbConn);
                command.CommandTimeout = 120;
                SqlDataAdapter sda = new SqlDataAdapter(command);
                sda.Fill(dt);
            }
            if (IsConnected == ConnType.PGSQL)
            {
                var command = new NpgsqlCommand(query, (NpgsqlConnection)DbConn);
                command.CommandTimeout = 120;
                NpgsqlDataAdapter sda = new NpgsqlDataAdapter(command);
                sda.Fill(dt);
            }

            return dt;
        }

    }

}
