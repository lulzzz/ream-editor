﻿using System;

namespace LinqEditor.Core.Backend.Models
{
    /// <summary>
    /// Wraps the result of initializing the linked appdomain
    /// </summary>
    [Serializable]
    public class InitializeResult
    {
        public Exception Error { get; set; }
        public string AssemblyPath { get; set; }
        public string SchemaNamespace { get; set; }
    }
}
