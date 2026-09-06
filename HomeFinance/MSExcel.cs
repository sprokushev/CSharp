// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel=Microsoft.Office.Interop.Excel;

namespace PSVClassLibrary
{
    public static class MSExcel
    {
        public enum Border
        {
            Left = 1,
            Right = 2,
            Top = 3,
            Bottom = 4
        }

        public static void RangeAutoFit(dynamic Range)
        {
            Range.EntireColumn.AutoFit();
        }

        public static void RangeAutoFilter(dynamic Range)
        {
            Range.AutoFilter();
        }


        public static void RangeBold(dynamic Range, bool value)
        {
            Range.Font.Bold = value;
        }

        public static void RangeBorder(dynamic Range, Excel.XlBorderWeight border = Excel.XlBorderWeight.xlThin)
        {
            Range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = border;
            Range.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = border;
            Range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = border;
            Range.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = border;
            Range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].Weight = border;
            Range.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = border;
        }

        public static void RangeWrapText(dynamic Range)
        {
            Range.WrapText = true;
        }

        public static void RangeHorizontalAlignment(dynamic Range, Excel.XlHAlign Alignment)
        {
            Range.HorizontalAlignment = Alignment;
        }

        public static void RangeVerticalAlignment(dynamic Range, Excel.XlVAlign Alignment)
        {
            Range.VerticalAlignment = Alignment;
        }

        // Format = "# ##0,00"
        // Format = "0,0000"
        // Format = "Основной"
        // Format = "ДД.ММ.ГГГГ"
        public static void RangeNumberFormat(dynamic Range, string Format)
        {
            Range.NumberFormatLocal = Format;
        }


        // возвращаем диапазон именованных ячеек
        public static Excel.Range NamedRange(Excel.Workbook wBook, string Name)
        {

            Excel.Range result = null;

            if ((Name != null) && (Name != "") && 
                (wBook != null) &&  
                (wBook.Names != null) && (wBook.Names.Count > 0))
            {
                foreach (var item in wBook.Names)
                {
                    Excel.Name namedRange = (Excel.Name)item;

                    if (namedRange.Name.ToLower() == Name.ToLower())
                    {
                        result = namedRange.RefersToRange;
                    }
                }

            }

            return result;
        }

        // возвращаем значение диапазона именованных ячеек
        public static T GetNamedRangeValue<T> (Excel.Workbook wBook, string Name)
        {
            Excel.Range rng = NamedRange(wBook, Name);
            try
            {
                if (rng != null)
                {
                    switch (rng.Value)
                    {
                        case string s:
                            return rng.Text;
                        case T t:
                            return rng.Value;
                        default:
                            return default(T);
                    }
                }
            }
            catch
            {
            }
            return default(T);
        }

        // возвращаем значение ячейки
        public static T GetRangeValue<T>(Excel.Range rng)
        {
            try
            {
                switch (rng.Value)
                {
                    case string s:
                        return rng.Text;
                    case T t:
                        return rng.Value;
                    default:
                        return default(T);
                }
            }
            catch
            {
                return default(T);
            }
        }




        // записываем значение в диапазон именованных ячеек
        public static void SetNamedRangeValue<T>(Excel.Workbook wBook, string Name, T value)
        {
            Excel.Range rng = NamedRange(wBook, Name);
            if (rng != null)
            {
                if (value != null) rng.Value2 = value; //-V3111
                else rng.Value2 = "";
            }
        }

        // поиск листа по имени
        public static Excel.Worksheet GetExcelSheet(Excel.Workbook Workbook, string ExcelSheetName, Action<Excel.Worksheet> dopMethod)
        {
            Excel.Worksheet result = null;

            if ((ExcelSheetName != null) && (ExcelSheetName != "") &&
                (Workbook != null)  &&
                (Workbook.Sheets != null) && (Workbook.Sheets.Count > 0))
            {

                foreach (var item in Workbook.Sheets)
                if (item is Excel.Worksheet)
                {
                    Excel.Worksheet sh = (Excel.Worksheet)item;

                    if (sh.Name.ToLower() == ExcelSheetName.ToLower())
                    {
                        result = sh;
                        if (dopMethod !=null) dopMethod(sh);
                        break;
                    }

                }
            }

            return result;
        }


        // выбор листа по имени
        public static void SetExcelSheet(Excel.Workbook Workbook, string ExcelSheetName)
        {
            if ((ExcelSheetName != null) && (ExcelSheetName != "") &&
                (Workbook != null) &&
                (Workbook.Sheets != null) && (Workbook.Sheets.Count > 0))
            {

                foreach (var item in Workbook.Sheets)
                    if (item is Excel.Worksheet)
                    {
                        Excel.Worksheet sh = (Excel.Worksheet)item;

                        if (sh.Name.ToLower() == ExcelSheetName.ToLower())
                        {
                            sh.Activate();
                            break;
                        }

                    }
            }
        }



    }
}
