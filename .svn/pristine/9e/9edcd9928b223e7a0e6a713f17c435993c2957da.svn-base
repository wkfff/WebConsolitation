using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;

using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls
{
	public partial class HandMasterForSingleRecord : Form
	{

		private string AssociationName = string.Empty;

		private IWorkplace masterWorplace = null;

		private DataRow[] masterClsDataRows = null;

		private DataRow masterClsBridgeRow = null;

		private IBridgeAssociation masterAssociation;

        public HandMasterForSingleRecord(IBridgeAssociation CurentAssociation, IWorkplace currentWorkplace, DataRow[] clsDataRow, DataRow clsBridgeRow)
		{
			InitializeComponent();
			masterAssociation = CurentAssociation;
			AssociationName = masterAssociation.FullDBName;
			masterWorplace = currentWorkplace;
            masterClsDataRows = clsDataRow;
			masterClsBridgeRow = clsBridgeRow;
            cbApplyToAllRecords.Enabled = CurentAssociation.AssociationClassType != AssociationClassTypes.BridgeBridge;
            cbApplyToAllRecords.Checked = CurentAssociation.AssociationClassType != AssociationClassTypes.BridgeBridge;
			//masterConversionTableName = masterAssociation.FullName;

			foreach (IAssociateRule rule in masterAssociation.AssociateRules.Values)
			{
                lbAssociationRules.Items.Add(new AssociationRuleItem(rule.ObjectKey, rule.Name));
			}
			if (lbAssociationRules.Items.Count > 0)
				lbAssociationRules.SetSelected(0, true);

            bool addToConvert = currentWorkplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject(CurentAssociation.ObjectKey,
                                                                                      (int)AssociateOperations.AddRecordIntoBridgeTable,
                                                                                      false);
            if (!addToConvert)
            {
                cbAddToTraslTable.Enabled = false;
                cbAddToTraslTable.Checked = false;
            }
		}

		private void panel6_Paint(object sender, PaintEventArgs e)
		{

		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
            masterClsDataRows = null;
			masterClsBridgeRow = null;
			masterWorplace = null;
			this.Close();
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
            foreach (DataRow clsDataRow in masterClsDataRows)
            {
		        string masterConversionTableName;
                if (lbAssociationRules.Items.Count > 0)
                {
                    masterConversionTableName = masterAssociation.ObjectKey + '.' +
                        ((AssociationRuleItem)lbAssociationRules.SelectedItem).Key;
                }
                else
                    masterConversionTableName = masterAssociation.ObjectKey;

                clsDataRow[AssociationName] = masterClsBridgeRow["ID"];
                if (cbApplyToAllRecords.Checked)
                    ApplyToAllsimilarRows(clsDataRow);
                else
                    ApplyOnlyThisRow(clsDataRow);
                
                if (cbAddToTraslTable.Checked)
                    try
                    {
                        masterWorplace.ActiveScheme.ConversionTables[masterConversionTableName].AddConversion(clsDataRow.ItemArray, masterClsBridgeRow.ItemArray);
                    }
                    catch 
                    {
                        throw;
                    }
            }
            masterClsDataRows = null;
			masterClsBridgeRow = null;
			Close();
		}

        /// <summary>
        /// сопоставление только текущей записи без поиска таких же в других источниках
        /// </summary>
        /// <param name="clsDataRow"></param>
        private void ApplyOnlyThisRow(DataRow clsDataRow)
        {
            // построение запроса на сопоставление текущей записи
            IDatabase db = masterWorplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                IDbDataParameter[] dbParams = new IDbDataParameter[1];
                dbParams[0] = new System.Data.OleDb.OleDbParameter(AssociationName, masterClsBridgeRow["ID"]);//db.CreateParameter(AssociationName, masterClsBridgeRow["ID"]);
                // получение параметров фильтра
                string updateQuery = string.Format("update {0} set {1} where {2}", masterAssociation.RoleData.FullDBName,
                    string.Format("{0} = ?", AssociationName), string.Format("ID = {0}", clsDataRow["ID"]));
                db.ExecQuery(updateQuery, QueryResultTypes.NonQuery, dbParams);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// сопоставление подобных записей по всем источникам
        /// </summary>
        /// <param name="row"></param>
        private void ApplyToAllsimilarRows(DataRow row)
        {
            // получение списка полей, по которым будем искать эти подобные записи
            // в них идут обычные поля, которые обязательны для заполнения
            Dictionary<string, object> rowParams = new Dictionary<string, object>();
            foreach (IDataAttribute attr in masterAssociation.RoleData.Attributes.Values)
            {
                DataAttributeKindTypes attrkind = attr.Kind;
                string attrName = attr.Name;
                if (attrName == masterAssociation.FullDBName)
                    continue;
                switch (attrkind)
                {
                    case DataAttributeKindTypes.Regular:
                        if (!attr.IsNullable)
                            rowParams.Add(attrName, row[attrName]);
                        break;
                }
            }

            // построение запроса на обновление записей
            IDatabase db = masterWorplace.ActiveScheme.SchemeDWH.DB;
            try
            {
                string[] paramsSigns = new string[rowParams.Count];
                IDbDataParameter[] dbParams = new IDbDataParameter[rowParams.Count + 1];
                int i = 0;
                dbParams[i] = new System.Data.OleDb.OleDbParameter(AssociationName, masterClsBridgeRow["ID"]); //db.CreateParameter(AssociationName, masterClsBridgeRow["ID"]);
                // получение параметров фильтра
                foreach (KeyValuePair<string, object> val in rowParams)
                {
                    paramsSigns[i] = string.Format("({0} = ?)", val.Key);
                    dbParams[i + 1] = new System.Data.OleDb.OleDbParameter(val.Key, val.Value);//db.CreateParameter(val.Key, val.Value);
                    i++;
                }
                string updateQuery = string.Format("update {0} set {1} where {2}", masterAssociation.RoleData.FullDBName,
                    string.Format("{0} = ?", AssociationName), string.Join("and", paramsSigns));
                db.ExecQuery(updateQuery, QueryResultTypes.NonQuery, dbParams);
            }
            finally
            {
                db.Dispose();
            }
        }
	}
}