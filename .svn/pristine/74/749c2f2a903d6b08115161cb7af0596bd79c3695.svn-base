using System;
using System.Collections.Generic;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common.Validations.Messages;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations
{
	/// <summary>
	/// —писок содержащий метод выделени€ валидированной €чейки при HasError = true
	/// </summary>
	public enum FireStyles
	{
		Color,
		Gradient,
		Image
	}

	public class BaseUltraGridCellValidationVisualizator : MasterDetailCompositeVisulizator
	{
		protected FireStyles fireStyle = FireStyles.Gradient;
		protected GradientStyle gradStyle = GradientStyle.VerticalBump;
		protected Boolean showToolTip = false;
		protected Image fireImage = null;
		protected Color gradColor1 = Color.Red;
		protected Color gradColor2 = Color.White;
		protected Color flatColor = Color.Red;
		
		public BaseUltraGridCellValidationVisualizator(MasterDetailMessage validationMessage) :
			base(validationMessage)
		{
		}


		/// <summary>
		/// определ€ет способ выделени€ валидированной €чейки 
		/// </summary>
		public FireStyles FireStyle
		{
			get { return fireStyle; }
			set { fireStyle = value; }
		}

		/// <summary>
		/// “ип градиентной заливки дл€ валидированной €чейки 
		/// </summary>
		public GradientStyle GradStyle
		{
			get { return gradStyle; }
			set { gradStyle = value; }
		}

		/// <summary>
		/// ќтображать нотификацию при наведении мыши
		/// </summary>
		public bool ShowToolTip
		{
			get { return showToolTip; }
			set { showToolTip = value; }
		}

		/// <summary>
		/// »зображение дл€ выделени€ €чейки
		/// </summary>
		public Image FireImage
		{
			set { fireImage = value; }
		}

		/// <summary>
		/// ѕервый градиентный цвет
		/// </summary>
		public Color GradColor1
		{
			get { return gradColor1; }
			set { gradColor1 = value; }
		}

		/// <summary>
		/// ¬торой градиентный цвет
		/// </summary>
		public Color GradColor2
		{
			get { return gradColor2; }
			set { gradColor2 = value; }
		}

		/// <summary>
		/// ÷вет при ровной заливке
		/// </summary>
		public Color FlatColor
		{
			get { return flatColor; }
			set { flatColor = value; }
		}

		protected void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			Fire();
		}
	}

	public class UltraGridCellValidationVisualizator : BaseUltraGridCellValidationVisualizator
	{
		private UltraGrid _grid;
		private readonly int _rowID;
		private readonly string columnKey;

		public UltraGridCellValidationVisualizator(UltraGridCellValidationMessage validationMessage)
			: base(validationMessage)
		{
			_rowID = validationMessage.RowID;
			columnKey = validationMessage.ColumnKey;

			BaseClsUI activeContent = (BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent;
			string contentkey = activeContent.Key.Split('_')[0];
			if (contentkey == validationMessage.MasterObjectKey)
			{
				if (!String.IsNullOrEmpty(validationMessage.DetailObjectKey)
				    && activeContent.GetActiveDetailTab().Key == validationMessage.DetailObjectKey)
				{
					Attach(activeContent.GetActiveDetailGridEx().ugData);
				}
				else if (String.IsNullOrEmpty(validationMessage.DetailObjectKey))
				{
					Attach(activeContent.UltraGridExComponent.ugData);
				}
			}
		}

		private UltraGridCell GetCell(UltraGrid grid)
		{
			if (grid == null)
				return null;
			UltraGridRow row = UltraGridHelper.FindGridRow(grid, "ID", _rowID);
            if (row == null)
                return null;
			return row.Band.Columns.Exists(columnKey) ? row.Cells[columnKey] : null;
		}

		private void Attach(UltraGrid grid)
		{
			if (_grid == null)
			{
				_grid = grid;

				grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
				grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
			}
		}

		private void Dettach()
		{
			if (_grid != null)
			{
				_grid.AfterCellUpdate -= grid_AfterCellUpdate;
				_grid.InitializeLayout -= grid_InitializeLayout;

				_grid = null;
			}
		}

		protected override void Activate()
		{
			Attach(((BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent).GetActiveDetailGridEx().ugData);
		}

		public override void Fire()
		{
			base.Fire();
			UltraGridCell cell = GetCell(_grid);
			if (cell != null)
			{
				switch(fireStyle)
				{
					case FireStyles.Gradient:
						cell.Appearance.BackColor = gradColor1;
						cell.Appearance.BackColor2 = gradColor2;
						cell.Appearance.BackGradientStyle = gradStyle;
						break;
					case FireStyles.Color:
						cell.Appearance.BackColor = flatColor;
						cell.Appearance.BackGradientStyle = GradientStyle.None;
						break;
					case FireStyles.Image:
						cell.Appearance.Image = fireImage;
						break;
				}
				if (ShowToolTip)
					cell.ToolTipText = ValidationMessage.Summary;
			}
		}

		public override void Hide()
		{
			base.Hide();

			UltraGridCell cell = GetCell(_grid);
			if (cell != null)
			{
				cell.Appearance.ResetBackColor();
				cell.Appearance.ResetBackColor2();
				if (fireImage != null) // на случай если картинка была установлена не валидатором
					cell.Appearance.ResetImage();
			}

			Dettach();
		}
		
		private void grid_AfterCellUpdate(object sender, CellEventArgs e)
		{
			if (e.Cell.Column.Key == columnKey.ToUpper() && Convert.ToInt32(e.Cell.Row.Cells["ID"].Value) == _rowID)
			{
				Hide();
			}
		}
	}

	public class UltraGridColumnCellsValidationVisualizator : BaseUltraGridCellValidationVisualizator
	{
		private UltraGrid grid;
		private readonly List<int> IDs;
		private readonly string columnKey;

		public UltraGridColumnCellsValidationVisualizator(UltraGridColumnCellsValidationMessage validationMessage)
			: base(validationMessage)
		{
			IDs = validationMessage.IDs;
			columnKey = validationMessage.ColumnKey;

			BaseClsUI activeContent = (BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent;
			if (activeContent.Key == validationMessage.MasterObjectKey)
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

		private UltraGridCell GetCell(UltraGrid grid, int rowID)
		{
			if (grid == null)
				return null;
			UltraGridRow row = UltraGridHelper.FindGridRow(grid, "ID", rowID);
			return row.Band.Columns.Exists(columnKey) ? row.Cells[columnKey] : null;
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
			if (grid != null)
			{
				grid.AfterCellUpdate -= grid_AfterCellUpdate;
				grid.InitializeLayout -= grid_InitializeLayout;

				grid = null;
			}
		}

		protected override void Activate()
		{
			Attach(((BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent).GetActiveDetailGridEx().ugData);
		}

		public override void Fire()
		{
			base.Fire();
			
			switch (fireStyle)
			{
				case FireStyles.Gradient:
					foreach (int ID in IDs)
					{
						UltraGridCell cell = GetCell(grid, ID);
						if (cell != null)
						{
							cell.Appearance.BackColor = gradColor1;
							cell.Appearance.BackColor2 = gradColor2;
							cell.Appearance.BackGradientStyle = gradStyle;
							if (ShowToolTip)
								cell.ToolTipText = ValidationMessage.Summary;
						}
					}
					break;
				case FireStyles.Color:
					foreach (int ID in IDs)
					{
						UltraGridCell cell = GetCell(grid, ID);
						if (cell != null)
						{
							cell.Appearance.BackColor = flatColor;
							cell.Appearance.BackGradientStyle = GradientStyle.None;
							if (ShowToolTip)
								cell.ToolTipText = ValidationMessage.Summary;
						}
					}
					break;
				case FireStyles.Image:
					foreach (int ID in IDs)
					{
						UltraGridCell cell = GetCell(grid, ID);
						if (cell != null)
						{
							cell.Appearance.Image = fireImage;
							if (ShowToolTip)
								cell.ToolTipText = ValidationMessage.Summary;	
						}
					}
					break;
			}
		}

		public override void Hide()
		{
			base.Hide();
			foreach (int ID in IDs)
			{
				UltraGridCell cell = GetCell(grid, ID);
				if (cell != null)
				{
					cell.Appearance.ResetBackColor();
					cell.Appearance.ResetBackColor2();
					if (fireImage != null) // на случай если картинка была установлена не валидатором
						cell.Appearance.ResetImage();
				}
			}
			Dettach();
		}

		private void grid_AfterCellUpdate(object sender, CellEventArgs e)
		{
			if (e.Cell.Column.Key == columnKey.ToUpper() && IDs.Contains(Convert.ToInt32(e.Cell.Row.Cells["ID"].Value)))
			{
				Hide();
			}
		}
	}

	public class UltraGridRowCellsValidationVisualizator : BaseUltraGridCellValidationVisualizator
	{
		private UltraGrid _grid;
		private readonly int _rowID;
		private readonly List<string> _columnsKeys;

		public UltraGridRowCellsValidationVisualizator(UltraGridRowCellsValidationMessage validationMessage)
			: base(validationMessage)
		{
			_rowID = validationMessage.RowID;
			_columnsKeys = validationMessage.ColumnsKeys;

			BaseClsUI activeContent = (BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent;
			if (activeContent.Key == validationMessage.MasterObjectKey)
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

		private UltraGridCell GetCell(UltraGrid grid, string columnKey)
		{
			if (grid == null)
				return null;
			UltraGridRow row = UltraGridHelper.FindGridRow(grid, "ID", _rowID);
            if (row == null)
                return null;
			return row.Band.Columns.Exists(columnKey) ? row.Cells[columnKey] : null;
		}

		private void Attach(UltraGrid grid)
		{
			if (_grid == null)
			{
				_grid = grid;

				grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
				grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
			}
		}

		private void Dettach()
		{
			if (_grid != null)
			{
				_grid.AfterCellUpdate -= grid_AfterCellUpdate;
				_grid.InitializeLayout -= grid_InitializeLayout;

				_grid = null;
			}
		}

		protected override void Activate()
		{
			Attach(((BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent).GetActiveDetailGridEx().ugData);
		}

		public override void Fire()
		{
			base.Fire();

			switch (fireStyle)
			{
				case FireStyles.Gradient:
					foreach (string columnKey in _columnsKeys)
					{
						UltraGridCell cell = GetCell(_grid, columnKey);
						if (cell != null)
						{
							cell.Appearance.BackColor = gradColor1;
							cell.Appearance.BackColor2 = gradColor2;
							cell.Appearance.BackGradientStyle = gradStyle;
							if (ShowToolTip)
								cell.ToolTipText = ValidationMessage.Summary;
						}
					}
					break;
				case FireStyles.Color:
					foreach (string columnKey in _columnsKeys)
					{
						UltraGridCell cell = GetCell(_grid, columnKey);
						if (cell != null)
						{
							cell.Appearance.BackColor = flatColor;
							cell.Appearance.BackGradientStyle = GradientStyle.None;
							if (ShowToolTip)
								cell.ToolTipText = ValidationMessage.Summary;
						}
					}
					break;
				case FireStyles.Image:
					foreach (string columnKey in _columnsKeys)
					{
						UltraGridCell cell = GetCell(_grid, columnKey);
						if (cell != null)
						{
							cell.Appearance.Image = fireImage;
							if (ShowToolTip)
								cell.ToolTipText = ValidationMessage.Summary;
						}
					}
					break;
			}
		}

		public override void Hide()
		{
			base.Hide();
			foreach (string columnKey in _columnsKeys)
			{
				UltraGridCell cell = GetCell(_grid, columnKey);
				if (cell != null)
				{
					cell.Appearance.ResetBackColor();
					cell.Appearance.ResetBackColor2();
					if (fireImage != null) // на случай если картинка была установлена не валидатором
						cell.Appearance.ResetImage();
				}
			}
			Dettach();
		}
				

		private void grid_AfterCellUpdate(object sender, CellEventArgs e)
		{
			if (_columnsKeys.Contains(e.Cell.Column.Key) && Convert.ToInt32(e.Cell.Row.Cells["ID"].Value) == _rowID)
			{
				Hide();
			}
		}
	}
}