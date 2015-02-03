﻿
namespace LinqEditor.Core.CodeAnalysis.Models
{
    /// <summary>
    /// Denotes the context for a user update
    /// </summary>
    public enum UserContext
    {
        /// <summary>
        /// Accessing an objects members.
        /// </summary>
        MemberCompletion,
        /// <summary>
        /// Could not infer type
        /// </summary>
        Unknown
    }
}
