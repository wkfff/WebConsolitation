using System;
using System.Drawing;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Common.Services;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	/// <summary>
	/// Отвечает за отрисовку индикаторов между строками грида.
	/// </summary>
	internal class UltraGridRowDrawFilter : IUIElementDrawFilter
	{
		private IUIElementDrawFilter wrappedDrawFilter;

		public event EventHandler Invalidate;
		public event QueryStateAllowedForRowEventHandler QueryStateAllowedForRow;
		public delegate void QueryStateAllowedForRowEventHandler(UltraGridRowDrawFilter sender, QueryStateAllowedForNodeEventArgs e);

		/// <summary>
		/// Расстояние от краев элемента.
		/// </summary>
		private readonly int mvarEdgeSensitivity;
		private UltraGridRow mvarDropHighLightRow;
		private DropLinePositionEnum mvarDropLinePosition;

		/// <summary>
		/// Используется событием QueryStateAllowedForRow.
		/// </summary>
		public class QueryStateAllowedForNodeEventArgs : EventArgs
		{
			public UltraGridRow Row;
			public DropLinePositionEnum DropLinePosition;
			public DropLinePositionEnum StatesAllowed;
		}

		public UltraGridRowDrawFilter(IUIElementDrawFilter wrappedDrawFilter)
		{
			this.wrappedDrawFilter = wrappedDrawFilter;
			mvarEdgeSensitivity = 0;
		}

		/// <summary>
		/// Элемент над которым в данный момент находится указатель мышы.
		/// </summary>
		public UltraGridRow DropHightLightRow
		{
			get { return mvarDropHighLightRow; }
			set
			{
				if (mvarDropHighLightRow.Equals(value)) 
					return;
				
				mvarDropHighLightRow = value;
				PositionChanged();
			}
		}

		public DropLinePositionEnum DropLinePosition
		{
			get { return mvarDropLinePosition; }
			set
			{
				if (mvarDropLinePosition == value)
					return;
				
				mvarDropLinePosition = value;
				PositionChanged();
			}
		}

		internal IUIElementDrawFilter WrappedDrawFilter
		{
			get { return wrappedDrawFilter; }
		}

		//Когда изменяется DropNode или DropPosition, необходимо
		//создать событие Invalidate event, для того, чтобы приложение
		//могло отрисовать грид.
		//Это необходимо, т.к. DrawFilter не имеет ссылки на компонент UltraGrid.
		private void PositionChanged()
		{
			if (Invalidate == null)
				return;

			Invalidate(this, EventArgs.Empty);
		}

		/// <summary>
		/// Скрывает индикаторы перетаскивания и устанавливает DropNode в null.
		/// </summary>
		public void ClearDropHighlight()
		{
			SetDropHighlightNode(null, DropLinePositionEnum.None);
		}

		/// <summary>
		/// Устанавливает строку и индикаторы перетаскивания.
		/// </summary>
		/// <param name="row">Строка грида над которой находится указатель мыши.</param>
		/// <param name="pointInGridCoords">Позиция указателя мыши в координатах грида.</param>
		public void SetDropHighlightNode(UltraGridRow row, Point pointInGridCoords)
		{
			DropLinePositionEnum newDropLinePosition;
			UIElement rowUIElement = row.GetUIElement(true);

			//Расстояние до края элемента, используется для определения 
			//положения индикаторов перетаскивания: выше, ниже или на строке
			int distanceFromEdge = mvarEdgeSensitivity;
			
			if (distanceFromEdge == 0)
			{
				//Значение по умолчанию половина высоты строки.
				distanceFromEdge = rowUIElement.Rect.Height / 2;
			}

			//Определяем в какой части строки находится точка
			if (pointInGridCoords.Y < (rowUIElement.Rect.Top + distanceFromEdge))
			{
				newDropLinePosition = DropLinePositionEnum.AboveNode;
			}
			else
			{
				newDropLinePosition =
					pointInGridCoords.Y > (rowUIElement.Rect.Bottom - distanceFromEdge)
					? DropLinePositionEnum.BelowNode
					: DropLinePositionEnum.OnNode;
			}

			SetDropHighlightNode(row, newDropLinePosition);
		}

		private void SetDropHighlightNode(UltraGridRow row, DropLinePositionEnum dropLinePosition)
		{
			bool isPositionChanged = false;

			try
			{
				//Проверяем изменилась ли строка или положение индикатора перетаскивания
				isPositionChanged = mvarDropHighLightRow == null || 
									!mvarDropHighLightRow.Equals(row) ||
				                    mvarDropLinePosition != dropLinePosition;
			}
			catch
			{
				//Если мы попали сюда, то mvarDropHighLightRow равно null
				//и соответственно мы не можем выполнить сравнение
				if (mvarDropHighLightRow == null)
				{
					isPositionChanged = !(row == null);
				}
			}

			//устанавливаем свойства без вызова PositionChanged
			mvarDropHighLightRow = row;
			mvarDropLinePosition = dropLinePosition;

			if (isPositionChanged)
			{
				PositionChanged();
			}
		}

		public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
		{
			DrawPhase drawPhase = DrawPhase.None;
			
			if (WrappedDrawFilter != null)
				drawPhase = WrappedDrawFilter.GetPhasesToFilter(ref drawParams);
			
			if (drawParams.Element is RowUIElement)
				return drawPhase | DrawPhase.AfterDrawElement;
			if (drawParams.Element is CellUIElement)
				return drawPhase | DrawPhase.AfterDrawElement;
			
			return drawPhase;
		}

		public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
		{
			if (WrappedDrawFilter != null)
				WrappedDrawFilter.DrawElement(drawPhase, ref drawParams);

			if ((mvarDropHighLightRow == null) || (mvarDropLinePosition == DropLinePositionEnum.None))
			{
				return false;
			}

			//Запрашиваем допустимыез начения индикаторов перетаскивания
			//to pass to the event
			QueryStateAllowedForNodeEventArgs eArgs = new QueryStateAllowedForNodeEventArgs();
			eArgs.Row = mvarDropHighLightRow;
			eArgs.DropLinePosition = mvarDropLinePosition;
			eArgs.StatesAllowed = DropLinePositionEnum.All;
			QueryStateAllowedForRow(this, eArgs);

			if ((eArgs.StatesAllowed & mvarDropLinePosition) != mvarDropLinePosition)
			{
				return false;
			}

			if (drawPhase == DrawPhase.AfterDrawElement && drawParams.Element is RowUIElement)
			{
				UltraGridRow row = (UltraGridRow)drawParams.Element.GetContext(typeof(UltraGridRow));

				if (!row.Equals(mvarDropHighLightRow))
				{
					return false;
				}

				Pen p = new Pen(Color.Red, 2);

				int leftEdge = drawParams.Element.Rect.Left + 19;
				int rightEdge = drawParams.Element.Rect.Right + 1;
				int lineVPosition = drawParams.Element.Rect.Top;

				if ((mvarDropLinePosition & DropLinePositionEnum.AboveNode) == DropLinePositionEnum.AboveNode)
				{
					lineVPosition = drawParams.Element.Rect.Top;
				}
				if ((mvarDropLinePosition & DropLinePositionEnum.BelowNode) == DropLinePositionEnum.BelowNode)
				{
					lineVPosition = drawParams.Element.Rect.Bottom - 1;
				}

				lineVPosition--;

				drawParams.Graphics.DrawLine(p, leftEdge, lineVPosition, rightEdge, lineVPosition);

				drawParams.Graphics.DrawLines(p, new Point[] { 
					new Point(leftEdge, lineVPosition),
					new Point(leftEdge - 3, lineVPosition + 3),
					new Point(leftEdge - 3, lineVPosition - 3),
					new Point(leftEdge, lineVPosition),});

				drawParams.Graphics.DrawLines(p, new Point[] { 
					new Point(rightEdge, lineVPosition),
					new Point(rightEdge + 3, lineVPosition + 3),
					new Point(rightEdge + 3, lineVPosition - 3),
					new Point(rightEdge, lineVPosition),});

				return true;
			}

			return false;
		}
	}

	/// <summary>
	/// Возможные состояния перетаскивания.
	/// </summary>
	[Flags]
	public enum DropLinePositionEnum
	{
		None = 0,
		OnNode = 1,
		AboveNode = 2,
		BelowNode = 4,
		All = OnNode | AboveNode | BelowNode
	}
}
