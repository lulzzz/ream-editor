﻿using LinqEditor.Core.Settings;
using LinqEditor.Test.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqEditor.Core.Tests
{
    [TestFixture]
    public class ConnectionTests
    {
        [TestCase(
            "",
            DatabaseTestData.Connstr1,
            @".\sqlexpress.Opera56100DB (Integrated Security)", 
            Description = "with catalog")]
        [TestCase(
            "MyName",
            DatabaseTestData.Connstr1,
            @"MyName .\sqlexpress.Opera56100DB (Integrated Security)",
            Description = "with catalog + display")]
        public void ToString_Parses_Valid_ConnectionString(string displayName, string connStr, string toStringed)
        {
            var conn = new Connection { ConnectionString = connStr, DisplayName = displayName, Kind = Models.Editor.ProgramType.MSSQLServer };

            var str = conn.ToString();

            Assert.AreEqual(toStringed, str);
        }

        [TestCase(DatabaseTestData.Invalid1)]
        [ExpectedException(typeof(ArgumentException))]
        public void Setting_ConnectionString_Throws_If_Value_Has_Invalid_Format(string connStr)
        {
            new Connection { ConnectionString = connStr };
        }

        [TestCase(DatabaseTestData.Connstr1, true)]
        [TestCase(DatabaseTestData.Connstr2, false)]
        public void Connection_Detects_Integrated_Security(string connStr, bool usingIntegratedSecurity)
        {
            var conn = new Connection { ConnectionString = connStr, Kind = Models.Editor.ProgramType.MSSQLServer };

            Assert.AreEqual(conn.UsingIntegratedSecurity, usingIntegratedSecurity);
        }


        [TestCase(DatabaseTestData.Connstr1, "Opera56100DB")]
        [TestCase(DatabaseTestData.Connstr2, "mydbname")]
        public void Parses_Initial_Catalog(string connStr, string initCat)
        {
            var conn = new Connection { ConnectionString = connStr, Kind = Models.Editor.ProgramType.MSSQLServer };

            Assert.AreEqual(conn.InitialCatalog, initCat);
        }

        [TestCase(DatabaseTestData.Connstr1, @".\sqlexpress")]
        [TestCase(DatabaseTestData.Connstr2, "sql.somewhere,1437")]
        public void Parses_Server(string connStr, string server)
        {
            var conn = new Connection { ConnectionString = connStr, Kind = Models.Editor.ProgramType.MSSQLServer };

            Assert.AreEqual(conn.DatabaseServer, server);
        }

        [TestCase(DatabaseTestData.Connstr2, "yyy")]
        public void Parses_DatabaseSecurity(string connStr, string sec)
        {
            var conn = new Connection { ConnectionString = connStr, Kind = Models.Editor.ProgramType.MSSQLServer };

            Assert.AreEqual(conn.DatabaseSecurity, sec);
        }
    }
}
