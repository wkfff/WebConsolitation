using System;
using System.Collections.Generic;
using Infragistics.Win.UltraWinExplorerBar;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.TranslationsTables;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation
{
    public partial class EntityNavigationControl : BaseNavigationCtrl, IBaseClsNavigation
    {
        public EntityNavigationControl()
        {
            ResourcesLoader.Loader.Initialize();
            Caption = "Классификаторы и таблицы";
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.Tables_classifiers_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.Tables_classifiers_24; }
        }

        /// <summary>
        /// Инициализация интерфейса
        /// </summary>
        public override void Initialize()
        {
            InitializeComponent();
            SetVisiblePermissions();
            ultraExplorerBar.ItemCheckStateChanged += ultraExplorerBar_ItemCheckStateChanged;
            Workplace.ViewClosed += Workplace_ViewClosed;
            Workplace.ActiveWorkplaceWindowChanged += Workplace_ActiveWorkplaceWindowChanged;
            Workplace.ActiveGroupChanged += new ActiveGroupChangedEventHandler(Workplace_ActiveGroupChanged);
            base.Initialize();

            InfragisticsRusification.LocalizeAll();
        }

        void Workplace_ActiveGroupChanged(object sender, GroupEventArgs e)
        {

        }

        /// <summary>
        /// Добавляет объект в навигационную область.
        /// </summary>
        private void AddItem2ExplorerBar(BaseViewObj viewContent)
        {
            AddItem2ExplorerBar(viewContent, -1);
        }

        /// <summary>
        /// Добавляет объект в навигационную область.
        /// </summary>
        private void AddItem2ExplorerBar(BaseViewObj viewContent, int imageIndex)
        {
            UltraExplorerBarItem ultraExplorerBarItem = new UltraExplorerBarItem();
            ultraExplorerBarItem.Key = viewContent.Key;
            ultraExplorerBarItem.Settings.AppearancesSmall.Appearance = new Infragistics.Win.Appearance();
            if (imageIndex > -1)
                ultraExplorerBarItem.Settings.AppearancesSmall.Appearance.Image = imageList.Images[imageIndex];
            else
                ultraExplorerBarItem.Settings.AppearancesSmall.Appearance.Image = viewContent.Icon.ToBitmap();
            ultraExplorerBarItem.Text = viewContent.Caption;
            ultraExplorerBarItem.Tag = viewContent.GetType();
            ultraExplorerBar.Groups[0].Items.Add(ultraExplorerBarItem);
        }

        private void ultraExplorerBar_ItemCheckStateChanged(object sender, ItemEventArgs e)
        {
            if (e.Item.Checked)
            {
                IViewContent vc = WorkplaceSingleton.Workplace.GetOpenedContent(e.Item.Key);
                if (vc != null)
                {
                    vc.WorkplaceWindow.SelectWindow();
                }
                else
                {
                    EntityNavigationListUI view = (EntityNavigationListUI)GetType().Assembly.CreateInstance(e.Item.Tag.ToString());
                    view.Workplace = Workplace;
                    view.Initialize();
                    view.ViewCtrl.Text = e.Item.Text;
                    OnActiveItemChanged(this, view);
                }
            }
        }

        private void Workplace_ViewClosed(object sender, ViewContentEventArgs e)
        {
            if (ultraExplorerBar.CheckedItem == null)
                return;

            if (e.Content.Key == ultraExplorerBar.CheckedItem.Key)
            {
                ultraExplorerBar.CheckedItem.Active = false;
                ultraExplorerBar.CheckedItem.Checked = false;
            }
        }

        private void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
            if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                // выделение пункта в навигационной области при переходе на классификаторы и таблицы
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                string probeKey = GetNavigationKey((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent);
                key = String.IsNullOrEmpty(probeKey) ? key : probeKey;

                if (!ultraExplorerBar.Groups[0].Items.Exists(key))
                    return;
                Workplace.SwitchTo(Caption);

                if (ultraExplorerBar.CheckedItem == null ||
                    key != ultraExplorerBar.CheckedItem.Key)
                {
                    if (ultraExplorerBar.Groups[0].Items.Exists(key))
                    {
                        ultraExplorerBar.EventManager.SetEnabled(
                            UltraExplorerBarEventGroups.AllEvents, false);
                        try
                        {
                            if (!(ultraExplorerBar.Parent is EntityNavigationControl))
                            {
                                ultraExplorerBar.Groups[0].Items[key].Checked = true;
                                ultraExplorerBar.Groups[0].Items[key].Active = true;
                            }
                        }
                        finally
                        {
                            ultraExplorerBar.EventManager.SetEnabled(
                            UltraExplorerBarEventGroups.AllEvents, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// метод перехода на интерфейс из другого интерфейса
        /// </summary>
        public override void SetActive(params object[] moduleParams)
        {
            if (moduleParams == null)
                return;
            if (moduleParams.Length < 2)
                return;

            if (Convert.ToString(moduleParams[0]) == typeof(AssociationNavigationListUI).FullName)
            {
                AssociationNavigationListUI anl = new AssociationNavigationListUI();
                anl.ActiveteViewObjectUI(Convert.ToString(moduleParams[1]));
            }

            if (Convert.ToString(moduleParams[0]) == typeof(TranslationsNavigationListUI).FullName)
            {
                TranslationsNavigationListUI anl = new TranslationsNavigationListUI();
                anl.ActiveteViewObjectUI(Convert.ToString(moduleParams[1]));
            }
        }

        /// <summary>
        /// получение названия списка навигации из названия элемента в списке
        /// </summary>
        /// <param name="viewObject"></param>
        /// <returns></returns>
        private static string GetNavigationKey(BaseViewObj viewObject)
        {
            if (viewObject.GetType() == typeof(DataClsUI))
                return typeof(DataClsNavigationListUI).FullName;
            if (viewObject.GetType() == typeof(AssociatedClsUI))
                return typeof(BridgeClsNavigationListUI).FullName;
            if (viewObject.GetType() == typeof(FactTablesUI))
                return typeof(FactTablesNavigationListUI).FullName;
            if (viewObject.GetType() == typeof(FixedClsUI))
                return typeof(FixedClsNavigationListUI).FullName;
            if (viewObject.GetType() == typeof(TranslationsTablesUI))
                return typeof(TranslationsNavigationListUI).FullName;
            if (viewObject.GetType() == typeof(AssociationUI))
                return typeof(AssociationNavigationListUI).FullName;
            return String.Empty;
        }

        #region IBaseClsNavigation Members


        public IInplaceClsView GetClsView(IEntity cls)
        {
            BaseClsUI ClsUI = null;
            switch (cls.ClassType)
            {
                case ClassTypes.clsFixedClassifier:
                    ClsUI = new FixedCls.FixedClsUI(cls);
                    break;
                case ClassTypes.clsBridgeClassifier:
                    ClsUI = new AssociatedCls.AssociatedClsUI(cls);
                    break;
                case ClassTypes.clsDataClassifier:
                    ClsUI = new DataCls.DataClsUI(cls);
                    break;
                case ClassTypes.clsFactData:
                    ClsUI = new FactTables.FactTablesUI(cls);
                    break;
            }
            ClsUI.Workplace = Workplace;
            ClsUI.InitModalCls(-1);
            ClsUI.Initialize();
            return ClsUI;
        }

        public IModalClsManager ClsManager()
        {
            return new ModalClsManager(Workplace);
        }

        #endregion

        /// <summary>
        /// Устанавливаем видимость отдельных частей интерфейса согласно правам на отображение
        /// </summary>
        private void SetVisiblePermissions()
        {
            IUsersManager um = Workplace.ActiveScheme.UsersManager;

            bool fixedClassifiersVisible = um.CheckPermissionForSystemObject("EntityNavigationListUI", (int)ServerLibrary.EntityNavigationListUI.Display, false);
            if (!fixedClassifiersVisible)
                fixedClassifiersVisible = um.CheckPermissionForSystemObject("FixedClassifiers", (int)UIClassifiersSubmoduleOperation.Display, false);
            if (fixedClassifiersVisible)
                AddItem2ExplorerBar(new FixedClsNavigationListUI(), 0);
            bool dataClassifiersVisible = um.CheckPermissionForSystemObject("EntityNavigationListUI", (int)ServerLibrary.EntityNavigationListUI.Display, false);
            if (!dataClassifiersVisible)
                dataClassifiersVisible = um.CheckPermissionForSystemObject("DataClassifiers", (int)UIClassifiersSubmoduleOperation.Display, false);
            if (dataClassifiersVisible)
                AddItem2ExplorerBar(new DataClsNavigationListUI(), 1);
            bool bridgeClassifiersVisible = um.CheckPermissionForSystemObject("EntityNavigationListUI", (int)ServerLibrary.EntityNavigationListUI.Display, false);
            if (!bridgeClassifiersVisible)
                bridgeClassifiersVisible = um.CheckPermissionForSystemObject("AssociatedClassifiers", (int)UIClassifiersSubmoduleOperation.Display, false);
            if (bridgeClassifiersVisible)
                AddItem2ExplorerBar(new BridgeClsNavigationListUI(), 2);
            bool factTablesVisible = um.CheckPermissionForSystemObject("EntityNavigationListUI", (int)ServerLibrary.EntityNavigationListUI.Display, false);
            if (!factTablesVisible)
                factTablesVisible = um.CheckPermissionForSystemObject("FactTables", (int)UIClassifiersSubmoduleOperation.Display, false);
            if (factTablesVisible)
                AddItem2ExplorerBar(new FactTablesNavigationListUI(), 3);
            bool associationsVisible = um.CheckPermissionForSystemObject("EntityNavigationListUI", (int)ServerLibrary.EntityNavigationListUI.Display, false);
            if (!associationsVisible)
                associationsVisible = um.CheckPermissionForSystemObject("Associations", (int)UIClassifiersSubmoduleOperation.Display, false);
            if (associationsVisible)
                AddItem2ExplorerBar(new AssociationNavigationListUI());
            bool translationTablesVisible = um.CheckPermissionForSystemObject("EntityNavigationListUI", (int)ServerLibrary.EntityNavigationListUI.Display, false);
            if (!translationTablesVisible)
                translationTablesVisible = um.CheckPermissionForSystemObject("TranslationTables", (int)UIClassifiersSubmoduleOperation.Display, false);
            if (translationTablesVisible)
                AddItem2ExplorerBar(new TranslationsNavigationListUI());
        }
    }
}