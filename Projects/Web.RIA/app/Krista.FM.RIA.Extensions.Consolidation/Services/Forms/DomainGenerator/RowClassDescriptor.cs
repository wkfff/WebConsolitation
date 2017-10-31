using System;

using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator
{
    public class RowClassDescriptor : ClassDescriptor
    {
        public override Type BaseType
        {
            get { return typeof(D_Report_Row); }
        }
    }
}
