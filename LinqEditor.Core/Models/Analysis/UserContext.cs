﻿
namespace LinqEditor.Core.Models.Analysis
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
        /// Tooltip context
        /// </summary>
        ToolTip,
        /// <summary>
        /// Could not infer type
        /// </summary>
        NotReady,
        /// <summary>
        /// Could not infer type
        /// </summary>
        Unknown
    }
}
