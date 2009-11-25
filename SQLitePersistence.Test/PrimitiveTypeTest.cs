using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

using NUnit.Framework;

using net.ReinforceLab.iPhone.Utils.SQLitePersistence;

#if SD_SQLITE
using System.Data.SQLite;
#else
using Mono.Data.SQLite;
#endif


namespace net.ReinforceLab.iPhone.Utils.SQLitePersistence.Test
{
    [TestFixture]
    public class PrimitiveTypeTest
    {
        SQLitePersistenceManager createInitialiData(SQLiteConnection connection)
        { 
            var pman = new SQLitePersistenceManager(connection);
            pman.CreateTable<IPrimitiveValues>();

            // add new rows
            using (var adapter = pman.GetAdapter<IPrimitiveValues>("SELECT * FROM " + typeof(IPrimitiveValues).Name))
            {
                var tbl = new DataTable();
                adapter.Fill(tbl);
                tbl.Columns["ByteValue"].DataType = typeof(Byte);

                for (int i = 0; i < 10; i++)
                {
                    var row = tbl.NewRow();

                    var dao = new PrimitiveValuesDAO(row);
                    dao.ByteValue = (byte)i;
                    dao.IntValue = i;
                    dao.DoubleValue = i;
                    dao.ArbVal = (i % 3 == 0) ? ArbEnumType.One : (i % 3 == 1) ? ArbEnumType.Two : ArbEnumType.Three;
                    dao.StringValue = "S" + i.ToString();
                    dao.DateTimeValue = DateTime.MinValue.AddDays(i);

                    tbl.Rows.Add(row);
                }
                adapter.Update(tbl);
            }
            
            return  pman;                
        }

        [Test]
        public void ValueAccessTest()
        {
            var tmpFile = System.IO.Path.GetTempFileName();
            var connection = new SQLiteConnection(string.Format("Data Source={0}", tmpFile));
            connection.Open();

            var pman = new SQLitePersistenceManager(connection);
            pman.CreateTable<IPrimitiveValues>();

            // add new rows
            using (var adapter = pman.GetAdapter<IPrimitiveValues>( "SELECT * FROM " + typeof(IPrimitiveValues).Name))
            {
                var tbl = new DataTable();
                adapter.Fill(tbl);
                tbl.Columns["ByteValue"].DataType = typeof(Byte);

                for (int i = 0; i < 10; i++)
                {
                    var row = tbl.NewRow();
                    
                    var dao = new PrimitiveValuesDAO(row);
                    dao.ByteValue = (byte)i;
                    dao.IntValue = i;                    
                    dao.DoubleValue = i;
                    dao.ArbVal = (i % 3 == 0) ? ArbEnumType.One : (i % 3 == 1) ? ArbEnumType.Two : ArbEnumType.Three;
                    dao.StringValue = "S" + i.ToString();
                    dao.DateTimeValue = DateTime.MinValue.AddDays(i);

                    tbl.Rows.Add(row);
                }                
                adapter.Update(tbl);
            }

            // verify
            using (var adapter = pman.GetAdapter<IPrimitiveValues>("SELECT * FROM " + typeof(IPrimitiveValues).Name))
            {
                var tbl = new DataTable();                
                adapter.Fill(tbl);                

                Assert.AreEqual(10, tbl.Rows.Count);
                for(int i =0; i < 10; i ++)
                {
                    var dao = new PrimitiveValuesDAO(tbl.Rows[i]);
                    Assert.AreEqual((byte)i, dao.ByteValue);
                    Assert.AreEqual(i, dao.IntValue);                    
                    Assert.AreEqual((double)i, dao.DoubleValue);
                    ArbEnumType exp =  (i % 3 == 0) ? ArbEnumType.One : (i % 3 == 1) ? ArbEnumType.Two : ArbEnumType.Three;
                    Assert.AreEqual(exp, dao.ArbVal);
                    Assert.AreEqual("S" + i.ToString(), dao.StringValue);
                    Assert.AreEqual(DateTime.MinValue.AddDays(i), dao.DateTimeValue);
                }
            }            
        }

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
    }
}
