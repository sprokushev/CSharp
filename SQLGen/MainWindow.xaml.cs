using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SQLGen
{
    public enum ScriptType { CREATE, ALTER, INSERT, INSERT_UPDATE, UPDATE, DELETE }
    public enum BaseScriptType { ALTER, DATA }
    public enum TargetDBType { None, MSSQL, PGSQL, MSSQL_LIQUIBASE, PGSQL_LIQUIBASE }
    public enum TableType { DICT, EVN, PERSONEVN, MORBUS }
    public enum ConnType { None, MSSQL, PGSQL }
    public enum AuthType { WINDOWS, DATABASE }
    public enum GeneralType { UNKNOWN, STRING, NUMBER, DATETIME, BOOLEAN }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void tbTaskNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbTaskUrl.Text.Trim() == "") tbTaskUrl.Text = "https://jira.is-mis.ru/browse/" + tbTaskNumber.Text.Trim();
        }

        private void tbGoUrl_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(tbTaskUrl.Text);
        }
    }
}

