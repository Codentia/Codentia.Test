using Codentia.Common;

namespace Codentia.Test.Objects
{
    /// <summary>
    /// Test Order
    /// </summary>
    public class TestOrder : IOrder
    {
        private TestAddress _address = null;
        private TestContactDetails _contact = null;
        private decimal _total = 0.0m;
        private string _reference = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestOrder"/> class.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="paymentAddress">The payment address.</param>
        /// <param name="contact">The contact.</param>
        /// <param name="total">The total.</param>
        public TestOrder(string reference, TestAddress paymentAddress, TestContactDetails contact, decimal total)
        {
            _reference = reference;
            _address = paymentAddress;
            _contact = contact;
            _total = total;
        }

        #region IOrder Members

        /// <summary>
        /// Gets the payment address.
        /// </summary>
        /// <value>
        /// The payment address.
        /// </value>
        public IAddress PaymentAddress
        {
            get
            {
                return _address;
            }
        }

        /// <summary>
        /// Gets the contact.
        /// </summary>
        /// <value>
        /// The contact.
        /// </value>
        public IContactDetails Contact
        {
            get
            {
                return _contact;
            }
        }

        /// <summary>
        /// Gets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public decimal Total
        {
            get
            {
                return _total;
            }
        }

        /// <summary>
        /// Gets the reference.
        /// </summary>
        /// <value>
        /// The reference.
        /// </value>
        public string Reference
        {
            get
            {
                return _reference;
            }
        }

        #endregion
    }
}
