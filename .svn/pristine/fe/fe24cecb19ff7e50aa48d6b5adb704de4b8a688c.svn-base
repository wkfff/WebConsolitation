using System;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations
{
	public class UltraGridRowValidationVisualizator : MasterDetailCompositeVisulizator
	{
		private UltraGrid grid;
		private int rowID;
		private UltraGridRow row;

		protected FireStyles fireStyle = FireStyles.Gradient;
		protected GradientStyle gradStyle = GradientStyle.VerticalBump;
		protected Boolean showToolTip = false;
		protected Image fireImage = null;
		protected Color gradColor1 = Color.Red;
		protected Color gradColor2 = Color.White;
		protected Color flatColor = Color.Red;

		public UltraGridRowValidationVisualizator(UltraGridRowValidationMessage validationMessage)
			: base(validationMessage)
		{
			rowID = validationMessage.RowID;

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

		private void Attach(UltraGrid grid)
		{
			if (this.grid == null)
			{
				this.grid = grid;

				row = UltraGridHelper.FindGridRow(grid, "ID", rowID); //а здесь ли????????? в гриде может пока еще не загружены данные

				grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
				grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
			}
		}

		private void Dettach()
		{
			if (grid != null)
			{
				grid.AfterCellUpdate -= grid_AfterCellUpdate;
				grid.InitializeLayout -= grid_InitializeLayout;

				row = null;
				grid = null;
			}
		}

		protected override void Activate()
		{
			UltraGridEx uge = ((BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent).GetActiveDetailGridEx();
			if (uge != null)
				Attach(uge.ugData);
		}

		public override void Fire()
		{
			base.Fire();

			if (row != null)
			{
				/*	row.Appearance.BackColor = Color.Red;*/
				switch (fireStyle)
				{
					case FireStyles.Gradient:
						row.Appearance.BackColor = gradColor1;
						row.Appearance.BackColor2 = gradColor2;
						row.Appearance.BackGradientStyle = gradStyle;
						break;
					case FireStyles.Color:
						row.Appearance.BackColor = flatColor;
						row.Appearance.BackGradientStyle = GradientStyle.None;
						break;
					case FireStyles.Image:
						row.Appearance.Image = fireImage;
						break;
				}
				if (showToolTip)
					row.ToolTipText = ValidationMessage.Summary;
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

		/// <summary>
		/// определяет способ выделения валидированной ячейки 
		/// </summary>
		public FireStyles FireStyle
		{
			get { return fireStyle; }
			set { fireStyle = value; }
		}

		/// <summary>
		/// Тип градиентной заливки для валидированной ячейки 
		/// </summary>
		public GradientStyle GradStyle
		{
			get { return gradStyle; }
			set { gradStyle = value; }
		}

		/// <summary>
		/// Отображать нотификацию при наведении мыши
		/// </summary>
		public bool ShowToolTip
		{
			get { return showToolTip; }
			set { showToolTip = value; }
		}

		/// <summary>
		/// Изображение для выделения ячейки
		/// </summary>
		public Image FireImage
		{
			set { fireImage = value; }
		}

		/// <summary>
		/// Первый градиентный цвет
		/// </summary>
		public Color GradColor1
		{
			get { return gradColor1; }
			set { gradColor1 = value; }
		}

		/// <summary>
		/// Второй градиентный цвет
		/// </summary>
		public Color GradColor2
		{
			get { return gradColor2; }
			set { gradColor2 = value; }
		}

		/// <summary>
		/// Цвет при ровной заливке
		/// </summary>
		public Color FlatColor
		{
			get { return flatColor; }
			set { flatColor = value; }
		}
	}
}