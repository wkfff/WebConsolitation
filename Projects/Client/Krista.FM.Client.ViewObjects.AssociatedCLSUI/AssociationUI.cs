using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Infragistics.Excel;
using Krista.FM.Client.Workplace.Gui;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association
{
    public partial class AssociationUI : BaseViewObj
    {
        // ������� ����������� �������������
        private AssociationView associateView;
        // ������� ����������
        private IBridgeAssociation curentAssociation = null;
        // ������� ����� � ������ ������ �������������
        private UltraToolbarsManager toolBar = null;
        private UltraToolbarsManager bridgeToolBar = null;
        // ����������, �������� �� �������������� 
        private bool IsattClsDataHierarchy = true;
        // ������� �������������� ������
        //attClsData attClsData = null;
        private IInplaceClsView attClsData = null;
        private UltraGrid clsDataGrid = null;
        // ������� ������������� ��������������
        //attClsData attClsBridge = null;
        private IInplaceClsView attClsBridge = null;
        private UltraGrid clsBridgeGrid = null;
        // ������ ��� �������������� ������
        private object FilterCondition = string.Empty;
        // ��������� �������� �� �������������
        private object[] activeAssociationParams = null;

        private const int developerRowDiapazon = 1000000000;

        private string associationName = string.Empty;

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.cls_Associate_16.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Associate_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Krista.FM.Client.ViewObjects.AssociatedCLSUI.Properties.Resources.cls_Associate_24; }
        }


        public AssociationUI(string key)
            : base(key)
        {
            Index = 3;
            Caption = "������������� ���������������";
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new AssociationView();
            associateView = (AssociationView)fViewCtrl;
        }

        /// <summary>
        ///  �������������
        /// </summary>
        public override void Initialize()
        {
            curentAssociation = GetActiveAssociation();

            base.Initialize();

            associateView.utbmAssociate.ToolClick += utbmAssociate_ToolClick;
            associateView.rbAllRecords.CheckedChanged += rbAllRecords_CheckedChanged;
            associateView.rbCurrentAssociate.CheckedChanged += rbCurrentAssociate_CheckedChanged;
            associateView.rbUnAssociate.CheckedChanged += rbUnAssociate_CheckedChanged;
            associateView.ultraTabControl1.ActiveTabChanged += ultraTabControl1_ActiveTabChanged;
            associateView.rbAllRecords.Checked = true;

            ViewCtrl.Text = curentAssociation.FullCaption;

            ReloadData();
        }

        /// <summary>
        /// ��������� ������� � ������� "��������"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void utbRefreshData_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "RefreshData":
                    ReloadData();
                    break;
                default:
                    break;
            }
        }

        private string captionString;

        /// <summary>
        /// �������� ������ �� �������� �������������
        /// </summary>
        private void HideNoAssociationRef()
        {
            List<string> refClmns = new List<string>();
            // �������� ��� ������ �������������� �� ������������
            foreach (IDataAttribute attr in attClsData.ActiveDataObj.Attributes.Values)
            {
                if (attr.Class == DataAttributeClassTypes.Reference)
                    refClmns.Add(attr.Name);
            }
            // ���������� ������� �� ������� �� ������� ������������ �������������
            associationName = curentAssociation.FullDBName;

            string associateColumnName = associationName + UltraGridEx.LOOKUP_COLUMN_POSTFIX;
            if (!attClsData.UltraGridExComponent.utmMain.Tools.Exists(associateColumnName))
                associateColumnName = associationName;
            ((StateButtonTool)attClsData.UltraGridExComponent.utmMain.Tools[associateColumnName]).Checked = true;
        }

        void rbUnAssociate_CheckedChanged(object sender, EventArgs e)
        {
            ShowNoAssociate();
        }

        void ShowNoAssociate()
        {
            if (associateView.rbUnAssociate.Checked)
            {
                HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
                hi.CurViewState = ViewState.Flat;
                LoadMode loadMode = hi.loadMode;
                hi.loadMode = LoadMode.AllRows;
                ViewUnAssciatedRows();
                attClsData.RefreshAttachedData();
                attClsData.UltraGridExComponent.SetHierarchyFilter(UltraGridEx.FilterConditionAction.Clear);
                hi.loadMode = loadMode;
            }
        }

        void rbCurrentAssociate_CheckedChanged(object sender, EventArgs e)
        {
            ShowCurrentAssociate();
        }

        void ShowCurrentAssociate()
        {
            if (associateView.rbCurrentAssociate.Checked)
            {
                HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
                hi.CurViewState = ViewState.Flat;
                LoadMode loadMode = hi.loadMode;
                hi.loadMode = LoadMode.AllRows;
                ViewCurrentAssociatedRow();
                attClsData.RefreshAttachedData();
                attClsData.UltraGridExComponent.SetHierarchyFilter(UltraGridEx.FilterConditionAction.Clear);
                hi.loadMode = loadMode;
            }
        }

        /// <summary>
        ///  ���������� ������ �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rbAllRecords_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                //SetFilter();
                ShowAllRows();
            }
        }

        private void ShowAllRows()
        {
            HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
            hi.CurViewState = hi.LevelsCount == 1 ? ViewState.Flat : ViewState.Hierarchy;
            attClsData.AdditionalFilter = string.Empty;
            attClsData.RefreshAttachedData();
        }

        //private bool isSetFilter;

        /// <summary>
        ///  ��������� ������� � ����������� �� ���������� �������
        /// </summary>
        private void SetFilter()
        {
            /*
            StateButtonTool btn = (StateButtonTool)toolBar.Tools["ShowHierarchy"];
            // ���������, ����� �� ������� ������ ����������, �.�. ����� RadioButton ������
            // ������ �����������, ���������� ��� ������ � �������������� ������
            if (associateView.rbAllRecords.Checked)
            {
                // ������� �������������� ��������� �
                HierarchyInfo hi = attClsData.UltraGridExComponent.HierarchyInfo;
                btn.SharedProps.Enabled = hi.LevelsCount > 1;
                //FilterCondition = null;
                //clsDataGrid.DisplayLayout.Bands[0].ColumnFilters[curentAssociation.FullDBName].FilterConditions.Clear();
                isSetFilter = false;
            }
            // ������, ������� ���������� ���������������� ������
            if (associateView.rbUnAssociate.Checked)
            {
                //attClsData.UltraGridExComponent.SetFilter(curentAssociation.FullDBName, -1);
                isSetFilter = true;
            }
            // ������, ������� �� ������ � ������������ ���������� ������, ������� ��� ������������
            if (associateView.rbCurrentAssociate.Checked)
            {
                
                if (clsBridgeGrid.Rows.Count >= 0 && clsBridgeGrid.ActiveRow == null)
                    clsBridgeGrid.Rows[0].Activate();
                if (clsBridgeGrid.Rows.Count == 0)
                {
                    MessageBox.Show("��������� ������� ����������, ������������ ������������� ����", "��������������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    associateView.rbAllRecords.Checked = true;
                    return;
                }

                FilterCondition = clsBridgeGrid.ActiveRow.Cells["ID"].Value;
                attClsData.UltraGridExComponent.SetFilter(curentAssociation.FullDBName, FilterCondition);
                 * 
                isSetFilter = true;
            }*/
        }

        private void ViewUnAssciatedRows()
        {
            attClsData.AdditionalFilter = string.Format(" and {0} = -1", curentAssociation.FullDBName);
        }

        private void ViewCurrentAssociatedRow()
        {
            attClsData.AdditionalFilter = clsBridgeGrid.ActiveRow != null ?
                string.Format(" and {0} = {1}", curentAssociation.FullDBName, clsBridgeGrid.ActiveRow.Cells["ID"].Value) :
                string.Format(" and {0} is NULL", curentAssociation.FullDBName);
        }

        public override void Activate(bool Activated)
        {
            if (Activated)
            {
                Workplace.ViewObjectCaption = captionString;
                attClsData.TrySetDataSource(DataSourceContext.CurrentDataSourceID);
            }
        }

        /// <summary>
        ///  ��������� ������� ����������
        /// </summary>
        /// <returns></returns>
        private IBridgeAssociation GetActiveAssociation()
        {
            IEntityAssociation entityAssociation = Workplace.ActiveScheme.RootPackage.FindAssociationByName(Key);
            if (entityAssociation is IBridgeAssociation)
                return entityAssociation as IBridgeAssociation;
            throw new InvalidOperationException(String.Format("������ � ������ ObjectKey={0} �� ������������� ���� IBridgeAssociation.", entityAssociation.ObjectKey));
        }

        /// <summary>
        /// ���������� ���������� �������.
        /// </summary>
        public override void InitializeData()
        {
            #region ��������� ��������� ������ ������ � ��������������

            ButtonTool btn1 = (ButtonTool)associateView.utbmAssociate.Tools["CreateBridge"];
            btn1.SharedProps.Enabled = curentAssociation.MappingRuleExist;

            CheckPermissions();
            SetPervissionsToAssociation();

            btn1 = (ButtonTool)associateView.utbmAssociate.Tools["AddToBridge"];
            btn1.SharedProps.Enabled = curentAssociation.MappingRuleExist;

            btn1 = (ButtonTool) associateView.utbmAssociate.Tools["AddToBridgeAll"];
            btn1.SharedProps.Visible = curentAssociation is IBridgeAssociationReport ? true : false;

            #endregion


            // ��������� �������������� ��� ������� � ��������� �������������
            AttachClsClassifiers();

            attClsData.UltraGridExComponent.ugData.BeginUpdate();
            attClsBridge.UltraGridExComponent.ugData.BeginUpdate();
            // ��������� ������ � ���������������
            attClsData.RefreshAttachedData();
            attClsBridge.RefreshAttachedData();

            StateButtonTool btn = (StateButtonTool)toolBar.Tools["ShowHierarchy"];
            IsattClsDataHierarchy = btn.SharedProps.Enabled;
            // ���������� ���������� ���� � ���������������� ������� � �������������� ������
            SetAssociateCount();
            // �������������� ������ �� ��� ������
            associateView.rbAllRecords.Checked = true;
            // ���������� ������ �������� �������������
            captionString = string.Format("�������������: {0}.{1} -> {2}.{3}", CommonMethods.GetDataObjSemanticRus(this.Workplace.ActiveScheme.Semantics, curentAssociation.RoleData), curentAssociation.RoleData.Caption,
                CommonMethods.GetDataObjSemanticRus(this.Workplace.ActiveScheme.Semantics, curentAssociation.RoleBridge), curentAssociation.RoleBridge.Caption);
            this.Workplace.ViewObjectCaption = captionString;

            this.Workplace.OperationObj.Text = "������������� ������";
            this.Workplace.OperationObj.StartOperation();
            if (attClsData.UltraGridExComponent.ugData.Rows.Count > 0)
                attClsData.UltraGridExComponent.ugData.Rows[0].Activate();


            attClsData.UltraGridExComponent.ugData.EndUpdate();
            attClsBridge.UltraGridExComponent.ugData.EndUpdate();

            // ������ ��� ������ �� ������������ ����� ���, ������� ��������� � �������������
            HideNoAssociationRef();

            TranslationTablesPageLoad(associateView.ultraTabControl1.ActiveTab);

            this.Workplace.OperationObj.StopOperation();
        }

        #region ���������� ��������� ���������������

        /// <summary>
        /// ������� ��������� �� ����������� ��������������
        /// </summary>
        /// <param name="CLSUI"></param>
        private void ClearAttachedCLS(IInplaceClsView CLSUI)
        {
            CLSUI.DetachViewObject();
            CLSUI.FinalizeViewObject();
            CLSUI = null;
        }

        /// <summary>
        ///  ��������� ��������������� � �������, ����������� ������ ���������������
        /// </summary>
        private void AttachClsClassifiers()
        {
            // ������� 2 ������� � ����������������. 
            // ������������ � ������ �����, � ������������� ������ � ����� ����� ������

            // ������� ���������� ������������� ������
            if (attClsData != null)
                ClearAttachedCLS(attClsData);
            // �������� ������������� ������
            IEntity clsData = curentAssociation.RoleData;
            attClsData = this.Workplace.GetClsView(clsData);
            //attClsData = attClsData.ProtocolsInplacer;
            attClsData.AttachCls(associateView.pDataCls, ref attClsData);

            // ���������  ��������� ����������� ����� � �������
            toolBar = attClsData.GetClsToolBar();

            attClsData.SelectDataSource += new VoidDelegate(attClsData_SelectDataSource);
            attClsData.RefreshData += new VoidDelegate(attClsData_RefreshData);

            clsDataGrid = attClsData.UltraGridExComponent.ugData;
            clsDataGrid.AfterRowActivate += new EventHandler(DataGrid_AfterRowActivate);
            clsDataGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(clsDataGrid_InitializeRow);
            clsDataGrid.SelectionDrag += new System.ComponentModel.CancelEventHandler(clsDataGrid_SelectionDrag);

            // �������� ������������ �������������
            // ������� ���������� ������������ �������������
            if (attClsBridge != null)
                ClearAttachedCLS(attClsBridge);

            attClsBridge = this.Workplace.GetClsView(curentAssociation.RoleBridge);
            attClsBridge.AttachCls(associateView.scAssociation.Panel2, ref attClsBridge);


            clsBridgeGrid = attClsBridge.UltraGridExComponent.ugData;
            bridgeToolBar = attClsBridge.GetClsToolBar();
            // ����������� ��� Drag&Drop
            clsBridgeGrid.AfterRowActivate += new EventHandler(BridgeGrid_AfterRowActivate);
            clsBridgeGrid.DragDrop += new DragEventHandler(clsDataGrid_DragDrop);
            clsBridgeGrid.DragEnter += new DragEventHandler(clsDataGrid_DragEnter);
            clsBridgeGrid.DragOver += new DragEventHandler(clsDataGrid_DragOver);
        }

        void attClsData_SelectDataSource()
        {
            ((IBridgeAssociation)curentAssociation).RefreshRecordsCount();
            SetAssociateCount();
            //ShowCurrentAssociate();
            if (associateView.rbCurrentAssociate.Checked || associateView.rbUnAssociate.Checked)
            {
                HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
                hi.CurViewState = ViewState.Hierarchy;
                hi.loadMode = LoadMode.AllRows;
                attClsData.UltraGridExComponent.PerfomAction("ShowHierarchy");
            }
        }

        void attClsData_RefreshData()
        {
            curentAssociation.RefreshRecordsCount();
            SetAssociateCount();
            HideNoAssociationRef();

            if (associateView.rbCurrentAssociate.Checked || associateView.rbUnAssociate.Checked)
            {
                HierarchyInfo hi = attClsData.GetHierarchyInfo(attClsData.UltraGridExComponent);
                hi.CurViewState = ViewState.Hierarchy;
                hi.loadMode = LoadMode.AllRows;
                attClsData.UltraGridExComponent.PerfomAction("ShowHierarchy");
            }
        }

        /// <summary>
        /// ���������� ���������� ������� ����� ������ ������ ���������
        /// </summary>
        void attClsData_OnAfterSourceSelect()
        {
            SetAssociateCount();
            HideNoAssociationRef();
        }

        /// <summary>
        /// ���������� ���������� ������� ����� ���������� ������ �������������� ������
        /// </summary>
        void attClsData_OnAfterRefreshData()
        {
            curentAssociation.RefreshRecordsCount();
            SetAssociateCount();
            HideNoAssociationRef();
        }

        #endregion

        #region ���������� Drag&Drop

        UltraGridRow DragDropRow = null;

        UltraGridRow LastEnteredRow = null;

        void clsDataGrid_DragOver(object sender, DragEventArgs e)
        {

            e.Effect = DragDropEffects.Move;
            UltraGridRow tmpRow = GetRowFromPos((UltraGrid)sender, e.X, e.Y);
            if (tmpRow != LastEnteredRow)
            {
                ResetROwColor(LastEnteredRow, (UltraGrid)sender);
                LastEnteredRow = tmpRow;
                SetRowColor(LastEnteredRow);
            }
        }

        void clsDataGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        bool needRefresh = false;

        /// <summary>
        ///  ��� Drag&Drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clsDataGrid_DragDrop(object sender, DragEventArgs e)
        {
            UltraGridRow tmpRow = GetRowFromPos((UltraGrid)sender, e.X, e.Y);
            if (tmpRow != LastEnteredRow && tmpRow != DragDropRow)
            {
                ResetROwColor(LastEnteredRow, (UltraGrid)sender);
                LastEnteredRow = tmpRow;
            }
            if (LastEnteredRow != null)
            {
                if (MessageBox.Show("����������� ������ �������������� ������ � ������� ������������� ��������������?", "������������� ������", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    clsDataGrid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                    try
                    {
                        DragDropRow.Cells[curentAssociation.FullDBName].Value = LastEnteredRow.Cells["ID"].Value;
                    }
                    finally
                    {
                        clsDataGrid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                    }
                    DragDropRow.Update();
                    LastEnteredRow.Activate();
                    needRefresh = false;
                }
            }
            else
                if (MessageBox.Show("�������� ������ � ������������ �������������?", "���������� ������", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    try
                    {
                        curentAssociation.CopyAndAssociateRow(Convert.ToInt32(DragDropRow.Cells["ID"].Value));
                        DragDropRow.Update();
                        needRefresh = true;
                    }
                    catch
                    {
                        throw;
                    }
        }

        /// <summary>
        ///  �������� Drag&Drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clsDataGrid_SelectionDrag(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!allowAddRecordsToBridgeCls) return;
            DragDropRow = clsDataGrid.ActiveRow;
            if (Convert.ToInt32(DragDropRow.Cells["ID"].Value) >= developerRowDiapazon && !this.Workplace.IsDeveloperMode)
                return;
            clsDataGrid.DoDragDrop(DragDropRow, DragDropEffects.Move);
            e.Cancel = true;
            DragDropRow.Refresh(RefreshRow.FireInitializeRow);
            ResetROwColor(LastEnteredRow, (UltraGrid)sender);
            DragDropRow = null;
            LastEnteredRow = null;
            SaveChanges();
            if (needRefresh)
            {
                attClsData.RefreshAttachedData();
                attClsBridge.RefreshAttachedData();
            }
            SetAssociateCount();
        }

        void clsDataGrid_DragLeave(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///  ���������� ��� ��������� ����� ������ � �����������
        /// </summary>
        /// <param name="row"></param>
        /// <param name="grid"></param>
        void ResetROwColor(UltraGridRow row, UltraGrid grid)
        {
            if (row != null)
            {
                row.Appearance.AlphaLevel = grid.DisplayLayout.Override.RowPreviewAppearance.AlphaLevel;
                row.Appearance.BackColor2 = grid.DisplayLayout.Override.RowPreviewAppearance.BackColor2;
                row.Appearance.BackHatchStyle = grid.DisplayLayout.Override.RowPreviewAppearance.BackHatchStyle;
            }
        }

        void SetRowColor(UltraGridRow row)
        {
            if (row == null) return;
            row.Appearance.AlphaLevel = 150;
            row.Appearance.BackColor2 = Color.Red;
            row.Appearance.BackHatchStyle = BackHatchStyle.Percent50;
        }

        /// <summary>
        /// �������� UltraGridRow �� �������� �����������. ������������ � �������.
        /// </summary>
        private UltraGridRow GetRowFromPos(UltraGrid grid, int X, int Y)
        {
            Point pt = new Point(X, Y);
            pt = grid.PointToClient(pt);
            UIElement elem = grid.DisplayLayout.UIElement.ElementFromPoint(pt);
            return GetRowFromElement(elem);
        }

        /// <summary>
        /// �������� UltraGridRow �� UIElement. ������������ � �������.
        /// </summary>
        /// <param name="elem">�������</param>
        /// <returns>������</returns>
        private UltraGridRow GetRowFromElement(UIElement elem)
        {
            UltraGridRow row = null;

            row = (UltraGridRow)elem.GetContext(typeof(UltraGridRow), true);

            return row;
        }
        #endregion

        /// <summary>
        ///  ������������ ���������������� ������ � �������������� ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clsDataGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            string str = curentAssociation.FullDBName;
            UltraGridRow row = e.Row;
            if (row.Cells[str].Value == DBNull.Value ||
                Convert.ToInt32(row.Cells[str].Value) == -1)
            {
                //row.Appearance.BackColor = Color.Salmon;
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (cell.Appearance.BackColor != Color.Empty)
                    {
                        cell.Appearance.BackColor2 = Color.Salmon;
                        cell.Appearance.AlphaLevel = 250;
                        cell.Appearance.BackHatchStyle = BackHatchStyle.SmallCheckerBoard;
                    }
                    else
                        cell.Appearance.BackColor = Color.Salmon;
                }
            }
            else
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (cell.Appearance.BackColor != Color.Salmon)
                    {
                        cell.Appearance.ResetBackColor2();
                    }
                    else
                        cell.Appearance.ResetBackColor();
                }
            }
        }

        /// <summary>
        ///  ���������� ����������� �� ����� � ��������������� ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataGrid_AfterRowActivate(object sender, EventArgs e)
        {
            if (CurrentClsBridgeRow != null)
            {
                // ���������� ����� � ���������� ������
                //CurrentClsBridgeRow.Appearance.ResetBackColor();
                foreach (UltraGridCell cell in CurrentClsBridgeRow.Cells)
                {
                    if (cell.Appearance.BackColor != Color.YellowGreen)
                    {
                        cell.Appearance.ResetBackColor2();
                    }
                    else
                        cell.Appearance.ResetBackColor();
                }
                CurrentClsBridgeRow.Selected = false;
                CurrentClsBridgeRow.Refresh();
            }
            CurrentClsBridgeRow = null;
        }

        void BridgeGrid_AfterRowActivate(object sender, EventArgs e)
        {
            if (associateView.rbCurrentAssociate.Checked)
                ShowCurrentAssociate();
                //SetFilter();
        }

        public override void InternalFinalize()
        {
            attClsData.FinalizeViewObject();
            attClsData = null;
            attClsBridge.FinalizeViewObject();
            attClsBridge = null;
            attClsData = null;
            attClsBridge = null;
            clsBridgeGrid = null;
            clsDataGrid = null;

            base.InternalFinalize();
        }

        /// <summary>
        ///  ���������� ���� ���������
        /// </summary>
        public override void SaveChanges()
        {
            attClsBridge.SaveChanges();
            attClsData.SaveChanges();
        }

        /// <summary>
        ///  ������ ���� ���������
        /// </summary>
        public override void CancelChanges()
        {
            
        }

        /// <summary>
        ///  ��������� ������ � ���������� ���� ������� � �������������� �������
        /// </summary>
        private void SetAssociateCount()
        {
            associateView.lAllDataCls.Text = string.Format("����� �������: {0}", curentAssociation.GetRecordsCountByCurrentDataSource(attClsData.CurrentSourceID));
            associateView.lNotClsData.Text = string.Format("�� ������������: {0}", curentAssociation.GetUnassociateRecordsCountByCurrentDataSource(attClsData.CurrentSourceID));

            associateView.lAllDataClsAllSources.Text = string.Format("�� ���� ����������: {0}", curentAssociation.GetAllRecordsCount());
            associateView.lUnAssociateAllSources.Text = string.Format("�� ���� ����������: {0}", curentAssociation.GetAllUnassociateRecordsCount());
        }

        /// <summary>
        ///  ����������� ���������� ��� �������. ���������� ��� ������ ���������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolBar_AfterToolCloseup(object sender, ToolDropdownEventArgs e)
        {
            attClsData.DataSourceSelected(sender, e);
            SetAssociateCount();
            SetFilter();
        }

        #region ��������� ������ � ���������� ���

        private int dataClsVisibleColumnsCount = 0;

        private int bridgeClsVisibleColumnsCount = 0;

        private void GetExcelReport()
        {
            // ���������� ������� �� ��������� ������ �� ������������� �� �������� ���������
            List<string> FieldsCaptions = new List<string>();
            string fieldsNames = string.Empty;
            GetColumnsNamesAndCaptions(ref fieldsNames, ref FieldsCaptions);
            string query = "select " + fieldsNames;
            query = query + string.Format(" from {0} dataCls, {1} bridgeCls where ((dataCls.{2} = bridgeCls.{3}) or ({2} is null))",
                curentAssociation.RoleData.FullDBName, curentAssociation.RoleBridge.FullDBName,
                curentAssociation.FullDBName, "ID");

            string fileName = this.captionString.Replace(">", string.Empty) + ".xls";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                this.Workplace.OperationObj.Text = "���������� ������";
                this.Workplace.OperationObj.StartOperation();
                Infragistics.Excel.Workbook wb = new Infragistics.Excel.Workbook();
                UltraGridExcelExporter excelExpoter = new UltraGridExcelExporter();
                excelExpoter.FileLimitBehaviour = FileLimitBehaviour.TruncateData;
                UltraGrid tmpGrid = new UltraGrid();
                tmpGrid.Parent = associateView.panel1;
                tmpGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(tmpGrid_InitializeRow);
                tmpGrid.Visible = true;
                InfragisticComponentsCustomize.CustomizeUltraGridParams(tmpGrid);
                // �������� ��� ��������� ������ �� ��������������
                Dictionary<int, string> dataSources = ((IClassifier)curentAssociation.RoleData).GetDataSourcesNames();

                try
                {
                    // �������� �� ���� ����������, ����� ������ � ���������� � ��������� �������
                    foreach (KeyValuePair<int, string> kvp in dataSources)
                    {
                        // ��������� � ��������� ������
                        string filter = string.Format(" and dataCls.RowType = 0 and dataCls.SourceID = {0} order by dataCls.{1}",
                            kvp.Key, curentAssociation.FullDBName);
                        DataTable dt = GetAssociationData(query + filter);
                        if (dt != null)
                        {
                            tmpGrid.DataSource = dt;

                            for (int i = 0; i <= FieldsCaptions.Count - 1; i++)
                                tmpGrid.DisplayLayout.Bands[0].Columns[i].Header.Caption = FieldsCaptions[i];

                            Infragistics.Excel.Worksheet ws = wb.Worksheets.Add(string.Format("SourceID={0}", kvp.Key));

                            Infragistics.Excel.WorksheetRow excelRow = ws.Rows[0];

                            WorksheetMergedCellsRegion reg = ws.MergedCellsRegions.Add(2, 0, 2, dataClsVisibleColumnsCount - 1);
                            reg.Value = DataClsCaption;

                            reg.CellFormat.RightBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.TopBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.Alignment = HorizontalCellAlignment.Center;

                            reg = ws.MergedCellsRegions.Add(2, dataClsVisibleColumnsCount, 2, dataClsVisibleColumnsCount + bridgeClsVisibleColumnsCount - 1);
                            reg.Value = BridgeClsCaption;

                            reg.CellFormat.RightBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.TopBorderStyle = CellBorderLineStyle.Thick;
                            reg.CellFormat.Alignment = HorizontalCellAlignment.Center;

                            excelRow.Cells[0].Value = kvp.Value;
                            excelRow.Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                            excelExpoter.Export(tmpGrid, ws, 3, 0);

                            for (int i = 3; i <= tmpGrid.Rows.Count + 3; i++)
                            {
                                WorksheetRow row = ws.Rows[i];
                                row.CellFormat.WrapText = ExcelDefaultableBoolean.True;
                                row.Cells[dataClsVisibleColumnsCount - 1].CellFormat.RightBorderStyle = CellBorderLineStyle.Thick;
                                row.CellFormat.ShrinkToFit = ExcelDefaultableBoolean.True;
                            }
                            tmpGrid.DataSource = null;
                        }
                    }
                    if (wb.Worksheets.Count == 0)
                        wb.Worksheets.Add("���� ������");
                    //wb.Save(fileName);
                    Infragistics.Excel.BIFF8Writer.WriteWorkbookToFile(wb, fileName);
                }
                finally
                {
                    excelExpoter.Dispose();
                    tmpGrid.Parent = null;
                    this.Workplace.OperationObj.StopOperation();
                }
            }
        }

        /// <summary>
        /// ��������� ������ �� �������������. ����� ��������� �� �������� �����
        /// </summary>
        /// <param name="SelectDataQuery">������ ��������� ������</param>
        /// <returns></returns>
        DataTable GetAssociationData(string SelectDataQuery)
        {
            IDatabase db = this.Workplace.ActiveScheme.SchemeDWH.DB;
            DataTable dt = null;
            try
            {
                dt = (DataTable)db.ExecQuery(SelectDataQuery, QueryResultTypes.DataTable);
            }
            finally
            {
                db.Dispose();
            }
            return dt;
        }

        private string DataClsCaption;
        private string BridgeClsCaption;

        /// <summary>
        ///  ��������� ������ � ������� ����� ��� ������� � ������ ������� ���������� �������
        /// </summary>
        /// <returns></returns>
        void GetColumnsNamesAndCaptions(ref string columnsNames, ref List<string> columnsCaptions)
        {
            columnsCaptions.Clear();
            // �������� ��������������� � �������������
            // �������� �������������� ������
            DataClsCaption = curentAssociation.RoleData.FullCaption;
            // �������� ������������� ��������������
            BridgeClsCaption = curentAssociation.RoleBridge.FullCaption;

            // �������� ������ ����� �������������� ������
            Dictionary<int, string> DataClsFieldDictionary = new Dictionary<int, string>();
            //int i = 1;
            foreach (UltraGridColumn clmn in clsDataGrid.DisplayLayout.Bands[0].Columns)
            {
                if (clmn.Hidden == false && clmn.Header.Caption != string.Empty)
                    DataClsFieldDictionary.Add(clmn.Header.VisiblePosition, string.Format("dataCls.{0}", CC.UltraGridEx.GetSourceColumnName(clmn.Key)));
            }
            List<string> DataClsFields = new List<string>();
            string val = string.Empty;
            for (int i = 0; i <= 50; i++)
            {
                DataClsFieldDictionary.TryGetValue(i, out val);
                if (val != null)
                    DataClsFields.Add(val);
            }

            // ���� ������ ������� ������ �� ������������, ������� �� 
            if (!DataClsFields.Contains(string.Format("dataCls.{0}", curentAssociation.FullDBName.ToUpper())))
                DataClsFields.Add(string.Format("dataCls.{0}", curentAssociation.FullDBName));
            // �������� ��������� ��� ������� �������������� ������
            List<string> DataClsFieldsCaptions = new List<string>();
            GetColumnsCaptions(clsDataGrid, DataClsFields, ref DataClsFieldsCaptions);
            // ��������� � ���������� ������� �������� ��������������
            foreach (string columnCaption in DataClsFieldsCaptions)
            {
                columnsCaptions.Add(columnCaption);// + "." + columnCaption);
            }
            dataClsVisibleColumnsCount = DataClsFieldsCaptions.Count;
            // �������� ������ ����� ������������� ��������������
            Dictionary<int, string> BridgeClsFieldsDictionary = new Dictionary<int, string>();
            //i = 1;
            foreach (UltraGridColumn clmn in clsBridgeGrid.DisplayLayout.Bands[0].Columns)
            {
                if (clmn.Hidden == false && clmn.Header.Caption != string.Empty)
                    BridgeClsFieldsDictionary.Add(clmn.Header.VisiblePosition, string.Format("bridgeCls.{0}", CC.UltraGridEx.GetSourceColumnName(clmn.Key)));
            }

            List<string> BridgeClsFields = new List<string>();
            val = string.Empty;
            for (int i = 0; i <= 50; i++)
            {
                BridgeClsFieldsDictionary.TryGetValue(i, out val);
                if (val != null)
                    BridgeClsFields.Add(val);
            }

            // �������� ��������� ��� ������� ������������� ��������������
            List<string> BridgeClsFieldsCaptions = new List<string>();
            GetColumnsCaptions(clsBridgeGrid, BridgeClsFields, ref BridgeClsFieldsCaptions);
            // ��������� � ���������� ������� �������� ��������������
            foreach (string columnCaption in BridgeClsFieldsCaptions)
            {
                columnsCaptions.Add(columnCaption);// + "." + columnCaption);
            }
            bridgeClsVisibleColumnsCount = BridgeClsFieldsCaptions.Count;
            string retutnValue = String.Join(", ", DataClsFields.ToArray());
            retutnValue = String.Concat(retutnValue, ", ");
            retutnValue = String.Concat(retutnValue, String.Join(", ", BridgeClsFields.ToArray()));
            columnsNames = retutnValue;
            columnsNames = columnsNames.Replace("_Remasked", string.Empty);
        }

        /// <summary>
        /// ���������� ������� ������������ ��� �������
        /// </summary>
        /// <returns></returns>
        void GetColumnsCaptions(UltraGrid grid, List<string> columnsNames, ref List<string> Captions)
        {
            // �������� ��� ������� �������� �����
            string str = string.Empty;
            foreach (string clmnName in columnsNames)
            {
                str = clmnName.Split('.')[1];
                Captions.Add(grid.DisplayLayout.Bands[0].Columns[str].Header.Caption);
            }
        }

        /// <summary>
        /// ������ � ���� �� ������, ������� �� ������������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmpGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            string str = curentAssociation.FullDBName;
            UltraGridRow Row = e.Row;
            if (Row.Cells[str].Value == DBNull.Value ||
                Convert.ToInt32(Row.Cells[str].Value) == -1)
            {
                Row.Appearance.AlphaLevel = 150;
                Row.Appearance.BackColor = Color.Salmon;
            }
        }
        #endregion

        #region �������� ���� � ��������� �� ��

        private bool allowAssociate = false;
        private bool allowClearAssociate = false;
        private bool allowAddRecordsToBridgeCls = false;
        private bool allowDelRecordsFromBridgeCls = false;

        /// <summary>
        /// �������� �����, ������� ����������� �� ������� �������������
        /// </summary>
        private void CheckPermissions()
        {
            string objectKey = this.curentAssociation.ObjectKey;

            IUsersManager um = this.Workplace.ActiveScheme.UsersManager;
            // �������� ����� ����� �� ��� �������������
            allowAssociate = um.CheckPermissionForSystemObject(objectKey, (int)AssociateForAllClassifiersOperations.Associate, false);
            allowClearAssociate = um.CheckPermissionForSystemObject(objectKey, (int)AssociateForAllClassifiersOperations.ClearAssociate, false);
            allowAddRecordsToBridgeCls = um.CheckPermissionForSystemObject(objectKey, (int)AssociateForAllClassifiersOperations.AddRecordIntoBridgeTable, false);
            allowDelRecordsFromBridgeCls = um.CheckPermissionForSystemObject(objectKey, (int)AssociateForAllClassifiersOperations.DelRecordFromBridgeTable, false);

            // �������� ����� �� ���������� �������������
            if (!allowAssociate)
                allowAssociate = um.CheckPermissionForSystemObject(objectKey, (int)AssociateOperations.Associate, false);
            if (!allowClearAssociate)
                allowClearAssociate = um.CheckPermissionForSystemObject(objectKey, (int)AssociateOperations.ClearAssociate, false);
            if (!allowAddRecordsToBridgeCls)
                allowAddRecordsToBridgeCls = um.CheckPermissionForSystemObject(objectKey, (int)AssociateOperations.AddRecordIntoBridgeTable, false);
            if (!allowDelRecordsFromBridgeCls)
                allowDelRecordsFromBridgeCls = um.CheckPermissionForSystemObject(objectKey, (int)AssociateOperations.DelRecordFromBridgeTable, false);
        }

        /// <summary>
        /// � ����������� �� ���� ��������� ��������� ��������
        /// </summary>
        private void SetPervissionsToAssociation()
        {
            ButtonTool btnAssociateMaster1 = (ButtonTool)associateView.utbmAssociate.Tools["AssociateMaster"];
            ButtonTool btnAssociateMaster2 = (ButtonTool)associateView.utbmAssociate.Tools["HandAssociate"];
            ButtonTool btnClearAssociateRef = (ButtonTool)associateView.utbmAssociate.Tools["ClearAllBridgeRef"];
            ButtonTool btnClearCurrentAssociateRef = (ButtonTool)associateView.utbmAssociate.Tools["ClearCurentBridgeRef"];
            ButtonTool btnAddRowToBridge = (ButtonTool)associateView.utbmAssociate.Tools["AddToBridge"];

            bool canAssociate = curentAssociation.AssociateRules.Count > 0;
            if (allowAssociate && canAssociate)
            {
                btnAssociateMaster1.SharedProps.Enabled = true;
                btnAssociateMaster2.SharedProps.Enabled = true;
            }
            else
            {
                btnAssociateMaster1.SharedProps.Enabled = false;
                btnAssociateMaster2.SharedProps.Enabled = false;
            }

            if (allowClearAssociate)
            {
                btnClearAssociateRef.SharedProps.Enabled = true;
                btnClearCurrentAssociateRef.SharedProps.Enabled = true;
            }
            else
            {
                btnClearAssociateRef.SharedProps.Enabled = false;
                btnClearCurrentAssociateRef.SharedProps.Enabled = false;
            }

            if (allowDelRecordsFromBridgeCls)
            {
                btnAddRowToBridge.SharedProps.Enabled = true;
            }
            else
            {
                btnAddRowToBridge.SharedProps.Enabled = false;
            }
        }
        #endregion
    }
}