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
using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;
using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.reports.OMCY_0001
{
    public partial class _default : CustomReportPage
    {
        #region Парамы для квери
        private CustomParam RegionBase { get { return (UserParams.CustomParam("RegionBase")); } }
        private CustomParam Year { get { return (UserParams.CustomParam("Year")); } }

        private CustomParam FieldGrid { get { return (UserParams.CustomParam("FieldGrid")); } }
        private CustomParam FieldChart { get { return (UserParams.CustomParam("FieldChart")); } }
        #endregion

        #region Оляля!
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.SpareMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.SecondaryMASDataProvider.GetCellset(s), dt, "Нет данных");
            return dt;
        }
        #endregion

        #region Ранжирование для грида
        void SetRang(DataTable dt, int ColVal, int ColRang, int StartRow)
        {
            int RowCount = dt.Rows.Count;
            int L_max = StartRow;
            int L_min = StartRow;
            for (int i = StartRow; i < RowCount; i++)
            {
                for (int j = StartRow; j < RowCount; j++)
                {
                    if (dt.Rows[j][ColVal] != System.DBNull.Value)
                    {
                        if (((System.Decimal)(dt.Rows[j][ColVal]) >= (System.Decimal)(dt.Rows[L_max][ColVal])) && (dt.Rows[j][ColRang] == DBNull.Value))
                        {
                            L_max = j;
                        }
                        if (((System.Decimal)(dt.Rows[j][ColVal]) < (System.Decimal)(dt.Rows[L_min][ColVal])) && (dt.Rows[j][ColRang] == DBNull.Value))
                        {
                            L_min = j;
                        }
                    }
                    
                }
                if (!true)
                {
                    dt.Rows[L_max][ColRang] = RowCount - (i - StartRow);
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
        int GetMaxRowFromCol(DataTable dt, int col)
        {
            int MaxIndex = 1;
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (System.DBNull.Value != dt.Rows[i][col])
                if ((System.Decimal)(dt.Rows[i][col]) > (System.Decimal)(dt.Rows[MaxIndex][col]))
                {
                    MaxIndex = i;
                }
            }
            return MaxIndex;
        }

        int GetMinRowFromCol(DataTable dt, int col)
        {
            int MaxIndex = 1;
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (System.DBNull.Value != dt.Rows[i][col])
                    if ((System.Decimal)(dt.Rows[i][col]) < (System.Decimal)(dt.Rows[MaxIndex][col]))
                    {
                        MaxIndex = i;
                    }
            }
            return MaxIndex;
        }

        #endregion

        #region Filter Grid

        void FilterGrid(DataTable dt,int Col,string Contain)
        {
            
            for (int i = 0;i< dt.Rows.Count;i++ )
            {
                if (!dt.Rows[i][Col].ToString().Contains(Contain))
                {
                    dt.Rows[i].Delete();
                    i--;
                }

            }
            
        }

        void ClearEmpty(DataTable dt, int Col)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][Col] == DBNull.Value)
                {
                    dt.Rows[i].Delete();
                    i--;                
                }
            }
        }




        #endregion

        #region SetStar

        protected void SetStar(UltraWebGrid G, int Col, int RowBaseVaslue,string Star,string Hint)
        {
            for (int i = 0; G.Rows.Count>i; i++)
            {
                if (G.Rows[i].Cells[Col].Value == G.Rows[RowBaseVaslue].Cells[Col].Value)
                {
                    G.Rows[i].Cells[Col].Style.BackgroundImage = Star;//"~/images/starYellowBB.png";
                    G.Rows[i].Cells[Col].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    G.Rows[i].Cells[Col].Title = Hint;
                }
            }


        }

        #endregion

        #region SetEnterForChart
        protected void SetEnter(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace(' ', '\n');
            }
        }
        #endregion

        #region GetAVG

        System.Decimal GetAVG(DataTable dt, int Col)
        {
            System.Decimal Sum = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Sum += (System.Decimal)(dt.Rows[i][Col]);
            }
            Sum = Sum/dt.Rows.Count;

            return Sum;
        }

        #endregion

        #region Для линейного множества
        Dictionary<string, int> GenUserParam(string q)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            DataTable dt = GetDSForChart(q);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;
        }
        #endregion

        #region Лоады

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            #region Главное чтобы костюмчик сидел!
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            G.Height = Unit.Empty;

            C.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);

            C1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            C2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            #endregion

            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);            
        }

        protected override void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                #region юзер парамсы
                FOPARAM.FillDictionaryValues(GenUserParam("LD"));
                MRGO.FillDictionaryValues(GenUserParam("MRGO"));
                MRGO.Width = 500;
                FOPARAM.Title = "Год";
                MRGO.Title = "Муниципальное образование";
                #endregion 
            }

            #region Paramsы
            Year.Value = FOPARAM.SelectedValue;
            RegionBase.Value = RegionSettingsHelper.Instance.RegionBaseDimension ;//+ string.Format(".[{0}]", MRGO.SelectedValue);
            FieldGrid.Value = UserMO.Checked?RegionSettingsHelper.Instance.GetPropertyValue("FieldMO"):RegionSettingsHelper.Instance.GetPropertyValue("FieldSO");
            #endregion

            G.DataBind();

            #region Звезды

            SetStar(G, G.Columns.Count - 2, LastMaxIndex, "~/images/starYellowBB.png", "Самый высокий уровень эффективности");
            SetStar(G, G.Columns.Count - 2, LastMinIndex, "~/images/starGrayBB.png", "Самый низкий уровень эффективности");

            SetStar(G, G.Columns.Count - 3, LastLastMinIndex, "~/images/starYellowBB.png","Самая низкая доля неэффективных расходов");
            SetStar(G, G.Columns.Count - 3, LastLastMaxIndex, "~/images/starGrayBB.png", "Самая высокая доля неэффективных расходов");

            #endregion

            #region Вызываем бинд
            C.DataBind();
            C1.DataBind();
            C2.DataBind();
            #endregion

            #region Sneak))
            CTable.Visible = PVDM.Checked;
            C1Table.Visible = !PVDM.Checked;
            #endregion

            #region Хедеры

            Hederglobal.Text = string.Format("Оценка эффективности расходования бюджетных средств  {0} в сфере «Образование»", Year.Value);
            Label2.Text = "Согласно Указу Президента РФ от 28 апреля 2008 г. № 607 и распоряжению Правительства РФ № 1313-р от 11 сентября  2008 г в редакции распоряжения №1246 от 26 июля 2010 года";

            Label4.Text = string.Format("Расчет неэффективных расходов в сфере  «Образование» за {0} год",Year.Value);
            CL.Text = string.Format("Структура неэффективных расходов консолидированного бюджета в сфере «Образование» за {0} год, %", Year.Value);

            Label1.Text = string.Format("Структура видов расходов бюджета в сфере «Образование» за {0} год, %", Year.Value);

            Label3.Text = string.Format("Динамика показателя «Доля неэффективных расходов» за {0} год ", Year.Value);

            Page.Title = Hederglobal.Text;
            #endregion
        }

        #endregion

        #region Grid
        int LastMaxIndex = 0;
        int LastLastMaxIndex = 0;

        int LastMinIndex = 0;
        int LastLastMinIndex = 0;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("G");

            FilterGrid(dt, 0, UserMO.Checked ? "муниципальный" : "Город");

            ClearEmpty(dt, 4);

            //FilterGrid(dt, 4, "");
             
            SetRang(dt, dt.Columns.Count - 2, dt.Columns.Count - 1, 0);
            LastMaxIndex = GetMaxRowFromCol(dt, dt.Columns.Count - 2);
            LastLastMaxIndex = GetMaxRowFromCol(dt, dt.Columns.Count - 3);

            LastMinIndex = GetMinRowFromCol(dt, dt.Columns.Count - 2);
            LastLastMinIndex = GetMinRowFromCol(dt, dt.Columns.Count - 3);

            G.DataSource = dt;  
        }

        
        protected void G_InitializeRow(object sender, RowEventArgs e)
        {

        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            int LastColumn = e.Layout.Bands[0].Columns.Count - 1;

            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth-15) * 0.16);
            e.Layout.Bands[0].Columns[LastColumn].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth - 15) * 0.05);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            
            for (int i = 1; i < LastColumn; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.00");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth - 15) * 0.7 / (LastColumn-1));
            }
            e.Layout.Bands[0].Columns[LastColumn].Header.Caption = "Ранг";
            e.Layout.Bands[0].Columns[LastColumn].CellStyle.HorizontalAlign = HorizontalAlign.Center;
        }
        #endregion

        #region dataBind Chart

        protected void C_DataBinding(object sender, EventArgs e)
        {
            FieldChart.Value = UserMO.Checked ? RegionSettingsHelper.Instance.GetPropertyValue("FieldMOChart") : RegionSettingsHelper.Instance.GetPropertyValue("FieldSOChart");
            
            DataTable dt = GetDSForChart("C");

            FilterGrid(dt, 0, UserMO.Checked ? "муниципальный" : "Город");
            SetEnter(dt);
            C.DataSource = dt;
        }

        protected void C1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("C1");
            FilterGrid(dt, 0, UserMO.Checked ? "муниципальный" : "Город");
            SetEnter(dt);
            C1.DataSource = dt;
        }

        System.Decimal AvgChart3 = 0;


        protected void C2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("C2");
            FilterGrid(dt, 0, UserMO.Checked ? "муниципальный" : "Город");
            AvgChart3 = GetAVG(dt, 1);
            SetEnter(dt);

            dt.Columns[1].ColumnName = String.Format("Значение показателя «Доля неэффективных расходов» за {0} год", dt.Columns[1].Caption);
            C2.DataSource = dt;
        }
        #endregion

        #region Диорамка среднее значение
        protected void C2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            Infragistics.UltraChart.Core.Primitives.Line l =
                new Infragistics.UltraChart.Core.Primitives.Line(new Point((int)xAxis.Map(xAxis.Minimum), (int)yAxis.Map(AvgChart3)), 
                new Point((int)xAxis.Map(xAxis.Maximum), (int)(yAxis.Map(AvgChart3))));


            l.lineStyle.DrawStyle = LineDrawStyle.Dot;
            
            l.PE.Stroke = Color.Green;
            l.PE.StrokeWidth = 10;

            l =
                new Infragistics.UltraChart.Core.Primitives.Line(new Point((int)xAxis.Map(xAxis.Minimum), (int)yAxis.Map(AvgChart3)),
                new Point((int)xAxis.Map(xAxis.Maximum), (int)(yAxis.Map(AvgChart3))));


            l.lineStyle.DrawStyle = LineDrawStyle.Solid;

            l.PE.Stroke = Color.Green;
            l.PE.StrokeWidth = 2;

            e.SceneGraph.Add(l);

            l = new Infragistics.UltraChart.Core.Primitives.Line(new Point(17, 365),
new Point(28, 365));

            l.lineStyle.DrawStyle = LineDrawStyle.Solid;

            l.PE.Stroke = Color.Green;
            l.PE.StrokeWidth = 2;

            e.SceneGraph.Add(l);

            Infragistics.UltraChart.Core.Primitives.Text textLabel;
            textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
            textLabel.PE.Fill = Color.Black;
            textLabel.bounds = new Rectangle(29, 315, 800, 100);
            textLabel.SetTextString(String.Format("Среднее значение показателя «Доля неэффективных расходов» за {0} год",Year.Value));
            e.SceneGraph.Add(textLabel);

        }
        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 1; i < G.Columns.Count; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = e.CurrentWorksheet.Columns[0].Width;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "### ### ##0.##";

            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 2;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        #endregion


    }
}
