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
        string _query1AssemblyPath;
        byte[] _query2AssemblyBytes;
        byte[] _query3AssemblyBytes;
        DatabaseSchema _schemaModel;
        Guid _schemaId = Guid.NewGuid();
        Guid _queryId1 = Guid.NewGuid();
        Guid _queryId2 = Guid.NewGuid();
        Guid _queryId3 = Guid.NewGuid();

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
            var querySource3 = templateService.GenerateQuery(_queryId3, "TypeTestTable.Take(1).Dump();", _schemaId.ToIdentifierWithPrefix("s"));

            var file1Result = CSharpCompiler.CompileToFile(querySource1, _queryId1.ToIdentifierWithPrefix("q"), PathUtility.TempPath, _schemaAssemblyPath);
            _query1AssemblyPath = file1Result.AssemblyPath;
            var bytes2Result = CSharpCompiler.CompileToBytes(querySource2, _queryId2.ToIdentifierWithPrefix("q"), _schemaAssemblyPath);
            _query2AssemblyBytes = bytes2Result.AssemblyBytes;
            var bytes3Result = CSharpCompiler.CompileToBytes(querySource3, _queryId3.ToIdentifierWithPrefix("q"), _schemaAssemblyPath);
            _query3AssemblyBytes = bytes3Result.AssemblyBytes;
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
            if (File.Exists(_query1AssemblyPath))
            {
                File.Delete(_query1AssemblyPath);
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
        public void Can_Execute_Query_Assembly_And_Fetch_Database_Rows_With_Basic_Types_Only_Using_Byte_Assembly()
        {
            var container = new Isolated<Runner>();
            var initResult = container.Value.Initialize(_schemaAssemblyPath, _database.ConnectionString);
            var executeResult = container.Value.Execute(_query2AssemblyBytes);

            container.Dispose();
            var rows = executeResult.Tables.First();
            Assert.IsNull(executeResult.Exception);
            Assert.AreEqual(4, executeResult.Tables.First().Rows.Count);
            Assert.AreEqual("Foo 3", executeResult.Tables.First().Rows[3][1]);
        }

        [Test]
        public void Can_Execute_Query_Assembly_And_Fetch_Database_Rows_With_Basic_Types_Only_Using_File_Assembly()
        {
            var container = new Isolated<Runner>();
            var initResult = container.Value.Initialize(_schemaAssemblyPath, _database.ConnectionString);
            var executeResult = container.Value.Execute(_query1AssemblyPath);

            container.Dispose();
            var rows = executeResult.Tables.First();
            Assert.IsNull(executeResult.Exception);
            Assert.AreEqual(4, executeResult.Tables.First().Rows.Count);
            Assert.AreEqual("Foo 3", executeResult.Tables.First().Rows[3][1]);
        }

        [Test]
        public void Can_Execute_Query_Assembly_And_Fetch_Database_Rows_With_All_DataTypes()
        {
            var container = new Isolated<Runner>();
            var initResult = container.Value.Initialize(_schemaAssemblyPath, _database.ConnectionString);
            var executeResult = container.Value.Execute(_query3AssemblyBytes);

            container.Dispose();
            var rows = executeResult.Tables.First();
            Assert.IsNull(executeResult.Exception);
            Assert.AreEqual(1, executeResult.Tables.First().Rows.Count);
            Assert.AreEqual(2147483647, (int)executeResult.Tables.First().Rows[0]["intcol"]);
        }
    }
}
