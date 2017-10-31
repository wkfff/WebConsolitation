using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Dashboards.reports.IP_0001_0003
{
    public partial class Default : CustomReportPage
    {

        private IDatabase db;

        private CustomParam parentID;
        private CustomParam projectID;
        private CustomParam typeI;

        private static Dictionary<string, string> dictTerr;

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

            parentID = UserParams.CustomParam("parent_id");
            projectID = UserParams.CustomParam("project_id");
            typeI = UserParams.CustomParam("type_i");
            
            Page.Title = PageTitle.Text = String.Format("Предлагаемые к реализации приоритетные инвестиционные проекты");
                        
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
                if (!Page.IsPostBack)
                {
                    ComboTerr.Title = "Место реализации";
                    ComboTerr.ParentSelect = true;
                    ComboTerr.MultiSelect = false;
                    ComboTerr.Width = 650;
                    FillTerrDictionary(ComboTerr);
                    
                    if (ComboTerr.GetRootNodesCount() == 0)
                    {
                        db.Dispose();
                        return;
                    }
                }

                string setString = String.Empty;
                GetSetParam(ComboTerr.SelectedNode, ref setString);
                parentID.Value = setString;

                Page.Title = PageTitle.Text = String.Format("Предлагаемые к реализации приоритетные инвестиционные проекты, {0}", ComboTerr.SelectedValue);
                
                UltraWebGrid1.DataBind();
            }
            finally
            {
                db.Dispose();
            }

        }

        private void GetSetParam(Node node, ref string setString)
        {
            if (String.IsNullOrEmpty(setString))
                setString = GetTerrID(node.Text);
            else
                setString += ", " + GetTerrID(node.Text);
            foreach (Node childNode in node.Nodes)
                GetSetParam(childNode, ref setString);
        }

        #region Грид

        private void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("Ид. №", typeof(string));
            dtGrid.Columns.Add("Наименование проекта", typeof(string));
            dtGrid.Columns.Add("Предполагаемое место реализации", typeof(string));
            dtGrid.Columns.Add("Цель проекта", typeof(string));
            dtGrid.Columns.Add("Основные технико-экономические параметры проекта", typeof(string));
            dtGrid.Columns.Add("Инициатор/куратор проекта (контактная информация)", typeof(string));

            string query = DataProvider.GetQueryText("IP_0001_0003_grid");
            DataTable dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            foreach (DataRow row in dtData.Rows)
            {
                DataRow newRow = dtGrid.NewRow();

                newRow["Ид. №"] = row["Code"];
                newRow["Наименование проекта"] = String.Format("<a href='../IP_0001_0004/Default.aspx?paramlist=project_id={0}'>{1}</a>",
                    HttpContext.Current.Server.UrlEncode(row["ID"].ToString()), row["Name"]);
                newRow["Предполагаемое место реализации"] = row["Terr"];
                newRow["Цель проекта"] = row["Goal"];

                string baseParams = String.Empty;
                if (row["ExpectedOutput"] != DBNull.Value)
                    baseParams = row["ExpectedOutput"].ToString();
                if (row["Production"] != DBNull.Value)
                    baseParams = String.IsNullOrEmpty(baseParams) ? row["Production"].ToString() : String.Format("{0},<br/>{1}", baseParams, row["Production"]);
                if (row["PaybackPeriod"] != DBNull.Value)
                    baseParams = String.IsNullOrEmpty(baseParams) ? row["PaybackPeriod"].ToString() : String.Format("{0},<br/>{1}", baseParams, row["PaybackPeriod"]);
                newRow["Основные технико-экономические параметры проекта"] = baseParams;

                string contact = String.Empty;
                if (row["InvestorName"] != DBNull.Value)
                    contact = row["InvestorName"].ToString();
                if (row["LegalAddress"] != DBNull.Value)
                    contact = String.IsNullOrEmpty(contact) ? row["LegalAddress"].ToString() : String.Format("{0},<br/>{1}", contact, row["LegalAddress"]);
                if (row["Email"] != DBNull.Value)
                    contact = String.IsNullOrEmpty(contact) ? row["Email"].ToString() : String.Format("{0},<br/>{1}", contact, row["Email"]);
                if (row["Phone"] != DBNull.Value)
                    contact = String.IsNullOrEmpty(contact) ? row["Phone"].ToString() : String.Format("{0},<br/>{1}", contact, row["Phone"]);
                if (row["Contact"] != DBNull.Value)
                    contact = String.IsNullOrEmpty(contact) ? row["Contact"].ToString() : String.Format("{0},<br/>{1}", contact, row["Contact"]);
                newRow["Инициатор/куратор проекта (контактная информация)"] = contact;

                dtGrid.Rows.Add(newRow);
            }

            (sender as UltraWebGrid).DataSource = dtGrid;

        }

        private void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;
            
            UltraGridBand band = grid.Bands[0];
            band.Columns[0].Width = Unit.Parse("100px");
            band.Columns[1].Width = Unit.Parse("300px");
            band.Columns[2].Width = Unit.Parse("150px");
            band.Columns[3].Width = Unit.Parse("150px");
            band.Columns[4].Width = Unit.Parse("150px");
            band.Columns[5].Width = Unit.Parse("150px");

            foreach (UltraGridColumn c in band.Columns)
                c.CellStyle.Wrap = true;

            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowDeleteDefault = AllowDelete.No;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;

            GridHeaderLayout headerLayout = new GridHeaderLayout(grid);
            headerLayout.AddCell("Ид. №");
            headerLayout.AddCell("Наименование проекта");
            headerLayout.AddCell("Предполагаемое место реализации");
            headerLayout.AddCell("Цель проекта");
            headerLayout.AddCell("Основные технико-экономические параметры проекта");
            headerLayout.AddCell("Инициатор/куратор проекта (контактная информация)");
            headerLayout.ApplyHeaderInfo();

        }

        #endregion

        private string GetTerrID(string terrName)
        {
            string result;
            if (!dictTerr.TryGetValue(terrName, out result))
                return null;
            else
                return result;
        }

        private void FillTerrDictionary(CustomMultiCombo combo)
        {
            string query = DataProvider.GetQueryText("IP_0001_0003_hmao");
            DataTable dtHmao = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dictTerr = new Dictionary<string, string>();
            AddTerritoryToDict(dtHmao.Rows[0]["Name"].ToString(), dtHmao.Rows[0]["ID"].ToString(), 0, 0, dict);
            combo.FillDictionaryValues(dict);
            combo.SetСheckedState("Ханты-Мансийский автономный округ - Югра", true);
        }

        private void AddTerritoryToDict(string name, string id, int projects, int level, Dictionary<string, int> dict)
        {
            parentID.Value = id;
            string query = DataProvider.GetQueryText("IP_0001_0003_sub_terr");
            DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            Dictionary<string, int> newDict = new Dictionary<string, int>();
            foreach (DataRow row in dt.Rows)
                AddTerritoryToDict(row["Name"].ToString(), row["ID"].ToString(), Convert.ToInt32(row["Projects"]), level + 1, newDict);
            if (projects > 0 || newDict.Count > 0)
            {
                dict.Add(name, level);
                dictTerr.Add(name, id);
                foreach (string key in newDict.Keys)
                {
                    int value;
                    newDict.TryGetValue(key, out value);
                    dict.Add(key, value);
                }
            }
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
