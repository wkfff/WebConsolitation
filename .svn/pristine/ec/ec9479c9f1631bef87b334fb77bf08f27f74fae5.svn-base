using System;
using System.Data;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Infragistics.WebUI.UltraWebGrid;
using System.Collections.ObjectModel;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0020
{
    public partial class sgm_0020 : CustomReportPage
    {
        protected DataTable dtFullData = new DataTable();
        protected string groupName1, groupName2;
        protected string mapName, totalCodes;
        Collection<string> years = new Collection<string>();
        protected SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
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
            dataRotator.formNumber = 2;
            if (dataRotator.isSubjectReport)
            {
                dataRotator.formNumber = 1;
            }

            dataObject.InitObject();
            if (!Page.IsPostBack)
            {
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillDeseasesList(ComboDesease, 0);
                dataRotator.FillSGMMapList(ComboMap, dataObject.dtAreaShort, true);
                ComboYear.SetСheckedState(Convert.ToString(Convert.ToInt32(ComboYear.SelectedValues[0]) - 1), true);
                supportClass.FillPeopleGroupList2(ComboChildGroup);
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, true);
                dataRotator.FillDeseasesList(null, -1);
            }

            dataRotator.FillRelationValues();

            totalCodes = String.Empty;

            string mapParamValue = UserParams.CustomParam("SGM0020MapName").Value;
            if (Page.IsPostBack || mapParamValue.Length == 0 || !regionNamer.sgmDictionary1.ContainsKey(mapParamValue))
            {
                foreach (string deseaseName in ComboDesease.SelectedValues)
                {
                    totalCodes = String.Format("{0},{1}", totalCodes, dataRotator.deseasesRelation[deseaseName]);
                }

                UserParams.CustomParam("SGM0020MapName").Value = ComboMap.SelectedValue;
                UserParams.CustomParam("SGM0020Desease").Value = totalCodes;
                UserParams.CustomParam("SGM0020Group").Value = ComboChildGroup.SelectedIndex.ToString();
                UserParams.CustomParam("SGM0020Year").Value = ComboYear.SelectedValuesString;
            }

            int groupIndex = Convert.ToInt32(UserParams.CustomParam("SGM0020Group").Value);
            mapName = Convert.ToString(UserParams.CustomParam("SGM0020MapName").Value);
            totalCodes = Convert.ToString(UserParams.CustomParam("SGM0020Desease").Value);
            years = new Collection<string>(UserParams.CustomParam("SGM0020Year").Value.Split(','));

            groupName1 = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            PeopleGroupType pgt = PeopleGroupType.pgtChildAndTeens;
            if (groupIndex == 0) pgt = PeopleGroupType.pgtLess1Year;
            if (groupIndex == 1) pgt = PeopleGroupType.pgtMore1Less2;
            if (groupIndex == 2) pgt = PeopleGroupType.pgtMore3Less6;
            if (groupIndex == 3) pgt = PeopleGroupType.pgtChild;
            if (groupIndex == 4) pgt = PeopleGroupType.pgtVillageChild;
            if (groupIndex == 5) pgt = PeopleGroupType.pgtTeen;
            if (groupIndex == 6) pgt = PeopleGroupType.pgtChildAndTeens;
            if (groupIndex == 7) pgt = PeopleGroupType.pgtVillageTeens;

            groupName2 = Convert.ToString(Convert.ToInt32(pgt));

            // Читаем данные из БД
            FillData();
            FillComponentData();
        }

        protected virtual void FillComponentData()
        {
            string totalYear = String.Empty;
            foreach (string year in years)
            {
                totalYear = String.Format("{0},{1}", totalYear, year);
            }

            totalYear =  totalYear.Trim(',');

            string territoryName = "субъекту";
            if (dataRotator.isSubjectReport)
            {
                territoryName = "району";
            }

            Page.Title = String.Format("Годовая динамика заболеваемости по {0} {1}", 
                territoryName, dataObject.GetRootMapName());
            LabelTitle.Text = Page.Title;
            LabelSubTitle.Text = String.Format("{0}, {1} гг.", mapName, totalYear);

            grid.Bands.Clear();
            grid.DataSource = dtFullData;
            grid.DataBind();
        }

        // Вот за этот конструктор уважаю себя
        protected virtual void FillData()
        {
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.mainColumnRange = totalCodes.TrimStart(',').Replace("42,", String.Empty);
            dataObject.useLongNames = true;
            int counter = 0;

            foreach (string year in years)
            {
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                    year, String.Empty, mapName, groupName1, String.Empty);
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                    Convert.ToString(counter * 4 + 1));
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                    year, String.Empty, mapName, groupName2, String.Empty);
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                    Convert.ToString(counter * 4 + 3));
                counter++;
            }
            
            dtFullData = dataObject.FillData();            
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
                grid.Columns[i].Header.Caption = "абс.";
                grid.Columns[i].Header.Title = "Абсолютная заболеваемость";
                CRHelper.FormatNumberColumn(grid.Columns[i], "N0");
                if (i % 2 == 0)
                {
                    grid.Columns[i].Header.Caption = "на 100 тыс.";
                    grid.Columns[i].Header.Title = "Заболеваемость на 100 тысяч населения";
                    CRHelper.FormatNumberColumn(grid.Columns[i], "N2");
                }
                grid.Columns[i].Width = 70;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                RowLayoutColumnInfo rlcInfo = e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo;
                if (i == 0)
                {
                    rlcInfo.OriginY = 0;
                    rlcInfo.SpanY = 3;
                }
                else
                {
                    rlcInfo.OriginY = 2;
                }
            }

            int multiHeaderPos1 = 1;
            int multiHeaderPos2 = 1;

            for (int i = 0; i < years.Count; i++)
            {
                var ch1 = new ColumnHeader(true) {RowLayoutColumnInfo = {OriginY = 1, OriginX = multiHeaderPos1}};
                multiHeaderPos1 += 2;
                ch1.RowLayoutColumnInfo.SpanX = 2;
                ch1.Style.Wrap = true;
                ch1.Style.HorizontalAlign = HorizontalAlign.Center;
                ch1.Caption = "Всего";
                e.Layout.Bands[0].HeaderLayout.Add(ch1);

                var  ch2 = new ColumnHeader(true) {RowLayoutColumnInfo = {OriginY = 1, OriginX = multiHeaderPos1}};
                multiHeaderPos1 += 2;
                ch2.RowLayoutColumnInfo.SpanX = 2;
                ch2.Style.Wrap = true;
                ch2.Style.HorizontalAlign = HorizontalAlign.Center;
                ch2.Caption = "Дети";
                e.Layout.Bands[0].HeaderLayout.Add(ch2);
                
                var ch3 = new ColumnHeader(true)
                              {
                                  RowLayoutColumnInfo =
                                      {
                                          OriginY = 0,
                                          OriginX = multiHeaderPos2,
                                          SpanX = 4
                                      },
                                  Caption = years[i]
                              };

                ch3.Style.Wrap = true;
                ch3.Style.HorizontalAlign = HorizontalAlign.Center;
                multiHeaderPos2 += 4;
                e.Layout.Bands[0].HeaderLayout.Add(ch3);
                 
            }
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
            UltraGridExporter1.PdfExporter.DownloadName = String.Format("0020.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
        }

        #endregion

    }
}
