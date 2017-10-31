using System;
using System.Collections;

using System.Data;
using System.Drawing;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class IT_0001_0005_v : CustomReportPage
    {
        private DateTime reportDate;

        private bool IsSmallResolution
        {
            get { return true; }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DundasMap1.Width = 750;
            DundasMap1.Height = 430;

            DundasMap2.Width = 750;
            DundasMap2.Height = 430;

            #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap1.NavigationPanel.Visible = !IsSmallResolution;
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
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 3;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Orange;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{P1} - #TOVALUE{P1}";

            DundasMap1.ShapeRules.Add(rule);

            // добавляем правила расстановки символов
            DundasMap1.SymbolRules.Clear();
            SymbolRule symbolRule = new SymbolRule();
            symbolRule.Name = "SymbolRule";
            symbolRule.Category = string.Empty;
            symbolRule.DataGrouping = DataGrouping.EqualInterval;
            symbolRule.SymbolField = "UnemploymentPopulation";
            symbolRule.ShowInLegend = "SymbolLegend";
            DundasMap1.SymbolRules.Add(symbolRule);

            // звезды для легенды
            for (int i = 1; i < 4; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbol" + i;
                predefined.MarkerStyle = MarkerStyle.Star;
                predefined.Width = 5 + (i * 5);
                predefined.Height = predefined.Width;
                predefined.Color = Color.DarkViolet;
                DundasMap1.SymbolRules["SymbolRule"].PredefinedSymbols.Add(predefined);
            }


            #endregion

            #region Настройка карты 2

            DundasMap2.Meridians.Visible = false;
            DundasMap2.Parallels.Visible = false;
            DundasMap2.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap2.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap2.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap2.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap2.Viewport.EnablePanning = true;
            DundasMap2.Viewport.OptimizeForPanning = false;
            DundasMap2.Viewport.BackColor = Color.Black;

            // добавляем легенду
            DundasMap2.Legends.Clear();

            // добавляем легенду раскраски
            legend1 = new Legend("TensionLegend");
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
            legend1.Title = "";
            legend1.TitleColor = Color.White;
            legend1.AutoFitMinFontSize = 12;
            DundasMap2.Legends.Add(legend1);


            // добавляем поля для раскраски
            DundasMap2.ShapeFields.Clear();
            DundasMap2.ShapeFields.Add("Name");
            DundasMap2.ShapeFields["Name"].Type = typeof(string);
            DundasMap2.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap2.ShapeFields.Add("TensionKoeff");
            DundasMap2.ShapeFields["TensionKoeff"].Type = typeof(double);
            DundasMap2.ShapeFields["TensionKoeff"].UniqueIdentifier = false;


            // добавляем правила раскраски
            DundasMap2.ShapeRules.Clear();
            rule = new ShapeRule();
            rule.Name = "TensionKoeffRule";
            rule.Category = String.Empty;
            rule.ShapeField = "TensionKoeff";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 3;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Orange;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "TensionLegend";
            rule.LegendText = "#FROMVALUE{P1} - #TOVALUE{P1}";
            DundasMap2.ShapeRules.Add(rule);

            #endregion

            // заполняем карту формами
            string regionStr = "УрФО";
            DundasMap1.Shapes.Clear();
            DundasMap1.LoadFromShapeFile(Server.MapPath(string.Format("../../../maps/РФ/РФ.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData1(DundasMap1);

            DundasMap2.Shapes.Clear();
            DundasMap2.LoadFromShapeFile(Server.MapPath(string.Format("../../../maps/РФ/РФ.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData2(DundasMap2);
        }

        #region Обработчики карты

        public static Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string subject = string.Empty;
            bool isRepublic = patternValue.Contains("Республика");
            bool isTown = patternValue.Contains("г.");
            string[] subjects = patternValue.Split(' ');
            if (subjects.Length > 1)
            {
                // пока такой глупый способ сопоставления имен субъектов
                switch (subjects[0])
                {
                    case "Чеченская":
                        {
                            subject = "Чечня";
                            break;
                        }
                    case "Карачаево-Черкесская":
                        {
                            subject = "Карачаево-Черкессия";
                            break;
                        }
                    case "Кабардино-Балкарская":
                        {
                            subject = "Кабардино-Балкария";
                            break;
                        }
                    case "Удмуртская":
                        {
                            subject = "Удмуртия";
                            break;
                        }
                    case "Чувашская":
                        {
                            subject = "Чувашия";
                            break;
                        }
                    default:
                        {
                            subject = (isRepublic || isTown) ? subjects[1] : subjects[0];
                            break;
                        }
                }
            }

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }
        public void FillMapData1(MapControl map)
        {
            DataTable dtMap = new DataTable();

            dtMap.Columns.Add(new DataColumn("Регион", typeof(string)));
            dtMap.Columns.Add(new DataColumn("Исполнено", typeof(double)));

            DataRow newRow = dtMap.NewRow();
            newRow[0] = "Дальневосточный федеральный округ";
            newRow[1] = 2.7;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Приволжский федеральный округ";
            newRow[1] = 16.6;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Северо-Западный федеральный округ";
            newRow[1] = 10.5;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Южный федеральный округ";
            newRow[1] = 4.2;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Сибирский федеральный округ";
            newRow[1] = 8.6;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Уральский федеральный округ";
            newRow[1] = 17.5;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Центральный федеральный округ";
            newRow[1] = 39.9;
            dtMap.Rows.Add(newRow);


            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();
                    if (RegionsNamingHelper.IsFO(subject))
                    {
                        Shape shape = FindMapShape(DundasMap1, subject, true);
                        if (shape != null)
                        {
                            shape["Name"] = subject;
                            shape["UnemploymentLevel"] = Convert.ToDouble(row[1]) / 100;
                            //shape.ToolTip = "#NAME #COMPLETEPERCENT{N2}%";

                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.Offset.X = -15;
                            if (!IsSmallResolution)
                            {
                                shape.Offset.Y = -30;
                            }

                            shape.Text = string.Format("{0}\n{1:P1}", RegionsNamingHelper.ShortName(row[0].ToString()), Convert.ToDouble(row[1]) / 100);
                            shape.Font = new Font("Arial", 12, FontStyle.Bold);

                            shape.BorderWidth = 2;
                            shape.TextColor = Color.White;
                            //shape.Font = new Font("Verdana", IsSmallResolution ? 7 : 8, FontStyle.Bold);
                            shape.TextVisibility = TextVisibility.Shown;
                        }
                    }
                }
            }
        }

        public void FillMapData2(MapControl map)
        {
            DataTable dtMap = new DataTable();

            dtMap.Columns.Add(new DataColumn("Регион", typeof(string)));
            dtMap.Columns.Add(new DataColumn("Исполнено", typeof(double)));

            DataRow newRow = dtMap.NewRow();
            newRow[0] = "Дальневосточный федеральный округ";
            newRow[1] = 2.2;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Приволжский федеральный округ";
            newRow[1] = 27.1;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Северо-Западный федеральный округ";
            newRow[1] = 9.4;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Южный федеральный округ";
            newRow[1] = 2.8;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Сибирский федеральный округ";
            newRow[1] = 9.3;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Уральский федеральный округ";
            newRow[1] = 13.6;
            dtMap.Rows.Add(newRow);

            newRow = dtMap.NewRow();
            newRow[0] = "Центральный федеральный округ";
            newRow[1] = 35.6;
            dtMap.Rows.Add(newRow);


            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();
                    if (RegionsNamingHelper.IsFO(subject))
                    {
                        Shape shape = FindMapShape(DundasMap2, subject, true);
                        if (shape != null)
                        {
                            shape["Name"] = subject;
                            shape["TensionKoeff"] = Convert.ToDouble(row[1]) / 100;
                            //shape.ToolTip = "#NAME #COMPLETEPERCENT{N2}%";

                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.Offset.X = -15;
                            if (!IsSmallResolution)
                            {
                                shape.Offset.Y = -30;
                            }

                            shape.Text = string.Format("{0}\n{1:P1}", RegionsNamingHelper.ShortName(row[0].ToString()), Convert.ToDouble(row[1]) / 100);
                            shape.Font = new Font("Arial", 12, FontStyle.Bold);

                            shape.BorderWidth = 2;
                            shape.TextColor = Color.White;
                            //shape.Font = new Font("Verdana", IsSmallResolution ? 7 : 8, FontStyle.Bold);
                            shape.TextVisibility = TextVisibility.Shown;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
