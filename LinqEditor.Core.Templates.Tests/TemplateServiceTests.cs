﻿using LinqEditor.Core;
using LinqEditor.Core.CodeAnalysis.Compiler;
using LinqEditor.Core.Schema.Helpers;
using LinqEditor.Core.Schema.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinqEditor.Core.Templates.Tests
{
    [TestFixture]
    public class TemplateServiceTests
    {
        DatabaseSchema _schemaModel;
        Guid _schemaId = Guid.NewGuid();
        string _schemaPath;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _schemaPath = PathUtility.TempPath + _schemaId.ToIdentifierWithPrefix("s") + ".dll";

            _schemaModel = new DatabaseSchema
            {
                Tables = new List<TableSchema>
                {
                    new TableSchema 
                    { 
                        Catalog = "catalog",
                        Schema= "schema",
                        Name = "table",
                        Columns = new List<ColumnSchema> 
                        {
                            new ColumnSchema { Index = 0, Name = "PK_ID", Type = "int" },
                            new ColumnSchema { Index = 1, Name = "Description", Type = "string" },
                            new ColumnSchema { Index = 2, Name = "Name", Type = "string" }
                        }
                    }
                }
            };
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            if (File.Exists(_schemaPath))
            {
                File.Delete(_schemaPath);
            }
        }

        [Test]
        public void Generates_Valid_Schema_Database_Source_Code()
        {
            var templateService = new TemplateService();
            var source = templateService.GenerateSchema(_schemaId, _schemaModel);
            var result = CSharpCompiler.CompileToBytes(source, _schemaId.ToIdentifierWithPrefix("s"));
            Assert.AreEqual(0, result.Errors.Count());
        }

        [Test]
        public void Generates_Valid_Query_Database_Source_Code()
        {
            var templateService = new TemplateService();

            var schemaSource = templateService.GenerateSchema(_schemaId, _schemaModel);
            var schemaResult = CSharpCompiler.CompileToFile(schemaSource, _schemaId.ToIdentifierWithPrefix("s"), PathUtility.TempPath);

            var queryId = Guid.NewGuid();
            var source = templateService.GenerateQuery(queryId, "table.Count();", _schemaId.ToIdentifierWithPrefix("s"));

            var result = CSharpCompiler.CompileToBytes(source, _schemaId.ToIdentifierWithPrefix("s"), _schemaPath);
            Assert.AreEqual(0, result.Errors.Count());
        }

        [Test]
        public void Generates_Valid_Code_Statements_Source_Code()
        {
            var templateService = new TemplateService();
            var codeId = Guid.NewGuid();
            var source = templateService.GenerateCodeStatements(codeId, "int x = 10;");
            var result = CSharpCompiler.CompileToBytes(source, codeId.ToIdentifierWithPrefix("c"));
            Assert.AreEqual(0, result.Errors.Count());
        }
    }
}
