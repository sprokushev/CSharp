using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLGen
{
    public partial class FormAddScript : Form
    {

        public string TaskFolder;
        public GITScript newScript;

        public FormAddScript()
        {
            InitializeComponent();
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            string file = Dlg.OpenFileDialog(TaskFolder);
            if (file != "")
            {
                tbScriptFilename.Text = file;
                tbScriptFilename_Leave(sender, e);

                file = Path.GetFileNameWithoutExtension(tbScriptFilename.Text.Trim());

                var arr = file.ToLower().Split(' ');

                if ((arr.Length>=1) && ((arr[0] == "ms") || (arr[0] == "mssql")))
                {
                    cbGITProject.Text = "msdbupdate";
                    cbGITProject_Leave(sender, e);
                }

                if ((arr.Length >= 1) && ((arr[0] == "pg") || (arr[0] == "pgsql")))
                {
                    cbGITProject.Text = "liquibase_project_new";
                    cbGITProject_Leave(sender, e);
                }

                if ((arr.Length >= 4) && ((arr[3] == "alter") || (arr[3] == "create")))
                {
                    if (cbGITProject.Text == "msdbupdate") cbGITTypeObject.Text = "";
                    else cbGITTypeObject.Text = "TABLE";
                    cbGITTypeObject_Leave(sender, e);
                }

                if ((arr.Length >= 4) && ((arr[3] == "insert") || (arr[3] == "update") || (arr[3] == "delete")) )
                {
                    if (cbGITProject.Text == "msdbupdate") cbGITTypeObject.Text = "Data";
                    else cbGITTypeObject.Text = "data";
                    cbGITTypeObject_Leave(sender, e);
                }

                if ((arr.Length >= 4) && (arr[3] == "view"))
                {
                    if (cbGITProject.Text == "msdbupdate") cbGITTypeObject.Text = "";
                    else cbGITTypeObject.Text = "VIEW";
                    cbGITTypeObject_Leave(sender, e);
                }

                if ((arr.Length >= 4) && (arr[3] == "proc"))
                {
                    if (cbGITProject.Text == "msdbupdate") cbGITTypeObject.Text = "";
                    else cbGITTypeObject.Text = "FUNCTION";
                    cbGITTypeObject_Leave(sender, e);
                }

                if (arr.Length >= 5)
                {
                    if (cbGITProject.Text == "msdbupdate") cbGITShemaObject.Text = "";
                    else cbGITShemaObject.Text = arr[4];
                    cbGITShemaObject_Leave(sender, e);
                }

                if (arr.Length >= 6)
                {
                    cbGITNameObject.Text = arr[5];
                    cbGITNameObject_Leave(sender, e);
                }

                if ((arr.Length >= 6) && ((arr[3] != "insert") && (arr[3] != "update") && (arr[3] != "delete")) )
                {
                    tbGITFilename.Text = arr[5];
                    tbGITFilename_Leave(sender, e);
                }

            }

        }

        private void tbScriptFilename_Leave(object sender, EventArgs e)
        {
            if (newScript == null) newScript = new GITScript();
            if (newScript.GITScriptname == tbScriptFilename.Text) return;
            newScript.GITScriptname = tbScriptFilename.Text;


            if (newScript.GITProject == "msdbupdate")
            {
                cbGITProject_Leave(sender, e);
            }

            if (newScript.GITTypeObject == "data")
            {
                cbGITTypeObject_Leave(sender, e);
            }
        }

        private void cbGITProject_Leave(object sender, EventArgs e)
        {
            if (newScript == null) newScript = new GITScript();
            if (newScript.GITProject == cbGITProject.Text) return;
            newScript.GITProject = cbGITProject.Text;

            string ProjectFolder = System.IO.Path.Combine(tbGITFolder.Text, newScript.GITProject);

            cbGITTypeObject.Enabled = true;
            cbGITTypeObject.Items.Clear();
            //cbGITTypeObject.Text = "";

            cbGITShemaObject.Enabled = true;
            cbGITShemaObject.Items.Clear();
            //cbGITShemaObject.Text = "";

            cbGITNameObject.Items.Clear();
            //cbGITNameObject.Text = "";

            //tbGITFilename.Text = "";

            switch (newScript.GITProject)
            {
                case "msdbupdate":
                    {
                        cbGITTypeObject.Items.Add("Data");
                        cbGITTypeObject.SelectedIndex = 0;
                        cbGITTypeObject.Text = "";
                        cbGITTypeObject.Enabled = false;
                        cbGITTypeObject_Leave(sender, e);

                        cbGITShemaObject.Text = "";
                        cbGITShemaObject.Enabled = false;
                        cbGITShemaObject_Leave(sender, e);

                        string dataFolder = System.IO.Path.Combine(ProjectFolder, "Data");
                        foreach (var item in Dlg.ListFilesInDir(dataFolder, true, false, false)) cbGITNameObject.Items.Add(item);

                        // надо разобрать имя файла, убрать тип БД
                        tbGITFilename.Text = GetGITFilenameForMsdbupdate(System.IO.Path.GetFileNameWithoutExtension(tbScriptFilename.Text).ToLower());
                        tbGITFilename_Leave(sender, e);

                        break;
                    }
                default:
                    {
                        foreach (var item in Dlg.ListFilesInDir(ProjectFolder, true, false, false).Where(x => ((x.ToLower() != "data") && (x.ToLower() != "task") && (x.ToLower() != "version") && (x.ToLower() != ".git")))) cbGITShemaObject.Items.Add(item);

                        string dboFolder = System.IO.Path.Combine(ProjectFolder, "dbo");
                        foreach (var item in Dlg.ListFilesInDir(dboFolder, true, false, false)) cbGITTypeObject.Items.Add(item);
                        cbGITTypeObject.Items.Add("data");

                        break;
                    }
            }

            //cbGITNameObject_Leave(sender, e);

        }

        private string GetGITFilenameForMsdbupdate(string filename)
        {
            var arr = filename.Split(' ');
            string res = "";

            for (int i = 0; i < arr.Length; i++)
            {
                var s = arr[i].ToLower();

                if ((s == "ms") || (s == "pg") || (s == "mssql") || (s == "pgsql"))
                {
                    arr[i] = "";
                }

                if (arr[i] != "")
                {
                    if (res != "") res = res + "_";
                    res = res + arr[i];
                }
            }

            res = res.Replace(" ", "_").Replace("-", "");
            return res;
        }



        private string GetGITFilenameForData(string filename)
        {
            var arr = filename.Split(' ');
            string res = "";

            for (int i = 0; i < arr.Length; i++)
            {
                var s = arr[i].ToLower();

                if ((s == "ms") || (s == "pg") || (s == "mssql") || (s == "pgsql"))
                {
                    arr[i] = "";
                }

                if (s.Contains("promedweb"))
                {
                    arr[i] = "";
                }

                if (int.TryParse(s, out int j))
                {
                    arr[i] = "";
                }

                if ((s == "alter") || (s == "create") || (s == "insert") || (s == "update") || (s == "delete") || (s == "view") || (s == "proc") || (s == "viewproc"))
                {
                    arr[i] = "";
                }

                if (arr[i] != "")
                {
                    if (res != "") res = res + "_";
                    res = res + arr[i];
                }
            }

            res = res.Replace(" ","_").Replace("-", "") + "_" + DateTime.Now.ToString("yyyyMMdd");
            return res;
        }

        private void cbGITTypeObject_Leave(object sender, EventArgs e)
        {
            if (newScript == null) newScript = new GITScript();
            if (newScript.GITTypeObject == cbGITTypeObject.Text) return;
            newScript.GITTypeObject = cbGITTypeObject.Text;

            if (newScript.GITProject != "msdbupdate")
            {
                cbGITShemaObject.Enabled = true;
                cbGITNameObject.Enabled = true;

                if ((newScript.GITProject != "") && (newScript.GITTypeObject == "data"))
                {
                    cbGITShemaObject.Text = "";
                    cbGITShemaObject.Enabled = false;
                    cbGITShemaObject_Leave(sender, e);

                    cbGITNameObject.Text = "";
                    cbGITNameObject.Enabled = false;
                    cbGITNameObject_Leave(sender, e);

                    // разобрать имя файла, оставить схему и имя таблицы + дата ГГГГММДД
                    tbGITFilename.Text = GetGITFilenameForData(System.IO.Path.GetFileNameWithoutExtension(tbScriptFilename.Text).ToLower());
                    tbGITFilename_Leave(sender, e);
                }
            }

        }

        private void cbGITShemaObject_Leave(object sender, EventArgs e)
        {
            if (newScript == null) newScript = new GITScript();
            if (newScript.GITShemaObject == cbGITShemaObject.Text) return;
            newScript.GITShemaObject = cbGITShemaObject.Text;

            if ((newScript.GITProject != "") && (newScript.GITTypeObject != "") && (newScript.GITShemaObject != "") && 
                (newScript.GITProject != "msdbupdate") && (newScript.GITTypeObject != "data"))
            {
                cbGITNameObject.Items.Clear();
                //cbGITNameObject.Text = "";
                cbGITNameObject.Enabled = true;

                string typeFolder = System.IO.Path.Combine(tbGITFolder.Text, newScript.GITProject, newScript.GITShemaObject, newScript.GITTypeObject);
                foreach (var item in Dlg.ListFilesInDir(typeFolder, true, false, false)) cbGITNameObject.Items.Add(item);

                //cbGITNameObject_Leave(sender, e);

                //tbGITFilename.Text = "";
                //tbGITFilename_Leave(sender, e);
            }

        }

        private void cbGITNameObject_Leave(object sender, EventArgs e)
        {
            if (newScript == null) newScript = new GITScript();

            string s = cbGITNameObject.Text;
            if ( (s != "") && (! cbGITNameObject.Items.Contains(s)) )
            {
                s = s.Substring(0, s.Length - 1);
                if (cbGITNameObject.Items.Contains(s)) cbGITNameObject.Text = s;
            }

            if (newScript.GITNameObject == cbGITNameObject.Text) return;

            newScript.GITNameObject = cbGITNameObject.Text;

            if ((newScript.GITProject != "") && (newScript.GITTypeObject != "") && (newScript.GITShemaObject != "") && (newScript.GITProject != "msdbupdate") && (newScript.GITNameObject != ""))
            {
                if ((tbGITFilename.Text != newScript.GITNameObject + "s") && (tbGITFilename.Text != newScript.GITNameObject + "l"))
                {
                    tbGITFilename.Text = newScript.GITNameObject;
                    tbGITFilename_Leave(sender, e);
                }
            }
        }

        private void tbGITFilename_Leave(object sender, EventArgs e)
        {
            if (newScript == null) newScript = new GITScript();
            if (newScript.GITFilename == tbGITFilename.Text) return;
            newScript.GITFilename = tbGITFilename.Text;
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            if (! File.Exists(newScript.GITScriptname) )
            {
                MessageBox.Show("Необходимо выбрать существующий файл для добавления в GIT!");
                tbScriptFilename.Focus();
                return;
            }

            string path = "";
            string file = "";

            if (newScript.GITProject == "msdbupdate") 
            {
                path = System.IO.Path.Combine(tbGITFolder.Text, newScript.GITProject, newScript.GITTypeObject, newScript.GITNameObject);
                Directory.CreateDirectory(path);

                file = System.IO.Path.Combine(path, newScript.GITFilename)+".sql";
                int i = 0;

                while (File.Exists(file))
                {
                    i++;
                    file = System.IO.Path.Combine(path, newScript.GITFilename + "_" + i.ToString()) + ".sql";
                };

            } else if (newScript.GITTypeObject == "data")
            { 
                path = System.IO.Path.Combine(tbGITFolder.Text, newScript.GITProject, newScript.GITTypeObject);
                Directory.CreateDirectory(path);
                
                file = System.IO.Path.Combine(path, newScript.GITFilename) + ".sql";
                int i = 0;

                while (File.Exists(file))
                {
                    i++;
                    file = System.IO.Path.Combine(path, newScript.GITFilename + "_" + i.ToString()) + ".sql";
                };
            } else
            {
                path = System.IO.Path.Combine(tbGITFolder.Text, newScript.GITProject, newScript.GITShemaObject, newScript.GITTypeObject, newScript.GITNameObject);
                Directory.CreateDirectory(path);

                file = System.IO.Path.Combine(path, newScript.GITFilename) + ".sql";
                int i = 0;

                while (File.Exists(file))
                {
                    i++;
                    file = System.IO.Path.Combine(path, newScript.GITFilename + "_v" + i.ToString()) + ".sql";
                };
            }

            newScript.GITFilename = Path.GetFileName(file);
        }
    }
}