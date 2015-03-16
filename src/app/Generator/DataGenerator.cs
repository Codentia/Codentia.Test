using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Codentia.Common.Helper;

namespace Codentia.Test.Generator
{
    /// <summary>
    /// Class for generating data - random or predictable data 
    /// </summary>
    public static class DataGenerator
    {
        private static Random _random = new Random();

        /// <summary>
        /// Generate a random integer between a low and high range
        /// </summary>
        /// <param name="intMin">low boundary</param>
        /// <param name="intMax">high boundary</param>
        /// <returns>int - random</returns>
        public static int GenerateRandomInteger(int intMin, int intMax)
        {
            if (intMax < intMin)
            {
                throw new ArgumentException(string.Format("intMax {0} must be greater than intMin {1}", intMax, intMin));
            }

            if (intMax == intMin)
            {
                throw new ArgumentException(string.Format("intMax {0} cannot be the same as intMin", intMax));
            }

            return _random.Next(intMin, intMax);
        }

        /// <summary>
        /// Generate a random monetary amount (decimal 2 dec places)
        /// </summary>
        /// <param name="maxCentile">maximum amount expressed as an integer of lowest monetary unit e.g. 10000 pence for GBP</param>
        /// <returns>decimal - currency</returns>
        public static decimal GenerateRandomCurrency(int maxCentile)
        {
            return GenerateRandomCurrency(0, maxCentile);
        }

        /// <summary>
        /// Generate a random monetary amount (decimal 2 dec places)
        /// </summary>
        /// <param name="minCentile">minimum amount expressed as an integer of lowest monetary unit e.g. 100 pence for GBP</param>
        /// <param name="maxCentile">maximum amount expressed as an integer of lowest monetary unit e.g. 10000 pence for GBP</param>
        /// <returns>decimal - currency</returns>
        public static decimal GenerateRandomCurrency(int minCentile, int maxCentile)
        {
            int amount = _random.Next(minCentile, maxCentile);

            return Convert.ToDecimal(amount / 100M);
        }

        /// <summary>
        /// Generate a random string of length stringLength, with standard omitting characters
        /// Cannot contain literal character \
        /// </summary>
        /// <param name="stringLength">string Length</param>
        /// <returns>string - random</returns>
        public static string GenerateRandomString(int stringLength)
        {
            List<char> list = GetPunctuationOmissionList();
            return GenerateRandomString(stringLength, list);
        }

        /// <summary>
        /// Generate a random string of length stringLength, optionally omitting characters in omittedCharacterList
        /// Cannot contain literal character \
        /// </summary>
        /// <param name="stringLength">string Length</param>
        /// <param name="omittedCharacterList">omitted Character List</param>
        /// <returns>string - random</returns>
        public static string GenerateRandomString(int stringLength, List<char> omittedCharacterList)
        {
            if (stringLength < 1)
            {
                throw new ArgumentException(string.Format("stringLength must be greater than or equal to 1", stringLength));
            }

            char literal = Convert.ToChar(@"\");

            if (omittedCharacterList == null)
            {
                omittedCharacterList = new List<char>();
            }

            omittedCharacterList.Add(literal);

            StringBuilder sb = new StringBuilder();
            char appendChar;

            int i = 0;
            while (i < stringLength)
            {
                bool append = true;

                appendChar = Convert.ToChar(Convert.ToInt32(93 * _random.NextDouble()) + 33);

                if (omittedCharacterList != null)
                {
                    if (omittedCharacterList.Contains(appendChar))
                    {
                        append = false;
                    }
                }

                if (append)
                {
                    sb.Append(appendChar);
                    i++;
                }
            }

            // Convert randomString to String and return the result.
            return sb.ToString();
        }

        /// <summary>
        /// Retrieve a string randomly from an XmlDoc for an element called elementName
        /// </summary>
        /// <param name="xmlDoc">xml Document</param>
        /// <param name="elementName">element Name</param>
        /// <returns>string - random string</returns>
        public static string RetrieveRandomStringFromXmlDoc(XmlDocument xmlDoc, string elementName)
        {
            string returnString = string.Empty;

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName(elementName);

            if (nodeList.Count == 0)
            {
                throw new ArgumentException(string.Format("elementName: {0} does not exist in the document", elementName));
            }

            int elementPos = Convert.ToInt32((nodeList.Count - 1) * _random.NextDouble());
            returnString = nodeList[elementPos].InnerText;

            return returnString;
        }

        /// <summary>
        /// Generate a random string of length mixed chars excluding punctuation
        /// </summary>
        /// <param name="passwordLength">password Length</param>
        /// <returns>string - password</returns>
        public static string GeneratePassword(int passwordLength)
        {
            List<char> omittedList = GetPunctuationOmissionList();
            return GenerateRandomString(passwordLength, omittedList);
        }

        /// <summary>
        /// Generate a random string of length mixed chars excluding punctuation appended to a domain name
        /// </summary>
        /// <param name="emailNameLength">email name length</param>
        /// <param name="domain">email domain</param>
        /// <returns>string - email</returns>
        public static string GenerateEmail(int emailNameLength, string domain)
        {
            ParameterCheckHelper.CheckIsValidId(emailNameLength, "emailNameLength");
            ParameterCheckHelper.CheckIsValidString(domain, "domain", false);

            if (domain.IndexOf("@") > -1)
            {
                throw new ArgumentException("domain name cannot contain @ symbol");
            }

            if (domain.IndexOf(".") == -1)
            {
                throw new ArgumentException("domain name must include at least one . symbol");
            }

            List<char> omittedList = GetPunctuationOmissionList();
            return string.Format("{0}@{1}", GenerateRandomString(emailNameLength, omittedList), domain);
        }

        /// <summary>
        /// Generate a random string of length mixed chars excluding punctuation separated by spaces
        /// </summary>
        /// <param name="maxTextLength">Could return less</param>
        /// <param name="omitSpacesCompletely">If true dont included spaces at all</param>
        /// <param name="noStartOrEndSpaces">Do not start or end with spaces</param>
        /// <returns>string - random text</returns>
        public static string GenerateRandomText(int maxTextLength, bool omitSpacesCompletely, bool noStartOrEndSpaces)
        {
            ParameterCheckHelper.CheckIsValidId(maxTextLength, "textLength");

            StringBuilder sb = new StringBuilder();
            List<char> list = GetPunctuationOmissionList();
            if (omitSpacesCompletely)
            {
                noStartOrEndSpaces = true;
            }

            while (sb.Length < maxTextLength + 50)
            {
                int i = GenerateRandomInteger(1, 10);
                sb.Append(GenerateRandomString(i, list));

                if (!omitSpacesCompletely)
                {
                    if (noStartOrEndSpaces)
                    {
                        if (sb.Length > 1 || sb.Length < maxTextLength - 5)
                        {
                            sb.Append(" ");
                        }
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }
            }

            string retVal = sb.ToString().Substring(1, maxTextLength);
            if (noStartOrEndSpaces)
            {
                char[] sp = { ' ' };
                retVal = retVal.TrimStart(sp);
                retVal = retVal.TrimEnd(sp);
            }

            return retVal;
        }

        /// <summary>
        /// Generate a random Boolean
        /// </summary>
        /// <returns>bool - true or false random</returns>
        public static bool GenerateRandomBoolean()
        {
            int seed = GenerateRandomInteger(0, 100);
            if (seed <= 50)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generate a Random PhoneNumber
        /// </summary>
        /// <returns>string - phone number</returns>
        public static string GenerateRandomPhoneNumber()
        {
            int part1 = GenerateRandomInteger(10000, 99999);
            int part2 = GenerateRandomInteger(10000, 99999);

            return string.Format("0{0}{1}", part1, part2);
        }

        private static List<char> GetPunctuationOmissionList()
        {
            List<char> charOmmissionList = new List<char>();
            charOmmissionList.Add('!');
            charOmmissionList.Add('@');
            charOmmissionList.Add('<');
            charOmmissionList.Add('>');
            charOmmissionList.Add('?');
            charOmmissionList.Add('+');
            charOmmissionList.Add('-');
            charOmmissionList.Add(',');
            charOmmissionList.Add('(');
            charOmmissionList.Add(')');
            charOmmissionList.Add('{');
            charOmmissionList.Add('}');
            charOmmissionList.Add('^');
            charOmmissionList.Add(':');
            charOmmissionList.Add(';');
            charOmmissionList.Add('\'');
            charOmmissionList.Add('"');
            charOmmissionList.Add(' ');
            charOmmissionList.Add('/');
            charOmmissionList.Add('[');
            charOmmissionList.Add(']');
            charOmmissionList.Add('`');
            charOmmissionList.Add('*');
            charOmmissionList.Add('#');
            charOmmissionList.Add('%');
            charOmmissionList.Add('$');
            charOmmissionList.Add('&');
            charOmmissionList.Add('=');
            charOmmissionList.Add('~');
            charOmmissionList.Add('|');

            return charOmmissionList;
        }
    }
}
