using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Common.Services
{
    public interface IStringTagProvider
    {
        string[] Tags
        {
            get;
        }

        string Convert(string tag);
    }
}
