using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.ServerLibrary;

using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls
{
	public class AssociatedClsUI : Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls.BaseClsUI
	{
        public AssociatedClsUI(IEntity dataObject)
            : this(dataObject, dataObject.ObjectKey)
		{
		}

        public AssociatedClsUI(IEntity dataObject, string key)
            : base(dataObject, key)
        {
            Index = 2;
            Caption = "Сопоставимые классификаторы";
            clsClassType = ClassTypes.clsBridgeClassifier;
        }

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.cls_Bridge_16.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Bridge_16; }
		}

		public override System.Drawing.Image TypeImage24
		{
			get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Bridge_24; }
		}

        public override bool HasDataSources()
        {
            return true;
        }

        protected override void SetViewObjectCaption()
        {
            if (ActiveDataObj == null)
                return;
            Workplace.ViewObjectCaption = string.Format("{0}: {1}", "Сопоставимый классификатор", ActiveDataObj.OlapName);
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();

            allowAddRecord = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllAssociatedClassifiersOperations.AddRecord, false);
            allowChangeHierarchy = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllAssociatedClassifiersOperations.ChangeClassifierHierarchy, false);
            allowClearClassifier = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllAssociatedClassifiersOperations.ClearClassifierData, false);
            allowDelRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllAssociatedClassifiersOperations.DelRecord, false);
            allowEditRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllAssociatedClassifiersOperations.EditRecord, false);
            allowImportClassifier = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllAssociatedClassifiersOperations.ImportClassifier, false);
            allowSetHierarchy = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllAssociatedClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier, false);

            if (!allowAddRecord)
                allowAddRecord = um.CheckPermissionForSystemObject(currentClassifierName, (int)AssociatedClassifierOperations.AddRecord, false);
            if (!allowChangeHierarchy)
                allowChangeHierarchy = um.CheckPermissionForSystemObject(currentClassifierName, (int)AssociatedClassifierOperations.ChangeClassifierHierarchy, false);
            if (!allowClearClassifier)
                allowClearClassifier = um.CheckPermissionForSystemObject(currentClassifierName, (int)AssociatedClassifierOperations.ClearClassifierData, false);
            if (!allowDelRecords)
                allowDelRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)AssociatedClassifierOperations.DelRecord, false);
            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)AssociatedClassifierOperations.EditRecord, false);
            if (!allowImportClassifier)
                allowImportClassifier = um.CheckPermissionForSystemObject(currentClassifierName, (int)AssociatedClassifierOperations.ImportClassifier, false);
            if (!allowSetHierarchy)
                allowSetHierarchy = um.CheckPermissionForSystemObject(currentClassifierName, (int)AssociatedClassifierOperations.SetHierarchyAndCodeDisintegrationForClassifier, false);

            if (!allowAddRecord && !allowChangeHierarchy && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;
        }

        public override void SetPermissionsToClassifier(UltraGridEx gridEx)
        {
            UltraToolbarsManager utm = gridEx.utmMain;
            ButtonTool btnSetHierarchy = null;
            try
            {
                btnSetHierarchy = (ButtonTool)utm.Tools["SetHierarchy"];
            }
            catch { }
            // настройка прав на очистку классификатора
            if (allowClearClassifier)
            {
                gridEx.AllowClearTable = true;
            }
            else
            {
                gridEx.AllowClearTable = false;
            }
            // настройка прав на установку иерархии и ращепление кода
            if (allowSetHierarchy)
            {
                if (btnSetHierarchy != null)
                    btnSetHierarchy.SharedProps.Enabled = true;
            }
            else
            {
                if (btnSetHierarchy != null)
                    btnSetHierarchy.SharedProps.Enabled = false;
            }

            if (!allowAddRecord && !allowChangeHierarchy && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;

            if (viewOnly)
            {
                gridEx.IsReadOnly = true;//SetComponentToState(UltraGridEx.ComponentStates.readonlyState);
            }
            else
                gridEx.IsReadOnly = false;
            // настройка прав на добавление записей в классификатор
            if (allowAddRecord)
            {
                gridEx.AllowAddNewRecords = true;
            }
            else
            {
                gridEx.AllowAddNewRecords = false;
            }
            // настройка прав на изменение иерархии в отдельных записях
            if (allowChangeHierarchy)
            {
                allowChangeHierarchy = true;
            }
            else
            {
                allowChangeHierarchy = false;
            }
            // настройка прав на удаление записей из классификатора
            if (allowDelRecords)
            {
                gridEx.AllowDeleteRows = true;
            }
            else
            {
                gridEx.AllowDeleteRows = false;
            }
            // настройка прав на редактирование классификатора
            if (allowEditRecords)
            {
                gridEx.AllowEditRows = true;
            }
            else
            {
                gridEx.AllowEditRows = false;
            }
            // настройка прав на импортирование классификатора их XML
            if (allowImportClassifier)
            {
                gridEx.AllowImportFromXML = true;
            }
            else
            {
                gridEx.AllowImportFromXML = false;
            }

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
                            String.Format("and ({0} = {1})", hi.ParentRefClmnName, (int)parentID));
                }
            }

            string checkRecCountSQL = string.Format("select count(ID) from {0} where {1}", ActiveDataObj.FullDBName, dataQuery);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object recCount = db.ExecQuery(checkRecCountSQL, QueryResultTypes.Scalar);
                AllRecordsCount = Convert.ToInt32(recCount);
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
                        CurrentRecordsCount = MaxFactTableRecordCount;
                    }
                    else
                    {
                        CurrentRecordsCount = Convert.ToInt32(recCount);
                    }
                    filterStr = dataQuery;
                    vo.ugeCls.ServerFilterEnabled = true;
                    return ActiveDataObj.GetDataUpdater(dataQuery, MaxFactTableRecordCount, ParametersListToArray(FilterParameters));
                }
                
                CurrentRecordsCount = AllRecordsCount = Convert.ToInt32(recCount);
                vo.ugeCls.ServerFilterEnabled = false;
            }
            return base.GetActiveUpdater(parentID, out filterStr);
        }

        public override bool EnableServerFilter()
        {
            return vo.ugeCls.ServerFilterEnabled;
        }
	}
}