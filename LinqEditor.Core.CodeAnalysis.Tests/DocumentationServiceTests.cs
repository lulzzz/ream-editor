﻿using LinqEditor.Core.CodeAnalysis.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinqEditor.Core.CodeAnalysis.Tests
{
    [TestFixture]
    public class DocumentationServiceTests
    {
        IDocumentationService _service;

        [TestFixtureSetUp]
        public void Initialize()
        {
            var mockStore = new Mock<ISymbolStore>();
            _service = new DocumentationService(mockStore.Object);
        }

        [TestCase("T:System.Int32", @"
<member name=""T:System.Int32"">
  <summary>Represents a 32-bit signed integer.</summary>
  <filterpriority>1</filterpriority>
</member>
")]
        [TestCase("T:System.Collections.Generic.HashSet`1", @"
    <member name=""T:System.Collections.Generic.HashSet`1"">
      <summary>Represents a set of values.</summary>
      <typeparam name=""T"">The type of elements in the hash set.</typeparam>
    </member>
")]
        public void Can_Return_Documentation_For_Members(string memberName, string expectedDocumentation)
        {
            return;
            var doc = _service.GetDocumentation(memberName);

            Assert.AreEqual(doc.ToString(), XElement.Parse(expectedDocumentation).ToString());
        }

        [Test]
        public void Can_Return_Documentation_For_Member()
        {
            var ctorId = "M:System.Collections.Generic.List`1.#ctor";
            //var x = _service.GetDocs(ctorId);
        }
    }
}
