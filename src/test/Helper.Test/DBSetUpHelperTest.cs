using System;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Test.Test.Helper.Test
{
    /// <summary>
    /// DBSetUpHelper Test
    /// </summary>
    [TestFixture]
    public class DBSetUpHelperTest
    {
        /// <summary>
        /// Scenario: Run EnsureTestDatabaseExists 
        /// Expected: Runs without error
        /// </summary>
        [Test]
        public void _001_EnsureTestDatabaseExists()
        {
            string scriptPath = @"Helper.Test\SQL\SetupTestDB.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "CETest1", scriptPath, true);

            scriptPath = @"Helper.Test\SQL\SetupTestDB2.sql";            
            DBSetupHelper.EnsureTestDatabaseExists("master", "test2", "CETest2", scriptPath, true);

            // test that it doesnt get rebuilt again
            scriptPath = @"Helper.Test\SQL\SetupTestDB2.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test2", "CETest2", scriptPath, false);
        }

        /// <summary>
        /// Scenario: Run EnsureTestDatabaseExists 
        /// Expected: Runs without error
        /// </summary>
        [Test]
        public void _002_EnsureTestDatabaseExists_NoMasterDbInConnString()
        {
            string scriptPath = @"Helper.Test\SQL\SetupTestDB.sql";
            Assert.That(delegate { DBSetupHelper.EnsureTestDatabaseExists("test1", "test1", "CETest1", scriptPath, true); }, Throws.InstanceOf<Exception>().With.Message.EqualTo("Connection String must be a master database"));
        }

        /// <summary>
        /// Scenario: Call with an erroneous script which fails
        /// Expected: Auto-retry occurs, followed by failure
        /// </summary>
        [Test]
        public void _003_EnsureTestDatabaseExists_WithError_Coverage()
        {
            string scriptPath = @"Helper.Test\SQL\BrokenSetup.sql";
            Assert.That(delegate { DBSetupHelper.EnsureTestDatabaseExists("master", "test1", "CETestBroken", scriptPath, true); }, Throws.InstanceOf<System.Data.SqlClient.SqlException>());
        }
    }
}
