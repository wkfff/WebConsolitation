using System;
using System.Collections;
using System.Data;
using System.Drawing;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0002_NAO
{
    public partial class DefaultMap : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtMap;
        private int firstYear = 2000;
        private int endYear = 2011;

        #region ��������� �������

        private CustomParam yearComboValue;
        private CustomParam monthComboValue;
        private CustomParam foComboValue;
        private CustomParam kdComboValue;
        private CustomParam selectedSubject;

        #endregion

        private bool onWall;
        private bool blackStyle;
        private double widthMultiplier;
        private double heightMultiplier;
        private int fontSizeMultiplier;
        private int pageWidth;
        private int pageHeight;
        private int mapBorderWidthMultiplier;

        private Color fontColor;
        private Color borderColor;

        private bool IsRfSelected
        {
            get { return ComboFO.SelectedIndex == 0 && ComboFO.SelectedNode.Level == 0; }
        }

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

            fontColor = blackStyle ? Color.White : Color.Black;
            borderColor = Color.Black;
        }


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ������������� ���������� �������

            yearComboValue = UserParams.CustomParam("year_combo_value", true);
            monthComboValue = UserParams.CustomParam("month_combo_value", true);
            foComboValue = UserParams.CustomParam("fo_combo_value", true);
            kdComboValue = UserParams.CustomParam("kd_combo_value", true);
            selectedSubject = UserParams.CustomParam("selected_subject");

            #endregion

            Session["blackStyle"] = null;

            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
                Session["onWall"] = null;
            }

            SetScaleSize();

            DundasMap.Width = pageWidth - 30;
            DundasMap.Height = pageHeight;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0002_NAO_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                yearComboValue.Value = endYear.ToString();
                monthComboValue.Value = month;
                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    foComboValue.Value = RegionSettings.Instance.Name;
                }
                else
                {
                    foComboValue.Value = "����������  ���������";
                }
                kdComboValue.Value = "������ ����� ";

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(yearComboValue.Value, true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(monthComboValue.Value, true);

                ComboFO.Title = "��";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary, true));
                ComboFO.Set�heckedState(foComboValue.Value, true);

                ComboKD.Width = 420;
                ComboKD.Title = "��� ������";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullKDIncludingList());
                ComboKD.RenameTreeNodeByName("������ ������� - ����� ", "������ ����� ");
                ComboKD.Set�heckedState(kdComboValue.Value, true);
            }

            yearComboValue.Value = ComboYear.SelectedValue;
            monthComboValue.Value = ComboMonth.SelectedValue;
            foComboValue.Value = ComboFO.SelectedValue;
            kdComboValue.Value = ComboKD.SelectedValue;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Page.Title = string.Format("���������� ������� ({0})", ComboFO.SelectedIndex == 0
                                                                       ? "��"
                                                                       :
                                                                   RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;
            Label2.Text = String.Format(
                    "���������� ����������������� �������� ��������� �� �� ������� ({3}) �� {0} {1} {2} ����", monthNum,
                    CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboKD.SelectedValue.TrimEnd(' '));

            int year = Convert.ToInt32(yearComboValue.Value);

            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = monthComboValue.Value;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.KDGroup.Value = kdComboValue.Value;
            selectedSubject.Value = IsRfSelected 
                ? String.Format("[����������].[������������].[{0}].Children", foComboValue.Value)
                : String.Format("[����������].[������������].[{0}]", foComboValue.Value);

            string mapName;
            if (RegionsNamingHelper.IsRF(foComboValue.Value))
            {
                mapName = "���������� ���������";
            }
            else if (RegionsNamingHelper.IsFO(foComboValue.Value))
            {
                mapName = foComboValue.Value;
            }
            else
            {
                mapName = RegionsNamingHelper.GetFoBySubject(foComboValue.Value);
            }

            string mapFileName = String.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(mapName));

            // ��������� ����� �������
            DundasMap.Shapes.Clear();

            DundasMap.LoadFromShapeFile(Server.MapPath(mapFileName), "NAME", true);

//            DundasMap.Serializer.Format = Dundas.Maps.WebControl.SerializationFormat.Binary;
//            DundasMap.Serializer.Load((Server.MapPath(mapFileName)));

            SetupMap();
            // ��������� ����� �������
            FillMapData();

            WallLink.Text = "���&nbsp;����������";
            WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());

            BlackStyleWallLink.Text = "���&nbsp;����������&nbsp;(������&nbsp;�����)";
            BlackStyleWallLink.NavigateUrl = String.Format("{0};onWall=true;blackStyle=true", UserParams.GetCurrentReportParamList());
        }

        private void SetScaleSize()
        {
            widthMultiplier = onWall ? 4.8 : 1;
            heightMultiplier = onWall ? 5 : 1;
            fontSizeMultiplier = onWall ? 5 : 1;
            mapBorderWidthMultiplier = onWall ? 3 : 1;
            pageWidth = onWall ? 5600 : (int)Session["width_size"];
            pageHeight = onWall ? 1800 : (int)Session["height_size"] - 200;

            Label1.Font.Size = Convert.ToInt32(14 * fontSizeMultiplier);
            Label2.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);

            DundasMap.Font.Size = 8 * fontSizeMultiplier;

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
            ComboFO.Visible = !onWall;
            ComboKD.Visible = !onWall;
            PopupInformer1.Visible = !onWall;
            RefreshButton1.Visible = !onWall;
        }

        #region ����������� �����

        private void SetupMap()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = !onWall;
            DundasMap.NavigationPanel.Visible = !onWall;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.BackColor = blackStyle ? Color.Black : Color.White;

            // ��������� �������
            Legend legend = new Legend("CompleteLegend");
            legend.Visible = true;
            legend.BackColor = blackStyle ? Color.Black : Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = fontColor;
            legend.Font = new Font("Verdana", 8*fontSizeMultiplier, FontStyle.Regular);
            legend.Title = "������� ����������";
            legend.TitleFont = new Font("Verdana", 9 * fontSizeMultiplier, FontStyle.Regular);
            legend.TitleColor = fontColor;
            legend.AutoFitText = false;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // ��������� ����
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

            // ��������� ������� ���������
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "CompletePercent";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = blackStyle ? Color.DarkOrange : Color.Yellow;
            rule.ToColor = blackStyle ? Color.Green : Color.LightGreen;
            rule.BorderColor = Color.FromArgb(50, fontColor);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{N2} - #TOVALUE{N2}";
            rule.GradientType = blackStyle ? GradientType.None : GradientType.ReversedCenter;
            rule.SecondaryColor = Color.White;
            DundasMap.ShapeRules.Add(rule);
        }

        /// <summary>
        /// ����� ����� �����
        /// </summary>
        /// <param name="map">�����</param>
        /// <param name="patternValue">������� ��� �����</param>
        /// <param name="searchFO">true, ���� ���� ��</param>
        /// <returns>��������� �����</returns>
        public static Shape FindMapShape(MapControl map, string patternValue)
        {
            string subject = string.Empty;
            bool isRepublic = patternValue.Contains("����������");
            bool isTown = patternValue.Contains("�.");
            string[] subjects = patternValue.Split(' ');
            if (subjects.Length > 1)
            {
                // ���� ����� ������ ������ ������������� ���� ���������
                switch (subjects[0])
                {
                    case "���������":
                        {
                            subject = "�����";
                            break;
                        }
                    case "���������-����������":
                        {
                            subject = "���������-���������";
                            break;
                        }
                    case "���������-����������":
                        {
                            subject = "���������-��������";
                            break;
                        }
                    case "����������":
                        {
                            subject = "��������";
                            break;
                        }
                    case "���������":
                        {
                            subject = "�������";
                            break;
                        }
                    default:
                        {
                            subject = (isRepublic || isTown ) ? subjects[1] : subjects[0];
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

        public void FillMapData()
        {
            string query = DataProvider.GetQueryText("FK_0001_0002_NAO_map");
            dtMap = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�������", dtMap);

            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Text = String.Empty;
                shape.Font = new Font("Verdana", 8 * fontSizeMultiplier);
            }

            foreach (DataRow row in dtMap.Rows)
            {
                // ��������� ����� �������
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();
                    if (RegionsNamingHelper.IsFO(subject) || RegionsNamingHelper.IsSubject(subject))
                    {
                        Shape shape = FindMapShape(DundasMap, subject);
                        if (shape != null)
                        {
                            if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                            {
                                double value = 100*Convert.ToDouble(row[1]);

                                shape["Name"] = subject;
                                shape["CompletePercent"] = value;
                                shape.ToolTip = "#NAME #COMPLETEPERCENT{N2}%";
                                shape.TextVisibility = TextVisibility.Shown;

                                shape.Text = String.Format("{0}\n{1:N2}%", RegionsNamingHelper.ShortName(subject), value);

                                shape.BorderColor = borderColor;
                                shape.TextColor = fontColor;

                                if (subject == foComboValue.Value)
                                {
                                    shape.BorderWidth = 3 * mapBorderWidthMultiplier;
                                }
                                else
                                {
                                    shape.BorderWidth = mapBorderWidthMultiplier;
                                }
                            }
                            else
                            {
                                shape.Text = subject;
                                shape.ToolTip = subject;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
