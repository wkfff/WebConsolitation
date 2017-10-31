using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Components;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        /*
        #region реализация интерфейса IBaseCLS

        // активный объект-поставщик данных (классификатор, таблица фактов)
        private IEntity activeDataObj;
        /// <summary>
        /// активный объект-поставщик данных (классификатор, таблица фактов)
        /// </summary>
        public IEntity ActiveDataObj
        {
            get { return this.activeDataObj; }
            set { this.activeDataObj = value; }
        }

        // компонент с гридом
        private UltraGridEx ultraGridExComponent;
        /// <summary>
        /// текщий компонент с гридом и тулбаром
        /// </summary>
        public UltraGridEx UltraGridExComponent
        {
            get { return this.vo.ugeCls; }
        }

        private int currentSourceID;

        public int CurrentSourceID
        {
            get { return this.CurrentDataSourceID; }
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

        private string currentFilterCaption;

        public string CurrentFilterCaption
        {
            get { return currentFilterCaption; }
            set { currentFilterCaption = value; }
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

        public IInplaceClsView ProtocolsInplacer
        {
            get { return (IInplaceClsView)this; }
        }

        #endregion
         * */
    }
}
