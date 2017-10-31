using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;

using System.Text;
using System.Collections.Generic;

using Infragistics.UltraChart.Core.Primitives;

using Infragistics.Documents.Excel;

using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Infragistics.WebUI.UltraWebNavigator;

using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using Orientation = Infragistics.Documents.Excel.Orientation;

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0001._0028._02
{
	public partial class _default : CustomReportPage
	{
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        // параметр запроса для последней актуальной даты
        private CustomParam region { get { return (UserParams.CustomParam("region")); } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        // выбранный столбец
        private CustomParam gridColumnSelected { get { return (UserParams.CustomParam("gridColumnSelected")); } }

        private CustomParam position { get { return (UserParams.CustomParam("position")); } }

        // Заголовок страницы
        private static String headerText = "Оценка экономического развития в муниципальных районах {0}.";

        // путь к карте
        private CustomParam FileMapName { get { return (UserParams.CustomParam("FileMapName")); } }

        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размеров
                Mapo.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
                Mapa.ColorSwatchPanel.Visible = 1 == 1;
                Mapa.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
                Mapo.ColorSwatchPanel.Visible = 1 == 1;
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }


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

        // --------------------------------------------------------------------
        Dictionary<string, int> GenDistonary(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            string ly =    cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            string lm =    cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[13];
            AID(d, ly, 0);
            AID(d, lm, 1);
            AID(d, cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[15], 2);
            string subS = "";

            //красота

            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                try
                {

                    if (ly != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7])
                    {
                        ly = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                        AID(d, ly, 0);
                        lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                        AID(d, lm, 1);
                        {
                            AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                        }

                    }
                    else
                        if (lm != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13])
                        {
                            lm = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                            AID(d, lm, 1);
                            {
                                AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                            }
                        }
                        else
                        {
                            {
                              ls = AID(d, cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[15], 2);
                            }

                        }
                }
                catch { }

            }
            return d;
        }

        string ls = "";

        string GetDateInNode(Node n)
        {
            string res = "";

            res ="[Период].[Период].[Год].[" + DelLastsChar(n.Parent.Parent.Text,' ') + "].[Полугодие " +CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(DelLastsChar(n.Parent.Text,' '))).ToString()
                 + "].[Квартал " + CRHelper.QuarterNumByMonthNum( CRHelper.MonthNum(DelLastsChar(n.Parent.Text,' '))).ToString()+ "].[" + DelLastsChar(n.Parent.Text,' ') + "].[" + DelLastsChar(n.Text,' ') + "]"; 

            return res;
        }

        private CustomParam p1 { get { return (UserParams.CustomParam("p1")); } }
        protected override void Page_Load(object sender, EventArgs e)
        {
            //RegionSettingsHelper.Instance.SetWorkingRegion("Kostroma");
            base.Page_Load(sender, e);
            //RegionSettingsHelper.Instance.SetWorkingRegion("PRTEST3");
            try
            {
                
                
                if (!Page.IsPostBack)
                {
                    try
                    {
                        Year.ShowSelectedValue = 2 * 2 == 5;
                        Year.ParentSelect = 3*3 == 9;
                        Year.FillDictionaryValues(GenDistonary("ld"));
                        Year.SetСheckedState(ls, 1 == 1);
                        Year.Title = "Период";
                        Year.PanelHeaderTitle = "Период:";
                        Year.Width = 100;
                    }
                    catch { }
                }
                Year.SetСheckedState(Year.SelectedNode.Level == 2 ? Year.SelectedValue : Year.SelectedNode.Nodes[Year.SelectedNode.Nodes.Count - 1].Text, 1 == 1);
                Year.SetСheckedState(Year.SelectedNode.Level == 2 ? Year.SelectedValue : Year.SelectedNode.Nodes[Year.SelectedNode.Nodes.Count - 1].Text, 1 == 1);
                p1.Value = GetDateInNode(Year.SelectedNode);
                SetMapa(Mapo,"mapa");
                SetMapa(Mapa, "mapo");
            }
            catch (Exception ex)
            {
                // неудачная загрузка ...
                throw new Exception(ex.Message, ex);
            }
            Page.Title = Hederglobal.Text;
            DateTime currDateTime = GetDateString(Year.GetSelectedNodePath(), Year.SelectedNode.Level);
            //Мониторинг ситуации на рынке труда по области в целом по состоянию на {0:dd.MM.yyyy}
            Label2.Text = string.Format("Уровень безработицы по муниципальным образованиям на {0:dd.MM.yyyy}, процент", currDateTime);
            Label3.Text = string.Format("Коэффициент напряженности на рынке труда на {0:dd.MM.yyyy}, человек на одну вакансию", currDateTime); 

            //RegionSettingsHelper.Instance.SetWorkingRegion("Gubkinski");
        }


        #region Обработчики карты

        #region xz
        public DateTime GetDateString(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 1)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            return GetDateString(Year.GetNodeLastChild(Year.SelectedNode), level + 1);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));
                        }
                }
            }
            return DateTime.MinValue;
        }
        #endregion

        public void FillMapData(string q)
        {

           // catch { }

        }

        #endregion
        // --------------------------------------------------------------------


        protected void SetMapa(MapControl DundasMap,string dataForMapa)
        {
            DundasMap.BorderLineColor = Color.Transparent;
            DundasMap.BorderLineWidth = 0;
            DundasMap.BorderLineStyle = MapDashStyle.None;
            DundasMap.ColorSwatchPanel.Visible = 1 == 1;
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            DundasMap.BackColor = System.Drawing.Color.Transparent;
            DundasMap.SelectionBorderColor = System.Drawing.Color.Transparent;
            DundasMap.SelectionMarkerColor = System.Drawing.Color.Transparent;

            // добавляем легенду
            Legend legend = new Legend("CompleteLegend");
            legend.Visible = false;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = "Процент исполнения";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем поля
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Complete");
            DundasMap.ShapeFields["Complete"].Type = typeof(double);
            DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;
            DundasMap.ShapeFields.Add("CompletePercent");
            DundasMap.ShapeFields["CompletePercent"].Type = typeof(double);
            DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Complete";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 10;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = true;
            rule.ShowInLegend = "CompleteLegend";
            DundasMap.ShapeRules.Add(rule);
            DundasMap.ColorSwatchPanel.Visible = 1 == 1;
            // заполняем карту данными
            DundasMap.Shapes.Clear();
            DundasMap.LoadFromShapeFile(Server.MapPath("~/maps/Кострома/Территор.shp"), "NAME", true);

            DundasMap.LoadFromShapeFile(Server.MapPath("~/maps/Кострома/Соседние.shp"), "NAME", true);
            DundasMap.LoadFromShapeFile(Server.MapPath("~/maps/Кострома/Города.shp"), "NAME", true);

            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(dataForMapa), "xz", dtGrid);

            if (dtGrid == null || DundasMap == null)
            {
                return;
            }
            {
                for (int i = 0; i < DundasMap.Shapes.Count; i++)
                
                {
                    
                    try
                    {

                    }
                    catch { }
                    for (int j = 0; j < dtGrid.Rows.Count; j++)    
                    {
                        DundasMap.Shapes[i].TextVisibility = TextVisibility.Shown;
                        DundasMap.Shapes[i].Visible = 1 == 1;
                        if (DundasMap.Shapes[i].Name == dtGrid.Rows[j].ItemArray[0].ToString())
                        {
                            try
                            {
                                //b = 2 == 1;
                                DundasMap.Shapes[i]["Complete"] = Convert.ToDouble((System.Decimal)(dtGrid.Rows[j].ItemArray[1]));

                                DundasMap.Shapes[i].Text =
                                    (string.Format("{0}\n{1}%", dtGrid.Rows[j].ItemArray[2].ToString(), Decimal.Round((System.Decimal)(dtGrid.Rows[j].ItemArray[1]), 2)));
                                DundasMap.Shapes[i].ToolTip =
    dataForMapa != "mapo" ? string.Format("{1}\nУровень безработицы: {0} % ", Decimal.Round(Convert.ToDecimal(dtGrid.Rows[j].ItemArray[1]), 2), dtGrid.Rows[j].ItemArray[2].ToString())
    : string.Format("{1}\nКоэффициент напряженности: {0} % ", Decimal.Round(Convert.ToDecimal(dtGrid.Rows[j].ItemArray[1]), 2), dtGrid.Rows[j].ItemArray[2].ToString());
                                if (dtGrid.Rows[j].ItemArray[1].ToString() == "")
                                {
                                    DundasMap.Shapes[i].Text = "";
                                }

                            }
                            catch { DundasMap.Shapes[i].Text = ""; }
                        }

                    }
                    

                }
            }
           
        }

	}
}
