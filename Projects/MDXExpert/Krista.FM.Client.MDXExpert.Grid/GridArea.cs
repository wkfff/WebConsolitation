using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Области грида
    /// </summary>
    [Flags]
    public enum GridArea
    {
        FiltersCaption,
        ColumnsCaption,
        RowsCaption,
        MeasuresCaption,
        MeasuresData,
        Rows,
        Columns,
        Nothing
    }
}
