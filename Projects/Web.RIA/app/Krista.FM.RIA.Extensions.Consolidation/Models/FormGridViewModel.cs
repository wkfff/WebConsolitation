using System.Collections.Generic;
using Ext.Net;

namespace Krista.FM.RIA.Extensions.Consolidation.Models
{
    public class FormGridViewModel
    {
        public List<ColumnBase> Columns { get; set; }

        public LayoutMarkupViewModel Layout { get; set; }
    }
}
