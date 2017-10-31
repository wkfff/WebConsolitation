using System;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations
{
	public class UltraGridColumnValidationVisualizator : MasterDetailCompositeVisulizator
	{
		private UltraGrid grid;
		private string columnKey;

		public UltraGridColumnValidationVisualizator(UltraGridColumnValidationMessage validationMessage)
			: base(validationMessage)
		{
			columnKey = validationMessage.ColumnKey;

			if (WorkplaceSingleton.Workplace.ActiveContent is BaseClsUI)
			{
				BaseClsUI activeContent = (BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent;
				string contentkey = activeContent.Key.Split('_')[0];
				if (contentkey == validationMessage.MasterObjectKey)
				{
					if (!String.IsNullOrEmpty(validationMessage.DetailObjectKey) && activeContent.GetActiveDetailTab().Key == validationMessage.DetailObjectKey)
					{
						Attach(activeContent.GetActiveDetailGridEx().ugData);
					}
					else if (String.IsNullOrEmpty(validationMessage.DetailObjectKey))
					{
						Attach(activeContent.UltraGridExComponent.ugData);
					}
				}
			}
		}

		private void Attach(UltraGrid grid)
		{
			if (this.grid == null)
			{
				this.grid = grid;

				grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
				grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
			}
		}

		private void Dettach()
		{
			grid.AfterCellUpdate -= grid_AfterCellUpdate;
			grid.InitializeLayout -= grid_InitializeLayout;

			grid = null;
		}

		protected override void Activate()
		{
			Attach(((BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent).GetActiveDetailGridEx().ugData);
		}

		public override void Fire()
		{
			base.Fire();
			
			if (grid != null)
			{
				grid.DisplayLayout.Bands[0].Columns[columnKey].Header.Appearance.BackColor = Color.FromKnownColor(KnownColor.Control);
				grid.DisplayLayout.Bands[0].Columns[columnKey].Header.Appearance.BackColor2 = Color.Red;
				grid.DisplayLayout.Bands[0].Columns[columnKey].Header.Appearance.BackGradientStyle = GradientStyle.HorizontalBump;
			}
		}

		public override void Hide()
		{
			base.Hide();

			if (grid != null)
			{
				grid.DisplayLayout.Bands[0].Columns[columnKey].Header.Appearance.ResetBackColor();
				grid.DisplayLayout.Bands[0].Columns[columnKey].Header.Appearance.ResetBackColor2();

				Dettach();
			}
		}

		private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			Fire();
		}

		private void grid_AfterCellUpdate(object sender, CellEventArgs e)
		{
			if (e.Cell.Column.Key == columnKey.ToUpper())
			{
				Hide();
			}
		}
	}
}