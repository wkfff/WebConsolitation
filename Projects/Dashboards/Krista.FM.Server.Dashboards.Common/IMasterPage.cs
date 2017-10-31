using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Server.Dashboards.Common
{
    public interface IMasterPage
    {
        void SetTwitterButtonText(string text);
        void SetHeaderVisible(bool visible);
    }
}
