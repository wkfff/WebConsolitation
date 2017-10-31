using System;
using System.Collections.Generic;
using System.Drawing;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.GlobalConstsUI
{
    public partial class GlobalConstsNavigation : BaseViewObject.BaseNavigationCtrl
    {
        private static GlobalConstsNavigation instance;

        internal static GlobalConstsNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GlobalConstsNavigation();
                }
                return instance;
            }
        }

        private Dictionary<string, GlobalConstsViewObj> openedViewObjects;

        public GlobalConstsNavigation()
        {
            instance = this;
            Caption = "Константы";
        }

        public override Image TypeImage16
        {
            get { return Properties.Resources.Apps_preferences_kcalc_constants_icon_16; }
        }

        public override Image TypeImage24
        {
            get { return Properties.Resources.Apps_preferences_kcalc_constants_icon_24; }
        }

        /// <summary>
        /// Инициализация объкта навигации.
        /// </summary>
        public override void Initialize()
        {
            InitializeComponent();

            openedViewObjects = new Dictionary<string, GlobalConstsViewObj>();

            uebNavi.ItemCheckStateChanged += uebNavi_ItemCheckStateChanged;

            Workplace.ViewClosed += new Krista.FM.Client.Common.Gui.ViewContentEventHandler(Workplace_ViewClosed);
            Workplace.ActiveWorkplaceWindowChanged += new EventHandler(Workplace_ActiveWorkplaceWindowChanged);
            base.Initialize();
        }

        void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
            if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                if (!openedViewObjects.ContainsKey(key))
                    return;
                Workplace.SwitchTo("Константы");
                if (uebNavi.CheckedItem == null)
                    return;
                if (key != uebNavi.CheckedItem.Key)
                {
                    uebNavi.Groups[0].Items[key].Checked = true;
                    uebNavi.Groups[0].Items[key].Active = true;
                }
            }
        }

        private void uebNavi_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            string key = e.Item.Key;
            if (e.Item.Checked)
            {
                if (!openedViewObjects.ContainsKey(key))
                {
                    GlobalConstsViewObj viewObject = new GlobalConstsViewObj(key);
                    viewObject.Workplace = Workplace;
                    viewObject.Initialize();
                    viewObject.LoadData();
                    OnActiveItemChanged(this, viewObject);
                    openedViewObjects.Add(key, viewObject);
                }
                else
                {
                    OnActiveItemChanged(this, openedViewObjects[key]);
                }
            }
        }

        private void Workplace_ViewClosed(object sender, Krista.FM.Client.Common.Gui.ViewContentEventArgs e)
        {
            string forRemove = string.Empty;
            foreach (KeyValuePair<string, GlobalConstsViewObj> item in openedViewObjects)
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

        internal static string GetCaptionFromGlobalConstsTypes(GlobalConstsTypes type)
        {
            switch (type)
            {
                case GlobalConstsTypes.General:
                    return "Настраиваемые";
                case GlobalConstsTypes.Configuration:
                    return "Конфигурационные";
                case GlobalConstsTypes.Custom:
                    return "Пользовательские";
                default:
                    throw new ArgumentException(String.Format("Неизвестный элемент перечисленяи: \"{0}\"", type));
            }
        }
    }

    public static class GlobalConstsKeys
    {
        public const string Configuration = "976E81D7-AEA2-4e66-A583-9121B77D1324";
        public const string General = "1F0EB52E-5A59-4a3c-BD58-0D5F02D9A235";
        public const string Custom = "7B0506E4-5A8C-4c05-BC36-5A4FF0A3B471";
    }
}
