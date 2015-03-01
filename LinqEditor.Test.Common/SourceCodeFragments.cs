﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqEditor.Test.Common
{
    public static class SourceCodeFragments
    {
        public const string ErrorExample1 = "v ar x = 10;";
        public const string ErrorExample2 = @"
var x = new List<int>();
x.DoesNotExists();
";

        public const string VarDeclOfInt = @"var x = 0x2a;";
        public const string FullDeclOfInt = @"int x1 = 10;";
        public const string VarDeclOfIntHashSet = @"var set = new HashSet<int>();";
        public const string FullDeclOfDataColumn = @"DataColumn col = new DataColumn();";
        public const string VarDeclOfQueryable = @"var x = new List<int>().AsQueryable();";
        public const string FullDeclOfMultipleInts = @"int x1 = 10, x2 = 20, x3 = 42;";
        public const string VarDeclOfIntList = @"var x = new List<int>();";
    }
}
