using System;
using System.Drawing;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AdministrationUI;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
    public class TemplatesNavigation : BaseNavigationCtrl
    {
        private static TemplatesNavigation instance;

        internal static TemplatesNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TemplatesNavigation();
                }
                return instance;
            }
        }

        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar ultraExplorerBar;

        public TemplatesNavigation()
        {
            instance = this;
            Caption = "Репозиторий отчетов";
        }

        public override Image TypeImage16
        {
			get { return Properties.Resources.Repository_16; }
        }

        public override Image TypeImage24
        {
			get { return Properties.Resources.Repository_24; }
        }

    	private AdministrationNavigation adminNavigation;

		internal IInplaceTasksPermissionsView GetTasksPermissions()
		{
			if (adminNavigation == null)
			{
				adminNavigation = new AdministrationNavigation();
				adminNavigation.Workplace = Client.Workplace.Gui.WorkplaceSingleton.Workplace;
			}
			
			return adminNavigation.GetTasksPermissions();
		}

        /// <summary>
        /// Инициализация объекта навигации.
        /// </summary>
        public override void Initialize()
        {
			try
			{
				Trace.TraceVerbose("Инициализация объекта навигации TemplatesNavigation");

				InitializeComponent();

				AddExplorerBarItem(new Gui.GeneralTemplatestUI());
				AddExplorerBarItem(new Gui.WebTemplatestUI());
				AddExplorerBarItem(new Gui.IPhoneTemplatestUI());
				AddExplorerBarItem(new Gui.SystemTemplatestUI());

				ultraExplorerBar.ItemCheckStateChanged += uebNavi_ItemCheckStateChanged;

				Workplace.ViewClosed += Workplace_ViewClosed;
				Workplace.ActiveWorkplaceWindowChanged += Workplace_ActiveWorkplaceWindowChanged;

				base.Initialize();
			}
			catch(Exception e)
			{
				Trace.TraceError("Ошибка инициализации TemplatesNavigation: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
			}
        }

        private void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
            if (Workplace.WorkplaceLayout.ActiveContent != null && e != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                if (ultraExplorerBar.Groups[0].Items.Exists(key))
                {
                    Workplace.SwitchTo("Репозиторий отчетов");

                    ultraExplorerBar.Groups[0].Items[key].Checked = true;
                    ultraExplorerBar.Groups[0].Items[key].Active = true;
                }
            }
        }

        private void uebNavi_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
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
					try
					{
						BaseViewObj viewObject = (BaseViewObj) e.Item.Tag;
						if (viewObject.Disposed)
						{
							e.Item.Tag = viewObject = (BaseViewObj) GetType().Assembly.CreateInstance(viewObject.GetType().FullName, true);
						}
						Trace.TraceVerbose("Открытие объекта просмотра \"{0}\"", viewObject.FullCaption);

						viewObject.Workplace = Workplace;
						viewObject.Initialize();
						viewObject.ViewCtrl.Text = e.Item.Text;

						OnActiveItemChanged(this, viewObject);

						viewObject.InitializeData();
					}
					catch (Exception ex)
					{
						Trace.TraceError("Ошибка инициализации объекта просмотра: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(ex));
					}
				}
            }
        }

        private void Workplace_ViewClosed(object sender, ViewContentEventArgs e)
        {
			if (ultraExplorerBar.CheckedItem != null && e.Content.Key == ultraExplorerBar.CheckedItem.Key)
			{
				ultraExplorerBar.CheckedItem.Active = false;
				ultraExplorerBar.CheckedItem.Checked = false;
			}
        }

        public override bool CanUnload
        {
            get
            {
                /*if (openedViewObject.editedTemplates.Count > 0)
                {
                    if (MessageBox.Show("Сохранить изменения", "Сохраненение данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        return openedViewObject.SaveData();
                    }
                    else
                    {
                        openedViewObject.ugeTemplates_OnCancelChanges(null);
                    }
                }*/
                return true;
            }
        }


		private void AddExplorerBarItem(BaseViewObj viewObj)
		{
			Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem =
				new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
			ultraExplorerBarItem.Key = viewObj.Key;
			ultraExplorerBarItem.Settings.AppearancesSmall.Appearance = new Infragistics.Win.Appearance();
			ultraExplorerBarItem.Settings.AppearancesLarge.Appearance = new Infragistics.Win.Appearance();
			ultraExplorerBarItem.Text = viewObj.FullCaption;
			ultraExplorerBarItem.Tag = viewObj;
			if (viewObj.Icon != null)
			{
				ultraExplorerBarItem.Settings.AppearancesSmall.Appearance.Image = new Icon(viewObj.Icon, 16, 16).ToBitmap();
				ultraExplorerBarItem.Settings.AppearancesLarge.Appearance.Image = new Icon(viewObj.Icon, 32, 32).ToBitmap();
			}
			ultraExplorerBar.Groups[0].Items.Add(ultraExplorerBarItem);
		}

        private void InitializeComponent()
        {
			Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
			this.ultraExplorerBar = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
			((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar)).BeginInit();
			this.SuspendLayout();
			// 
			// ultraExplorerBar
			// 
			this.ultraExplorerBar.AcceptsFocus = Infragistics.Win.DefaultableBoolean.True;
			this.ultraExplorerBar.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
			this.ultraExplorerBar.Dock = System.Windows.Forms.DockStyle.Fill;
			ultraExplorerBarGroup1.Text = "New Group";
			this.ultraExplorerBar.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1});
			this.ultraExplorerBar.GroupSettings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
			this.ultraExplorerBar.GroupSettings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
			this.ultraExplorerBar.GroupSettings.BorderStyleItemArea = Infragistics.Win.UIElementBorderStyle.None;
			this.ultraExplorerBar.GroupSettings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
			this.ultraExplorerBar.GroupSettings.Style = Infragistics.Win.UltraWinExplorerBar.GroupStyle.LargeImagesWithText;
			this.ultraExplorerBar.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ultraExplorerBar.ItemSettings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
			this.ultraExplorerBar.ItemSettings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
			this.ultraExplorerBar.Location = new System.Drawing.Point(0, 0);
			this.ultraExplorerBar.Name = "ultraExplorerBar";
			this.ultraExplorerBar.ShowDefaultContextMenu = false;
			this.ultraExplorerBar.Size = new System.Drawing.Size(216, 472);
			this.ultraExplorerBar.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
			this.ultraExplorerBar.TabIndex = 2;
			this.ultraExplorerBar.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
			// 
			// TemplatesNavigation
			// 
			this.Controls.Add(this.ultraExplorerBar);
			this.Name = "TemplatesNavigation";
			((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar)).EndInit();
			this.ResumeLayout(false);
        }
    }
}
