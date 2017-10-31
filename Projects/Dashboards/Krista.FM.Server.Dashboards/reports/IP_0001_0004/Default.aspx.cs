using System;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.SessionState;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Drawing;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.Shared;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using System.Text.RegularExpressions;
using Infragistics.UltraChart.Shared.Events;
using Krista.FM.Common;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Data.Series;

namespace Krista.FM.Server.Dashboards.reports.IP_0001_0004
{
    public partial class Default : CustomReportPage
    {

        private IDatabase db;

        private CustomParam projectID;

        private static Dictionary<string, string> dictProject;
        private static Dictionary<string, string> dictProjectRev;

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

            HeaderTable.Width = String.Format("{0}px", (int)UltraWebGrid1.Width.Value);

            #region Гриды

            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            
            #endregion

            projectID = UserParams.CustomParam("project_id", true);

            Page.Title = PageTitle.Text = String.Format("Карточка приоритетного инвестиционного проекта");
                        
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
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
                }

                projectID.Value = GetProjectID(ComboProject.SelectedValue);

                UltraWebGrid1.DataBind();
            }
            finally
            {
                db.Dispose();
            }

        }

        #region Грид 1

        private void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string[] param = { "Предполагаемое место реализации", "Инвестор:", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;юридический адрес", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;почтовый адрес",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;электронная почта", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;телефон",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;контактное лицо", "Цель проекта", "Технико-экономические показатели:",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;планируемые результаты", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;виды конечной продукции",
                                 "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;срок окупаемости", "Степень проработки проекта",
                                 "Предполагаемый полезный социально-экономический эффект от реализации проекта", "Вид деятельности" };
            string query = DataProvider.GetQueryText("IP_0001_0004_grid1");
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
                newRow["Значение"] = row[i + 2];
                dtGrid.Rows.Add(newRow);
            }

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
            string query = DataProvider.GetQueryText("IP_0001_0004_project");
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
