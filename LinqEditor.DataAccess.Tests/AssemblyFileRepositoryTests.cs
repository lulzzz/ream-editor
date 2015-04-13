﻿using LinqEditor.Core.Models;
using LinqEditor.Core.Models.Database;
using LinqEditor.Core.Settings;
using LinqEditor.Test.Common;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqEditor.DataAccess.Tests
{
    [TestFixture]
    public class AssemblyFileRepositoryTests
    {
        Connection _sqlServerConn;

        [TestFixtureSetUp]
        public void Setup()
        {
            _sqlServerConn = new SqlServerConnection
            {
                Id = Guid.NewGuid(),
                ConnectionString = SqlServerTestData.Connstr1
            };
        }


        [Test]
        public async void Returns_Non_Null_File_Path()
        {
            var provider = new AssemblyFileRepository();
            string path = await provider.GetAssemblyFilePath(_sqlServerConn);
            Assert.IsNotNull(path);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async void Passing_Null_Connection_Throws()
        {
            var provider = new AssemblyFileRepository();
            string path = await provider.GetAssemblyFilePath(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async void Passing_Empty_Connection_Throws()
        {
            var provider = new AssemblyFileRepository();
            var conn = new SqlServerConnection();
            string path = await provider.GetAssemblyFilePath(conn);
        }
    }
}
