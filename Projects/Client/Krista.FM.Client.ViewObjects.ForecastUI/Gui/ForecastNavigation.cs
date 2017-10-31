using System;
using System.Drawing;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.ForecastUI;
using Krista.FM.Client.ViewObjects.ForecastUI.Properties;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common.Services;
using Krista.FM.ServerLibrary.Forecast;

namespace Krista.FM.Client.ViewObjects.ForecastUI
{
	public partial class ForecastNavigation : BaseNavigationCtrl
	{
		private static ForecastNavigation instance;
		const String forecastUICaption = "Прогноз развития региона";

		internal static ForecastNavigation Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ForecastNavigation();
				}
				return instance;
			}
		}


		public ForecastNavigation()
		{
			ResourceService.RegisterStrings("Krista.FM.Client.ViewObjects.ForecastUI.Resources", typeof(ForecastNavigation).Assembly);
			ResourceService.RegisterImages("Krista.FM.Client.ViewObjects.ForecastUI.Resources", typeof(ForecastNavigation).Assembly);
			instance = this;
			Caption = forecastUICaption;//"${res:ForecastUICaption}";
		}

        public override Image TypeImage16
        {
            get { return Resources.ru.forecast_16; }
        }

        public override Image TypeImage24
        {
            get { return Resources.ru.forecast_24; }
        }

		public override void Initialize()
		{
			InitializeComponent();

			// установка обработчика предшествующего выбору элемента в навигационной области.
			ultraExplorerBar.ItemCheckStateChanged += ultraExplorerBar_ItemCheckStateChanged;
			// установка обработчика при выборе элемента в навигационной области.
			ultraExplorerBar.ItemCheckStateChanging += ultraExplorerBar_ItemCheckStateChanging;

			Workplace.ViewClosed += Workplace_ViewClosed;
			Workplace.ActiveWorkplaceWindowChanged += Workplace_ActiveWorkplaceWindowChanged;

			base.Initialize();
		}

		/// <summary>
		/// Предварение выбора элемента в навигационной области.
		/// </summary>
		private void ultraExplorerBar_ItemCheckStateChanging(object sender, Infragistics.Win.UltraWinExplorerBar.CancelableItemEventArgs e)
		{
			
		}

		/// <summary>
		/// Выбор элемента в навигационной области.
		/// </summary>
		private void ultraExplorerBar_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
		{
			if (e.Item.Checked)
			{
				if (e.Item.Key == SchemeObjectsKeys.f_S_Scenario_Key)
				{
					IViewContent vc = WorkplaceSingleton.Workplace.GetOpenedContent(e.Item.Key);
					if (vc != null)
					{
						vc.WorkplaceWindow.SelectWindow();
					}
					else
					{
						BaseClsUI viewObject = new ScenarioUI(Workplace.ActiveScheme.ForecastService);
						viewObject.Workplace = Workplace;
						viewObject.Initialize();
						viewObject.ViewCtrl.Text = e.Item.Text;

						OnActiveItemChanged(this, viewObject);

						viewObject.InitializeData();
					}
				}
				if (e.Item.Key == SchemeObjectsKeys.f_S_Valuation_Key)
				{
					IViewContent vc = WorkplaceSingleton.Workplace.GetOpenedContent(e.Item.Key);
					if (vc != null)
					{
						vc.WorkplaceWindow.SelectWindow();
					}
					else
					{
						BaseClsUI viewObject = new ValuationUI(Workplace.ActiveScheme.ForecastService);
						viewObject.Workplace = Workplace;
						viewObject.Initialize();
						viewObject.ViewCtrl.Text = e.Item.Text;

						OnActiveItemChanged(this, viewObject);

						viewObject.InitializeData();
					}
				}

				if (e.Item.Key == SchemeObjectsKeys.f_S_Form2p_Key)
				{
					IViewContent vc = WorkplaceSingleton.Workplace.GetOpenedContent(e.Item.Key);
					if (vc != null)
					{
						vc.WorkplaceWindow.SelectWindow();
					}
					else
					{
						BaseClsUI viewObject = new Form2pUI(Workplace.ActiveScheme.Form2pService);
						viewObject.Workplace = Workplace;
						viewObject.Initialize();
						viewObject.ViewCtrl.Text = e.Item.Text;

						OnActiveItemChanged(this, viewObject);

						viewObject.InitializeData();
					}
				}

			}
		}

		private void Workplace_ViewClosed(object sender, ViewContentEventArgs e)
		{
			if (ultraExplorerBar.CheckedItem == null)
				return;
			if ((e.Content.Key == SchemeObjectsKeys.f_S_Scenario_Key) || 
				(e.Content.Key == SchemeObjectsKeys.f_S_Valuation_Key) ||
			    (e.Content.Key == SchemeObjectsKeys.f_S_Form2p_Key))
			{
				ultraExplorerBar.CheckedItem.Active = false;
				ultraExplorerBar.CheckedItem.Checked = false;
			}
		}

		private void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
		{
			if (Workplace.WorkplaceLayout.ActiveContent != null)
			{
				String key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
				if (!ultraExplorerBar.Groups[0].Items.Exists(key) &&
					!ultraExplorerBar.Groups[1].Items.Exists(key))
					return;
				Workplace.SwitchTo(forecastUICaption);
				if (ultraExplorerBar.CheckedItem == null)
					return;
				if (key != ultraExplorerBar.CheckedItem.Key)
				{
					if (ultraExplorerBar.Groups[0].Items.Exists(key))
					{
						ultraExplorerBar.Groups[0].Items[key].Checked = true;
						ultraExplorerBar.Groups[0].Items[key].Active = true;
					}
					else if (ultraExplorerBar.Groups[1].Items.Exists(key))
					{
						ultraExplorerBar.Groups[1].Items[key].Checked = true;
						ultraExplorerBar.Groups[1].Items[key].Active = true;
					}
				}
			}
		}

		public override void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsComponents(components);
			base.Customize();
		}
	}
}
