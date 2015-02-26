﻿using LinqEditor.Core.Models.Analysis;

namespace LinqEditor.Core.CodeAnalysis.Services
{
    /// <summary>
    /// Provides code analysis services for fixed compilation units/templates
    /// </summary>
    public interface ITemplateCodeAnalysis
    {
        int IndexOffset { get; }
        int LineOffset { get; }
        bool IsReady { get; }
        void Initialize(string assemblyPath = null);
        /// <summary>
        /// Analyses the specified source fragment.
        /// </summary>
        /// <param name="sourceFragment">The source fragment.</param>
        /// <param name="updateIndex">Index of the update.</param>
        /// <returns></returns>
        AnalysisResult Analyze(string sourceFragment, int updateIndex);
        AnalysisResult Analyze(string sourceFragment);
    }
}
