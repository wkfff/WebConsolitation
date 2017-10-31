using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0001
{
	static public class ForMarks
	{
        
        public static ArrayList Getmarks(string prefix)
        {
            ArrayList AL = new ArrayList();
            string CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + "1");
            int i = 2;
            while (!string.IsNullOrEmpty(CurMarks))
            {
                AL.Add(CurMarks.ToString());

                CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + i.ToString());

                i++;
            }

            return AL;
        }

        public static CustomParam SetMarks(CustomParam param, ArrayList AL, params bool[] clearParam)
        {
            if (clearParam.Length > 0 && clearParam[0]) { param.Value = ""; }
            int i;
            for (i = 0; i < AL.Count - 1; i++)
            {
                param.Value += AL[i].ToString() + ",";
            }
            param.Value +=  AL[i].ToString();

            return param;
        }


	}
}
