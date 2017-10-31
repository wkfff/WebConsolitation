using System;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Common.Services;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
	internal class UltraGridCellFlagsDrawFilter : IUIElementDrawFilter
	{
		public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
		{
			if (drawParams.Element is CellUIElement)
				return DrawPhase.AfterDrawElement;
			return DrawPhase.None;
		}

		public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
		{
			if (drawPhase == DrawPhase.AfterDrawElement && drawParams.Element is CellUIElement)
			{
				if (((CellUIElement)drawParams.Element).Column.Key.ToUpper() == "FLAGS")
				{
					try
					{
						Color color = Color.White;
						if (((CellUIElement) drawParams.Element).Row.Selected || ((CellUIElement) drawParams.Element).Cell.Activated)
						{
							color = SystemColors.Highlight;
						}

						int v;
						if (((CellUIElement) drawParams.Element).Cell.Value is DBNull)
							v = 0;
						else
							v = Convert.ToInt32(((CellUIElement) drawParams.Element).Cell.Value);

						if ((v & 1) == 1)
						{
							drawParams.Graphics.DrawIcon(
								ResourceService.GetIcon("BudgetReport"),
								drawParams.Element.Rect.X + 2,
								drawParams.Element.Rect.Y + 2);
						}
						else
						{
							drawParams.Graphics.DrawImage(
								Lighter(ResourceService.GetIcon("BudgetReport").ToBitmap(), 95, color.R, color.G, color.B),
								drawParams.Element.Rect.X + 2,
								drawParams.Element.Rect.Y + 2);
						}

						if ((v & 2) == 2)
						{
							drawParams.Graphics.DrawIcon(
								ResourceService.GetIcon("SignAlert"),
								drawParams.Element.Rect.X + 2 + 16,
								drawParams.Element.Rect.Y + 2);
						}
						else
						{
							drawParams.Graphics.DrawImage(
								Lighter(ResourceService.GetIcon("SignAlert").ToBitmap(), 95, color.R, color.G, color.B),
								drawParams.Element.Rect.X + 2 + 16,
								drawParams.Element.Rect.Y + 2);
						}

						if ((v & 4) == 4)
						{
							drawParams.Graphics.DrawIcon(
								ResourceService.GetIcon("AddNewFile"),
								drawParams.Element.Rect.X + 2 + 16*2,
								drawParams.Element.Rect.Y + 2);
						}
						else
						{
							drawParams.Graphics.DrawImage(
								Lighter(ResourceService.GetIcon("AddNewFile").ToBitmap(), 95, color.R, color.G, color.B),
								drawParams.Element.Rect.X + 2 + 16*2,
								drawParams.Element.Rect.Y + 2);

						}

						return true;
					}
					catch(Exception ex)
					{
						Trace.TraceError("Ошибка при отрисовке флагов: {0}", Diagnostics.KristaDiagnostics.ExpandException(ex));
						return false;
					}
				}
			}
			return false;
		}

		private static Image Lighter(Image imgLight, int level, int nRed, int nGreen, int nBlue)
		{
			Graphics graphics = Graphics.FromImage(imgLight); //convert image to graphics object
			int conversion = (5 * (level - 50)); //calculate new alpha value
			Pen pLight = new Pen(Color.FromArgb(conversion, nRed, nGreen, nBlue), imgLight.Width * 2); //create mask with blended alpha value and chosen color as pen 
			graphics.DrawLine(pLight, -1, -1, imgLight.Width, imgLight.Height); //apply created mask to graphics object
			graphics.Save(); //save created graphics object and modify image object by that
			graphics.Dispose(); //dispose graphics object
			return imgLight; //return modified image
		}
	}
}
