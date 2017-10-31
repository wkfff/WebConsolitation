using System;
using System.Collections.Generic;
using Krista.FM.RIA.Core.ViewModel;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests.Stub
{
    public class ViewServiceStub : IViewService
    {
        public string GetDataFilter()
        {
            throw new NotImplementedException();
        }

        public string GetClientScript()
        {
            throw new NotImplementedException();
        }

        public List<ActionDescriptor> Actions
        {
            get { return new List<ActionDescriptor>(); }
        }
    }
}
