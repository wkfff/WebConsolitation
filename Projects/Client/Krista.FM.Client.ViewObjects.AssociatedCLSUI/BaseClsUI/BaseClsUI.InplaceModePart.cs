using System;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common;

using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinMaskedEdit;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{

    public delegate void AfterSourceSelect();

    public delegate void AfterRefreshData();


    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        #region ���������� ���������� IInplaceClsView
        /// <summary>
        /// ����� ��� ��������� �������������� � ������ ���������
        /// </summary>		
        public void AttachViewObject(Control parentControl)
        {
            vo.utbToolbarManager.ResetDockWithinContainer();
            vo.pnDataTemplate.Parent = parentControl;
            vo.utbToolbarManager.DockWithinContainer = vo.pnDataTemplate;
            vo.pnDataTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            vo.ugeCls.IsReadOnly = true;
            vo.ugeCls.StateRowEnable = false;
            vo.utbToolbarManager.Tools["disin"].SharedProps.Enabled = false;
        }

        public void AttachViewObject(Control parentControl, bool isReadonly)
        {
            vo.utbToolbarManager.ResetDockWithinContainer();
            vo.pnDataTemplate.Parent = parentControl;
            vo.utbToolbarManager.DockWithinContainer = vo.pnDataTemplate;
            vo.pnDataTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            vo.ugeCls.IsReadOnly = isReadonly;
            vo.ugeCls.StateRowEnable = false;
            vo.utbToolbarManager.Tools["disin"].SharedProps.Enabled = false;
        }

        public DataSet GetClsDataSet()
        {
            return dsObjData;
        }

        // ������� ���������� � ��������� ������
        public bool InInplaceMode = false;

        /// <summary>
        /// ������������� ������� ��� ������ � ��������� ������
        /// </summary>
        /// <param name="oldID">ID �� ������� ����� ��������� �����</param>
        public void InitModalCls(int oldID)
        {
            InInplaceMode = true;
        }

        /// <summary>
        ///	 ����� ��� ���������� ������ �� ���������� �����
        /// </summary>
        public object RefreshAttachedData()
        {
            return RefreshAttachedData(this.CurrentDataSourceID);
        }

        /// <summary>
        ///	 ����� ��� ���������� ������ �� ���������� �����
        /// </summary>
        public object RefreshAttachedData(int sourceID)
        {
            this.CurrentDataSourceID = sourceID;
            return InnerRefreshData();
        }

        /// <summary>
        /// ���������� ��������� ������������� �������� �������������� (ID)
        /// </summary>
        /// <returns>ID</returns>
        public int GetSelectedID()
        {
            return UltraGridHelper.GetActiveID(vo.ugeCls.ugData);
        }

        public List<int> GetSelectedIDs()
        {
            List<int> idList;
            UltraGridHelper.GetSelectedIDs(vo.ugeCls.ugData, out idList);
            return idList;
        }

        public void GetColumnsValues(string[] getColumns, ref object[] columnsValues)
        {
            UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(vo.ugeCls.ugData);
            if (activeRow == null)
                return;
            int counter = 0;
            foreach (string columnName in getColumns)
            {
                columnsValues[counter] = activeRow.Cells[columnName].Value;
                counter++;
            }
        }

        // �������� ������-��������� ������ (�������������, ������� ������)
        private IEntity activeDataObj;
        /// <summary>
        /// �������� ������-��������� ������ (�������������, ������� ������)
        /// </summary>
        public IEntity ActiveDataObj
        {
            get { return this.activeDataObj; }
            set { this.activeDataObj = value; }
        }

        // ��������� � ������
        private UltraGridEx ultraGridExComponent;
        /// <summary>
        /// ������ ��������� � ������ � ��������
        /// </summary>
        public UltraGridEx UltraGridExComponent
        {
            get { return this.vo.ugeCls; }
        }

        public int CurrentSourceID
        {
            get { return this.CurrentDataSourceID; }
        }

        private int _refVariant = -1;
        /// <summary>
        /// ������ �� �������
        /// </summary>
        public int RefVariant
        {
            get { return _refVariant; }
            set { _refVariant = value; }
        }

        public void AttachCls(Control ctrl, ref IInplaceClsView attCls)
        {
            attCls.AttachViewObject(ctrl);
            ComponentCustomizer.CustomizeInfragisticsControls(ctrl);
        }

        public void DetachViewObject()
        {
            vo.utbToolbarManager.ResetDockWithinContainer();
            vo.pnDataTemplate.Parent = null;
        }

        public void FinalizeViewObject()
        {
            this.InternalFinalize();
        }

        public UltraToolbarsManager GetClsToolBar()
        {
            return vo.ugeCls.utmMain;
        }

        public void DataSourceSelected(object sender, ToolDropdownEventArgs e)
        {
            this.utbToolbarManager_AfterToolCloseup(sender, e);
        }

        private VoidDelegate refreshData = null;

        public event VoidDelegate RefreshData
        {
            add { this.refreshData += value; }
            remove { this.refreshData -= value; }
        }

        private VoidDelegate selectDataSource = null;

        public event VoidDelegate SelectDataSource
        {
            add { this.selectDataSource += value; }
            remove { this.selectDataSource -= value; }
        }

        public IInplaceClsView ProtocolsInplacer
        {
            get { return (IInplaceClsView)this; }
        }


        #endregion
    }
}