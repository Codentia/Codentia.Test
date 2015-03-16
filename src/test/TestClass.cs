using System;
using System.Data;
using Codentia.Common.Data;
using Codentia.Common.Helper;
using Codentia.Test.Helper;

namespace Codentia.Test.Test
{
    /// <summary>
    /// Test Class
    /// </summary>
    public static class TestClass
    {
        /// <summary>
        /// Test Method with one Id parameter
        /// </summary>        
        /// <param name="myId1">my id 1</param>
        public static void OneCheckedIdParam(int myId1)
        {
            ParameterCheckHelper.CheckIsValidId(myId1, "myId1");            

            if (!SqlHelper.DoesIdExistInTable("test1", "TestTable1", myId1))
            {
                throw new Exception(string.Format("myId1: {0} does not exist", myId1));
            }
        }

        /// <summary>
        /// Test Method with two Id parameters
        /// </summary>        
        /// <param name="myId1">my Id 1</param>
        /// <param name="myId2">my Id 2</param>
        public static void OneCheckedIdOneOptionalIdParam(int myId1, int myId2)
        {
            ParameterCheckHelper.CheckIsValidId(myId1, "myId1");
            
            if (!SqlHelper.DoesIdExistInTable("test1", "TestTable1", myId1))
            {
                throw new Exception(string.Format("myId1: {0} does not exist", myId1));
            }
        }

        /// <summary>
        /// Test Method with one byte parameter
        /// </summary>        
        /// <param name="myByte1">my byte 1</param>
        public static void OneCheckedByteParam(byte myByte1)
        {
            ParameterCheckHelper.CheckIsValidByte(myByte1, "myByte1");
        }

        /// <summary>
        /// Test Method with two byte parameter
        /// </summary>        
        /// <param name="myByte1">my byte 1</param>
        /// <param name="myByte2">my byte 2</param>
        public static void OneCheckedByteOneOptionalByteParam(byte myByte1, byte myByte2)
        {
            ParameterCheckHelper.CheckIsValidId(myByte1, "myByte1");           
        }

        /// <summary>
        /// Test Method with two Id parameters
        /// </summary>
        /// <param name="myId1">my id 1</param>
        /// <param name="myId2">my id 2</param>
        public static void TwoCheckedIdParams(int myId1, int myId2)
        {
            ParameterCheckHelper.CheckIsValidId(myId1, "myId1");
            ParameterCheckHelper.CheckIsValidId(myId2, "myId2");

            if (!SqlHelper.DoesIdExistInTable("test1", "TestTable1", myId1))
            {
                throw new Exception(string.Format("myId1: {0} does not exist", myId1));
            }

            if (!SqlHelper.DoesIdExistInTable("test1", "TestTable3", myId2))
            {
                throw new Exception(string.Format("myId2: {0} does not exist", myId2));
            }
        }

        /// <summary>
        /// Test Method with one Id parameter
        /// </summary>        
        /// <param name="myString1">my string 1</param>
        public static void OneCheckedStringParam(string myString1)
        {
            ParameterCheckHelper.CheckIsValidString(myString1, "myString1", false);
        }

        /// <summary>
        /// Called when [date time param].
        /// </summary>
        /// <param name="string1">The string1.</param>
        /// <param name="dateTime">The date time.</param>
        public static void OneDateTimeParam(string string1, DateTime dateTime)
        {
            ParameterCheckHelper.CheckIsValidString(string1, "string1", false);
        }

        /// <summary>
        /// Test Method with two checked string parameters
        /// </summary>        
        /// <param name="myString1">my string 1</param>
        /// <param name="myString2">my string 2</param>
        public static void TwoCheckedStringParams(string myString1, string myString2)
        {
            ParameterCheckHelper.CheckIsValidString(myString1, "myString1", false);
            ParameterCheckHelper.CheckIsValidString(myString2, "myString2", false);
        }

        /// <summary>
        /// Test Method with one checked string parameter with length check
        /// </summary>        
        /// <param name="myString1">my string 1</param>        
        public static void OneCheckedStringParamWithLength(string myString1)
        {
            ParameterCheckHelper.CheckIsValidString(myString1, "myString1", 20, false);
        }

        /// <summary>
        /// Test Method with one checked id parameters plus params with other data types
        /// </summary>
        /// <param name="myId1">My id1.</param>
        /// <param name="myBool">if set to <c>true</c> [my bool].</param>
        /// <param name="myDateTime">My date time.</param>
        /// <param name="myGuid">My GUID.</param>
        /// <param name="myDecimal">My decimal.</param>
        public static void OneCheckedIdAndOtherTypes(int myId1, bool myBool, DateTime myDateTime, Guid myGuid, decimal myDecimal)
        {
            ParameterCheckHelper.CheckIsValidId(myId1, "myId1");

            if (!SqlHelper.DoesIdExistInTable("test1", "TestTable1", myId1))
            {
                throw new Exception(string.Format("myId1: {0} does not exist", myId1));
            }
        }

        /// <summary>
        /// Test Method with one checked string parameter with length check
        /// </summary>        
        /// <param name="myString1">my string 1</param>        
        public static void OneCheckedStringParamWithExistsCheck(string myString1)
        {
            ParameterCheckHelper.CheckIsValidString(myString1, "myString1", 20, false);

            DataTable dt = DbInterface.ExecuteQueryDataTable("test1", string.Format("SELECT * FROM TestTable1 WHERE TestString='{0}'", myString1));
            if (dt.Rows.Count == 0)
            {
                throw new Exception(string.Format("myString1: {0} does not exist", myString1));
            }
        }

        /// <summary>
        /// Test Overload Method with one Id parameter
        /// </summary>        
        /// <param name="myId1">my id 1</param>
        public static void OneParamOverload(int myId1)
        {
            ParameterCheckHelper.CheckIsValidId(myId1, "myId1");

            if (!SqlHelper.DoesIdExistInTable("test1", "TestTable1", myId1))
            {
                throw new Exception(string.Format("myId1: {0} does not exist", myId1));
            }
        }

        /// <summary>
        /// Test Overload Method with one String parameter
        /// </summary>        
        /// <param name="myString1">my string 1</param>
        public static void OneParamOverload(string myString1)
        {
            ParameterCheckHelper.CheckIsValidString(myString1, "myString1", 20, false);

            DataTable dt = DbInterface.ExecuteQueryDataTable("test1", string.Format("SELECT * FROM TestTable1 WHERE TestString='{0}'", myString1));
            if (dt.Rows.Count == 0)
            {
                throw new Exception(string.Format("myString1: {0} does not exist", myString1));
            }
        }

        /// <summary>
        /// Inserts the test parent.
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="database">The database.</param>
        /// <param name="myParentString">My parent string.</param>
        /// <param name="myParentInt">My parent int.</param>
        /// <param name="myParentGuid">My parent GUID.</param>
        /// <returns>int of id</returns>
        public static int InsertTestParent(Guid txnId, string database, string myParentString, int myParentInt, Guid myParentGuid)
        {
             DbParameter[] spParams =
                {
                    new DbParameter("@TestParentString", DbType.StringFixedLength, 100, myParentString),
                    new DbParameter("@TestParentInt", DbType.Int32, myParentInt),
                    new DbParameter("@TestParentGuid", DbType.Guid, myParentGuid),
                    new DbParameter("@TestDataLoadParentId", DbType.Int32, ParameterDirection.Output, 0)
                };

            DbInterface.ExecuteProcedureNoReturn(database, "dbo.TestDataLoadParent_Insert", spParams, txnId);

            int parentId = Convert.ToInt32(spParams[3].Value);            

            return parentId;
        }

        /// <summary>
        ///  For testing loading xml documents - test child 
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="database">The database</param>
        /// <param name="testParentId">The test parent id.</param>
        /// <param name="myChildString">My child string.</param>
        /// <param name="myChildInt">My child int.</param>
        /// <param name="myChildGuid">My child GUID.</param>
        /// <returns>int of id</returns>
        public static int InsertTestChild(Guid txnId, string database, int testParentId, string myChildString, int myChildInt, Guid myChildGuid)
        {
             DbParameter[] spParams =
                {
                    new DbParameter("@TestDataLoadParentId", DbType.Int32, testParentId),
                    new DbParameter("@TestChildString", DbType.StringFixedLength, 100, myChildString),
                    new DbParameter("@TestChildInt", DbType.Int32, myChildInt),
                    new DbParameter("@TestChildGuid", DbType.Guid, myChildGuid),
                    new DbParameter("@TestDataLoadChildId", DbType.Int32, ParameterDirection.Output, 0)
                };

            DbInterface.ExecuteProcedureNoReturn(database, "dbo.TestDataLoadChild_Insert", spParams, txnId);

            int childId = Convert.ToInt32(spParams[4].Value);            

            return childId;
        }

        /// <summary>
        ///  For testing loading xml documents - test child - test overload
        /// </summary>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="database">The database</param>
        /// <param name="testParentId">The test parent id.</param>
        /// <param name="myChildString">My child string.</param>
        /// <param name="myChildInt">My child int.</param>
        /// <returns>int of Id</returns>
        public static int InsertTestChild(Guid txnId, string database, int testParentId, string myChildString, int myChildInt)
        {
            return InsertTestChild(txnId, database, testParentId, myChildString, myChildInt, Guid.NewGuid());
        }
    }
}
