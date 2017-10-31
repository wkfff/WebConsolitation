using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Krista.FM.Expert.PivotData;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;

namespace Krista.FM.Client.MDXExpert
{
    class MapPivotFieldBrowseAdapter : PivotFieldBrowseAdapter
    {
        public MapPivotFieldBrowseAdapter(PivotField pivotField, CustomReportElement reportElement)
            : base(pivotField, reportElement)
        {
        }

    }
}
