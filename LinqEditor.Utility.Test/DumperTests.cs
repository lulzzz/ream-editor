﻿using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using LinqEditor.Utility.Helpers;

namespace LinqEditor.Utility.Tests
{
    [TestFixture]
    public class DumperTests
    {
        public class Foo 
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public Guid Key { get; set; }
        }

        protected IQueryable<Foo> Repository { get; set; }

        [TestFixtureSetUp]
        public void Initialize()
        {
            var data = new List<Foo>()
            {
                new Foo { Id = 0, Value = "foo", Key = Guid.NewGuid() },
                new Foo { Id = 1, Value = "bar", Key = Guid.NewGuid() },
                new Foo { Id = 2, Value = "baz", Key = Guid.NewGuid() },
                new Foo { Id = 3, Value = "qux", Key = Guid.NewGuid() },
                new Foo { Id = 4, Value = "quu", Key = Guid.NewGuid() },
                new Foo { Id = 5, Value = "qxx", Key = Guid.NewGuid() }
            };

            // no sql maps
            Dumper.SqlColumns = new Dictionary<string, IDictionary<string, int>>();
            Repository = data.AsQueryable<Foo>();
        }

        [Test]
        public void Can_Dump_Anonymous_Types()
        {
            Repository.Select(x => new { foo = x.Id }).Dump();
            
            var dump = Dumper.FlushDumps();
            Assert.AreEqual("not the same", dump.First().Rows.Count);
            Assert.AreEqual(1, dump.First().Columns.Count);
        }

        [Test]
        public void Dumps_Anymous_Types_With_Friendly_DisplayName()
        {
            Repository.Select(x => new { foo = x.Id, bar = x.Value }).Dump();

            var dump = Dumper.FlushDumps();
            Assert.AreEqual("Anonymous 1", dump.First().TableName);
        }
    }
}
