﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqEditor.Core.Models.Analysis
{
    public class AnalysisResult
    {
        public IEnumerable<CompletionEntry> MemberCompletions { get; set; }
        public UserContext Context { get; set; }
        public IEnumerable<Warning> Warnings { get; set; }
        public IEnumerable<Error> Errors { get; set; }
    }
}
