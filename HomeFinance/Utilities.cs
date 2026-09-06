// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

namespace HomeFinance
{
    /// <summary>
    /// Вспомогательные функции
    /// </summary>
    public static class Utilities
    {

        // -------------------------------------------------------------------------------------------------------
        /// <summary>Диалоговое окно для открытия файла Excel</summary>
        /// <param name="pathname">Путь к начальному каталогу</param>
        /// <param name="defaultExt">Расширение по умолчанию</param>
        /// <param name="filter">Список расширений</param>
        /// <returns>Итоговое имя файла</returns>
        public static string OpenExcelDialog(string pathname, string defaultExt = ".xlsx", string filter = "(*.xls)|*.xls|(*.xlsx)|*.xlsx|Все файлы (*.*)|*.*")
        {
            return OpenFileDialog(pathname, defaultExt, filter);
        }

        // -------------------------------------------------------------------------------------------------------
        /// <summary>Диалоговое окно для открытия файла</summary>
        /// <param name="pathname">Путь к начальному каталогу</param>
        /// <param name="defaultExt">Расширение по умолчанию</param>
        /// <param name="filter">Список расширений</param>
        /// <returns>Итоговое имя файла</returns>
        public static string OpenFileDialog(string pathname, string defaultExt = ".sql", string filter = "(*.sql)|*.sql|Все файлы (*.*)|*.*")
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = pathname;
            dlg.DefaultExt = defaultExt; // Default file extension
            dlg.Filter = filter; // Filter files by extension
            dlg.CheckFileExists = true;

            var result = dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filename = dlg.FileName;

                if (File.Exists(filename))
                {
                    return filename;
                }
            }

            return "";
        }

        // -------------------------------------------------------------------------------------------------------
        /// <summary>Считать страницу excel в DataTable</summary>
        /// <param name="xlsFile">Полное имя файла</param>
        /// <param name="sheetNum">Номер страницы в книге</param>
        public static System.Data.DataTable LoadExcel(string xlsFile, int sheetNum)
        {
            // целевая таблица
            System.Data.DataTable dataTable = null;

            // считываем excel-таблицу в DataSet
            System.Data.DataTable Excel = new System.Data.DataTable();

            using (var stream = File.Open(xlsFile, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;
                reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };

                var dataSet = reader.AsDataSet(conf);

                // Now you can get data from each sheet by its index or its "name"  
                if (
                    (dataSet != null) &&
                    (dataSet.Tables.Count >= sheetNum)
                    )
                {
                    Excel = dataSet.Tables[sheetNum - 1];
                }
            }

            if (
                (Excel == null) ||
                (Excel.Rows.Count < 1) ||
                (Excel.Columns.Count < 1)
                )
            {
                MessageBox.Show("Excel-файл " + xlsFile + " не был прочитан успешно или он пустой) !");
                dataTable = null;
                return dataTable;
            }

            dataTable = new System.Data.DataTable();

            try
            {
                foreach (DataColumn col0 in Excel.Columns)
                {
                    string columnname = col0.ColumnName;

                    if (!string.IsNullOrWhiteSpace(columnname))
                    {
                        var column = new DataColumn(columnname);
                        column.DataType = Type.GetType("System.String");
                        dataTable.Columns.Add(column);
                    }
                }

                for (int i = 0; i < Excel.Rows.Count; i++)
                {
                    // новая строка
                    DataRow row = dataTable.NewRow();

                    foreach (DataColumn col in dataTable.Columns)
                    {
                        string value = "";

                        // найти колонку в excel
                        if (Excel.Columns.Contains(col.ColumnName))
                        {
                            DataColumn col0 = Excel.Columns[col.ColumnName];

                            // найти значение в excel
                            value = Excel.Rows[i][col0].ToString().Trim();
                        }

                        // заполним значение
                        if (string.IsNullOrWhiteSpace(value)) row[col] = DBNull.Value;
                        else if (value.Trim().ToLower() == "null") row[col] = DBNull.Value;
                        else row[col] = value;
                    }

                    dataTable.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                dataTable = null;
            }

            if ((dataTable != null) && (dataTable.Columns.Count > 0) && (dataTable.Rows.Count > 0))
            {
                //MessageBox.Show($"Файл {xlsFile} загружен!");
            }
            else
            {
                dataTable = null;
                MessageBox.Show($"Файл {xlsFile} НЕ загружен!");
            }

            return dataTable;
        }

        // -------------------------------------------------------------------------------------------------------
        /// <summary>Считать значение ячейки из excel</summary>
        /// <param name="xlsfile">Полное имя файла</param>
        /// <param name="sheetnum">Номер страницы в книге</param>
        /// <param name="column">Столбец</param>
        /// <param name="row">Столбец</param>
        public static string GetValueExcel(string xlsfile, int sheetnum, int row, int column)
        {
            string result = "";

            Excel.Application excelApp = null;
            Workbook excelWorkBook = null;
            try
            {
                excelApp = new Excel.Application();
                excelApp.Visible = false;
                excelWorkBook = excelApp.Workbooks.Open(xlsfile, null, true);
                Worksheet excelWorkSheet = excelWorkBook.Sheets[sheetnum];

                // считать из excel ячейку
                result = excelWorkSheet.Cells[row, column].Text;
                result = result.Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
                result = "";
            }

            if (excelWorkBook != null) excelWorkBook.Close();
            if (excelApp != null) excelApp.Quit();

            return result;
        }

        // -------------------------------------------------------------------------------------------------------
        /// <summary>Сохранить DataTable в excel</summary>
        /// <param name="dataTable">DataTable</param>
        /// <param name="isSave">=true - сохранить в файл</param>
        /// <param name="path">Полное имя файла</param>
        /// <param name="isShow">=true - оставить excel открытым</param>
        public static void GenerateExcel(System.Data.DataTable dataTable, bool isSave, string path, bool isShow)
        {
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);

            // create a excel app along side with workbook and worksheet and give a name to it
            Excel.Application excelApp = new Excel.Application();
            Workbook excelWorkBook = excelApp.Workbooks.Add();
            _Worksheet xlWorksheet = excelWorkBook.Sheets[1];
            Range xlRange = xlWorksheet.UsedRange;
            foreach (System.Data.DataTable table in dataSet.Tables)
            {
                //Add a new worksheet to workbook with the Datatable name
                Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
                excelWorkSheet.Name = table.TableName;

                // add all the columns
                for (int i = 1; i < table.Columns.Count + 1; i++)
                {
                    excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
                }

                // add all the rows
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    for (int k = 0; k < table.Columns.Count; k++)
                    {
                        excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
                    }
                }
            }
            if (isSave) excelWorkBook.SaveAs(path);

            if (!isShow)
            {
                excelWorkBook.Close();
                excelApp.Quit();
            }
            else excelApp.Visible = true;
        }

    }
}
