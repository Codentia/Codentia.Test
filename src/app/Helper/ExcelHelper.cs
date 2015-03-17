using System;
using System.Data;
using System.Data.OleDb;

namespace Codentia.Test.Helper
{
    /// <summary>
    /// This class contains methods to help performing unit tests which manipulate Excel (XLS) spreadsheet data.
    /// </summary>
    public static class ExcelHelper
    {
        /// <summary>
        /// Load an excel worksheet (from a spreadsheet), returning the results (all rows, all columns) as a DataTable.
        /// </summary>
        /// <param name="localFilePath">Full or relative path to the spreadsheet to be opened</param>
        /// <param name="sheetName">Name of the work-sheet within the file to load</param>
        /// <param name="firstRowAsHeader">Should the first row of data be returned as column headings</param>
        /// <returns>DataTable of xls sheet</returns>
        public static DataTable LoadXLSSheetToDataTable(string localFilePath, string sheetName, bool firstRowAsHeader)
        {
            DataTable returnData = new DataTable();
            ////string connString = string.Format("Provider=Microsoft.Jet.Oledb.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR={1};IMEX=1;\"", localFilePath.Replace("\\", "\\\\"), firstRowAsHeader ? "YES" : "NO");
            string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR={1};IMEX=1;\"", localFilePath.Replace("\\", "\\\\"), firstRowAsHeader ? "YES" : "NO");
            OleDbConnection conn = new OleDbConnection(connString);
            conn.Open();

            OleDbDataAdapter oleAdapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}$]", sheetName), conn);
            oleAdapter.Fill(returnData);
            oleAdapter.Dispose();
            
            conn.Close();
            conn.Dispose();

            return returnData;
        }        
    }
}

/*
for reference, this snippet loads into a known schema:
 * 
 *             if (schema != null)
            {
                this.importedData = schema.Clone();
                OleDbCommand oleCommand = new OleDbCommand("SELECT * FROM [Sheet1$]");
                oleCommand.Connection = conn;
                OleDbDataReader oleReader = oleCommand.ExecuteReader();

                while (oleReader.Read())
                {
                    DataRow newRow = importedData.NewRow();
                    for (int i = 0; i < oleReader.FieldCount; i++)
                    {
                        newRow[i] = oleReader.GetValue(i);
                    }
                    importedData.Rows.Add(newRow);
                }
            }
*/