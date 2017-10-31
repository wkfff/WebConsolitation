using System;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Drawing;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class RG_0001_0003 : CustomReportPage
    {
        private DateTime currentDate;
        private int year;
        private int lastYear;
        private string month;
        private int day;
        private string str;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Настройка грида

            GridBrick.BrowserSizeAdapting = false;
            GridBrick.Height = Unit.Empty;
            GridBrick.Width = Unit.Empty;
            GridBrick.RedNegativeColoring = false;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            currentDate = CubeInfo.GetLastDate(DataProvidersFactory.SpareMASDataProvider, "RG_0001_0003_date");
            day = currentDate.Day;
            month = CRHelper.RusMonthGenitive(currentDate.Month);
            CRHelper.SaveToErrorLog(month);
            year = currentDate.Year;
            lastYear = currentDate.Year - 1;

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 5);
            UserParams.PeriodYear.Value = lastYear.ToString();

           // IPadElementHeader1.Text = "Охват и доступность услуг дошкольного образования";

            lbInfo.Text = string.Empty;
            UltraChart1.Visible = true;
            InitializeTable1();
            BindInfoText();
        }

        #region

        private void BindInfoText()
        {
            string query = DataProvider.GetQueryText("RG_0001_0003_text1");
            DataTable dtInfo = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtInfo);
            if (dtInfo.Rows.Count > 0)
            {
                lbInfo.Text = string.Format(@"&nbsp;&nbsp; По состоянию на <b> <span style='color:white;'>&nbsp;{0}&nbsp;{1}&nbsp;{2}&nbsp;</span></b> общая численность детей в возрасте от 1 до 7 лет составляет <b><span style='color:white;'>&nbsp;{3:N2}&nbsp;тыс. человек</span></b>,&nbsp;численность детей данного возраста, охваченных услугами дошкольного образования, составляет <b><span style='color:white;'>&nbsp;{4:N2}&nbsp;тыс.человек</span></b> (&nbsp;<img style = 'background-color:transparent' align = 'center' src='../../../TemporaryImages/Chart_RG_0001_0003_{5}.png'> &nbsp;<span style='color:white;'>{6:N1}</span>%&nbsp; от общей численности детей от 1 до 7 лет).", day, CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)), year, dtInfo.Rows[0][1], dtInfo.Rows[0][2], dtInfo.Rows[0][3].ToString().Replace(",", "_").Replace(".", "_"), dtInfo.Rows[0][3]);
            }

            query = DataProvider.GetQueryText("RG_0001_0003_text2");
            dtInfo = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtInfo);
            if (dtInfo.Rows.Count > 0)
            {
                DataTable dtChart = new DataTable();
                dtChart.Columns.Add(new DataColumn("1", typeof(double)));

                int value = 0;
                if (dtInfo.Rows[0][2] != DBNull.Value && dtInfo.Rows[0][2].ToString() != string.Empty)
                {

                    UltraChart1.Width = 35;
                    UltraChart1.Height = 35;

                    UltraChart1.ChartType = ChartType.PieChart;
                    UltraChart1.Border.Thickness = 0;
                    UltraChart1.Border.Color = Color.Black;
                    UltraChart1.BorderColor = Color.Black;

                    UltraChart1.BackColor = Color.Black;
                    UltraChart1.BackColor = Color.Black;

                    UltraChart1.PieChart.OthersCategoryPercent = 0;
                    UltraChart1.PieChart.OthersCategoryText = "Прочие";
                    UltraChart1.PieChart.Labels.Visible = false;
                    UltraChart1.PieChart.Labels.LeaderLinesVisible = false;
                    UltraChart1.PieChart.Labels.FontColor = Color.Black;
                    UltraChart1.Tooltips.FormatString =
                        "<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";

                    UltraChart1.Legend.Visible = false;
                    UltraChart1.PieChart.StartAngle = 270;
                    UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
                    UltraChart1.ColorModel.Skin.ApplyRowWise = true;
                    UltraChart1.ColorModel.Skin.PEs.Clear();

                    for (int i = 1; i <= 2; i++)
                    {
                        PaintElement pe = new PaintElement();
                        Color color = GetColor(i);
                        Color stopColor = GetColor(i);

                        pe.Fill = color;
                        pe.FillStopColor = stopColor;
                        pe.ElementType = PaintElementType.Gradient;
                        pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                        pe.FillOpacity = 150;
                        UltraChart1.ColorModel.Skin.PEs.Add(pe);
                    }

                    value = Convert.ToInt32(dtInfo.Rows[0][2]);
                    DataRow row = dtChart.NewRow();
                    row[0] = value;
                    dtChart.Rows.Add(row);

                    row = dtChart.NewRow();
                    row[0] = 0;
                    dtChart.Rows.Add(row);

                    row = dtChart.NewRow();
                    row[0] = 0;
                    dtChart.Rows.Add(row);

                    row = dtChart.NewRow();
                    row[0] = 100 - value;
                    dtChart.Rows.Add(row);

                    UltraChart1.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'));
                    
                    UltraChart1.DataSource = dtChart;
                    UltraChart1.DataBind();

                    //UltraChart1.SaveTo(Path.Combine(CRHelper.BasePath, String.Format("TemporaryImages\\Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'))), ImageFormat.Png);
                    string serverPath = Path.Combine(CRHelper.BasePath, String.Format("TemporaryImages\\Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_')));

                    MemoryStream stream = new MemoryStream();
                    UltraChart1.SaveTo(stream, ImageFormat.Png);
                    Bitmap bmp = new Bitmap(stream);
                    Bitmap copyBmp = bmp.Clone(new Rectangle(1, 1, bmp.Width - 1, bmp.Height - 1), bmp.PixelFormat);
                    copyBmp.Save(serverPath);
                }

                lbInfo.Text = String.Format(@"{0}  <br />&nbsp;&nbsp; Очередность в дошкольные образовательные   учреждения  в целом по ЦФО составляет <b>&nbsp;<span style='color:white;'>{1:N0}</span>&nbsp;человек </b> (<img style = 'background-color:transparent' align = 'center' src='../../../TemporaryImages/Chart_RG_0001_0003_{3}.png'>&nbsp;<span style='color:white;'>{2:N1}</span>%&nbsp; от общей численности детей в возрасте от 1 до 7 лет).", lbInfo.Text, dtInfo.Rows[0][1], dtInfo.Rows[0][2], value.ToString().Replace(',', '_').Replace('.', '_'));
            }

            query = DataProvider.GetQueryText("RG_0001_0003_text3");
            DataTable dtText = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtText);
            str = string.Empty;
            int endYear;
            int firstYear;
            if (dtText.Rows.Count > 0 && dtInfo.Rows.Count > 0)
            {
                if (dtInfo.Rows[0][3] != DBNull.Value && dtInfo.Rows[0][3].ToString() != string.Empty)
                {
                    firstYear = Convert.ToInt32(dtInfo.Rows[0][3]);
                    if (dtInfo.Rows[0][4] != DBNull.Value && dtInfo.Rows[0][4].ToString() != string.Empty)
                    {
                        endYear = Convert.ToInt32(dtInfo.Rows[0][4]);
                        for (int i = firstYear; i <= endYear; i++)
                        {
                            if (i == firstYear)
                            {
                                str += string.Format("в{1}{0}{1}году планируется в ", i, "&nbsp;");
                            }
                            else
                            {
                                str += string.Format("&nbsp;в&nbsp;{0}&nbsp;году - ", i);
                            }
                            for (int j = 0; j < dtText.Rows.Count; j++)
                            {

                                if (i == Convert.ToInt32(dtText.Rows[j][1]))
                                {
                                    str += string.Format("{0}, ",
                                                         dtText.Rows[j][0].ToString().Replace("ая", "ой").Replace(
                                                             " область", ""));
                                }

                            }
                            str += "областях;";
                        }
                    }
                }


                string nullregions = string.Empty;
                query = DataProvider.GetQueryText("RG_0001_0003_text4");
                DataTable dtNull = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                   dtNull);

                if (dtNull.Rows.Count > 0)
                {
                    string list = string.Empty;
                    for (int i = 0; i < dtNull.Rows.Count; i++)
                    {
                        list += string.Format("{0}",
                                              dtNull.Rows[i][0].ToString().Replace("ая", "ой").Replace("область",
                                                                                                       "области").
                                                  Replace("г. Москва", "г.&nbsp;Москва"));
                    }
                    nullregions = string.Format("В&nbsp;{0}&nbsp;с&nbsp;{1}&nbsp;года очерёдность отсутствует.", list, lastYear);
                }

                lbInfo.Text =
                    String.Format(
                        @"{0}  <br/>&nbsp;&nbsp; Предполагаемый год ликвидации очерёдности в регионах находится в диапазоне от <b>&nbsp;<span style='color:white;'>{1}</span>&nbsp;</b> до <b>&nbsp;<span style='color:white;'>{2}</span>&nbsp;</b> года. Ликвидировать  очерёдность {3}. {4}",
                        lbInfo.Text, dtInfo.Rows[0][3], dtInfo.Rows[0][4], str
                                                                               .TrimEnd(';').Replace(", областях",
                                                                                                     " областях"),
                        nullregions);

                lbInfo.Text = String.Format(@"{0}", lbInfo.Text);
                CRHelper.SaveToErrorLog(lbInfo.Text);
            }
            UltraChart1.Visible = false;
        }

        private DataTable GridDt;

        private void InitializeTable1()
        {
            string query = DataProvider.GetQueryText("RG_0001_0003_grid");
            GridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Субъект РФ", GridDt);

            if (GridDt.Rows.Count > 0)
            {
                FontRowLevelRule levelRule = new FontRowLevelRule(GridDt.Columns.Count - 1);
                levelRule.AddFontLevel("1", new Font(GridBrick.Grid.DisplayLayout.RowStyleDefault.Font.Name, 12, FontStyle.Bold));
                GridBrick.AddIndicatorRule(levelRule);
                GridBrick.DataTable = GridDt;
            }

        }

        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[3].Value != null)
            {
                SetupChart();
                DataTable dtChart = new DataTable();
                dtChart.Columns.Add(new DataColumn("1", typeof(double)));

                double value = Convert.ToDouble(e.Row.Cells[3].Value.ToString());
                DataRow row = dtChart.NewRow();
                row[0] = value;
                dtChart.Rows.Add(row);

                row = dtChart.NewRow();
                row[0] = 0;
                dtChart.Rows.Add(row);

                row = dtChart.NewRow();
                row[0] = 0;
                dtChart.Rows.Add(row);

                row = dtChart.NewRow();
                row[0] = 100 - value;
                dtChart.Rows.Add(row);

                UltraChart1.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Chart_FO_0035_0028_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'));
              
                UltraChart1.DataSource = dtChart;
                UltraChart1.DataBind();

                StringWriter writer = new StringWriter();
                HtmlTextWriter output = new HtmlTextWriter(writer);

                UltraChart1.RenderControl(output);

              //  UltraChart1.SaveTo( Path.Combine (CRHelper.BasePath , String.Format("TemporaryImages\\Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'))), ImageFormat.Png);

                string serverPath = Path.Combine(CRHelper.BasePath, String.Format("TemporaryImages\\Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_')));
                
                MemoryStream stream = new MemoryStream();
                UltraChart1.SaveTo(stream, ImageFormat.Png);
                Bitmap bmp = new Bitmap(stream);
                Bitmap copyBmp = bmp.Clone(new Rectangle(1, 1, bmp.Width - 1, bmp.Height - 1), bmp.PixelFormat);
                copyBmp.Save(serverPath);

                e.Row.Cells[3].Style.BackgroundImage = String.Format("../../../TemporaryImages/Chart_RG_0001_0003_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'));
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-left: 2px";
                e.Row.Cells[3].Value = String.Format("{0:N1}", e.Row.Cells[3].Value);
            }
        }

        private void SetupChart()
        {
            UltraChart1.Width = 35;
            UltraChart1.Height = 35;

            UltraChart1.ChartType = ChartType.PieChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Border.Color = Color.Black;
            UltraChart1.BorderColor = Color.Black;

            UltraChart1.BackColor = Color.Black;
            UltraChart1.BackColor = Color.Black;
            
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.PieChart.OthersCategoryText = "Прочие";
            UltraChart1.PieChart.Labels.Visible = false;
            UltraChart1.PieChart.Labels.LeaderLinesVisible = false;
            UltraChart1.PieChart.Labels.FontColor = Color.Black;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";

            UltraChart1.Legend.Visible = false;
            UltraChart1.PieChart.StartAngle = 270;
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.ForestGreen;
                    }
                case 2:
                    {
                        return Color.Red;
                    }
                default:
                    {
                        return Color.Black;
                    }
            }
        }

        void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(60, 60, 60);

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "0000");
           
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Субъект РФ");

            GridHeaderCell cell1 = headerLayout.AddCell("Численность детей, охваченных услугами дошкольного образования, всего человек");
            cell1.AddCell(string.Format("Справочно по данным Минобразования РФ в {0}", lastYear));
            cell1.AddCell(string.Format("На {0:dd.MM.yyyy}", currentDate));
            headerLayout.AddCell("Охват детей услугами дошкольного образования в возрасте от 1 до 7 лет, %");
            GridHeaderCell cell2 = headerLayout.AddCell("Численность детей дошкольного возраста, состоящих на учете для определения в гос. и муницип. ДОУ (очередность 1-7), человек, из них:");
            cell2.AddCell("в возрасте от 1 до 7 лет", "", 2);
            cell2.AddCell("в возрасте от 3 до 7 лет", "", 2);
            headerLayout.AddCell("Год ликвидации очереди"); 

            headerLayout.ApplyHeaderInfo();
            
            e.Layout.Bands[0].Columns[1].Width =  CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(90);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(115);
            e.Layout.Bands[0].Columns[4].Width =  CRHelper.GetColumnWidth(90);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(90);
            e.Layout.Bands[0].Columns[6].Width =  CRHelper.GetColumnWidth(100);
       
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[4].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[5].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[6].CellStyle.Font.Size = 12;
        }

        #endregion
    }
}
