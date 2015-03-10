﻿using LinqEditor.Core.Models.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqEditor.Core.CodeAnalysis.Helpers
{
    public interface IToolTipHelper
    {
        /// <summary>
        /// Returns a tooltip, if available for the given index.
        /// </summary>
        ToolTipData GetToolTip(int index);
    }
}
