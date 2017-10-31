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
using System.Xml;

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
using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.reports.OMCY_0003
{
    public partial class _default : CustomReportPage
    {
        private String dir
        {
            get { return Server.MapPath("~") + "\\"; }
        }

        private CustomParam FieldGrid { get { return (UserParams.CustomParam("FieldGrid")); } }
        private CustomParam Filter { get { return (UserParams.CustomParam("Filter")); } }

        private CustomParam FieldChart { get { return (UserParams.CustomParam("FieldChart")); } }

        private CustomParam BC_R { get { return (UserParams.CustomParam("BC_R")); } }
        private CustomParam BC_C { get { return (UserParams.CustomParam("BC_C")); } }

        private CustomParam Year { get { return (UserParams.CustomParam("year_")); } }
        private CustomParam RegionBaseDimension { get { return (UserParams.CustomParam("RegionBaseDimension")); } }
        private CustomParam RegionBaseDimension_ { get { return (UserParams.CustomParam("RegionBaseDimension_")); } }

        string[] GridColHeder = { "Территория", "Значение,\n рубль", "Абсолютное отклонение от АППГ", "Темп роста\n к АППГ", "Отклонение \n(от ФО)", "Ранг по ФО", "Отклонение\n (от РФ)", "Ранг по РФ" };

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.SpareMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.SpareMASDataProvider.GetCellset(s), dt, "Нет данных");
            return dt;
        }
        //Интегральная оценка органов местного самоуправления

        #region Ранжирование для грида
        void SetRang(DataTable dt, int ColVal, int ColRang, int StartRow)
        {
            int RowCount = dt.Rows.Count;
            int L_max = StartRow;
            int L_min = StartRow;
            int rang = 0;
            for (int i = StartRow; i < RowCount; i++)
            {

                for (int j = StartRow; j < RowCount; j++)
                {
                    if (dt.Rows[j][ColVal] != System.DBNull.Value)
                    {
                        if (((System.Decimal)(dt.Rows[j][ColVal]) <= (System.Decimal)(dt.Rows[L_max][ColVal])) && (dt.Rows[j][ColRang] == DBNull.Value))
                        {
                            L_max = j;
                        }
                        if (((System.Decimal)(dt.Rows[j][ColVal]) > (System.Decimal)(dt.Rows[L_min][ColVal])) && (dt.Rows[j][ColRang] == DBNull.Value))
                        {
                            L_min = j;
                        }
                    }
                    else
                    {
                        //                        minys++;
                    }

                }
                if (true)
                {

                    //for (int j = 0; j < dt.Rows.Count; j++)
                    //{
                    //    if (dt.Rows[L_max][ColVal].ToString() == dt.Rows[j][ColVal].ToString())
                    //    { }
                    //}
                    if (dt.Rows[L_max][ColRang] == DBNull.Value)
                        dt.Rows[L_max][ColRang] = ++rang;//RowCount - (i - StartRow)-minys;


                }
                else
                {
                    dt.Rows[L_max][ColRang] = i - StartRow + 1;
                }


                L_max = L_min;
            }

        }

        #endregion

        #region GetMAX and GetMin
        int GetMaxRowFromCol(UltraWebGrid dt, int col)
        {
            int MaxIndex = 1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    if (System.DBNull.Value != dt.Rows[i].Cells[col].Value)
                        if ((System.Decimal)(dt.Rows[i].Cells[col].Value) > (System.Decimal)(dt.Rows[MaxIndex].Cells[col].Value))
                        {
                            MaxIndex = i;
                        }
                }
                catch { }

            }
            return MaxIndex;
        }

        int GetMinRowFromCol(UltraWebGrid dt, int col)
        {
            int MaxIndex = 1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    if (System.DBNull.Value != dt.Rows[i].Cells[col].Value)
                        if ((System.Decimal)(dt.Rows[i].Cells[col].Value) < (System.Decimal)(dt.Rows[MaxIndex].Cells[col].Value))
                        {
                            MaxIndex = i;
                        }
                }
                catch { }
            }
            return MaxIndex;
        }

        #endregion

        #region sfere
        Dictionary<string, int> FillSfere()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dir + "\\reports\\OMCY_0003\\table.xml");
            XmlNode root = xmlDoc.ChildNodes[1];
            DataTable dt = new DataTable();
            Year.Value = root.ChildNodes[0].ChildNodes[0].InnerText;
            foreach (XmlNode n in root.ChildNodes)
            {
                d.Add(n.ChildNodes[0].InnerText, 0);
                //Year.Value = n.ChildNodes[0].InnerText;
            }
            return d;
        }
        Dictionary<int, RadioButton> GRB;
        static int ra = 1;
        void SetSfereparam()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dir + "\\reports\\OMCY_0003\\table.xml");
            XmlNode root = xmlDoc.ChildNodes[1];
            DataTable dt = new DataTable();
            GRB = new Dictionary<int, RadioButton>();
            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.ChildNodes[0].InnerText == Sfera.SelectedValue)
                {
                    FieldGrid.Value = "";
                    Filter.Value = "";
                    PlaceHolder1.Controls.Clear();
                    Random r = new Random();
                    ra = ra++;
                    for (int i = 1; i < n.ChildNodes.Count; i++)
                    {
                        FieldGrid.Value += string.Format(@",{0},[ОМСУ__Показатели].[ОМСУ__Показатели].[null]", n.ChildNodes[i].InnerText);
                        Filter.Value += string.Format("and(not IsEmpty({0}))", n.ChildNodes[i].InnerText);

                        RadioButton rb = new RadioButton();
                        //font-size: 11pt;
                        //font-family: Verdana
                        rb.Style.Add("font-size", "10pt");

                        rb.ID = "s" + ra.ToString() + "a" + i.ToString();//CRHelper.GetRandomColor().A.ToString() + CRHelper.GetRandomColor().B.ToString();

                        //rb.ID = ra;

                        rb.Style.Add("font-family", "Verdana");
                        GRB.Add(i - 1, rb);
                        PlaceHolder1.Controls.Add(rb);
                        Label l = new Label();
                        l.Text = "<br>";
                        PlaceHolder1.Controls.Add(l);
                        rb.Text = UserComboBox.getLastBlock(n.ChildNodes[i].InnerText);
                        rb.GroupName = "sfere" + ra.ToString();
                        rb.ValidationGroup = rb.GroupName;
                        rb.CheckedChanged += new EventHandler(RadioButton1_CheckedChanged);
                        rb.AutoPostBack = 1 == 1;
                        rb.Checked = 1 == 2;


                    };
                    GRB[0].Checked = 1 == 1;
                    FieldGrid.Value = FieldGrid.Value.Remove(0, 1);
                    Filter.Value = Filter.Value.Remove(0, 3);

                }
            }

            Label6.Visible = (Sfera.SelectedValue == "Обеспечение здоровья");
        }
        #endregion

        #region Filter Grid

        void FilterGrid(DataTable dt, int Col, string Contain)
        {

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!dt.Rows[i][Col].ToString().Contains(Contain))
                {
                    dt.Rows[i].Delete();
                    i--;
                }

            }

        }



        #endregion

        #region SetStar

        protected void SetStar(UltraWebGrid G, int Col, int RowBaseVaslue, string Star, string Title)
        {
            for (int i = 0; G.Rows.Count > i; i++)
            {
                try
                {
                    if (G.Rows[i].Cells[Col].Value.ToString() == G.Rows[RowBaseVaslue].Cells[Col].Value.ToString())
                    {
                        G.Rows[i].Cells[Col + 1].Title = Title;
                        G.Rows[i].Cells[Col + 1].Style.BackgroundImage = Star;//"~/images/starYellowBB.png";
                        G.Rows[i].Cells[Col + 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        //"Самый высокий интегральный показатель"
                    }
                }
                catch { }
            }


        }

        #endregion

        #region Для линейного множества
        Dictionary<string, int> GenUserParam(string q)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            DataTable dt = GetDSForChart(q);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;
        }
        #endregion

        #region Лоады

        private void AddLineAppearencesUltraChart1()
        {
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.ColorModel.Skin.ApplyRowWise = true;
            UltraChart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 22; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }
                    case 8:
                        {
                            color = Color.Yellow;
                            break;
                        }
                    case 9:
                        {
                            color = Color.YellowGreen;
                            break;
                        }
                    case 10:
                        {
                            color = Color.Tomato;
                            break;
                        }
                    case 11:
                        {
                            color = Color.Sienna;
                            break;
                        }
                    case 12:
                        {
                            color = Color.SandyBrown;
                            break;
                        }
                    case 13:
                        {
                            color = Color.Salmon;
                            break;
                        }
                    case 14:
                        {
                            color = Color.RosyBrown;
                            break;
                        }
                    case 15:
                        {
                            color = Color.Purple;
                            break;
                        }
                    case 16:
                        {
                            color = Color.Orchid;
                            break;

                        }
                    case 17:
                        {
                            color = Color.Moccasin;
                            break;
                        }
                    case 18:
                        {
                            color = Color.MediumSeaGreen;
                            break;
                        }
                    case 19:
                        {
                            color = Color.Linen;
                            break;
                        }
                    case 20:
                        {
                            color = Color.LightCoral;
                            break;
                        }
                    case 21:
                        {
                            color = Color.Khaki;
                            break;
                        }
                    case 22:
                        {
                            color = Color.Indigo;
                            break;
                        }

                }
                pe.Fill = color;
                pe.StrokeWidth = 0;
                UltraChart.ColorModel.Skin.PEs.Add(pe);
                pe.Stroke = Color.Black;
                pe.StrokeWidth = 0;
                LineAppearance lineAppearance2 = new LineAppearance();

                lineAppearance2.IconAppearance.Icon = SymbolIcon.Square;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.IconAppearance.PE = pe;

                UltraChart.LineChart.LineAppearances.Add(lineAppearance2);

                UltraChart.LineChart.Thickness = 0;
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            G.Height = Unit.Empty;


            C.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            map.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            map.Height = 700;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;
            //UltraGridExporter1.ExcelExportButton.Visible = 1 == 2;

            UltraChart.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart.Height = 600;



            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart.Border.Thickness = 0;
            // UltraChart.Data.ZeroAligned = true;

            UltraChart.Axis.X.Extent = 60;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.Margin.Near.Value = 3;
            UltraChart.Axis.X.Margin.Far.Value = 2;
            UltraChart.Axis.Y.Margin.Near.Value = 7;
            UltraChart.Axis.Y.Margin.Far.Value = 3;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 25;
            UltraChart.Axis.Y2.Visible = true;
            UltraChart.Axis.Y2.Extent = 65;
            UltraChart.Axis.Y2.LineThickness = 0;
            UltraChart.Axis.Y2.Margin.Near.Value = 7;
            UltraChart.Axis.Y2.Margin.Far.Value = 3;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            //UltraChart.Axis.X.Labels.Font.Size = 10;
            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
            //UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 20;


            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<b> <DATA_VALUE:N2></b>";
            UltraChart.Tooltips.Display = TooltipDisplay.MouseMove;
            UltraChart.ColorModel.ModelStyle = ColorModels.Office2007Style;
            //UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            AddLineAppearencesUltraChart1();
            C.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>:<b><DATA_VALUE:### ### ##0.##></b>";


            #endregion
        }
        void FormatGridRang()
        {

            for (int k = 0; k < G.Columns.Count; k++)
            {
                if (G.Columns[k].Header.Caption == "null")
                {
                    for (int i = 0; i < G.Rows.Count; i++)
                    {
                        int max_r = i;
                        int min_r = i;
                        for (int j = 0; j < G.Rows.Count; j++)
                        {
                            try
                            {
                                if (G.Rows[i].Cells[k - 1].Value.ToString() == G.Rows[j].Cells[k - 1].Value.ToString())
                                {

                                    if (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) > System.Decimal.Parse(G.Rows[max_r].Cells[k].Value.ToString()))
                                    {
                                        max_r = j;
                                    }
                                    if (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) < System.Decimal.Parse(G.Rows[min_r].Cells[k].Value.ToString()))
                                    {
                                        min_r = j;
                                    }

                                }
                            }
                            catch { }
                        }
                        try
                        {
                            if (min_r != max_r)
                            {
                                string s = G.Rows[min_r].Cells[k].Value.ToString() + " - " + G.Rows[max_r].Cells[k].Value.ToString();
                                System.Decimal max_r_ = System.Decimal.Parse(G.Rows[max_r].Cells[k].Value.ToString());
                                System.Decimal min_r_ = System.Decimal.Parse(G.Rows[min_r].Cells[k].Value.ToString());
                                for (int j = 0; j < G.Rows.Count; j++)
                                {
                                    try
                                    {
                                        if ((System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) <= max_r_) &&
                                            (System.Decimal.Parse(G.Rows[j].Cells[k].Value.ToString()) >= min_r_))
                                        {
                                            G.Rows[j].Cells[k].Text = s;
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }


                    }
                }
            }
            for (int i = 0; i < G.Rows.Count; i++)
            {
                for (int j = 0; j < G.Columns.Count; j++)
                {
                    G.Rows[i].Cells[j].Value = G.Rows[i].Cells[j].Value == null ? "-" : G.Rows[i].Cells[j].Value;
                }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {

            RegionBaseDimension.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                UserMO.FillDictionaryValues(GenUserParam("LD"));
                UserMO.Title = "Год";
                UserMO.Width = 150;
                C.Data.SwapRowsAndColumns = !C.Data.SwapRowsAndColumns;
                Sfera.FillDictionaryValues(FillSfere());
                Sfera.SetСheckedState(Year.Value, 1 == 1);
                Sfera.Title = "Сфера";
                BC_C.Value = "columns";
                BC_R.Value = "rows";

            }
            Year.Value = UserMO.SelectedValue;
            SetSfereparam();

            if (!Page.IsPostBack)
            {
            }
            RegionBaseDimension.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionComparableDimentionDownLevel");
            RadioButton1_CheckedChanged(GRB[0], null);



            Year.Value = UserMO.SelectedValue;
            G.DataBind();
            FormatGridRang();
            for (int i = dtGrid.Columns.Count - 2; i > 0; i -= 2)
            {
                LastMaxIndex = GetMaxRowFromCol(G, i);
                LastMinIndex = GetMinRowFromCol(G, i);
                SetStar(G, i, LastMinIndex, "~/images/starYellowBB.png", "Наименьшее отклонение");
                SetStar(G, i, LastMaxIndex, "~/images/starGrayBB.png", "Наибольшее отклонение");
            }

            C.DataBind();
            RegionBaseDimension.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionComparableDimentionDownLevel");
            SetMapSettings();
            UltraChart.DataBind();
            SetHeder();

        }

        #endregion

        string GetFirstWord()
        {
            string s1 = UserComboBox.getLastBlock(FieldChart.Value).Split(' ')[0];
            string s2 = UserComboBox.getLastBlock(FieldChart.Value).Split(' ')[1];
            return s1 + " " + s2;
        }

        void SetHeder()
        {
            #region хедеры
            Hederglobal.Text = string.Format("Оценка эффективности деятельности органов местного самоуправления ({0})", RegionSettingsHelper.Instance.Name);

            Page.Title = Hederglobal.Text;
            Label2.Text = "Оценка проводится на основании распоряжения Правительства Ханты-Мансийского автономного округа - Югры от 1 апреля 2009 года № 118-рп «Об оценке эффективности деятельности органов местного самоуправления городских округов и муниципальных районов Ханты Мансийского автономного округа - Югры» в редакции Постановления Губернатора автономного округа от 26.02.2010 № 46";
            if (Sfera.SelectedValue == "Оценка эффективности деятельности ОМСУ")
            {
                Label3.Text = string.Format("Оценка эффективности деятельности ОМСУ за {0} год", Year.Value);
            }
            else
            {
                Label3.Text = string.Format("Расчет уровня эффективности деятельности органов местного самоуправления в сфере «{0}» за {1} год", Sfera.SelectedValue, UserMO.SelectedValue);
            }
            Label4.Text = string.Format("Распределение территорий по значению показателя «{0}» <!--в сфере {1}--> в {2} году", UserComboBox.getLastBlock(FieldChart.Value), Sfera.SelectedValue, UserMO.SelectedValue);

            Label1.Text = string.Format("{0}<!--в сфере {1}--> за {2} год", UserComboBox.getLastBlock(FieldChart.Value), Sfera.SelectedValue, UserMO.SelectedValue);
            #endregion
        }

        #region ДатаБинда грида

        int LastMaxIndex = 0;
        int LastMinIndex = 0;
        DataTable dtGrid = null;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("G");
            dtGrid = dt;
            for (int i = dt.Columns.Count - 1; i > 1; i -= 2)
            {
                dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.Replace("\"", "&quot;");
                dt.Columns[i].Caption = dt.Columns[i].Caption.Replace("\"", "&quot;");
                dt.Columns[i - 1].ColumnName = dt.Columns[i - 1].ColumnName.Replace("\"", "&quot;");
                dt.Columns[i - 1].Caption = dt.Columns[i - 1].Caption.Replace("\"", "&quot;");
                SetRang(dt, i - 1, i, 0);
            }
            ((UltraWebGrid)sender).DataSource = dt;
        }

        protected void G_InitializeLayout1(object sender, LayoutEventArgs e)
        {
            int LastColumn = e.Layout.Bands[0].Columns.Count - 1;
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;

            for (int i = 1; i < LastColumn + 1; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.00");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth - 15) * 0.7 / (LastColumn));
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[LastColumn - 1], "### ### ##0.00");
            e.Layout.Bands[0].Columns[LastColumn].Width = 54;
            e.Layout.Bands[0].Columns[0].Header.Caption = "";
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth - 15) * 0.20);
            if ((UltraWebGrid)(sender) == G)
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = "Территория";
            }
            else
            {

            }
            e.Layout.AllowSortingDefault = AllowSorting.Yes;
            e.Layout.SortCaseSensitiveDefault = Infragistics.WebUI.Shared.DefaultableBoolean.True;
            e.Layout.SortingAlgorithmDefault = SortingAlgorithm.BubbleSort;
            //e.Layout.a
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                if (e.Layout.Bands[0].Columns[i].Header.Caption == "null")
                {
                    e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder("коэффициент", i - 1, 1, 1));
                    e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder("место", i, 1, 1));
                    e.Layout.Bands[0].Columns[i - 1].Header.RowLayoutColumnInfo.SpanX = 2;
                    e.Layout.Bands[0].Columns[i].Width = (40 + 44 + 90 + 5) / 2;
                    e.Layout.Bands[0].Columns[i - 1].Width = (40 + 44 + 90 + 5) / 2;
                }
            }
        }

        ColumnHeader GenColumnHeder(string Caption, int x, int y, int spanX)
        {
            ColumnHeader CH = new ColumnHeader(1 == 1);
            CH.RowLayoutColumnInfo.OriginX = x;
            CH.RowLayoutColumnInfo.OriginY = y;
            CH.RowLayoutColumnInfo.SpanX = spanX;
            CH.RowLayoutColumnInfo.SpanY = 1;
            CH.Caption = Caption;
            CH.Style.Wrap = 1 == 1;
            CH.Style.HorizontalAlign = HorizontalAlign.Center;
            CH.Style.BackColor = Color.Transparent;


            return CH;
        }
        #endregion

        #region mapa
        bool Reverce = 1 == 1;
        public void SetShape(string nameFO, string nameMini, System.Decimal val, System.String rang)
        {

            Shape sh = FindMapShape(nameMini, nameFO);
            if (sh == null) return;

            sh["IndicatorValue"] = val;




            sh.Text = string.Format("{0} \n {1:### ##0.00} ({2})", nameFO, val, rang);

            sh.Name = sh.Text;
            sh["NAME"] = sh.Text;
            sh.Visible = 1 == 1;
            sh.TextVisibility = TextVisibility.Shown;
            sh.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;

            sh.ToolTip = string.Format("{0}\n{1}:{2:### ### ##0.00}\nместо: {3}", nameFO, GetFirstWord(), val, rang);
        }
        protected void LoadMap()
        {
            string RegionName = "ХМАО";

            string DirectoryName = RegionName; //ShorteningFO(RegionName);

            string FileName = RegionName;//ShorteningFO(RegionName);

            string MapPath = Server.MapPath(string.Format("../../maps/Субъекты/{0}/Выноски.shp", DirectoryName));
            map.LoadFromShapeFile(MapPath, "NAME", true);
            MapPath = Server.MapPath(string.Format("../../maps/Субъекты/{0}/Города.shp", DirectoryName));
            map.LoadFromShapeFile(MapPath, "NAME", true);

            MapPath = Server.MapPath(string.Format("../../maps/Субъекты/{0}/Территор.shp", DirectoryName));
            map.LoadFromShapeFile(MapPath, "NAME", true);
        }


        void SetMapSettings()
        {

            #region Настройка карты
            map.Meridians.Visible = false;
            map.Parallels.Visible = false;
            //map.ZoomPanel.Visible = 1 == 1;
            //map.ZoomPanel.Dock = PanelDockStyle.Right;
            //map.NavigationPanel.Visible = 1 == 1;
            //map.NavigationPanel.Dock = PanelDockStyle.Right;
            map.Viewport.EnablePanning = true;



            // добавляем поля для раскраски
            map.ShapeFields.Clear();
            map.ShapeFields.Add("Name");
            map.ShapeFields["Name"].Type = typeof(string);
            map.ShapeFields["Name"].UniqueIdentifier = true;
            map.ShapeFields.Add("IndicatorValue");
            map.ShapeFields["IndicatorValue"].Type = typeof(decimal);
            map.ShapeFields["IndicatorValue"].UniqueIdentifier = false;

            #endregion

            // добавляем легенду
            map.Legends.Clear();
            // добавляем легенду раскраски
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Title = GetFirstWord();//"Уровень эффективности";
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Left;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
            legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            legend1.AutoFitMinFontSize = 7;
            map.Legends.Add(legend1);

            // добавляем правила раскраски
            map.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "IndicatorValueRule";
            rule.Category = String.Empty;
            rule.ShapeField = "IndicatorValue";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 7;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Reverce ? Color.Green : Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = !Reverce ? Color.Green : Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";

            rule.LegendText = "#FROMVALUE{N1} - #TOVALUE{N1}";//"LegendText";
            map.ShapeRules.Add(rule);

    //        string layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp",
    //            RegionSettingsHelper.Instance.GetRegionSetting("ShortName"),
    //"Города"));

    //        map.LoadFromShapeFile(layerName, "NAME", true);

    //        layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", RegionSettingsHelper.Instance.GetRegionSetting("ShortName"),
    //            "Выноски"));

    //        map.LoadFromShapeFile(layerName, "NAME", true);

    //        layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", RegionSettingsHelper.Instance.GetRegionSetting("ShortName"),
    //"Территор"));
            LoadMap();
            //map.LoadFromShapeFile(layerName, "NAME", true);

            DataTable dt = GetDSForChart("mapa");
            if (dt.Columns.Count < 4)
            {
                return;
            }
            delEmptyRow(dt, 3);
            SetRang(dt, 3, 4, 0);
            DataTable dt_ = new DataTable();
            for (int i = 0; i < dt.Columns.Count - 1; i++)
            {
                dt_.Columns.Add(dt.Columns[i].Caption, dt.Columns[i].DataType);
            }
            dt_.Columns.Add("rang");
            object[] o = new object[dt.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count-1; j++)
                {
                    dt_.Rows.Add(o);
                    dt_.Rows[i][j] = dt.Rows[i][j];
                }
                dt_.Rows[i][dt_.Columns.Count - 1] = dt.Rows[i][dt_.Columns.Count - 1].ToString();

            }
            #region rang
            //for (int k = 0; k < G.Columns.Count; k++)
            {
                //if (G.Columns[k].Header.Caption == "null")
                {
                    int k = 4;
                    for (int i = 0; i < dt_.Rows.Count; i++)
                    {
                        int max_r = i;
                        int min_r = i;
                        for (int j = 0; j < dt_.Rows.Count; j++)
                        {
                            try
                            {
                                if (dt_.Rows[i][k - 1].ToString() == dt_.Rows[j][k - 1].ToString())
                                {

                                    if (System.Decimal.Parse(dt_.Rows[j][k].ToString()) > System.Decimal.Parse(dt_.Rows[max_r][k].ToString()))
                                    {
                                        max_r = j;
                                    }
                                    if (System.Decimal.Parse(dt_.Rows[j][k].ToString()) < System.Decimal.Parse(dt_.Rows[min_r][k].ToString()))
                                    {
                                        min_r = j;
                                    }

                                }
                            }
                            catch { }
                        }
                        try
                        {
                            if (min_r != max_r)
                            {
                                string s = dt_.Rows[min_r][k].ToString() + " - " + dt_.Rows[max_r][k].ToString();
                                System.Decimal max_r_ = System.Decimal.Parse(dt_.Rows[max_r][k].ToString());
                                System.Decimal min_r_ = System.Decimal.Parse(dt_.Rows[min_r][k].ToString());
                                for (int j = 0; j < dt_.Rows.Count; j++)
                                {
                                    try
                                    {
                                        if ((System.Decimal.Parse(dt_.Rows[j][k].ToString()) <= max_r_) &&
                                            (System.Decimal.Parse(dt_.Rows[j][k].ToString()) >= min_r_))
                                        {
                                            dt_.Rows[j][k] = s;
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }


                    }
                }
            }
            for (int i = 0; i < G.Rows.Count; i++)
            {
                for (int j = 0; j < G.Columns.Count; j++)
                {
                    G.Rows[i].Cells[j].Value = G.Rows[i].Cells[j].Value == null ? "-" : G.Rows[i].Cells[j].Value;
                }
            }
            #endregion

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][3] != DBNull.Value)
                    SetShape(dt.Rows[i][2].ToString(), dt.Rows[i][1].ToString(), (System.Decimal.Parse(dt.Rows[i][3].ToString())), (dt_.Rows[i][4].ToString()));
            }



        }
        void delEmptyRow(DataTable dt, int col)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][col] == DBNull.Value)
                {
                    dt.Rows[i].Delete();
                    i--;
                }
            }
        }

        string CaseRegion(string _)
        {
            switch (_)
            {
                //Если бы каталог назывался не УрФо то можно было бы нормаьлно реализвать
                case "Центральный федеральный округ": { return "ЦФО"; }
                case "Северо-Западный федеральный округ": { return "СФО"; }
                case "Южный федеральный округ": { return "ЮФО"; }
                case "Северо-Кавказский федеральный округ": { return "СКФО"; }
                case "Приволжский федеральный округ": { return "ПФО"; }
                case "Уральский федеральный округ": { return "УрФО"; }
                case "Сибирский федеральный округ": { return "СФО"; }
                case "Дальневосточный федеральный округ": { return "ДФО"; }
            }
            return _;


        }
        string ezm = "zz";
        public Shape FindMapShape(string nameFO, string name)
        {
            if (name == "")
            {
                return null;
            };
            for (int i = 0; i < map.Shapes.Count; i++)
                try
                {
                    string s = "";

                    s = map.Shapes[i]["NAME"].ToString();
                    s = s.Remove(0, 2);
                    {
                        if (s.Split('_').Length > 1)
                        {
                            s = s.Split('_')[0];

                            string _name = name.Remove(0, 2);

                            if ((_name.Contains(s) || s.Contains(_name)) || (_name == s))
                            {
                                return map.Shapes[i];
                            }
                        }
                    }
                }
                catch { }

            if (name[0] == 'г')
            {
                return null;
            }
            for (int i = 0; i < map.Shapes.Count; i++)
            {
                string s = "";
                try
                {


                    try
                    {

                        map.Shapes[i].Text = map.Shapes[i]["NAME"].ToString();
                        s = map.Shapes[i]["CODE"].ToString();

                        if (nameFO == s)
                        {
                            return map.Shapes[i];
                        }
                    }
                    catch { }
                    try
                    {
                        s = map.Shapes[i]["NAME"].ToString().Split('_')[0];
                        if (name.Contains(s) || s.Contains(name))
                        {
                            return map.Shapes[i];
                        }
                    }
                    catch { }
                }
                catch { }


            }

            return null;
        }
        #endregion

        #region Расчет медианы

        bool Even(int input)
        {
            if (input % 2 == 0)
            {
                return true;
            }
            return false;
        }

        int MedianIndex(int length)
        {
            if (length == 0)
            {
                return 0;
            }

            if (Even(length))
            {
                return length / 2 - 1;
            }
            else
            {
                return (length + 1) / 2 - 1;
            }
        }

        double MedianValue(DataTable dt, int medianValueColumn)
        {
            if (dt.Rows.Count == 0)
            {
                return 0;
            }

            if (Even(dt.Rows.Count))
            {
                double value1;
                double value2;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value1);
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count) + 1][medianValueColumn].ToString(),
                        out value2);
                return (value1 + value2) / 2;
            }
            else
            {
                double value;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value);
                return value;
            }
        }

        #endregion

        #region стройка диограм
        #region Обработчики диаграммы
        protected void db()
        {
            string unit;
            string query = DataProvider.GetQueryText("C");
            DataTable dtChart = new DataTable();
            DataTable medianDT = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart);

            //C.DataSource = dtChart;
            //return;

            for (int i = 0; i < dtChart.Columns.Count; i++)
            {
                dtChart.Columns[i].ColumnName = dtChart.Columns[i].ColumnName.Replace("\"", "&quot;");
            }
            double minValue = Double.PositiveInfinity;
            double maxValue = Double.NegativeInfinity;
            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("ДАННЫЕ", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Trim();
                    row[0] = row[0].ToString().Replace(" муниципальный район", " р-н");
                    row[0] = row[0].ToString().Replace("Город ", "Г. ");
                }
            }
            if (dtChart.Rows.Count > 1)
            {
                double avgValue = 0;
                for (int i = 0; i < dtChart.Rows.Count; ++i)
                {
                    double value = Convert.ToDouble(dtChart.Rows[i][1]);
                    avgValue += value;
                    minValue = value < minValue ? value : minValue;
                    maxValue = value > maxValue ? value : maxValue;
                }
                avgValue /= dtChart.Rows.Count;
                // рассчитываем медиану
                int medianIndex = MedianIndex(dtChart.Rows.Count);
                medianDT = dtChart.Clone();
                double medianValue = MedianValue(dtChart, 1);
                for (int i = 0; i < dtChart.Rows.Count - 1; i++)
                {
                    medianDT.ImportRow(dtChart.Rows[i]);
                    double value;
                    Double.TryParse(dtChart.Rows[i][1].ToString(), out value);
                    double nextValue;   
                    Double.TryParse(dtChart.Rows[i + 1][1].ToString(), out nextValue);
                    if (((value <= avgValue) && (nextValue > avgValue)) && (i == medianIndex))
                    {
                        if (medianValue > avgValue)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dtChart, 1);
                        }
                        else
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dtChart, 1);
                            row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;

                        }
                    }
                    else
                    {
                        if ((value <= avgValue) && (nextValue > avgValue))
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                        }

                        if (i == medianIndex)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dtChart, 1);
                        }
                    }
                }
                medianDT.ImportRow(dtChart.Rows[dtChart.Rows.Count - 1]);

                if (!Double.IsPositiveInfinity(minValue) && !Double.IsNegativeInfinity(maxValue))
                {
                    C.Axis.Y.RangeType = AxisRangeType.Custom;
                    C.Axis.Y.RangeMax = maxValue * 1.1;
                    C.Axis.Y.RangeMin = minValue / 1.1;
                }
            }
            C.DataSource =(medianDT == null) ? null : medianDT.DefaultView;

            
        }
        #endregion


        protected void C_DataBinding(object sender, EventArgs e)
        {
            //DataTable dt = GetDSForChart("C");
            db();
            C.Height = 400;
            C.Tooltips.FormatString = "<SERIES_LABEL>\n" + GetFirstWord() + ":<b><DATA_VALUE:### ### ##0.00></b>";

        }

        protected void C2_DataBinding(object sender, EventArgs e)
        {

        }
        protected void C_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Series != null && (box.Series.Label == "Среднее" || box.Series.Label == "Медиана"))
                        {
                            box.PE.Fill = Color.Orange;
                            box.PE.FillStopColor = Color.OrangeRed;
                        }
                    }
                }
            }
        }
        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            try
            {
                e.CurrentWorksheet.Rows[2].Height = 100 * 10;
                e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
                e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[1].Cells[0].Value = Label3.Text;//Hederglobal.Text;
                for (int i = 2; i < G.Columns.Count; i += 2)
                {
                    e.CurrentWorksheet.Rows[2].Cells[i].Value = "Ранг";
                    e.CurrentWorksheet.Columns[i - 1].Width = e.CurrentWorksheet.Columns[i - 1].Width / 3;

                    e.CurrentWorksheet.Columns[i - 1].CellFormat.FormatString = "### ### ##0.00";
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "### ### ##0.00";

                }
                e.CurrentWorksheet.Rows[2].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                for (int j = 0; e.Workbook.Worksheets.Count > j; j++)
                {
                    for (int i = 1; i < 15; i++)
                    {
                        e.Workbook.Worksheets[j].Columns[i].Width = e.Workbook.Worksheets[1].Columns[0].Width;
                        e.Workbook.Worksheets[j].Columns[i].CellFormat.FormatString = "### ### ##0.00";
                    }

                }

            }
            catch { }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            //..Worksheet sheet2 = workbook.Worksheets.Add("Муниципальные районы");
            for (int i = 0; i < G.Columns.Count; i++)
            {
                G.Columns[i].Header.Caption = G.Columns[i].Header.Caption.Replace("&quot;", "\"");
            }
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 3;
            UltraGridExporter1.ExcelExporter.Export(G, sheet1);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        #endregion

        #region Обработчики диаграммы
        DataTable dtChart;
        bool ShowTop = 1 == 1;
        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("C2");
            dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Columns[2].DataType.Name != "String")
            {
                dtChart.Columns.Remove(dtChart.Columns[0]);
                ShowTop = 1 == 1;
                UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
                UltraChart.Axis.X.Extent = 60;
                for (int i = 0; i < dtChart.Rows.Count; i++)
                {
                    for (int j = 0; j < dtChart.Columns.Count; j++)
                    {
                        if (dtChart.Rows[i][j] == DBNull.Value)
                        {
                            // dtChart.Rows[i].Delete();
                        }
                    }

                }

            }
            else
            {
                UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
                UltraChart.Axis.X.Extent = 160;
                ShowTop = 1 == 2;
                DataTable dt = new DataTable();
                dt.Columns.Add("aa");
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    dt.Columns.Add(dtChart.Rows[0][i].ToString(), typeof(System.Decimal));
                }
                object[] o = new object[dt.Columns.Count];
                for (int i = 1; i < dtChart.Rows.Count; i++)
                {
                    dt.Rows.Add(dtChart.Rows[i].ItemArray);
                }
                dtChart = dt;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][0] = SetCavuch(dt.Rows[i][0].ToString());
                }
            }

            UltraChart.DataSource = dtChart;
        }
        string SetCavuch(string s)
        {
            bool b = 2 == 1;
            string ns = "";
            for (int i = 0; i < s.Length; i++)
            {

                char b_ = s[i];
                if (b_ == '"')
                {
                    b = !b;
                    b_ = b ? '«' : '»';

                }

                ns += b_;

            }
            return ns;
        }


        private int GetMaxRowIndex(string col)
        {
            int result = 0;
            double value = 0;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][col] != DBNull.Value)
                    if (value < Convert.ToDouble(dtChart.Rows[i][col]))
                    {
                        value = Convert.ToDouble(dtChart.Rows[i][col]);
                        result = i;
                    }
            }
            return result;
        }

        private int GetMinRowIndex(string col)
        {
            int result = 0;
            double value = 10;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][col] != DBNull.Value)
                    if (value > Convert.ToDouble(dtChart.Rows[i][col]))
                    {
                        value = Convert.ToDouble(dtChart.Rows[i][col]);
                        result = i;
                    }
            }
            return result;
        }

        private string GetRegion(int row)
        {
            return dtChart.Rows[row][0].ToString();
        }

        private double GetValue(int row, string col)
        {
            return Convert.ToDouble(dtChart.Rows[row][col].ToString());
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {   
                for (int j = i + 1; j < e.SceneGraph.Count; j++)
                {
                    Infragistics.UltraChart.Core.Primitives.PointSet p;

                    if (e.SceneGraph[i] is Box)
                    {
                        Box b = (Box)e.SceneGraph[i];
                        if (b.rect.Height == b.rect.Width)
                        {
                            if (e.SceneGraph[j] is Box)
                            {
                                Box b2 = (Box)e.SceneGraph[j];
                                if (b2.rect.Height == b2.rect.Width)
                                {
                                    if ((b.rect.X == b2.rect.X) & (Math.Abs(b.rect.Y - b2.rect.Y) <= 4))
                                    {
                                        b2.rect.X += b2.rect.Width + 1;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            if (!ShowTop)
            {
                return;
            }
            //
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            Collection<string> indicators = new Collection<string>();
            Collection<string> maxRegions = new Collection<string>();
            Collection<double> maxValue = new Collection<double>();
            Collection<string> minRegions = new Collection<string>();
            Collection<double> minValue = new Collection<double>();
            Collection<int> leftBound = new Collection<int>();
            Collection<int> leftWidth = new Collection<int>();

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {

                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null &&
                    primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;

                    int maxRowIndex = GetMaxRowIndex(text.GetTextString());
                    int minRowIndex = GetMinRowIndex(text.GetTextString());

                    indicators.Add(text.GetTextString());
                    maxRegions.Add(GetRegion(maxRowIndex));
                    maxValue.Add(GetValue(maxRowIndex, text.GetTextString()));

                    minRegions.Add(GetRegion(minRowIndex));
                    minValue.Add(GetValue(minRowIndex, text.GetTextString()));

                    leftBound.Add(text.bounds.X);
                    leftWidth.Add(text.bounds.Width);
                }
            }

            for (int i = 0; i < indicators.Count; i++)
            {
                Text newMaxText = new Text();
                newMaxText.labelStyle.Font = new Font("Verdana", 8);

                newMaxText.PE.Fill = Color.Black;
                newMaxText.bounds = new Rectangle(leftBound[i], (int)yAxis.Map(maxValue[i]) - 20, leftWidth[i], 15);
                newMaxText.labelStyle.VerticalAlign = StringAlignment.Center;
                newMaxText.labelStyle.HorizontalAlign = StringAlignment.Center;
                newMaxText.SetTextString(RegionsNamingHelper.ShortName(maxRegions[i]));
                e.SceneGraph.Add(newMaxText);


                Text newMinText = new Text();
                newMinText.labelStyle.Font = new Font("Verdana", 8);
                newMinText.PE.Fill = Color.Black;
                newMinText.bounds = new Rectangle(leftBound[i], (int)yAxis.Map(minValue[i]) + 10, leftWidth[i], 15);
                newMinText.labelStyle.VerticalAlign = StringAlignment.Center;
                newMinText.labelStyle.HorizontalAlign = StringAlignment.Center;
                newMinText.SetTextString(RegionsNamingHelper.ShortName(minRegions[i]));
                e.SceneGraph.Add(newMinText);
            }
            #region
            //string textValue = GetRegion(GetMaxRowIndex(text.GetTextString()));




            //    if (primitive is Box)
            //    {
            //        Box box = (Box)primitive;
            //        if (box.DataPoint != null && box.Value != null)
            //        {
            //            string otherMeasureText = string.Empty;
            //            double otherValue = 0;
            //            if (dtChart != null && dtChart.Rows[box.Row][2] != DBNull.Value && 
            //                dtChart.Rows[box.Row][2].ToString() != string.Empty)
            //            {
            //                otherValue = Convert.ToDouble(dtChart.Rows[box.Row][2]);

            //            }

            //            double value = Convert.ToDouble(box.Value);
            //            if (value > 0)
            //            {

            //                box.PE.ElementType = PaintElementType.Gradient;
            //                box.PE.FillGradientStyle = GradientStyle.Horizontal;
            //                box.PE.Fill = Color.Green;
            //                box.PE.FillStopColor = Color.ForestGreen;
            //            }
            //            else
            //            {

            //                box.PE.ElementType = PaintElementType.Gradient;
            //                box.PE.FillGradientStyle = GradientStyle.Horizontal;
            //                box.PE.Fill = Color.Red;
            //                box.PE.FillStopColor = Color.Maroon;
            //            }
            //        }
            //        else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
            //        {
            //            box.PE.ElementType = PaintElementType.CustomBrush;
            //            LinearGradientBrush brush = new LinearGradientBrush(box.rect, Color.Green, Color.Red, 45, false);
            //            box.PE.CustomBrush = brush;
            //        }
            //    }
            //}
            #endregion
        }

        #endregion

        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)(sender);
            //rb.Checked = 1 == 1;
            FieldChart.Value = String.Format("[ОМСУ__Показатели].[ОМСУ__Показатели].[Всего].[Уровни].[{0}]", rb.Text);
            C.DataBind();
            SetMapSettings();
            SetHeder();

        }

        protected void C_DataBinding1(object sender, EventArgs e)
        {

        }

        protected void RadioButton1_CheckedChanged1(object sender, EventArgs e)
        {
            //string buf = BC_R.Value;
            if ((RadioButton)(sender) == RadioButton1)
            {
                BC_C.Value = "columns";
                BC_R.Value = "rows";

            }
            else
            {
                BC_C.Value = "rows";
                BC_R.Value = "columns";

            }
            //C.DataBind();

            UltraChart.DataBind();

        }

        protected void G_InitializeRow1(object sender, RowEventArgs e)
        {

        }

        protected void G_SortColumn(object sender, SortColumnEventArgs e)
        {
            //e.
        }


    }
}
