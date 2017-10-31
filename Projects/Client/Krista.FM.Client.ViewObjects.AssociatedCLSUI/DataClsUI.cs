using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win;

using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls
{
	public class DataClsUI : Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls.BaseClsUI
	{
        public DataClsUI(IEntity dataObject)
            : this(dataObject, dataObject.ObjectKey)
		{
		}

        public DataClsUI(IEntity dataObject, string key)
            : base(dataObject, key)
        {
            Index = 0;
            Caption = "Классификаторы данных";
            clsClassType = ClassTypes.clsDataClassifier;
        }

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.cls_Data_16.GetHicon()); }
        }
        
        public override System.Drawing.Image TypeImage16
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Data_16; }
		}

		public override System.Drawing.Image TypeImage24
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Data_24; }
		}

        public override bool HasDataSources()
        {
            return ((IClassifier)ActiveDataObj).IsDivided;
        }

        protected override void SetViewObjectCaption()
        {
            if (ActiveDataObj == null)
                return;
            Workplace.ViewObjectCaption = string.Format("{0}: {1}", "Классификатор данных", GetClsRusName());
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();

            
            allowAddRecord = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllDataClassifiersOperations.AddRecord, false);
            allowChangeHierarchy = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllDataClassifiersOperations.ChangeClassifierHierarchy, false);
            allowClearClassifier = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllDataClassifiersOperations.ClearClassifierData, false);
            allowDelRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllDataClassifiersOperations.DelRecord, false);
            allowEditRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllDataClassifiersOperations.EditRecord, false);
            allowImportClassifier = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllDataClassifiersOperations.ImportClassifier, false);
            allowSetHierarchy = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllDataClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier, false);
            allowAddNewDataSource = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllDataClassifiersOperations.AddClassifierForNewDataSource, false);

            if (!allowAddRecord)
                allowAddRecord = um.CheckPermissionForSystemObject(currentClassifierName, (int)DataClassifiesOperations.AddRecord, false);
            if (!allowChangeHierarchy)
                allowChangeHierarchy = um.CheckPermissionForSystemObject(currentClassifierName, (int)DataClassifiesOperations.ChangeClassifierHierarchy, false);
            if (!allowClearClassifier)
                allowClearClassifier = um.CheckPermissionForSystemObject(currentClassifierName, (int)DataClassifiesOperations.ClearClassifierData, false);
            if (!allowDelRecords)
                allowDelRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)DataClassifiesOperations.DelRecord, false);
            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)DataClassifiesOperations.EditRecord, false);
            if (!allowImportClassifier)
                allowImportClassifier = um.CheckPermissionForSystemObject(currentClassifierName, (int)DataClassifiesOperations.ImportClassifier, false);
            if (!allowSetHierarchy)
                allowSetHierarchy = um.CheckPermissionForSystemObject(currentClassifierName, (int)DataClassifiesOperations.SetHierarchyAndCodeDisintegrationForClassifier, false);
            if (!allowAddNewDataSource)
                allowAddNewDataSource = um.CheckPermissionForSystemObject(currentClassifierName, (int)DataClassifiesOperations.AddClassifierForNewDataSource, false);

            if (!allowAddRecord && !allowChangeHierarchy && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;
        }

        public override void  SetPermissionsToClassifier(UltraGridEx gridEx)
        {
            UltraToolbarsManager utm = gridEx.utmMain;
            ButtonTool btnSetHierarchy = null;

            if (utm.Tools.Exists("SetHierarchy"))
            {
                btnSetHierarchy = (ButtonTool)utm.Tools["SetHierarchy"];
            }

            // настройка прав на очистку классификатора
            gridEx.AllowClearTable = allowClearClassifier;

            // настройка прав на установку иерархии и ращепление кода
            if (btnSetHierarchy != null)
            {
                btnSetHierarchy.SharedProps.Enabled = allowSetHierarchy;
            }

            if (!allowAddRecord && !allowChangeHierarchy && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;

            gridEx.IsReadOnly = viewOnly;

            // настройка прав на добавление записей в классификатор
            gridEx.AllowAddNewRecords = allowAddRecord;

            // настройка прав на удаление записей из классификатора
            gridEx.AllowDeleteRows = allowDelRecords;

            // настройка прав на редактирование классификатора
            gridEx.AllowEditRows = allowEditRecords;

            // настройка прав на импортирование классификатора их XML
            gridEx.AllowImportFromXML = allowImportClassifier;

            gridEx.SetComponentSettings();
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            dataQuery = string.Empty;
            // получаем интерфейс поставщика данных
            HierarchyInfo hi = vo.ugeCls.HierarchyInfo;
            dataQuery = String.Concat("(RowType = 0) and ", GetDataSourcesFilter());
            AddFilter();
            if (hi.ObjectHierarchyType == Components.HierarchyType.ParentChild)
            {
                // для первой загрузки больших иерархических классификаторов выбираем только записи верхнего уровня
                if (hi.loadMode == LoadMode.OnParentExpand)
                {
                    if (parentID == null)
                        dataQuery = String.Concat(dataQuery, String.Format("and ({0} is null)", hi.ParentRefClmnName));
                    else
                        dataQuery = String.Concat(dataQuery,
                            String.Format("and ({0} = {1})", hi.ParentRefClmnName, (int) parentID));
                }
            }

            string checkRecCountSQL = string.Format("select count(ID) from {0} where {1}", ActiveDataObj.FullDBName, dataQuery);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object recCount = db.ExecQuery(checkRecCountSQL, QueryResultTypes.Scalar);
                if (Convert.ToInt32(recCount) > MaxFactTableRecordCount)
                {
                    bool showMessage = true;
                    if (vo.ugeCls.ServerFilterEnabled)
                    {
                        string userFilter;
                        List<UltraGridEx.FilterParamInfo> serverFilterParameters;
                        vo.ugeCls.BuildServerFilter(out userFilter, out serverFilterParameters);
                        FilterParameters = serverFilterParameters;
                        if (!string.IsNullOrEmpty(userFilter))
                            dataQuery = String.Concat(dataQuery, " AND ", userFilter);

                        checkRecCountSQL = string.Format("select count(ID) from {0} where {1}", ActiveDataObj.FullDBName, dataQuery);
                        recCount = db.ExecQuery(checkRecCountSQL, QueryResultTypes.Scalar, ParametersListToArray(FilterParameters));
                        if (Convert.ToInt32(recCount) <= MaxFactTableRecordCount)
                        {
                            showMessage = false;
                        }
                    }
                    vo.ugeCls.ServerFilterEnabled = true;
                    if (showMessage)
                    {
                        //  если записей слищком много - спрашиваем пользователя хочет ли он видеть первые 5000
                        string messageStr = String.Concat("Слишком большой объект выборки ({0}).", Environment.NewLine,
                                                          "Могут быть показаны только {1} записей.");
                        messageStr = String.Format(messageStr, recCount, MaxFactTableRecordCount);
                        // если нет - формируем фильтр таким образом чтобы
                        MessageBox.Show(messageStr, "Информационное сообщение", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    filterStr = dataQuery;
                    return ActiveDataObj.GetDataUpdater(dataQuery, MaxFactTableRecordCount, ParametersListToArray(FilterParameters));
                }
            }
            vo.ugeCls.ServerFilterEnabled = false;

            return base.GetActiveUpdater(parentID, out filterStr);
        }

        public override bool EnableServerFilter()
        {
            return vo.ugeCls.ServerFilterEnabled;
        }
	}
}