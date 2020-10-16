using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using System.Runtime.CompilerServices;

namespace SQLGen
{
    public class Task
    {
        // номер задачи
        string _task;
        public string TaskNumber
        {
            get
            {
                if (_task == null) _task = "";
                return this._task.Trim();
            }
            set
            {
                this._task = value.Trim();
            }
        }


        // Url задачи
        public string TaskUrl { get; set; }

        // описание задачи
        public string TaskDesc { get; set; }

        // исполнитель задачи (автор скрипта)
        public string TaskExecutor { get; set; }

        // скрипты по задаче для отправки в GIT
        public List<GITScript> Scripts { get; set; }

        public Task()
        {
            this.Scripts = new List<GITScript>();
        }

        // информация о задаче в скрипт
        public string TaskInfoToScript
        {
            get
            {
                string s = "";

                s = "/*";
                s = s + Environment.NewLine + "-- Author: " + this.TaskExecutor;
                s = s + Environment.NewLine + "-- Change: " + this.TaskUrl;
                s = s + Environment.NewLine + "-- Description: ";
                s = s + Environment.NewLine + this.TaskDesc;
                s = s + Environment.NewLine + "*/";
                return s;
            }
        }

        // Найти скрипт по имени
/*      public Script FindScriptByName(string name)
        {
            return this.Scripts.Find(x => x.ScriptName.ToLower() == name.ToLower());
        }
*/

        // Найти скрипт по имени файла
/*      public Script FindScriptByFilename(string name)
        {
            return this.Scripts.Find(x => x.ScriptFilename.ToLower() == name.ToLower());
        }
*/

        // Добавить новый скрипт в список
        public GITScript AddScript(string GITScriptname, string GITProject, string GITTypeObject, string GITSchemaObject, string GITNameObject, string GITFilename)
        {
            GITScript newScript = new GITScript();

            newScript.GITScriptname = GITScriptname;
            newScript.GITProject = GITProject;
            newScript.GITTypeObject = GITTypeObject;
            newScript.GITShemaObject = GITSchemaObject;
            newScript.GITNameObject = GITNameObject;
            newScript.GITFilename = GITFilename;

            if (this.Scripts == null) this.Scripts = new List<GITScript>();
            this.Scripts.Add(newScript);

            return newScript;
        }
    }

    public partial class MainWindow : Window
    {

        public Task Task;
        public List<string> YmlList = new List<string>();

        public void SetTask(Task _task)
        {

            if (Task == null) Task = new Task();

            if (Task.TaskNumber != "")
            {
                var dir = System.IO.Path.Combine(tbTaskFolder.Text, Task.TaskNumber);
                string filename = dir + "\\" + Task.TaskNumber + ".task";
                if (File.Exists(filename) || (System.Windows.Forms.MessageBox.Show("Сохранить задачу " + Task.TaskNumber + " в папке  " + dir + " ?", "Сохранить", System.Windows.Forms.MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                   )
                {
                    SaveTask(Task);
                }

            }

            tbTaskNumber.Text = "";
            tbTaskUrl.Text = "";
            tbTaskDesc.Text = "";
            tbTaskExecutor.Text = (string)Microsoft.Win32.Registry.GetValue(keyName, "TaskExecutor", "sergey.prokushev@rtmis.ru");
            tbTaskFolder.Text = (string)Microsoft.Win32.Registry.GetValue(keyName, "TaskFolder", "");
            tbGITFolder.Text = (string)Microsoft.Win32.Registry.GetValue(keyName, "GITFolder", "");
            Table.TaskNumber = "";
            Query.TaskNumber = "";

            Task.Scripts.Clear();

            if (_task != null)
            {
                tbTaskNumber.Text = _task.TaskNumber;
                tbTaskUrl.Text = _task.TaskUrl;
                tbTaskDesc.Text = _task.TaskDesc;
                tbTaskExecutor.Text = _task.TaskExecutor;

                Table.TaskNumber = _task.TaskNumber;
                Query.TaskNumber = _task.TaskNumber;

                if (_task.Scripts != null)
                {
                    foreach (var item in _task.Scripts)
                    {
                        Task.AddScript(item.GITScriptname, item.GITProject, item.GITTypeObject, item.GITShemaObject, item.GITNameObject, item.GITFilename);
                    }
                }

            }
            TaskNumberChanged();
            TaskUrlChanged();
            TaskExecutorChanged();
            TaskFolderChanged();
            GITFolderChanged();
            tabTask.Focus();
            dgFilesInTask.ItemsSource = Task.Scripts;
            dgFilesInTaskRefresh();
            tbTaskNumber.Focus();
        }

        public void SaveTask(Task _task)
        {
            if (_task != null)
            {
                var options = new JsonSerializerOptions
                {
                    IgnoreReadOnlyProperties = true,
                    WriteIndented = true
                };

                var dir = System.IO.Path.Combine(tbTaskFolder.Text, _task.TaskNumber);
                if ((dir != "") && (!System.IO.Directory.Exists(dir)) &&
                     (System.Windows.Forms.MessageBox.Show("Создать папку задачи " + _task.TaskNumber + " в каталоге задач " + tbTaskFolder.Text + " ?", "Создать", System.Windows.Forms.MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                     )
                {
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(dir);
                }

                string filename = dir + "\\" + _task.TaskNumber + ".task";
                try
                {
                    string jsonString = JsonSerializer.Serialize<Task>(_task, options);
                    File.WriteAllText(filename, jsonString);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void dgFilesInTaskRefresh()
        {
            dgFilesInTask.Items.Refresh();
        }

        private void btGoUrl_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(tbTaskUrl.Text);
        }

        private void tbTaskExecutor_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.TaskExecutor = tbTaskExecutor.Text;
        }

        private void tbTaskUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.TaskUrl = tbTaskUrl.Text;
        }

        private void tbTaskDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.TaskDesc = tbTaskDesc.Text;
        }


        private void DeleteScript_Click(object sender, RoutedEventArgs e)
        {
            tabTask.Focus();
            dgFilesInTask.Focus();
            if (dgFilesInTask.SelectedIndex >= 0)
            {
                GITScript script = dgFilesInTask.SelectedItem as GITScript;
                Task.Scripts.Remove(script);
                dgFilesInTaskRefresh();
            }
        }

        private void AddScript_Click(object sender, RoutedEventArgs e)
        {
            tabTask.Focus();
            dgFilesInTask.Focus();
            if (tbTaskNumber.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо заполнить Номер задачи !");
                tbTaskNumber.Focus();
                return;
            }
            else
            {
                FormAddScript dlg1 = new FormAddScript();
                dlg1.newScript = new GITScript();
                dlg1.TaskFolder = System.IO.Path.Combine(tbTaskFolder.Text, tbTaskNumber.Text);
                dlg1.tbGITFolder.Text = tbGITFolder.Text.Trim();
                foreach (var item in Dlg.ListFilesInDir(dlg1.tbGITFolder.Text, true, false, false)) dlg1.cbGITProject.Items.Add(item);


                if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (Task.Scripts == null) Task.Scripts = new List<GITScript>();
                    Task.Scripts.Add(dlg1.newScript);
                    dgFilesInTaskRefresh();
                }
                dlg1.Dispose();

            }
        }


        private void TaskNumberChanged()
        {
            bool Changed = (Task.TaskNumber != tbTaskNumber.Text.Trim());

            Task.TaskNumber = tbTaskNumber.Text;

            string TaskUrlDefault = (string)Microsoft.Win32.Registry.GetValue(keyName, "TaskUrlDefault", "https://jira.is-mis.ru/browse/");
            tbTaskUrl.Text = TaskUrlDefault + tbTaskNumber.Text.Trim();

            var dir = System.IO.Path.Combine(tbTaskFolder.Text, tbTaskNumber.Text);
            if ((dir != "") && (!System.IO.Directory.Exists(dir)) &&
                 (System.Windows.Forms.MessageBox.Show("Создать папку задачи " + tbTaskNumber.Text + " в каталоге задач " + tbTaskFolder.Text + " ?", "Создать", System.Windows.Forms.MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                 )
            {
                System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(dir);
            }

            if (Changed) dgFilesInTaskRefresh();
        }


        private void tbTaskNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            TaskNumberChanged();
        }

        private void dgScripts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /*            if ((dgScripts != null) && (dgScripts.SelectedItem != null))
                        {
                            CurrentScript = dgScripts.SelectedItem as Script;
                            switch (CurrentScript.Type)
                            {
                                case BaseScriptType.ALTER:
                                    SetTable(CurrentScript.Table);
                                    break;
                                case BaseScriptType.DATA:
                                    SetQuery(CurrentScript.Query);
                                    break;
                                default:
                                    break;
                            }
                        }*/

        }




        private void btFolder_Click(object sender, RoutedEventArgs e)
        {
            string dir = Dlg.FolderBrowserDialog(tbTaskFolder.Text);
            if (dir != "")
            {
                tbTaskFolder.Text = dir;
                TaskFolderChanged();
                dgFilesInTaskRefresh();
            }
        }

        private void TaskFolderChanged()
        {
            if (tbTaskFolder.Text.Trim() != "")
                Microsoft.Win32.Registry.SetValue(keyName, "TaskFolder", tbTaskFolder.Text.Trim());
        }

        private void tbTaskFolder_LostFocus(object sender, RoutedEventArgs e)
        {
            TaskFolderChanged();
        }

        private void TaskExecutorChanged()
        {
            if (tbTaskExecutor.Text.Trim() != "")
                Microsoft.Win32.Registry.SetValue(keyName, "TaskExecutor", tbTaskExecutor.Text.Trim());
        }

        private void tbTaskExecutor_LostFocus(object sender, RoutedEventArgs e)
        {
            TaskExecutorChanged();
        }

        private void TaskUrlChanged()
        {
            int pos = tbTaskUrl.Text.IndexOf(tbTaskNumber.Text.Trim());
            string TaskUrlDefault = tbTaskUrl.Text.Substring(0, pos).Trim();
            if (TaskUrlDefault != "")
            {
                Microsoft.Win32.Registry.SetValue(keyName, "TaskUrlDefault", TaskUrlDefault);
            }
        }

        private void tbTaskUrl_LostFocus(object sender, RoutedEventArgs e)
        {
            TaskUrlChanged();
        }

        private void miNewTask_Click(object sender, RoutedEventArgs e)
        {
            tabTask.Focus();
            dgFilesInTask.Focus();

            if (Connect.IsConnected != ConnType.None)
            {
                SetTask(null);
            }
        }

        private void miSaveTask_Click(object sender, RoutedEventArgs e)
        {
            tabTask.Focus();
            dgFilesInTask.Focus();

            if (tbTaskNumber.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо заполнить Номер задачи !");
                tbTaskNumber.Focus();
                return;
            }
            else if (Connect.IsConnected != ConnType.None)
            {
                SaveTask(Task);
            }
        }

        private void miOpenTask_Click(object sender, RoutedEventArgs e)
        {
            tabTask.Focus();
            dgFilesInTask.Focus();

            string filename = Dlg.OpenTaskDialog(tbTaskFolder.Text);
            if ((filename != "") && (File.Exists(filename)))
                try
                {
                    string jsonString = File.ReadAllText(filename);
                    Task loadTask = JsonSerializer.Deserialize<Task>(jsonString);
                    SetTask(loadTask);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

        }

        private void btGITFolder_Click(object sender, RoutedEventArgs e)
        {
            string dir = Dlg.FolderBrowserDialog(tbGITFolder.Text);
            if (dir != "")
            {
                tbGITFolder.Text = dir;
                GITFolderChanged();
            }

        }

        private void GITFolderChanged()
        {
            if (tbGITFolder.Text.Trim() != "")
                Microsoft.Win32.Registry.SetValue(keyName, "GITFolder", tbGITFolder.Text.Trim());
        }

        private void tbGITFolder_LostFocus(object sender, RoutedEventArgs e)
        {
            GITFolderChanged();
        }

        private void btSendGIT_Click(object sender, RoutedEventArgs e)
        {

            var Projects = Task.Scripts.GroupBy(p => p.GITProject).Select(g => g.First()).ToList();

            foreach (var project in Projects)
            {
                string GITProjectPath = System.IO.Path.Combine(tbGITFolder.Text, project.GITProject);
                string GITTaskPath = System.IO.Path.Combine(tbGITFolder.Text, project.GITProject, "task");
                string GITTaskFile = System.IO.Path.Combine(GITTaskPath, Task.TaskNumber.ToLower() + ".yml");

                YmlList.Clear();
                YmlList.Add("databaseChangeLog:");

                foreach (var script in Task.Scripts.Where(s => s.GITProject == project.GITProject))
                {
                    if (!File.Exists(script.GITScriptname))
                    {
                        MessageBox.Show("Файл " + script.GITScriptname + " не существует и не будет скопирован!");
                    }
                    else
                    {
                        string GITPath = "";

                        if (script.GITProject == "msdbupdate")
                        {
                            GITPath = System.IO.Path.Combine(GITProjectPath, script.GITTypeObject, script.GITNameObject);
                        }
                        else if (script.GITTypeObject == "data")
                        {
                            GITPath = System.IO.Path.Combine(GITProjectPath, script.GITTypeObject);
                        }
                        else
                        {
                            GITPath = System.IO.Path.Combine(GITProjectPath, script.GITShemaObject, script.GITTypeObject, script.GITNameObject);
                        }

                        string FileInGIT = System.IO.Path.Combine(GITPath, script.GITFilename);
                        string YmlRow = "- include: { file: \".." + FileInGIT.Replace(GITProjectPath, "").Replace("\\", "/") +
                            "\", relativeToChangelogFile: \"true\" }";
                        YmlList.Add(YmlRow);

                        if (File.Exists(FileInGIT))
                        {
                            MessageBox.Show("Файл " + FileInGIT + " уже выложен в GIT!");
                        }
                        else
                            try
                            {
                                File.Copy(script.GITScriptname, FileInGIT, false);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка при копировании в GIT файла " + FileInGIT + " - " + ex.Message);
                            }

                    }
                }

                if ((project.GITProject == "liquibase_project_new") && (YmlList.Count > 1))
                {
                    if (File.Exists(GITTaskFile))
                    {
                        MessageBox.Show("Файл " + GITTaskFile + " уже выложен в GIT!");
                    }
                    else
                        try
                        {
                            System.IO.File.AppendAllLines(GITTaskFile, YmlList);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка при копировании в GIT файла " + GITTaskFile + " - " + ex.Message);
                        }

                }
            }

            MessageBox.Show("Файлы отправлены в GIT!");

        }
    }

    public class GITScript
    {
        // Имя исходного файла
        public string GITScriptname { get; set; }

        // Проект GIT
        public string GITProject { get; set; }

        // Тип объекта
        public string GITTypeObject { get; set; }

        // Схема
        public string GITShemaObject { get; set; }

        // Имя объекта
        public string GITNameObject { get; set; }

        // Имя файла для GIT
        public string GITFilename { get; set; }
    }


}


