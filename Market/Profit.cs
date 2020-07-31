using PSVClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel=Microsoft.Office.Interop.Excel;

namespace Market
{
    public partial class ThisAddIn
    {
        // Обновить все данные, связанные с котировками
        private void ribbon_btnProfitClicked()
        {

            ribbon_btnRefreshAllClicked();

            if ((this.Application != null) && (this.Application.ActiveWorkbook != null))
            {

                Excel.Worksheet ProfitSheet = MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, ExcelProfitName, sh => { });
                Excel.Worksheet TinkoffSheet = MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, ExcelTinkoffName, sh => { });
                Excel.Worksheet HistorySheet = MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, ExcelHistoryName, sh => { });
                Excel.Worksheet BrokerSheet = null;

                if (HistorySheet == null)
                {
                    // лист не найден - добавляем
                    HistorySheet = (Excel.Worksheet)this.Application.ActiveWorkbook.Sheets.Add();
                    HistorySheet.Name = ExcelHistoryName;
                    HistorySheet.Activate();

                    // заголовок
                    int _count = 1;
                    HistorySheet.Cells[_count, 1] = "Ticker";
                    HistorySheet.Cells[_count, 2] = "Date";
                    HistorySheet.Cells[_count, 3] = "IsCurrency";
                    HistorySheet.Cells[_count, 4] = "Count";
                    HistorySheet.Cells[_count, 5] = "Valute";
                    HistorySheet.Cells[_count, 6] = "Price";
                    HistorySheet.Cells[_count, 7] = "Summa";
                    HistorySheet.Cells[_count, 8] = "Dividend";
                    HistorySheet.Cells[_count, 9] = "MarketPrice";
                    HistorySheet.Cells[_count, 10] = "MarketSumma";
                    HistorySheet.Cells[_count, 11] = "Curs";
                }


                if (ProfitSheet != null)
                {
                    Dictionary<string,ProfitTicker> Profits = new Dictionary<string,ProfitTicker>();
                    int max_rows;
                    int max_columns;
                    string ticker;

                    if (this.Application != null)
                        this.Application.StatusBar = $"Собираем портфели с листов данного excel-файла";

                    // портфель Тинькофф
                    if (TinkoffSheet != null)
                    {
                        max_rows = TinkoffSheet.UsedRange.Rows.Count;

                        for (int i = 2; i <= max_rows; i++)
                        {
                            ticker = TinkoffSheet.Cells[i, 2].Text;
                            
                            if ((ticker != null) && (ticker != ""))
                            {
                                double value;

                                ProfitTicker item = null;
                                if (Profits.ContainsKey(ticker))
                                {
                                    item = Profits[ticker];
                                }
                                else
                                {
                                    item = new ProfitTicker();
                                    item.ticker = ticker;
                                    Profits.Add(ticker, item);
                                }

                                item.name = TinkoffSheet.Cells[i, 3].Text;
                                item.InstrumentTypeName = TinkoffSheet.Cells[i, 4].Text;


                                if (TinkoffSheet.Cells[i, 5].Value is double)
                                    value = TinkoffSheet.Cells[i, 5].Value;
                                else
                                    double.TryParse(TinkoffSheet.Cells[i, 5].Text, out value);
                                item.Count += value;

                                item.currency = TinkoffSheet.Cells[i, 6].Text;

                                if (TinkoffSheet.Cells[i, 8].Value is double)
                                    value = TinkoffSheet.Cells[i, 8].Value;
                                else
                                    double.TryParse(TinkoffSheet.Cells[i, 8].Text, out value);
                                item.Summa += value;
                                
                                if (TinkoffSheet.Cells[i, 14].Value is double)
                                    value = TinkoffSheet.Cells[i, 14].Value;
                                else
                                    double.TryParse(TinkoffSheet.Cells[i, 14].Text, out value);
                                item.MarketSumma += value;
                            }
                        }
                    }

                    foreach (var broker in BrokersName)
                    {
                        // находим лист
                        BrokerSheet = MSExcel.GetExcelSheet(this.Application.ActiveWorkbook, broker, sh => { });

                        // портфель брокера
                        if (BrokerSheet != null)
                        {
                            max_rows = BrokerSheet.UsedRange.Rows.Count;

                            for (int i = 2; i <= max_rows; i++)
                            {
                                ticker = BrokerSheet.Cells[i, 2].Text;

                                if ((ticker != null) && (ticker != ""))
                                {
                                    double value;

                                    ProfitTicker item = null;
                                    if (Profits.ContainsKey(ticker))
                                    {
                                        item = Profits[ticker];
                                    }
                                    else
                                    {
                                        item = new ProfitTicker();
                                        item.ticker = ticker;
                                        Profits.Add(ticker, item);
                                    }

                                    item.name = BrokerSheet.Cells[i, 3].Text;
                                    item.InstrumentTypeName = BrokerSheet.Cells[i, 4].Text;

                                    if (BrokerSheet.Cells[i, 7].Value is double)
                                        value = BrokerSheet.Cells[i, 7].Value;
                                    else
                                        double.TryParse(BrokerSheet.Cells[i, 7].Text, out value);
                                    item.Count += value;

                                    if (BrokerSheet.Cells[i, 8].Value is double)
                                        value = BrokerSheet.Cells[i, 8].Value;
                                    else
                                        double.TryParse(BrokerSheet.Cells[i, 8].Text, out value);
                                    item.Nominal = value;

                                    item.currency = BrokerSheet.Cells[i, 9].Text;

                                    if (BrokerSheet.Cells[i, 11].Value is double)
                                        value = BrokerSheet.Cells[i, 11].Value;
                                    else
                                        double.TryParse(BrokerSheet.Cells[i, 11].Text, out value);
                                    item.Summa += value;

                                    if (BrokerSheet.Cells[i, 17].Value is double)
                                        value = BrokerSheet.Cells[i, 17].Value;
                                    else
                                        double.TryParse(BrokerSheet.Cells[i, 17].Text, out value);
                                    item.MarketSumma += value;
                                }
                            }
                        }

                    }

                    ProfitSheet.Activate();

                    max_rows = ProfitSheet.UsedRange.Rows.Count;
                    int count_rows = 1;

                    if (this.Application != null)
                        this.Application.StatusBar = $"Заполняем лист {ExcelProfitName}";

                    // перебираем строки в excel: если есть в портфелях - заполняем, если нет - обнуляем 
                    for (int i=2; i<=max_rows; i++)
                    {
                        ticker = ProfitSheet.Cells[i, 1].Text;
                        if ((ticker == null) || (ticker == "")) break;

                        count_rows++;
                        if ((Profits != null) && (Profits.ContainsKey(ticker)))
                        {
                            Profits[ticker].IsFound = true;
                            FillProfit(ProfitSheet, count_rows, ticker, Profits[ticker], HistorySheet);
                        }
                        else
                        {
                            FillProfit(ProfitSheet, count_rows, ticker, null, HistorySheet);
                        }

                        if (this.Application != null)
                            this.Application.StatusBar = $"Заполняем лист {ExcelProfitName} - {ticker}";

                    }

                    // все, что не найдено - добавляем в конец таблицы
                    if (Profits != null)
                    {
                        foreach (var item in Profits.Values)
                        {
                            if ((!item.IsFound) && (item.Count != 0))
                            {
                                ticker = item.ticker;
                                count_rows++;
                                FillProfit(ProfitSheet, count_rows, ticker, item, HistorySheet);

                                if (this.Application != null)
                                    this.Application.StatusBar = $"Заполняем лист {ExcelProfitName} - {ticker}";
                            }
                        }
                    }

                    // итоги
                    count_rows++;
                    ProfitSheet.Cells[count_rows, 5].FormulaR1C1 = $"=SUBTOTAL(9,R[{2- count_rows}]C:R[-1]C)";
                    ProfitSheet.Cells[count_rows, 17].FormulaR1C1 = $"=SUBTOTAL(9,R[{2 - count_rows}]C:R[-1]C)";

                    max_columns = ProfitSheet.UsedRange.Columns.Count;
                    if (max_rows > count_rows)
                    {
                        ProfitSheet.Range[ProfitSheet.Cells[count_rows+1, 1], ProfitSheet.Cells[max_rows, max_columns]].ClearContents();
                    }
                    max_rows = count_rows;

                    // оформляем
                    MSExcel.RangeBold(ProfitSheet.Range[ProfitSheet.Cells[1, 1], ProfitSheet.Cells[max_rows, max_columns]],false);
                    MSExcel.RangeBold(ProfitSheet.Range[ProfitSheet.Cells[1, 1], ProfitSheet.Cells[1, max_columns]], true);
                    MSExcel.RangeBold(ProfitSheet.Range[ProfitSheet.Cells[max_rows, 1], ProfitSheet.Cells[max_rows, max_columns]], true);
                    MSExcel.RangeWrapText(ProfitSheet.Range[ProfitSheet.Cells[1, 1], ProfitSheet.Cells[1, max_columns]]);
                    MSExcel.RangeBorder(ProfitSheet.Range[ProfitSheet.Cells[1, 1], ProfitSheet.Cells[max_rows, max_columns]]);
                    MSExcel.RangeAutoFilter(ProfitSheet.Cells);
                    MSExcel.RangeVerticalAlignment(ProfitSheet.Cells, Excel.XlVAlign.xlVAlignCenter);


                    // обновляем семейный бюджет
                    string FamilyBudgetFile = MSExcel.GetNamedRangeValue<string>(this.Application.ActiveWorkbook, "FamilyBudgetFile");
                    string FamilyBudgetInvestField = MSExcel.GetNamedRangeValue<string>(this.Application.ActiveWorkbook, "FamilyBudgetInvest");
                    string FamilyBudgetToInvestField = MSExcel.GetNamedRangeValue<string>(this.Application.ActiveWorkbook, "FamilyBudgetToInvest");

                    if ((FamilyBudgetFile != "") && (FamilyBudgetInvestField != "") && (FamilyBudgetInvestField != "") && (File.Exists(FamilyBudgetFile)))
                    {

                        Excel.Workbook curWorkbook = this.Application.ActiveWorkbook;

                        // открываем файл семейного бюджета
                        Excel.Workbook familyWorkbook = this.Application.Workbooks.Open(FamilyBudgetFile);

                        if (familyWorkbook != null)
                        {
                            double value;

                            // считываем затраты на инвестиции
                            value = MSExcel.GetNamedRangeValue<double>(familyWorkbook, FamilyBudgetToInvestField);
                            MSExcel.SetNamedRangeValue<double>(curWorkbook, FamilyBudgetToInvestField, value);

                            if (System.Windows.Forms.MessageBox.Show("Обновить результат инвестиций в семейном бюджете ?", "Внимание!", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                // заполняем результат инвестиций
                                value = MSExcel.GetNamedRangeValue<double>(curWorkbook, FamilyBudgetInvestField);
                                MSExcel.SetNamedRangeValue<double>(familyWorkbook, FamilyBudgetInvestField, value);
                                familyWorkbook.RefreshAll();
                            }

                            familyWorkbook.Save();
                            familyWorkbook.Close();
                        }

                        if (curWorkbook != null) curWorkbook.Activate();
                    }


                }

                this.Application.ActiveWorkbook.RefreshAll();
            }

            if (this.Application != null)
                this.Application.StatusBar = false;

        }

        void FillProfit (Excel.Worksheet ProfitSheet, int Row, string ticker, ProfitTicker item, Excel.Worksheet HistorySheet)
        {
            if ((ProfitSheet != null) && (Row>1) && (ticker != null) && (ticker != ""))
            {
                if ((item == null) || (item.Count == 0))
                {
                    // позиции из "доходности" нет в портфелях или кол-во равно 0 - обнуляем (распродал)
                    ProfitSheet.Cells[Row, 5] = "";
                    ProfitSheet.Cells[Row, 9] = "";
                    ProfitSheet.Cells[Row, 14] = "";
                }
                else
                {
                    ProfitSheet.Cells[Row, 1] = ticker;
                    ProfitSheet.Cells[Row, 2] = item.name;
                    ProfitSheet.Cells[Row, 4] = item.InstrumentTypeName;
                    if (item.Count > 0) 
                        if (item.IsCurrency)
                            ProfitSheet.Cells[Row, 9] = item.Count;
                        else
                            ProfitSheet.Cells[Row, 5] = item.Count;
                    if (item.Nominal > 0) ProfitSheet.Cells[Row, 6] = item.Nominal;
                    ProfitSheet.Cells[Row, 7] = item.currency;
                    if ((item.Summa > 0) && (! item.IsCurrency)) ProfitSheet.Cells[Row, 9] = item.Summa;
                    if (item.MarketSumma > 0) ProfitSheet.Cells[Row, 14] = item.MarketSumma;

                    if (Row>10)
                    {
                        // копируем формулы из предыдущей строки
                        ProfitSheet.Cells[Row, 8].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 8].FormulaR1C1;
                        ProfitSheet.Cells[Row, 10].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 10].FormulaR1C1;
                        ProfitSheet.Cells[Row, 11].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 11].FormulaR1C1;
                        ProfitSheet.Cells[Row, 12].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 12].FormulaR1C1;
                        ProfitSheet.Cells[Row, 13].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 13].FormulaR1C1;
                        ProfitSheet.Cells[Row, 15].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 15].FormulaR1C1;
                        ProfitSheet.Cells[Row, 16].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 16].FormulaR1C1;
                        ProfitSheet.Cells[Row, 17].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 17].FormulaR1C1;
                        ProfitSheet.Cells[Row, 18].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 18].FormulaR1C1;
                        ProfitSheet.Cells[Row, 19].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 19].FormulaR1C1;
                        ProfitSheet.Cells[Row, 20].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 20].FormulaR1C1;
                        ProfitSheet.Cells[Row, 21].FormulaR1C1 = ProfitSheet.Cells[Row - 1, 21].FormulaR1C1;
                    }


                    // дополняем историю
                    if (HistorySheet != null)
                    {
                        int max_rows_history = HistorySheet.UsedRange.Rows.Count;
                        DateTime date = DateTime.Today;
                        int _count = 0;

                        for (int j = 2; j <= max_rows_history; j++)
                        {
                            if (ticker == MSExcel.GetRangeValue<string>(HistorySheet.Cells[j, 1]))
                            if (date == MSExcel.GetRangeValue<DateTime>(HistorySheet.Cells[j, 2]))
                            {
                                _count = j;
                                break;
                            }
                        }

                        if (_count == 0) _count = max_rows_history + 1;

                        if (_count > 1)
                        {
                            HistorySheet.Cells[_count, 1] = ticker;
                            HistorySheet.Cells[_count, 2] = date;
                            if (item.IsCurrency) 
                                HistorySheet.Cells[_count, 3] = 1;
                            else
                                HistorySheet.Cells[_count, 3] = 0;
                            HistorySheet.Cells[_count, 4] = ProfitSheet.Cells[Row, 5];
                            HistorySheet.Cells[_count, 5] = ProfitSheet.Cells[Row, 7];
                            HistorySheet.Cells[_count, 6] = ProfitSheet.Cells[Row, 8];
                            HistorySheet.Cells[_count, 7] = ProfitSheet.Cells[Row, 9];
                            HistorySheet.Cells[_count, 8] = ProfitSheet.Cells[Row, 10];
                            HistorySheet.Cells[_count, 9] = ProfitSheet.Cells[Row, 12];
                            HistorySheet.Cells[_count, 10] = ProfitSheet.Cells[Row, 14];
                            HistorySheet.Cells[_count, 11] = ProfitSheet.Cells[Row, 15];
                        }
                    }
                }
            }
        }


        public class ProfitTicker
        {
            public bool IsFound { get; set; } = false;
            public string ticker { get; set; }
            public string name { get; set; }
            public string InstrumentTypeName { get; set; }
            public double Count { get; set; }
            public double Nominal { get; set; }
            public string currency { get; set; }

            public double Price
            {
                get
                {
                    double value = 0;
                    if (Count != 0) value = Summa / Count;
                    return value;
                }
            }

            public double Summa { get; set; }
           
            public double MarketPrice 
            {
                get
                {
                    double value = 0;
                    if (Count != 0) value = MarketSumma / Count;
                    return value;
                }
            }

            public double MarketSumma { get; set; }

            public bool IsCurrency 
            { 
                get
                {
                    return Currencies.IsValuteByTicker(ticker);
                }
            }
        }
    }
}