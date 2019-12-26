using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSVClassLibrary
{
    public class MSOffice
    {

        public static void OpenExcelWorkbook(string FilePath) => OpenExcelWorkbook(FilePath, true, "", null);
        public static void OpenExcelWorkbook(string FilePath, bool IsVisible) => OpenExcelWorkbook(FilePath, IsVisible, "", null);

        public static void OpenExcelWorkbook(string FilePath, bool IsVisible, string NameMacro,  object[] args)
        {
            Application excel = new Application();
            Workbook wb = excel.Workbooks.Open(FilePath);
            excel.Visible = IsVisible;
            try
            {
                excel.Run(NameMacro, args);
            }
            catch (Exception)
            {
            }
        }

    }
}
