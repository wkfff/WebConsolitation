using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using Dundas.Maps.WebControl;
using System.Drawing.Imaging;
using Infragistics.UltraGauge.Resources;

/**
 *  Уровень жизни населения.
 */
namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_0001._OverallTable
{
    public partial class Default : CustomReportPage
    {

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "Нет данных");
            return dt;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {            
            G.Width = CustomReportConst.minScreenWidth - 15;
            RegionBase = UserParams.CustomParam("RegionBase");

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = false;

        }

        // --------------------------------------------------------------------
        string heder;

        Dictionary<string, string> CaptionKey;
        Dictionary<string, string> KeyCaption;
        Dictionary<string, bool> KeyAgr;

        private CustomParam RegionBase;

        Dictionary<string, int> GenDict()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            try
            {                
                int i = 0;
                //ну один то там по любому есть, мы же откуда то пришли   
                string b = RegionSettingsHelper.Instance.GetPropertyValue("Reports_0");

                //там хранится иксемелька в удобном для мя виду
                CaptionKey = new Dictionary<string, string>();
                KeyCaption = new Dictionary<string, string>();
                KeyAgr = new Dictionary<string, bool>();

                //пока не вытащим пустоту
                for (; !string.IsNullOrEmpty(b);)
                {
                    //вытаскиваем из хмельки Report_1, Report_2 .... пустота
                    try
                    {
                        i++;
                        string[] lastS = b.Split(';');
                        d.Add(lastS[1], 0);
                        //указаия на то агрегировать на верхние или нет может и не быть
                        if (PrevReport == lastS[0])
                        {
                            try
                            {                                
                                Aggregate = lastS[2].ToUpper() == "TRUE";                                
                            }
                            catch {}

                        }
                        //заполняем словарики
                        KeyAgr.Add(lastS[0], lastS[2].ToUpper() == "TRUE");
                        CaptionKey.Add(lastS[1], lastS[0]);
                        KeyCaption.Add(lastS[0], lastS[1]);                        

                        b = RegionSettingsHelper.Instance.GetPropertyValue("Reports_" + i.ToString());
                    }
                    catch
                    {
                        //на всякий случай, чтобы не зациклился
                        break;
                    }
                }
            }
            catch { }
            return d;

        }

        void GetPrevReport()
        {
                   
        }
        string Name="";
        void GetCaptionReport()
        {
            
        }

        bool Aggregate = 1 == 1;
        string PrevReport = "";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);            
            if (!Page.IsPostBack)
            {
                //берём територию
                RegionBase.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                try
                {       
                    //Определяем с каково отчёта к нам сюда пажаловали
                    PrevReport = Page.Request.QueryString["pok"];
                    //если пусто значит не с какого
                    if (string.IsNullOrEmpty(PrevReport))
                    {
                        //ставим тот какторый там перывый
                        PrevReport = "LifeLevel";
                        HyperLink1.Visible = 1 == 2;                    
                    }
                }
                catch
                {
                   //ну вы поняли...
                    PrevReport = "LifeLevel";
                    HyperLink1.Visible = 1 == 2;                    
                }
                String.Format(PrevReport);                
                //заполняем списочек всем тем что есть у нас в иксемельке
                a.FillDictionaryValues(GenDict());                
                //активным делаем тот с каторого пришли
                a.SetСheckedState(KeyCaption[PrevReport], 1 == 1);                
                a.Width = 600;
                a.Title = "Отчёт";
                //ссылочку назад
                HyperLink1.NavigateUrl = "../" + PrevReport + "/default.aspx";
            }
            else
            { 
                //перезаплняем табличку соответсвия так как она терь пустая
                GenDict(); 
            }
                //ищем каталог по имени рускаму
                PrevReport = CaptionKey[a.SelectedValue];
                try
                {
                    //смотрим что там с агрегированием на верхние
                    Aggregate = KeyAgr[PrevReport];
                }
                catch { }
                
                String.Format("prev" + PrevReport);
                //заголовочек
                Label1.Text = a.SelectedValue;
                //там ранше был кот, теперь его нет
                GetPrevReport();
                //надобы это не тут написать
                Hederglobal.Text = "Cводный отчет";
                Hederglobal.Text = a.SelectedValue + " (Cводный отчет)";
                //и наконец то строим его
                G.DataBind();
                //шоб само подогналось ставим ничего    
                G.Height = Unit.Empty;
                Page.Title = Label1.Text + " ("+Hederglobal.Text.ToLower()+")";
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            string xz = DataProvider.GetQueryText(PrevReport);
            DataTable dt = new DataTable();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(xz);
            //оляяя!
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(cs, dt, "Оляля!");
            string[] o = {"].["};
            int CountHiden = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //удаляем уровень данные
                if (dt.Rows[i][0].ToString()[0] == '(')
                {
                    dt.Rows[i].Delete();
                    CountHiden++;
                    
                    i--;
                }
                else
                {
                    //Запоминаем уровень чтобы патом отфарматировать
                    dt.Rows[i][0] = cs.Axes[1].Positions[i-CountHiden].Members[0].UniqueName.Split(o, StringSplitOptions.None).Length.ToString() + ";" + dt.Rows[i][0].ToString();               
                }
                
                
                
            }

            G.DataSource = dt;


        }
        int prevRow = 0;
        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                //В первой самы верхний уровень, он не нужен
                if (e.Row.Index == 0)
                {
                    e.Row.Hidden = 1 == 1;
                    return;
                }
                //Определяем отступ
                int level = (int.Parse(e.Row.Cells[0].Text.Split(';')[0]) - 5) * 12;
                //Cтавим нормальное имя показателя
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split(';')[1];
                //и его отступ
                e.Row.Cells[0].Style.Padding.Left = level;
                //дописываем единицу измерения, если есть канеш
                if (e.Row.Cells[1].Text != "Неизвестные данные")
                {
                    e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower();
                }
                
                //если не задан флаг сумирования не нижних уровней 
                if (Aggregate&(prevRow < level))
                {
                    //то чистим все у каторых есть подчинённые
                    for (int i = 1; i < e.Row.Cells.Count; i++) G.Rows[e.Row.Index - 1].Cells[i].Value = null;
                }
                prevRow = level;

            }
            catch { }

        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                //навводим красату в первом столбике
                e.Layout.Bands[0].Columns[0].Width = 300;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[0].Header.Caption = "Показатель";
                //ну и тд
                e.Layout.Bands[0].CellClickAction = CellClickAction.NotSet;
                e.Layout.Bands[0].AllowSorting = AllowSorting.No;
            }
            catch { }
            //скрываем столбец с единичками измерения, ибо патом мы их приплюсуем к показателю
            e.Layout.Bands[0].Columns[1].Hidden = 1 == 1;
            //форматируем столбцы ну и тд
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.##");
                    e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                }

        }


        #region Экспорт Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {

        }


        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;            
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 1;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G, sheet1);            
        }

        #endregion

    }

}

