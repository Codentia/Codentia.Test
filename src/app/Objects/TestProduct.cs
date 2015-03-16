using Codentia.Common;

namespace Codentia.Test.Objects
{
    /// <summary>
    /// Test Product
    /// </summary>
    public struct TestProduct : IProduct
    {
        private string _category;
        private string _code;
        private string _description;
        private string _imageFullSizeUrl;
        private string _imageThumbNailUrl;
        private string _merchant;
        private decimal _price;
        private int _stockLevel;
        private decimal _weight;
        private string _title;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestProduct"/> struct.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="imageFullSizeUrl">The image full size URL.</param>
        /// <param name="imageThumbNailUrl">The image thumb nail URL.</param>
        /// <param name="merchant">The merchant.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantityInStock">The quantity in stock.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="category">The category.</param>
        public TestProduct(string code, string title, string description, string imageFullSizeUrl, string imageThumbNailUrl, string merchant, decimal price, int quantityInStock, decimal weight, string category)
        {
            _code = code;
            _title = title;
            _description = description;
            _imageFullSizeUrl = imageFullSizeUrl;
            _imageThumbNailUrl = imageThumbNailUrl;
            _merchant = merchant;
            _price = price;
            _stockLevel = quantityInStock;
            _weight = weight;
            _category = category;
        }

        #region IProduct Members

        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category
        {
            get
            {
                return _category;
            }
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        public string Code
        {
            get
            {
                return _code;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Gets or sets the image full size URL.
        /// </summary>
        /// <value>
        /// The image full size URL.
        /// </value>
        public string ImageFullSizeUrl
        {
            get
            {
                return _imageFullSizeUrl;
            }

            set
            {
                _imageFullSizeUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the image thumbnail URL.
        /// </summary>
        /// <value>
        /// The image thumbnail URL.
        /// </value>
        public string ImageThumbnailUrl
        {
            get
            {
                return _imageThumbNailUrl;
            }

            set
            {
                _imageThumbNailUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the merchant.
        /// </summary>
        /// <value>
        /// The merchant.
        /// </value>
        public string Merchant
        {
            get
            {
                return _merchant;
            }

            set
            {
                _merchant = value;
            }
        }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public decimal Price
        {
            get
            {
                return _price;
            }

            set
            {
                _price = value;
            }
        }

        /// <summary>
        /// Gets or sets the quantity in stock.
        /// </summary>
        /// <value>
        /// The quantity in stock.
        /// </value>
        public int QuantityInStock
        {
            get
            {
                return _stockLevel;
            }

            set
            {
                _stockLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public decimal Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                _weight = value;
            }
        }

        #endregion
    }
}
