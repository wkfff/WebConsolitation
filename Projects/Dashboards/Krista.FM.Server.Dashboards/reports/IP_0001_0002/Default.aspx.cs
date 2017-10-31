using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Dashboards.reports.IP_0001_0002
{
    public partial class Default : CustomReportPage
    {

        private IDatabase db;

        private CustomParam projectID;
        private CustomParam year;
        private CustomParam quarter;
        private CustomParam refTypeI;
        private CustomParam code;

        DataTable dtChart1;

        private static Dictionary<string, string> dictProject;
        private static Dictionary<string, string> dictProjectRev;

        static int beginYear = 2000;
        static int endYear = DateTime.Now.Year;

        static string selectedYear;

        private static int Resolution
        {
            get { return CRHelper.GetScreenWidth; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            db = GetDataBase();

            // Установка размеров
            if (Resolution < 900)
            {
                UltraWebGrid1.Width = Unit.Parse("725px");
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid1.Width = Unit.Parse("950px");
            }
            else
            {
                UltraWebGrid1.Width = Unit.Parse("1200px");
            }

            UltraWebGrid2.Width = UltraWebGrid1.Width;
            UltraWebGrid3.Width = UltraWebGrid1.Width;
            UltraChart1.Width = new Unit(UltraWebGrid1.Width.Value - 5);
            UltraWebGrid4.Width = UltraWebGrid1.Width;

            HeaderTable.Width = String.Format("{0}px", (int)UltraWebGrid1.Width.Value);

            #region Гриды

            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid2.Height = Unit.Empty;
            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid2_InitializeLayout);
            UltraWebGrid2.DataBinding += new EventHandler(UltraWebGrid2_DataBinding);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid3.Height = Unit.Empty;
            UltraWebGrid3.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid3_InitializeLayout);
            UltraWebGrid3.DataBinding += new EventHandler(UltraWebGrid3_DataBinding);
            UltraWebGrid3.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid4.Height = Unit.Empty;
            UltraWebGrid4.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid4_InitializeLayout);
            UltraWebGrid4.DataBinding += new EventHandler(UltraWebGrid4_DataBinding);
            UltraWebGrid4.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid4_InitializeRow);
            UltraWebGrid4.DisplayLayout.NoDataMessage = "Нет данных";
            
            #endregion

            #region Настройка диаграммы

            UltraChart1.Height = Unit.Parse("500px");

            UltraChart1.ChartType = ChartType.StackColumnChart;

            UltraChart1.Border.Thickness = 0;

            UltraChart1.ColumnChart.NullHandling = NullHandling.Zero;

            UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart1.Axis.X.Margin.Near.Value = 10;
            UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart1.Axis.X.Margin.Far.Value = 10;
            UltraChart1.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart1.Axis.Y.Margin.Near.Value = 10;
            UltraChart1.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart1.Axis.Y.Margin.Far.Value = 10;

            UltraChart1.Axis.X.Extent = 40;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.WrapText = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.X.Labels.HorizontalAlign = StringAlignment.Center;
            UltraChart1.Axis.X.MajorGridLines.Visible = false;
            UltraChart1.Axis.X.MinorGridLines.Visible = false;
            //UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 45;
            UltraChart1.Axis.Y.Extent = 100;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 10;
            UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
            UltraChart1.Legend.Visible = true;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LimeGreen;
            Color color2 = Color.Firebrick;

            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;

            UltraChart1.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Style = GradientStyle.ForwardDiagonal;
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart1.Effects.Enabled = true;
            UltraChart1.Effects.Effects.Add(effect);

            ChartTextAppearance appearance1 = new ChartTextAppearance();
            appearance1.Column = 1;
            appearance1.Row = -2;
            appearance1.VerticalAlign = StringAlignment.Near;
            appearance1.HorizontalAlign = StringAlignment.Far;
            appearance1.ItemFormatString = "<SERIES_LABEL>";
            appearance1.ChartTextFont = new Font("Verdana", 8);
            appearance1.Visible = true;
            appearance1.FontColor = Color.Black;
            UltraChart1.ColumnChart.ChartText.Add(appearance1);

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            projectID = UserParams.CustomParam("project_id", true);
            year = UserParams.CustomParam("year");
            quarter = UserParams.CustomParam("quarter");
            code = UserParams.CustomParam("code");
            refTypeI = UserParams.CustomParam("ref_type_i");

            Page.Title = PageTitle.Text = String.Format("Карточка приоритетного инвестиционного проекта");            
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {

                if (Request.Form["__EVENTTARGET"] != null &&
                                Request.Form["__EVENTTARGET"].Contains("ComboYear"))
                {
                    selectedYear = ComboYear.SelectedValue;
                }
                
                if (!Page.IsPostBack)
                {
                    ComboProject.Title = "Инвестиционный проект";
                    ComboProject.ParentSelect = true;
                    ComboProject.MultiSelect = false;
                    ComboProject.Width = 650;
                    FillProjectsDictionary(ComboProject);

                    if (ComboProject.GetRootNodesCount() == 0)
                    {
                        db.Dispose();
                        return;
                    }

                    ComboYear.Title = "Год реализации";
                    ComboYear.ParentSelect = true;
                    ComboYear.MultiSelect = false;
                    ComboYear.Width = 300;
                }

                projectID.Value = GetProjectID(ComboProject.SelectedValue);

                UltraWebGrid1.DataBind();
                UltraWebGrid2.DataBind();
                UltraWebGrid3.DataBind();
                UltraChart1.DataBind();

                FillYearsDictionary(ComboYear);

                selectedYear = ComboYear.SelectedValue;

                UltraWebGrid4.Bands.Clear();
                UltraWebGrid4.DataBind();
                
                if (Request.Form["__EVENTTARGET"] != null &&
                                Request.Form["__EVENTTARGET"].Contains("ComboYear"))
                {
                    Response.Redirect("~/reports/IP_0001_0002/default.aspx#grid4");
                }
            }
            finally
            {
                db.Dispose();
            }

        }

        #region Грид 1

        private void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string[] param = { "Место реализации", "Инвестор:", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;юридический адрес", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;почтовый адрес",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;электронная почта", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;телефон",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;контактное лицо", "Цель проекта", "Год начала реализации", "Год окончания реализации",
                                 "Технико-экономические показатели:", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;планируемые результаты",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;виды конечной продукции", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;срок окупаемости",
                                 "основание для заключения инвестиционного соглашения", "Инвестиционное соглашение", "Использование дополнительных механизмов реализации",
                                 "Наличие экспертного соглашения", "Вид деятельности", "Дата включения в реестр приоритетных инвестиционных проектов" };
            string query = DataProvider.GetQueryText("IP_0001_0002_grid1");
            DataTable dtProject = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            DataRow row = dtProject.Rows[0];

            Page.Title = PageTitle.Text = String.Format("Приоритетный инвестиционный проект: «{0}», (Идентификационный номер: {1})", row["Name"], row["Code"]);

            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("Наименование поля", typeof(string));
            dtGrid.Columns.Add("Значение", typeof(string));
            for (int i = 0; i < param.Length && i < dtProject.Columns.Count - 2; ++i)
            {
                DataRow newRow = dtGrid.NewRow();
                newRow["Наименование поля"] = param[i];
                newRow["Значение"] = param[i] != "Дата включения в реестр приоритетных инвестиционных проектов" ? row[i + 2] : String.Format("{0:D}", row[i + 2]);
                dtGrid.Rows.Add(newRow);
            }

            Int32.TryParse(row["BeginYear"].ToString(), out beginYear);
            Int32.TryParse(row["EndYear"].ToString(), out endYear);

            (sender as UltraWebGrid).DataSource = dtGrid;
        }

        private void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;
            
            UltraGridBand band = grid.Bands[0];
            band.Columns[0].Width = Unit.Parse("200px");
            band.Columns[1].Width = Unit.Parse(String.Format("{0}px", (int)grid.Width.Value - 201));
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[0].CellStyle.Font.Bold = true;
            band.Columns[1].CellStyle.Wrap = true;
            band.HeaderLayout.Clear();

            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowDeleteDefault = AllowDelete.No;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            //e.Layout.RowSelectorStyleDefault.Width = Unit.Parse("1px");
        }

        #endregion

        #region Грид 2

        private void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            LabelGrid2.Text = "Плановый объем инвестиций";

            DataTable dtGrid2 = new DataTable();
            dtGrid2.Columns.Add("Источник инвестиций", typeof(string));
            dtGrid2.Columns.Add(" ", typeof(string));
            dtGrid2.Columns.Add("Всего", typeof(double));
            for (int year = beginYear; year <= endYear; ++year)
                dtGrid2.Columns.Add(year.ToString(), typeof(double));

            string query = DataProvider.GetQueryText("IP_0001_0002_grid2");
            DataTable dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            foreach (DataRow dataRow in dtData.Rows)
            {
                string name = dataRow["Name"].ToString();
                string year = dataRow["DateYear"].ToString();
                double value = 0;
                Double.TryParse(dataRow["Value"].ToString(), out value);
                if (dtGrid2.Columns.IndexOf(year) > -1)
                {
                    DataRow row = GetRow(dtGrid2, 0, name);
                    row[year] = value;
                    row["Всего"] = MathHelper.Plus(row["Всего"], row[year], DBNull.Value, CalcMode.CalcIfOne);
                    row[1] = "тыс.руб.";
                }
                dtGrid2.AcceptChanges();
            }

            (sender as UltraWebGrid).DataSource = dtGrid2;
        }

        private void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;

            UltraGridBand band = grid.Bands[0];

            band.Columns[0].Width = Unit.Parse("200px");
            band.Columns[0].CellStyle.Font.Bold = true;
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[1].Width = Unit.Parse("50px");
            for (int i = 2; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse("90px");
                CRHelper.FormatNumberColumn(band.Columns[i], "N2");
            }

            band.HeaderStyle.Wrap = true;
            band.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

        }

        #endregion

        #region Грид 3

        private void UltraWebGrid3_DataBinding(object sender, EventArgs e)
        {
            LabelGrid3.Text = "Плановые формы и объемы государственной поддержки";

            DataTable dtGrid3 = new DataTable();
            dtGrid3.Columns.Add("Форма государственной поддержки", typeof(string));
            dtGrid3.Columns.Add(" ", typeof(string));
            dtGrid3.Columns.Add("Всего", typeof(double));
            for (int year = beginYear; year <= endYear; ++year)
                dtGrid3.Columns.Add(year.ToString(), typeof(double));

            string query = DataProvider.GetQueryText("IP_0001_0002_grid3");
            DataTable dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            foreach (DataRow dataRow in dtData.Rows)
            {
                string name = dataRow["Name"].ToString();
                string year = dataRow["DateYear"].ToString();
                double value = 0;
                Double.TryParse(dataRow["Value"].ToString(), out value);
                if (dtGrid3.Columns.IndexOf(year) > -1)
                {
                    DataRow row = GetRow(dtGrid3, 0, name);
                    row[year] = value;
                    row["Всего"] = MathHelper.Plus(row["Всего"], row[year], DBNull.Value, CalcMode.CalcIfOne);
                    row[1] = "тыс.руб.";
                }
                dtGrid3.AcceptChanges();
            }

            (sender as UltraWebGrid).DataSource = dtGrid3;
        }

        private void UltraWebGrid3_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;

            UltraGridBand band = grid.Bands[0];

            band.Columns[0].Width = Unit.Parse("200px");
            band.Columns[0].CellStyle.Font.Bold = true;
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[1].Width = Unit.Parse("50px");
            for (int i = 2; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse("90px");
                CRHelper.FormatNumberColumn(band.Columns[i], "N2");
            }

            band.HeaderStyle.Wrap = true;
            band.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

        }

        #endregion

        #region Диаграмма

        private void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            LabelChart1.Text = "Сравнение плановых и фактических объемов инвестиций, тыс. руб.";

            dtChart1 = new DataTable();
            dtChart1.Columns.Add("Год", typeof(string));
            dtChart1.Columns.Add("Внебюджетные средства", typeof(double));
            dtChart1.Columns.Add("Государственная поддержка", typeof(double));

            code.Value = "5";
            year.Value = (beginYear - 1).ToString();
            string query = DataProvider.GetQueryText("IP_0001_0002_chart1_fact").Replace("and d_ydu.Name like 'Квартал%'", String.Empty);
            DataTable dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);

            DataRow row = dtChart1.NewRow();
            row["Год"] = "Год перед\nначалом\nреализации - Факт";
            row["Внебюджетные средства"] = dtData.Rows.Count != 0 ? dtData.Rows[0][0] : DBNull.Value;
            dtChart1.Rows.Add(row);
            row = dtChart1.NewRow();
            row["Год"] = " ";
            dtChart1.Rows.Add(row);
            for (int i = beginYear; i <= endYear; ++i)
            {
                bool atLeastOneRow = false;
                year.Value = i.ToString();
                row = dtChart1.NewRow();
                row["Год"] = String.Format("{0} - План", i);
                refTypeI.Value = "2";
                query = DataProvider.GetQueryText("IP_0001_0002_chart1_plan");
                dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                row["Государственная поддержка"] = dtData.Rows.Count != 0 ? dtData.Rows[0][0] : DBNull.Value;
                refTypeI.Value = "1";
                query = DataProvider.GetQueryText("IP_0001_0002_chart1_plan");
                dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                row["Внебюджетные средства"] = dtData.Rows.Count != 0 ? dtData.Rows[0][0] : DBNull.Value;
                //row["Внебюджетные средства"] = MathHelper.Minus(row["Внебюджетные средства"], row["Государственная поддержка"], DBNull.Value, CalcMode.CalcIfOne);
                if (row["Внебюджетные средства"] != DBNull.Value || row["Государственная поддержка"] != DBNull.Value)
                {
                    dtChart1.Rows.Add(row);
                    atLeastOneRow = true;
                }
                
                row = dtChart1.NewRow();
                row["Год"] = String.Format("{0} - Факт", i);
                code.Value = "6";
                query = DataProvider.GetQueryText("IP_0001_0002_chart1_fact");
                dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                row["Государственная поддержка"] = dtData.Rows.Count != 0 ? dtData.Rows[0][0] : DBNull.Value;
                code.Value = "5";
                query = DataProvider.GetQueryText("IP_0001_0002_chart1_fact");
                dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                row["Внебюджетные средства"] = dtData.Rows.Count != 0 ? dtData.Rows[0][0] : DBNull.Value;
                if (row["Внебюджетные средства"] != DBNull.Value || row["Государственная поддержка"] != DBNull.Value)
                {
                    dtChart1.Rows.Add(row);
                    atLeastOneRow = true;
                }
                if (atLeastOneRow)
                {
                    row = dtChart1.NewRow();
                    row["Год"] = " ";
                    dtChart1.Rows.Add(row);
                }
            }

            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.Series.Clear();
            for (int i = 1; i < 3; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                series.Label = dtChart1.Columns[i].ColumnName;
                UltraChart1.Series.Add(series);
            }
            
        }

        private void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];
            string prevYear = String.Empty;
            Text prevText = null;
            foreach (Primitive p in e.SceneGraph)
            {
                if (p is Text)
                {
                    Text text = p as Text;
                    if (String.IsNullOrEmpty(text.Path) && (text.GetTextString().EndsWith("План") || text.GetTextString().EndsWith("Факт")))
                    {
                        if (text.GetTextString().EndsWith("План"))
                            text.SetTextString(String.Format("План"));
                        else
                            text.SetTextString(String.Format("Факт"));
                        if (text.bounds.Y > 25)
                            text.bounds.Y -= 15;
                        else
                            text.bounds.Y = 10;
                        if (text.bounds.Y > yAxis.Map(0) - 20)
                            text.bounds.Y = (int)yAxis.Map(0) - 20;
                        text.bounds.Height = 15;
                        text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    }
                    if (text.Path == "Border.Title.Grid.X")
                    {
                        if (text.GetTextString() == "<SERIES_LABEL>")
                            text.SetTextString(String.Empty);
                        text.SetTextString(text.GetTextString().Replace(" - Факт", String.Empty).Replace(" - План", String.Empty));
                        text.labelStyle.HorizontalAlign = StringAlignment.Center;
                        if (prevYear == text.GetTextString())
                        {
                            text.Visible = false;
                            prevText.bounds.X = (int)((prevText.bounds.X + text.bounds.X) / 2);
                        }
                        prevYear = text.GetTextString();
                        prevText = text;
                    }
                }
                if (p is Box)
                {
                    Box box = p as Box;
                    if (box.DataPoint != null && box.Series != null && !String.IsNullOrEmpty(box.Series.Label))
                    {
                        DataRow row = GetExistedRow(dtChart1, 0, box.Series.Label);
                        if (row != null)
                        {
                            string label;
                            if (box.Series.Label.EndsWith("Факт"))
                                label = "Фактический";
                            else
                                label = "Плановый";

                            label += String.Format(" объем инвестиций - {0:N2} тыс.руб.<br/>",
                                MathHelper.Plus(row["Государственная поддержка"], row["Внебюджетные средства"], 0, CalcMode.CalcIfOne));

                            label += "Из них:";
                            if (MathHelper.IsDouble(row["Государственная поддержка"]))
                            {
                                label += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;господдержка - {0:N2} тыс.руб.", row["Государственная поддержка"]);
                            }
                            else
                            {
                                label += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;господдержка - {0:N2} тыс.руб.", 0);
                            }
                            if (MathHelper.IsDouble(row["Внебюджетные средства"]))
                            {
                                label += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;внебюджетные средства - {0:N2} тыс.руб.", row["Внебюджетные средства"]);
                            }
                            else
                            {
                                label += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;внебюджетные средства - {0:N2} тыс.руб.", 0);
                            }

                            box.DataPoint.Label = label;
                        }
                    }
                }
            }
        }

        #endregion

        #region Грид 4

        static object[,] growth;

        private void UltraWebGrid4_DataBinding(object sender, EventArgs e)
        {
            
            LabelGrid4.Text = "Целевые показатели";

            DataTable dtGrid4 = new DataTable();

            dtGrid4.Columns.Add("Показатель", typeof(string));
            dtGrid4.Columns.Add("Единица измерения", typeof(string));
            dtGrid4.Columns.Add("За год перед началом реализации", typeof(double));
            dtGrid4.Columns.Add("Всего за период реализации", typeof(double));
            dtGrid4.Columns.Add("Всего за год", typeof(double));
            dtGrid4.Columns.Add("1 квартал", typeof(double));
            dtGrid4.Columns.Add("2 квартал", typeof(double));
            dtGrid4.Columns.Add("3 квартал", typeof(double));
            dtGrid4.Columns.Add("4 квартал", typeof(double));

            string query = DataProvider.GetQueryText("IP_0001_0002_grid4_list");
            DataTable dtList = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            growth = new object[dtList.Rows.Count, 4];
            for (int j = 0; j < dtList.Rows.Count; ++j)
            {
                DataRow listRow = dtList.Rows[j];
                DataRow row = dtGrid4.NewRow();
                
                row["Показатель"] = listRow["Name"];
                row["Единица измерения"] = listRow["Symbol"];

                year.Value = (beginYear - 1).ToString();
                code.Value = listRow["id"].ToString();
                query = DataProvider.GetQueryText("IP_0001_0002_grid4_prev_year");
                row["За год перед началом реализации"] = db.ExecQuery(query, QueryResultTypes.Scalar) ?? DBNull.Value;
                quarter.Value = "_";
                query = DataProvider.GetQueryText("IP_0001_0002_grid4_quarter_sum").Replace(" and d_ydu.DateYear = " + year.Value, String.Empty);
                row["Всего за период реализации"] = db.ExecQuery(query, QueryResultTypes.Scalar) ?? DBNull.Value;
                year.Value = selectedYear;
                query = DataProvider.GetQueryText("IP_0001_0002_grid4_quarter_sum");
                row["Всего за год"] = db.ExecQuery(query, QueryResultTypes.Scalar) ?? DBNull.Value;
                year.Value = (Convert.ToInt32(selectedYear) - 1).ToString();
                quarter.Value = "4";
                query = DataProvider.GetQueryText("IP_0001_0002_grid4_quarter_sum");
                object prevQuarter = db.ExecQuery(query, QueryResultTypes.Scalar) ?? DBNull.Value;
                year.Value = selectedYear;
                for (int i = 1; i < 5; ++i)
                {
                    quarter.Value = i.ToString();
                    query = DataProvider.GetQueryText("IP_0001_0002_grid4_quarter_sum");
                    row[i.ToString() + " квартал"] = db.ExecQuery(query, QueryResultTypes.Scalar) ?? DBNull.Value;
                    growth[j, i - 1] = MathHelper.Minus(row[i.ToString() + " квартал"], prevQuarter);
                    prevQuarter = row[i.ToString() + " квартал"];
                }

                dtGrid4.Rows.Add(row);
            }

            (sender as UltraWebGrid).DataSource = dtGrid4;

        }

        private void UltraWebGrid4_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;

            UltraGridBand band = grid.Bands[0];
            band.Columns[0].Width = Unit.Parse("300px");
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[1].Width = Unit.Parse("100px");
            for (int i = 2; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse("100px");
                CRHelper.FormatNumberColumn(band.Columns[i], "N2");
            }
            band.HeaderStyle.Wrap = true;
            band.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            GridHeaderLayout layout = new GridHeaderLayout(grid);
            layout.AddCell("Показатель");
            layout.AddCell(" ");
            layout.AddCell("За год перед началом реализации");
            layout.AddCell("Всего за период реализации");
            GridHeaderCell cell = layout.AddCell(selectedYear);
            cell.AddCell("Всего за год");
            cell.AddCell("1 квартал");
            cell.AddCell("2 квартал");
            cell.AddCell("3 квартал");
            cell.AddCell("4 квартал");
            layout.ApplyHeaderInfo();

        }

        private void UltraWebGrid4_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (MathHelper.IsDouble(growth[e.Row.Index, i]))
                    if (MathHelper.SubZero(growth[e.Row.Index, i]))
                        e.Row.Cells[5 + i].Title = String.Format("Снижение на {0:N2} {1} по сравнению с предыдущим периодом",
                            -Convert.ToDouble(growth[e.Row.Index, i]), e.Row.Cells[1]);
                    else if (MathHelper.AboveZero(growth[e.Row.Index, i]))
                        e.Row.Cells[5 + i].Title = String.Format("Повышение на {0:N2} {1} по сравнению с предыдущим периодом",
                            Convert.ToDouble(growth[e.Row.Index, i]), e.Row.Cells[1]);
            }
        }

        #endregion

        private DataRow GetExistedRow(DataTable table, int columnIndex, string pattern)
        {
            foreach (DataRow row in table.Rows)
                if (row[columnIndex].ToString() == pattern)
                    return row;
            return null;
        }

        private DataRow GetRow(DataTable table, int columnIndex, string pattern)
        {
            foreach (DataRow row in table.Rows)
                if (row[columnIndex].ToString() == pattern)
                    return row;
            DataRow newRow = table.NewRow();
            newRow[columnIndex] = pattern;
            table.Rows.Add(newRow);
            return newRow;
        }

        private string GetProjectID(string projectName)
        {
            string result;
            if (!dictProject.TryGetValue(projectName, out result))
                return null;
            else
                return result;
        }

        private string GetProjectName(string projectID)
        {
            string result;
            if (!dictProjectRev.TryGetValue(projectID, out result))
                return null;
            else
                return result;
        }

        private void FillProjectsDictionary(CustomMultiCombo combo)
        {
            string query = DataProvider.GetQueryText("IP_0001_0002_project");
            DataTable dtExec = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dictProject = new Dictionary<string, string>();
            dictProjectRev = new Dictionary<string, string>();
            foreach (DataRow row in dtExec.Rows)
            {
                dict.Add(row["Name"].ToString(), 0);
                dictProject.Add(row["Name"].ToString(), row["ID"].ToString());
                dictProjectRev.Add(row["ID"].ToString(), row["Name"].ToString());
            }
            combo.FillDictionaryValues(dict);
            if (!String.IsNullOrEmpty(projectID.Value))
                combo.SetСheckedState(GetProjectName(projectID.Value), true);
        }

        private void FillYearsDictionary(CustomMultiCombo combo)
        {
            string query = DataProvider.GetQueryText("IP_0001_0002_years");
            DataTable dtExec = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (DataRow row in dtExec.Rows)
            {
                dict.Add(row["DateYear"].ToString(), 0);
            }
            combo.FillDictionaryValues(dict);
            if (String.IsNullOrEmpty(selectedYear))
                combo.SelectLastNode();
            else
                combo.SetСheckedState(selectedYear, true);
        }

        private static IDatabase GetDataBase()
        {
            try
            {
                HttpSessionState sessionState = HttpContext.Current.Session;
                LogicalCallContextData cnt =
                    sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
                if (cnt != null)
                    LogicalCallContextData.SetContext(cnt);
                IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];
                return scheme.SchemeDWH.DB;
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

    }
}
