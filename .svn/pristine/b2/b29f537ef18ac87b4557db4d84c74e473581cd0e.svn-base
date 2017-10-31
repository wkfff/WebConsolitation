using System;
using System.Drawing;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Components;
using Krista.FM.Common;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Gui
{
	internal abstract class WebTemplatestUIBase: TemplatestUIBase
	{
		protected WebTemplatestUIBase(string key, TemplateTypes templateType)
			: base(key, templateType)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			Grid.DrawFilter = new UltraGridCellFlagsDrawFilter();
			Grid.MouseClick += OnGridMouseClick;
			ViewObject.ugeTemplates.OnMouseEnterGridElement += OnMouseEnterGridElement;
			ViewObject.ugeTemplates.OnMouseLeaveGridElement += OnMouseLeaveGridElement;

			CommandList.Add(typeof(Commands.OpenTemplateCommand).Name, new Commands.OpenTemplateCommand());
			CommandList.Add(typeof(Commands.EditTemplateCommand).Name, new Commands.EditTemplateCommand());
			CommandList.Add(typeof(Commands.AddDocumentTemplateCommand).Name, new Commands.AddDocumentTemplateCommand());
			CommandList.Add(typeof(Commands.SaveTemplateCommand).Name, new Commands.SaveTemplateCommand());
		}

		protected override GridColumnsStates OnGetGridColumnsState(object sender)
		{
			GridColumnsStates states = base.OnGetGridColumnsState(sender);

			states[TemplateFields.Flags].IsHiden = false;
			states[TemplateFields.Code].IsHiden = false;
			states[TemplateFields.DocumentFileName].IsHiden = false;
			states[TemplateFields.Type].IsHiden = true;

			return states;
		}

		/// <summary>
		/// Возвращает тип документа используемый по умолчанию.
		/// </summary>
		internal override TemplateDocumentTypes DefaultDocumentTypes
		{
			get { return TemplateDocumentTypes.WebReport; }
		}

		private void OnGridMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			UIElement elem = Grid.DisplayLayout.UIElement.ElementFromPoint(e.Location);

			// Обрабатываем нажатие мыши на ячейку грида Flags
			if (elem.Parent != null && elem.Parent.Parent is CellUIElement &&
				((CellUIElement)elem.Parent.Parent).Cell.Column.Key.ToUpper() == TemplateFields.Flags.ToUpper())
			{
				UltraGridCell cell = ((CellUIElement) elem.Parent.Parent).Cell;
				
				// координата внутри ячейки
				Point pt = e.Location - new Size(elem.Rect.Location);

				byte t = Convert.ToByte(1 << ((pt.X) >> 4));
				if ((t & 7) != 0 && pt.Y < 17)
				{
					byte v = Convert.ToByte(cell.Value);
					cell.Value = (v & t) == t ? v & ~(7 & t) : v | (7 & t);
					cell.Row.Update();
				}
			}
		}

		private void OnMouseEnterGridElement(object sender, UIElementEventArgs e)
		{
            if (e.Element is CellUIElement && ((CellUIElement)e.Element).Column.Key.ToUpper() == TemplateFields.Flags.ToUpper())
				Grid.MouseMove += OnGridMouseMove;
		}

		private void OnMouseLeaveGridElement(object sender, UIElementEventArgs e)
		{
			if (e.Element is CellUIElement && ((CellUIElement) e.Element).Column.Key.ToUpper() == TemplateFields.Flags.ToUpper())
				Grid.MouseMove -= OnGridMouseMove;
		}

		private void OnGridMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Отображение и скрытие вылетающих подсказок для поля флагов
			Point gridPoint = Grid.PointToClient(System.Windows.Forms.Control.MousePosition);
			UIElement elem = Grid.DisplayLayout.UIElement.ElementFromPoint(gridPoint);
			if (!(elem is ImageUIElement))
				return;

			// координата внутри ячейки
			Point cellPoint = gridPoint - new Size(elem.Rect.Location);

			// номер выбранной иконки
			byte t = Convert.ToByte(1 << ((cellPoint.X) >> 4));
			if (cellPoint.Y < 17 && t <= 4)
			{
				Point toolTipPoint = gridPoint + new Size(6, 6);
				string text = EnumHelper.GetEnumItemDescription(typeof (TemplateFlags), t);

				// меняем позицию и текст хинта
				if (ViewObject.ugeTemplates.ToolTip.IsVisible &&
					text != ViewObject.ugeTemplates.ToolTip.ToolTipText)
				{
					ViewObject.ugeTemplates.ToolTip.Hide();
					ViewObject.ugeTemplates.ToolTip.ToolTipText = text;
					ViewObject.ugeTemplates.ToolTip.Show(Grid.PointToScreen(toolTipPoint));
				}

				// отображаем хинт
				if (!ViewObject.ugeTemplates.ToolTip.IsVisible)
				{
					ViewObject.ugeTemplates.ToolTip.ToolTipText = text;
					ViewObject.ugeTemplates.ToolTip.Show(Grid.PointToScreen(toolTipPoint));
				}
			}
			else
			{
				// скрываем хинт
				if (ViewObject.ugeTemplates.ToolTip.IsVisible)
					ViewObject.ugeTemplates.ToolTip.Hide();
			}
		}
	}
}
