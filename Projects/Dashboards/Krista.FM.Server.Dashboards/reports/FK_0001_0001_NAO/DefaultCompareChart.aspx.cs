using System;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0001_NAO
{
    public partial class DefaultCompareChart : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "ßíâàðü";
        private DateTime currentDate;
        
        private bool onWall;
        private bool blackStyle;
        private double widthMultiplier;
        private double heightMultiplier;
        private int fontSizeMultiplier;
        private int primitiveSizeMultiplier;
        private int pageWidth;
        private int pageHeight;

        #region Ïàðàìåòðû çàïðîñà

        private CustomParam yearComboValue;
        private CustomParam monthComboValue;
        private CustomParam subjectComboValue;
        private CustomParam levelComboValue;
        private CustomParam selectedLevel;

        #endregion

        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);

            blackStyle = false;
            if (Session["blackStyle"] != null)
            {
                blackStyle = Convert.ToBoolean(((CustomParam)Session["blackStyle"]).Value);
            }

            string regionTheme = RegionSettings.Instance.Id;
            CRHelper.SetPageTheme(this, blackStyle ? regionTheme + "BlackStyle" : regionTheme);
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Èíèöèàëèçàöèÿ ïàðàìåòðîâ çàïðîñà

            yearComboValue = UserParams.CustomParam("year_combo_value", true);
            monthComboValue = UserParams.CustomParam("month_combo_value", true);
            subjectComboValue = UserParams.CustomParam("subject_combo_value", true);
            levelComboValue = UserParams.CustomParam("level_combo_value", true);
            selectedLevel = UserParams.CustomParam("selected_level");

            #endregion

            Session["blackStyle"] = null;

            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
                Session["onWall"] = null;
            }

            SetScaleSize();

            UltraChart.Width = pageWidth - 50;
            UltraChart.Height = pageHeight;
            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Íàñòðîéêà äèàãðàìì
            
            UltraChart.ChartType = ChartType.DoughnutChart;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:P2>";
            UltraChart.DoughnutChart.Concentric = true;
            UltraChart.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart.DoughnutChart.ShowConcentricLegend = false;
            UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.DoughnutChart.Labels.FormatString = "<ITEM_LABEL> <DATA_VALUE:P2>";

            UltraChart.Legend.Visible = false;
            UltraChart.Legend.Location = LegendLocation.Left;
            //UltraChart.Legend.SpanPercentage = 30;
            UltraChart.Legend.Margins.Bottom = 0;
            UltraChart.Legend.Margins.Top = 0;
            UltraChart.Legend.Margins.Left = 0;
            UltraChart.Legend.Margins.Right = 0;

            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 9; i++)
            {
                PaintElement pe = GetGradientPaintElement(GetColorColumnChart(i), 100);

                UltraChart.ColorModel.Skin.PEs.Add(pe);
            }
            UltraChart.ColorModel.Skin.ApplyRowWise = true;

            #endregion
        }
        private static PaintElement GetGradientPaintElement(Color fillStartColor, byte fillOpacity)
        {
            PaintElement pe = new PaintElement();
            pe.ElementType = PaintElementType.Gradient;
            pe.Fill = fillStartColor;
            pe.FillStopColor = CRHelper.GetDarkColor(fillStartColor, 60);
            pe.FillOpacity = fillOpacity;
            pe.FillGradientStyle = GradientStyle.BackwardDiagonal;
            return pe;
        }


        private static Color GetColorColumnChart(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.LawnGreen;
                    }
                case 2:
                    {
                        return Color.Magenta;
                    }
                case 3:
                    {
                        return Color.Gold;
                    }
                case 4:
                    {
                        return Color.Peru;
                    }
                case 5:
                    {
                        return Color.Cyan;
                    }
                case 6:
                    {
                        return Color.PeachPuff;
                    }
                case 7:
                    {
                        return Color.MediumSlateBlue;
                    }
                case 8:
                    {
                        return Color.ForestGreen;
                    }
                case 9:
                    {
                        return Color.HotPink;
                    }
            }
            return Color.White;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                string query = DataProvider.GetQueryText("FK_0001_0001_NAO_date");
                DataTable dtDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                yearComboValue.Value = endYear.ToString();
                monthComboValue.Value = month;
                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    subjectComboValue.Value = RegionSettings.Instance.Name;
                }
                else
                {
                    subjectComboValue.Value = "Àìóðñêàÿ îáëàñòü";
                }

                levelComboValue.Value = "Êîíñîëèäèðîâàííûé áþäæåò ñóáúåêòà";

                ComboYear.Title = "Ãîä";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetÑheckedState(yearComboValue.Value, true);

                ComboMonth.Title = "Ìåñÿö";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetÑheckedState(monthComboValue.Value, true);

                ComboSubject.Title = "Ôåäåðàëüíûé îêðóã";
                ComboSubject.Width = 410;
                ComboSubject.MultiSelect = false;
                ComboSubject.ParentSelect = false;
                ComboSubject.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                ComboSubject.SetÑheckedState(subjectComboValue.Value, true);

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Óðîâåíü áþäæåòà";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetÑheckedState(levelComboValue.Value, true);
            }

            yearComboValue.Value = ComboYear.SelectedValue;
            monthComboValue.Value = ComboMonth.SelectedValue;
            subjectComboValue.Value = ComboSubject.SelectedValue;
            levelComboValue.Value = ComboSKIFLevel.SelectedValue;

            UserParams.Region.Value = ComboSubject.SelectedNodeParent;
            UserParams.StateArea.Value = ComboSubject.SelectedValue;
            selectedLevel.Value = levelComboValue.Value;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);

            Page.Title = String.Format("Ñðàâíåíèå ïëàíîâîé è ôàêòè÷åñêîé ñòðóêòóðû ðàñõîäîâ ({0})", UserParams.StateArea.Value);
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{3} çà {0} {1} {2} ãîäà", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, levelComboValue.Value);

            UserParams.PeriodMonth.Value = monthComboValue.Value;
            UserParams.PeriodYear.Value = yearComboValue.Value;
            UserParams.PeriodHalfYear.Value = String.Format("Ïîëóãîäèå {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Êâàðòàë {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            
            UltraChart.DataBind();

            WallLink.Text = "Äëÿ&nbsp;âèäåîñòåíû";
            WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());

            BlackStyleWallLink.Text = "Äëÿ&nbsp;âèäåîñòåíû&nbsp;(÷åðíûé&nbsp;ñòèëü)";
            BlackStyleWallLink.NavigateUrl = String.Format("{0};onWall=true;blackStyle=true", UserParams.GetCurrentReportParamList());
        }

        private void SetScaleSize()
        {
            widthMultiplier = onWall ? 4.8 : 1;
            heightMultiplier = onWall ? 5 : 1;
            fontSizeMultiplier = onWall ? 5 : 1;
            primitiveSizeMultiplier = onWall ? 4 : 1;
            pageWidth = onWall ? 5600 : (int)Session["width_size"];
            pageHeight = onWall ? 1800 : (int)Session["height_size"] - 230;

            Font font8 = new Font("Verdana", Convert.ToInt32(8 * fontSizeMultiplier));
            Color fontColor = blackStyle ? Color.White : Color.Black;

            UltraChart.DoughnutChart.Labels.Font = font8;
            UltraChart.DoughnutChart.Labels.FontColor = fontColor;

            UltraChart.DoughnutChart.Labels.LeaderLineColor = fontColor;
            UltraChart.DoughnutChart.Labels.LeaderDrawStyle = LineDrawStyle.Solid;
            UltraChart.DoughnutChart.Labels.LeaderLineThickness = primitiveSizeMultiplier;
            
            UltraChart.Legend.Font = font8;
            UltraChart.Legend.FontColor = fontColor;

            UltraChart.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            CalloutAnnotation planAnnotation = new CalloutAnnotation();
            planAnnotation.TextStyle.Font = font8;
            planAnnotation.TextStyle.FontColor = Color.Black;
            planAnnotation.Text = "Èñïîëíåíî";
            planAnnotation.Width = 80 * primitiveSizeMultiplier;
            planAnnotation.Height = 20 * primitiveSizeMultiplier;
            planAnnotation.Location.Type = LocationType.Percentage;
            planAnnotation.Location.LocationX = 50;
            planAnnotation.Location.LocationY = 9;

            CalloutAnnotation factAnnotation = new CalloutAnnotation();
            factAnnotation.TextStyle.Font = font8;
            factAnnotation.TextStyle.FontColor = Color.Black;
            factAnnotation.Text = "Íàçíà÷åíî";
            factAnnotation.Width = 80 * primitiveSizeMultiplier;
            factAnnotation.Height = 20 * primitiveSizeMultiplier;
            factAnnotation.Location.Type = LocationType.Percentage;
            factAnnotation.Location.LocationX = 50;
            factAnnotation.Location.LocationY = 73;

            UltraChart.Annotations.Add(planAnnotation);
            UltraChart.Annotations.Add(factAnnotation);

            Label1.Font.Size = Convert.ToInt32(14 * fontSizeMultiplier);
            Label2.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);

            if (onWall)
            {
                ComprehensiveDiv.Style.Add("width", "5600px");
                ComprehensiveDiv.Style.Add("height", "2100px");
                //ComprehensiveDiv.Style.Add("border", "medium solid #FF0000");
            }

            if (onWall && Page.Master is IMasterPage)
            {
                ((IMasterPage)Page.Master).SetHeaderVisible(false);
            }

            WallLink.Visible = !onWall;
            BlackStyleWallLink.Visible = !onWall;
            ComboYear.Visible = !onWall;
            ComboMonth.Visible = !onWall;
            ComboSubject.Visible = !onWall;
            ComboSKIFLevel.Visible = !onWall;
            PopupInformer1.Visible = !onWall;
            RefreshButton1.Visible = !onWall;
        }

        #region Îáðàáîò÷èêè äèàãðàìì

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0001_NAO_compare_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Ðàéîí", dtChart);

            foreach (DataColumn column in dtChart.Columns)
            {
                column.ColumnName = GetShortRzPrName(column.ColumnName);
            }

            UltraChart.DataSource = dtChart;
        }

        private static string GetShortRzPrName(string fullName)
        {
            string shortName = fullName;

            switch (fullName)
            {
                case "ÎÁÙÅÃÎÑÓÄÀÐÑÒÂÅÍÍÛÅ ÂÎÏÐÎÑÛ":
                    {
                        return "Îáùåãîñóä.âîïðîñû";
                    }
                case "ÍÀÖÈÎÍÀËÜÍÀß ÎÁÎÐÎÍÀ":
                    {
                        return "Íàöèîíàëüíàÿ îáîðîíà";
                    }
                case "ÍÀÖÈÎÍÀËÜÍÀß ÁÅÇÎÏÀÑÍÎÑÒÜ È ÏÐÀÂÎÎÕÐÀÍÈÒÅËÜÍÀß ÄÅßÒÅËÜÍÎÑÒÜ":
                    {
                        return "Íàö.áåçîïàñíîñòü è ïðàâîîõðàíèò.äåÿò.";
                    }
                case "ÍÀÖÈÎÍÀËÜÍÀß ÝÊÎÍÎÌÈÊÀ":
                    {
                        return "Íàöèîíàëüíàÿ ýêîíîìèêà";
                    }
                case "ÆÈËÈÙÍÎ-ÊÎÌÌÓÍÀËÜÍÎÅ ÕÎÇßÉÑÒÂÎ":
                    {
                        return "ÆÊÕ";
                    }
                case "ÎÕÐÀÍÀ ÎÊÐÓÆÀÞÙÅÉ ÑÐÅÄÛ":
                    {
                        return "Îõðàíà îêðóæ.ñðåäû";
                    }
                case "ÎÁÐÀÇÎÂÀÍÈÅ":
                    {
                        return "Îáðàçîâàíèå";
                    }
                case "ÊÓËÜÒÓÐÀ, ÊÈÍÅÌÀÒÎÃÐÀÔÈß":
                    {
                        return "Êóëüòóðà è êèíåìàòîãðàôèÿ";
                    }
                case "ÊÓËÜÒÓÐÀ, ÊÈÍÅÌÀÒÎÃÐÀÔÈß, ÑÐÅÄÑÒÂÀ ÌÀÑÑÎÂÎÉ ÈÍÔÎÐÌÀÖÈÈ":
                case "ÑÐÅÄÑÒÂÀ ÌÀÑÑÎÂÎÉ ÈÍÔÎÐÌÀÖÈÈ":
                    {
                        return "ÑÌÈ";
                    }
                case "ÇÄÐÀÂÎÎÕÐÀÍÅÍÈÅ":
                    {
                        return "Çäðàâîîõðàíåíèå";
                    }
                case "ÇÄÐÀÂÎÎÕÐÀÍÅÍÈÅ, ÔÈÇÈ×ÅÑÊÀß ÊÓËÜÒÓÐÀ È ÑÏÎÐÒ":
                    {
                        return "Çäðàâîîõðàíåíèå, ñïîðò";
                    }
                case "ÔÈÇÈ×ÅÑÊÀß ÊÓËÜÒÓÐÀ È ÑÏÎÐÒ":
                    {
                        return "Ôèçè÷åñêàÿ êóëüòóðà è ñïîðò";
                    }
                case "ÑÎÖÈÀËÜÍÀß ÏÎËÈÒÈÊÀ":
                    {
                        return "Ñîöèàëüíàÿ ïîëèòèêà";
                    }
                case "ÌÅÆÁÞÄÆÅÒÍÛÅ ÒÐÀÍÑÔÅÐÒÛ":
                    {
                        return "Ìåæáþäæåòíûå òðàíñôåðòû";
                    }
                case "ÌÅÆÁÞÄÆÅÒÍÛÅ ÒÐÀÍÑÔÅÐÒÛ ÎÁÙÅÃÎ ÕÀÐÀÊÒÅÐÀ ÁÞÄÆÅÒÀÌ ÑÓÁÚÅÊÒÎÂ ÐÎÑÑÈÉÑÊÎÉ ÔÅÄÅÐÀÖÈÈ È ÌÓÍÈÖÈÏÀËÜÍÛÕ ÎÁÐÀÇÎÂÀÍÈÉ":
                    {
                        return "ÌÁÒ áþäæåòàì ñóá.ÐÔ è ÌÎ";
                    }
                case "ÎÁÑËÓÆÈÂÀÍÈÅ ÃÎÑÓÄÀÐÑÒÂÅÍÍÎÃÎ È ÌÓÍÈÖÈÏÀËÜÍÎÃÎ ÄÎËÃÀ":
                    {
                        return "Îáñëóæ.ãîñ.è ìóí.äîëãà";
                    }
            }
            return shortName;
        }

        #endregion
    }
}