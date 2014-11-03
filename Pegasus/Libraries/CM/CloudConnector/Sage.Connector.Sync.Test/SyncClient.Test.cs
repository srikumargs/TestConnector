using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Sync.Common;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Sync.Test
{
    /// <summary>
    /// Summary description for SyncClient
    /// </summary>
    [TestClass]
    public class SyncClientTest
    {
        private const string TenantId = "52c4537d-9b9d-4e8d-b337-3fa9ba1414d1";

        private void ClearTable()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.Connection.Execute("DELETE FROM SyncCustomers");
            }
        }

        [TestMethod]
        public void SyncClient_Create()
        {
            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                client.CloseDatabase();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SyncClient_Create_NoTenant()
        {
            using (var client = new SyncClient(null, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                client.CloseDatabase();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SyncClient_Create_NoPath()
        {
            using (var client = new SyncClient(TenantId, null))
            {
                client.OpenDatabase();
                client.CloseDatabase();
            }
        }

        [TestMethod]
        public void SyncClient_TenantId()
        {
            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                Assert.IsTrue(TenantId.Equals(client.TenantId));
            }
        }

        [TestMethod]
        public void SyncClient_DataPath()
        {
            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                Assert.IsTrue(AppDomain.CurrentDomain.BaseDirectory.Equals(client.DataPath));
            }
        }

        [TestMethod]
        public void SyncClient_DatabaseState()
        {
            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                Assert.IsTrue(client.DatabaseState == SyncDbState.Closed);
                client.OpenDatabase();
                Assert.IsTrue(client.DatabaseState == SyncDbState.Opened);
            }
        }

        [TestMethod]
        public void SyncClient_BeginSession()
        {
            var session = new List<Object>();

            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                client.BeginSession(session, "Customers", 4);
                try
                {

                }
                finally
                {
                    client.EndSession();
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SyncClient_BeginSession_NoResourceKind()
        {
            var session = new List<Object>();

            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                client.BeginSession(session, null, 4);
                try
                {

                }
                finally
                {
                    client.EndSession();
                }
            }
        }

        [TestMethod]
        public void SyncClient_SessionState()
        {
            var session = new List<Object>();

            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                Assert.IsTrue(client.Mode == SyncMode.None);
                client.BeginSession(session, "Customers", 4);
                try
                {
                    Assert.IsTrue(client.CloudTick == 4);
                    Assert.IsTrue(client.ResourceKind.Equals("Customers"));
                    Assert.IsTrue(client.Tick >= client.CloudTick);
                    Assert.IsTrue(client.Mode == SyncMode.Session);
                }
                finally
                {
                    client.EndSession();
                }
            }
        }

        [TestMethod]
        public void SyncClient_DeleteTest()
        {
            var session = new List<Object>();

            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                client.BeginSession(session, "Customers", 4);
                try
                {
                    client.BeginSync(SyncType.Internal);

                    try
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var p = new Person()
                            {
                                Id = Guid.NewGuid(),
                                FirstName = String.Format("FirstName{0}", i),
                                LastName = String.Format("LastName{0}", i)
                            };

                            var address = new PersonAddress()
                            {
                                Id = Guid.NewGuid(),
                                Address = String.Format("{0} Main Street", i),
                                City = "Anaheim",
                                State = "CA."
                            };

                            p.Addresses.Add(address);

                            var contact = new Person()
                            {
                                Id = Guid.NewGuid(),
                                FirstName = String.Format("Contact FirstName{0}", i),
                                LastName = String.Format("Conatct LastName{0}", i)
                            };
                            p.Contacts.Add(contact);

                            client.SyncEntity(p);
                        }

                    }
                    finally
                    {
                        var tick = client.EndSync(true);
                        Assert.IsTrue(tick == client.Tick);
                    }
                }
                finally
                {
                    client.EndSession();
                }

                client.BeginSession(session, "Customers", 5);

                try
                {
                    client.BeginSync(SyncType.Internal);
                    try
                    {
                        /* No-op, delete all items */
                    }
                    finally
                    {
                        client.EndSync(true);
                    }

                    var deleted = client.DeletedEntities;

                    foreach (var item in deleted)
                    {
                        var deletedItem = item.Value;
                        Debug.WriteLine("ResourceKind = {0}, ExternalId = {1}, IsRoot = {2}", deletedItem.ResourceKind, deletedItem.ExternalId, deletedItem.IsRoot);
                    }

                }
                finally
                {
                    client.EndSession();
                }
            }
        }
        [TestMethod]
        public void SyncClient_DeleteChildTest()
        {
            ClearTable();

            var session = new List<Object>();
            IList<Guid> customerGuids = new List<Guid>();
            IList<Guid> addressGuids = new List<Guid>();
            IList<Guid> contactGuids = new List<Guid>();

            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                client.BeginSession(session, "Customers", 4);
                try
                {
                    client.BeginSync(SyncType.Internal);

                    try
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var custGuid = Guid.NewGuid();
                            customerGuids.Add(custGuid);
                            var p = new Person()
                            {
                                Id = custGuid,
                                FirstName = String.Format("FirstName{0}", i),
                                LastName = String.Format("LastName{0}", i)
                            };

                            for (int j = 0; j < 2; j++)
                            {
                                var addrGuid = Guid.NewGuid();

                                addressGuids.Add(addrGuid);
                                var address = new PersonAddress()
                                {
                                    Id = addrGuid,
                                    Address = String.Format("{0} Main Street", i),
                                    City = "Anaheim",
                                    State = "CA."
                                };

                                p.Addresses.Add(address);
                            }

                            var contactGuid = Guid.NewGuid();
                            contactGuids.Add(contactGuid);
                            var contact = new Person()
                            {
                                Id = contactGuid,
                                FirstName = String.Format("Contact FirstName{0}", i),
                                LastName = String.Format("Conatct LastName{0}", i)
                            };
                            p.Contacts.Add(contact);

                            client.SyncEntity(p);
                        }

                    }
                    finally
                    {
                        var tick = client.EndSync(true);
                        Assert.IsTrue(tick == client.Tick);
                    }
                }
                finally
                {
                    client.EndSession();
                }

                client.BeginSession(session, "Customers", 5);

                try
                {
                    client.BeginSync(SyncType.Internal);
                    try
                    {
                        var idx = -1;
                        for (int i = 0; i < 5; i++)
                        {
                            var p = new Person()
                            {
                                Id = customerGuids[i],
                                FirstName = String.Format("FirstName{0}", i),
                                LastName = String.Format("LastName{0}", i)
                            };
                            if (i != 4)
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    idx++;
                                    Debug.Print(idx.ToString());
                                    var address = new PersonAddress()
                                    {
                                        Id = addressGuids[idx],
                                        Address = String.Format("{0} Main Street", i),
                                        City = "Anaheim",
                                        State = "CA."
                                    };


                                    p.Addresses.Add(address);
                                }
                            }
                            else
                            {

                                for (int j = 0; j < 1; j++)
                                {
                                    idx++;
                                    Debug.Print(idx.ToString());
                                    var address = new PersonAddress()
                                    {
                                        Id = addressGuids[idx],
                                        Address = String.Format("{0} Main Street", i),
                                        City = "Anaheim",
                                        State = "CA."
                                    };


                                    p.Addresses.Add(address);
                                }
                            }
                            var contact = new Person()
                            {
                                Id = contactGuids[i],
                                FirstName = String.Format("Contact FirstName{0}", i),
                                LastName = String.Format("Conatct LastName{0}", i)
                            };
                            p.Contacts.Add(contact);
                            client.SyncEntity(p);

                        }

                    }
                    finally
                    {
                        client.EndSync(true);
                    }

                    var deleted = client.DeletedEntities;
                    Assert.AreEqual(1, deleted.Count);
                    foreach (var item in deleted)
                    {
                        var deletedItem = item.Value;
                        Debug.WriteLine("ResourceKind = {0}, ExternalId = {1}, IsRoot = {2}", deletedItem.ResourceKind, deletedItem.ExternalId, deletedItem.IsRoot);
                    }

                }
                finally
                {
                    client.EndSession();
                }
            }
        }

        [TestMethod]
        public void SyncClient_BeginSync()
        {
            var session = new List<Object>();

            ClearTable();

            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                client.BeginSession(session, "Customers", 4);
                try
                {
                    client.BeginSync(SyncType.External);
                    try
                    {
                        var p = new Person();

                        for (int i = 0; i < 20; i++)
                        {
                            p.Id = Guid.NewGuid();
                            p.FirstName = String.Format("FirstName{0}", i);
                            p.LastName = String.Format("LastName{0}", i);

                            client.SyncEntity(p);
                        }

                        Assert.IsTrue(client.SyncChangeCount == 20);

                    }
                    finally
                    {
                        var tick = client.EndSync(true);
                        Assert.IsTrue(tick == client.Tick);
                    }
                }
                finally
                {
                    client.EndSession();
                }
            }
        }

        [TestMethod]
        public void SyncClient_SyncEntity()
        {
            var session = new List<Object>();

            ClearTable();

            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                client.BeginSession(session, "Customers", 4);
                try
                {
                    client.BeginSync(SyncType.External);
                    try
                    {
                        var p = new Person
                        {
                            Id = Guid.NewGuid(),
                            FirstName = String.Format("FirstName{0}", 0),
                            LastName = String.Format("LastName{0}", 0)
                        };

                        client.SyncEntity(p);

                        Assert.IsTrue(session.Count == 1);
                        Assert.IsTrue(session[0].Equals(p));
                    }
                    finally
                    {
                        var tick = client.EndSync(true);
                        Assert.IsTrue(tick == client.Tick);
                    }
                }
                finally
                {
                    client.EndSession();
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SyncClient_SyncEntity_BadState()
        {
            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                client.OpenDatabase();
                client.BeginSession(null, "Customers", 4);
                try
                {
                    var p = new Person
                    {
                        Id = Guid.NewGuid(),
                        FirstName = String.Format("FirstName{0}", 0),
                        LastName = String.Format("LastName{0}", 0)
                    };

                    client.SyncEntity(p);
                }
                finally
                {
                    client.EndSession();
                }
            }
        }

        [TestMethod]
        public void SyncClient_RemoveEntity()
        {
            var session = new List<Object>();
            var cloudTick = 4;

            ClearTable();

            var p = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = String.Format("FirstName{0}", 0),
                LastName = String.Format("LastName{0}", 0)
            };

            using (var client = new SyncClient(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                for (var x = 0; x < 2; x++)
                {
                    client.OpenDatabase();

                    client.BeginSession(session, "Customers", cloudTick);
                    try
                    {
                        client.BeginSync((x == 0) ? SyncType.Internal : SyncType.External);
                        try
                        {
                            if (x == 0)
                            {
                                client.SyncEntity(p);
                            }
                            else
                            {
                                Assert.IsTrue(client.RemoveEntity(p.Id.ToString()));
                            }

                            Assert.IsTrue(client.SyncChangeCount == 1);
                        }
                        finally
                        {
                            cloudTick = client.EndSync(true);
                            if (x == 1)
                            {
                                Assert.IsTrue(client.DeletedEntities.Count == 1);
                            }
                        }
                    }
                    finally
                    {
                        client.EndSession();
                    }

                    client.CloseDatabase();
                }
            }
        }

    }
}
