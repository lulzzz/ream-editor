﻿using LinqEditor.Core.Models.Database;
using LinqEditor.Core.Settings;
using System;
using System.Threading.Tasks;

namespace LinqEditor.Schema.Providers
{
    public class SQLiteSchemaProvider : ISQLiteSchemaProvider
    {
        public Task<DatabaseSchema> GetDatabaseSchema(Connection connection)
        {
            throw new NotImplementedException();
        }
    }
}
