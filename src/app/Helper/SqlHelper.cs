using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using Codentia.Common.Data;
using Codentia.Common.Helper;

namespace Codentia.Test.Helper
{   
    /// <summary>
    /// This class contains a number of "utility" methods aimed at easing the construction of detailed unit tests.
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>
        /// Standard Name of system check table - mitsysUnitTestCheck
        /// </summary>
        public const string SystemCheckTable = "mitsysUnitTestCheck";   

        /// <summary>
        /// Executes a simple query (select count(*) - as column names unknown) to return the number of rows in a given
        /// table.
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Table to perform operation on</param>
        /// <returns>int of count</returns>
        public static int GetRowCountFromTable(string database, string table)
        {
            return DbInterface.ExecuteQueryScalar<int>(database, string.Format("SELECT COUNT(*) FROM {0}", table));
        }

        /// <summary>
        /// Get an aggregated checksum (binary) for a given table
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Table to perform operation on</param>
        /// <returns>checksum string</returns>
        public static string GetAggregatedChecksumFromTable(string database, string table)
        {
            return DbInterface.ExecuteQueryScalar<string>(string.Format("SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM {0}", table));
        }

        /// <summary>
        /// Executes a series of queries to determine the current row count in every table of the specified database, with checksums
        /// Data is ordered by table name
        /// </summary>
        /// <param name="database">database to analysed</param>
        /// <returns>DataTable (TableName, RowCount)</returns>
        public static DataTable GetDatabaseState(string database)
        {
            DataTable tables = DbInterface.ExecuteQueryDataTable(database, "SELECT name FROM SysObjects WHERE OBJECTPROPERTY(id, 'IsUserTable') = 1 ORDER BY name");

            DataTable rowCounts = new DataTable();
            rowCounts.Columns.Add("TableName", typeof(string));
            rowCounts.Columns.Add("RowCount", typeof(int));
            rowCounts.Columns.Add("Checksum", typeof(string));

            for (int i = 0; i < tables.Rows.Count; i++)
            {
                string name = Convert.ToString(tables.Rows[i]["name"]);
                int rowcount = SqlHelper.GetRowCountFromTable(database, name);
                string checksum = SqlHelper.GetAggregatedChecksumFromTable(database, name);

                rowCounts.Rows.Add(new object[] { name, rowcount, checksum });
            }

            return rowCounts;
        }

        /// <summary>
        /// Executes a simple query to return a random value from the foreign key column in a given table
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="columnName">Name of foreign key column</param>
        /// <param name="tableName">Name of the table to operate on</param>
        /// <returns>int of fk id</returns>
        public static int GetRandomForeignKeyIdFromTable(string database, string columnName, string tableName)
        {
            ParameterCheckHelper.CheckIsValidString(columnName, "columnName", false);

            if (!DbSystem.DoesUserTableExist(database, tableName))
            {
                throw new Exception(string.Format("tableName: {0} does not exist", tableName));
            }

            if (DbSystem.DoesUserTableHaveData(database, tableName) == false)
            {
                throw new Exception(string.Format("tableName - {0} is empty", tableName));
            }
            
            string fkeyTable = columnName.Replace("Id", string.Empty);

            string whereStatement = string.Format(" WHERE {0} IN (SELECT {0} FROM {1})", columnName, fkeyTable);

            string commandText = string.Format(DbSystem.GetRandomIdCommandText(), columnName, tableName, whereStatement);

            return DbInterface.ExecuteQueryScalar<int>(database, commandText);
        }

        /// <summary>
        /// Executes a simple query to return a random row of n specified columns from a given table        
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="tableName">Name of the table to operate on</param>
        /// <param name="columnNames">Comma delimited list of columns</param>
        /// <returns>DataRow of random columns</returns>
        public static DataRow GetRandomColumnsFromTable(string database, string tableName, string columnNames)
        {
            if (!DbSystem.DoesUserTableExist(database, tableName))
            {
                throw new Exception(string.Format("tableName: {0} does not exist", tableName));
            }

            if (DbSystem.DoesUserTableHaveData(database, tableName) == false)
            {
                throw new Exception(string.Format("tableName - {0} is empty", tableName));
            }

            ParameterCheckHelper.CheckIsValidString(columnNames, "columnName", false);

            string commandText = string.Format(DbSystem.GetRandomIdCommandText(), columnNames, tableName, string.Empty);
            return DbInterface.ExecuteQueryDataTable(database, commandText).Rows[0];
        }

        /// <summary>
        /// Executes a simple query to return a random value from a specified column of a given table     
        /// </summary>
        /// <typeparam name="TScalar">The type of the scalar.</typeparam>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="tableName">Name of the table to operate on</param>
        /// <param name="columnName">Can only be a single column</param>
        /// <returns>TScalar object</returns>        
        public static TScalar GetRandomColumnFromTable<TScalar>(string database, string tableName, string columnName)
        {
            if (!DbSystem.DoesUserTableExist(database, tableName))
            {
                throw new Exception(string.Format("tableName: {0} does not exist", tableName));
            }

            if (DbSystem.DoesUserTableHaveData(database, tableName) == false)
            {
                throw new Exception(string.Format("tableName - {0} is empty", tableName));
            }

            ParameterCheckHelper.CheckIsValidString(columnName, "columnName", false);

            if (columnName.Contains(","))
            {
                throw new Exception("Cannot use this method for multiple columns - use GetRandomColumnsFromTable");
            }

            string commandText = string.Format(DbSystem.GetRandomIdCommandText(), columnName, tableName, string.Empty);

            return DbInterface.ExecuteQueryScalar<TScalar>(database, commandText);     
        }

        /// <summary>
        /// Executes a simple query to return a random value from the IDENTITY column of a given table
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Name of the table to operate on</param>
        /// <returns>int of random id</returns>
        public static int GetRandomIdFromTable(string database, string table)
        {
            return SqlHelper.GetRandomIdFromTable(database, table, string.Empty); 
        }

         /// <summary>
        /// Executes a simple query to return a random value from the IDENTITY column of a given table
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Name of the table to operate on</param>
        /// <param name="activeFlagState">if true returns random Id from active records, if false returns random Id from inactive records</param>
        /// <returns>int of random id</returns>
        public static int GetRandomIdFromTable(string database, string table, bool activeFlagState)
        {
            string filterClause = " IsActive=0";
            if (activeFlagState)
            {
                filterClause = " IsActive=1";
            }

            return SqlHelper.GetRandomIdFromTable(database, table, filterClause);
        }
                 
        /// <summary>
        /// Executes a simple query to return a random value from the IDENTITY column of a given table.
        /// Avoid a specific value (as it may already have been used)
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Name of the table to operate on</param>
        /// <param name="excludeId">Exclude another specified Id value from being returned</param>
        /// <returns>int of random id</returns>
        public static int GetRandomIdFromTable(string database, string table, int excludeId)
        {           
            string filterClause = string.Format(" {0}Id!={1}", table, excludeId);
            return SqlHelper.GetRandomIdFromTable(database, table, filterClause);            
        }

        /// <summary>
        /// Executes a simple query to return a random value from the IDENTITY column of a given table.
        /// Avoid a specific value (as it may already have been used)
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="tableName">Name of the table to operate on</param>
        /// <param name="excludeId">Exclude another specified Id value from being returned</param>
        /// <param name="activeFlagState">if true returns random Id from active records, if false returns random Id from inactive records</param>
        /// <returns>int of random id</returns>
        public static int GetRandomIdFromTable(string database, string tableName, int excludeId, bool activeFlagState)
        {
            string filterClause = " IsActive=0";
            if (activeFlagState)
            {
                filterClause = " IsActive=1";
            }

            filterClause = string.Format("{0} AND {1}Id!={2}", filterClause, tableName, excludeId);

            return SqlHelper.GetRandomIdFromTable(database, tableName, filterClause);
        }

        /// <summary>
        /// Executes a simple query to return a random value from the IDENTITY column of a given table with a WHERE clause
        /// Avoid a specific value (as it may already have been used)
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="tableName">Name of the table to operate on</param>
        /// <param name="filterClause">A Where statement without the WHERE keyword</param>
        /// <returns>int of random id</returns>
        public static int GetRandomIdFromTable(string database, string tableName, string filterClause)
        {
            if (!DbSystem.DoesUserTableExist(database, tableName))
            {
                throw new Exception(string.Format("tableName: {0} does not exist", tableName));
            }

            if (DbSystem.DoesUserTableHaveData(database, tableName) == false)
            {
                throw new Exception(string.Format("tableName - {0} is empty", tableName));
            }

            string whereStatement = string.Empty;
            if (!string.IsNullOrEmpty(filterClause))
            {
                whereStatement = string.Format(" WHERE {0}", filterClause);
            }

            string idCol = string.Format("{0}Id", tableName).Replace("_", string.Empty);

            string commandText = string.Format(DbSystem.GetRandomIdCommandText(), idCol, tableName, whereStatement);           

            return DbInterface.ExecuteQueryScalar<int>(database, commandText);
        }

        /// <summary>
        /// Compare the schema within a given DataTable to that in the Sql Database for the specified table
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="dt">DataTable to examine</param>
        /// <param name="tableName">Name of the table for comparison</param>
        /// <returns>bool - true if schema is the same, otherwise false</returns>
        public static bool CompareDataTableSchemaToDatabase(string database, DataTable dt, string tableName)
        {
            DataTable comparisonTable;
            bool isMatch = true;

            // as we are expecting a complete match, lets leverage the intelligence in SqlClient
            // by selecting 0 rows from the table and comparing the returned DataTable to the one we are
            // examining
            comparisonTable = DbInterface.ExecuteQueryDataTable(database, string.Format("SELECT TOP 0 * FROM {0}", tableName));

            // first, check that the data tables have the same number of columns
            if (dt.Columns.Count == comparisonTable.Columns.Count)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string colName = dt.Columns[i].ColumnName;
                    if (!comparisonTable.Columns.Contains(colName))
                    {
                        isMatch = false;
                        break;
                    }
                    else
                    {
                        if (!comparisonTable.Columns[colName].DataType.Equals(dt.Columns[i].DataType))
                        {
                            isMatch = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                isMatch = false;
            }

            return isMatch;
        }

        /// <summary>
        /// Compare the contents of two DataTables - 2 data table parameter version - wrapper for legacy use with new 3 parameter version
          /// </summary>
        /// <param name="dt1">Primary table</param>
        /// <param name="dt2">Secondary table</param>
        /// <returns>bool - true if tables are the same, otherwise false</returns>
        public static bool CompareDataTables(DataTable dt1, DataTable dt2)
        {
            return SqlHelper.CompareDataTables(dt1, dt2, string.Empty, string.Empty);
        }

        /// <summary>
        /// Compare the contents of two DataTables
        /// Does NOT validate schema, only data contained within.
        /// Works by processing the first table against the second (e.g. check all values in first are present in the second)
        /// </summary>
        /// <param name="dt1">Primary table</param>
        /// <param name="dt2">Secondary table</param>
        /// <param name="dbTableName">Optional a table name for using OmittedColumnsList</param>
        /// <param name="omittedColumnsList">Optional A ; delimitted list in format TABLENAME1.COLUMNNAME1;TABLENAME1.COLUMNNAME2; of columns to omit from checking</param>
        /// <returns>bool - true if tables are the same, otherwise false</returns>
        public static bool CompareDataTables(DataTable dt1, DataTable dt2, string dbTableName, string omittedColumnsList)
        {
            bool isMatch = true;

            if (dt1.Columns.Count != dt2.Columns.Count)
            {
                Console.Out.WriteLine(string.Format("Column are different. {0} != {1}", dt1.Columns.Count, dt2.Columns.Count));
                isMatch = false;
            }
            else
            {
                if (dt1.Rows.Count == 0 && dt2.Rows.Count == 0)
                {
                    Console.Out.WriteLine("Both tables are empty");
                }
                else
                {
                    if (dt1.Rows.Count != dt2.Rows.Count)
                    {
                        Console.Out.WriteLine(string.Format("Rowcounts are different. {0} != ", dt1.Rows.Count, dt2.Rows.Count));
                        isMatch = false;
                    }
                    else
                    {
                        for (int j = 0; j < dt1.Columns.Count; j++)
                        {
                            string compareColumnName = dt1.Columns[j].ColumnName;
                            if (!string.IsNullOrEmpty(dbTableName))
                            {
                                compareColumnName = string.Format("{0}.{1}", dbTableName, dt1.Columns[j].ColumnName);
                            }

                            if (omittedColumnsList.IndexOf(compareColumnName) > -1)
                            {
                                Console.WriteLine("{0} data compared - no", compareColumnName);
                            }
                            else
                            {
                                for (int i = 0; i < dt1.Rows.Count; i++)
                                {
                                    if (dt1.Rows[i][j].GetType() == typeof(byte[]))
                                    {
                                        string col1 = BitConverter.ToString((byte[])dt1.Rows[i][j]);
                                        string col2 = BitConverter.ToString((byte[])dt2.Rows[i][j]);

                                        if (col1 != col2)
                                        {
                                            Console.WriteLine("{0} data compared - yes - *** DIFFERENT ***  table1 value: {1}, table2 value: {2}", compareColumnName, col1.Length >= 100 ? col1.Substring(0, 100) : col1.Substring(0, col1.Length), col2.Length >= 100 ? col2.Substring(0, 100) : col2.Substring(0, col2.Length));
                                            isMatch = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (dt1.Rows[i][j].GetType() == typeof(System.DateTime))
                                        {
                                            DateTime datetime1 = Convert.ToDateTime(dt1.Rows[i][j]);
                                            DateTime datetime2 = Convert.ToDateTime(dt2.Rows[i][dt1.Columns[j].ColumnName]);

                                            if (!datetime1.Equals(datetime2))
                                            {
                                                Console.WriteLine("{0} data compared - yes - *** DIFFERENT ***  table1 value: {1}, table2 value: {2}", compareColumnName, datetime1.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK"), datetime2.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK"));
                                                isMatch = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (!dt1.Rows[i][j].Equals(dt2.Rows[i][dt1.Columns[j].ColumnName]))
                                            {
                                                Console.WriteLine("{0} data compared - yes - *** DIFFERENT ***  table1 value: {1}, table2 value: {2}", compareColumnName, dt1.Rows[i][j], dt2.Rows[i][dt1.Columns[j].ColumnName]);
                                                isMatch = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (!isMatch)
                                {
                                    break;
                                }

                                Console.WriteLine("{0} data compared - yes - same", compareColumnName);
                            }
                        }
                    }
                }
            }

            return isMatch;
        }

        /// <summary>
        /// Executes a simple query to get an id that isnt used in the table (max + 100)
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Name of the table to operate on</param>        
        /// <returns>int of unused id</returns>
        public static int GetUnusedIdFromTable(string database, string table)
        {
            return SqlHelper.GetMaxIdFromTable(database, table, string.Empty, false) + 100;
        }

        /// <summary>
        /// Gets the unused id from table.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="table">The table.</param>
        /// <param name="allowEmptyTable">if set to <c>true</c> [allow empty table].</param>
        /// <returns>int of unused id</returns>
        public static int GetUnusedIdFromTable(string database, string table, bool allowEmptyTable)
        {
            return SqlHelper.GetMaxIdFromTable(database, table, string.Empty, true) + 100;
        }

        /// <summary>
        /// Executes a simple query to get the max id from a table (max + 100)
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Name of the table to operate on</param>  
        /// <returns>int of max id</returns>
        public static int GetMaxIdFromTable(string database, string table)
        {
            return SqlHelper.GetMaxIdFromTable(database, table, string.Empty);
        }

        /// <summary>
        /// Gets the max id from table.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="table">The table.</param>
        /// <param name="activeFlagState">if set to <c>true</c> [active flag state].</param>
        /// <returns>int of max id</returns>
        public static int GetMaxIdFromTable(string database, string table, bool activeFlagState)
        {
            return SqlHelper.GetMaxIdFromTable(database, table, activeFlagState, false);
        }

        /// <summary>
        /// Executes a simple query to get the max id from a table (max + 100)
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Name of the table to operate on</param>
        /// <param name="activeFlagState">if true returns Max Id of active records, if false returns Max Id of inactive records</param>
        /// <param name="returnZeroifEmpty">if set to <c>true</c> [return zeroif empty].</param>
        /// <returns>
        /// int of max id
        /// </returns>
        public static int GetMaxIdFromTable(string database, string table, bool activeFlagState, bool returnZeroifEmpty)
        {
            string filterClause = "IsActive=0";
            if (activeFlagState)
            {
                filterClause = "IsActive=1";
            }

            return SqlHelper.GetMaxIdFromTable(database, table, filterClause, returnZeroifEmpty);                                        
        }

        /// <summary>
        /// Gets the max id from table.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="table">The table.</param>
        /// <param name="filterClause">The filter clause.</param>
        /// <returns>int of max id</returns>
        public static int GetMaxIdFromTable(string database, string table, string filterClause)
        {
            return SqlHelper.GetMaxIdFromTable(database, table, filterClause, false);
        }

        /// <summary>
        /// Executes a simple query to get the max id from a table (max + 100)
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Name of the table to operate on</param>
        /// <param name="filterClause">A Where statement without the WHERE keyword</param>
        /// <param name="returnZeroIfEmpty">if set to <c>true</c> [return zero if empty].</param>
        /// <returns>
        /// int of max id
        /// </returns>
        public static int GetMaxIdFromTable(string database, string table, string filterClause, bool returnZeroIfEmpty)
        {
            if (DbSystem.DoesUserTableHaveData(database, table) == false)
            {
                if (returnZeroIfEmpty)
                {
                    return 0;
                }
                else
                {
                    throw new Exception(string.Format("table - {0} is empty", table));
                }
            }

            string whereStatement = string.Empty;
            if (!string.IsNullOrEmpty(filterClause))
            {
                whereStatement = string.Format(" WHERE {0}", filterClause);
            }

            string idCol = string.Format("{0}Id", table).Replace("_", string.Empty);

            return DbInterface.ExecuteQueryScalar<int>(database, string.Format("SELECT MAX({0}) FROM dbo.{1}{2}", idCol, table, whereStatement));
        }

        /// <summary>
        /// Executes a simple query to check whether an id has been used in table        
        /// </summary>
        /// <param name="database">Database to operate on</param>
        /// <param name="table">Name of the table to operate on</param>   
        /// <param name="id">id to check</param>
        /// <returns>bool - true if id exists in table, otherwise false</returns>
        public static bool DoesIdExistInTable(string database, string table, int id)
        {
            if (DbSystem.DoesUserTableHaveData(database, table) == false)
            {
                throw new Exception(string.Format("table - {0} is empty", table));
            }

            bool retVal = false;
            string idCol = string.Format("{0}Id", table.Replace("_", string.Empty));

            DataTable dt = DbInterface.ExecuteQueryDataTable(database, string.Format("SELECT 1 FROM {0} WHERE {0}Id={1}", table, id));

            if (dt.Rows.Count > 0)
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Executes a simple query to delete all records from a table
        /// </summary>
        /// <param name="database">Database to operate on</param>
        /// <param name="table">Name of the table to operate on</param>  
        public static void DeleteFromTable(string database, string table)
        {           
            DbInterface.ExecuteQueryNoReturn(database, string.Format("DELETE FROM {0}", table));
        }

        /// <summary>
        /// Look for first non-existant combination of ids from two tables
        /// </summary>        
        /// <param name="database">The database.</param>
        /// <param name="table1">The table1.</param>
        /// <param name="table2">The table2.</param>
        /// <param name="targetTable">The target table.</param>
        /// <returns>int array</returns>
        public static int[] NonExistantIdCombination(string database, string table1, string table2, string targetTable)
        {
            if (DbSystem.DoesUserTableHaveData(database, table1) == false)
            {
                throw new Exception(string.Format("tableName - {0} is empty", table1));
            }

            if (DbSystem.DoesUserTableHaveData(database, table2) == false)
            {
                throw new Exception(string.Format("tableName - {0} is empty", table2));
            }

            if (DbSystem.DoesUserTableHaveData(database, targetTable) == false)
            {
                throw new Exception(string.Format("tableName - {0} is empty", targetTable));
            }

            List<int> list = new List<int>();

            string idTableTemplate = "SELECT {0}Id FROM {0}";

            DataTable dt1 = DbInterface.ExecuteQueryDataTable(database, string.Format(idTableTemplate, table1));
            DataTable dt2 = DbInterface.ExecuteQueryDataTable(database, string.Format(idTableTemplate, table2));

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                int id1 = Convert.ToInt32(dt1.Rows[i][string.Format("{0}Id", table1)]);

                for (int j = 0; j < dt2.Rows.Count; j++)
                {
                    int id2 = Convert.ToInt32(dt2.Rows[j][string.Format("{0}Id", table2)]);
                    string targetQuery = string.Format("SELECT {0}Id, {1}Id FROM {2} WHERE {0}Id={3} AND {1}Id={4}", table1, table2, targetTable, id1, id2);

                    DataTable dt3 = DbInterface.ExecuteQueryDataTable(database, targetQuery);
                    if (dt3.Rows.Count == 0)
                    {
                        list.Add(id1);
                        list.Add(id2);
                        break;
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Return a table id from a tokenised value as string 
        /// ie. tokenisedValue - [MyTable] would return MyTableId as a string
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="tokenisedValue">e.g. [MyTable]</param>
        /// <returns>string - id</returns>
        public static string GetDatabaseValueFromToken(string database, string tokenisedValue)
        {
            string tableName = tokenisedValue.Replace("[", string.Empty).Replace("]", string.Empty);
            int id = SqlHelper.GetRandomIdFromTable(database, tableName);

            return Convert.ToString(id);
        }

        /// <summary>
        /// Return an unused table id from a tokenised value as string 
        /// ie. tokenisedValue - [MyTable] would return MyTableId as a string
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="tokenisedValue">e.g. [MyTable]</param>
        /// <returns>string - unused id</returns>
        public static string GetDatabaseNonExistantValueFromToken(string database, string tokenisedValue)
        {
            string tableName = tokenisedValue.Replace("[", string.Empty).Replace("]", string.Empty);
            int id = SqlHelper.GetUnusedIdFromTable(database, tableName);

            return Convert.ToString(id);
        }

        /// <summary>
        /// Clear Data In Database
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="clearAllData">if true, add all user tables in database to the list of cleardown files in the xmlTableList , otherwise just use files in the xmlTableList</param>
        /// <param name="xmlTableListFilePath">(optional) filepath of an xml document root cleardowntables, element name - cleardowntable</param>
        public static void ClearDataInDatabase(string database, bool clearAllData, string xmlTableListFilePath)
        {
            List<string> tableList = new List<string>();
            List<string> noDeleteTableList = new List<string>();

            if (!string.IsNullOrEmpty(xmlTableListFilePath))
            {            
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlTableListFilePath);                

                XmlNode rootNode = xmlDoc.ChildNodes[0];                

                if (rootNode.ChildNodes.Count > 0)
                {
                    for (int i = 0; i < rootNode.ChildNodes.Count; i++)
                    {
                        XmlNode node = rootNode.ChildNodes[i];
                        string tableName = node.InnerText;
                        
                        if (IsNoDeleteNode(node))
                        {
                            if (!noDeleteTableList.Contains(tableName))
                            {
                                noDeleteTableList.Add(tableName);
                            }                                
                        }

                        if (!tableList.Contains(tableName) && !noDeleteTableList.Contains(tableName))
                        {
                            tableList.Add(tableName);
                        }                       
                    }
                }
            }

            if (clearAllData)
            {
                DataTable dt = DbSystem.GetAllUserTables(database);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string tableName = Convert.ToString(dt.Rows[i]["TableName"]);

                        if (!tableList.Contains(tableName) && !noDeleteTableList.Contains(tableName))
                        {
                            tableList.Add(tableName);
                        }
                    }
                }
            }

            if (tableList.Count > 0)
            {
                IEnumerator<string> ie = tableList.GetEnumerator();

                while (ie.MoveNext())
                {
                    string commandText = string.Format("DELETE FROM dbo.[{0}]", ie.Current);

                    try
                    {
                        DbInterface.ExecuteQueryNoReturn(database, commandText);
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                        if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                        {
                            int posChildStart = message.IndexOf("table");
                            int posChildEnd = message.IndexOf("column");

                            string childTableName = message.Substring(posChildStart + 11, (posChildEnd - 14) - posChildStart);

                            message = string.Format("The table: {0} (child table of {1}) still contains data. Add that child table to the FK XML table list.", childTableName, ie.Current);
                        }

                        throw new Exception(message);
                    }
                }
            }
        }

        /// <summary>
        /// Create System Check Table
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="checkGuid">guid used in checking process</param>        
        public static void CreateSystemCheckTable(string database, Guid checkGuid)
        {
            if (DbSystem.DoesUserTableExist(database, SystemCheckTable))
            {
                string dropCommandText = string.Format("DROP TABLE dbo.[{0}]", SystemCheckTable);
                DbInterface.ExecuteQueryNoReturn(database, dropCommandText);
            }

            StringBuilder sbCreate = new StringBuilder();

            sbCreate.Append(string.Format("CREATE TABLE [dbo].[{0}] (", SystemCheckTable));
            sbCreate.Append("[CheckGuid] [uniqueidentifier] NOT NULL,");
            sbCreate.Append("[CheckDate] [datetime] NOT NULL DEFAULT GETDATE()");
            sbCreate.Append(") ON [PRIMARY]");

            DbInterface.ExecuteQueryNoReturn(database, sbCreate.ToString());

            StringBuilder sbInsert = new StringBuilder();
            sbInsert.Append(string.Format("INSERT INTO [dbo].[{0}] (CheckGuid) ", SystemCheckTable));
            sbInsert.Append(string.Format("VALUES ('{0}')", checkGuid));

            DbInterface.ExecuteQueryNoReturn(database, sbInsert.ToString());
        }

        /// <summary>
        /// Read System Check Table
        /// </summary>
        /// <param name="database">the database</param>  
        /// <returns>Dictionary of Guid and DateTime</returns>
        public static Dictionary<Guid, DateTime> ReadSystemCheckTable(string database)
        {
            if (!DbSystem.DoesUserTableExist(database, SystemCheckTable))
            {
                throw new Exception(string.Format("{0} has not been created. CreateSystemCheckTable needs to be called before this method", SystemCheckTable));
            }

            string selectText = string.Format("SELECT * FROM {0}", SystemCheckTable);
            DataTable dt = DbInterface.ExecuteQueryDataTable(database, selectText);

            Guid g = new Guid(Convert.ToString(dt.Rows[0]["CheckGuid"]));
            DateTime dte = Convert.ToDateTime(dt.Rows[0]["CheckDate"]);

            Dictionary<Guid, DateTime> dict = new Dictionary<Guid, DateTime>();

            dict.Add(g, dte);

            return dict;
        }      

        /// <summary>
        /// Retrieve the id for a specific value from a lookup table
        /// <para></para>
        /// ie SELECT [TABLENAME]Id FROM [TABLENAME] WHERE [TABLENAME]Code = [VALUE]
        /// <para></para>
        /// Therefore: the schema of the table must have the 2 columns [TABLENAME]Id and [TABLENAME]Code
        /// </summary>
        /// <param name="database">the database</param>  
        /// <param name="table">the table being queried</param>
        /// <param name="lookupValue">lookup Value (i.e. value of the Code column)</param>
        /// <returns>int - id of value being retrieved</returns>
        public static int GetIdFromTable(string database, string table, string lookupValue)
        {
            string columnName = string.Format("{0}Code", table);            
            return GetIdFromTable(database, table, columnName, lookupValue, DbType.String);
        }

        /// <summary>
        /// Retrieve the id for a specific value for specific filter column name
        /// <para></para>
        /// ie SELECT [TABLENAME]Id FROM [TABLENAME] WHERE [COLUMNNAME] = [VALUE]
        /// </summary>
        /// <param name="database">the database</param>  
        /// <param name="table">the table being queried</param>
        /// <param name="filterColumnName">the column in the table to be queried</param>
        /// <param name="filterColumnValue">value of the filter column to be queried)</param>
        /// <param name="dbType">the column type</param>
        /// <returns>int - id of value being retrieved</returns>
        public static int GetIdFromTable(string database, string table, string filterColumnName, string filterColumnValue, DbType dbType)
        {
            if (DbSystem.DoesUserTableHaveData(database, table) == false)
            {
                throw new ArgumentException(string.Format("table - {0} is empty", table));
            }

            string pattern = "SELECT {0}Id FROM dbo.[{0}] WHERE {1} = '{2}'";
            switch (dbType)
            {
                case DbType.Boolean:
                case DbType.Byte:
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.Single:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    pattern = "SELECT {0}Id FROM dbo.[{0}] WHERE {1} = {2}";
                    break;
            }
           
            return DbInterface.ExecuteQueryScalar<int>(database, string.Format(pattern, table, filterColumnName, filterColumnValue));
        }

        /// <summary>
        /// Executes SELECT * FROM [Table] WHERE [Table]Id=idValue
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Table to perform operation on</param>
        /// <param name="idValue">value of id</param>
        /// <returns>DataTable object</returns>
        public static DataTable GetRowFromTable(string database, string table, int idValue)
        {
            return DbInterface.ExecuteQueryDataTable(database, string.Format("SELECT * FROM {0} WHERE {0}Id = {1}", table, idValue));
        }

        /// <summary>
        /// Executes SELECT [Table]Id, [Table]Code FROM [Table] ORDER BY {0}Id
        /// </summary>
        /// <param name="database">Database (from config) to perform operation on</param>
        /// <param name="table">Table to perform operation on</param>
        /// <returns>DataTable object</returns>
        public static DataTable GetAllRowsFromLookupTable(string database, string table)
        {
            return DbInterface.ExecuteQueryDataTable(database, string.Format("SELECT {0}Id, {0}Code FROM {0} ORDER BY {0}Id", table));
        }

        private static bool IsNoDeleteNode(XmlNode node)
        {
            bool noDelete = false;
            if (node.Attributes.Count > 0)
            {
                if (node.Attributes["nodelete"] != null)
                {
                    if (node.Attributes["nodelete"].Value.ToLower() == "y")
                    {
                        noDelete = true;
                    }
                }
            }

            return noDelete;
        }
    }
}
