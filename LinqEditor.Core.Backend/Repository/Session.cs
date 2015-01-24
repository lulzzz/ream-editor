﻿using LinqEditor.Core.Templates;
using LinqEditor.Core.Backend.Isolated;
using LinqEditor.Core.Schema.Models;
using LinqEditor.Core.Schema.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqEditor.Core.CodeAnalysis.Compiler;
using LinqEditor.Core.Backend.Models;
using LinqEditor.Core.CodeAnalysis.Models;

namespace LinqEditor.Core.Backend.Repository
{
    public class Session : ISession, IDisposable
    {
        private string _connectionString;
        private DatabaseSchema _sqlSchema;
        private string _schemaPath;
        private byte[] _schemaImage;
        private string _schemaNamespace;
        private Guid _sessionId = Guid.NewGuid();
        private Guid _queryId = Guid.NewGuid();

        //private AppDomain _executionDomain;

        private ICSharpCompiler _compiler;
        private ISqlSchemaProvider _schemaProvider;
        private ITemplateService _generator;

        //private DbEntityProvider _entityProvider;

        private Isolated<Runner> _container;

        public Session(ICSharpCompiler compiler, ISqlSchemaProvider schemaProvider, ITemplateService generator)
        {
            _compiler = compiler;
            _schemaProvider = schemaProvider;
            _generator = generator;
        }

        public async Task<InitializeResult> Initialize(string connectionString)
        {
            // ok to use Task.Run when "just" offloading from UI thread
            // http://blogs.msdn.com/b/pfxteam/archive/2012/04/12/10293335.aspx?Redirected=true
            return await Task.Run(() =>
            {
                _connectionString = connectionString;
                _sqlSchema = _schemaProvider.GetSchema(_connectionString);
                _schemaNamespace = "";
                var schemaSource = _generator.GenerateSchema(_sessionId, out _schemaNamespace, _sqlSchema);
                var result = _compiler.Compile(schemaSource, _schemaNamespace, generateFiles: true);
                _schemaPath = result.AssemblyPath;
                _schemaImage = result.AssemblyBytes;
                // loads schema in new appdomain
                _container = new Isolated<Runner>();
                return _container.Value.Initialize(_schemaPath, _connectionString);
            });
        }

        public async Task<ExecuteResult> Execute(string sourceFragment)
        {
            return await Task.Run(() =>
            {
                string queryNamespace;
                var querySource = _generator.GenerateQuery(_queryId, out queryNamespace, sourceFragment, _schemaNamespace);
                var result = _compiler.Compile(querySource, queryNamespace, generateFiles: false, references: _schemaPath);

                if (result.Success)
                {
                    var containerResult = _container.Value.Execute(result.AssemblyBytes);

                    return new ExecuteResult
                    {
                        Success = containerResult.Success,
                        QueryText = containerResult.QueryText,
                        Tables = containerResult.Tables,
                        Warnings = result.Warnings
                    };
                }

                return new ExecuteResult
                {
                    Success = false,
                    Errors = result.Errors,
                    Warnings = result.Warnings
                };
                    
            });
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
