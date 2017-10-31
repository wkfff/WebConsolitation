using System;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinTabControl;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common.Validations.Messages;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations
{
	public class MasterDetailCompositeVisulizator : ValidationResultVisualizator
	{
		private UltraTab tab;
		private UltraTab secondTab;

		public MasterDetailCompositeVisulizator(MasterDetailMessage validationMessage)
			: base(validationMessage)
		{
			if (WorkplaceSingleton.Workplace.ActiveContent is BaseClsUI)
			{
				BaseClsUI activeContent = (BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent;
				string contentkey = activeContent.Key.Split('_')[0];
				if (contentkey == validationMessage.MasterObjectKey)
				{
					if (!String.IsNullOrEmpty(validationMessage.DetailObjectKey))
					{
						Attach(activeContent.GetDetailTab(validationMessage.DetailObjectKey));
					}
					else if (String.IsNullOrEmpty(validationMessage.DetailObjectKey))
					{
						Attach(tab);
					}
				}
			}
		}

		private void Attach(UltraTab tab)
		{
			if (this.tab == null && tab != null)
			{
				this.tab = tab;
				tab.TabControl.ActiveTabChanged += new ActiveTabChangedEventHandler(TabControl_ActiveTabChanged);
			}
		}

		private void Dettach()
		{
			if (tab != null)
			{
				tab.TabControl.ActiveTabChanged -= TabControl_ActiveTabChanged;
				tab = null;
			}
		}

		private void TabControl_ActiveTabChanged(object sender, ActiveTabChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(((MasterDetailMessage)ValidationMessage).DetailObjectKey) &&
			    e.Tab.Key == ((MasterDetailMessage)ValidationMessage).DetailObjectKey)
				Activate();
		}

		protected virtual void Activate()
		{
		}

		public override void Fire()
		{
			if (tab != null)
			{
				FireTab(tab);
				secondTab = GetSecondTab(((MasterDetailMessage)ValidationMessage).SecondDetailObjectKey,
				                         (BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent);
				if (secondTab != null)
				{
					FireTab(secondTab);
				}
			}
		}

		private UltraTab GetSecondTab(string tabKey, BaseClsUI viewObject)
		{
			if (string.IsNullOrEmpty(tabKey))
				return null;
			return viewObject.GetDetailTab(tabKey);
		}

		public override void FireNotice()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Hide()
		{
			if (tab != null)
			{
				HideTab(tab);
				Dettach();
			}
			if (secondTab != null)
				HideTab(secondTab);
		}

		private void FireTab(UltraTab tab)
		{
			tab.Appearance.BackColor = Color.FromKnownColor(KnownColor.Control);
			tab.Appearance.BackColor2 = Color.Red;
			tab.Appearance.BackGradientStyle = GradientStyle.HorizontalBump;
			tab.ActiveAppearance.BackColor = Color.FromKnownColor(KnownColor.Control);
			tab.ActiveAppearance.BackColor2 = Color.Red;
			tab.ActiveAppearance.BackGradientStyle = GradientStyle.HorizontalBump;
		}

		private void HideTab(UltraTab tab)
		{
			tab.ActiveAppearance.ResetBackColor();
			tab.ActiveAppearance.ResetBackColor2();

			tab.Appearance.ResetBackColor();
			tab.Appearance.ResetBackColor2();
		}
	}
}