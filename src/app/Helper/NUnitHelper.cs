using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using Codentia.Common.Data;
using Codentia.Common.Helper;
using Codentia.Test.Generator;
using Codentia.Test.Reflector;
using NUnit.Framework;

namespace Codentia.Test.Helper
{
    /// <summary>
    /// Class for doing common NUnit methods
    /// </summary>
    public class NUnitHelper
    {
        /// <summary>
        /// Do the standard negative tests for integers and strings
        /// integer - 0, -1 and non-existant
        /// string - null, empty, optionally length if default value is ~~N where N is length to be measured, and optional non-existant value to check for
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="assemblyName">Assembly dll name e.g. Codentia.Test.Test.dll</param>
        /// <param name="className">Fully qualified class name e.g. Codentia.Test.Test.TestClass</param>
        /// <param name="methodName">Method Name e.g. OneCheckedStringParamWithLength</param>
        /// <param name="defaultValues">comma delimited name/value pairs with = e.g. myId1=[TestTable1],myId2=[TestTable3] - use [] for the table for existant and non-existant ids</param>
        /// <param name="omittedParameters">comma delimited list of int or string parameters to be omitted from negative tests</param>
        public static void DoNegativeTests(string database, string assemblyName, string className, string methodName, string defaultValues, string omittedParameters)
        {
            DoNegativeTests(database, assemblyName, className, methodName, null, defaultValues, omittedParameters);
        }

        /// <summary>
        /// Do the standard negative tests for integers and strings
        /// integer - 0, -1 and non-existant
        /// string - null, empty, optionally length if default value is ~~N~~ where N is length to be measured, and optional non-existant value to check for with pattern $$NONEXISTANTSTRING$$
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="assemblyName">Assembly dll name e.g. Codentia.Test.Test.dll</param>
        /// <param name="className">Fully qualified class name e.g. Codentia.Test.Test.TestClass</param>
        /// <param name="methodName">Method Name e.g. OneCheckedStringParamWithLength</param>
        /// <param name="signature">For uses with overloads - comma delimited list of types to match overload being tested</param>
        /// <param name="defaultValues">comma delimited name/value pairs with = e.g. myId1=[TestTable1],myId2=[TestTable3] - use [] for the table for existant and non-existant ids</param>
        /// <param name="omittedParameters">comma delimited list of int or string parameters to be omitted from negative tests</param>
        public static void DoNegativeTests(string database, string assemblyName, string className, string methodName, string signature, string defaultValues, string omittedParameters)
        {
            ParameterCheckHelper.CheckIsValidString(database, "database", false);
            ParameterCheckHelper.CheckIsValidString(assemblyName, "assemblyName", false);
            ParameterCheckHelper.CheckIsValidString(className, "className", false);
            ParameterCheckHelper.CheckIsValidString(methodName, "methodName", false);

            Method method = new Method(database, assemblyName, className, methodName, signature, defaultValues, omittedParameters);         
            
            for (int i = 0; i < method.ParameterDataTable.Rows.Count; i++)
            {
                DataRow dr = method.ParameterDataTable.Rows[i];
                if (Convert.ToBoolean(dr["omitted"]) == false)
                {
                    DoAllNegativeTestsForParameter(method, dr);
                }
            }                                     
        }

        /// <summary>
        /// Unit Test Data Checker
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="alwaysClearDownData">always clear down data</param>
        /// <param name="clearDownAllData">if true, clear down all data, otherwise use just the tables in xmlTableList</param>
        /// <param name="xmlTableListFilePath">(optional) filepath of an xml document root cleardowntables, element name - cleardowntable</param>
        /// <returns>bool - true if data cleared down, otherwise false</returns>
        public static bool UnitTestDataChecker(string database, bool alwaysClearDownData, bool clearDownAllData, string xmlTableListFilePath)
        {
            return UnitTestDataChecker(database, alwaysClearDownData, clearDownAllData, AssemblyHelper.GetCallingAssemblyName(new StackTrace()), xmlTableListFilePath);
        }           

        /// <summary>
        /// Is the data in a position to be cleared down?
        /// <para></para>
        /// The conditions are :-
        /// <para></para>
        /// 1) If no system test table exists - return true
        /// <para></para>
        /// 2) If the system test table exists
        /// <para></para>
        /// a) if the windows temp file exists and the checkguid inside is different to the one is the system test table - return true
        /// <para></para>
        /// b) if checkguid is same in table and file but the configured [CleardownDataThresholdSeconds] threshold has passed - return true
        /// <para></para>
        /// 3) otherwise return false
        /// </summary>
        /// <param name="database">the database</param>
        /// <param name="callingAssembly">the name of the assembly calling the wrapper procedure e.g. my.blah.dll</param>
        /// <returns>bool - true if data needs to be cleared down, otherwise false</returns>
        internal static bool IsCandidateForClearDown(string database, string callingAssembly)
        {            
            if (DbSystem.DoesUserTableExist(database, SqlHelper.SystemCheckTable) == false)
            {
                return true;
            }
            else
            {
                Dictionary<Guid, DateTime> dict = SqlHelper.ReadSystemCheckTable(database);
                
                IEnumerator<Guid> ie = dict.Keys.GetEnumerator();
                ie.MoveNext();
                Guid checkGuid = ie.Current;
                DateTime checkDateTime = dict[ie.Current];

                if (FileHelper.DoesWindowsTempFileExist(callingAssembly, checkGuid.ToString()) == false)
                {
                    return true;
                }
                else
                {
                    int cleardownDataThresholdSeconds = 180;
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CleardownDataThresholdSeconds"]))
                    {
                        cleardownDataThresholdSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["CleardownDataThresholdSeconds"]);
                    }

                    if (checkDateTime.AddSeconds(cleardownDataThresholdSeconds) < DateTime.Now)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool UnitTestDataChecker(string database, bool alwaysClearDownData, bool clearDownAllData, string callingAssembly, string xmlTableListFilePath)
        {
            bool clearDown = alwaysClearDownData;
            if (alwaysClearDownData == false)
            {
                clearDown = IsCandidateForClearDown(database, callingAssembly);
            }

            if (clearDown)
            {
                SqlHelper.ClearDataInDatabase(database, clearDownAllData, xmlTableListFilePath);
            }

            return clearDown;
        }
        
        private static void DoAllNegativeTestsForParameter(Method method, DataRow paramRow)
        {
            string paramType = Convert.ToString(paramRow["type"]);

            switch (paramType)
            {
                case "System.Int32":
                    DoIntegerTests(method, paramRow);                    
                    break;

                case "System.String":
                    DoStringTests(method, paramRow);
                    break;

                case "System.Byte":
                    DoByteTests(method, paramRow);
                    break;
            }
        }

        private static void DoIntegerTests(Method method, DataRow paramRow)            
        {
            string name = Convert.ToString(paramRow["name"]);
            string defaultValue = Convert.ToString(paramRow["defaultValue"]);
            string paramType = Convert.ToString(paramRow["type"]);
            
            // 0
            object[] arguments = method.GetArgumentArray(name, "0");
            string expectedMessage = string.Format("{0}: 0 is not valid", name);
            string conditionMessage = string.Format("Param: {0}, Value: {1}", name, "0");
            DoAssert(method, arguments, expectedMessage, conditionMessage);

            // -1
            arguments = method.GetArgumentArray(name, "-1");
            expectedMessage = string.Format("{0}: -1 is not valid", name);
            conditionMessage = string.Format("Param: {0}, Value: {1}", name, "-1");
            DoAssert(method, arguments, expectedMessage, conditionMessage);
            
            // non-existant
            if (defaultValue.Contains("[") && defaultValue.Contains("]"))
            {
                string nonExistantValue = SqlHelper.GetDatabaseNonExistantValueFromToken(method.Database, defaultValue);
                arguments = method.GetArgumentArray(name, nonExistantValue);
                expectedMessage = string.Format("{0}: {1} does not exist", name, nonExistantValue);
                conditionMessage = string.Format("Param: {0}, Value: {1}", name, nonExistantValue);
                DoAssert(method, arguments, expectedMessage, conditionMessage);
            }
        }

        private static void DoStringTests(Method method, DataRow paramRow)
        {
            string name = Convert.ToString(paramRow["name"]);
            string defaultValue = Convert.ToString(paramRow["defaultValue"]);            

            // empty
            object[] arguments = method.GetArgumentArray(name, string.Empty);
            string expectedMessage = string.Format("{0} is not specified", name);
            string conditionMessage = string.Format("Param: {0}, Value: {1}", name, "empty");
            DoAssert(method, arguments, expectedMessage, conditionMessage);

            if (!method.IsOverload)
            {
                // null
                arguments = method.GetArgumentArray(name, null);
                expectedMessage = string.Format("{0} is not specified", name);
                conditionMessage = string.Format("Param: {0}, Value: {1}", name, "null");
                DoAssert(method, arguments, expectedMessage, conditionMessage);
            }

            if (defaultValue.Contains("~~"))
            {
                int maxChars = Convert.ToInt32(ExtractStringValue(defaultValue, "~~"));
                string useValue = DataGenerator.GenerateRandomString(maxChars + 1);
                arguments = method.GetArgumentArray(name, useValue);
                expectedMessage = string.Format("{0} exceeds maxLength {1} as it has {2} chars", name, maxChars, maxChars + 1);
                conditionMessage = string.Format("Param: {0}, Value: {1}", name, useValue);
                DoAssert(method, arguments, expectedMessage, conditionMessage);
            }

            if (defaultValue.Contains("$$"))
            {
                string useValue = ExtractStringValue(defaultValue, "$$");
                arguments = method.GetArgumentArray(name, useValue);
                expectedMessage = string.Format("{0}: {1} does not exist", name, useValue);
                conditionMessage = string.Format("Param: {0}, Value: {1}", name, useValue);
                DoAssert(method, arguments, expectedMessage, conditionMessage);
            }
        }

        private static void DoByteTests(Method method, DataRow paramRow)
        {
            string name = Convert.ToString(paramRow["name"]);
            string defaultValue = Convert.ToString(paramRow["defaultValue"]);
            string paramType = Convert.ToString(paramRow["type"]);

            // 0
            object[] arguments = method.GetArgumentArray(name, "0");
            string expectedMessage = string.Format("{0}: 0 is not valid", name);
            string conditionMessage = string.Format("Param: {0}, Value: {1}", name, "0");
            DoAssert(method, arguments, expectedMessage, conditionMessage);
        }

        private static string ExtractStringValue(string value, string token)
        {
            string retVal = value;

            int startPos = retVal.IndexOf(token);

            if (startPos > -1)
            {
                retVal = retVal.Substring(startPos + 2, retVal.Length - (startPos + 2));
            }

            int endPos = retVal.IndexOf(token);

            if (endPos > -1)
            {
                retVal = retVal.Substring(0, endPos);
            }

            return retVal;
        }
        
        [Test]
        private static void DoAssert(Method method, object[] arguments, string expectedMessage, string testConditionMessage)
        {
            string message = string.Empty;
            
            try
            {                
                method.Invoke(arguments);
            }
            catch (Exception ex)
            {
                message = ex.InnerException.Message;
            }

            string outputLine = string.Format("DoAssert: Method: {0},  Test Condition: {1} - FAILED - {2}", method.Name, testConditionMessage, message);

            if (expectedMessage == message)
            {
                outputLine = string.Format("DoAssert: Method: {0},  Test Condition: {1} - PASSED", method.Name, testConditionMessage);
            }
           
            Console.WriteLine(outputLine);            

            Assert.That(message, Is.EqualTo(expectedMessage));
        }       
    }
}
