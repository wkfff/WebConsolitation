using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.iPhone.IT_0002_0005.reports.iPhone.IT_0002_0005
{
    public partial class IT_0004_0001_Map : UserControl
    {
        private string queryName = String.Empty;

        public string QueryName
        {
            get
            {
                return queryName;
            }
            set
            {
                queryName = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            DundasMap1.Width = 750;
            DundasMap1.Height = 300;

            #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = false;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap1.NavigationPanel.Visible = false;
            DundasMap1.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap1.Viewport.EnablePanning = true;
            DundasMap1.Viewport.OptimizeForPanning = false;
            DundasMap1.Viewport.BackColor = Color.Black;

            // добавляем легенду
            DundasMap1.Legends.Clear();

            // добавляем легенду раскраски
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Left;
            legend1.BackColor = Color.FromArgb(75, 255, 255, 255);
            legend1.BackSecondaryColor = Color.Black;
            legend1.BackGradientType = GradientType.TopBottom;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Black;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.FromArgb(192, 192, 192);
            legend1.Font = new Font("MS Sans Serif", 12, FontStyle.Bold);
            legend1.AutoFitText = false;
            legend1.TitleColor = Color.White;
            legend1.Title = "";
            legend1.AutoFitMinFontSize = 12;
            DundasMap1.Legends.Add(legend1);

            // добавляем поля для раскраски
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof(string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("UnemploymentLevel");
            DundasMap1.ShapeFields["UnemploymentLevel"].Type = typeof(double);
            DundasMap1.ShapeFields["UnemploymentLevel"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "UnemploymentLevel";
            rule.DataGrouping = DataGrouping.Optimal;
            bool direct = CustomParam.CustomParamFactory("direct_assess").Value == "BDESC" ? true : false;
            rule.ColorCount = 3;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = direct ? Color.Red : Color.Green;
            rule.MiddleColor = Color.Orange;
            rule.ToColor = direct ? Color.Green : Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{P1} - #TOVALUE{P1}";

            DundasMap1.ShapeRules.Add(rule);

            #endregion

            DundasMap1.Shapes.Clear();
            DundasMap1.LoadFromShapeFile(Server.MapPath("../../../maps/РФ/РФ.shp"), "NAME", true);
            // заполняем карту данными
            FillMapData1(DundasMap1, dtIncomes);
        }

        #region Обработчики карты

        public static Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string[] subjects = patternValue.Split(' ');
            ArrayList shapeList = map.Shapes.Find(subjects[0], true, false);

            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }

            return null;
        }
       
        #endregion

        public void FillMapData1(MapControl map, DataTable dtMap)
        {
            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    string subject = row[0].ToString().Replace("УФО", "УрФО");
                    if (RegionsNamingHelper.IsFO(RegionsNamingHelper.FullName(subject)))
                    {
                        Shape shape = FindMapShape(DundasMap1, RegionsNamingHelper.FullName(subject), true);
                        if (shape != null)
                        {
                            shape["Name"] = subject;
                            shape["UnemploymentLevel"] = Convert.ToDouble(row[1]);

                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.Offset.X = -15;

                            shape.Text = string.Format("{0}\n{1:P2}", row[0], Convert.ToDouble(row[1]));
                            shape.Font = new Font("Arial", 12, FontStyle.Bold);

                            shape.BorderWidth = 2;
                            shape.TextColor = Color.White;
                            shape.TextVisibility = TextVisibility.Shown;
                        }
                    }
                }
            }
        }
    }
}