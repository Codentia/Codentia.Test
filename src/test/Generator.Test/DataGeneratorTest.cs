using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Codentia.Common.Helper;
using NUnit.Framework;

namespace Codentia.Test.Generator.Test
{
    /// <summary>
    /// Unit testing framework for DataGenerator
    /// <see cref="DataGenerator"/>
    /// </summary>
    [TestFixture]
    public class DataGeneratorTest
    {
        private char _literal = Convert.ToChar(@"\");

        /// <summary>
        /// Scenario: Run GenerateRandomInteger with intMax less than intMin
        /// Expected: intMax must be greater than intMin exception raised
        /// </summary>
        [Test]
        public void _001_GenerateRandomInteger_IntMaxLessThanIntMin()
        {
            Assert.That(delegate { int i = DataGenerator.GenerateRandomInteger(300, 200); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("intMax 200 must be greater than intMin 300"));            
        }

        /// <summary>
        /// Scenario: Run GenerateRandomInteger with intMax equals intMin
        /// Expected: intMax cannot be the same as intMin exception raised
        /// </summary>
        [Test]
        public void _002_GenerateRandomInteger_IntMaxEqualsIntMin()
        {
            Assert.That(delegate { int i = DataGenerator.GenerateRandomInteger(300, 300); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("intMax 300 cannot be the same as intMin"));            
        }

        /// <summary>
        /// Scenario: Run GenerateRandomInteger with valid Params
        /// Expected: runs successfully
        /// </summary>
        [Test]
        public void _003_GenerateRandomInteger_ValidParams()
        {
            int i = DataGenerator.GenerateRandomInteger(200, 300);

            Assert.That(i, Is.GreaterThanOrEqualTo(200));
            Assert.That(i, Is.LessThanOrEqualTo(300));            
        }

        /// <summary>
        /// Scenario: Call the RandomInteger method twice in a row with the same arguments.
        /// Expected: Different values
        /// </summary>
        [Test]
        public void _003b_GenerateRandomInteger_Determinism()
        {
            int i1 = DataGenerator.GenerateRandomInteger(200, 300);
            int i2 = DataGenerator.GenerateRandomInteger(200, 300);

            Assert.That(i1, Is.Not.EqualTo(i2));
        }

        /// <summary>
        /// Scenario: Run GenerateRandomString with invalid stringLength
        /// Expected: stringLength must be greater than or equal to 1 exception raised
        /// </summary>
        [Test]
        public void _004_GenerateRandomString_ValidParams()
        {
            Assert.That(delegate { string s1 = DataGenerator.GenerateRandomString(-10, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("stringLength must be greater than or equal to 1"));
            Assert.That(delegate { string s1 = DataGenerator.GenerateRandomString(0, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("stringLength must be greater than or equal to 1"));            
        }

        /// <summary>
        /// Scenario: Run GenerateRandomString with valid stringLength and null ommittedList
        /// Expected: String is generated and does not contain literal 
        /// </summary>
        [Test]
        public void _005_GenerateRandomString_NullOmittedList()
        {
            string s1 = DataGenerator.GenerateRandomString(20, null);
            Console.WriteLine("Generated string: {0}", s1);

            Assert.That(s1.Length, Is.EqualTo(20));
            Assert.That(s1.IndexOf(_literal), Is.EqualTo(-1));

            string s2 = DataGenerator.GenerateRandomString(20, null);
            Console.WriteLine("Generated string: {0}", s2);
            
            Assert.That(s2.Length, Is.EqualTo(20));
            Assert.That(s1, Is.Not.EqualTo(s2));            
            Assert.That(s2.IndexOf(_literal), Is.EqualTo(-1));
        }

        /// <summary>
        /// Scenario: Run GenerateRandomString with valid stringLength and an omittedList (10 iterations)
        /// Expected: String is generated and no characters from omittedList 
        /// </summary>
        [Test]
        public void _006_GenerateRandomStringWithAnOmittedList()
        {
            List<char> omittedList = new List<char>();
            omittedList.Add('a');
            omittedList.Add('Z');
            omittedList.Add('3');
            omittedList.Add('[');
            omittedList.Add('}');

            List<string> results = new List<string>();

            for (int i = 10; i < 20; i++)
            {
                string s = DataGenerator.GenerateRandomString(i, omittedList);
                Console.WriteLine("Generated string: {0}", s);

                Assert.That(s.Length, Is.EqualTo(i));
                Assert.That(s.IndexOf(_literal), Is.EqualTo(-1));

                for (int j = 0; j < s.Length; j++)
                {
                    char c = Convert.ToChar(s.Substring(j, 1));
                    Assert.That(omittedList.Contains(c), Is.False, "Generated string contains a character from the omittedList");
                }

                if (results.Count == 0)
                {
                    results.Add(s);
                }
                else
                {
                    Assert.That(results.Contains(s), Is.False, "Random string already created");
                    results.Add(s);
                }
            }

            omittedList = GetTestPunctuationOmissionList();
            results = new List<string>();

            for (int i = 10; i < 20; i++)
            {
                string s = DataGenerator.GenerateRandomString(i);
                Console.WriteLine("Generated string: {0}", s);

                Assert.That(s.Length, Is.EqualTo(i));
                Assert.That(s.IndexOf(_literal), Is.EqualTo(-1));

                for (int j = 0; j < s.Length; j++)
                {
                    char c = Convert.ToChar(s.Substring(j, 1));
                    Assert.That(omittedList.Contains(c), Is.False, "Generated string contains a character from the omittedList");
                }

                if (results.Count == 0)
                {
                    results.Add(s);
                }
                else
                {
                    Assert.That(results.Contains(s), Is.False, "Random string already created");
                    results.Add(s);
                }
            }
        }

        /// <summary>
        /// Scenario: Run GenerateRandomString with for lengths of 6 and 8 excluding amp; gtr, ltr, ` and \ literal char 
        /// Expected: A valid password string is generated 
        /// </summary>
        [Test]
        public void _007_GeneratePassword()
        {
            string s1 = DataGenerator.GeneratePassword(6);
            Console.WriteLine("Generated string: {0}", s1);

            Assert.That(s1.Length, Is.EqualTo(6));

            List<char> li = GetTestPunctuationOmissionList();
            IEnumerator<char> ie = li.GetEnumerator();

            while (ie.MoveNext())
            {
                Assert.That(s1.Contains(ie.Current.ToString()), Is.False, "Excluded punctuation included");
            }

            //// Sleep to avoid duplicate string being created
            //// Thread.Sleep(30);

            string s2 = DataGenerator.GeneratePassword(8);
            Console.WriteLine("Generated string: {0}", s2);
            Assert.That(s2.Length, Is.EqualTo(8));
            Assert.That(s2.IndexOf(_literal), Is.EqualTo(-1));

            IEnumerator<char> ie2 = li.GetEnumerator();
            while (ie2.MoveNext())
            {
                Assert.That(s2.Contains(ie2.Current.ToString()), Is.False, "Excluded punctuation included");
            }
        }

        /// <summary>
        /// Scenario: Run RetrieveRandomStringFromXmlDoc with an invalid element name
        /// Expected: Element name does not exist exception raised
        /// </summary>
        [Test]
        public void _008_RetrieveRandomStringFromXmlDoc_InvalidElementName()
        {
            StringBuilder sbXml = new StringBuilder();
            sbXml.Append("<root>");
            sbXml.Append("<street>Way</street>");
            sbXml.Append("<street>Road</street>");
            sbXml.Append("<street>Street</street>");
            sbXml.Append("<street>Grove</street>");
            sbXml.Append("<street>Avenue</street>");
            sbXml.Append("</root>");

            XmlDocument xml = XMLHelper.GetXmlDoc(sbXml.ToString(), "streets");
            Assert.That(delegate { string s1 = DataGenerator.RetrieveRandomStringFromXmlDoc(xml, "road"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("elementName: road does not exist in the document"));
        }

        /// <summary>
        /// Scenario: Run RetrieveRandomStringFromXmlDoc with a valid element name
        /// Expected: Runs successfully 
        /// </summary>
        [Test]
        public void _009_RetrieveRandomStringFromXmlDoc_ValidElementName()
        {
            StringBuilder sbXml = new StringBuilder();
            sbXml.Append("<root>");
            sbXml.Append("<street>Way</street>");
            sbXml.Append("<street>Road</street>");
            sbXml.Append("<street>Street</street>");
            sbXml.Append("<street>Grove</street>");
            sbXml.Append("<street>Avenue</street>");
            sbXml.Append("</root>");
            string[] arr = { "Way", "Road", "Street", "Grove", "Avenue" };

            // try 10 loops
            for (int i = 0; i < 10; i++)
            {
                XmlDocument xml = XMLHelper.GetXmlDoc(sbXml.ToString(), "streets");
                string s = DataGenerator.RetrieveRandomStringFromXmlDoc(xml, "street");
                Console.WriteLine("Retrieved string: {0}", s);

                bool stringRetrieved = false;
                for (int j = 0; j < arr.Length; j++)
                {
                    if (s == arr[j])
                    {
                        stringRetrieved = true;
                    }
                }

                Assert.That(stringRetrieved, Is.True, "A valid string has not been retrieved");
            }
        }

        /// <summary>
        /// Scenario: Run GenerateRandomCurrency
        /// Expected: Runs successfully 
        /// </summary>
        [Test]
        public void _010_GenerateRandomCurrency()
        {
            for (int i = 0; i < 10; i++)
            {
                decimal amount = DataGenerator.GenerateRandomCurrency(10000);
                Console.WriteLine("Retrieved amount: {0}", amount);

                Assert.That(amount, Is.GreaterThanOrEqualTo(0M));
                Assert.That(amount, Is.LessThanOrEqualTo(100M));
            }

            for (int i = 0; i < 10; i++)
            {
                decimal amount = DataGenerator.GenerateRandomCurrency(100, 200);
                Console.WriteLine("Retrieved amount: {0}", amount);

                Assert.That(amount, Is.GreaterThanOrEqualTo(1M));
                Assert.That(amount, Is.LessThanOrEqualTo(2M));
            }
        }

        /// <summary>
        /// Scenario: Run GenerateEmail with invalid length
        /// Expected: Runs with relevant exceptions 
        /// </summary>
        [Test]
        public void _011_GenerateEmail_InvalidLength()
        {
            // 0
            Assert.That(delegate { DataGenerator.GenerateEmail(0, string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailNameLength: 0 is not valid"));

            // -1
            Assert.That(delegate { DataGenerator.GenerateEmail(-1, string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("emailNameLength: -1 is not valid"));
        }

        /// <summary>
        /// Scenario: Run GenerateEmail with invalid domain strings
        /// Expected: Runs with relevant exceptions  
        /// </summary>
        [Test]
        public void _012_GenerateEmail_InvalidDomain()
        {
            // empty
            Assert.That(delegate { DataGenerator.GenerateEmail(10, string.Empty); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("domain is not specified"));

            // null
            Assert.That(delegate { DataGenerator.GenerateEmail(10, null); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("domain is not specified"));

            // contains @ char
            Assert.That(delegate { DataGenerator.GenerateEmail(10, "@blah.com"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("domain name cannot contain @ symbol"));

            // no . char
            Assert.That(delegate { DataGenerator.GenerateEmail(10, "blah"); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("domain name must include at least one . symbol"));
        }

        /// <summary>
        /// Scenario: Run GenerateEmail with valid params
        /// Expected: Runs successfully and doesnt contain excluded punctuation in non-domain part of email
        /// </summary>
        [Test]
        public void _013_GenerateEmail_ValidParams()
        {
            List<char> li = GetTestPunctuationOmissionList();

            for (int i = 0; i < 21; i++)
            {
                string emailAddress = DataGenerator.GenerateEmail(i + 3, "mattchedit.com");

                Assert.That(emailAddress.Contains("@mattchedit.com"), Is.True, "must contain domain name");

                string primary = emailAddress.Replace("@mattchedit.com", string.Empty);

                IEnumerator<char> ie = li.GetEnumerator();

                while (ie.MoveNext())
                {
                    Assert.That(primary.Contains(ie.Current.ToString()), Is.False, "Excluded punctuation included");
                }
            }
        }

        /// <summary>
        /// Scenario: Run GenerateRandomText with invalid length
        /// Expected: Exceptions raised
        /// </summary>
        [Test]
        public void _014_GenerateRandomText_InvalidLength()
        {
            // 0
            Assert.That(delegate { DataGenerator.GenerateRandomText(0, false, false); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("textLength: 0 is not valid"));

            // -1
            Assert.That(delegate { DataGenerator.GenerateRandomText(-1, false, false); }, Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("textLength: -1 is not valid"));            
        }

        /// <summary>
        /// Scenario: Run GenerateRandomText with valid params
        /// Expected: Runs successfully and doesnt contain excluded punctuation
        /// </summary>
        [Test]
        public void _015_GenerateRandomText_ValidParams()
        {
            // with spaces
            string s = DataGenerator.GenerateRandomText(500, false, false);
            Assert.That(s.IndexOf(" "), Is.GreaterThanOrEqualTo(-1), string.Format("spaces expected: {0}", s));
            Assert.That(s.Length, Is.LessThanOrEqualTo(500));                       

            s = DataGenerator.GenerateRandomText(1500, false, false);            
            Assert.That(s.IndexOf(" "), Is.GreaterThanOrEqualTo(-1), string.Format("spaces expected: {0}", s));            
            Assert.That(s.Length, Is.LessThanOrEqualTo(1500));

            // with spaces but not at start or end
            s = DataGenerator.GenerateRandomText(750, false, true);
            Assert.That(s.IndexOf(" "), Is.GreaterThan(-1), string.Format("spaces expected: {0}", s));            
            Assert.That(s.Length, Is.LessThanOrEqualTo(750));
            Assert.That(s.StartsWith(" "), Is.False, string.Format("spaces not expected: {0}", s));
            Assert.That(s.EndsWith(" "), Is.False, string.Format("spaces not expected: {0}", s));

            // without spaces
            s = DataGenerator.GenerateRandomText(50, true, false);
            Assert.That(s.IndexOf(" "), Is.EqualTo(-1));            
            Assert.That(s.Length, Is.LessThanOrEqualTo(50));

            s = DataGenerator.GenerateRandomText(450, true, false);
            Assert.That(s.IndexOf(" "), Is.EqualTo(-1));            
            Assert.That(s.Length, Is.LessThanOrEqualTo(450));

            // without spaces but not at start or end
            s = DataGenerator.GenerateRandomText(650, false, true);
            Assert.That(s.IndexOf(" "), Is.GreaterThanOrEqualTo(1));
            Assert.That(s.Length, Is.LessThanOrEqualTo(650));            
            Assert.That(s.StartsWith(" "), Is.False, string.Format("spaces not expected: {0}", s));
            Assert.That(s.EndsWith(" "), Is.False, string.Format("spaces not expected: {0}", s));
        }

        /// <summary>
        /// Scenario: Run GenerateBoolean
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _016_GenerateRandomBoolean()
        {
            bool passed = false;
            bool b = DataGenerator.GenerateRandomBoolean();
            if (b)
            {
                while (!passed)
                {
                    if (!DataGenerator.GenerateRandomBoolean())
                    {
                        passed = true;
                    }
                }
            }
            else
            {
                while (!passed)
                {
                    if (DataGenerator.GenerateRandomBoolean())
                    {
                        passed = true;
                    }
                }
            }
        }

        /// <summary>
        /// Scenario: Run GenerateBoolean
        /// Expected: Runs successfully
        /// </summary>
        [Test]
        public void _016_GenerateRandomPhoneNumber()
        {
            string phone = DataGenerator.GenerateRandomPhoneNumber();
            Assert.That(phone.Length, Is.EqualTo(11));
        }

        private static List<char> GetTestPunctuationOmissionList()
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
