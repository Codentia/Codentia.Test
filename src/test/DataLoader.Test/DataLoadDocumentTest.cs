using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Test.DataLoader.Test
{
    /// <summary>
    /// DataLoadDocument Test
    /// </summary>
    [TestFixture]
    public class DataLoadDocumentTest
    {
        /// <summary>
        /// TestFixture SetUp
        /// </summary>
        [TestFixtureSetUp]
        public void _001_TestFixtureSetUp()
        {
            string scriptPath = @"Helper.Test\SQL\SetupTestDB3.sql";
            DBSetupHelper.EnsureTestDatabaseExists("master", "test3", "MITTest3", scriptPath, true);
        }

        /// <summary>
        /// Scenario: Test Execute
        /// Expected: Execute works
        /// </summary>
        [Test]
        public void _002_Execute()
        {
            DataLoadDocument dld = new DataLoadDocument("test3", @"Helper.Test\XML\TestMethodCallLoad.xml");
            dld.Execute();
        }

        /// <summary>
        /// Scenario: Test Execute
        /// Expected: Execute works
        /// </summary>
        [Test]
        public void _003_ExecuteWithError()
        {
            DataLoadDocument dld = new DataLoadDocument("test3", @"Helper.Test\XML\TestMethodCallException.xml");
            try
            {
                dld.Execute();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Scenario: Test Execute, but with a method containing elementcentric params and sub-xml
        /// Expected: Execute works
        /// </summary>
        [Test]
        public void _004_WithElementCentricParamsAndXml()
        {
            DataLoadDocument dld = new DataLoadDocument("test3", @"Helper.Test\XML\WithElementCentricParamsAndXml.xml");
            dld.Execute();
        }
    }
}
