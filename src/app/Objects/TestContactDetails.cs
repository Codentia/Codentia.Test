using Codentia.Common;

namespace Codentia.Test.Objects
{
    /// <summary>
    /// Test Contact Details
    /// </summary>
    public class TestContactDetails : IContactDetails
    {
        private string _emailAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestContactDetails"/> class.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        public TestContactDetails(string emailAddress)
        {
            _emailAddress = emailAddress;
        }

        #region IContactDetails Members

        /// <summary>
        /// Gets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string EmailAddress
        {
            get
            {
                return _emailAddress;
            }
        }

        #endregion
    }
}
