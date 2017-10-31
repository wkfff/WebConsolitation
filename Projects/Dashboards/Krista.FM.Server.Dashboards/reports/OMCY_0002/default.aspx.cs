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

namespace Krista.FM.Server.Dashboards.reports.OMCY_0002
{
    public partial class _default : CustomReportPage
    {

        private CustomParam RegionBase { get { return (UserParams.CustomParam("RegionBase")); } }
        private CustomParam Year { get { return (UserParams.CustomParam("Year")); } }
        string[] GridColHeder = { "Территория", "Значение,\n рубль", "Абсолютное отклонение от АППГ", "Темп роста\n к АППГ", "Отклонение \n(от ФО)", "Ранг по ФО", "Отклонение\n (от РФ)", "Ранг по РФ" };



        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.SpareMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.SecondaryMASDataProvider.GetCellset(s), dt, "Нет данных");
            return dt;
        }

        #region Other
        private String dir
        {
            get { return Server.MapPath("~") + "\\"; }
        }

        string MaskRows = "[ОМСУ__Показатели].[ОМСУ__Показатели].[{0}]";
        string DelDouble(string s)
        {
            try
            {
                for (; s.IndexOf('\n') != -1; )
                {
                    s = s.Replace("\n", " ");
                }
                for (; s.IndexOf('\r') != -1; )
                {
                    s = s.Replace("\r", " ");
                }
            }
            catch { }
            try
            {
                for (; s.IndexOf("  ") != -1; )
                {
                    s = s.Replace("  ", " ");
                }
            }
            catch { }

            try
            {
                if (s[0] == ' ')
                {
                    s.Remove(0, 1);
                }
                if (s[s.Length - 1] == ' ')
                {
                    s.Remove(s.Length - 1, 1);
                }
            }
            catch { }
            return s;
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
                if (true)
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

        protected void SetStar(UltraWebGrid G, int Col, int RowBaseVaslue, string Star)
        {
            for (int i = 0; G.Rows.Count > i; i++)
            {
                if (G.Rows[i].Cells[Col].Value == G.Rows[RowBaseVaslue].Cells[Col].Value)
                {
                    G.Rows[i].Cells[Col].Style.BackgroundImage = Star;//"~/images/starYellowBB.png";
                    G.Rows[i].Cells[Col].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }


        }

        protected void SetOtherIfNoZero(UltraWebGrid G, int Col)
        {
            for (int i = 0; G.Rows.Count > i; i++)
            {
                //Игнор строк   
                try
                {
                    if (G.Rows[i].Cells[Col].Value != null)
                    {
                        if ((System.Decimal)(G.Rows[i].Cells[G.Rows[i].Cells.Count - 1].Value) == -1)
                        {
                            if ((System.Decimal)(G.Rows[i].Cells[Col].Value) > 0)
                            {
                                G.Rows[i].Cells[Col].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            }
                            if ((System.Decimal)(G.Rows[i].Cells[Col].Value) < 0)
                            {
                                G.Rows[i].Cells[Col].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            }
                        }
                        else
                        {
                            if ((System.Decimal)(G.Rows[i].Cells[Col].Value) > 0)
                            {
                                G.Rows[i].Cells[Col].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            }
                            if ((System.Decimal)(G.Rows[i].Cells[Col].Value) < 0)
                            {
                                G.Rows[i].Cells[Col].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            }
                        }
                        G.Rows[i].Cells[5].Text = string.Format("{0:### ### ##0.00}%", G.Rows[i].Cells[5].Value);
                    }
                    G.Rows[i].Cells[Col].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
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
            for(int i = 1; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;
        }
        #endregion

        #region Лоады
        string BN = "APPLEMAC-SAFARI";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            G.Height = Unit.Empty;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            
            RegionBase.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                FOPARAM.FillDictionaryValues(GenUserParam("LD"));
                MRGO.FillDictionaryValues(GenUserParam("MRGO"));

                MRGO.Width = 500;   

                FOPARAM.Title = "Год";

                MRGO.Title = "Муниципальное образование";

            }
            
            Year.Value = FOPARAM.SelectedValue;
            RegionBase.Value = RegionSettingsHelper.Instance.RegionBaseDimension + string.Format(".[{0}]", MRGO.SelectedValue);
            Label4.Text =  string.Format("Показатели эффективности деятельности органов местного самоуправления ({0})",MRGO.SelectedValue);
            G.DataBind();
            SetOtherIfNoZero(G, 5);
            Hederglobal.Text = string.Format("Доклад главы администрации о достигнутых значениях показателей эффективности деятельности органов местного самоуправления ({0})", UserComboBox.getLastBlock(RegionSettingsHelper.Instance.RegionBaseDimension));
            //Доклад главы администраций о достигнутых значениях показателей эффективности деятельности органов местного самоуправления (параметр 2) 
            Page.Title = Hederglobal.Text;

        }
        #endregion

        #region Гридик
        protected void G_DataBinding(object sender, EventArgs e)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dir + "\\reports\\OMCY_0002\\table.xml");
            XmlNode root = xmlDoc.ChildNodes[1];
            DataTable dt = new DataTable();


            dt.Columns.Add("№ п.п.");
            dt.Columns.Add("Показатели");
            dt.Columns.Add("Единица измерения");
            dt.Columns.Add("2008", typeof(System.Decimal));
            dt.Columns.Add("2009", typeof(System.Decimal));
            dt.Columns.Add("Изменение", typeof(System.Decimal));
            dt.Columns.Add("2010", typeof(System.Decimal));
            dt.Columns.Add("2011", typeof(System.Decimal));
            dt.Columns.Add("2012", typeof(System.Decimal));
            dt.Columns.Add("reverce", typeof(System.Decimal));

            DataTable dtTable = GetDSForChart("G");


            int index = 0;
            foreach (XmlNode node in root.ChildNodes)
            {
                index++;
                if (node.ChildNodes.Count > 4)
                {
                    int i = 0;
                    #region Поискв дататабле
                    
                    for (i = 0; i < dtTable.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(DelDouble(node.ChildNodes[3].InnerText)))
                        {
                            string s1 = DelDouble(node.ChildNodes[3].InnerText);

                            //Что то типа нечёткого поиска
                            string sXML = DelDouble(node.ChildNodes[3].InnerText);
                            string sDtTable = DelDouble(dtTable.Rows[i][0].ToString());

                            if (
                                ((sXML.Contains(sDtTable))
                                || (sDtTable.Contains(sXML)))
                                && (Math.Abs(sXML.Length - sDtTable.Length)<10)
                                )                            
                                {
                                    break;
                                }
                            }

                        }
                    #endregion

                    System.Decimal TempAdd = 0;                    
                    try
                    {
                        TempAdd = (((System.Decimal)dtTable.Rows[i][2] / (System.Decimal)dtTable.Rows[i][1]) - 1) * 100;
                    }
                    catch {}
                    
                    if (i != dtTable.Rows.Count)
                        if ((node.ChildNodes.Count == 6) &&  DelDouble(node.ChildNodes[5].InnerText) == "-1")
                        {
                            #region Строки к которым не применяется числовое форматирование
                            dt.Rows.Add(index.ToString() + "_" + node.ChildNodes[1].InnerText+"_0",
                                node.ChildNodes[0].InnerText,
                                node.ChildNodes[2].InnerText,
                                dtTable.Rows[i][1],
                                dtTable.Rows[i][2],
                                null,
                                dtTable.Rows[i][3],
                                dtTable.Rows[i][4],
                                dtTable.Rows[i][5],
                                dtTable.Rows[i][6]);
                            #endregion
                        }
                        else
                        {
                            #region Строки с даными
                            dt.Rows.Add(index.ToString() + "_" + node.ChildNodes[1].InnerText,
                                node.ChildNodes[0].InnerText,
                                node.ChildNodes[2].InnerText,
                                dtTable.Rows[i][1],
                                dtTable.Rows[i][2],
                                TempAdd,
                                dtTable.Rows[i][3],
                                dtTable.Rows[i][4],
                                dtTable.Rows[i][5],
                                dtTable.Rows[i][6]);
                            #endregion
                        }


                }
                else
                {
                    try
                    {
                        #region Имена разделов, вопщ те каторые должны быть жирныси
                        dt.Rows.Add(index.ToString() + "_" + node.ChildNodes[1].InnerText,
                                                   node.ChildNodes[0].InnerText,
                                                   null,
                                                   null,
                                                   null,
                                                   null,
                                                   null,
                                                   null,
                                                   null,
                                                   null);
                        #endregion
                    }
                    catch {
                        try
                        {
                            #region Если незадан уроень раздела....
                            dt.Rows.Add(index.ToString() + "_" + null,
                           node.ChildNodes[0].InnerText,
                           null,
                           null,
                           null,
                           null,
                           null,
                           null,
                           null,
                           null);
#endregion
                        }
                        catch { }
                    }
                }
            }

            G.DataSource = dt;
        }
        #region Наводилки крастоты
        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            string[] index_and_offset = e.Row.Cells[0].Text.Split('_');

            


            int offset = 0;

            try
            {
                offset = int.Parse(index_and_offset[1]);
            }
            catch
            {
 
            }
            
            if (offset == -1)
            {
                e.Row.Cells[1].Style.Font.Bold = 1 == 1;
                e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[1].ColSpan = 8;
            }
                else
                if (offset == 0)
                {
                    e.Row.Cells[1].Style.Font.Bold = 2 == 1;
                    e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Left;
                }
                else
                {
                    e.Row.Cells[1].Style.Padding.Left = offset * 10;
                }
            e.Row.Cells[0].Text = index_and_offset[0];

            #region Исключения
            bool Specific = ((index_and_offset.Length == 3) && (index_and_offset[2] == "0"));
            if (Specific)
            {
                for (int i = 3; i < e.Row.Cells.Count; i++)
                {
                    try
                    {
                        if (e.Row.Cells[i].Value.ToString() == "0")
                        {
                            e.Row.Cells[i].Text = "-";
                        }
                        else
                        {
                            
                            e.Row.Cells[i].Text = string.Format("{0:#######} г.", e.Row.Cells[i].Value);
                        }
                    }
                    catch { }
                }
            }
            else
            {
                try
                {
                    if (e.Row.Cells[2].Text.Contains("да (нет)"))
                    {
                        for (int i = 3; i < e.Row.Cells.Count; i++)
                            try
                            {
                                if (i != 5)
                                {
                                    e.Row.Cells[i].Text = ((((System.Decimal)e.Row.Cells[i].Value) == 0) ? "нет" : "да");
                                }
                                else { e.Row.Cells[5].Text = "-"; }
                            }
                            catch { }
                    }
                }
                catch { }
            }

            #endregion
            

            
        }
        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.00");
            }

            e.Layout.SortingAlgorithmDefault = SortingAlgorithm.NotSet;
            e.Layout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            e.Layout.CellClickActionDefault = CellClickAction.NotSet;
            e.Layout.SortCaseSensitiveDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;
            
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = 1 == 1;

            e.Layout.Bands[0].Columns[0].Width = 30;
            if (BN == "IE")
            {
                e.Layout.Bands[0].Columns[1].Width = 410;
                
            }
            else
            {
                e.Layout.Bands[0].Columns[1].Width = 450;
            }
            
            //e.Layout.Bands[0].Columns[1].Width = 450;

            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.SpanY = 2;


            for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
            }
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder("Завершённый период",3,0,1,1));

            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder("Отчетный период", 4, 0, 2, 1));

            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder("Плановый период", 6, 0, 3, 1));
            //Плановый период

        }
        ColumnHeader GenColumnHeder(string Caption, int x, int y, int spanX, int spanY)
        {
            ColumnHeader CH = new ColumnHeader(1 == 1);
            CH.RowLayoutColumnInfo.OriginX = x;
            CH.RowLayoutColumnInfo.OriginY = y;
            CH.RowLayoutColumnInfo.SpanX = spanX;
            CH.RowLayoutColumnInfo.SpanY = spanY;
            CH.Caption = Caption;
            CH.Style.Wrap = 1 == 1;
            CH.Style.HorizontalAlign = HorizontalAlign.Center;
            return CH;
        }
        #endregion

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label4.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            try
            {
                e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Columns[1].Width = 20000;
                e.CurrentWorksheet.Columns[2].Width = 8000;
                e.CurrentWorksheet.Columns[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Columns[2].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                for (int i = 2; i < G.Rows.Count+2; i++)
                {
                    e.CurrentWorksheet.Rows[i].Height = 800;                    
                }
                for (int i = 3; G.Columns.Count>i;i++ )
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "### ### ##0.00";
                    //for (int j = 2; j < G.Rows.Count + 2; j++)
                    //{
                    //    //if (e.CurrentWorksheet.Rows[j].Cells[i].Value.ToString().IndexOf("<!---->")!=-1)
                    //    //{
                    //    //    e.CurrentWorksheet.Rows[j].Cells[i].Value = e.CurrentWorksheet.Rows[j].Cells[i].Value.ToString().Replace("<!---->","");
                    //    //}
                        
                    //}
                }
            }
            catch { }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 3;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        #endregion

        protected void G_Click(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
            //
        }

    }
}
