using System;
using System.Diagnostics;
using Codentia.Test.Helper;
using NUnit.Framework;

namespace Codentia.Test.Test.Helper.Test
{
    /// <summary>
    /// AssemblyHelper Test
    /// </summary>
    [TestFixture]
    public class AssemblyHelperTest
    {
         /// <summary>
        /// Scenario: Run GetBaseObject 
        /// Expected: Runs without error
        /// </summary>
        [Test]
        public void _001_GetBaseObject()
        {
            Type type = AssemblyHelper.GetClass("Codentia.Test.Test.dll", "Codentia.Test.Test.TestInstanceClass");
            object obj = AssemblyHelper.GetBaseObject(type);
        }

        /// <summary>
        /// Scenario: 
        /// Expected: 
        /// </summary>
        [Test]
        public void _002_GetCallingAssemblyName()
        {
            string callingAssemblyName = TestCallingMethod();
            Assert.That(callingAssemblyName, Is.EqualTo("Codentia.Test.Test.dll"));
        }

        private string TestCallingMethod()
        {
            return AssemblyHelper.GetCallingAssemblyName(new StackTrace());
        }
    }
}
