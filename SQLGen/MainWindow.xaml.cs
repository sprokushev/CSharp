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

        // ключ реестра для хранения параметров
        public static string keyName { get; } = "HKEY_CURRENT_USER\\Software\\PSV";

        public MainWindow()
        {
            InitializeComponent();
        }


        public string SaveFileDialog(string filename, out FileStream fs)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = filename; // Default file name
            dlg.DefaultExt = ".sql"; // Default file extension
            dlg.Filter = "(*.sql)|*.sql|Все файлы (*.*)|*.*"; // Filter files by extension
            dlg.CheckFileExists = false;
            dlg.OverwritePrompt = false;
            fs = null;

            FileMode mode = FileMode.Create;

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                filename = dlg.FileName;

                if (File.Exists(filename))
                {
                    if (System.Windows.Forms.MessageBox.Show("Добавить в существующий файл ?", "Добавить", System.Windows.Forms.MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                    {
                        mode = FileMode.Append;
                    }
                }
                fs = new FileStream(filename, mode);
                return filename;
            }
            else return "";
        }

    }
}

