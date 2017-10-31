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

namespace Krista.FM.Server.Dashboards.reports.IP_0001_0001
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

            Page.Title = PageTitle.Text = String.Format("Реализуемые приоритетные инвестиционные проекты");
                        
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

                Page.Title = PageTitle.Text = String.Format("Реализуемые приоритетные инвестиционные проекты, {0}", ComboTerr.SelectedValue);
                
                UltraWebGrid1.DataBind();
            }
            finally
            {
                db.Dispose();
            }

        }

        private void GetSetParam(Node node, ref string setString)
        {
            if (node == null)
                return;
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
            dtGrid.Columns.Add("Место реализации", typeof(string));
            dtGrid.Columns.Add("Год начала/окончания реализации", typeof(string));
            dtGrid.Columns.Add("Плановые объемы инвестиций, тыс.р.", typeof(double));
            dtGrid.Columns.Add("Плановые объемы господдержки, тыс.р.", typeof(double));

            string query = DataProvider.GetQueryText("IP_0001_0001_grid");
            DataTable dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            foreach (DataRow row in dtData.Rows)
            {
                DataRow newRow = dtGrid.NewRow();

                newRow["Ид. №"] = row["Code"];
                //newRow["Наименование проекта"] = row["Name"];
                newRow["Наименование проекта"] = String.Format("<a href='../IP_0001_0002/Default.aspx?paramlist=project_id={0}'>{1}</a>",
                    HttpContext.Current.Server.UrlEncode(row["ID"].ToString()), row["Name"]);
                newRow["Место реализации"] = row["Terr"];
                newRow["Год начала/окончания реализации"] = String.Format("{0} - {1}", row["BeginYear"], row["EndYear"]);

                projectID.Value = row["ID"].ToString();
                typeI.Value = "1";
                query = DataProvider.GetQueryText("IP_0001_0001_sum");
                newRow["Плановые объемы инвестиций, тыс.р."] = db.ExecQuery(query, QueryResultTypes.Scalar) ?? DBNull.Value;
                typeI.Value = "2";
                query = DataProvider.GetQueryText("IP_0001_0001_sum");
                newRow["Плановые объемы господдержки, тыс.р."] = db.ExecQuery(query, QueryResultTypes.Scalar) ?? DBNull.Value;

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
            band.Columns[1].CellStyle.Wrap = true;
            band.Columns[2].CellStyle.Wrap = true;
            band.Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            CRHelper.FormatNumberColumn(band.Columns[4], "N2");
            CRHelper.FormatNumberColumn(band.Columns[5], "N2");

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
            headerLayout.AddCell("Место реализации");
            headerLayout.AddCell("Год начала/окончания реализации");
            headerLayout.AddCell("Плановые объемы инвестиций, тыс.р.");
            headerLayout.AddCell("Плановые объемы господдержки, тыс.р.");
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
            string query = DataProvider.GetQueryText("IP_0001_0001_hmao");
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
            string query = DataProvider.GetQueryText("IP_0001_0001_sub_terr");
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
