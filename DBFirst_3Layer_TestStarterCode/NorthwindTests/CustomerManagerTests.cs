using NUnit.Framework;
using NorthwindBusiness;
using NorthwindData;
using System.Linq;

namespace NorthwindTests
{
    public class CustomerTests
    {
        CustomerManager _customerManager;
        [SetUp]
        public void Setup()
        {
            _customerManager = new CustomerManager();
            // remove test entry in DB if present
            using (var db = new NorthwindContext())
            {
                var selectedCustomers =
                from c in db.Customers
                where c.CustomerId == "MANDA"
                select c;

                db.Customers.RemoveRange(selectedCustomers);
                db.SaveChanges();
            }
        }

        [Test]
        public void WhenANewCustomerIsAdded_TheNumberOfCustomersIncreasesBy1()
        {
            using (var db = new NorthwindContext())
            {
                var numberOfCustomer = db.Customers.Count();
                _customerManager.Create("MANDA", "Nish", "Sparta");
                var numberOfCustomerAfter = db.Customers.Count();
                Assert.That(numberOfCustomer + 1, Is.EqualTo(numberOfCustomerAfter));
            }
        }

        [Test]
        public void WhenANewCustomerIsAdded_TheirDetailsAreCorrect()
        {
            using (var db = new NorthwindContext())
            {
                
                _customerManager.Create("MANDA", "Nish", "Sparta");
                var customer = db.Customers.Find("MANDA");
                Assert.That(customer.ContactName, Is.EqualTo("Nish"));
                Assert.That(customer.CompanyName, Is.EqualTo("Sparta"));
            }
        }

        [Test]
        public void WhenACustomersDetailsAreChanged_TheDatabaseIsUpdated()
        {
            using (var db = new NorthwindContext())
            {

                _customerManager.Create("MANDA", "Nish", "Sparta");
                var wasUpdated = _customerManager.Update("MANDA", "Nish Mandal","UK","Birmingham", "B1 1AA");
                var customer = db.Customers.Find("MANDA");

                Assert.That(customer.ContactName, Is.EqualTo("Nish Mandal"));
                Assert.That(customer.Country, Is.EqualTo("UK"));
                Assert.That(customer.City, Is.EqualTo("Birmingham"));
                Assert.That(customer.PostalCode, Is.EqualTo("B1 1AA"));
            }
        }

        [Test]
        public void WhenACustomerIsUpdated_SelectedCustomerIsUpdated()
        {

            using (var db = new NorthwindContext())
            {
                _customerManager.Create("MANDA", "Nish", "Sparta");
                var wasUpdated = _customerManager.Update("MANDA", "Nish Mandal", "UK", "Birmingham", "B1 1AA");
                var customer = db.Customers.Find("MANDA");


                Assert.That(customer.CustomerId, Is.EqualTo(_customerManager.SelectedCustomer.CustomerId));
                Assert.That(customer.ContactName, Is.EqualTo(_customerManager.SelectedCustomer.ContactName));
                Assert.That(customer.Country, Is.EqualTo(_customerManager.SelectedCustomer.Country));
                Assert.That(customer.City, Is.EqualTo(_customerManager.SelectedCustomer.City));
                Assert.That(customer.PostalCode, Is.EqualTo(_customerManager.SelectedCustomer.PostalCode));
            }
        }

        [Test]
        public void WhenACustomerIsNotInTheDatabase_Update_ReturnsFalse()
        {
            _customerManager.Create("MANDA", "Nish", "Sparta");
            var wasUpdated = _customerManager.Update("MANDALLLL", "Nish Mandal", "UK", "Birmingham", "B1 1AA");
            

            Assert.That(wasUpdated, Is.False);
        }

        [Test]
        public void WhenACustomerIsRemoved_TheNumberOfCustomersDecreasesBy1()
        {
            using (var db = new NorthwindContext())
            {
                var numberOfCustomer = db.Customers.Count();
                _customerManager.Create("MANDA", "Nish", "Sparta");
                _customerManager.Delete("MANDA");
                var numberOfCustomerAfter = db.Customers.Count();
                Assert.That(numberOfCustomer, Is.EqualTo(numberOfCustomerAfter));
            }
        }

        [Test]
        public void WhenACustomerIsRemoved_TheyAreNoLongerInTheDatabase()
        {
            using (var db = new NorthwindContext())
            {
                var numberOfCustomer = db.Customers.Count();
                _customerManager.Create("MANDA", "Nish", "Sparta");
                _customerManager.Delete("MANDA");
                var customer = db.Customers.Find("MANDA");
                Assert.That(customer, Is.Null);
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (var db = new NorthwindContext())
            {
                var selectedCustomers =
                from c in db.Customers
                where c.CustomerId == "MANDA"
                select c;

                db.Customers.RemoveRange(selectedCustomers);
                db.SaveChanges();
            }
        }
    }
}