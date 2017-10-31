using System;
using System.Collections.Generic;

namespace Krista.FM.RIA.Core.ViewModel
{
    public class DefaultViewService : IViewService
    {
        private readonly string filter;

        public DefaultViewService()
        {
            this.filter = String.Empty;
        }

        public DefaultViewService(string filter)
        {
            this.filter = filter;
        }

        public List<ActionDescriptor> Actions
        {
            get { return new List<ActionDescriptor>(); }
        }

        public string GetDataFilter()
        {
            return this.filter;
        }

        public string GetClientScript()
        {
            return String.Empty;
        }
    }
}
