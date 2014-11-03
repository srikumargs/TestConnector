using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Sync.Test
{
    [ExternalIdentifier("Id")]
    public class PersonAddress
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class PersonBase
    {
        public PersonBase()
        {
            Kids = new List<Person>();
            Addresses = new List<PersonAddress>();
            Contacts = new List<Person>();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Person> Kids { get; set; }
        public List<PersonAddress> Addresses { get; set; }
        public List<Person> Contacts { get; set; }
    }

    [ExternalIdentifier("Id")]
    public class Person : PersonBase
    {
    }

    [TestClass]
    public class SyncObjectTest
    {
        #region SyncObject

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SyncObject_Create_NoObject()
        {
            var x = new SyncObject(null, "Id");

            Assert.IsFalse(x == null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SyncObject_Create_NoProperty()
        {
            var p = new PersonBase { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };

            var x = new SyncObject(p);

            Assert.IsFalse(x == null);
        }

        [TestMethod]
        public void SyncObject_Create()
        {
            var p = new PersonBase { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };

            var x = new SyncObject(p, "Id");

            Assert.IsFalse(x == null);
        }

        [TestMethod]
        public void SyncObject_Create_NoName()
        {
            var p = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };

            var x = new SyncObject(p);

            Assert.IsFalse(x == null);
        }

        [TestMethod]
        public void SyncObject_Create_Children()
        {
            var p = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var c = new Person { Id = Guid.NewGuid(), FirstName = "Jimmy", LastName = "Doe" };

            p.Kids.Add(c);

            var x = new SyncObject(p, "Id");

            Assert.IsFalse(x == null);
            Assert.IsTrue(x.Children.Count == 1);
        }

        [TestMethod]
        public void SyncObject_Hash()
        {
            var p = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var c = new Person { Id = p.Id, FirstName = "john", LastName = "Doe" };

            var x = new SyncObject(p, "Id");
            var y = new SyncObject(c, "Id");

            Assert.IsFalse(x.HashKey.Equals(y.HashKey));
        }

        [TestMethod]
        public void SyncObject_ExternalId()
        {
            var p = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var c = new Person { Id = Guid.NewGuid(), FirstName = "Jimmy", LastName = "Doe" };

            p.Kids.Add(c);

            var x = new SyncObject(p, "Id");

            Assert.IsFalse(x == null);
            Assert.IsTrue(x.ExternalId.Equals(p.Id.ToString()));
            Assert.IsTrue(x.Children.Count == 1);
            Assert.IsTrue(x.Children[0].ExternalChildId.Equals(c.Id.ToString()));
        }

        [TestMethod]
        public void SyncObject_ResourceKind()
        {
            var p = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var c = new Person { Id = Guid.NewGuid(), FirstName = "Jimmy", LastName = "Doe" };

            p.Kids.Add(c);

            var x = new SyncObject(p, "Id");

            Assert.IsFalse(x == null);
            Assert.IsTrue(String.IsNullOrEmpty(x.ResourceKind));
            Assert.IsTrue(x.Children.Count == 1);
            Assert.IsTrue(x.Children[0].ResourceKind.Equals("Kids"));
        }

        #endregion
    }
}