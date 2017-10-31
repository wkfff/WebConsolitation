using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP
{
    public class ExcCostsAIPRolesProvider : IParameterValueProvider
    {
        private readonly IEO15ExcCostsAIPExtension extension;

        public ExcCostsAIPRolesProvider(IEO15ExcCostsAIPExtension extension)
        {
            this.extension = extension;
        }

        public string GetValue()
        {
            return extension.UserGroup;
        }
    }
}
