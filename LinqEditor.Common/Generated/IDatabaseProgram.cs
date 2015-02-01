﻿using IQToolkit;
using System.Collections.Generic;
using System.Data;

namespace LinqEditor.Common.Generated
{
    /// <summary>
    /// Dynamic query class implements this interface to make working with it easier.
    /// </summary>
    public interface IDatabaseProgram
    {
        IEnumerable<DataTable> Execute(IEntityProvider entityProvider);
    }
}
