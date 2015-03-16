using System.Web;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Test.Test.Helper.Test
{
    /// <summary>
    /// Testing fixture for HttpHelper class. 
    /// </summary>
    [TestFixture]
    public class HttpHelperTest
    {
        /// <summary>
        /// Scenario: Create a http context with a username
        /// Expected: Successful creation
        /// </summary>
        [Test]
        public void _001_CreateHttpContext_UserName()
        {
            HttpContext hc = HttpHelper.CreateHttpContext("blah@blah.com");
            Assert.That(hc, Is.Not.Null, "Object expected");
            Assert.That(hc.User.Identity.Name, Is.EqualTo("blah@blah.com"), "Identity not created properly");
            Assert.That(hc.User.Identity.IsAuthenticated, Is.True, "true expected");
        }

         /// <summary>
        /// Scenario: Create a http context with no username
        /// Expected: Successful creation
        /// </summary>
        [Test]
        public void _002_CreateHttpContext_NoUserName()
        {
            HttpContext hc = HttpHelper.CreateHttpContext(string.Empty);
            Assert.That(hc, Is.Not.Null, "Object expected");
        }
    }
}
