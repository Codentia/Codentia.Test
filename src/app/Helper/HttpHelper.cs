using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.UI;

namespace Codentia.Test.Helper
{
    /// <summary>
    /// Http Helper
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// CreateHttpContext - create an http context for testing
        /// </summary>
        /// <param name="userName">user Name</param>
        /// <returns>HttpContext object</returns>
        public static HttpContext CreateHttpContext(string userName)
        {
            TextWriter stringWriter = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(stringWriter);
            HttpRequest hr = new HttpRequest("c:\test.aspx", "http://a/a.aspx", string.Empty);
            HttpResponse hp = new HttpResponse(hw);
            HttpContext hc = new HttpContext(hr, hp);
            GenericIdentity defaultIdentity = new GenericIdentity(userName);
            IPrincipal ip = new GenericPrincipal(defaultIdentity, null);
            hc.User = ip;
            return hc;
        }
    }
}
