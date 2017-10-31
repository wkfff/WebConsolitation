using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

namespace Krista.FM.Server.Dashboards.reports.MO_SP_0002
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();
        private DataTable dtGrid1 = new DataTable();
        private int endYear = 2011;
        private GridHeaderLayout headerLayout;
        private GridHeaderLayout headerLayout1;
        private CustomParam selectedYears { get { return (UserParams.CustomParam("selectedYears")); } }
        private CustomParam parent_indicators;
        private CustomParam indicators;
        private string[] gridHeaders;
         
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 2 / 3);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 2 / 3);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            HyperLink1.NavigateUrl = "~/reports/MO_SP_0001/default.aspx";
            HyperLink1.Text = "Сравнение&nbsp;населенных&nbsp;пунктов&nbsp;по&nbsp;показателям&nbsp;сэр";
        }



        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            indicators = UserParams.CustomParam("indicators");
            parent_indicators = UserParams.CustomParam("parent_indicators");
            ComboIndicators.MultiSelect = true;
            FillComboIndicators();

            ComboYear.MultiSelect = true;
            FillComboYear();
            UserParams.Filter.Value = "Город Ханты-Мансийск";

            FillComboMo();

            gridTable_0.Visible = false;
            gridTable_.Visible = false;
            if (!Page.IsPostBack)
            {
                Table2.Visible = false;
                FillComboTypeGrid();

                ComboYear.Title = "Год";
                ComboYear.Width = 200;


                ComboMo.Title = "Выберите населенный пункт";
                ComboMo.Width = 400;
                ComboMo.MultiSelect = false;

                ComboIndicators.Title = "Выберите показатели";
                ComboIndicators.Width = 650;
                ComboIndicators.ParentSelect = true;
                ComboIndicators.ShowSelectedValue = true;
            }
            else
            {
                Table2.Visible = true;
                gridTable_0.Visible = ComboTypeTable.SelectedValue != "Годовые данные";

                gridTable_.Visible = !gridTable_0.Visible;
                
            }

            CRHelper.SaveToErrorLog(ComboMo.SelectedValue);
            UserParams.Filter.Value = moNameDictionary[ComboMo.SelectedValue];


            string toolType = String.Empty;
            string parentIndicators = String.Empty;


           /* foreach (Infragistics.WebUI.UltraWebNavigator.Node node in ComboIndicators.SelectedNodes)
            {
                if (node.Level == 0)
                {
                    foreach (Infragistics.WebUI.UltraWebNavigator.Node node1 in node.Nodes)
                    {
                        if (indicatorLevelDictionary[node1.Text] == 1)
                        {
                            toolType = String.Format("{0},{1}, {1}.Children", toolType, indicatorNameDictionary[node1.Text]);
                            addedIndicators.Add(node1.Text);
                        }
                        else
                        {
                            if (!addedIndicators.Contains(childParentDictionary[node1.Text]))
                            {

                                toolType = String.Format("{0},{1}, {2}", toolType, childParentDictionary[node1.Text], indicatorNameDictionary[node1.Text]);
                                addedIndicators.Add(childParentDictionary[node1.Text]);
                            }
                            else
                            {
                                toolType = String.Format("{0},{1}", toolType, indicatorNameDictionary[node1.Text]);
                            }
                        }
                    }
                
                }
                else
                {
                    if (indicatorLevelDictionary[node.Text] == 1)
                    {
                        toolType = String.Format("{0},{1}, {1}.Children", toolType, indicatorNameDictionary[node.Text]);
                        addedIndicators.Add(node.Text);
                    }
                    else
                    {   
                        if (!addedIndicators.Contains(childParentDictionary[node.Text]))
                        {

                            toolType = String.Format("{0},{1}, {2}", toolType, childParentDictionary[node.Text], indicatorNameDictionary[node.Text]);
                            addedIndicators.Add(childParentDictionary[node.Text]);
                        }
                        else
                        {
                            toolType = String.Format("{0},{1}", toolType, indicatorNameDictionary[node.Text]);
                        }
                    }
                }
            }*/
            foreach (Infragistics.WebUI.UltraWebNavigator.Node node in ComboIndicators.SelectedNodes)
            {
                if (node.Level == 0)
                {
                    foreach (Infragistics.WebUI.UltraWebNavigator.Node node1 in node.Nodes)
                    {
                        if (indicatorLevelDictionary[node1.Text] == 1)
                        {
                            toolType = String.Format("{0},{1}", toolType, indicatorNameDictionary[node1.Text]);
                            addedIndicators.Add(node1.Text);
                        }
                        else
                        {
                            if (!addedIndicators.Contains(childParentDictionary[node1.Text]))
                            {

                                toolType = String.Format("{0},{1}, {2}", toolType, childParentDictionary[node1.Text], indicatorNameDictionary[node1.Text]);
                                addedIndicators.Add(childParentDictionary[node1.Text]);
                            }
                            else
                            {
                                toolType = String.Format("{0},{1}", toolType, indicatorNameDictionary[node1.Text]);
                            }
                        }
                    }
                }
                else
                {
                    if (indicatorLevelDictionary[node.Text] == 1)
                    {
                        toolType = String.Format("{0},{1}", toolType, indicatorNameDictionary[node.Text]);
                        addedIndicators.Add(node.Text);
                    }
                    else
                    {
                        if (indicatorLevelDictionary[node.Text] == 2)
                        {
                            toolType = String.Format("{0},{2}", toolType, indicatorNameDictionary[node.Parent.Text], indicatorNameDictionary[node.Text]);
                            addedIndicators.Add(node.Text);
                            parentIndicators = String.Format("{0},{1}", parentIndicators, indicatorNameDictionary[node.Parent.Text]);
                        }
                        else
                        {
                            if (!addedIndicators.Contains(childParentDictionary[node.Text]))
                            {

                                toolType = String.Format("{0}, {2}", toolType, childParentDictionary[node.Text], indicatorNameDictionary[node.Text]);
                                addedIndicators.Add(childParentDictionary[node.Text]);
                                parentIndicators = String.Format("{0},{1}", parentIndicators, childParentDictionary[node.Text]);
                            }
                            else
                            {
                                toolType = String.Format("{0},{1}", toolType, indicatorNameDictionary[node.Text]);
                            }
                        }
                    }
                }
            }
            parentIndicators = parentIndicators.TrimStart(',');
            if (parentIndicators != String.Empty)
            {
                parent_indicators.Value = parentIndicators + ",";
            }
            else
            { 
                parent_indicators.Value = parentIndicators;
            }
            toolType = toolType.TrimStart(',');
            indicators.Value = toolType;

            string yearListQuater = String.Empty;
            string yearList = String.Empty;

            int k = 0;
            if (ComboYear.SelectedNodes.Count != 0)
            {
                if (ComboYear.SelectedNodes[0].Text == "Выбрать все")
                {
                    gridHeaders = new string[ComboYear.SelectedNodes[0].Nodes.Count];
                }
                else
                {
                    gridHeaders = new string[ComboYear.SelectedValues.Count];
                }
            }
            else
            {
                gridHeaders = new string[ComboYear.SelectedValues.Count];
            }
                foreach (Infragistics.WebUI.UltraWebNavigator.Node node in ComboYear.SelectedNodes)
                {
                    if (node.Level == 0)
                    {
                        foreach (Infragistics.WebUI.UltraWebNavigator.Node node1 in node.Nodes)
                        {
                            yearList += String.Format("{0}", yearNameDictionary[node1.Text]) + ".[Полугодие 1].[Квартал 1],";
                            yearList += String.Format("{0}", yearNameDictionary[node1.Text]) + ".[Полугодие 1].[Квартал 2],";
                            yearList += String.Format("{0}", yearNameDictionary[node1.Text]) + ".[Полугодие 2].[Квартал 3],";
                            yearList += String.Format("{0}", yearNameDictionary[node1.Text]) + ".[Полугодие 2].[Квартал 4],";
                            gridHeaders[k] = node1.Text;
                            k += 1;
                            yearListQuater = String.Format("{0},{1}", yearListQuater, yearNameDictionary[node1.Text]);

                        }
                    }
                    else
                    {
                        yearList += String.Format("{0}", yearNameDictionary[node.Text]) + ".[Полугодие 1].[Квартал 1],";
                        yearList += String.Format("{0}", yearNameDictionary[node.Text]) + ".[Полугодие 1].[Квартал 2],";
                        yearList += String.Format("{0}", yearNameDictionary[node.Text]) + ".[Полугодие 2].[Квартал 3],";
                        yearList += String.Format("{0}", yearNameDictionary[node.Text]) + ".[Полугодие 2].[Квартал 4],";
                        gridHeaders[k] = node.Text;
                        k += 1;
                        yearListQuater = String.Format("{0},{1}", yearListQuater, yearNameDictionary[node.Text]);
                    }
                }
            
            if (yearList.Length > 1)
            {
                yearList = yearList.Remove(yearList.Length - 1);
                yearListQuater = yearListQuater.Remove(0, 1);
            }
            UserParams.PeriodYear.Value = yearList;

            PageTitle.Text = "Паспорт населенного пункта";
            Page.Title = PageTitle.Text;
            Label1.Text = LoadHtmls(ComboMo.SelectedValue);
            PageSubTitle.Text = String.Empty;

            if (Page.IsPostBack)
            {
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
                UltraWebGrid.Columns.Clear();
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
                UltraWebGrid1.Columns.Clear();
                UltraWebGrid1.Bands.Clear();
                UserParams.PeriodYearQuater.Value = yearListQuater;
                UltraWebGrid1.DataBind();
                                
                if (dtGrid.Rows.Count < 20)
                {
                    UltraWebGrid.Height = Unit.Empty;                   
                }
                if (UltraWebGrid1.Rows.Count < 20)
                {
                    UltraWebGrid1.Height = Unit.Empty;
                }
                PageSubTitle.Text = String.Format("Сравнительная оценка показателей социально-экономического развития населенного пункта, {0}", ComboMo.SelectedValue);
                if (ComboTypeTable.SelectedIndex == 0 && Page.IsPostBack)
                {
                    try
                    {
                        UltraWebGrid.Columns[1].Hidden = true;
                    }
                    catch { }

                }
                if (ComboTypeTable.SelectedIndex == 1 && Page.IsPostBack)
                {
                    try
                    {
                        UltraWebGrid1.Columns[1].Hidden = true;
                    }
                    catch { }
                }
            }
           
            //UltraWebGrid.Visible = CheckBoxQuartGrid.Checked;
            //UltraWebGrid1.Visible = CheckBoxYearGrid.Checked;
            //gridTable_.Visible = CheckBoxYearGrid.Checked;
            //gridTable_0.Visible = CheckBoxQuartGrid.Checked;
        }

        private void FillComboTypeGrid()
        {
            Dictionary<string, int> _ = new Dictionary<string, int>();
            _.Add("Квартальные данные", 0);
            _.Add("Годовые данные", 0);
            ComboTypeTable.FillDictionaryValues(_);

            ComboTypeTable.Title = "Выберите периодичность";

            ComboTypeTable.Width = 400;
        }

        #region Заполнение комбиков
        private Dictionary<string, string> indicatorNameDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> childParentDictionary = new Dictionary<string, string>();
        private Dictionary<string, int> indicatorLevelDictionary = new Dictionary<string, int>();
        private Collection<string> addedIndicators = new Collection<string>();

        private void FillComboIndicators()
        {
            DataTable dtIndicators = new DataTable();
            string query = DataProvider.GetQueryText("MO_0003_0001_indicators");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtIndicators);
            indicatorLevelDictionary.Add("Выбрать все",0);
            for (int i = 0; i < dtIndicators.Rows.Count; i++)
            {
                int level = 0;

                switch (dtIndicators.Rows[i]["Уровень"].ToString())
                {
                    case "Уровень 1":
                        {
                            level = 1;
                            break;
                        }
                    case "Уровень 2":
                        {
                            level = 2;
                            break;
                        }
                    case "Уровень 3":
                        {
                            level = 3;
                            break;
                        }
                    case "Уровень 4":
                        {
                            level = 4;
                            break;
                        }
                }
                childParentDictionary.Add(dtIndicators.Rows[i]["Показатель"].ToString(), dtIndicators.Rows[i]["Юник нейм родителя"].ToString());
                indicatorLevelDictionary.Add(dtIndicators.Rows[i]["Показатель"].ToString(), level);
                indicatorNameDictionary.Add(dtIndicators.Rows[i]["Показатель"].ToString(), dtIndicators.Rows[i]["Юник нейм"].ToString());
            }
            if (!Page.IsPostBack)
            {
                ComboIndicators.FillDictionaryValues(indicatorLevelDictionary);
            }
        }

        private Dictionary<string, string> moNameDictionary;// = new Dictionary<string, string>();

        private void FillComboMo()
        {
            DataTable dtMo = new DataTable();
            string query = DataProvider.GetQueryText("MO_0003_0001_mo");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtMo);

            Dictionary<string, int> moLevelDictionary = new Dictionary<string, int>();
            moNameDictionary = new Dictionary<string, string>();

            string ParentName = "";
            string PrevParentName = "";

            for (int i = 0; i < dtMo.Rows.Count; i++)
            {
                ParentName = dtMo.Rows[i]["ParentName"].ToString();
                if (ParentName != PrevParentName)
                {
                    PrevParentName = ParentName;
                    moLevelDictionary.Add(ParentName, 0);        
                }

                moLevelDictionary.Add(dtMo.Rows[i]["Показатель"].ToString(), 1);

                CRHelper.SaveToErrorLog(dtMo.Rows[i]["Показатель"].ToString()+"|");

                moNameDictionary.Add(dtMo.Rows[i]["Показатель"].ToString(), dtMo.Rows[i]["UniqueName"].ToString());
            }
            ComboMo.ParentSelect = false;
            if (!Page.IsPostBack)
            {
                ComboMo.FillDictionaryValues(moLevelDictionary);
            }
        }

        private Dictionary<string, string> yearNameDictionary = new Dictionary<string, string>();

        private void FillComboYear()
        {
            DataTable dtYear = new DataTable();
            string query = DataProvider.GetQueryText("MO_0003_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtYear);

            Dictionary<string, int> yearLevelDictionary = new Dictionary<string, int>();
            yearLevelDictionary.Add("Выбрать все", 0);
            for (int i = 0; i < dtYear.Rows.Count; i++)
            {
                yearLevelDictionary.Add(dtYear.Rows[i]["Показатель"].ToString(), 1);
                yearNameDictionary.Add(dtYear.Rows[i]["Показатель"].ToString(), dtYear.Rows[i]["ДанныеНа"].ToString());
            }

            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(yearLevelDictionary);
            }
        }

        private string LoadHtmls(string subjectID)
        {
            string xmlFile = HttpContext.Current.Server.MapPath("~/reports/MO_SP_0002/Default.Settings.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                foreach (XmlNode regionNode in rootNode.ChildNodes)
                {
                    if (regionNode.Attributes["id"].Value == subjectID)
                    {
                        return regionNode.InnerText;
                    }
                }
            }
            return string.Empty;
        }
        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MO_0003_0001_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);
            bool flag = false;
            
            for (int i = 2; i < dtGrid.Columns.Count - 3; i++)
            {
                flag = true;
                for (int j = 0; j < dtGrid.Rows.Count - 1; j++)
                {
                    if (dtGrid.Rows[j][i] != DBNull.Value)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    dtGrid.Columns.Remove(dtGrid.Columns[i]);
                    i -= 1;
                }
            }

            for (int i = 0; i < dtGrid.Rows.Count-1; i += 3)
            {
                flag = false;
               
                if (int.Parse(dtGrid.Rows[i]["Нулл3"].ToString()) != 0 && (dtGrid.Rows[i]["Нулл"].ToString() == "Уровень 2" || dtGrid.Rows[i]["Нулл"].ToString() == "Уровень 3")
                    && (dtGrid.Rows[i][0].ToString().Split(';')[0] == "Среднемесячная заработная плата работников органов местного самоуправления" ||
                    dtGrid.Rows[i][0].ToString().Split(';')[0] == "Поголовье скота и птицы в сельскохозяйственных организациях (без субъектов малого предпринимательства с численностью до 60 человек)"
                    || dtGrid.Rows[i][0].ToString().Split(';')[0] == "Поголовье скота и птицы в сельскохозяйственных организациях (без субъектов малого предпринимательства с численностью до 60 человек)"
                    || dtGrid.Rows[i][0].ToString().Split(';')[0] == "Производство важнейших видов промышленной продукции"))
                {
                    flag = true;
                }
                if (flag)
                {
                    dtGrid.Rows.Remove(dtGrid.Rows[i]);
                    dtGrid.Rows.Remove(dtGrid.Rows[i]);
                    dtGrid.Rows.Remove(dtGrid.Rows[i]);
                    i -= 3;
                }
            }
            for (int i = 0; i < dtGrid.Rows.Count; i+=3 )
            {
                if (dtGrid.Rows[i][dtGrid.Columns.Count - 1] != DBNull.Value)
                {
                    dtGrid.Rows[i][0] = dtGrid.Rows[i][0].ToString().Insert(dtGrid.Rows[i][0].ToString().IndexOf(';'),", "+ dtGrid.Rows[i][dtGrid.Columns.Count - 1].ToString().ToLower());
                }
            }
            if (dtGrid.Rows.Count >= 5)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (dtGrid.Rows.Count>0)
            {

                e.Layout.GroupByBox.Hidden = true;
                e.Layout.HeaderStyleDefault.Wrap = true;
                e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
                e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
                e.Layout.AllowSortingDefault = AllowSorting.No;

                for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 3; i = i + 1)
                {
                    int widthColumn = CRHelper.GetColumnWidth(89);
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Grid.Columns[i], "N2");
                }
                int widthColumn1 = CRHelper.GetColumnWidth(310);
                e.Layout.Bands[0].Columns[0].Width = widthColumn1;
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                headerLayout.AddCell("Показатель");
                headerLayout.AddCell(" ");

                e.Layout.Bands[0].Grid.Columns[e.Layout.Bands[0].Grid.Columns.Count - 1].Hidden = true;
                e.Layout.Bands[0].Grid.Columns[e.Layout.Bands[0].Grid.Columns.Count - 2].Hidden = true;
                e.Layout.Bands[0].Grid.Columns[e.Layout.Bands[0].Grid.Columns.Count - 3].Hidden = true;
                int k = 0;
                for (int i = 2; i < e.Layout.Bands[0].Grid.Columns.Count - 3; i ++)
                {
                    GridHeaderCell headerCell = headerLayout.AddCell(dtGrid.Rows[dtGrid.Rows.Count - 1][i].ToString());

                    headerCell.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
                    if (dtGrid.Rows[dtGrid.Rows.Count - 1][i].ToString() == dtGrid.Rows[dtGrid.Rows.Count - 1][i + 1].ToString())
                    {
                        headerCell.AddCell(e.Layout.Bands[0].Columns[i + 1].Header.Caption);
                        if (dtGrid.Rows[dtGrid.Rows.Count - 1][i].ToString() == dtGrid.Rows[dtGrid.Rows.Count - 1][i + 2].ToString())
                        {
                            headerCell.AddCell(e.Layout.Bands[0].Columns[i + 2].Header.Caption);
                            if (dtGrid.Rows[dtGrid.Rows.Count - 1][i].ToString() == dtGrid.Rows[dtGrid.Rows.Count - 1][i + 3].ToString())
                            {
                                headerCell.AddCell(e.Layout.Bands[0].Columns[i + 3].Header.Caption);
                                i += 3;
                            }
                            else
                            {
                                i += 2;
                            }
                        }
                        else
                        {
                            i += 1;
                        }
                    }
                    else
                    {
                        // i += 1;
                    }
                }

                headerLayout.ApplyHeaderInfo();
                dtGrid.Rows.Remove(dtGrid.Rows[dtGrid.Rows.Count - 1]);
            }
            else
            {
                UltraWebGrid.Bands.Clear();
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (dtGrid.Rows.Count > 0)
            {
                if (e.Row.Cells[0].Text.EndsWith("Отклонение"))
                {
                    if (UltraWebGrid.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 3].Value != null &&
                        UltraWebGrid.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 3].Value.ToString() == "Уровень 1")
                    {
                        UltraWebGrid.Rows[e.Row.Index - 2].Style.Font.Bold = true;
                        UltraWebGrid.Rows[e.Row.Index - 1].Hidden = true;
                        e.Row.Hidden = true;
                        UltraWebGrid.Rows[e.Row.Index - 2].Cells[0].Text = UltraWebGrid.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0];
                        UltraWebGrid.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = System.Drawing.Color.White;
                        for (int i = 1; i < e.Row.Cells.Count - 3; i++)
                        {
                            UltraWebGrid.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = System.Drawing.Color.White;
                            UltraWebGrid.Rows[e.Row.Index - 2].Cells[i].Value = null;
                        }
                        UltraWebGrid.Rows[e.Row.Index - 2].Cells[1].Text = "";
                    }
                    else
                    {
                        UltraWebGrid.Rows[e.Row.Index - 2].Cells[0].Text = UltraWebGrid.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0];
                        e.Row.Cells[0].Text = "";
                        UltraWebGrid.Rows[e.Row.Index - 1].Cells[0].Text = "";
                        UltraWebGrid.Rows[e.Row.Index - 2].Cells[0].RowSpan = 3;

                      //  e.Row.Cells[0].Cells[0].Style.BorderDetails.WidthBottom = 0;
                        UltraWebGrid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.WidthBottom = 0;

                        UltraWebGrid.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = System.Drawing.Color.White;
                        UltraWebGrid.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = System.Drawing.Color.White;
                        e.Row.Cells[0].Style.BackColor = System.Drawing.Color.White;

                        UltraWebGrid.Rows[e.Row.Index - 2].Cells[1].Style.BackColor = System.Drawing.Color.White;
                        UltraWebGrid.Rows[e.Row.Index - 1].Cells[1].Style.BackColor = System.Drawing.Color.White;
                        e.Row.Cells[1].Style.BackColor = System.Drawing.Color.White;

                        UltraWebGrid.Rows[e.Row.Index - 1].Cells[1].Text = "Темп роста";
                        e.Row.Cells[1].Text = "Отклонение";
                        UltraWebGrid.Rows[e.Row.Index - 2].Cells[1].Text = "Значение";


                        UltraWebGrid.Rows[e.Row.Index - 1].Cells[0].Style.Padding.Left = 20;
                        for (int i = 1; i < e.Row.Cells.Count - 3; i++)
                        {

                            UltraWebGrid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.WidthBottom = 0;
                            UltraWebGrid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.WidthBottom = 0;
                            UltraWebGrid.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = System.Drawing.Color.White;
                            UltraWebGrid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = System.Drawing.Color.White;
                            e.Row.Cells[i].Style.BackColor = System.Drawing.Color.White;

                            if (UltraWebGrid.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 3].Value != null &&
                                 UltraWebGrid.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 3].Value.ToString() == "Уровень 1")
                            {
                                UltraWebGrid.Rows[e.Row.Index - 2].Cells[i].Style.Font.Bold = true;
                                UltraWebGrid.Rows[e.Row.Index - 2].Cells[i + 1].Style.Font.Bold = true;
                                UltraWebGrid.Rows[e.Row.Index - 2].Cells[i + 2].Style.Font.Bold = true;

                                UltraWebGrid.Rows[e.Row.Index - 2].Cells[i].Value = String.Empty;

                            } 
                            else
                            {
                                bool direct = !(UltraWebGrid.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 2].Value != null &&
                                          UltraWebGrid.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 2].Value.ToString() == "1");
                                SetConditionArrow(UltraWebGrid.Rows[e.Row.Index - 1], i, 1, direct);
                            }
                            if (e.Row.Cells[i].Value != null)
                            {
                                e.Row.Cells[i].Title = "Прирост к предыдущему кварталу";
                            }
                            if (UltraWebGrid.Rows[e.Row.Index - 1].Cells[i].Value != null)
                            {
                                UltraWebGrid.Rows[e.Row.Index - 1].Cells[i].Title = "Темп роста к предыдущему кварталу";
                            }
                        }
                    }
                }
            }
        }

        public static void SetConditionArrow(UltraGridRow row, int index, int borderValue, bool direct)
        {
            double value;
            if (row.Cells[index] != null &&
                row.Cells[index].Value != null &&
                double.TryParse(row.Cells[index].Value.ToString(),out value))
            {
                string img = string.Empty;
                if (direct)
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                    }
                    else if (value < borderValue)
                    {
                        img = "~/images/arrowRedDownBB.png";
                    }
                }
                else
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowRedUpBB.png";
                    }
                    else if (value < borderValue)
                    {
                        img = "~/images/arrowGreenDownBB.png";
                    }
                }
                row.Cells[index].Style.BackgroundImage = img;
                row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
                if ((row.Cells[index].Value != null))
                {
                   // row.Cells[index].Title = "Темп роста к предыдущему кварталу";
                    row.Cells[index].Value = String.Format("{0:P2}", Convert.ToDouble(row.Cells[index].Value));
                }
            }
        }

        #endregion

        #region Обработчики второго грида
        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            UltraWebGrid1.Bands.Clear();
            string query = DataProvider.GetQueryText("MO_0003_0001_grid1");
            
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid1);
            bool flag = false;
            for (int i = 2; i < dtGrid1.Columns.Count - 3; i++)
            {
                flag = true;
                for (int j = 0; j < dtGrid1.Rows.Count - 1; j++)
                {
                    if (dtGrid1.Rows[j][i] != DBNull.Value)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    dtGrid1.Columns.Remove(dtGrid1.Columns[i]);
                    i -= 1;
                }
            }
            for (int i = 0; i < dtGrid1.Rows.Count; i += 3)
            {
                flag = false;

                if (int.Parse(dtGrid1.Rows[i]["Нулл3"].ToString()) != 0 && (dtGrid1.Rows[i]["Нулл"].ToString() == "Уровень 2" || dtGrid1.Rows[i]["Нулл"].ToString() == "Уровень 3")
                    && (dtGrid1.Rows[i][0].ToString().Split(';')[0] == "Среднемесячная заработная плата работников органов местного самоуправления" ||
                    dtGrid1.Rows[i][0].ToString().Split(';')[0] == "Поголовье скота и птицы в сельскохозяйственных организациях (без субъектов малого предпринимательства с численностью до 60 человек)"
                    || dtGrid1.Rows[i][0].ToString().Split(';')[0] == "Поголовье скота и птицы в сельскохозяйственных организациях (без субъектов малого предпринимательства с численностью до 60 человек)"
                    || dtGrid1.Rows[i][0].ToString().Split(';')[0] == "Производство важнейших видов промышленной продукции"))
                {
                    flag = true;
                }
                if (flag)
                {
                    dtGrid1.Rows.Remove(dtGrid1.Rows[i]);
                    dtGrid1.Rows.Remove(dtGrid1.Rows[i]);
                    dtGrid1.Rows.Remove(dtGrid1.Rows[i]);
                    i -= 3;
                }
            }
            for (int i = 0; i < dtGrid1.Rows.Count; i += 3)
            {
                if (dtGrid1.Rows[i][dtGrid1.Columns.Count - 1] != DBNull.Value)
                {
                    dtGrid1.Rows[i][0] = dtGrid1.Rows[i][0].ToString().Insert(dtGrid1.Rows[i][0].ToString().IndexOf(';'), ", " + dtGrid1.Rows[i][dtGrid1.Columns.Count - 1].ToString().ToLower());
                }
            }
            if (dtGrid1.Rows.Count >= 5)
            {
                UltraWebGrid1.DataSource = dtGrid1;
            }
            else
            {
                UltraWebGrid1.DataSource = null;
            }
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (dtGrid1.Rows.Count > 0)
            {
                e.Layout.GroupByBox.Hidden = true;
                e.Layout.HeaderStyleDefault.Wrap = true;
                e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
                e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
                e.Layout.AllowSortingDefault = AllowSorting.No;
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
                {
                    int widthColumn = CRHelper.GetColumnWidth(89);
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                } 
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(310);
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                headerLayout1.AddCell("Показатель");
                headerLayout1.AddCell("");
                e.Layout.Bands[0].Grid.Columns[e.Layout.Bands[0].Grid.Columns.Count - 1].Hidden = true;
                e.Layout.Bands[0].Grid.Columns[e.Layout.Bands[0].Grid.Columns.Count - 2].Hidden = true;
                e.Layout.Bands[0].Grid.Columns[e.Layout.Bands[0].Grid.Columns.Count - 3].Hidden = true;
                for (int i = 2; i < e.Layout.Bands[0].Grid.Columns.Count - 3; i++)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Grid.Columns[i], "N2");
                    GridHeaderCell headerCell = headerLayout1.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
                }

                headerLayout1.ApplyHeaderInfo();
            }
            else
            {
                UltraWebGrid1.Bands.Clear();
            }
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
           
                if (dtGrid1.Rows.Count > 0)
                {
                    if (e.Row.Cells[0].Text.EndsWith("Отклонение"))
                    {


                        if (UltraWebGrid1.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 3].Value != null &&
                            UltraWebGrid1.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 3].Value.ToString() == "Уровень 1")
                        {
                            UltraWebGrid1.Rows[e.Row.Index - 2].Style.Font.Bold = true;
                            UltraWebGrid1.Rows[e.Row.Index - 1].Hidden = true;
                            e.Row.Hidden = true;
                            UltraWebGrid1.Rows[e.Row.Index - 2].Cells[0].Text = UltraWebGrid1.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0];
                            UltraWebGrid1.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = System.Drawing.Color.White;
                            for (int i = 1; i < e.Row.Cells.Count - 3; i++)
                            {
                                UltraWebGrid1.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = System.Drawing.Color.White;
                                UltraWebGrid1.Rows[e.Row.Index - 2].Cells[i].Value = null;
                            }

                            UltraWebGrid1.Rows[e.Row.Index - 2].Cells[1].Text = "";

                        }
                        else
                        {
                            UltraWebGrid1.Rows[e.Row.Index - 2].Cells[0].Text = UltraWebGrid1.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0];
                            UltraWebGrid1.Rows[e.Row.Index - 2].Cells[0].RowSpan = 3;
                            e.Row.Cells[0].Text = "";
                            UltraWebGrid1.Rows[e.Row.Index - 1].Cells[0].Text = "";

                            UltraWebGrid1.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.WidthBottom = 0;
                            UltraWebGrid1.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = System.Drawing.Color.White;
                            UltraWebGrid1.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = System.Drawing.Color.White;
                            e.Row.Cells[0].Style.BackColor = System.Drawing.Color.White;

                            UltraWebGrid1.Rows[e.Row.Index - 2].Cells[1].Style.BackColor = System.Drawing.Color.White;
                            UltraWebGrid1.Rows[e.Row.Index - 1].Cells[1].Style.BackColor = System.Drawing.Color.White;
                            e.Row.Cells[1].Style.BackColor = System.Drawing.Color.White;

                            UltraWebGrid1.Rows[e.Row.Index - 1].Cells[1].Text = "Темп роста";
                            e.Row.Cells[1].Text = "Отклонение";
                            UltraWebGrid1.Rows[e.Row.Index - 2].Cells[1].Text = "Значение";


                            UltraWebGrid1.Rows[e.Row.Index - 1].Cells[0].Style.Padding.Left = 20;
                            for (int i = 2; i < e.Row.Cells.Count - 3; i++)
                            {

                                UltraWebGrid1.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.WidthBottom = 0;
                                UltraWebGrid1.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.WidthBottom = 0;
                                UltraWebGrid1.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = System.Drawing.Color.White;
                                UltraWebGrid1.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = System.Drawing.Color.White;
                                e.Row.Cells[i].Style.BackColor = System.Drawing.Color.White;

                                if (UltraWebGrid1.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 3].Value != null &&
                                     UltraWebGrid1.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 3].Value.ToString() == "Уровень 1")
                                {
                                    UltraWebGrid1.Rows[e.Row.Index - 2].Cells[i].Style.Font.Bold = true;
                                    UltraWebGrid1.Rows[e.Row.Index - 2].Cells[i + 1].Style.Font.Bold = true;
                                    UltraWebGrid1.Rows[e.Row.Index - 2].Cells[i + 2].Style.Font.Bold = true;

                                    UltraWebGrid1.Rows[e.Row.Index - 2].Cells[i].Value = String.Empty;

                                }
                                else
                                {
                                    bool direct = !(UltraWebGrid1.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 2].Value != null &&
                                              UltraWebGrid1.Rows[e.Row.Index - 2].Cells[e.Row.Cells.Count - 2].Value.ToString() == "1");
                                    SetConditionArrow(UltraWebGrid1.Rows[e.Row.Index - 1], i, 1, direct);
                                }
                                if (e.Row.Cells[i].Value != null)
                                {
                                    if (e.Row.PrevRow.PrevRow.Cells[i].Value != null)
                                    {
                                        e.Row.Cells[i].Title = "Прирост к предыдущему году";
                                    }
                                    else
                                    {
                                        e.Row.Cells[i].Value = null;
                                    }
                                }
                                if (UltraWebGrid1.Rows[e.Row.Index - 1].Cells[i].Value != null)
                                {
                                    UltraWebGrid1.Rows[e.Row.Index - 1].Cells[i].Title = "Темп роста к предыдущему году";
                                }
                            }
                        }
                    }
                }
         
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
           
                ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
                ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text ;

                Workbook workbook = new Workbook();
                Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
              //  
               
                if (ComboTypeTable.SelectedIndex == 0)
                {
                    UltraWebGrid.Columns[1].Hidden = false;
                    ReportExcelExporter1.Export(headerLayout, sheet1, 3);
                }
                else
                {
                    UltraWebGrid1.Columns[1].Hidden = false;
                    ReportExcelExporter1.Export(headerLayout1, sheet1, 3);
                }
            
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
         //   e.Workbook.Worksheets["Таблица1"].Rows[1].Height = 500;
         //   e.Workbook.Worksheets["Таблица2"].Rows[0].Cells[0].Value = null;
         //   e.Workbook.Worksheets["Таблица2"].Rows[1].Cells[0].Value = null;
        }
        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            TransformGrid();
            if (ComboTypeTable.SelectedIndex == 0)
            {
                UltraWebGrid.Columns[1].Hidden = false;
                ReportPDFExporter1.Export(headerLayout, section1);
            }
            else
            {
                UltraWebGrid1.Columns[1].Hidden = false;
                ReportPDFExporter1.Export(headerLayout1, section1);
            }
        }


        private void TransformGrid()
        {
            for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
            {
                UltraWebGrid.Rows[i].Cells[0].Style.BorderStyle = BorderStyle.None;
            }
            for (int i = 0; i < UltraWebGrid1.Rows.Count; i += 3)
            {
                UltraWebGrid1.Rows[i].Cells[0].Style.BorderStyle = BorderStyle.None;
            }
        }
        #endregion

        protected void CheckBoxYearGrid_CheckedChanged(object sender, EventArgs e)
        {
            //UltraWebGrid.Visible = CheckBoxQuartGrid.Checked;
            //UltraWebGrid1.Visible = CheckBoxYearGrid.Checked;
            //gridTable_.Visible = CheckBoxYearGrid.Checked;
            //gridTable_0.Visible = CheckBoxQuartGrid.Checked;

            //information_.Visible = CheckBoxInfo.Checked;
        }
    }
}