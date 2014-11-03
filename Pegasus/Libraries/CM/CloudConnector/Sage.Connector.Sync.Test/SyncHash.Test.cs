using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sage.Connector.Sync.Test
{
	[TestClass]
	public class SyncHashTest
	{
		[TestMethod]
		public void SyncHash_Hash()
		{
            var p = new Person {Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe"};

		    var hash = SyncHash.GetHash(p);

            Assert.IsFalse(String.IsNullOrEmpty(hash));
		}

        [TestMethod]
        public void SyncHash_Reproducable()
        {
            var p1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };

            var hash1 = SyncHash.GetHash(p1);
            Assert.IsTrue(hash1.Equals(SyncHash.GetHash(p1)));
        }

        [TestMethod]
        public void SyncHash_Difference()
        {
            var p1 = new Person { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var p2 = new Person { Id = p1.Id, FirstName = "JOhn", LastName = "Doe" };

            var hash1 = SyncHash.GetHash(p1);
            Assert.IsFalse(hash1.Equals(SyncHash.GetHash(p2)));
        }
	}
}
