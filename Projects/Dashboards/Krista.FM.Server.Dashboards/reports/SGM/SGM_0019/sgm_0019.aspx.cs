using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0019
{
    public partial class sgm_0019 : CustomReportPage
    {
        protected DataTable dtFullData = new DataTable();
        protected string month;
        protected string year1, year2;
        protected string groupName1, groupName2;
        protected string mapName, foName, rfName;
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMRegionNamer regionNamer = new SGMRegionNamer();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            grid.Width = Unit.Empty;
            grid.Height = Unit.Empty;

            SetExportHandlers();
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            dataRotator.CheckSubjectReport();
            regionNamer.FillSGMRegionsDictionary1();
            base.Page_Load(sender, e);
            dataRotator.formNumber = 1;
            dataObject.InitObject();
            if (!Page.IsPostBack)
            {
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillMonthListEx(ComboMonth, ComboYear.SelectedValue);
                dataRotator.FillSGMMapListOnlySubjects(ComboMap, dataObject.dtAreaShort, true);
            }
            else
            {
                dataRotator.FillSGMMapListOnlySubjects(null, dataObject.dtAreaShort, true);
            }

            dataRotator.FillDeseasesList(null, 0);
            string mapParamValue = UserParams.CustomParam("SGM0019MapName").Value;
            
            if (Page.IsPostBack
                || mapParamValue == string.Empty
                || !regionNamer.sgmDictionary1.ContainsKey(mapParamValue))
            {
                UserParams.CustomParam("SGM0019MapName").Value = ComboMap.SelectedValue;
                UserParams.CustomParam("SGM0019Year").Value = ComboYear.SelectedValue;
                UserParams.CustomParam("SGM0019Month").Value = dataRotator.GetMonthParamString(ComboMonth, ComboYear.SelectedValue);
            }

            year1 = UserParams.CustomParam("SGM0019Year").Value;
            month = UserParams.CustomParam("SGM0019Month").Value;
            mapName = UserParams.CustomParam("SGM0019MapName").Value;

            year2 = Convert.ToString(Convert.ToInt32(year1) - 1);
            dataRotator.CheckFormNumber(Convert.ToInt32(year1), ref month);
            foName = dataObject.GetFOName(dataObject.GetMapCode(mapName), false);
            
            if (foName.Length == 0)
            {
                foName = mapName;
            }
            
            rfName = dataRotator.mapList[0];


            groupName1 = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            groupName2 = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtChildAndTeens));

            FillData();
            FillComponentData();
        }

        protected virtual void FillComponentData()
        {
            string monthText = supportClass.GetMonthLabelFull(month, 0);
            if (month.Split(',').Length > 11)
            {
                monthText = String.Empty;
            }

            string territoryName = "субъекту";
            string foNameStr = "ФО";
            if (dataRotator.isSubjectReport)
            {
                territoryName = "району";
                foNameStr = "ТО";
            }


            Page.Title = String.Format(
                "Анализ заболеваемости по {1} {0} (рост/снижение, сравнение с {2} и {0})",
                dataObject.GetRootMapName(), territoryName, foNameStr);
            LabelTitle.Text = Page.Title;
            LabelSubTitle.Text = String.Format("{0}, {1} {2}-{3} гг.{4}",
                mapName, monthText, year1, year2, dataRotator.GetFormHeader());

            grid.Bands.Clear();
            grid.DataSource = dtFullData;
            grid.DataBind();
        }

        // Вот за этот конструктор уважаю себя
        protected virtual void FillData()
        {
            dataObject.InitObject();
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.useLongNames = true;
            dataObject.mainColumnRange = dataRotator.GetDeseaseCodes(0).Replace("42,", String.Empty);
            // Заболевание по субъекту текущий год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, mapName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "1");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, mapName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "3");
            // Заболевание по субъекту прошлый год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year2, month, mapName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "5");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year2, month, mapName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "7");
            // Рост \ снижение текстовка
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                "1", "5", "2", "6");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                "3", "7", "4", "8");
            // ФО
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, foName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "11");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, foName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "13");
            // РФ
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, rfName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "15");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, rfName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "17");
            dtFullData = dataObject.FillData();
            // Подчистим ненужные колонки с отн. показателями(они чтобы посчитать рост\снижение)
            dtFullData.Columns.RemoveAt(17);
            dtFullData.Columns.RemoveAt(15);
            dtFullData.Columns.RemoveAt(13);
            dtFullData.Columns.RemoveAt(11);
            dtFullData.Columns.RemoveAt(7);
            dtFullData.Columns.RemoveAt(5);
            dtFullData.Columns.RemoveAt(3);
            dtFullData.Columns.RemoveAt(1);
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Header.Caption = "Наименование заболеваний";
            e.Layout.Bands[0].Columns[0].Width = 215;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Header.Caption = "всего";
                if (i % 2 == 0) grid.Columns[i].Header.Caption = "у детей";
                grid.Columns[i].Width = 70;
                grid.Columns[i].Header.Title = "Заболеваемость на 100 тысяч населения";
                CRHelper.FormatNumberColumn(grid.Columns[i], "N2");
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                RowLayoutColumnInfo rlcInfo = e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo;
                if (i == 0)
                {
                    rlcInfo.OriginY = 0;
                    rlcInfo.SpanY = 2;
                }
                else
                {
                    rlcInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;
            var captions = new string[5];
            captions[0] = String.Format("{0} {1}г.", supportClass.GetMonthLabelFull(month, 0), year1);
            captions[1] = String.Format("{0} {1}г.", supportClass.GetMonthLabelFull(month, 0), year2);
            captions[2] = "Рост/снижение";
            captions[3] = String.Format("{0} {1}", supportClass.GetFOShortName(foName), captions[0]);
            captions[4] = String.Format("{0} {1}", supportClass.GetFOShortName(rfName), captions[0]);

            for (int i = 0; i < 5; i++)
            {
                var ch = new ColumnHeader(true) {RowLayoutColumnInfo = {OriginY = 0, OriginX = multiHeaderPos}};
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                ch.Style.HorizontalAlign = HorizontalAlign.Center;
                ch.Caption = captions[i];
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            supportClass.SetCellImageEx(e.Row, 3, 1, 1);
            supportClass.SetCellImageEx(e.Row, 4, 2, 2);
        }

        #region PDFExport

        protected virtual void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        protected virtual void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            InitializeExportLayout(e);

            exportClass.ExportCaptionText(e, LabelTitle.Text);
            exportClass.ExportSubCaptionText(e, LabelSubTitle.Text);
        }

        protected virtual void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = String.Format("0019.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
        }

        #endregion


    }
}
