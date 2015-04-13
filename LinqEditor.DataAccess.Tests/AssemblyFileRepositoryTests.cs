﻿using LinqEditor.Core.Models;
using LinqEditor.Core.Models.Database;
using LinqEditor.Core.Settings;
using LinqEditor.Schema;
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
        ISchemaProvider _schemaProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            _sqlServerConn = new SqlServerConnection
            {
                Id = Guid.NewGuid(),
                ConnectionString = SqlServerTestData.Connstr1
            };

            var schemaProviderMock = new Mock<ISchemaProvider>();

            _schemaProvider = schemaProviderMock.Object;
        }

        [Test]
        public void Constructor_Requires_Non_Null_Arguments()
        {
            Assert.Throws<ArgumentNullException>(() => new AssemblyFileRepository(null));
        }

        [Test]
        public async void Returns_Non_Null_File_Path()
        {
            var provider = new AssemblyFileRepository(_schemaProvider);
            string path = await provider.GetAssemblyFilePath(_sqlServerConn);
            Assert.IsNotNull(path);
        }

        [Test]
        public void Passing_Null_Connection_Throws()
        {
            var provider = new AssemblyFileRepository(_schemaProvider);
            Assert.Throws<ArgumentNullException>(async () => await provider.GetAssemblyFilePath(null));
        }

        [Test]
        public void Passing_Empty_Connection_Throws()
        {
            var provider = new AssemblyFileRepository(_schemaProvider);
            var conn = new SqlServerConnection();
            Assert.Throws<ArgumentException>(async () => await provider.GetAssemblyFilePath(conn));
        }
    }
}
