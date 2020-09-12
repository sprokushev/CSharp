using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLGen
{
    class Task
    {
        // номер задачи
        public string TaskNumber { get; set; }

        // Url задачи
        public string TaskUrl { get; set; }

        // описание задачи
        public string TaskDesc { get; set; }

        // исполнитель задачи (автор скрипта)
        public string TaskExecutor { get; set; }

        // скрипты по задаче
        List<Script> Scripts { get; set; }
    }

    class Script
    {
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

        // тип скрипта
        BaseScriptType Type { get; set; }

        // ссылка на объект скрипта
        TableDB Table { get; set; }
        QueryDB Query { get; set; }
    }
}
