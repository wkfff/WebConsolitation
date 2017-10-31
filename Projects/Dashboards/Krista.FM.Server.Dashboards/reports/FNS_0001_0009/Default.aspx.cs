using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0009
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear;
        private int endYear = 2011;
        private string month = "������";
        private bool internalCirculatoinExtrude = false;
        private static MemberAttributesDigest okvedDigest;
        private static MemberAttributesDigest budgetLevelDigest;

        private bool GrowRateRanking
        {
            get { return Convert.ToBoolean(growRateRanking.Value); }
        }

        #region ��������� �������

        // ������ �����
        private CustomParam incomesTotal;
        // ������� �� � ��
        private CustomParam regionsLevel;
        // ��������� ������
        private CustomParam selectedRegion;
        // ���������� ���
        private CustomParam predYear;
        // ����������� ���
        private CustomParam predpredYear;
        // ������ �������
        private CustomParam fnsKDGroup;

        // ��� ��������� ���� ��� ������������������ ������� ��������
        private CustomParam consolidateBudgetDocumentSKIFType;
        // ��� ��������� ���� ��� �������
        private CustomParam regionBudgetDocumentSKIFType;
        // ������� ������� ���� ��� �������
        private CustomParam regionBudgetSKIFLevel;

        // ��� ��������� ���� ��� ������� ��������
        private CustomParam localBudgetDocumentSKIFType;
        // ������� ������� ���� ��� ������� ��������
        private CustomParam localBudgetSKIFLevel;

        // ����������������� ������ ��������
        private CustomParam regionsConsolidateBudget;

        // �������� ����� ��� ����� �����
        private CustomParam growRateRanking;

        // ������� ������ �����
        private CustomParam incomesTotalItem;
        // ������� ������������� �����������
        private CustomParam gratuitousIncomesItem;

        // ��������� ��������� ������
        private CustomParam rubMultiplier;

        // ������� �������
        private CustomParam level;

        // ������� �������
        private CustomParam OKVDGroup;

        // ������� �������
        private CustomParam budgetLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ������������� ���������� �������

            if (level == null)
            {
                level = UserParams.CustomParam("level");
            }
            if (fnsKDGroup == null)
            {
                fnsKDGroup = UserParams.CustomParam("fns_kd_group");
            }
            if (OKVDGroup == null)
            {
                OKVDGroup = UserParams.CustomParam("selected_OKVD");
            }
            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (predYear == null)
            {
                predYear = UserParams.CustomParam("predYear");
            }
            if (predpredYear == null)
            {
                predpredYear = UserParams.CustomParam("predpredYear");
            }
            incomesTotal = UserParams.CustomParam("incomes_total");
            regionsLevel = UserParams.CustomParam("regions_level");
            consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            growRateRanking = UserParams.CustomParam("grow_rate_ranking");
            incomesTotalItem = UserParams.CustomParam("incomes_total_item");
            gratuitousIncomesItem = UserParams.CustomParam("gratuitous_incomes_item");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            localBudgetDocumentSKIFType = UserParams.CustomParam("local_budget_document_skif_type");
            localBudgetSKIFLevel = UserParams.CustomParam("local_budget_skif_level");

            budgetLevel = UserParams.CustomParam("budget_level");

            #endregion

            growRateRanking.Value = RegionSettingsHelper.Instance.GetPropertyValue("GrowRateRanking");

            if (GrowRateRanking)
            {
                PopupInformer1.HelpPageUrl = "Default_GrowRateRanking.html";
            }

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth );
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 250);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";
            UltraWebGrid.InitializeLayout +=
                new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport +=
                new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            CrossLink1.Text = "���� ����� ������� �������� ������������� �����������";
            CrossLink1.NavigateUrl = "";
            CrossLink1.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            internalCirculatoinExtrude = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("InternalCirculationExtrude"));
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0001_0009_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboMonth.Title = "�����";

                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(month, true);

                ComboRegion.Visible = true;
                ComboRegion.Title = "����������";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillSettlements(RegionsNamingHelper.LocalSettlementTypes, true));
                ComboRegion.Set�heckedState("��� ����������", true);

                ComboKD.Width = 280;
                ComboKD.Title = "��� ������";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullFNSKDIncludingList());
                ComboKD.Set�heckedState("��������� ������ ", true);
                ComboKD.Visible = true;


                okvedDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "OKVEDList");
                comboOKVED.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(okvedDigest.UniqueNames, okvedDigest.MemberLevels));
                comboOKVED.Title = "�����";
                comboOKVED.Width = 320;
                comboOKVED.MultiSelect = false;
                comboOKVED.ParentSelect = true;
                comboOKVED.Set�heckedState("��� ���� �����", true);
                comboOKVED.Visible = false;

                ComboBudgetLevel.Title = "������� �������";
                ComboBudgetLevel.Width = 200;
                ComboBudgetLevel.ParentSelect = true;
                ComboBudgetLevel.MultiSelect = false;
                budgetLevelDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0001_0009_budgetLevelDigest");
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetLevelDigest.UniqueNames, budgetLevelDigest.MemberLevels));
                ComboBudgetLevel.Set�heckedState("����.������ ��������", true);
                //ComboBudgetLevel.SelectLastNode();
                
                //ComboBudgetLevel.Visible = false;
               
            }

            Page.Title = "����������� �� ����� ������������� ������������ � ����� ��������� �������";
            Label1.Text = Page.Title;
            if (ComboRegion.SelectedValue == "��� ����������")
            {
                selectedRegion.Value = string.Format("{0}.[��� ������]", RegionSettingsHelper.Instance.RegionDimension);
            }
            else
            {
                selectedRegion.Value = RegionsNamingHelper.LocalSettlementUniqueNames[ComboRegion.SelectedValue];
            }
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            DateTime currDateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue.ToString()), CRHelper.MonthNum(ComboMonth.SelectedValue), 01);
            currDateTime = currDateTime.AddMonths(1);
            string incom = string.Empty;
            if (IncomesList.SelectedIndex == 0)
            {
                incom = ComboKD.SelectedValue;
            }
            else
            {
                incom = comboOKVED.SelectedValue;
            }
            string date = String.Format("{0:dd.MM.yyyy}", currDateTime);
            Label2.Text = String.Format("{1}, {2}, ������ �� ��������� �� {0} ����", date, ComboRegion.SelectedValue, incom.TrimEnd(' '));

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            predYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1);
            predpredYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 2);
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            fnsKDGroup.Value = ComboKD.SelectedValue;
            if (comboOKVED.SelectedValue == "��� ���� �����")
            {
                OKVDGroup.Value = "[�����].[������������].[��� ���� �����]";
            }
            else
             OKVDGroup.Value = string.Format("[�����].[������������].[��� ���� �����].[{0}]", comboOKVED.SelectedValue);
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
            localBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetDocumentSKIFType");
            localBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetSKIFLevel");
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotalItem.Value = internalCirculatoinExtrude
                  ? "������ ������� ��� ���������� �������� "
                  : "������ ������� c ����������� ��������� ";
            gratuitousIncomesItem.Value = internalCirculatoinExtrude
                  ? "������������� ����������� ��� ���������� �������� "
                  : "������������� ����������� c ����������� ��������� ";

            budgetLevel.Value = budgetLevelDigest.GetMemberUniqueName(ComboBudgetLevel.SelectedValue).Replace("[������ ��������].[������ ��������]", "[������ ��������]");

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            if (IncomesList.SelectedIndex == 0)
            {
                string query = DataProvider.GetQueryText("FNS_0001_0009_compare_grid1");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�����", dtGrid);
                comboOKVED.Visible = false;
                ComboKD.Visible = true;
            } else
            {
                string query = DataProvider.GetQueryText("FNS_0001_0009_compare_grid2");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "��", dtGrid);
                comboOKVED.Visible = true;
                ComboKD.Visible = false;
            }
            /*if (dtGrid.Rows.Count > 0)
            {
                for (int k = 0; k < dtGrid.Rows.Count; k++)
                {
                    DataRow row = dtGrid.Rows[k];
                    string kd = dtGrid.Rows[k][0].ToString();
                    kd = TrimName(kd);
                    dtGrid.Rows[k][0] = kd;
                }
            }*/
            UltraWebGrid.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //if (UltraWebGrid != null && UltraWebGrid.Rows.Count < 30)��� ���������
            //{
            //    UltraWebGrid.Height = Unit.Empty;
            //}
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(240);
            if (IncomesList.SelectedIndex == 0)
            {
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(110);
            }
            else
            {
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(160);
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(235);
            }
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 1;
            for (int k = 2; k < e.Layout.Bands[0].Columns.Count; k++)
            {

                string formatString = "N2";
                e.Layout.Bands[0].Columns[k].Width = 126;
                if ((k == 3) || (k == 5) || (k == 6) || (k == 8) || (k == 9))
                {
                    formatString = "P2";
                    if (IncomesList.SelectedIndex == 0)
                    {
                        e.Layout.Bands[0].Columns[k].Width = 88;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[k].Width = 84;
                    }
                }
                e.Layout.Bands[0].Columns[k].Format = formatString;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
            }
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        DateTime currDateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue.ToString()), CRHelper.MonthNum(ComboMonth.SelectedValue), 01);
            string date = String.Format("{0:dd.MM.yyyy}", currDateTime);
              currDateTime = currDateTime.AddMonths(1);
              e.Layout.Bands[0].Columns[1].Header.Caption = "��� ";
              currDateTime = currDateTime.AddYears(-2);
              date = String.Format("{0:dd.MM.yyyy}", currDateTime);
              e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("�� {0} ����, ���. ���.", date);
              e.Layout.Bands[0].Columns[3].Header.Caption = "����.���, %";
              currDateTime = currDateTime.AddYears(1);
              date = String.Format("{0:dd.MM.yyyy}", currDateTime);
              e.Layout.Bands[0].Columns[4].Header.Caption = string.Format("�� {0} ����, ���. ���.", date); ;
              e.Layout.Bands[0].Columns[5].Header.Caption = "����.���, %";
              e.Layout.Bands[0].Columns[6].Header.Caption =  string.Format("���� ����� {0} � {1},%", Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue.ToString()) - 1), Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue.ToString()) - 2));
              currDateTime = currDateTime.AddYears(1);
              date = String.Format("{0:dd.MM.yyyy}", currDateTime);
              e.Layout.Bands[0].Columns[7].Header.Caption = string.Format("�� {0} ����, ���. ���.", date);
              e.Layout.Bands[0].Columns[8].Header.Caption = "����.���, %";
              e.Layout.Bands[0].Columns[9].Header.Caption = string.Format("���� ����� {0} � {1},%", Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue.ToString())), Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue.ToString()) - 1));

              if (IncomesList.SelectedIndex == 0)
              {
                  e.Layout.Bands[0].Columns[0].Header.Title = "������� � ���������� ��������������� �������������� ����� ������������� ������������";
                  e.Layout.Bands[0].Columns[1].Header.Title = "��� ������� ��������������� �������������� ����� ������������� ������������";
                  e.Layout.Bands[0].Columns[2].Header.Title = "��������� ������� �� ����������� ������ ������������ ����";
                  e.Layout.Bands[0].Columns[3].Header.Title = "�������� ��� ���� ������������� ������������ � ����� ������ ����������� � ����������� ����";
                  e.Layout.Bands[0].Columns[4].Header.Title = "��������� ������� �� ����������� ������ �������� ����";
                  e.Layout.Bands[0].Columns[5].Header.Title = "�������� ��� ���� ������������� ������������ � ����� ������ ����������� � ������� ����";
                  e.Layout.Bands[0].Columns[6].Header.Title = "���� ����� ����������� � ������������ ������� ����������� ����";
                  e.Layout.Bands[0].Columns[7].Header.Title = "��������� ������� � ������� ����";
                  e.Layout.Bands[0].Columns[8].Header.Title = "�������� ��� ���� ������������� ������������ � ����� ������ ����������� � ������� ����";
                  e.Layout.Bands[0].Columns[9].Header.Title = "���� ����� ����������� � ������������ ������� ����������� ����";
              }
              else
              {
                  e.Layout.Bands[0].Columns[0].Header.Title = "������, ��������� � ������ �������";
                  e.Layout.Bands[0].Columns[1].Header.Title = "��� ������, ���������, ������ �������";
                  e.Layout.Bands[0].Columns[2].Header.Title = "��������� ������� �� ����������� ������ ������������ ����";
                  e.Layout.Bands[0].Columns[3].Header.Title = "�������� ��� ��������� ��������� � ����� ������ ����������� � ����������� ����";
                  e.Layout.Bands[0].Columns[4].Header.Title = "��������� ������� �� ����������� ������ �������� ����";
                  e.Layout.Bands[0].Columns[5].Header.Title = "�������� ��� ��������� ��������� � ����� ������ ����������� � ������� ����";
                  e.Layout.Bands[0].Columns[6].Header.Title = "���� ����� ����������� � ������������ ������� ����������� ����";
                  e.Layout.Bands[0].Columns[7].Header.Title = "��������� ������� � ������� ����";
                  e.Layout.Bands[0].Columns[8].Header.Title = "�������� ��� ��������� ��������� � ����� ������ ����������� � ������� ����";
                  e.Layout.Bands[0].Columns[9].Header.Title = "���� ����� ����������� � ������������ ������� ����������� ����";
              }

        }


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 9; i < e.Row.Cells.Count; i += 3)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "�������� �������";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "���� �������";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].Value != null &&
                           (e.Row.Cells[0].Value.ToString().Contains("��� ������")))
                {
                    e.Row.Cells[0].Value = "������� ������� ������������� �������";
                }
                if (e.Row.Cells[0].Value != null &&
                           (e.Row.Cells[0].Value.ToString().Contains("������")))
                {
                    e.Row.Cells[0].Value = "����������� ������ �������������� ������";
                }
                
            }
            int levelColumnIndex = 10;
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[levelColumnIndex] != null && e.Row.Cells[levelColumnIndex].Value.ToString() != string.Empty)
                {
                    string level = e.Row.Cells[levelColumnIndex].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "(All)":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "������":
                        case "������":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "���������":
                        case "���������":
                            {
                                fontSize = 10;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                }
            }
        }

        private static string TrimName(string name)
        {
            while (Char.IsDigit(name[0]))
            {
                name = name.Remove(0, 1);
            }
            return name;
        }

        #endregion

        #region ������� � Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            string label = Label2.Text.Replace("<br/>", "");
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 65;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Page.Title;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = label;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int width = 150;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 30 * 3;
            e.CurrentWorksheet.Columns[1].Width = width * 30;
            e.CurrentWorksheet.Columns[2].Width = width * 30;
            e.CurrentWorksheet.Columns[3].Width = width * 30;
            e.CurrentWorksheet.Columns[4].Width = width * 30;
            e.CurrentWorksheet.Columns[5].Width = width * 30;
            e.CurrentWorksheet.Columns[6].Width = width * 30;
            e.CurrentWorksheet.Columns[7].Width = width * 30;
            e.CurrentWorksheet.Columns[8].Width = width * 30;
            e.CurrentWorksheet.Columns[9].Width = width * 30;
            e.CurrentWorksheet.Columns[10].Width = width * 30;
            e.CurrentWorksheet.Columns[11].Width = width * 30;
            e.CurrentWorksheet.Columns[12].Width = width * 30;
            e.CurrentWorksheet.Columns[13].Width = width * 30;
            e.CurrentWorksheet.Columns[14].Width = width * 30;

            int columnCountt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCountt; i = i + 1)
            {
                e.CurrentWorksheet.Rows[6].Cells[i].CellFormat.FillPattern = FillPatternStyle.None;
            }
            e.CurrentWorksheet.Rows[3].Height = e.CurrentWorksheet.Rows[3].Height * 2 + 100;
            e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
            int columnCounttt = UltraWebGrid.Columns.Count;
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[1].CellFormat.Alignment = HorizontalCellAlignment.Center;
            for (int i = 2; i < columnCounttt; i = i + 1)
            {
                if ((i == 3) || (i == 5) || (i == 6) || (i == 8) || (i == 9) || (i == 10) || (i == 13) || (i == 14) || (i == 15))
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00%;[Red]-#,##0.00%";
                }
                else
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                }
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }

            for (int k = 1; k < columnCounttt; k = k + 1)
            {
                e.CurrentWorksheet.Rows[3].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
  
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("������");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.GridElementCaption = Label2.Text;
            UltraGridExporter1.HeaderChildCellHeight = 60;
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
        }

        #endregion
        private void FullComboOKVED()
        {
            Dictionary<string, int> cods = new Dictionary<string, int>();
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("OKVEDList");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);
            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    cods.Add(row[0].ToString(), 0);
                }
            }
            comboOKVED.FillDictionaryValues(cods);
        }
        public int sts { get; set; }
    }

         
}
