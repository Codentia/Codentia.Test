using System;
using System.Collections.Generic;
using System.Data;
using Codentia.Common.Data;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Test.Test.Helper.Test
{
    /// <summary>
    /// This fixture provides positive tests for the SqlHelper module of the UnitTestHelper library.
    /// Negative tests (e.g. bad queries) are not provided as there should be no handling for bad arguments.
    /// Any exceptions thrown are desirable and should cause a resultant test failure, e.g. are not a flaw in
    /// the module but rather its intended behaviour.
    /// <para></para>
    /// Tests will be run against the MIT_Common_DL database
    /// </summary>
    [TestFixture]
    public class SqlHelperTest
    {
        /// <summary>
        /// Validate that all required data is available for these tests
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            string scriptPath = @"Helper.Test\SQL\SetupTestDB.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "MITTest1", scriptPath, true);    
       
            scriptPath = @"Helper.Test\SQL\SetUpTestLookupDB.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test4", "MITTest4", scriptPath, true);
        }

        /// <summary>
        /// Scenario: Method run against a table known to contain one row
        /// Expected: 1
        /// </summary>
        [Test]
        public void _001_GetRowCountFromTable_OneRow()
        {
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestTable1"), Is.EqualTo(1), "Incorrect Rowcount returned");
        }

        /// <summary>
        /// Scenario: Method run against a table known to contain NO rows
        /// Expected: 0
        /// </summary>
        [Test]
        public void _002_GetRowCountFromTable_NoRows()
        {
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestTable2"), Is.EqualTo(0), "Incorrect Rowcount returned");
        }

        /// <summary>
        /// Scenario: Method run against a table with two rows, until both values have been retrieved
        /// Expected: Varying results - known values
        /// </summary>
        [Test]
        public void _003_GetRandomIdFromTable()
        {
            // test a non-existant table (for coverage)
            Assert.That(delegate { SqlHelper.GetRandomIdFromTable("test1", "TableNotFound"); }, Throws.Exception.With.Message.EqualTo("tableName: TableNotFound does not exist"));

            // now test random ids are found correctly
            bool found1 = false;
            bool found2 = false;
            int safety = 0;

            while ((!found1 || !found2) && safety < 100000)
            {
                int val = SqlHelper.GetRandomIdFromTable("test1", "TestTable3");

                if (val == 1)
                {
                    found1 = true;
                }

                if (val == 2)
                {
                    found2 = true;
                }

                safety++;
            }

            Assert.That(safety, Is.LessThan(100000), "Safety limit exceeded");
            Assert.That(found1, Is.True, "Did not find id=1");
            Assert.That(found2, Is.True, "Did not find id=2");
        }

        /// <summary>
        /// Scenario: Random id retrieved from deterministic set
        /// Expected: Alternate value (see test for details)
        /// </summary>
        [Test]
        public void _004_GetRandomIdFromTable_Exclusion()
        {
            int val = 0;

            val = SqlHelper.GetRandomIdFromTable("test1", "TestTable3", 1);
            Assert.That(val, Is.EqualTo(2), "Incorrect value retrieved");

            val = SqlHelper.GetRandomIdFromTable("test1", "TestTable3", 2);
            Assert.That(val, Is.EqualTo(1), "Incorrect value retrieved");
        }

        /// <summary>
        /// Scenario: Try to call GetRandomIdFromTable against an empty table
        /// Expected: Empty table exception raised
        /// </summary>
        [Test]
        public void _005_GetRandomIdFromTable_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetRandomIdFromTable("test1", "TestTableEmpty"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("tableName - TestTableEmpty is empty"));
        }

        /// <summary>
        /// Scenario: Method used to test a data table created by a query against its own source table
        /// Expected: true
        /// </summary>
        [Test]
        public void _006_CompareDataTableSchemaToDatabase_Match()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("test1", "SELECT * FROM TestTable2");
            Assert.That(SqlHelper.CompareDataTableSchemaToDatabase("test1", dt, "TestTable2"), Is.True, "Got false for set created from same table.");
        }

        /// <summary>
        /// Scenario: Method used to test various non-matching data tables against a database table
        /// Expected: false
        /// </summary>
        [Test]
        public void _007_CompareDataTableSchemaToDatabase_NoMatch()
        {
            Assert.That(SqlHelper.CompareDataTableSchemaToDatabase("test1", new DataTable(), "TestTable2"), Is.False, "Got true for new datatable");

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("TestCol1"));
            dt.Columns.Add(new DataColumn("TestCol2"));
            dt.Columns.Add(new DataColumn("TestCol3"));
            dt.Columns.Add(new DataColumn("TestCol4"));

            Assert.That(SqlHelper.CompareDataTableSchemaToDatabase("test1", dt, "TestTable2"), Is.False, "Got true for table with differing column names");

            dt.Columns.Clear();
            dt.Columns.Add(new DataColumn("TestTable2Id"));
            dt.Columns.Add(new DataColumn("TestInt"));
            dt.Columns.Add(new DataColumn("TestString"));
            dt.Columns.Add(new DataColumn("TestDateTime"));

            dt.Columns["TestInt"].DataType = typeof(int);
            dt.Columns["TestString"].DataType = typeof(string);
            dt.Columns["TestDateTime"].DataType = typeof(int);

            Assert.That(SqlHelper.CompareDataTableSchemaToDatabase("test1", dt, "TestTable2"), Is.False, "Got true for table with differing column data types");
        }
        
        /// <summary>
        /// Scenario: Method used to compare known matching tables
        /// Expected: true
        /// </summary>
        [Test]
        public void _014_CompareDataTables_Match()
        {
            DataTable dt1 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable3");
            DataTable dt2 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable3");
            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.True, "No match found for identical tables");
        }

        /// <summary>
        /// Scenario: Method used to compare known matching tables
        /// Expected: true
        /// </summary>
        [Test]
        public void _015_CompareDataTables_EmptyTables()
        {
            DataTable dt1 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable2");
            DataTable dt2 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable2");
            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.True, "No match found for identical tables");
        }

        /// <summary>
        /// Scenario: Method used to compare known different tables
        /// Expected: false
        /// </summary>
        [Test]
        public void _016_CompareDataTables_NoMatch()
        {
            DataTable dt1 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable1");
            DataTable dt2 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable3");
            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.False, "Match found for different tables");

            dt1 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable3");
            dt2.Rows[0]["TestTable3Id"] = 170;
            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.False, "Match found for different tables");

            dt2.Rows[0]["TestTable3Id"] = 1;
            dt1.Columns.Add(new DataColumn("TestCol1", typeof(int)));
            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.False, "Match found for different tables");
        }

        /// <summary>
        /// Scenario: Make sure IsTablePopulated handles table with underscores in name (Valid)
        /// Expected: false
        /// </summary>
        [Test]
        public void _017_IsTablePopulated_HandleUnderscoreTable_TableExists()
        {
            Assert.That(DbSystem.DoesUserTableHaveData("test1", "TestTable_Underscore"), Is.True, "Got false for populated table");
        }

        /// <summary>
        /// Scenario: Make sure IsTablePopulated handles table with underscores in name (Valid)
        /// Expected: false
        /// </summary>
        [Test]
        public void _018_IsTablePopulated_HandleUnderscoreTable_TableDoesntExist()
        {
            Assert.That(delegate { bool isPop = DbSystem.DoesUserTableHaveData("test1", "Test_TableIDONTEXISTANDIFDOTHATSWRONG"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("tableName: Test_TableIDONTEXISTANDIFDOTHATSWRONG does not exist"));
        }

        /// <summary>
        /// Scenario: Make sure GetRandomIdFromTable handles table with underscores in name
        /// Expected: false
        /// </summary>
        [Test]
        public void _019_GetRandomIdFromTable_HandleUnderscoreTable_TableExists()
        {
            bool found1 = false;
            bool found2 = false;
            int safety = 0;

            while ((!found1 || !found2) && safety < 100000)                                                  
            {
                int val = SqlHelper.GetRandomIdFromTable("test1", "TestTable_Underscore");

                if (val == 1)
                {
                    found1 = true;
                }               

                if (val == 2)
                {
                    found2 = true;
                }  

                safety++;
            }

            Assert.That(safety, Is.LessThan(100000), "Safety limit exceeded");
            Assert.That(found1, Is.True, "Did not find id=1");
            Assert.That(found2, Is.True, "Did not find id=2");
        }

        /// <summary>
        /// Scenario: Make sure GetRandomIdFromTable handles table with underscores in name
        /// Expected: false
        /// </summary>
        [Test]
        public void _020_GetRandomIdFromTable_HandleUnderscoreTable_TableDoesntExist()
        {
            Assert.That(delegate { bool isPop = DbSystem.DoesUserTableHaveData("test1", "Test_MYTABLEHENOTHERE"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("tableName: Test_MYTABLEHENOTHERE does not exist"));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomIdFromTable against an empty table
        /// Expected: Empty table exception raised
        /// </summary>
        [Test]
        public void _020a_GetRandomIdFromTable_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetRandomIdFromTable("test1", "TestTableEmpty", 1); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("tableName - TestTableEmpty is empty"));
        }

        /// <summary>
        /// Scenario: GetRowCountByTable executed, results compared to manual query
        /// Expected: Matching data tables
        /// </summary>
        [Test]
        public void _023_GetDatabaseState()
        {
            DataTable dt = SqlHelper.GetDatabaseState("test1");
            int tableCount = DbInterface.ExecuteQueryScalar<int>("SELECT COUNT(*) FROM Sysobjects WHERE OBJECTPROPERTY(id, 'IsUserTable') = 1");

            Assert.That(tableCount, Is.GreaterThan(0), "No tables found to test with");
            Assert.That(dt.Rows.Count, Is.EqualTo(tableCount), "Table count does not match");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int rowcount = SqlHelper.GetRowCountFromTable("test1", Convert.ToString(dt.Rows[i]["TableName"]));
                Assert.That(Convert.ToInt32(dt.Rows[i]["RowCount"]), Is.EqualTo(rowcount), "Rowcount mismatches");

                if (rowcount > 0)
                {
                    string checksum = SqlHelper.GetAggregatedChecksumFromTable("test1", Convert.ToString(dt.Rows[i]["TableName"]));
                    Assert.That(Convert.ToString(dt.Rows[i]["Checksum"]), Is.EqualTo(checksum), "Checksum mismatches");
                }
            }
        }

        /// <summary>
        /// Scenario: GetAggregatedChecksumFromTable called on a range of tables
        /// Expected: Matches manual computation
        /// </summary>
        [Test]
        public void _024_GetAggregatedChecksumFromTable()
        {
            DataTable tables = DbInterface.ExecuteQueryDataTable("SELECT name FROM SysObjects WHERE OBJECTPROPERTY(id, 'IsUserTable') = 1 ORDER BY name");

            for (int i = 0; i < tables.Rows.Count; i++)
            {
                string manual = DbInterface.ExecuteQueryScalar<string>(string.Format("SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM {0}", Convert.ToString(tables.Rows[i]["name"])));
                Assert.That(SqlHelper.GetAggregatedChecksumFromTable("test1", Convert.ToString(tables.Rows[i]["name"])), Is.EqualTo(manual), "Checksum mismatches");
            }
        }

        /// <summary>
        /// Scenario: Compare two identical data tables which have differing rowcounts
        /// Expected: false
        /// </summary>
        [Test]
        public void _029_CompareDataTables_DifferingRowCounts()
        {
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Col1");

            dt1.Rows.Add(new object[] { "value" });

            DataTable dt2 = dt1.Copy();
            dt2.Rows.Add(new object[] { "value2" });

            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.False, "Expected false - rowcounts do not match");
            Assert.That(SqlHelper.CompareDataTables(dt2, dt1), Is.False, "Expected false - rowcounts do not match");
        }

        /// <summary>
        /// Scenario: Compare two identical data tables which have differing rowcounts
        /// Expected: false
        /// </summary>
        [Test]
        public void _030_DoesIdExistInTable_IdExists()
        {
            int retVal = SqlHelper.GetRandomIdFromTable("test1", "TestTable1");
            bool retCheck = SqlHelper.DoesIdExistInTable("test1", "TestTable1", retVal);
            Assert.That(retCheck, Is.True, "retCheck expected to be true");
        }

        /// <summary>
        /// Scenario: Compare two identical data tables which have differing rowcounts
        /// Expected: false
        /// </summary>
        [Test]
        public void _031_DoesIdExistInTable_NonExistantId()
        {
            bool retCheck = SqlHelper.DoesIdExistInTable("test1", "TestTable1", -1);
            Assert.That(retCheck, Is.False, "retCheck expected to be false");
        }

        /// <summary>
        /// Scenario: Try to call DoesIdExistInTable against an empty table
        /// Expected: Empty table exception raised
        /// </summary>
        [Test]
        public void _031a_DoesIdExistInTable_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.DoesIdExistInTable("test1", "TestTableEmpty", 1); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("table - TestTableEmpty is empty"));
        }

        /// <summary>
        /// Scenario: Get an unused id for a table
        /// Expected: false
        /// </summary>
        [Test]
        public void _032_GetUnusedIdFromTable()
        {
            int retVal = SqlHelper.GetUnusedIdFromTable("test1", "TestTable1");
            Assert.That(SqlHelper.DoesIdExistInTable("test1", "TestTable1", retVal), Is.False, "retCheck expected to be false");
        }

        /// <summary>
        /// Scenario: Try to call GetUnusedIdFromTable against an empty table
        /// Expected: Empty table exception raised
        /// </summary>
        [Test]
        public void _032a_GetUnusedIdFromTable_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetUnusedIdFromTable("test1", "TestTableEmpty"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("table - TestTableEmpty is empty"));

            // test with allow empty overload
            Assert.That(SqlHelper.GetUnusedIdFromTable("test1", "TestTableEmpty", true), Is.GreaterThan(0));
        }

        /// <summary>
        /// Scenario: Try to call GetMaxIdFromTable against an empty table
        /// Expected: Empty table exception raised
        /// </summary>
        [Test]
        public void _034_GetMaxIdFromTable_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetMaxIdFromTable("test1", "TestTableEmpty"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("table - TestTableEmpty is empty"));
        }

        /// <summary>
        /// Scenario: Get the max id for a table
        /// Expected: returns max Id
        /// </summary>
        [Test]
        public void _035_GetMaxIdFromTable()
        {
            int checkVal = DbInterface.ExecuteQueryScalar<int>("SELECT MAX(TestTable1Id) FROM TestTable1");
            int retVal = SqlHelper.GetMaxIdFromTable("test1", "TestTable1");
            Assert.That(retVal, Is.EqualTo(checkVal));
        }

        /// <summary>
        /// Scenario: Try to call GetMaxIdFromTable (ActiveFlag parameter version) against an empty table
        /// Expected: Empty table exception raised
        /// </summary>
        [Test]
        public void _036_GetMaxIdFromTable_ActiveFlag_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetMaxIdFromTable("test1", "TestTableEmpty", true); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("table - TestTableEmpty is empty"));
        }

         /// <summary>
        /// Scenario: Get the max id for a table (ActiveFlag parameter version) 
        /// Expected: returns max Id
        /// </summary>
        [Test]
        public void _037_GetMaxIdFromTable_ActiveFlag()
        {
            int checkValActive = DbInterface.ExecuteQueryScalar<int>("SELECT MAX(TestTableActiveId) FROM TestTableActive WHERE IsActive=1");
            int checkValInactive = DbInterface.ExecuteQueryScalar<int>("SELECT MAX(TestTableActiveId) FROM TestTableActive WHERE IsActive=0");
            int retValActive = SqlHelper.GetMaxIdFromTable("test1", "TestTableActive", true);
            int retValInactive = SqlHelper.GetMaxIdFromTable("test1", "TestTableActive", false);

            Assert.That(retValActive, Is.EqualTo(checkValActive));
            Assert.That(retValInactive, Is.EqualTo(checkValInactive));
        }

        /// <summary>
        /// Scenario: Try to call GetMaxIdFromTable (FilterClause parameter version) against an empty table
        /// Expected: Empty table exception raised
        /// </summary>
        [Test]
        public void _038_GetMaxIdFromTable_FilterClause_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetMaxIdFromTable("test1", "TestTableEmpty", "Id='blah'"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("table - TestTableEmpty is empty"));
        }

        /// <summary>
        /// Scenario: Get the max id for a table (FilterClause parameter version) 
        /// Expected: returns max Id
        /// </summary>
        [Test]
        public void _039_GetMaxIdFromTable_FilterClause()
        {
            int checkVal = DbInterface.ExecuteQueryScalar<int>("test1", "SELECT MAX(TestTableActiveId) FROM TestTableActive WHERE TestInt=1");
            int retVal = SqlHelper.GetMaxIdFromTable("test1", "TestTableActive", "TestInt=1");

            Assert.That(retVal, Is.EqualTo(checkVal));
        }

        /// <summary>
        /// Scenario: Try to call TestTableDelete 
        /// Expected: Removes records
        /// </summary>
        [Test]
        public void _040_DeleteFromTable()
        {
            DbInterface.ExecuteQueryNoReturn("INSERT INTO TestTableDelete (TestInt) VALUES (1)");
            int count = SqlHelper.GetRowCountFromTable("test1", "TestTableDelete");
            Assert.That(count, Is.GreaterThan(0));
            SqlHelper.DeleteFromTable("test1", "TestTableDelete");
            count = SqlHelper.GetRowCountFromTable("test1", "TestTableDelete");
            Assert.That(count, Is.EqualTo(0));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomIdFromTable (Active Flag only version) 
        /// Expected: Gets a random id
        /// </summary>
        [Test]
        public void _041_GetRandomIdFromTable_ActiveFlag()
        {
            int retValActive = SqlHelper.GetRandomIdFromTable("test1", "TestTableActive", true);
            Assert.That(retValActive, Is.GreaterThan(0));
            int retValInactive = SqlHelper.GetRandomIdFromTable("test1", "TestTableActive", false);
            Assert.That(retValInactive, Is.GreaterThan(0));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomIdFromTable (ExcludeId and Active Flag version) 
        /// Expected: Gets a random id
        /// </summary>
        [Test]
        public void _042_GetRandomIdFromTable_ExcludeId_ActiveFlag()
        {
            int retValActive = SqlHelper.GetRandomIdFromTable("test1", "TestTableActive", 1, true);
            Assert.That(retValActive, Is.GreaterThan(0));
            int retValInactive = SqlHelper.GetRandomIdFromTable("test1", "TestTableActive", 3, false);
            Assert.That(retValInactive, Is.GreaterThan(0));
        }

        /// <summary>
        /// Scenario: Try to call CompareDataTables With Db TableName
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _043_CompareDataTables_WithDbTableName()
        {
            DataTable dt1 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable1");
            DataTable dt2 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable1");

            dt1.TableName = "table";
            dt2.TableName = "table";

            Assert.That(SqlHelper.CompareDataTables(dt1, dt2, "table", string.Empty), Is.True);
        }

        /// <summary>
        /// Scenario: Try to call CompareDataTables With OmittedColumns
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _044_CompareDataTables_WithOmittedColumns()
        {
            DataTable dt1 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable1");
            DataTable dt2 = DbInterface.ExecuteQueryDataTable("SELECT * FROM TestTable1");

            dt1.TableName = "table";
            dt2.TableName = "table";

            Assert.That(SqlHelper.CompareDataTables(dt1, dt2, string.Empty, "TestString"), Is.True);
            Assert.That(SqlHelper.CompareDataTables(dt1, dt2, "table", "table.TestString"), Is.True);
        }

        /// <summary>
        /// Scenario: Try to call CompareDataTables With byte array
        /// Expected: Runs successfully 
        /// </summary>
        [Test]
        public void _045_CompareDataTables_WithByteArray()
        {
            DataTable dtSchema = new DataTable();
            dtSchema.Columns.Add(new DataColumn("Column1", typeof(byte[])));

            // matching
            DataTable dt1 = dtSchema.Clone();
            dt1.Rows.Add(new byte[] { 1, 2, 3, 4 });
            dt1.Rows.Add(new byte[] { 4, 3, 2, 1 });

            DataTable dt2 = dt1.Copy();

            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.True);

            // not matching
            dt1 = dtSchema.Clone();
            dt1.Rows.Add(new byte[] { 1, 2, 3, 4 });
            dt1.Rows.Add(new byte[] { 4, 3, 2, 1 });

            dt2 = dtSchema.Clone();
            dt2.Rows.Add(new byte[] { 4, 3, 2, 1 });
            dt2.Rows.Add(new byte[] { 1, 2, 3, 4 });

            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.False);
        }   
    
        /// <summary>
        /// Scenario: Try to call NonExistantIdCombination_InvalidTables
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _046_NonExistantIdCombination_InvalidTables()
        {
            Assert.That(delegate { SqlHelper.NonExistantIdCombination("test1", "TestTableEmpty", "TestTableEmpty2", "TestTableEmpty3"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("tableName - TestTableEmpty is empty"));
            Assert.That(delegate { SqlHelper.NonExistantIdCombination("test1", "TestTableCombo", "TestTableEmpty2", "TestTableEmpty3"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("tableName - TestTableEmpty2 is empty"));
            Assert.That(delegate { SqlHelper.NonExistantIdCombination("test1", "TestTableCombo", "TestTableCombo2", "TestTableEmpty3"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("tableName - TestTableEmpty3 is empty"));
        }

         /// <summary>
        /// Scenario: Try to call NonExistantIdCombination
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _047_NonExistantIdCombination()
        {
            int[] array = SqlHelper.NonExistantIdCombination("test1", "TestTableCombo", "TestTableCombo2", "TestTableCombo3");

            Assert.That(array[0], Is.EqualTo(1));
            Assert.That(array[1], Is.EqualTo(2));            
        }

        /// <summary>
        /// Scenario: Try to call GetRandomForeignKeyIdFromTable with non existant table
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _048_GetRandomForeignKeyIdFromTable_NonExistentTable()
        {
            Assert.That(delegate { SqlHelper.GetRandomForeignKeyIdFromTable("test1", "MyColumn", "NONEXISTANTTABLE"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("tableName: NONEXISTANTTABLE does not exist"));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomForeignKeyIdFromTable with an empty table
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _049_GetRandomForeignKeyIdFromTable_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetRandomForeignKeyIdFromTable("test1", "MyColumn", "TestTableEmpty"); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("tableName - TestTableEmpty is empty"));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomForeignKeyIdFromTable with an empty column name
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _050_GetRandomForeignKeyIdFromTable_EmptyColumnName()
        {
            // null
            Assert.That(delegate { SqlHelper.GetRandomForeignKeyIdFromTable("test1", null, "TestTableEmpty"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("columnName is not specified"));

            // empty
            Assert.That(delegate { SqlHelper.GetRandomForeignKeyIdFromTable("test1", string.Empty, "TestTableEmpty"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("columnName is not specified"));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomForeignKeyIdFromTable with an empty column name
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _051_GetRandomForeignKeyIdFromTable()
        {
            int id = SqlHelper.GetRandomForeignKeyIdFromTable("test1", "TestTable4Id", "TestTable5");
            Assert.That(id, Is.GreaterThan(0));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnFromTable with an non-existant table
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _052_GetRandomColumnFromTable_NonExistantTable()
        {
            Assert.That(delegate { SqlHelper.GetRandomColumnFromTable<string>("test1", "TABLEDOESNOTEXIST", string.Empty); }, Throws.Exception.With.Message.EqualTo("tableName: TABLEDOESNOTEXIST does not exist"));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnFromTable with an empty table
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _053_GetRandomColumnFromTable_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetRandomColumnFromTable<string>("test1", "TestTableEmpty", string.Empty); }, Throws.Exception.With.Message.EqualTo("tableName - TestTableEmpty is empty"));           
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnFromTable with an empty column name
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _054_GetRandomColumnFromTable_EmptyColumn()
        {
            Assert.That(delegate { SqlHelper.GetRandomColumnFromTable<string>("test1", "TestTable1", string.Empty); }, Throws.Exception.With.Message.EqualTo("columnName is not specified"));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnFromTable with a comma delimited list
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _056_GetRandomColumnFromTable_DelimitedList()
        {
            Assert.That(delegate { SqlHelper.GetRandomColumnFromTable<string>("test1", "TestTable1", "comma,comma"); }, Throws.Exception.With.Message.EqualTo("Cannot use this method for multiple columns - use GetRandomColumnsFromTable"));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnFromTable with valid data - string       
        /// Expected: String returned
        /// </summary>
        [Test]
        public void _057_GetRandomColumnFromTable_ValidData_String()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("test1", "SELECT TestString FROM TestTable4");

            List<string> list = new List<string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(Convert.ToString(dt.Rows[i]["TestString"]));
            }

            string retVal = SqlHelper.GetRandomColumnFromTable<string>("test1", "TestTable4", "TestString");

            Assert.That(list.Contains(retVal));            
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnFromTable with valid data - int      
        /// Expected: Int returned
        /// </summary>
        [Test]
        public void _058_GetRandomColumnFromTable_ValidData_Int()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("test1", "SELECT TestInt FROM TestTable4");

            List<int> list = new List<int>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(Convert.ToInt32(dt.Rows[i]["TestInt"]));
            }

            int retVal = SqlHelper.GetRandomColumnFromTable<int>("test1", "TestTable4", "TestInt");

            Assert.That(list.Contains(retVal));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnFromTable with valid data - DateTime
        /// Expected: DateTime returned
        /// </summary>
        [Test]
        public void _059_GetRandomColumnFromTable_ValidData_DateTime()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("test1", "SELECT TestDateTime FROM TestTable4");

            List<DateTime> list = new List<DateTime>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(Convert.ToDateTime(dt.Rows[i]["TestDateTime"]));
            }

            DateTime retVal = SqlHelper.GetRandomColumnFromTable<DateTime>("test1", "TestTable4", "TestDateTime");

            Assert.That(list.Contains(retVal));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnFromTable with valid data - Guid
        /// Expected: Guid returned
        /// </summary>
        [Test]
        public void _060_GetRandomColumnFromTable_ValidData_Guid()
        {
            DataTable dt = DbInterface.ExecuteQueryDataTable("test1", "SELECT TestGuid FROM TestTable6");

            List<Guid> list = new List<Guid>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new Guid(Convert.ToString(dt.Rows[i]["TestGuid"])));
            }

            Guid retVal = SqlHelper.GetRandomColumnFromTable<Guid>("test1", "TestTable6", "TestGuid");

            Assert.That(list.Contains(retVal));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnsFromTable with valid data
        /// Expected: Randow row returned
        /// </summary>
        [Test]
        public void _061_GetRandomColumnsFromTable_ValidData()
        {
            DataRow dr = SqlHelper.GetRandomColumnsFromTable("test1", "TestTable6", "TestInt, TestGuid");

            List<int> list1 = new List<int>();
            List<Guid> list2 = new List<Guid>();

            DataTable dt = DbInterface.ExecuteQueryDataTable("SELECT TestInt, TestGuid FROM TestTable6");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list1.Add(Convert.ToInt32(dt.Rows[i]["TestInt"]));
                list2.Add(new Guid(Convert.ToString(dt.Rows[i]["TestGuid"])));
            }

            Assert.That(list1.Contains(Convert.ToInt32(dr["TestInt"])));
            Assert.That(list2.Contains(new Guid(Convert.ToString(dr["TestGuid"]))));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnsFromTable with an empty table
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _062_GetRandomColumnsFromTable_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetRandomColumnsFromTable("test1", "TestTableEmpty", string.Empty); }, Throws.Exception.With.Message.EqualTo("tableName - TestTableEmpty is empty"));
        }

        /// <summary>
        /// Scenario: Try to call GetRandomColumnsFromTable with an empty column name
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _063_GetRandomColumnsFromTable_EmptyColumn()
        {
            Assert.That(delegate { SqlHelper.GetRandomColumnsFromTable("test1", "TestTable1", string.Empty); }, Throws.Exception.With.Message.EqualTo("columnName is not specified"));
        }        

        /// <summary>
        /// Scenario: Try to call GetRandomColumnsFromTable with an non-existant table
        /// Expected: Raises exception 
        /// </summary>
        [Test]
        public void _064_GetRandomColumnsFromTable_NonExistantTable()
        {
            Assert.That(delegate { SqlHelper.GetRandomColumnsFromTable("test1", "TABLEDOESNOTEXIST", string.Empty); }, Throws.Exception.With.Message.EqualTo("tableName: TABLEDOESNOTEXIST does not exist"));
        }

        /// <summary>
        /// Scenario: Test datatables with datetimes in them - prove we handle this specially and report full value
        /// Expected: Output on failure shows full datetime, not truncated string
        /// </summary>
        [Test]
        public void _065_CompareDataTables_DateTime()
        {
            DataTable dt1 = new DataTable();
            dt1.Columns.Add(new DataColumn("col1", typeof(DateTime)));

            DataTable dt2 = new DataTable();
            dt2.Columns.Add(new DataColumn("col1", typeof(DateTime)));

            dt1.Rows.Add(new object[] { DateTime.Now });
            dt2.Rows.Add(new object[] { DateTime.Now.AddMilliseconds(1) });

            Assert.That(SqlHelper.CompareDataTables(dt1, dt2), Is.False);
        }

        /// <summary>
        /// Scenario: Create a database with no FKs and cleardown all tables without an xml table list
        /// Expected: All tables get cleared without error
        /// </summary>
        [Test]
        public void _066_ClearDataInDatabase_All_WithoutXmlList_NoFK()
        {
            string scriptPath = @"Helper.Test\SQL\SetUpTestDBWithDataAndNoFK.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "testnofk", "MITTestNoFK", scriptPath, true);

            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotParent"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild2"), Is.True);

            SqlHelper.ClearDataInDatabase("testnofk", true, string.Empty);

            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotParent"), Is.False);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild"), Is.False);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild2"), Is.False);
       }

        /// <summary>
        /// Scenario: Create a database with FKs and cleardown all tables without an xml table list
        /// Expected: Exception will occur as parent will be deleted before child
        /// </summary>
        [Test]
        public void _067_ClearDataInDatabase_All_WithoutXmlList_WithFK()
        {
            string scriptPath = @"Helper.Test\SQL\SetUpTestDBWithDataAndFK.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "testwithfk", "MITTestWithFK", scriptPath, true);

            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete1"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete2"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete3"), Is.True);

            string expectedMessage = "The table: TestDataDelete2 (child table of TestDataDelete1) still contains data. Add that child table to the FK XML table list.";
            Assert.That(delegate { SqlHelper.ClearDataInDatabase("testwithfk", true, string.Empty); }, Throws.Exception.With.Message.EqualTo(expectedMessage));
        }

        /// <summary>
        /// Scenario: Create a database with FKs and cleardown all tables with an xml table list
        /// Expected: All tables get cleared without error
        /// </summary>
        [Test]
        public void _068_ClearDataInDatabase_All_WithXmlList_WithFK()
        {
            string scriptPath = @"Helper.Test\SQL\SetUpTestDBWithDataAndFK.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "testwithfk", "MITTestWithFK", scriptPath, true);

            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete1"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete2"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete3"), Is.True);

            SqlHelper.ClearDataInDatabase("testwithfk", true, @"Helper.Test\XML\ClearDown.xml");

            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete1"), Is.False);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete2"), Is.False);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete3"), Is.False);
        }

        /// <summary>
        /// Scenario: Create a database with no FKs and cleardown all tables with an xml table list unless the node has a nodelete="y" attribute
        /// Expected: All tables except the nodelete table get cleared without error
        /// </summary>
        [Test]
        public void _069_ClearDataInDatabase_All_WithXmlListWithNoDeletes_NoFK()
        {
            string scriptPath = @"Helper.Test\SQL\SetUpTestDBWithDataAndNoFK.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "testnofk", "MITTestNoFK", scriptPath, true);

            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotParent"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild2"), Is.True);

            SqlHelper.ClearDataInDatabase("testnofk", true, @"Helper.Test\XML\ClearDownWithNoDelete.xml");

            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotParent"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild2"), Is.False);
        }

        /// <summary>
        /// Scenario: Create a database with FKs and cleardown all tables with an xml table list
        /// Expected: All tables get cleared without error
        /// </summary>
        [Test]
        public void _070_ClearDataInDatabase_JustList_WithXmlList_WithFK()
        {
            string scriptPath = @"Helper.Test\SQL\SetUpTestDBWithDataAndFK.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "testwithfk", "MITTestWithFK", scriptPath, true);

            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete1"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete2"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete3"), Is.True);

            SqlHelper.ClearDataInDatabase("testwithfk", false, @"Helper.Test\XML\ClearDown2.xml");

            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete1"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete2"), Is.False);
            Assert.That(DbSystem.DoesUserTableHaveData("testwithfk", "TestDataDelete3"), Is.False);
        }

        /// <summary>
        /// Scenario: Create a database with no FKs and cleardown all tables in an xml table list unless the node has a nodelete="y" attribute
        /// Expected: All tables except the nodelete table get cleared without error
        /// </summary>
        [Test]
        public void _071_ClearDataInDatabase_JustList_WithXmlListWithNoDeletes_NoFK()
        {
            string scriptPath = @"Helper.Test\SQL\SetUpTestDBWithDataAndNoFK.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "testnofk", "MITTestNoFK", scriptPath, true);

            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotParent"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild2"), Is.True);

            SqlHelper.ClearDataInDatabase("testnofk", false, @"Helper.Test\XML\ClearDownWithNoDelete.xml");

            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotParent"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild"), Is.True);
            Assert.That(DbSystem.DoesUserTableHaveData("testnofk", "TestDataNotChild2"), Is.False);
        }
       
        /// <summary>
        /// Scenario: Create the system check table in a free database twice and read data
        /// Expected: Table gets created and populated and is read back
        /// </summary>
        [Test]
        public void _072_CreateAndReadSystemCheckTable()
        {
            DateTime checkDate = DateTime.Now.AddMilliseconds(-3);

            Guid g = Guid.NewGuid();
            SqlHelper.CreateSystemCheckTable("test1", g);

            Dictionary<Guid, DateTime> dict = SqlHelper.ReadSystemCheckTable("test1");

            IEnumerator<Guid> ie = dict.Keys.GetEnumerator();
            ie.MoveNext();
            Assert.That(ie.Current, Is.EqualTo(g));
            Assert.That(dict[ie.Current].AddMilliseconds(3), Is.GreaterThanOrEqualTo(checkDate));

            // run again to test recreating when it exists
            g = Guid.NewGuid();
            SqlHelper.CreateSystemCheckTable("test1", g);

            dict = SqlHelper.ReadSystemCheckTable("test1");

            ie = dict.Keys.GetEnumerator();
            ie.MoveNext();
            Assert.That(ie.Current, Is.EqualTo(g));
            Assert.That(dict[ie.Current].AddMilliseconds(3), Is.GreaterThanOrEqualTo(checkDate));
        }  
     
        /// <summary>
        /// Scenario: Create the system check table in a free database twice and read data
        /// Expected: Table gets created and populated and is read back
        /// </summary>
        [Test]
        public void _073_ReadSystemCheckTable_TableDoesNotExist()
        {
            string scriptPath = @"Helper.Test\SQL\SetupTestDB.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "MITTest1", scriptPath, true);

            Assert.That(delegate { Dictionary<Guid, DateTime> dict = SqlHelper.ReadSystemCheckTable("test1"); }, Throws.Exception.With.Message.EqualTo("mitsysUnitTestCheck has not been created. CreateSystemCheckTable needs to be called before this method"));
        }

        /// <summary>
        /// Scenario: Call GetIdFromTable but the table is empty
        /// Expected: Exception
        /// </summary>
        [Test]
        public void _074_GetIdFromTable_Lookup_EmptyTable()
        {
            Assert.That(delegate { SqlHelper.GetIdFromTable("test4", "TestLookupEmpty", "blah"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("table - TestLookupEmpty is empty"));
        }

        /// <summary>
        /// Scenario: Call GetIdFromTable but for a non-existant value
        /// Expected: returns 0
        /// </summary>
        [Test]
        public void _075_GetIdFromTable_Lookup_NonExistantValue()
        {
            Assert.That(SqlHelper.GetIdFromTable("test4", "TestLookup", "blah"), Is.EqualTo(0));
        }

        /// <summary>
        /// Scenario: Call GetIdFromTable an value that exists
        /// Expected: returns id of look up value
        /// </summary>
        [Test]
        public void _076_GetIdFromTable()
        {
            // Lookup table
            Assert.That(SqlHelper.GetIdFromTable("test4", "TestLookup", "LookupVal 1"), Is.EqualTo(1));

            // any table - quotes in the WHERE clause
            Assert.That(SqlHelper.GetIdFromTable("test4", "MyTable", "MyTableString", "I am Spartacus", DbType.String), Is.EqualTo(1));

            // any table - no quotes in the WHERE clause
            Assert.That(SqlHelper.GetIdFromTable("test4", "MyTable", "MyTableYesNo", "0", DbType.Int32), Is.EqualTo(2));
        }

        /// <summary>
        /// Scenario: Call GetLookUpIdFromTable an value that exists
        /// Expected: returns id of look up value
        /// </summary>
        [Test]
        public void _077_GetRowFromTable()
        {
            DataTable dt = SqlHelper.GetRowFromTable("test4", "TestLookup", 1);
            Assert.That(dt.Rows.Count, Is.EqualTo(1));
            Assert.That(dt.Columns.Count, Is.EqualTo(2));
            Assert.That(dt.Rows[0]["TestLookupId"], Is.EqualTo(1));
            Assert.That(dt.Rows[0]["TestLookupCode"], Is.EqualTo("LookupVal 1"));
        }

        /// <summary>
        /// Scenario: Call GetAllRowsFromLookupTable
        /// Expected: A DataTable of all rows
        /// </summary>
        [Test]
        public void _078_GetAllRowsFromLookupTable()
        {
            DataTable dt = SqlHelper.GetAllRowsFromLookupTable("test4", "TestLookup");
            Assert.That(dt.Rows.Count, Is.EqualTo(2));
            Assert.That(dt.Columns.Count, Is.EqualTo(2));
            Assert.That(dt.Rows[0]["TestLookupId"], Is.EqualTo(1));
            Assert.That(dt.Rows[0]["TestLookupCode"], Is.EqualTo("LookupVal 1"));
            Assert.That(dt.Rows[1]["TestLookupId"], Is.EqualTo(2));
            Assert.That(dt.Rows[1]["TestLookupCode"], Is.EqualTo("LookupVal 2"));
        }
    }
}
