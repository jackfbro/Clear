using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Data;
using System.Reflection;

namespace Clear.Utility
{
    /// <summary>
    /// Excel functionality with NPOI compoment buliding jusf for xlsx
    /// </summary>
    public class NPOIExcelHelper
    {
        public static MemoryStream RenderListToExcel<T>(string sheetName, IEnumerable<T> list, string[] headers)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = string.IsNullOrEmpty(sheetName) ? workbook.CreateSheet() : workbook.CreateSheet(sheetName);
            ICellStyle cellStyle = GetBorderStyle(workbook);
            //handing header
            IRow headerRow = sheet.CreateRow(0);
            Type t = typeof(T);
            PropertyInfo[] properties = t.GetProperties();
            if (headers!=null)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = cellStyle;
                }
            }
            else
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i);
                    cell.SetCellValue(properties[i].Name);
                    cell.CellStyle = cellStyle;
                    
                }
            }
            //handing value     
            int rowIndex = 1;
            foreach (T item in list)
            {
                IRow sheetRow = sheet.CreateRow(rowIndex);
                int cellIndex = 0;
                foreach (PropertyInfo p in properties)
                {
                    ICell cell = sheetRow.CreateCell(cellIndex);
                    cell.CellStyle = cellStyle;
                    cell.SetCellValue(Convert.ToString(p.GetValue(item)));
                    cellIndex++;
                }
                rowIndex++;
            }
            MemoryStream ms = new MemoryStream();            
            workbook.Write(ms);
            ms.Flush();
            //ms.Seek(0, SeekOrigin.Begin);
            //ms.Position = 0;
            sheet = null;
            headerRow = null;
            workbook = null;
            return ms;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName">可空默认sheet1</param>
        /// <param name="sourceTable"></param>
        /// <returns></returns>

        public static Stream RenderDataTableToExcel(string sheetName, DataTable sourceTable)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = string.IsNullOrEmpty(sheetName) ? workbook.CreateSheet() : workbook.CreateSheet(sheetName);
            ICellStyle cellStyle = GetBorderStyle(workbook);
            //handing header
            IRow headerRow = sheet.CreateRow(0);
            foreach (DataColumn column in sourceTable.Columns)
            {
                ICell cell = headerRow.CreateCell(column.Ordinal);
                cell.SetCellValue(column.ColumnName);
                cell.CellStyle = cellStyle;
            }
            //handing value
            int rowIndex = 0;
            foreach (DataRow row in sourceTable.Rows)
            {
                IRow sheetRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in sourceTable.Columns)
                {
                    ICell cell = sheetRow.CreateCell(column.Ordinal);
                    cell.CellStyle = cellStyle;
                    cell.SetCellValue(row[column].ToString());
                }
                rowIndex++;
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            sheet = null;
            headerRow = null;
            workbook = null;
            return ms;
        }
        /// <summary>
        /// 保存excel
        /// </summary>
        /// <param name="sheetName">可空默认sheet1</param>
        /// <param name="sourceTable"></param>
        /// <param name="fileName">\文件名\路径</param>
        public static void RenderDataTableToExcel(string sheetName, DataTable sourceTable, string fileName)
        {
            MemoryStream ms = RenderDataTableToExcel(sheetName, sourceTable) as MemoryStream;
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            byte[] data = ms.ToArray();
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            data = null;
        }
        public static DataTable RenderDataTableFromExcel(Stream excelFileStream, string sheetName, int headerRowIndex)
        {
            XSSFWorkbook workbook = new XSSFWorkbook(excelFileStream);
            ISheet sheet = workbook.GetSheet(sheetName);
            DataTable dataTable = new DataTable();
            IRow headerRow = sheet.GetRow(headerRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                dataTable.Columns.Add(column);
            }
            int rowCount = sheet.LastRowNum;
            for (int i = sheet.FirstRowNum + 1; i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dataTable.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    dataRow[j] = row.GetCell(j).ToString();
                }
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return dataTable;
        }
        public static DataTable RenderDataTableFromExcel(Stream excelFileStream, int sheetIndex, int headerRowIndex)
        {
            XSSFWorkbook workbook = new XSSFWorkbook(excelFileStream);
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            DataTable dataTable = new DataTable();
            IRow headerRow = sheet.GetRow(headerRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                dataTable.Columns.Add(column);
            }
            int rowCount = sheet.LastRowNum;
            for (int i = sheet.FirstRowNum + 1; i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dataTable.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    dataRow[j] = row.GetCell(j).ToString();
                }
                dataTable.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return dataTable;
        }
        public static ICellStyle GetBorderStyle(IWorkbook workbook)
        {
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;
            return cellStyle;
        }
    }
}
/* NPOI Help
using NPOI.HSSF;//Excel 2003
using NPOI.XSSF;//Excel 2007
using NPOI.XWPF;//Word 2007
using NPOI.DDF;//NPOI 繪圖區讀寫函式庫
using NPOI.HPSF;//NPOI 文件摘要資訊讀寫函式庫
using NPOI.Util;//NPOI 工具函式庫
using NPOI.POIFS;//NPOI OLE 格式存取函式庫
ICSharpCode.SharpZipLib.DLL 檔案壓縮函式庫
*/