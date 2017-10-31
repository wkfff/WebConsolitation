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

using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;

using Infragistics.UltraChart.Core.Primitives;

using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebChart;

using System.Globalization;

using Infragistics.Documents.Reports.Report.Text;

using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Infragistics.WebUI.UltraWebNavigator;

using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using Orientation = Infragistics.Documents.Excel.Orientation;

using Infragistics.UltraChart.Core.Layers;

using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.reports.FO.FO_0002._001
{
    public partial class _default : CustomReportPage
    {
        int lastYear = 2008;

        ArrayList al = new ArrayList();

        string ls = "";

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            return dt;
        }

        private CustomParam p1 { get { return (UserParams.CustomParam("p1")); } }
        private CustomParam p2 { get { return (UserParams.CustomParam("p2")); } }

        int vuschet = 2;
        string Lregion = "";

        #region это для параметра с териториями
        Dictionary<string, int> GenDForRegion(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            int i = 1;
            for (; i < cs.Axes[0].Positions.Count; i++)
            {
                d.Add(cs.Axes[0].Positions[i].Members[0].Caption, 0);
            }
            Lregion = cs.Axes[0].Positions[1].Members[0].Caption;
            return d;
        }
        #endregion

        #region Для этово, как там его, блин, аааа параметра ирархичного с датами!

        string DelLastsChar(string s, Char c)
        {
            for (int i = s.Length - 1; i > 0; i--)
            {
                if (s[i] == c)
                {
                    s = s.Remove(i, 1);
                }
                else
                {
                    break;
                }
            }
            return s;

        }

        string AID(Dictionary<string, int> d, string str, int level)
        {
            string lev = "";
            for (; ; )
            {
                try
                {
                    d.Add(str + " " + lev, level);
                    break;
                }
                catch
                {
                }
                lev += " ";
            }
            return str + " " + lev;

        }

        Dictionary<string, int> GenDistonary(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            ////.Text = "xz";
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            ////.Text = "xz1";
            ////.Text = cs.Axes[1].Positions[0].Members[0].UniqueName;

            string year = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            ////.Text = year+" ";
            string poly = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[9];
            ////.Text += poly+" ";//lm + cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[15 - vuschet];
            string qvart = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[11];
            ////.Text += qvart+" ";
            string mounth = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[13];
            ////.Text += mounth;

            AID(d, year, 0);

            AID(d, poly, 1);

            AID(d, qvart, 2);

            AID(d, mounth, 3);


            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                if (year != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7])
                {
                    year = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                    AID(d, year, 0);
                    mounth = "";
                    qvart = "";
                    poly = "";
                }

                if (poly != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[9])
                {
                    poly = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[9];
                    AID(d, poly, 1);
                }

                if (qvart != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[11])
                {
                    qvart = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[11];
                    AID(d, qvart, 2);
                }

                if (mounth != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13])
                {
                    mounth = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                    ls = AID(d, mounth, 3);
                }

            }


            string subS = "";

            ls2 = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName;
            //ls = //cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName.Split('[', ']')[13];

            return d;
        }
        string ls2 = "";

        #endregion

        string mounth;
        Dictionary<string, int> GenPeriod(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> res = new Dictionary<string, int>();
            string s = cs.Axes[0].Positions[0].Members[0].UniqueName;
            string year = s.Split('[', ']')[s.Split('[', ']').Length - 8];
            mounth = s.Split('[', ']')[s.Split('[', ']').Length - 2];
            res.Add(year, 0);
            for (int i = 0; i < cs.Axes[0].Positions.Count; i++)
            {

                s = cs.Axes[0].Positions[i].Members[0].UniqueName;
                if (s.Split('[', ']')[s.Split('[', ']').Length - 8] != year)
                {
                    year = s.Split('[', ']')[s.Split('[', ']').Length - 8];
                    try
                    {
                        AID(res, year, 0);
                    }
                    catch { }
                }
                mounth = s.Split('[', ']')[s.Split('[', ']').Length - 2];
                try
                {
                    mounth = AID(res, mounth, 1);
                }
                catch { }
            }


            return res;
        }

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
                param.Value += AL[i].ToString();
                return param;
            }


        }
        Dictionary<string, int> GenLevel()
        {
            Dictionary<string, int> res = new Dictionary<string, int>();

            ArrayList al = ForMarks.Getmarks("paramLevel_");
            for (int i = 0; i < al.Count; i++)
            {
                res.Add(al[i].ToString(), 0);
            }
            return res;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            //G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            //C1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 3 - 5);
            //C2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 3 - 5);
            //C3.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 3 - 5);
            //ConfChart(C1);
            //ConfChart(C3);
            //ConfChart(C2);
            ////###########################################################################
            //UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            //UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            //UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            //UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;
            //############################################################################
        }
		protected override void Page_Load(object sender, EventArgs e)
		{
            base.Page_Load(sender, e);
            //RegionSettingsHelper.Instance.SetWorkingRegion("Novoorsk");

            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(GenPeriod("LD"));
                //ComboYear.SetСheckedState(mounth, 1 == 1);
                ComboYear.ParentSelect = 1 == 2;
                //ComboYear.ShowSelectedValue = 1 == 2;
                ComboYear.Width = 200;

                ComboLevel.FillDictionaryValues(GenLevel());
                ComboLevel.ParentSelect = 1 == 2;
                //ComboLevel.ShowSelectedValue = 1 == 1;

                //ComboYear.SetСheckedState(RegionSettingsHelper.Instance.GetPropertyValue("Конс.бюджет МО"), 1 == 1);

                ComboLevel.Width = 300;
            }
		}
	}
}
