﻿using Krista.FM.Common.Constants;
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Core.Params
{
    public class OktmoSverdlovskValueProvider : IParameterValueProvider
    {
        public string GetValue()
        {
            return OKTMO.Sverdlovsk;
        }
    }
}
