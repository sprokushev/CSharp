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


    public static class Dlg
    {

        public static List<string> ListFilesInDir(string dir, Boolean AddDir, Boolean AddFile, Boolean IsRecursive)
        {

            List<string> res = new List<string>();

            if (Directory.Exists(dir))
            {
                res.AddRange(ProcessDirectory(dir, dir, AddDir, AddFile, IsRecursive));
            }

            return res;
        }

        private static List<string> ProcessDirectory(string path, string rootpath, Boolean AddDir, Boolean AddFile, Boolean IsRecursive)
        {
            List<string> res = new List<string>();

            if (AddDir == false)
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(path);
                foreach (string fileName in fileEntries)
                {
                    res.Add(fileName.Replace(rootpath + System.IO.Path.DirectorySeparatorChar, string.Empty));
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(path);
            foreach (string subdirectory in subdirectoryEntries)
            {
                if (AddFile == false)
                {
                    res.Add(subdirectory.Replace(rootpath + System.IO.Path.DirectorySeparatorChar, string.Empty));
                }
                if (IsRecursive == true)
                {
                    res.AddRange(ProcessDirectory(subdirectory, rootpath, AddDir, AddFile, IsRecursive));
                }
            }

            return res;
        }


        public static string SaveFileDialog(string filename, out FileStream fs)
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

            return "";
        }

        public static string OpenFileDialog(string pathname)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = pathname;
            string filename = "";
            dlg.DefaultExt = ".sql"; // Default file extension
            dlg.Filter = "(*.sql)|*.sql|Все файлы (*.*)|*.*"; // Filter files by extension
            dlg.CheckFileExists = true;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                filename = dlg.FileName;

                if (File.Exists(filename))
                {
                    return filename;
                }
            }

            return "";
        }

        public static string OpenTaskDialog(string pathname)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = pathname;
            string filename = "";
            dlg.DefaultExt = ".task"; // Default file extension
            dlg.Filter = "(*.task)|*.task|Все файлы (*.*)|*.*"; // Filter files by extension
            dlg.CheckFileExists = true;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                filename = dlg.FileName;

                if (File.Exists(filename))
                {
                    return filename;
                }
            }

            return "";
        }

        public static string FolderBrowserDialog(string pathname)
        {

            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.SelectedPath = pathname;
                fbd.ShowNewFolderButton = true;
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
            }

            return "";
        }

    }

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




    }
}

