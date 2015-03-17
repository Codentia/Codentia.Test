using System;
using System.Threading;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Test.Test.Helper.Test
{  
    /// <summary>
    /// Testing fixture for NUnitHelper class. 
    /// </summary>
    [TestFixture]
    public class NUnitHelperTest
    {       
        /// <summary>
        /// Scenario: Test DoNegativeTests
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _001_DoNegativeTests_NegativeTest()
        {
            Assert.That(delegate { NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.NonExistTestClass", "TwoCheckedStringParams", string.Empty, string.Empty); }, Throws.Exception.With.Message.EqualTo("Class: Codentia.Test.Test.NonExistTestClass does not exist in Assembly: Codentia.Test.Test.dll"));
            Assert.That(delegate { NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "NoExistMethod", string.Empty, string.Empty); }, Throws.Exception.With.Message.EqualTo("No method matching name: NoExistMethod and type signature exists in class: Codentia.Test.Test.TestClass"));            
            Assert.That(delegate { NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneParamOverload", "System.Guid", string.Empty, string.Empty); }, Throws.Exception.With.Message.EqualTo("No method matching name: OneParamOverload and type signature exists in class: Codentia.Test.Test.TestClass"));
        }

        /// <summary>
        /// Scenario: Test OneCheckedIdParam
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _002_OneCheckedIdParam()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedIdParam", "myId1=[TestTable1]", string.Empty);          
        }

        /// <summary>
        /// Scenario: Test OneCheckedIdOneOptionalIdParam
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _003_OneCheckedIdOneOptionalIdParam()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedIdOneOptionalIdParam", "myId1=[TestTable1]", "myId2");          
        }

        /// <summary>
        /// Scenario: Test TwoCheckedIdParams
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _004_TwoCheckedIdParams()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "TwoCheckedIdParams", "myId1=[TestTable1],myId2=[TestTable3]", string.Empty);          
        }

        /// <summary>
        /// Scenario: Test OneCheckedStringParam
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _005_OneCheckedStringParam()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedStringParam", string.Empty, string.Empty);          
        }

        /// <summary>
        /// Scenario: Test TwoCheckedStringParams
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _006_TwoCheckedStringParams()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "TwoCheckedStringParams", string.Empty, string.Empty);          
        }

        /// <summary>
        /// Scenario: Test TwoCheckedStringParams
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _007_OneCheckedStringParamWithLength()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedStringParamWithLength", "myString1=~~20~~", string.Empty);
        }

        /// <summary>
        /// Scenario: Test OneCheckedIdAndOtherTypes
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _008_OneCheckedIdAndOtherTypes()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedIdAndOtherTypes", "myId1=[TestTable1]", string.Empty);
        }

        /// <summary>
        /// Scenario: Test OneCheckedStringParamWithExistsCheck
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _009_OneCheckedStringParamWithExistsCheck()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedStringParamWithExistsCheck", "myString1=~~20~~$$NONEXISTANT$$", string.Empty);
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedStringParamWithExistsCheck", "myString1=$$NONEXISTANT$$~~20~~", string.Empty);
        }

        /// <summary>
        /// Scenario: Overload Test
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _010_OverloadTest()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneParamOverload", "System.Int32", "myId1=[TestTable1]", string.Empty);            
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneParamOverload", "System.String", "myString1=$$NONEXISTANT$$", string.Empty);
        }

        /// <summary>
        /// Scenario: Test OneCheckedIdParam (as 002, but with a casing error)
        /// Expected: Should do no asserts, but should output a warning to console
        /// </summary>
        [Test]
        public void _011_OneCheckedIdParam_CaseError()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedIdParam", "MYId1=[TestTable1]", string.Empty);
        }

        /// <summary>
        /// Scenario: Test a method with a DateTime parameter set to DateTime.Now
        /// Expected: Executes without error
        /// </summary>
        [Test]
        public void _012_WithDateTime()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneDateTimeParam", "string1=,dateTime=NOW", string.Empty);            
        }

        /// <summary>
        /// Scenario: Test IsCandidateForClearDown with a fresh database (ie no system test table) 
        /// Expected: returns true
        /// </summary>
        [Test]
        public void _013_IsCandidateForClearDown_True_NoSystemTable()
        {
            string scriptPath = @"Helper.Test\SQL\SetupTestDB.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "CETest1", scriptPath, true);  
            Assert.That(NUnitHelper.IsCandidateForClearDown("test1", "my.blah.dll"), Is.True);
        }

        /// <summary>
        /// Scenario: Test IsCandidateForClearDown with a new database, create system test table and windows temp file with different contents
        /// Expected: returns true
        /// </summary>
        [Test]
        public void _014_IsCandidateForClearDown_True_SystemTable_DifferentGuid()
        {
            string scriptPath = @"Helper.Test\SQL\SetupTestDB.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "CETest1", scriptPath, true);

            Guid g1 = Guid.NewGuid();
            Guid g2 = Guid.NewGuid();

            SqlHelper.CreateSystemCheckTable("test1", g1);
            FileHelper.CreateWindowsTempFile("my.blah.dll", g2.ToString());

            Assert.That(NUnitHelper.IsCandidateForClearDown("test1", "my.blah.dll"), Is.True);
        }

        /// <summary>
        /// Scenario: Test IsCandidateForClearDown with a new database, create system test table and windows temp file with same contents but passed time threshold
        /// Expected: returns true
        /// </summary>
        [Test]
        public void _015_IsCandidateForClearDown_True_SystemTable_SameGuid_PassedThreshold()
        {
            string scriptPath = @"Helper.Test\SQL\SetupTestDB.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "CETest1", scriptPath, true);

            Guid g1 = Guid.NewGuid();            

            SqlHelper.CreateSystemCheckTable("test1", g1);
            FileHelper.CreateWindowsTempFile("my.blah.dll", g1.ToString());

            Thread.Sleep(2100);

            Assert.That(NUnitHelper.IsCandidateForClearDown("test1", "my.blah.dll"), Is.True);
        }

        /// <summary>
        /// Scenario: Test IsCandidateForClearDown with a new database, create system test table and windows temp file with same contents but within time threshold
        /// Expected: returns false
        /// </summary>
        [Test]
        public void _016_IsCandidateForClearDown_True_SystemTable_SameGuid_UnderThreshold()
        {
            string scriptPath = @"Helper.Test\SQL\SetupTestDB.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "CETest1", scriptPath, true);

            Guid g1 = Guid.NewGuid();

            SqlHelper.CreateSystemCheckTable("test1", g1);
            FileHelper.CreateWindowsTempFile("my.blah.dll", g1.ToString());

            Assert.That(NUnitHelper.IsCandidateForClearDown("test1", "my.blah.dll"), Is.False);
        }

        /// <summary>
        /// Scenario: Test UnitTestDataChecker with a cleardown
        /// Expected: returns true
        /// </summary>
        [Test]
        public void _017_UnitTestDataChecker_True_FullCleardown()
        {
            string scriptPath = @"Helper.Test\SQL\SetUpTestDBWithDataAndNoFK.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "CETest1", scriptPath, true);           
    
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotParent"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild2"), Is.EqualTo(1));

            Assert.That(NUnitHelper.UnitTestDataChecker("test1", true, true, string.Empty), Is.True);

            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotParent"), Is.EqualTo(0));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild"), Is.EqualTo(0));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild2"), Is.EqualTo(0));
        }

        /// <summary>
        /// Scenario: Test UnitTestDataChecker with a cleardown
        /// Expected: returns true
        /// </summary>
        [Test]
        public void _018_UnitTestDataChecker_True_PartialCleardown()
        {
            string scriptPath = @"Helper.Test\SQL\SetUpTestDBWithDataAndNoFK.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "CETest1", scriptPath, true);

            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotParent"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild2"), Is.EqualTo(1));

            Assert.That(NUnitHelper.UnitTestDataChecker("test1", true, false, @"Helper.Test\XML\ClearDown3.xml"), Is.True);

            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotParent"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild2"), Is.EqualTo(0));
        }

        /// <summary>
        /// Scenario: Test UnitTestDataChecker without a cleardown
        /// Expected: returns false
        /// </summary>
        [Test]
        public void _019_UnitTestDataChecker_False()
        {
            string scriptPath = @"Helper.Test\SQL\SetUpTestDBWithDataAndNoFK.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "CETest1", scriptPath, true);            

            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotParent"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild2"), Is.EqualTo(1));

            Guid g1 = Guid.NewGuid();

            SqlHelper.CreateSystemCheckTable("test1", g1);
            FileHelper.CreateWindowsTempFile("Codentia.Test.Test.dll", g1.ToString());

            Assert.That(NUnitHelper.UnitTestDataChecker("test1", false, false, string.Empty), Is.False);

            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotParent"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild"), Is.EqualTo(1));
            Assert.That(SqlHelper.GetRowCountFromTable("test1", "TestDataNotChild2"), Is.EqualTo(1));
        }

        /// <summary>
        /// Scenario: Test OneCheckedByteParam
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _020_OneCheckedByteParam()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedByteParam", string.Empty, string.Empty);
        }

        /// <summary>
        /// Scenario: Test OneCheckedByteOneOptionalByteParam
        /// Expected: Asserts work
        /// </summary>
        [Test]
        public void _021_OneCheckedByteOneOptionalByteParam()
        {
            NUnitHelper.DoNegativeTests("test1", "Codentia.Test.Test.dll", "Codentia.Test.Test.TestClass", "OneCheckedByteOneOptionalByteParam", string.Empty, "myByte2");
        }
    }
}
