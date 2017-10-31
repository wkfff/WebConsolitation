using System.Collections.Generic;
using System.Windows.Forms;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ProcessManager;

namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    public partial class MDObjectsManagementNavigation : BaseNavigationCtrl
    {
        private static MDObjectsManagementNavigation instance;

        public static MDObjectsManagementNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MDObjectsManagementNavigation();
                }
                return instance;
            }
        }

        private Dictionary<string, MDObjectsManagementUI> openedViewObjects;

        public MDObjectsManagementNavigation()
        {
            instance = this;
            Caption = "Управление многомерными моделями";            
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.MDObjectsManagementUI_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.MDObjectsManagementUI_24; }
        }

        public override void Initialize()
        {
            InitializeComponent();

            openedViewObjects = new Dictionary<string, MDObjectsManagementUI>();

            uebMain.ItemCheckStateChanged += new Infragistics.Win.UltraWinExplorerBar.ItemCheckStateChangedEventHandler(uebMain_ItemCheckStateChanged);

            Workplace.ViewClosed += Workplace_ViewClosed;
            Workplace.ActiveWorkplaceWindowChanged += new System.EventHandler(Workplace_ActiveWorkplaceWindowChanged);

			if (Workplace.ActiveScheme.Processor.OlapDBWrapper.GetDatabaseErrorsCount() > 0)
			{
				AddExplorerBarItem(MDOObjectsKeys.DatabaseErrors, "Ошибки базы данных");
			}

            if (Workplace.ActiveScheme.SchemeMDStore.IsAS2005())
            {
                AddExplorerBarItem(MDOObjectsKeys.ProcessOption, "Опции расчета");
            }

            base.Initialize();
        }

		/// <summary>
		/// Добавляет объект в навигационную область.
		/// </summary>
		private void AddExplorerBarItem(string key, string caption)
		{
			Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem =
				new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
			ultraExplorerBarItem.Key = key;
			ultraExplorerBarItem.Settings.AppearancesSmall.Appearance = new Infragistics.Win.Appearance();
			ultraExplorerBarItem.Text = caption;
			uebMain.Groups[0].Items.Add(ultraExplorerBarItem);
		}
		
		void Workplace_ActiveWorkplaceWindowChanged(object sender, System.EventArgs e)
        {
            if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                if (!openedViewObjects.ContainsKey(key))
                    return;
                Workplace.SwitchTo("Управление многомерными моделями");
                if (uebMain.CheckedItem == null)
                    return;
                if (key != uebMain.CheckedItem.Key)
                {
                    uebMain.Groups[0].Items[key].Checked = true;
                    uebMain.Groups[0].Items[key].Active = true;
                }
            }
        }

        void uebMain_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            if (e.Item.Checked)
            {
                if (!openedViewObjects.ContainsKey(e.Item.Key))
                {
                    MDObjectsManagementUI viewObject = new MDObjectsManagementUI(e.Item.Key);
                    viewObject.Workplace = Workplace;
                    viewObject.Initialize();
                    //viewObject.LoadData();
                    OnActiveItemChanged(this, viewObject);
                    openedViewObjects.Add(e.Item.Key, viewObject);
                }
                else
                {
                    OnActiveItemChanged(this, openedViewObjects[e.Item.Key]);
                }
            }
        }

        /// <summary>
        /// Переход на интерфейс Менеджер расчетов
        /// </summary>
        /// <param name="args"></param>
        public void OpenProcessManagerView(ProceccManagerEventArgs args, bool fromMessage)
        {
            if (fromMessage)
            {
                // Если мы попадаем сюда по ссылки из сообщения, то показываем только область просмотра без навигации
                MDObjectsManagementUI viewObj = new MDObjectsManagementUI(MDOObjectsKeys.ProcessManager);
                viewObj.Workplace = Workplace;
                viewObj.Initialize();
                ((Workplace.Workplace)Workplace).ShowView(viewObj);
                ((ProcessManagerView)viewObj.ViewCtrl.Controls[0]).PackageFilter = args.BatchID;
                return;
            }

            MDObjectsManagementUI viewObject;
            if (!openedViewObjects.ContainsKey(MDOObjectsKeys.ProcessManager))
            {
                viewObject = new MDObjectsManagementUI(MDOObjectsKeys.ProcessManager);
                viewObject.Workplace = Workplace;
                viewObject.Initialize();
                ((ProcessManagerView)viewObject.ViewCtrl.Controls[0]).PackageFilter = args.BatchID;
                Instance.OnActiveItemChanged(this, viewObject);
                openedViewObjects.Add(MDOObjectsKeys.ProcessManager, viewObject);
            }
            else
            {
                ((ProcessManagerView)openedViewObjects[MDOObjectsKeys.ProcessManager].ViewCtrl.Controls[0]).PackageFilter = args.BatchID;
                Instance.OnActiveItemChanged(this, openedViewObjects[MDOObjectsKeys.ProcessManager]);
            }
        }

		private void Workplace_ViewClosed(object sender, Krista.FM.Client.Common.Gui.ViewContentEventArgs e)
        {
            string forRemove = null;
            foreach (KeyValuePair<string, MDObjectsManagementUI> item in openedViewObjects)
            {
                if (item.Value == e.Content)
                {
                    forRemove = item.Key;
                    break;
                }
            }

            if (forRemove != null)
                openedViewObjects.Remove(forRemove);
        }

        private void uebMain_ItemClick(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {   
            MessageBox.Show(sender.GetType().Name); 
        }
    }

    public static class MDOObjectsKeys
    {
        public const string Partitions = "BFE93C3C-2C97-4129-8A19-55D2787D83AA";
        public const string DimensionsNew = "EF6FEC85-3D1B-40a9-8B4A-5D4F732A5AC2";
        public const string ProcessManager = "4DCED6EB-C10B-43c6-AFC7-97322DE1DCE7";
		public const string DatabaseErrors = "5DCED6EB-C20B-45c6-AFC8-37342DE1DCE7";
		public const string Others = "0F8FFA72-16E1-4e8e-8116-6421EAC1CAD6";
        public const string ProcessOption = "873C8590-D870-4BE8-A8C0-A924C1CDC021";
    }
}

