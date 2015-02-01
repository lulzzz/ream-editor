﻿using LinqEditor.Core;
using LinqEditor.Core.Backend.Isolated;
using LinqEditor.Core.CodeAnalysis.Compiler;
using LinqEditor.Core.Schema.Helpers;
using LinqEditor.Core.Schema.Models;
using LinqEditor.Core.Schema.Services;
using LinqEditor.Core.Templates;
using LinqEditor.Test.Common.SqlServer;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace LinqEditor.Core.Backend.Tests
{
    /// <summary>
    /// Tests integration of components
    /// </summary>
    [TestFixture(Category="Database")]
    public class RunnerDatabaseTests
    {
        
        Database _database;
        string _schemaAssemblyPath;
        string _queryAssemblyPath;
        byte[] _queryAssemblyBytes;
        DatabaseSchema _schemaModel;
        Guid _schemaId = Guid.NewGuid();
        Guid _queryId1 = Guid.NewGuid();
        Guid _queryId2 = Guid.NewGuid();

        [TestFixtureSetUp]
        public void Initialize()
        {
            _database = new Database("UnitTest");
            var schemaProvider = new SqlSchemaProvider();
            var templateService = new TemplateService();
            _schemaModel = schemaProvider.GetSchema(_database.ConnectionString);
            var schemaSource = templateService.GenerateSchema(_schemaId, _schemaModel);
            var schemaResult = CSharpCompiler.CompileToFile(schemaSource, _schemaId.ToIdentifierWithPrefix("s"), PathUtility.TempPath);
            _schemaAssemblyPath = schemaResult.AssemblyPath;
            var querySource1 = templateService.GenerateQuery(_queryId1, "Foo.Dump();", _schemaId.ToIdentifierWithPrefix("s"));
            var querySource2 = templateService.GenerateQuery(_queryId2, "Foo.Dump();", _schemaId.ToIdentifierWithPrefix("s"));

            var fileResult = CSharpCompiler.CompileToFile(querySource1, _queryId1.ToIdentifierWithPrefix("q"), PathUtility.TempPath, _schemaAssemblyPath);
            _queryAssemblyPath = fileResult.AssemblyPath;
            var bytesResult = CSharpCompiler.CompileToBytes(querySource2, _queryId2.ToIdentifierWithPrefix("q"), _schemaAssemblyPath);
            _queryAssemblyBytes = bytesResult.AssemblyBytes;
        }

        [SetUp]
        public void Setup()
        {
            _database.RecreateTestData();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            _database.Dispose();

            if (File.Exists(_schemaAssemblyPath))
            {
                File.Delete(_schemaAssemblyPath);
            }
            if (File.Exists(_queryAssemblyPath))
            {
                File.Delete(_queryAssemblyPath);
            }
        }

        [Test]
        public void Can_Load_Initial_Schema_Assembly()
        {
            var container = new Isolated<Runner>();
            var initResult = container.Value.Initialize(_schemaAssemblyPath, _database.ConnectionString);
            container.Dispose();
            Assert.IsNull(initResult.Error);
        }

        [Test]
        public void Can_Execute_Query_Assembly_And_Fetch_Database_Rows_With_Basic_Types_Only()
        {
            var container = new Isolated<Runner>();
            var initResult = container.Value.Initialize(_schemaAssemblyPath, _database.ConnectionString);
            var executeResult = container.Value.Execute(_queryAssemblyBytes);

            container.Dispose();
            var rows = executeResult.Tables.First();
            Assert.IsNull(executeResult.Exception);
            Assert.AreEqual(4, executeResult.Tables.First().Rows.Count);
            Assert.AreEqual("Foo 3", executeResult.Tables.First().Rows[3][1]);
        }
    }
}
