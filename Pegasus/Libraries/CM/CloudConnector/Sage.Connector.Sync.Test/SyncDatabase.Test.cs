using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.Sync.Common;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Sync.Test
{
    [TestClass]
    public class SyncDatabaseTest
    {
        private const string TenantId = "52c4537d-9b9d-4e8d-b337-3fa9ba1414d1";

        /// <summary>
        /// Utility method to generate sample data.
        /// </summary>
        /// <param name="recordCount">The number of records to create.</param>
        /// <param name="active">True if the record should be marked as active, false if it should be marked deleted.</param>
        private void GenerateTestData(int recordCount, bool active)
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.BeginSession("Customers", 3);
                try
                {
                    var conn = db.Connection;

                    conn.Execute("DELETE FROM SyncCustomers");

                    db.BeginSync(SyncType.Internal);
                    try
                    {
                        for (int i = 0; i < recordCount; i++)
                        {
                            var p = new Person
                            {
                                Id = Guid.NewGuid(),
                                FirstName = String.Format("Test{0}", i),
                                LastName = "Testing"
                            };
                            var s = new SyncObject(p);

                            var row = db.ReadMetaRecord(s, SyncReadFlags.AutoAdd);

                            row[SyncFields.HashKey] = s.HashKey;
                            row[SyncFields.Active] = active;
                            row[SyncFields.Deleted] = !active;

                            db.WriteMetaRecords();
                        }
                    }
                    finally
                    {
                        db.EndSync(true);
                    }
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        #region SyncDatabase

        [TestMethod]
        public void SyncDatabase_Create()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SyncDatabase_Create_NoTenant()
        {
            using (var db = new SyncDatabase(null, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SyncDatabase_Create_NoPath()
        {
            using (var db = new SyncDatabase(TenantId, ""))
            {
                db.OpenDatabase();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void SyncDatabase_Create_BadPath()
        {
            using (var db = new SyncDatabase(TenantId, "x:\\does not exist"))
            {
                db.OpenDatabase();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SyncDatabase_BeginSession_NoResource()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.BeginSession(null, 0);
                try
                {

                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SyncDatabase_BeginSession_BadCloudId()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.BeginSession("Customers", (-1));
                try
                {

                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        [TestMethod]
        public void SyncDatabase_BeginSession()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.BeginSession("Customers", 3);
                try
                {

                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        [ExpectedException(typeof (InvalidOperationException))]
        public void SyncDatabase_BeginSession_BadState1()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.BeginSession("Customers", 3);
                db.CloseDatabase();
            }
        }

        [TestMethod]
        public void SyncDatabase_StateTransition()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.CloseDatabase();
                db.OpenDatabase();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SyncDatabase_BeginSession_BadState2()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.EndSession();
                db.CloseDatabase();
            }
        }

        [TestMethod]
        public void SyncDatabase_BeginSync()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();

                db.BeginSession("Customers", 3);
                try
                {
                    db.BeginSync(SyncType.Internal);
                    try
                    {
                        Assert.IsTrue(db.Mode == SyncMode.FullSync);
                        Assert.IsTrue(db.CloudTick == 3);
                        Assert.IsTrue(db.Tick > 0);
                    }
                    finally
                    {
                        db.EndSync(true);
                    }
                }
                finally
                {
                    db.EndSession();
                }

                db.CloseDatabase();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SyncDatabase_BeginSync_NoSession()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.BeginSync(SyncType.Internal);
                try
                {

                }
                finally
                {
                    db.EndSync(true);
                }
            }
        }

        [TestMethod]
        public void SyncDatabase_RevertTick()
        {
            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.BeginSession("Customers", 10);
                try
                {
                    db.BeginSync(SyncType.Internal);
                    try
                    {
                        
                    }
                    finally
                    {
                        db.EndSync(true);
                    }
                    
                    var tick = db.Tick;
                    db.RevertDigestTick();
                    Assert.IsTrue(db.Tick == --tick);
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        [TestMethod]
        public void SyncDatabase_ReadWrite()
        {
            GenerateTestData(10, true);
        }

        [TestMethod]
        public void SyncDatabase_ActiveKeys()
        {
            GenerateTestData(10, true);

            using (var db = new SyncDatabase(TenantId, AppDomain.CurrentDomain.BaseDirectory))
            {
                db.OpenDatabase();
                db.BeginSession("Customers", 10);
                try
                {
                    var keys = db.GetActiveResourceKeys("Customers");
                    Assert.IsTrue(keys.Count == 10);

                    db.BeginSync(SyncType.Internal);
                    try
                    {
                        
                    }
                    finally
                    {
                        var deleted = db.EndSync(true);
                        Assert.IsTrue(deleted.Count == 10);
                    }

                    keys = db.GetActiveResourceKeys("Customers");
                    Assert.IsTrue(keys.Count == 0);
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        #endregion
    }
}
