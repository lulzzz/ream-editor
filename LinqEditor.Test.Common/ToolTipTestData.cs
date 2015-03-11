﻿using LinqEditor.Core.Models.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqEditor.Test.Common
{
    public static class ToolTipTestData
    {


        // key ->
        // tuple(
        //     item1: src,
        //     item2: offset,
        //     item3: tooltip
        // )
        
        public static Dictionary<string, Tuple<string, int, ToolTipData>> Data =
            new Dictionary<string, Tuple<string, int, ToolTipData>>
        {
            {VarDeclOfInt_InitialDecl, Tuple.Create(SourceCodeFragments.VarDeclOfInt, 0, new ToolTipData {
                ItemName = "struct System.Int32",
                Description = "Represents a 32-bit signed integer.",
                Addendums = new List<string> { }
            })}, // mscorlib.dll
            {VarDeclOfIntHashSet_InitialDecl, Tuple.Create(SourceCodeFragments.VarDeclOfIntHashSet, 0, new ToolTipData {
                ItemName = "class System.Collections.Generic.HashSet<T>",
                Description = "Represents a set of values.",
                Addendums = new List<string> { "T is System.Int32" }
            })},
            {VarDeclOfIntHashSet_Ctor, Tuple.Create(SourceCodeFragments.VarDeclOfIntHashSet, 11, new ToolTipData {
                ItemName = "HashSet<int>.HashSet() (+ 3 overload(s))",
                Description = "Initializes a new instance of the System.Collections.Generic.HashSet<T> class that is empty and uses the default equality comparer for the set type.",
                Addendums = new List<string> { }
            })},
            {VarDeclOfIntHashSet_IntGenericTypeParam, Tuple.Create(SourceCodeFragments.VarDeclOfIntHashSet, 24, new ToolTipData {
                ItemName = "struct System.Int32",
                Description = "Represents a 32-bit signed integer.",
                Addendums = new List<string> { }
            })},
            {VarDeclOfListWithCopyFromCtor_Ctor, Tuple.Create(SourceCodeFragments.VarDeclOfListWithCopyFromCtor, 53, new ToolTipData {
                ItemName = "List<int>.List(IEnumerable<int> collection) (+ 2 overload(s))",
                Description = "Initializes a new instance of the System.Collections.Generic.List<T> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.",
                Addendums = new List<string> { "Exceptions:", "\tSystem.ArgumentNullException" }
            })},
            {FullDeclOfDataColumn_InitialDecl, Tuple.Create(SourceCodeFragments.FullDeclOfDataColumn, 3, new ToolTipData {
                ItemName = "class System.Data.DataColumn",
                Description = "Represents the schema of a column in a System.Data.DataTable.",
                Addendums = new List<string> { }
            })},
            {FullDeclOfDataColumn_Ctor, Tuple.Create(SourceCodeFragments.FullDeclOfDataColumn, 20, new ToolTipData {
                ItemName = "DataColumn.DataColumn() (+ 4 overload(s))",
                Description = "Initializes a new instance of a System.Data.DataColumn class as type string.",
                Addendums = new List<string> { }
            })},
            {VarDeclOfDictionaryWithTuples_InitialDecl, Tuple.Create(SourceCodeFragments.VarDeclOfDictionaryWithTuples, 0, new ToolTipData {
                ItemName = "class System.Collections.Generic.Dictionary<TKey, TValue>",
                Description = "Represents a collection of keys and values.",
                // TODO: figure out why there's no space here!
                Addendums = new List<string> { "TKey is System.String", "TValue is System.Tuple<System.String,System.Collections.Generic.IEnumerable<System.String>>" }
            })},
            {VarDeclOfDictionaryWithTuples_TupleGenericTypeParam, Tuple.Create(SourceCodeFragments.VarDeclOfDictionaryWithTuples, 37, new ToolTipData {
                ItemName = "class System.Tuple<T1, T2>",
                Description = "Represents a 2-tuple, or pair.",
                Addendums = new List<string> { "T1 is System.String", "T2 is System.Collections.Generic.IEnumerable<System.String>" }
            })},
            {FullDeclOfDataColumn_MemberProperty, Tuple.Create(SourceCodeFragments.FullDeclOfDataColumn, 55, new ToolTipData {
                ItemName = "object DataColumn.DefaultValue",
                Description = "Gets or sets the default value for the column when you are creating new rows.",
                Addendums = new List<string> { "Exceptions:", "\tSystem.InvalidCastException" }
            })},
            {ComplexStatementsExample_TupleItem2Property, Tuple.Create(SourceCodeFragments.ComplexStatementsExample, 340, new ToolTipData {
                ItemName = "int Tuple<int, int>.Item2",
                Description = "Gets the value of the current System.Tuple<T1, T2> object's second component.",
                Addendums = new List<string> { }
            })},
        };

        static void stub()
        {
            var from = new List<Tuple<int, int>> 
            {
                Tuple.Create(1,2), Tuple.Create(3,4), Tuple.Create(5,6), Tuple.Create(7,8), Tuple.Create(9, 0)
            };
            var x = new List<Tuple<int, int>>(from).AsQueryable();
            Func<Tuple<int, int>, bool> filter = (t) => 
            {
                return true;
            };
            var filtered = x.Where(z => filter(z)).Select(z => z.Item1 + z.Item2).TakeWhile(z => z < 10);
        }

        public const string VarDeclOfInt_InitialDecl = "VarDeclOfInt_InitialDecl"; // initial decl
        public const string VarDeclOfIntHashSet_InitialDecl = "VarDeclOfIntHashSet_InitialDecl"; // initial decl
        public const string VarDeclOfIntHashSet_Ctor = "VarDeclOfIntHashSet_Ctor"; // ctor
        public const string VarDeclOfIntHashSet_IntGenericTypeParam = "VarDeclOfIntHashSet_IntGenericTypeParam"; // int type param
        public const string VarDeclOfListWithCopyFromCtor_Ctor = "VarDeclOfListWithCopyFromCtor_Ctor"; // ctor
        public const string FullDeclOfDataColumn_InitialDecl = "FullDeclOfDataColumn_InitialDecl"; // initial decl
        public const string FullDeclOfDataColumn_Ctor = "FullDeclOfDataColumn_Ctor"; // ctor
        public const string FullDeclOfDataColumn_MemberProperty = "FullDeclOfDataColumn_MemberProperty"; // property access
        public const string VarDeclOfDictionaryWithTuples_InitialDecl = "VarDeclOfDictionaryWithTuples_InitialDecl"; // initial decl
        public const string VarDeclOfDictionaryWithTuples_TupleGenericTypeParam = "VarDeclOfDictionaryWithTuples_TupleGenericTypeParam"; // tuple type param

        public const string ComplexStatementsExample_TupleItem2Property = "ComplexStatementsExample_TupleItem2Property";
    }
}
