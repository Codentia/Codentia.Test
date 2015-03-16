using Codentia.Test.Generator;

namespace Codentia.Test.Test
{
    /// <summary>
    /// Test Instance Class
    /// </summary>
    public class TestInstanceClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestInstanceClass"/> class.
        /// </summary>
        public TestInstanceClass()
        {
            string s = DataGenerator.GeneratePassword(10);
        }

        /// <summary>
        /// Mies the test method.
        /// </summary>
        public void MyTestMethod()
        {
            string s = DataGenerator.GeneratePassword(10);
        }
    }
}
