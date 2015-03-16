using NUnit.Framework;

namespace Codentia.Test.Generator.Test
{
    /// <summary>
    /// Unit testing framework for InternetDataGenerator
    /// <see cref="InternetDataGenerator"/>
    /// </summary>
    [TestFixture]
    public class InternetDataGeneratorTest
    {
        /// <summary>
        /// Scenario: 
        /// Expected: 
        /// </summary>
        [Test]
        public void _001_GenerateEmailAddress()
        {
            string s = InternetDataGenerator.GenerateEmailAddress(5, "blah.com");
        }

        /// <summary>
        /// Scenario: 
        /// Expected: 
        /// </summary>
        [Test]
        public void _002_GenerateRandomWebAddress()
        {
            string s = InternetDataGenerator.GenerateRandomWebAddress();
        }
    }
}
