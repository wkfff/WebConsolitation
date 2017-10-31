using System;
using System.Drawing;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.ForecastUI.Validations.Visualizations
{
    /*class UltraGridRowValidationVisualizator : MasterDetailCompositeVisulizator
    {
        private UltraGrid grid;
        private int rowID;
        private UltraGridRow row;

        public UltraGridRowValidationVisualizator(UltraGridRowValidationMessage validationMessage)
            : base(validationMessage)
        {
            rowID = validationMessage.RowID;

            ForecastUI activeContent = (ForecastUI)WorkplaceSingleton.Workplace.ActiveContent;
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

        private void Attach(UltraGrid grid)
        {
            if (this.grid == null)
            {
                this.grid = grid;

//                row = UltraGridHelper.FindGridRow(grid, "ID", rowID);

                grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
                grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
            }
        }

        private void Dettach()
        {
            grid.AfterCellUpdate -= grid_AfterCellUpdate;
            grid.InitializeLayout -= grid_InitializeLayout;

            row = null;
            grid = null;
        }

        protected override void Activate()
        {
            Attach(((ForecastUI)WorkplaceSingleton.Workplace.ActiveContent).GetActiveDetailGridEx().ugData);
        }

        public override void Fire()
        {
            base.Fire();

			if (grid != null)
			{
				row = UltraGridHelper.FindGridRow(grid, "ID", rowID);
				if (row != null)
					row.Appearance.BackColor = Color.Red;
			}
        }

        public override void Hide()
        {
            base.Hide();

            if (row != null)
            {
                row.Appearance.ResetBackColor();

                Dettach();
            }
        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            Fire();
        }

        private void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Row == row)
            {
                Hide();
            }
        }
    }*/
}
