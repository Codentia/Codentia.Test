using Codentia.Common;

namespace Codentia.Test.Objects
{
    /// <summary>
    /// Test Address class
    /// </summary>
    public class TestAddress : IAddress
    {
        private int _id;
        private string _city;
        private int _countryId;
        private string _county;
        private string _town;
        private string _street;
        private string _houseName;
        private string _firstname;
        private string _lastname;
        private string _country;
        private string _postcode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAddress"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="firstname">The firstname.</param>
        /// <param name="lastname">The lastname.</param>
        /// <param name="houseName">Name of the house.</param>
        /// <param name="street">The street.</param>
        /// <param name="town">The town.</param>
        /// <param name="city">The city.</param>
        /// <param name="county">The county.</param>
        /// <param name="country">The country.</param>
        /// <param name="countryId">The country id.</param>
        /// <param name="postcode">The postcode.</param>
        public TestAddress(int id, string firstname, string lastname, string houseName, string street, string town, string city, string county, string country, int countryId, string postcode)
        {
            _id = id;
            _city = city;
            _countryId = countryId;
            _county = county;
            _town = town;
            _street = street;
            _houseName = houseName;
            _firstname = firstname;
            _lastname = lastname;
            _country = country;
            _postcode = postcode;
        }

        #region IAddress Members

        /// <summary>
        /// Gets the Database Id of the address
        /// </summary>
        public int AddressId
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Gets the City
        /// </summary>
        public string City
        {
            get
            {
                return _city;
            }
        }

        /// <summary>
        /// Gets the Country Id
        /// </summary>
        public int CountryId
        {
            get
            {
                return _countryId;
            }
        }

        /// <summary>
        /// Gets the County
        /// </summary>
        public string County
        {
            get
            {
                return _county;
            }
        }

        /// <summary>
        /// Gets the HouseName
        /// </summary>
        public string HouseName
        {
            get
            {
                return _houseName;
            }
        }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return _firstname;
            }
        }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        public string LastName
        {
            get
            {
                return _lastname;
            }
        }

        /// <summary>
        /// Gets the Postcode
        /// </summary>
        public string Postcode
        {
            get
            {
                return _postcode;
            }
        }

        /// <summary>
        /// Gets the Street
        /// </summary>
        public string Street
        {
            get
            {
                return _street;
            }
        }

        /// <summary>
        /// Gets the Town
        /// </summary>
        public string Town
        {
            get
            {
                return _town;
            }
        }

        /// <summary>
        /// Gets the country.
        /// </summary>
        public string Country
        {
            get
            {
                return _country;
            }
        }

        /// <summary>
        /// Concatenates the address.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="isPostCodeRequired">if set to <c>true</c> [is post code required].</param>
        /// <returns>
        /// string of concatenated address
        /// </returns>
        public string ConcatenateAddress(string delimiter, bool isPostCodeRequired)
        {
            string postcode = string.Empty;

            if (isPostCodeRequired)
            {
                postcode = string.Format("{0}{1}", delimiter, _postcode);
            }

            return string.Format("{1} {2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{9}", delimiter, _firstname, _lastname, _houseName, _street, _town, _city, _county, _country, postcode);
        }

        #endregion
    }
}
