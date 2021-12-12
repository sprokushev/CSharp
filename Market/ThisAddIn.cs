// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Excel = Microsoft.Office.Interop.Excel;
//using Office = Microsoft.Office.Core;

using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Win32;
using PSVClassLibrary;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Market
{
    public partial class ThisAddIn
    {
        // токен авторизации в клиенте Тинькофф
        public static string token { get; set; }

        // ключ реестра для хранения зашифрованного токена
        public static string keyName { get; } = "HKEY_CURRENT_USER\\Software\\PSV";

        // параметры
        public static string InvestFileName { get; set; }

        public static string ExcelListName { get; set; }
        public static int ExcelList_columnTicker { get; set; } = 1;
        public static int ExcelList_columnValuteCurs { get; set; } = 6;

        public static string ExcelTinkoffName { get; set; }
        public static string ExcelProfitName { get; set; }
        public static string ExcelDividendName { get; set; }
        public static string ExcelHistoryName { get; set; }

        public static List<string> BrokersName { get; set; }
        public static Dictionary<string, string> MarketTypes { get; set; }

        public static string TickerName { get; set; }
        public static string LastDateName { get; set; }
        public static string PriceName { get; set; }
        public static string NominalName { get; set; }

        // ссылка на панель
        MarketRibbon Ribbon;

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            // настраиваем панель
            Ribbon = new MarketRibbon();
            Ribbon.btnTokenClicked += ribbon_btnTokenClicked;
            Ribbon.btnOptionsClicked += ribbon_btnOptionsClicked;
            Ribbon.btnMarketListClicked += ribbon_btnMarketListClicked;
            Ribbon.btnRefreshValuteCursClicked += ribbon_btnRefreshValuteCursClicked;
            Ribbon.btnTinkoffClicked += ribbon_btnTinkoffClicked;
            Ribbon.btnRefreshAllClicked += ribbon_btnRefreshAllClicked;
            Ribbon.btnRefreshBrokersClicked += ribbon_btnRefreshBrokersClicked;
            Ribbon.btnProfitClicked += ribbon_btnProfitClicked;
            Ribbon.btnBrokerBuyClicked += ribbon_btnBrokerBuyClicked;
            Ribbon.btnBrokerSellClicked += ribbon_btnBrokerSellClicked;
            Ribbon.btnDividendClicked += ribbon_btnDividendClicked;

            return Globals.Factory.GetRibbonFactory().CreateRibbonManager(new IRibbonExtension[] { Ribbon });
        }


        // Обновить все данные, связанные с котировками
        private void ribbon_btnRefreshAllClicked()
        {
            //ribbon_btnRefreshValuteCursClicked();
            //ribbon_btnMarketListClicked();
            ribbon_btnTinkoffClicked();
            ribbon_btnRefreshBrokersClicked();
        }


        // Вызов формы регистрации токена авторизации
        private void ribbon_btnTokenClicked()
        {
            FormAskToken dlg1 = new FormAskToken();
           
            if (dlg1.ShowDialog() == DialogResult.OK)
            {
                LoadToken();
            };

            dlg1.Dispose();
        }

        // загрузить токен
        private void LoadToken()
        {
            // считываем из реестра параметры
            if ((keyName != null) && (keyName != ""))
            {
                // Пробуем считать из реестра токен авторизации для доступа в личный кабинет брокера Тинькофф
                byte[] bytes = (byte[])Registry.GetValue(keyName, "Token", null);

                // Дешифруем токен
                if (bytes != null) 
                    token = CryptoClass.decrypt_function(bytes);
                else 
                    token = "";

                if (Ribbon != null)
                    if (token == "")
                    {
                        Ribbon.grpTinkoff.Label = "Токен НЕ зарегистрирован!";
                        Ribbon.grpStock.Visible = false;
                        Ribbon.grpInvest.Visible = false;
                    }
                    else
                    {
                        Ribbon.grpTinkoff.Label = "Токен зарегистрирован";
                        Ribbon.grpStock.Visible = true;
                        Ribbon.grpInvest.Visible = true;
                    }
            }
        }

        // Вызов формы настроек книги Excel
        private void ribbon_btnOptionsClicked()
        {
            FormOptions dlg1 = new FormOptions();

            if (dlg1.ShowDialog() == DialogResult.OK)
            {
                LoadOptions();
            };

            dlg1.Dispose();
        }

        // загрузить настройки
        private void LoadOptions()
        {
            // считываем из реестра параметры
            if ((keyName != null) && (keyName != ""))
            {
                InvestFileName = (string)Registry.GetValue(keyName, "InvestFileName", "инвестиции");

                ExcelListName = (string)Registry.GetValue(keyName, "marketList", "List");
                ExcelTinkoffName = (string)Registry.GetValue(keyName, "Tinkoff", "Tinkoff");
                ExcelProfitName = (string)Registry.GetValue(keyName, "Profit", "Profit");
                ExcelDividendName = (string)Registry.GetValue(keyName, "Dividend", "Dividend");
                ExcelHistoryName = (string)Registry.GetValue(keyName, "History", "History");

                TickerName = (string)Registry.GetValue(keyName, "TickerName", "Тикер");
                LastDateName = (string)Registry.GetValue(keyName, "LastDateName", "Дата котировки");
                PriceName = (string)Registry.GetValue(keyName, "PriceName", "Котировка в валюте");
                NominalName = (string)Registry.GetValue(keyName, "NominalName", "Номинал");

                {
                    var values = (string[])Registry.GetValue(keyName, "Brokers", null);
                    BrokersName = new List<string>();
                    if (values != null)
                        foreach (var item in values)
                            BrokersName.Add(item);
                }


                {
                    var values = (string[])Registry.GetValue(keyName, "Types", null);
                    MarketTypes = new Dictionary<string, string>();
                    if (values != null)
                        foreach (var item in values)
                        {
                            string[] keyvalue = item.Split('=');
                            if (keyvalue.Length == 2)
                            {
                                MarketTypes.Add(keyvalue[0], keyvalue[1]);
                            }
                        }
                }


                if (this.Application != null)
                {
                    if (Ribbon != null)
                    {

                        // проверяем имя файла
                        string filename = this.Application.ActiveWorkbook.Name;
                        if (filename.ToLower().Contains(InvestFileName.ToLower()))
                        {
                            // считываем токен, обновляем информацию на панели о статусе регистрации токена
                            LoadToken();
                        }
                    }

                }


            }
        }


        void Application_WorkbookOpen(Excel.Workbook Wb)
        {
            // Первоначальное считывание настроек
            LoadOptions();
            // считываем курсы валют, обновляем информацию на панели
            ribbon_btnRefreshValuteCursClicked();
            // считываем список ценных бумаг
            FillListPapersFromExcel();
        }

        void AppEvents_SheetChange(object Sh, Excel.Range Target)
        {
            Excel.Worksheet sheet = (Excel.Worksheet)Sh;

            if (sheet.Name.ToLower() == ExcelListName.ToLower()) 
                ListPapersChanged = true;
        }


        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            ((Excel.AppEvents_Event)this.Application).WorkbookOpen += new Excel.AppEvents_WorkbookOpenEventHandler(Application_WorkbookOpen);

            ((Excel.AppEvents_Event)this.Application).NewWorkbook += new Excel.AppEvents_NewWorkbookEventHandler(Application_WorkbookOpen);

            // Call this function is the user right clicks on a cell
            this.Application.SheetBeforeRightClick += new Excel.AppEvents_SheetBeforeRightClickEventHandler(Application_SheetBeforeRightClick);

           // this.Application.SheetChange += new Excel.AppEvents_SheetChangeEventHandler(AppEvents_SheetChange);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
