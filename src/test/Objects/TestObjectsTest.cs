using Codentia.Test.Objects;
using NUnit.Framework;

namespace Codentia.Test.Test.Objects
{
    /// <summary>
    /// TestFixture - for TestObjects
    /// </summary>
    [TestFixture]
    public class TestObjectsTest
    {
        /// <summary>
        /// Scenario: Test object
        /// Expected: Properties return values from constructor
        /// </summary>
        [Test]
        public void _001_ContactDetails()
        {
            TestContactDetails tcd = new TestContactDetails("test001@mattchedit.com");
            Assert.That(tcd.EmailAddress, Is.EqualTo("test001@mattchedit.com"));
        }

        /// <summary>
        /// Scenario: Test object
        /// Expected: Properties return values from constructor
        /// </summary>
        [Test]
        public void _002_Address()
        {
            TestAddress ta = new TestAddress(1, "First", "Last", "House", "Street", "Town", "City", "County", "Country", 2, "Postcode");
            Assert.That(ta.AddressId, Is.EqualTo(1));
            Assert.That(ta.FirstName, Is.EqualTo("First"));
            Assert.That(ta.LastName, Is.EqualTo("Last"));
            Assert.That(ta.HouseName, Is.EqualTo("House"));
            Assert.That(ta.Street, Is.EqualTo("Street"));
            Assert.That(ta.Town, Is.EqualTo("Town"));
            Assert.That(ta.City, Is.EqualTo("City"));
            Assert.That(ta.County, Is.EqualTo("County"));
            Assert.That(ta.Country, Is.EqualTo("Country"));
            Assert.That(ta.CountryId, Is.EqualTo(2));
            Assert.That(ta.Postcode, Is.EqualTo("Postcode"));
        }

        /// <summary>
        /// Scenario: Test object
        /// Expected: Properties return values from constructor
        /// </summary>
        [Test]
        public void _003_Product()
        {
            TestProduct tp = new TestProduct("code", "title", "description", "full", "thumb", "merchant", 10.0m, 5, 15.0m, "category1");
            Assert.That(tp.Code, Is.EqualTo("code"));
            Assert.That(tp.Title, Is.EqualTo("title"));
            Assert.That(tp.Description, Is.EqualTo("description"));
            Assert.That(tp.ImageFullSizeUrl, Is.EqualTo("full"));
            Assert.That(tp.ImageThumbnailUrl, Is.EqualTo("thumb"));
            Assert.That(tp.Merchant, Is.EqualTo("merchant"));
            Assert.That(tp.Price, Is.EqualTo(10.0m));
            Assert.That(tp.QuantityInStock, Is.EqualTo(5));
            Assert.That(tp.Weight, Is.EqualTo(15.0m));
            Assert.That(tp.Category, Is.EqualTo("category1"));

            // now update (setters)
            tp.Title = "title2";
            tp.Description = "description2";
            tp.ImageFullSizeUrl = "full2";
            tp.ImageThumbnailUrl = "thumb2";
            tp.Merchant = "merchant2";
            tp.Price = 20.0m;
            tp.QuantityInStock = 10;
            tp.Weight = 30.0m;

            // and re-test
            Assert.That(tp.Code, Is.EqualTo("code"));
            Assert.That(tp.Title, Is.EqualTo("title2"));
            Assert.That(tp.Description, Is.EqualTo("description2"));
            Assert.That(tp.ImageFullSizeUrl, Is.EqualTo("full2"));
            Assert.That(tp.ImageThumbnailUrl, Is.EqualTo("thumb2"));
            Assert.That(tp.Merchant, Is.EqualTo("merchant2"));
            Assert.That(tp.Price, Is.EqualTo(20.0m));
            Assert.That(tp.QuantityInStock, Is.EqualTo(10));
            Assert.That(tp.Weight, Is.EqualTo(30.0m));
            Assert.That(tp.Category, Is.EqualTo("category1"));
        }

        /// <summary>
        /// Scenario: Test object
        /// Expected: Properties return values from constructor
        /// </summary>
        [Test]
        public void _004_Order()
        {
            TestContactDetails tcd = new TestContactDetails("test004@mattchedit.com");
            TestAddress ta = new TestAddress(1, "First", "Last", "House", "Street", "Town", "City", "County", "Country", 2, "Postcode");

            TestOrder to = new TestOrder("reference", ta, tcd, 100.0m);
            Assert.That(to.Reference, Is.EqualTo("reference"));
            Assert.That(to.Total, Is.EqualTo(100.0m));
            Assert.That(to.PaymentAddress.ConcatenateAddress(",", true), Is.EqualTo(ta.ConcatenateAddress(",", true)));
            Assert.That(to.Contact.EmailAddress, Is.EqualTo(tcd.EmailAddress));
        }
    }
}
