using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLAP;
using IniParser;
using IniParser.Model;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;


namespace PSVClassLibrary
{
    public enum AppModeTypes { None, List, Sum, Xls }

    public class ArgApp
    {
        // токен авторизации в клиенте Тинькофф
        public string token { get; set; }

        // ключ реестра для хранения зашифрованного токена
        public string keyName { get; } = "HKEY_CURRENT_USER\\Software\\PSV";

        // режим запуска программы
        public AppModeTypes AppMode { get; set; }

        // ini-файл программы
        string inifile;
        public IniData INI { get; }

        // справочник уточнения типов ценных бумаг
        public KeyDataCollection MarketTypes { get; set; }

        // заполняемый excel-файл
        public Excel.Application ExcelApp { get; set; }
        public Excel.Workbook ExcelBook { get; set; }
        // лист для портфеля Тинькофф
        public Excel.Worksheet ExcelTinkoffSheet { get; set; } 
        // лист для списка всех доступных ценных бумаг
        public Excel.Worksheet ExcelListSheet { get; set; }
        // листы разных брокеров для актуализации котировок
        public Dictionary<string,BrokerInfo> Brokers { get; set; }

        public class BrokerInfo
        {
            public Excel.Worksheet ExcelSheet { get; set; }

            public string TickerName { get; set; }
            public string LastDateName { get; set; }
            public string PriceName { get; set; }
            public string ChangePriceName { get; set; }
            public string ValuteCursName { get; set; }
            public string RubPriceName { get; set; }
        }


        public ArgApp()
        {
            token = "";
            AppMode = AppModeTypes.None;
            inifile = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".ini";
            if (File.Exists(inifile))
            {
                var parser = new FileIniDataParser();
                INI = parser.ReadFile(inifile, Encoding.GetEncoding("windows-1251"));
            }
            ExcelApp = null;
            ExcelBook = null;
            ExcelTinkoffSheet = null;
            ExcelListSheet = null;
            Brokers = new Dictionary<string, BrokerInfo>();
        }

        [Verb(IsDefault = true)]
        public void Help(string help)
        {
            if ((help == null) || (help == "")) return;

            string exe_name = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine($"{exe_name} - программа для считывания биржевой информации через Тинкофф-инвестиции.{Environment.NewLine}Возвращаемое программой значение - общая сумма ценных бумаг и наличной валюты на брокерском счете, номинированных в указанной валюте.");

            Console.WriteLine(help);
        }

        public void WriteErrorToLog(string logfile, string errormessage)
        {
            File.AppendAllText(logfile, Environment.NewLine + DateTime.Now + Environment.NewLine + errormessage, System.Text.Encoding.GetEncoding(1251));
        }

        [Global(Description = "/token=текстовый файл с токеном авторизации в первой строке. Токен после считывания шифруется и сохраняется в шифрованном виде, затем программа завершает работу. Достаточно однократно считать токен, при всех последующих запусках зашифрованный токен используется для авторизации пользователя в банковском API", Aliases = "t")]
        public void Token(string value)
        {
            string[] data;
            AppMode = AppModeTypes.None;

            if (File.Exists(value))
            {
                data = File.ReadAllLines(value);
                if ((data == null) || (data.Length == 0) || (data[0] == ""))
                {
                    throw new ArgumentException($"Файл {value} с токеном авторизации пустой или не содержит токен!");
                }
                token = data[0];

                // Шифруем токен
                byte[] Encrypted_Bytes = CryptoClass.encrypt_function(token);

                // Записывем токен в реестр
                Registry.SetValue(keyName, "Token", Encrypted_Bytes);

                Console.WriteLine($"Токен зашифрован и сохранен для последующих запусков. Файл {value} можно удалить! Запустите повторно программу без ключей t или token.");
            }
            else
            {
                throw new FileNotFoundException($"Файл {value} с токеном авторизации не найден!");
            }
        }

        [Global(Description = "Режим запуска программы: /mode=list - вывести на экран список активов Тинькофф со стоимостью в указанной валюте; /mode=sum - вывести на экран общую сумму активов Тинькофф, номинированных в указанной валюте; /mode=xls - заполнить excel-файл в соответствии с параметрами в Tinkoff.ini. По умолчанию = sum", Aliases = "m")]
        public void Mode(string value)
        {
            value = value.ToLower();
            switch (value)
            {
                case "list":
                    AppMode = AppModeTypes.List;
                    break;
                case "sum":
                    AppMode = AppModeTypes.Sum;
                    break;
                case "xls":
                    AppMode = AppModeTypes.Xls;
                    break;
                default:
                    AppMode = AppModeTypes.None;
                    break;
            }

            if (AppMode == AppModeTypes.Xls) 
            {

                if (INI == null)
                {
                    AppMode = AppModeTypes.None;
                    throw new FileLoadException($"Файл {inifile} не доступен для работы!");
                }


                // Читаем имя excel-файла из Tinkoff.ini
                string excelfile = INI["MAIN"]["excel"];
                if (!File.Exists(excelfile))
                {
                    AppMode = AppModeTypes.None;
                    throw new FileNotFoundException($"Файл {excelfile} не существует!");
                }

                ExcelApp = new Excel.Application();
                if (ExcelApp != null)
                {
                    ExcelApp.Visible = false;
                    ExcelBook = ExcelApp.Workbooks.Open(excelfile);
                    
                    if (ExcelBook != null)
                    {

                        // Ищем лист для заполнения списка ценных бумаг
                        value = INI["MAIN"]["sheet"];
                        for (int i = 1; i <= ExcelBook.Sheets.Count; i++)
                        {
                            var sh = (Excel.Worksheet)ExcelBook.Sheets.get_Item(i);
                            if (sh.Name == value)
                            {
                                ExcelListSheet = sh;
                            }
                        }

                        // Ищем лист для заполнения портфеля Тинькофф
                        value = INI["TINKOFF"]["sheet"];
                        for (int i = 1; i <= ExcelBook.Sheets.Count; i++)
                        {
                            var sh = (Excel.Worksheet)ExcelBook.Sheets.get_Item(i);
                            if (sh.Name == value)
                            {
                                ExcelTinkoffSheet = sh;
                            }
                        }

                        // Ищем листы для заполнения других брокеров
                        value = INI["MAIN"]["brokers"];
                        string[] brokers = value.Split(',');
                        foreach (var broker in brokers)
                        {
                            value = INI[broker]["sheet"];

                            for (int i = 1; i <= ExcelBook.Sheets.Count; i++)
                            {
                                var sh = (Excel.Worksheet)ExcelBook.Sheets.get_Item(i);
                                if (sh.Name == value)
                                {
                                    Brokers.Add(broker, new BrokerInfo()
                                    {
                                        ExcelSheet = sh,
                                        TickerName = INI[broker]["ticker"],
                                        LastDateName = INI[broker]["lastdate"],
                                        PriceName = INI[broker]["price"],
                                        ChangePriceName = INI[broker]["change"],
                                        ValuteCursName = INI[broker]["curs"],
                                        RubPriceName = INI[broker]["rubprice"]
                                    });
                                }
                            }

                        }

                        // Считываем из Tinkoff.ini секцию с уточнением типов ценных бумаг
                        MarketTypes = INI["TYPES"];

                    }
                }

                if ((ExcelApp == null) || (ExcelBook == null))
                {
                    AppMode = AppModeTypes.None;
                    throw new FileLoadException($"Файл {excelfile} не доступен для работы!");
                }

            }


        }

    }

}
