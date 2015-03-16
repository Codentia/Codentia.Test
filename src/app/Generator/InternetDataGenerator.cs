using System;
using System.Xml;
using Codentia.Common.Helper;

namespace Codentia.Test.Generator
{
    /// <summary>
    /// Common methods for creating Internet related data
    /// </summary>
    public static class InternetDataGenerator
    {
        /// <summary>
        /// Generate Email Address
        /// </summary>
        /// <param name="seed">seed for generating randomness</param>
        /// <param name="domainName">Domain Name</param>
        /// <returns>string - email address</returns>
        public static string GenerateEmailAddress(int seed, string domainName)
        {
            return DataGenerator.GenerateEmail(seed + 3, domainName);
        }

        /// <summary>
        /// Create a random Web Address
        /// </summary>
        /// <returns>string - web address</returns>
        public static string GenerateRandomWebAddress()
        {
            string domainType = DataGenerator.RetrieveRandomStringFromXmlDoc(InternetDataGenerator.GetDomainType(), "domaintype");
            return string.Format("http://www.{0}.{1}", Guid.NewGuid().ToString().Replace("-", string.Empty), domainType);
        }                

        /// <summary>
        /// Get WebAddressDomain list and return as Xml Document
        /// </summary>
        /// <returns>XmlDocument object</returns>
        public static XmlDocument GetDomainType()
        {
            string list = "com,co.uk,org,biz,tv,au,uk.com";
            return XMLHelper.ConvertCSVStringToXmlDoc(list, "domaintype");
        }
    }
}
