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

        // Добавить купон / дивиденд
        private void ribbon_btnDividendClicked()
        {
            // выбрать лист с дивидендами
            MSExcel.SetExcelSheet(this.Application.ActiveWorkbook, ExcelDividendName);

            if (IsDividendActiveSheet(this.Application.ActiveWorkbook))
            {
                if (this.Application.Selection != null)
                {
                    Excel.Range cells = this.Application.Selection;

                    Excel.Worksheet DividendSheet = this.Application.ActiveWorkbook.ActiveSheet;

                    string ticker = "";
                    string account = "";
                    string name = "";
                    
                    if (cells.Row > 1)
                    {
                        ticker = DividendSheet.Cells[cells.Row, 2].Text;
                        account = DividendSheet.Cells[cells.Row, 1].Text;
                        name = DividendSheet.Cells[cells.Row, 3].Text;
                    }

                    FormDividend dlg1 = new FormDividend();
                    dlg1.thisAddIn = this;
                    dlg1.ticker = ticker;
                    dlg1.seektxt = name;
                    dlg1.account = account;

                    if ((dlg1.ShowDialog() == DialogResult.OK) && (dlg1.summa > 0))
                    {
                        // добавляем строку в excel
                        int max_rows = DividendSheet.UsedRange.Rows.Count;
                        int max_columns = DividendSheet.UsedRange.Columns.Count;

                        max_rows++;

                        DividendSheet.Cells[max_rows, 1] = dlg1.account;
                        DividendSheet.Cells[max_rows, 2] = dlg1.ticker;
                        DividendSheet.Cells[max_rows, 3] = dlg1.name;
                        if (dlg1.operation == FormDividend.DividendType.Cupon)
                            DividendSheet.Cells[max_rows, 4] = "Купон";
                        else
                            DividendSheet.Cells[max_rows, 4] = "Дивиденд";
                        DividendSheet.Cells[max_rows, 5] = DateTime.Today;
                        DividendSheet.Cells[max_rows, 6] = dlg1.summa;
                        DividendSheet.Cells[max_rows, 7] = dlg1.currency;
                        DividendSheet.Cells[max_rows, 8].FormulaR1C1 = DividendSheet.Cells[max_rows - 1, 8].FormulaR1C1;
                        DividendSheet.Cells[max_rows, 9].FormulaR1C1 = DividendSheet.Cells[max_rows - 1, 9].FormulaR1C1;

                        MSExcel.RangeBold(DividendSheet.Range[DividendSheet.Cells[max_rows, 1], DividendSheet.Cells[max_rows, max_columns]], false);
                        MSExcel.RangeBorder(DividendSheet.Range[DividendSheet.Cells[max_rows, 1], DividendSheet.Cells[max_rows, max_columns]]);

                        dlg1.Dispose();
                    }
                }
            }
        }


        private bool IsDividendActiveSheet(Excel.Workbook wBook)
        {
            // проверяем имя файла
            string filename = wBook.Name;
            bool checkFile = filename.ToLower().Contains(InvestFileName.ToLower());

            // проверяем имя листа
            string sheetname = wBook.ActiveSheet.Name;
            bool checkDividend = ((sheetname.ToLower() == ExcelDividendName.ToLower()));

            return (checkFile && checkDividend);
        }


        private void AddDividendMenuItem(string name)
        {
            Office.CommandBarButton MenuItem;

            // контекстное меню для ячейки или диапазона ячеек
            MenuItem = (Office.CommandBarButton)GetCellContextMenu().Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
            MenuItem.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            MenuItem.Caption = $"Дивиденд/купон {name}";
            MenuItem.FaceId = 384;
            MenuItem.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(DividendMenuItemClick);

            // контекстное меню для строки
            MenuItem = (Office.CommandBarButton)GetRowContextMenu().Controls.Add(Office.MsoControlType.msoControlButton, missing, missing, 1, true);
            MenuItem.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            MenuItem.Caption = $"Дивиденд/купон {name}";
            MenuItem.FaceId = 384;
            MenuItem.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(DividendMenuItemClick);

        }

        void DividendMenuItemClick(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            ribbon_btnDividendClicked();
        }

    }

}
