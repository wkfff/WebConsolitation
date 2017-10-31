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
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using System.Drawing;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebGrid;
using Microsoft.AnalysisServices.AdomdClient;


namespace Krista.FM.Server.Dashboards.FO_BOR_0001_005_0001.Default
{
    public partial class _default : CustomReportPage
    {


        private CustomParam LD { get { return (UserParams.CustomParam("LD")); } }
        private CustomParam GRBS { get { return (UserParams.CustomParam("GRBS")); } }

        System.Collections.Generic.Dictionary<string, int> GenUserParam(string query)
        {
            DataTable dt = GetDS(query);
            System.Collections.Generic.Dictionary<string, int> d = new System.Collections.Generic.Dictionary<string, int>();
            foreach (DataColumn Column in dt.Columns)
            {
                if ((Column.Caption != "LD_") && (Column.Caption != "Fo_BOR"))
                {
                    LD.Value = Column.Caption;
                    d.Add(LD.Value, 0);
                }
            }
            return d;
        }

        protected DataTable GetDS(string q)
        {
            DataTable dt = new DataTable(q);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(q), q, dt);
            return dt;

        }


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            G.Width = CustomReportConst.minScreenWidth - 10;
            G.Height = Unit.Empty;

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
                if (!Page.IsPostBack)
                {
                    ComboYear.FillDictionaryValues(GenUserParam("LD_"));
                    ComboYear.SetСheckedState(LD.Value, 1 == 1);
                    ComboYear.Title = "Период";
                    ComboYear.Width = 140;

                    comboFo.FillDictionaryValues(GenUserParam("Fo_BOR"));
                    comboFo.Title = "ГРБС";
                    comboFo.Width = 1000;
                } 
                string PrevReport = CustomParam.CustomParamFactory("CRepoort").Value;

                //Удалаяяем этот папраметр, шоб в следущий ра
                Session.Remove("CRepoort");
                //если пусто значит не с какого

                if (!string.IsNullOrEmpty(PrevReport))
                {
                    comboFo.SetСheckedState(PrevReport.Split(':')[0].Replace('_', ' '), 1 == 1);
                    ComboYear.SetСheckedState(PrevReport.Split(':')[1], 1 == 1);
                }

                LD.Value = ComboYear.SelectedValue;
                GRBS.Value = comboFo.SelectedValue;

                System.Decimal val = (System.Decimal)(GetDS("XZ1").Rows[0][0]);
                GenGridChekedData();


            }
            catch
            {
                G.Bands.Clear();
            }
            Label6.Text = string.Format("Результативность решения тактических задач в {0} году ({1})", LD.Value, comboFo.SelectedValue);
            Page.Title = Label6.Text;
        }

        #region Стройка грида
        void GenGridChekedData()
        {
            DataTable dt = GetDS("G");
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("G"));

            DataTable dtres = new DataTable();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dtres.Columns.Add(dt.Columns[i].ToString(), dt.Columns[i].DataType);
            }

            int CountUpLevel = 0;
            int CountCenterLevel = 0;
            int CountDownLevel = 0;
            int lastRowUpLevel = 0;
            int lastRowCenterLevel = -1;
            System.Decimal SumPercentUp = 0;
            System.Decimal SumPercentCenter = 0;
            System.Decimal LastMassCenter = 0;
            dt.Rows[0].Delete();

            //dtres.Rows.Add("Цель;Результативность достижения цели", null, null, null, null);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] xz = { "].[" };

                if (cs.Axes[1].Positions[i + 1].Members[0].UniqueName.Split(xz, StringSplitOptions.None).Length == 5)
                {

                    if (i != 0)
                    {
                        dtres.Rows[lastRowCenterLevel][4] = SumPercentCenter;

                        SumPercentUp += LastMassCenter * SumPercentCenter;
                        dtres.Rows[lastRowUpLevel][4] = SumPercentUp;

                    }
                    CountUpLevel++;
                    CountDownLevel = 0;
                    CountCenterLevel = 0;
                    SumPercentCenter = 0;
                    lastRowCenterLevel = -1;

                    SumPercentUp = 0;
                    dtres.Rows.Add("Цель",
                        null, null, null, null);
                    dtres.Rows.Add(CountUpLevel.ToString() + ". " + dt.Rows[i][0].ToString(),
                        dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3], dt.Rows[i][4]);
                    lastRowUpLevel = dtres.Rows.Count - 1;
                    dtres.Rows.Add("    Вес в деятельности ОИВ: " + cs.Axes[1].Positions[i + 1].Members[0].MemberProperties[0].Value, 0
                    , null, null, null);
                }
                else
                {
                    if (cs.Axes[1].Positions[i + 1].Members[0].UniqueName.Split(xz, StringSplitOptions.None).Length == 6)
                    {

                        CountCenterLevel++;

                        if (lastRowCenterLevel != -1)
                        {
                            dtres.Rows[lastRowCenterLevel][4] = SumPercentCenter;
                            SumPercentUp += LastMassCenter * SumPercentCenter;
                        }

                        LastMassCenter = System.Decimal.Parse(cs.Axes[1].Positions[i + 1].Members[0].MemberProperties[1].Value.ToString());

                        SumPercentCenter = 0;

                        CountDownLevel = 0;
                        if (CheckBox1.Checked)
                        { }
                        dtres.Rows.Add("Задача",
                            null, null, null, null);

                        dtres.Rows.Add(CountUpLevel.ToString() + "." + CountCenterLevel.ToString() + ". " + dt.Rows[i][0].ToString(),
                            dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3], dt.Rows[i][4]);

                        lastRowCenterLevel = dtres.Rows.Count - 1;

                        dtres.Rows.Add("    Вес в цели: " + cs.Axes[1].Positions[i + 1].Members[0].MemberProperties[1].Value.ToString(),
                            null, null, null, null);
                        dtres.Rows.Add("Наименование показателя непосредственного результата", 0
                        , dt.Rows[i][2], dt.Rows[i][3], dt.Rows[i][4]);

                    }
                    else
                    {

                        SumPercentCenter += (System.Decimal)(dt.Rows[i][4]) * System.Decimal.Parse(cs.Axes[1].Positions[i + 1].Members[0].MemberProperties[3].Value.ToString());

                        CountDownLevel++;

                        dtres.Rows.Add(CountUpLevel.ToString() + "." + CountCenterLevel.ToString() + "." + CountDownLevel.ToString() + ". " + dt.Rows[i][0].ToString(),
                                dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3], dt.Rows[i][4]);

                        dtres.Rows.Add("    Единица измерения: " + cs.Axes[1].Positions[i + 1].Members[0].MemberProperties[2].Value.ToString(),
                            null, null, null, null);

                        dtres.Rows.Add("    Вес в задаче: " + cs.Axes[1].Positions[i + 1].Members[0].MemberProperties[3].Value.ToString(),
                            null, null, null, null);

                    }
                }

            }
            dtres.Rows[lastRowCenterLevel][4] = SumPercentCenter;

            SumPercentUp += LastMassCenter * SumPercentCenter;
            dtres.Rows[lastRowUpLevel][4] = SumPercentUp;




            G.DataSource = dtres;
            G.DataBind();

            G.Rows[0].Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;
            G.Rows[0].Cells[0].Style.BorderDetails.ColorTop = Color.LightGray;
            G.Rows[0].Cells[1].Style.BorderDetails.ColorTop = Color.LightGray;

            for (int i = 0; i < G.Rows.Count; i++)
            {
                G.Rows[i].Style.BackColor = Color.Transparent;
                G.Rows[i].Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;
                if ((G.Rows[i].Cells[0].Text.Split(' ')[0] + "-").Split('.').Length == 2)
                {
                    //G.Rows[i].Style.BackColor = //f1f1f2;
                    G.Rows[i].Cells[0].Style.CustomRules = "background-color: #f1f1f2";
                    G.Rows[i].Cells[1].RowSpan = 2;
                    G.Rows[i].Cells[1].ColSpan = 2;
                    G.Rows[i].Cells[3].RowSpan = 2;
                    G.Rows[i].Cells[3].ColSpan = 2;

                    G.Rows[i].Cells[1].Text = string.Format("{0:##0%}", G.Rows[i].Cells[4].Value);

                    G.Rows[i].Cells[1].Style.ForeColor = ((System.Decimal)(G.Rows[i].Cells[4].Value) >= 1 ? Color.Green : Color.Red);
                    G.Rows[i].Cells[1].Style.Font.Size = 20;

                    G.Rows[i].Cells[3].Text = ((System.Decimal)(G.Rows[i].Cells[4].Value) >= 1 ? "<img style=\"vertical-align:baseline\" src=\"../../images/GreenAllert.gif\">" : "<img style=\"vertical-align:baseline\" src=\"../../images/RedAllert.gif\">");

                    G.Rows[i].Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent; 
                    i++;
                    G.Rows[i].Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;

                    G.Rows[i].Style.BackColor = Color.Transparent;
                    G.Rows[i].Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;
                    G.Rows[i].Cells[0].Style.ForeColor = Color.Gray;
                }
                else
                    if ((G.Rows[i].Cells[0].Text.Split(' ')[0] + "-").Split('.').Length == 3)
                    {
                        if (!CheckBox1.Checked)
                        {
                            G.Rows[i].Hidden = 1 == 1;
                            i++;
                            G.Rows[i].Hidden = 1 == 1;
                            continue;
                        }
                        G.Rows[i].Cells[0].Style.CustomRules = "background-color: #f1f1f2";

                        G.Rows[i].Cells[1].RowSpan = 2;
                        G.Rows[i].Cells[1].ColSpan = 2;
                        G.Rows[i].Cells[3].RowSpan = 2;
                        G.Rows[i].Cells[3].ColSpan = 2;

                        //G.Rows[i].Cells[0].Style.Font.Bold = 1 == 1;


                        G.Rows[i].Cells[1].Text = string.Format("{0:##0%}", G.Rows[i].Cells[4].Value);

                        G.Rows[i].Cells[1].Style.ForeColor = ((System.Decimal)(G.Rows[i].Cells[4].Value) >= 1 ? Color.Green : Color.Red);
                        G.Rows[i].Cells[1].Style.Font.Size = 16;


                        G.Rows[i].Cells[3].Text = ((System.Decimal)(G.Rows[i].Cells[4].Value) >= 1 ? "<img style=\"vertical-align:baseline\" src=\"../../images/GreenAllertSmall.gif\">" : "<img style=\"vertical-align:baseline\" src=\"../../images/RedAllertSmall.gif\">");

                        G.Rows[i].Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                        i++;
                        G.Rows[i].Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;

                        G.Rows[i].Style.BackColor = Color.Transparent;
                        G.Rows[i].Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;
                        G.Rows[i].Cells[0].Style.ForeColor = Color.Gray;



                    }
                    else
                        if ((G.Rows[i].Cells[0].Text.Split(' ')[0] + "-").Split('.').Length == 4)
                        {
                            if (!CheckBox1.Checked)
                            {
                                G.Rows[i].Hidden = 1 == 1;
                                i++;
                                G.Rows[i].Hidden = 1 == 1;
                                i++;
                                G.Rows[i].Hidden = 1 == 1;
                                continue;
                            }

                            G.Rows[i].Cells[1].RowSpan = 3;
                            G.Rows[i].Cells[2].RowSpan = 3;
                            G.Rows[i].Cells[3].RowSpan = 3;
                            G.Rows[i].Cells[4].RowSpan = 3;

                            G.Rows[i].Cells[3].Style.ForeColor = ((System.Decimal)(G.Rows[i].Cells[4].Value) >= 1 ? Color.Green : Color.Red);
                            G.Rows[i].Cells[4].Style.ForeColor = ((System.Decimal)(G.Rows[i].Cells[4].Value) >= 1 ? Color.Green : Color.Red);

                            G.Rows[i].Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                            i++;
                            G.Rows[i].Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;

                            G.Rows[i].Style.BackColor = Color.Transparent;
                            G.Rows[i].Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;
                            G.Rows[i].Cells[0].Style.ForeColor = Color.Gray;

                            G.Rows[i].Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                            i++;
                            G.Rows[i].Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;

                            G.Rows[i].Style.BackColor = Color.Transparent;
                            G.Rows[i].Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;
                            G.Rows[i].Cells[0].Style.ForeColor = Color.Gray;

                            if (!CheckBox1.Checked)
                            {
                                G.Rows[i].Hidden = 1 == 1;
                                i++;
                                G.Rows[i].Hidden = 1 == 1;
                                i++;
                                G.Rows[i].Hidden = 1 == 1;
                                continue;
                            }
                        }
                        else
                        {
                            
                            if (G.Rows[i].Cells[0].Text == "Задача")
                            {
                                
                                G.Rows[i].Cells[0].ColSpan = 1;
                                G.Rows[i].Cells[0].Text = "Задача";
                                G.Rows[i].Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
                                G.Rows[i].Cells[0].Style.Font.Bold = 1 == 1;
                                G.Rows[i].Cells[0].Style.Font.Size = 11;

                                G.Rows[i].Cells[1].ColSpan = 4;
                                G.Rows[i].Cells[1].Text = "Результативность решения задачи";

                                G.Rows[i].Cells[1].Style.HorizontalAlign = HorizontalAlign.Center;
                                G.Rows[i].Cells[1].Style.Font.Bold = 1 == 1;
                                G.Rows[i].Cells[1].Style.Font.Size = 11;
                                G.Rows[i].Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;
                                if (!CheckBox1.Checked)
                                {
                                    G.Rows[i].Hidden = 1 == 1;
                                    continue;
                                }
                            }else
                                if (G.Rows[i].Cells[0].Text == "Цель")
                                {
                                    
                                    G.Rows[i].Cells[0].ColSpan = 1;
                                    G.Rows[i].Cells[0].Text = "Цель";
                                    G.Rows[i].Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
                                    G.Rows[i].Cells[0].Style.Font.Bold = 1 == 1;
                                    G.Rows[i].Cells[0].Style.Font.Size = 11;
                                    G.Rows[i].Cells[0].Style.Font.Name = "Arial";

                                    G.Rows[i].Cells[1].ColSpan = 4;
                                    G.Rows[i].Cells[1].Text = "Результативность решения задач в рамках цели";

                                    G.Rows[i].Cells[1].Style.HorizontalAlign = HorizontalAlign.Center;
                                    G.Rows[i].Cells[1].Style.Font.Bold = 1 == 1;
                                    G.Rows[i].Cells[1].Style.Font.Size = 11;
                                    G.Rows[i].Cells[1].Style.Font.Name = "Arial";
                                    G.Rows[i].Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;
                                    if (i != 0)
                                        if (!CheckBox1.Checked)
                                        {
                                            G.Rows[i].Hidden = 1 == 1;
                                            continue;
                                        }
                                }
                                else
                                {
                                    
                                    G.Rows[i].Cells[0].Style.BackColor = Color.DarkGray;
                                    G.Rows[i].Cells[0].Text = "Наименование показателя непосредственного результата";
                                    G.Rows[i].Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;

                                    G.Rows[i].Cells[1].Style.BackColor = Color.DarkGray;
                                    G.Rows[i].Cells[1].Text = "План";

                                    G.Rows[i].Cells[2].Style.BackColor = Color.DarkGray;
                                    G.Rows[i].Cells[2].Text = "Факт";

                                    G.Rows[i].Cells[3].Style.BackColor = Color.DarkGray;
                                    G.Rows[i].Cells[3].Text = "Абсолютное отклонение";

                                    G.Rows[i].Cells[4].Style.BackColor = Color.DarkGray;
                                    G.Rows[i].Cells[4].Text = "Относительное отклонение";
                                    if (!CheckBox1.Checked)
                                    {
                                        G.Rows[i].Hidden = 1 == 1;
                                        continue;
                                    }
                                } 
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

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.RowSelectorsDefault = RowSelectors.No;

            e.Layout.Bands[0].HeaderLayout.Clear();
            //e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder("Цель", 0, 0,1));
            //e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder("Результативность достижения цели", 1, 0,4));
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = 430 / 4;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.##");
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            }
            e.Layout.Bands[0].Columns[0].Width = 800;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "### ### ### ##0.##%");
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.NoDataMessage = "Нет данных";


        }


        #endregion


        #region Экпорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {

        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            foreach (Worksheet sheet in e.Workbook.Worksheets)
            {
                sheet.Rows[0].Height = 0;
                //for (int i = 0; i < G.Rows.Count; i++)
                //{
                //    for (int j = 0; j < G.Columns.Count; j++)
                //    {
                //        try
                //        {
                //            if (sheet.Rows[i].Cells[j].Value.ToString().IndexOf("<img") != -1)
                //            {
                //                sheet.Rows[i].Cells[j].Value = "";
                //            }
                //        }
                //        catch { }
                //        //sheet.Rows[i].Cells[j].Value = (sheet.Rows[i].Cells[j].ToString()[0] == '<' ? "" : sheet.Rows[i].Cells[j].Value);
                //    }
                //}

                for (int i = 1; i < G.Rows.Count + 1; i++)
                {
                    sheet.Rows[i].Height = 37 * 18;

                    for (int j = 0; j < G.Columns.Count; j++)
                    {
                        try
                        {
                            sheet.Columns[j].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                            if (sheet.Rows[i].Cells[j].Value.ToString().IndexOf("<img") != -1)
                            {
                                sheet.Rows[i].Cells[j].Value = "";
                            }
                            if ((G.Rows[i - 1].Cells[j].RowSpan > 1) || (G.Rows[i - 1].Cells[j].ColSpan > 1))
                            {
                                sheet.MergedCellsRegions.Add(i, j, i + G.Rows[i - 1].Cells[j].RowSpan - 1, j + G.Rows[i - 1].Cells[j].ColSpan - 1);
                            }



                        }
                        catch { }
                        //БУДЬ ПРОКЛЯТ ТОТ ДЕНЬ КОГДА Я СЕЛ ЗА КЛАВУ ЭТОЙ ЧЕРЕПАХИ!!!!!!!!!
                    }
                }
                sheet.Columns[0].Width = 800 * 37;
                sheet.Columns[1].Width = 80 * 37;
                sheet.Columns[2].Width = 80 * 37;
                sheet.Columns[3].Width = 80 * 37;
                sheet.Columns[4].Width = 80 * 37;

                sheet.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Columns[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Columns[2].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Columns[3].CellFormat.WrapText = ExcelDefaultableBoolean.True;



                sheet.Columns[1].CellFormat.FormatString = "#,##0";
                sheet.Columns[2].CellFormat.FormatString = "#,##0.0##";
                sheet.Columns[3].CellFormat.FormatString = "#,##0.0##";
                sheet.Columns[4].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;

            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();

            //for (int i = 0; i < AllGrid.Count; i++)
            {
                Worksheet sheet = workbook.Worksheets.Add("Таблица");
                UltraGridExporter1.ExcelExporter.ExcelStartRow = 0;
                for (int i = 0; i < G.Rows.Count; i++)
                {
                    G.Rows[i].Hidden = false;
                }
                    UltraGridExporter1.ExcelExporter.Export(G, sheet);
            }

        }

        #endregion



        protected void G_InitializeRow(object sender, RowEventArgs e)
        {

        }
    }
}
