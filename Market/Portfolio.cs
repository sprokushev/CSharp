// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
using PSVClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;

namespace Market
{
    public partial class ThisAddIn
    {


        // Добавить покупку
        private void ribbon_btnBrokerBuyClicked()
        {
            BrokerOperation(FormPortfolio.OperType.Buy);
        }

        // Добавить продажу
        private void ribbon_btnBrokerSellClicked()
        {
            BrokerOperation(FormPortfolio.OperType.Sell);
        }


        private void BrokerOperation(FormPortfolio.OperType operation)
        {
            if (IsBrokerActiveSheet(this.Application.ActiveWorkbook))
            {
                if (this.Application.Selection != null)
                {
                    Excel.Range cells = this.Application.Selection;

                    Excel.Worksheet BrokerSheet = this.Application.ActiveWorkbook.ActiveSheet;

                    string ticker = "";
                    string account = "";
                    string name = "";
                    if (cells.Row > 1)
                    {
                        ticker = BrokerSheet.Cells[cells.Row, 2].Text;
                        account = BrokerSheet.Cells[cells.Row, 1].Text;
                        name = BrokerSheet.Cells[cells.Row, 3].Text;
                    }

                    FormPortfolio dlg1 = new FormPortfolio();
                    dlg1.thisAddIn = this;
                    dlg1.ticker = ticker;
                    dlg1.seektxt = name;
                    dlg1.account = account;
                    dlg1.operation = operation;

                    if ((dlg1.ShowDialog() == DialogResult.OK) && (dlg1.count > 0))
                    {
                        // добавляем строку в excel
                        int max_rows = BrokerSheet.UsedRange.Rows.Count;
                        int max_columns = BrokerSheet.UsedRange.Columns.Count;
                        
                        max_rows++;
                        
                        BrokerSheet.Cells[max_rows, 1] = dlg1.account;
                        BrokerSheet.Cells[max_rows, 2] = dlg1.ticker;
                        BrokerSheet.Cells[max_rows, 3] = dlg1.name;
                        BrokerSheet.Cells[max_rows, 4] = dlg1.InstrumentTypeName;

                        switch (dlg1.operation)
                        {
                            case FormPortfolio.OperType.Buy:
                                BrokerSheet.Cells[max_rows, 5] = "Покупка";
                                BrokerSheet.Cells[max_rows, 7] = dlg1.count;
                                BrokerSheet.Cells[max_rows, 11] = dlg1.summa;
                                break;
                            case FormPortfolio.OperType.PlanBuy:
                                BrokerSheet.Cells[max_rows, 5] = "ПЛАН";
                                BrokerSheet.Cells[max_rows, 7] = dlg1.count;
                                BrokerSheet.Cells[max_rows, 11] = dlg1.summa;
                                break;
                            case FormPortfolio.OperType.Sell:
                            default:
                                BrokerSheet.Cells[max_rows, 5] = "Продажа";
                                BrokerSheet.Cells[max_rows, 7] = -dlg1.count;
                                BrokerSheet.Cells[max_rows, 11] = -dlg1.summa;
                                break;
                        }

                        BrokerSheet.Cells[max_rows, 6] = DateTime.Today;
                                                
                        if (dlg1.nominal > 0)
                            BrokerSheet.Cells[max_rows, 8] = dlg1.nominal;
                        
                        BrokerSheet.Cells[max_rows, 9] = dlg1.currency;
                        BrokerSheet.Cells[max_rows, 10].FormulaR1C1 = BrokerSheet.Cells[max_rows - 1, 10].FormulaR1C1;
                        BrokerSheet.Cells[max_rows, 13] = dlg1.price;
                        BrokerSheet.Cells[max_rows, 14].FormulaR1C1 = BrokerSheet.Cells[max_rows - 1, 14].FormulaR1C1;
                        BrokerSheet.Cells[max_rows, 15].FormulaR1C1 = BrokerSheet.Cells[max_rows - 1, 15].FormulaR1C1;
                        BrokerSheet.Cells[max_rows, 16].FormulaR1C1 = BrokerSheet.Cells[max_rows - 1, 16].FormulaR1C1;
                        BrokerSheet.Cells[max_rows, 17].FormulaR1C1 = BrokerSheet.Cells[max_rows - 1, 17].FormulaR1C1;
                        BrokerSheet.Cells[max_rows, 18].FormulaR1C1 = BrokerSheet.Cells[max_rows - 1, 18].FormulaR1C1;
                        BrokerSheet.Cells[max_rows, 19].FormulaR1C1 = BrokerSheet.Cells[max_rows - 1, 19].FormulaR1C1;

                        MSExcel.RangeBold(BrokerSheet.Range[BrokerSheet.Cells[max_rows, 1], BrokerSheet.Cells[max_rows, max_columns]], false);
                        MSExcel.RangeBorder(BrokerSheet.Range[BrokerSheet.Cells[max_rows, 1], BrokerSheet.Cells[max_rows, max_columns]]);

                        dlg1.Dispose();
                    }
                }
            }
        }




        private bool IsBrokerActiveSheet(Excel.Workbook wBook)
        {
            // проверяем имя файла
            string filename = wBook.Name;
            bool checkFile = filename.ToLower().Contains(InvestFileName.ToLower());

            // проверяем имя листа
            string sheetname = wBook.ActiveSheet.Name;
            bool checkBroker = false;

            foreach (var item in BrokersName)
            {
                if (sheetname.ToLower() == item.ToLower())
                {
                    checkBroker = true;
                    break;
                }
            }

            return (checkFile && checkBroker);
        }

                                    

        private Office.CommandBar GetRowContextMenu()
        {
            return this.Application.CommandBars["Row"];
        }

        private Office.CommandBar GetCellContextMenu()
        {
            return this.Application.CommandBars["Cell"];
        }

        private void ResetCellMenu()
        {
            GetRowContextMenu().Reset();
            GetCellContextMenu().Reset();
        }


        private void Application_SheetBeforeRightClick(object Sh, Excel.Range Target, ref bool Cancel)
        {
            // reset the cell context menu back to the default
            ResetCellMenu();

            if (IsBrokerActiveSheet(this.Application.ActiveWorkbook))
            {
                // добавляем пункт контекстного меню
                if (this.Application.Selection != null)
                {
                    Excel.Range cells = this.Application.Selection;
                    if (cells.Row > 1)
                    AddBrokerMenuItem(this.Application.ActiveWorkbook.ActiveSheet.Cells[cells.Row, 3].Text);
                }
            }

            if (IsDividendActiveSheet(this.Application.ActiveWorkbook))
            {
                // добавляем пункт контекстного меню
                if (this.Application.Selection != null)
                {
                    Excel.Range cells = this.Application.Selection;
                    if (cells.Row > 1)
                        AddDividendMenuItem(this.Application.ActiveWorkbook.ActiveSheet.Cells[cells.Row, 3].Text);
                }
            }


        }

        private void AddBrokerMenuItem(string name)
        {
            Office.CommandBarButton MenuItem;

            // контекстное меню для ячейки или диапазона ячеек
            MenuItem = (Office.CommandBarButton)GetCellContextMenu().Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
            MenuItem.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            MenuItem.Caption = $"Продажа {name}";
            MenuItem.FaceId = 375;
            MenuItem.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(BrokerSellMenuItemClick);

            MenuItem = (Office.CommandBarButton)GetCellContextMenu().Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
            MenuItem.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            MenuItem.Caption = $"Покупка {name}";
            MenuItem.FaceId = 374;
            MenuItem.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(BrokerBuyMenuItemClick);

            // контекстное меню для строки
            MenuItem = (Office.CommandBarButton)GetRowContextMenu().Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
            MenuItem.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            MenuItem.Caption = $"Продажа {name}";
            MenuItem.FaceId = 375;
            MenuItem.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(BrokerSellMenuItemClick);

            MenuItem = (Office.CommandBarButton)GetRowContextMenu().Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
            MenuItem.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            MenuItem.Caption = $"Покупка {name}";
            MenuItem.FaceId = 374;
            MenuItem.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(BrokerBuyMenuItemClick);
        }

        void BrokerBuyMenuItemClick(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            ribbon_btnBrokerBuyClicked();
        }

        void BrokerSellMenuItemClick(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            ribbon_btnBrokerSellClicked();
        }

    }
}
