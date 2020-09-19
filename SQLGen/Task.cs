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
                return this._task.Trim();
            }
            set
            {
                this._task = value.Trim();
                if (Scripts != null)
                {
                    foreach (var item in Scripts)
                    {
                        if (item.Table != null) item.Table.TaskNumber = this._task;
                        if (item.Query != null) item.Query.TaskNumber = this._task;
                    }
                }
            }
        }


        // Url задачи
        public string TaskUrl { get; set; }

        // описание задачи
        public string TaskDesc { get; set; }

        // исполнитель задачи (автор скрипта)
        public string TaskExecutor { get; set; }

        // скрипты по задаче
        public List<Script> Scripts { get; set; }

        public Task()
        {
            this.Scripts = new List<Script>();
        }


        // Найти скрипт по имени
        public Script FindScriptByName(string name)
        {
            return this.Scripts.Find(x => x.ScriptName.ToLower() == name.ToLower());
        }

        // Найти скрипт по имени файла
        public Script FindScriptByFilename(string name)
        {
            return this.Scripts.Find(x => x.ScriptFilename.ToLower() == name.ToLower());
        }

        // Добавить новый скрипт в список
        public Script AddScript(string ScriptName, BaseScriptType Type, string ScriptFilename, TableDB Table, QueryDB Query)
        {
            Script newScript = new Script();

            newScript.Type = Type;

            if (ScriptName == "")
            {
                int cnt = 0;
                if (this.Scripts != null)
                {
                    cnt = this.Scripts.Count;
                    foreach (var item in Scripts)
                    {
                        int num = 0;
                        if (int.TryParse(item.ScriptName, out num)) if (num > cnt) cnt = num;

                    }
                }
                cnt++;
                ScriptName = cnt.ToString();
            }

            newScript.ScriptName = ScriptName;
            newScript.ScriptFilename = ScriptFilename;
            newScript.Table = new TableDB();
            newScript.Table.TaskNumber = this.TaskNumber;
            if (Table != null) newScript.Table.Fill(Table);
            newScript.Query = new QueryDB();
            newScript.Query.TaskNumber = this.TaskNumber;
            if (Query != null) newScript.Query.Fill(Query);

            if (this.Scripts == null) this.Scripts = new List<Script>();
            this.Scripts.Add(newScript);

            return newScript;
        }

    }

    public class Script
    {

        // название скрипта
        string _name;
        public string ScriptName
        {
            get
            {
                if (_name == null) return "";
                else return _name.Trim();
            }
            set
            {
                _name = value.Trim();
            }
        }

        // тип скрипта
        public BaseScriptType Type { get; set; }
        
        [JsonIgnore]
        public string ScriptType_string
        {
            get
            {
                switch (Type)
                {
                    case BaseScriptType.ALTER:
                        return "ALTER";
                    case BaseScriptType.DATA:
                    default:
                        return "DATA";
                }
            }
            set
            {
                if (value.Trim().ToUpper() == "ALTER") Type = BaseScriptType.ALTER;
                else Type = BaseScriptType.DATA;
            }
        }

        // имя файла со скриптом (полный путь)
        string _filename;
        public string ScriptFilename
        {
            get
            {
                if (_filename == null) return "";
                else return _filename.Trim();
            }
            set
            {
                _filename = value.Trim();
            }
        }

        // ссылка на объект скрипта
        public TableDB Table { get; set; }
        public QueryDB Query { get; set; }
    }


    public partial class MainWindow : Window
    {

        public Task Task;
        public Script CurrentScript;

        public List<string> ScriptTypenames = new List<string> {
            "ALTER",
            "DATA",
        };

        public void SetTask(Task _task)
        {
            if (Task == null) Task = new Task();

            tbTaskNumber.Text = "";
            tbTaskUrl.Text = "";
            tbTaskDesc.Text = "";
            tbTaskExecutor.Text = "";

            ScriptTypename.ItemsSource = ScriptTypenames;
            Task.Scripts.Clear();
            dgScripts.ItemsSource = Task.Scripts;

            if (_task != null)
            {
                tbTaskNumber.Text = _task.TaskNumber;
                tbTaskUrl.Text = _task.TaskUrl;
                tbTaskDesc.Text = _task.TaskDesc;
                tbTaskExecutor.Text = _task.TaskExecutor;

                if (_task.Scripts != null)
                {
                    foreach (var item in _task.Scripts)
                    {
                        Task.AddScript(item.ScriptName, item.Type, item.ScriptFilename, item.Table, item.Query);
                    }
                }

            }

            tabTask.Focus();
            dgScriptsRefresh();
            tbTaskNumber.Focus();
        }


        private void dgScriptsRefresh()
        {
            ListCollectionView cvTasks = (ListCollectionView)CollectionViewSource.GetDefaultView(dgScripts.ItemsSource);

            if (cvTasks.IsAddingNew) cvTasks.CommitNew();
            if (cvTasks.IsEditingItem) cvTasks.CommitEdit();

            if (cvTasks != null && cvTasks.CanSort == true)
            {
                cvTasks.SortDescriptions.Clear();
                cvTasks.SortDescriptions.Add(new SortDescription("ScriptName", ListSortDirection.Ascending));
            }

            dgScripts.Items.Refresh();
        }

        private void tbGoUrl_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(tbTaskUrl.Text);
        }

        private void tbTaskExecutor_TextChanged(object sender, TextChangedEventArgs e)
        {
            Microsoft.Win32.Registry.SetValue(keyName, "TaskExecutor", tbTaskExecutor.Text.Trim());
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
            dgScripts.Focus();
            if (dgScripts.SelectedIndex >= 0)
            {
                Script script = dgScripts.SelectedItem as Script;
                Task.Scripts.Remove(script);
                dgScriptsRefresh();
            }
        }

        private void AddAlterScript_Click(object sender, RoutedEventArgs e)
        {
            dgScripts.Focus();
            if (tbTaskNumber.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо заполнить Номер задачи !");
                tbTaskNumber.Focus();
                return;
            }
            else
            {
                CurrentScript = Task.AddScript("", BaseScriptType.ALTER, "", null, null);
                SetTable(CurrentScript.Table);
                dgScriptsRefresh();
            }
        }

        private void AddDataScript_Click(object sender, RoutedEventArgs e)
        {
            dgScripts.Focus();
            if (tbTaskNumber.Text.Trim() == "")
            {
                MessageBox.Show("Необходимо заполнить Номер задачи !");
                tbTaskNumber.Focus();
                return;
            }
            else
            {
                CurrentScript = Task.AddScript("", BaseScriptType.DATA, "", null, null);
                SetQuery(CurrentScript.Query);
                dgScriptsRefresh();
            }
        }

        private void tbTaskNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbTaskUrl.Text.Trim() == "") tbTaskUrl.Text = "https://jira.is-mis.ru/browse/" + tbTaskNumber.Text.Trim();
            Task.TaskNumber = tbTaskNumber.Text;

        }

        private void dgScripts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((dgScripts != null) && (dgScripts.SelectedItem != null))
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
            }

        }

    }

}
