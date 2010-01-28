using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

using NUnit.Framework;

using com.ReinforceLab.SQLitePersistence;
using Mono.Data.Sqlite;

namespace com.ReinforceLab.SQLitePersistence.Test
{
    [TestFixture]
    public class PrimitiveTypeTest
    {
        SQLitePersistenceManager createInitialiData(SqliteConnection connection)
        { 
            var pman = new SQLitePersistenceManager(connection);
            pman.CreateTeble<PrimitiveValuesDAO>();

            // add ows
            var rows = new List<PrimitiveValuesDAO>();
            for (int i = 0; i < 10; i++)
            {
                var dao = new PrimitiveValuesDAO();
                dao.ByteValue = (byte)i;
                dao.IntValue = i;
                dao.DoubleValue = i;
                dao.ArbVal = (i % 3 == 0) ? ArbEnumType.One : (i % 3 == 1) ? ArbEnumType.Two : ArbEnumType.Three;
                dao.StringValue = "S" + i.ToString();
                dao.DateTimeValue = DateTime.MinValue.AddDays(i);
                rows.Add(dao);
            }
            pman.AcceptChanges<PrimitiveValuesDAO>(rows);
                        
            return  pman;                
        }

        [Test]
        public void ValueAccessTest()
        {
            var tmpFile = System.IO.Path.GetTempFileName();
            var connection = new SqliteConnection(string.Format("Data Source={0}", tmpFile));
            var pman = createInitialiData(connection);

            // verify
            var rows = pman.Read<PrimitiveValuesDAO>("SELECT * FROM " + typeof(PrimitiveValuesDAO).Name).ToArray();
            Assert.AreEqual(10, rows.Length);
            for (int i = 0; i < 10; i++)
            {
                var dao = rows[i];
                Assert.AreEqual((byte)i, dao.ByteValue);
                Assert.AreEqual(i, dao.IntValue);
                Assert.AreEqual((double)i, dao.DoubleValue);
                ArbEnumType exp = (i % 3 == 0) ? ArbEnumType.One : (i % 3 == 1) ? ArbEnumType.Two : ArbEnumType.Three;
                Assert.AreEqual(exp, dao.ArbVal);
                Assert.AreEqual("S" + i.ToString(), dao.StringValue);
                Assert.AreEqual(DateTime.MinValue.AddDays(i), dao.DateTimeValue);
            }

        }
        /*
        [Test]
        public void WhereTest()
        {
            var tmpFile = System.IO.Path.GetTempFileName();
            var connection = new SQLiteConnection(string.Format("Data Source={0}", tmpFile));
            connection.Open();

            var pman = createInitialiData(connection);

            // verify            
            using (var adapter = pman.GetAdapter<IPrimitiveValues>("SELECT * FROM " + typeof(IPrimitiveValues).Name + " WHERE IntValue = 4 "))
            {
                var tbl = new DataTable();
                adapter.Fill(tbl);

                Assert.AreEqual(1, tbl.Rows.Count);
                var dao = new PrimitiveValuesDAO(tbl.Rows[0]);
                Assert.AreEqual(4, dao.IntValue);
                Assert.AreEqual("S4", dao.StringValue);                
            }
        }

        [Test]
        public void DeleteTest()
        {
            var tmpFile = System.IO.Path.GetTempFileName();
            var connection = new SQLiteConnection(string.Format("Data Source={0}", tmpFile));
            connection.Open();

            var pman = createInitialiData(connection);

            // delete
            using (var adapter = pman.GetAdapter<IPrimitiveValues>("SELECT * FROM " + typeof(IPrimitiveValues).Name + " WHERE IntValue = 4 "))
            {
                var tbl = new DataTable();
                adapter.Fill(tbl);
                tbl.Rows[0].Delete();
                adapter.Update(tbl);
            }

            // verify
            using (var adapter = pman.GetAdapter<IPrimitiveValues>("SELECT * FROM " + typeof(IPrimitiveValues).Name ))
            {
                var tbl = new DataTable();
                adapter.Fill(tbl);
                Assert.AreEqual(9, tbl.Rows.Count);                
            }
        }

        [Test]
        public void UpdateTest()
        {
            var tmpFile = System.IO.Path.GetTempFileName();
            var connection = new SQLiteConnection(string.Format("Data Source={0}", tmpFile));
            connection.Open();

            var pman = createInitialiData(connection);

            // modify
            using (var adapter = pman.GetAdapter<IPrimitiveValues>("SELECT * FROM " + typeof(IPrimitiveValues).Name + " WHERE IntValue = 4 "))
            {
                var tbl = new DataTable();
                adapter.Fill(tbl);
                var dao = new PrimitiveValuesDAO(tbl.Rows[0]);
                Assert.AreEqual("S4", dao.StringValue);
                dao.StringValue = "SS4";                
                adapter.Update(tbl);
            }

            // verify
            using (var adapter = pman.GetAdapter<IPrimitiveValues>("SELECT * FROM " + typeof(IPrimitiveValues).Name + " WHERE IntValue = 4 "))
            {
                var tbl = new DataTable();
                adapter.Fill(tbl);
                var dao = new PrimitiveValuesDAO(tbl.Rows[0]);
                Assert.AreEqual("SS4", dao.StringValue);
            }
        }
         * */
    }
}
