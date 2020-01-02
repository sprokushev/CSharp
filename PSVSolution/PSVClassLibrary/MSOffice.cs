using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System;


namespace PSVClassLibrary
{
    /// <summary>Класс для работы с MS Office</summary>
    public class MSOffice
    {

        /// <summary>Открыть файл Excel поверх текущего приложения</summary>
        /// <param name="FilePath">Каталог и имя файла excel</param>
        public static void OpenExcelWorkbook(string FilePath) => OpenExcelWorkbook(FilePath, "", null);

        /// <summary>Открыть файл Excel и выполнить макрос</summary>
        /// <param name="FilePath">Каталог и имя файла excel</param>
        /// <param name="NameMacro">Имя макроса</param>
        /// <param name="args">массив аргументов макроса</param>
        public static void OpenExcelWorkbook(string FilePath, string NameMacro,  object[] args)
        {
            Excel.Application excel = new Excel.Application();
            Excel.Workbook wb = excel.Workbooks.Open(FilePath);
            excel.Visible = true;
            try
            {
                excel.Run(NameMacro, args);
            }
            catch (Exception e)
            {
            }
            finally
            {
            }
        }

    }
}
