using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Office.Interop.Excel;

namespace HomeFinance
{
    public partial class ThisAddIn
    {

        /// <summary>
        /// ссылка на панель
        /// </summary>
        HomeFinanceRibbon Ribbon;

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            // настраиваем панель
            Ribbon = new HomeFinanceRibbon();
            Ribbon.btnFillRashodClicked += ribbon_btnFillRashodClicked;
            Ribbon.btnLoadBankExcelClicked += ribbon_btnLoadBankExcelClicked;

            return Globals.Factory.GetRibbonFactory().CreateRibbonManager(new IRibbonExtension[] { Ribbon });
        }

        /// <summary>
        /// Обновить вид и статью расхода в файле "Семейный бюджет"
        /// </summary>
        private void ribbon_btnFillRashodClicked()
        {
            if (
                (this.Application != null) && 
                (this.Application.ActiveWorkbook != null)
                )
            {
                // имя файла
                string filename = this.Application.ActiveWorkbook.Name;

                // проверяем, что это нужный файл
                if (
                    (! string.IsNullOrWhiteSpace(filename)) && 
                    filename.ToLower().Contains("семейный бюджет")
                    )
                {
                    // заполняем список соответствия операции и вида и статьи расхода
                    List<ItemOutcomeLink> itemOutcomeLinks = new List<ItemOutcomeLink>();
                    int NextRowItemOutcomeSheet = 2;

                    Excel.Worksheet ItemOutcomeSheet = PSVClassLibrary.MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, "Статьи расхода", sh => { });
                    if (ItemOutcomeSheet != null)
                    {
                        // если есть лист "Статьи расхода"

                        // перебираем строки начиная с 2-й
                        while (!string.IsNullOrWhiteSpace(ItemOutcomeSheet.Cells[NextRowItemOutcomeSheet, 1].Text))
                        {
                            itemOutcomeLinks.Add(new ItemOutcomeLink
                            {
                                Oper = ItemOutcomeSheet.Cells[NextRowItemOutcomeSheet, 1].Text.Trim(),
                                KindOutcome = ItemOutcomeSheet.Cells[NextRowItemOutcomeSheet, 2].Text.Trim(),
                                ItemOutcome = ItemOutcomeSheet.Cells[NextRowItemOutcomeSheet, 3].Text.Trim()
                            });
                           
                            NextRowItemOutcomeSheet++;
                        }

                    }

                    Excel.Worksheet sheet = this.Application.ActiveWorkbook.ActiveSheet;

                    // проверим имя текущего листа (должен быть числом в диапазоне от 2024 до 2099)
                    if (
                        (sheet != null) &&
                        int.TryParse(sheet.Name, out int i) &&
                        (i >= 2024) &&
                        (i < 2099)
                        )
                    {
                        /* в строке 3 - заголовки
                         * ищем колонки с заголовками "Операция", "Дата", "Доход", "Расход", "Вид расхода", "Статья расхода", "Месяц" 
                         */

                        int _col = 1;
                        string _title = sheet.Cells[3, _col].Text;
                        _title = _title.ToLower().Trim();

                        int colOper = 0;
                        int colData = 0;
                        int colIncome = 0;
                        int colOutcome = 0;
                        int colKindOutcome = 0;
                        int colItemOutcome = 0;
                        int colMonth = 0;

                        while (!string.IsNullOrWhiteSpace(_title))
                        {
                            if (_title == "операция")
                            {
                                colOper = _col;
                            }

                            if (_title == "дата")
                            {
                                colData = _col;
                            }

                            if (_title == "доход")
                            {
                                colIncome = _col;
                            }

                            if (_title == "расход")
                            {
                                colOutcome = _col;
                            }

                            if (_title == "вид расхода")
                            {
                                colKindOutcome = _col;
                            }

                            if (_title == "статья расхода")
                            {
                                colItemOutcome = _col;
                            }

                            if (_title == "месяц")
                            {
                                colMonth = _col;
                            }

                            _col++;
                            _title = sheet.Cells[3, _col].Text;
                            _title = _title.ToLower().Trim();
                        }

                        // проверка наличия всех нужных столбцов
                        if (
                            (colOper > 0) &&
                            (colData > 0) &&
                            (colIncome > 0) &&
                            (colOutcome > 0) &&
                            (colKindOutcome > 0) &&
                            (colItemOutcome > 0) &&
                            (colMonth > 0) 
                            )
                        {
                            // перебираем строки начиная с 4-й, у которых заполнена колонка "Дата"
                            int _row = 4;

                            while (! string.IsNullOrWhiteSpace(sheet.Cells[_row, colData].Text))
                            {
                                // проверяем колонку "Доход"
                                if (string.IsNullOrWhiteSpace(sheet.Cells[_row, colIncome].Text))
                                {
                                    sheet.Cells[_row, colIncome].FormulaR1C1 = $"=IF(SUM(RC[-16]:RC[-4])>0,SUM(RC[-16]:RC[-4]),0)";
                                }

                                // проверяем колонку "Расход"
                                if (string.IsNullOrWhiteSpace(sheet.Cells[_row, colOutcome].Text))
                                {
                                    sheet.Cells[_row, colOutcome].FormulaR1C1 = $"=IF(SUM(RC[-17]:RC[-5])<0,-SUM(RC[-17]:RC[-5]),0)";
                                }

                                // проверяем колонку "Месяц"
                                if (string.IsNullOrWhiteSpace(sheet.Cells[_row, colMonth].Text))
                                {
                                    sheet.Cells[_row, colMonth].FormulaR1C1 = $"=CHOOSE(MONTH(RC[-21]), \"Январь\", \"Февраль\", \"Март\", \"Апрель\", \"Май\", \"Июнь\", \"Июль\", \"Август\", \"Сентябрь\", \"Октябрь\", \"Ноябрь\", \"Декабрь\")";
                                }

                                string Oper = sheet.Cells[_row, colOper].Text;
                                Oper = Oper.Trim();

                                if (! string.IsNullOrWhiteSpace(Oper)) 
                                {
                                    // если заполнена "Операция"

                                    // ищем соответствие в справочнике
                                    var link = itemOutcomeLinks.Where(x => x.Oper.ToLower() == Oper.ToLower()).FirstOrDefault();
                                    if (link != null)
                                    {
                                        // соответствие найдено
                                        string KindOutcome = sheet.Cells[_row, colKindOutcome].Text;
                                        KindOutcome = KindOutcome.Trim();
                                        string ItemOutcome = sheet.Cells[_row, colItemOutcome].Text;
                                        ItemOutcome = ItemOutcome.Trim();


                                        if (string.IsNullOrWhiteSpace(KindOutcome) &&
                                            (!string.IsNullOrWhiteSpace(link.KindOutcome)))
                                        {
                                            // Если пустой "Вид расхода" 
                                            sheet.Cells[_row, colKindOutcome] = link.KindOutcome.Trim();
                                            KindOutcome = link.KindOutcome.Trim();
                                        }


                                        if (string.IsNullOrWhiteSpace(ItemOutcome) &&
                                            (!string.IsNullOrWhiteSpace(link.ItemOutcome)) &&
                                            (KindOutcome.ToLower() == link.KindOutcome.ToLower())
                                            )
                                        {
                                            // Если пустая "Статья расхода" 
                                            sheet.Cells[_row, colItemOutcome] = link.ItemOutcome;
                                            ItemOutcome = link.ItemOutcome.Trim();
                                        }
                                    }
                                    else
                                    {
                                        // соответствие не найдено - добавим в справочник для будущего дозаполнения
                                        ItemOutcomeSheet.Cells[NextRowItemOutcomeSheet, 1] = Oper;
                                        NextRowItemOutcomeSheet++;
                                        itemOutcomeLinks.Add(new ItemOutcomeLink { Oper = Oper });
                                    }
                                }
                                _row++;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не хватает колонок");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выберите лист с именем в диапазоне от 2022 до 2099");
                    }
                }
                else
                {
                    MessageBox.Show("Это не файл Семейный бюджет");
                }
            }

        }

        /// <summary>
        /// Банки
        /// </summary>
        private enum BankType { UNKNOWN, TINKOFF, SBERBANK, ALFABANK };

        /// <summary>
        /// С даты
        /// </summary>
        DateTime fromDate = DateTime.MinValue;

        /// <summary>
        /// По дату
        /// </summary>
        DateTime toDate = DateTime.MinValue;

        /// <summary>
        /// последняя выбранная карта
        /// </summary>
        int lastcolCard = 0;

        /// <summary>
        /// Загрузить excel из банка
        /// </summary>
        private void LoadBankExcel()
        {
            BankType bankType = BankType.UNKNOWN;

            if (
                (this.Application != null) &&
                (this.Application.ActiveWorkbook != null) &&
                (this.Application.ActiveWorkbook.ActiveSheet != null)
            )
            {
                if (this.Application.ActiveCell == null)
                {
                    MessageBox.Show("Надо выбрать заголовок столбца (в 3-ей строке), в который планируется загрузить данные");
                    return;
                }

                // имя файла
                string filename = this.Application.ActiveWorkbook.Name;

                // проверяем, что это нужный файл
                if (
                    (!string.IsNullOrWhiteSpace(filename)) &&
                    filename.ToLower().Contains("семейный бюджет")
                   )
                {
                    Excel.Worksheet sheet = this.Application.ActiveWorkbook.ActiveSheet;
                    var cell = this.Application.ActiveCell;

                    // проверим имя текущего листа (должен быть числом в диапазоне от 2024 до 2099)
                    if (
                        (sheet != null) &&
                        int.TryParse(sheet.Name, out int i) &&
                        (i >= 2024) &&
                        (i < 2099)
                        )
                    {
                        /* в строке 3 - заголовки
                        * ищем колонки с заголовками "Операция", "Дата", "Вклад в банке" и выбранный столбец для загрузки сумм
                        */

                        int _col = 1;
                        string _title = sheet.Cells[3, _col].Text;
                        _title = _title.ToLower().Trim();

                        int colOper = 0;
                        int colDate = 0;
                        int colBank = 0;
                        int colCard = 0;
                        int colCNY = 0;
                        List<string> cards = new List<string>();

                        while (!string.IsNullOrWhiteSpace(_title))
                        {
                            if (_title == "операция")
                            {
                                colOper = _col;
                            }

                            if (_title == "дата")
                            {
                                colDate = _col;
                            }

                            if (_title == "вклад в банке")
                            {
                                colBank = _col;
                            }

                            if (_title == "валюта, юани")
                            {
                                colCNY = _col;
                            }

                            if (
                                (colDate > 0) &&
                                (_col > colDate) &&
                                (_col != colBank)
                            )
                            {
                                cards.Add(_title);
                            }

                            _col++;
                            _title = sheet.Cells[3, _col].Text;
                            _title = _title.ToLower().Trim();
                        }

                        string selectedcell = cell.Text.ToLower();

                        // выбираем карту, для которой загружаем excel
                        if (cards.Contains(selectedcell))
                        {
                            colCard = colDate + cards.IndexOf(selectedcell) + 1;
                        }

                        if (colCard == 0)
                        {
                            MessageBox.Show("Надо выбрать заголовок столбца (в 3-ей строке), в который планируется загрузить данные");
                            return;
                        }

                        if (selectedcell.Contains("tinkoff"))
                        {
                            bankType = BankType.TINKOFF;
                        } 
                        else if (selectedcell.Contains("сбербанк"))
                        {
                            bankType = BankType.SBERBANK;
                        }
                        else if (selectedcell.Contains("альфа"))
                        {
                            bankType = BankType.ALFABANK;
                        }
                        else
                        {
                            MessageBox.Show("Загрузка Excel реализована только для Tinkoff, Сбербанк, Альфа-Банк");
                            return;
                        }

                        // перебираем строки начиная с 4-й, у которых заполнена колонка "Дата"
                        int _row = 4;
                        string value = sheet.Cells[_row, colOper].Text;

                        // ищем первую пустую строку
                        while (!string.IsNullOrWhiteSpace(value))
                        {
                            _row++;
                            value = sheet.Cells[_row, colOper].Text;

                            if (value == "ВСЕГО")
                            {
                                MessageBox.Show("Надо добавить пустые строки для заполнения");
                                return;
                            }
                        }

                        // идем в обратном порядке по колонке, куда загружаем суммы и ищем первую не пустую строку
                        if ( (fromDate == DateTime.MinValue) || (toDate == DateTime.MinValue) || (lastcolCard != colCard))
                        {
                            DateTime lastDate;
                            int _lastrow = _row;
                            value = sheet.Cells[_lastrow, colCard].Text;
                            string _lastdate = sheet.Cells[_lastrow, colDate].Text;

                            while (string.IsNullOrWhiteSpace(value))
                            {
                                _lastrow--;
                                value = sheet.Cells[_lastrow, colCard].Text;
                                _lastdate = sheet.Cells[_lastrow, colDate].Text;
                            }

                            if (_lastdate == "Дата")
                            {
                                _lastdate = sheet.Cells[_lastrow - 1, colDate].Text;
                                DateTime.TryParse(_lastdate, out lastDate);
                                lastDate = lastDate.AddDays(-1);
                            }
                            else
                            {
                                DateTime.TryParse(_lastdate, out lastDate);
                            }

                            // Выбрать диапазон дат
                            fromDate = lastDate.AddDays(1);
                            toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            lastcolCard = colCard;
                        }

                        FormAskDates dlg1 = new FormAskDates();
                        dlg1.dtFrom.Value = fromDate;
                        dlg1.dtTo.Value = toDate;
                        if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            fromDate = dlg1.dtFrom.Value;
                            toDate = dlg1.dtTo.Value;
                        }
                        else
                        {
                            return;
                        }

                        // выбор файла
                        string xlsfile = "";
                        /*if (bankType == BankType.TINKOFF)
                        {
                            xlsfile = Utilities.OpenExcelDialog("", ".xls", "(*.xls)|*.xls|(*.xlsx)|*.xlsx|Все файлы (*.*)|*.*");
                        }
                        else 
                        {*/
                            xlsfile = Utilities.OpenExcelDialog("", ".xlsx", "(*.xlsx)|*.xlsx|(*.xls)|*.xls|Все файлы (*.*)|*.*");
                        //}

                        if (
                            (!string.IsNullOrWhiteSpace(xlsfile)) &&
                            File.Exists(xlsfile)
                           )
                        {
                            // загружаем 1 лист выбранного excel-файла
                            var xls = Utilities.LoadExcel(xlsfile, 1);

                            if (
                                (xls != null) &&
                                (xls.Columns.Count > 0) &&
                                (xls.Rows.Count > 0)
                                )
                            {
                                for (int cnt_i = 0; cnt_i < xls.Rows.Count; cnt_i++)
                                {
                                    // перебираем строки загружаемого файла

                                    // предыдущая строка
                                    DataRow prev_row = null;
                                    if (cnt_i - 1 >= 0)
                                    {
                                        prev_row = xls.Rows[cnt_i - 1];
                                    }

                                    // текущая строка
                                    DataRow row = xls.Rows[cnt_i];

                                    // следующая строка
                                    DataRow next_row = null;
                                    if (cnt_i + 1 < xls.Rows.Count)
                                    {
                                        next_row = xls.Rows[cnt_i + 1];
                                    }

                                    // определяем значения, в зависимости от банка
                                    if (
                                        (bankType == BankType.TINKOFF) &&
                                        (xls.Columns.Count >= 12) &&
                                        (row[7].ToString().ToUpper() == "\"ОК\"" || row[7].ToString().ToUpper() == "ОК")
                                    )
                                    {
                                        string txtDate = row[2].ToString();
                                        string txtSumma = row[3].ToString();
                                        string txtOper = row[8].ToString() + " " + row[9].ToString() + " " + row[11].ToString();

                                        // нормализация значений
                                        txtOper = txtOper.Trim();
                                        if (string.IsNullOrWhiteSpace(txtOper))
                                        {
                                            break;
                                        }

                                        if (
                                            (row[11].ToString() == "Между своими счетами") &&
                                            (next_row != null) &&
                                            (next_row[11].ToString() == row[11].ToString()) &&
                                            double.TryParse(row[3].ToString(), out double sum1) &&
                                            double.TryParse(next_row[3].ToString(), out double next_sum) &&
                                            (sum1 + next_sum == 0)
                                        )
                                        {
                                            txtSumma = "0";
                                        }
                                        else if (
                                            (row[11].ToString() == "Между своими счетами") &&
                                            (prev_row != null) &&
                                            (prev_row[11].ToString() == row[11].ToString()) &&
                                            double.TryParse(row[3].ToString(), out double sum2) &&
                                            double.TryParse(prev_row[3].ToString(), out double prev_sum) &&
                                            (sum2 + prev_sum == 0)
                                        )
                                        {
                                            txtSumma = "0";
                                        }

                                        Double.TryParse(txtSumma, out double sum);

                                        txtDate = txtDate.Split(new char[] { ',', ' ' })[0];

                                        DateTime.TryParse(txtDate, out DateTime date);

                                        if ((date >= fromDate) && (date < toDate.AddDays(1)))
                                        {
                                            // надо добавить новую строку
                                            Range line = (Range)sheet.Rows[_row];
                                            line.Insert();

                                            // заполняем текущую строку
                                            sheet.Cells[_row, colOper] = txtOper;
                                            sheet.Cells[_row, colDate] = date.ToOADate();
                                            if (sum != 0) sheet.Cells[_row, colCard] = sum;

                                            // следующая строка
                                            _row++;
                                        }
                                    }
                                    else if (
                                        (bankType == BankType.ALFABANK) &&
                                        (xls.Columns.Count >= 13)
                                    )
                                    {
                                        string txtDate = row[0].ToString();

                                        string txtSummaRUR = "";
                                        string txtSummaCNY = "";

                                        if (row[8].ToString() == "RUR")
                                        {
                                            txtSummaRUR = row[7].ToString();
                                            if (row[12].ToString() != "Пополнение")
                                            {
                                                txtSummaRUR = "-" + txtSummaRUR;
                                            }
                                        }
                                        else
                                        {
                                            txtSummaCNY = row[7].ToString();
                                            if (row[12].ToString() != "Пополнение")
                                            {
                                                txtSummaCNY = "-" + txtSummaCNY;
                                            }
                                        }

                                        string txtOper = row[6].ToString();

                                        // нормализация значений
                                        txtOper = txtOper.Trim();
                                        if (string.IsNullOrWhiteSpace(txtOper))
                                        {
                                            break;
                                        }

                                        Double.TryParse(txtSummaRUR, out double sumRUR);
                                        Double.TryParse(txtSummaCNY, out double sumCNY);

                                        txtDate = txtDate.Split(new char[] { ',', ' ' })[0];

                                        DateTime.TryParse(txtDate, out DateTime date);

                                        if ((date >= fromDate) && (date < toDate.AddDays(1)))
                                        {
                                            // надо добавить новую строку
                                            Range line = (Range)sheet.Rows[_row];
                                            line.Insert();

                                            // заполняем текущую строку
                                            sheet.Cells[_row, colOper] = txtOper;
                                            sheet.Cells[_row, colDate] = date.ToOADate();
                                            if (sumRUR != 0) sheet.Cells[_row, colCard] = sumRUR;
                                            if (sumCNY != 0) sheet.Cells[_row, colCNY] = sumCNY;

                                            // следующая строка
                                            _row++;
                                        }
                                    }
                                    else if(
                                        (bankType == BankType.SBERBANK) &&
                                        (xls.Columns.Count >= 9) &&
                                        (row[8].ToString() == "Активная")
                                    )
                                    {
                                        string txtDate = row[1].ToString();
                                        string txtSumma = row[6].ToString().Replace(" ","").Trim();
                                        string txtZnak = row[2].ToString().Trim();
                                        string txtOper = row[3].ToString().Trim();
                                        string txtOper1 = row[7].ToString().Trim();
                                        if (!string.IsNullOrWhiteSpace(txtOper1))
                                        {
                                            txtOper += " " + txtOper1;
                                        }

                                        // нормализация значений
                                        txtDate = txtDate
                                            .Replace(" янв. ", ".01.")
                                            .Replace(" января ", ".01.")
                                            .Replace(" январь ", ".01.")
                                            .Replace(" фев. ", ".02.")
                                            .Replace(" февраля ", ".02.")
                                            .Replace(" февраль ", ".02.")
                                            .Replace(" мар. ", ".03.")
                                            .Replace(" марта ", ".03.")
                                            .Replace(" март ", ".03.")
                                            .Replace(" апр. ", ".04.")
                                            .Replace(" апреля ", ".04.")
                                            .Replace(" апрель ", ".04.")
                                            .Replace(" мая ", ".05.")
                                            .Replace(" май ", ".05.")
                                            .Replace(" июн. ", ".06.")
                                            .Replace(" июня ", ".06.")
                                            .Replace(" июнь ", ".06.")
                                            .Replace(" июл. ", ".07.")
                                            .Replace(" июля ", ".07.")
                                            .Replace(" июль ", ".07.")
                                            .Replace(" авг. ", ".08.")
                                            .Replace(" августа ", ".08.")
                                            .Replace(" август ", ".08.")
                                            .Replace(" сент. ", ".09.")
                                            .Replace(" сентября ",".09.")
                                            .Replace(" сентябрь ", ".09.")
                                            .Replace(" окт. ", ".10.")
                                            .Replace(" октября ", ".10.")
                                            .Replace(" октябрь ", ".10.")
                                            .Replace(" нояб. ", ".11.")
                                            .Replace(" ноября ", ".11.")
                                            .Replace(" ноябрь ", ".11.")
                                            .Replace(" дек. ", ".12.")
                                            .Replace(" декабря ", ".12.")
                                            .Replace(" декабрь ", ".12.")
                                            .Split(new char[] { ',' })[0]
                                            .Trim();
                                        
                                        // пропускаем "лишние строки"
                                        if (!string.IsNullOrWhiteSpace(txtSumma))
                                        {

                                            if (
                                                txtZnak == "Расходы" ||
                                                txtZnak == "Списания"
                                                )
                                            {
                                                txtSumma = "-" + txtSumma;
                                            }

                                            DateTime.TryParse(txtDate, out DateTime date);

                                            Double.TryParse(txtSumma, out double sum);

                                            if ((date >= fromDate) && (date < toDate.AddDays(1)))
                                            {
                                                // надо добавить новую строку
                                                Range line = (Range)sheet.Rows[_row];
                                                line.Insert();

                                                // заполняем текущую строку
                                                sheet.Cells[_row, colOper] = txtOper;
                                                sheet.Cells[_row, colDate] = date.ToOADate();
                                                if (sum != 0) sheet.Cells[_row, colCard] = sum;

                                                // следующая строка
                                                _row++;
                                            }
                                        }
                                    }
                                }
                                MessageBox.Show("Файл загружен");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Загрузить excel из Тинькофф
        /// </summary>
        private void ribbon_btnLoadBankExcelClicked()
        {
            LoadBankExcel();
        }

        /// <summary>
        /// Загрузить excel из Сбербанка
        /// </summary>
        private void ribbon_btnLoadSberbankClicked()
        {
            LoadBankExcel();
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
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


    public class ItemOutcomeLink
    {
        public string Oper { get; set; }
        public string KindOutcome { get; set; }
        public string ItemOutcome { get; set; }
    }
}
