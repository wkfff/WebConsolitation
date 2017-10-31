using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;

namespace Krista.FM.Client.Components
{
	public static class InfragisticsHelper
	{
		/// <summary>
		/// "Зажигает" контрол фоновой подсветкой зеленым градиентом.
		/// </summary>
		public static void BurnTool(ToolBase tool, bool burn)
		{
			if (!tool.SharedProps.Enabled)
				return;

			if (!burn)
			{
				tool.SharedProps.AppearancesSmall.Appearance.BackColor = Color.Empty;
				tool.SharedProps.AppearancesSmall.Appearance.BackColor2 = Color.Empty;
				tool.SharedProps.AppearancesSmall.Appearance.BackGradientStyle = GradientStyle.None;
				tool.SharedProps.AppearancesSmall.HotTrackAppearance.BackColor = Color.Empty;
				tool.SharedProps.AppearancesSmall.HotTrackAppearance.BackColor2 = Color.Empty;
				tool.SharedProps.AppearancesSmall.HotTrackAppearance.BackGradientStyle = GradientStyle.None;
			}
			else
			{
				tool.SharedProps.AppearancesSmall.Appearance.BackColor = Color.FromKnownColor(KnownColor.Control);
				tool.SharedProps.AppearancesSmall.Appearance.BackColor2 = Color.FromKnownColor(KnownColor.Chartreuse);
				tool.SharedProps.AppearancesSmall.Appearance.BackGradientStyle = GradientStyle.VerticalBump;
				tool.SharedProps.AppearancesSmall.HotTrackAppearance.BackColor = Color.FromKnownColor(KnownColor.Control);
				tool.SharedProps.AppearancesSmall.HotTrackAppearance.BackColor2 = Color.FromKnownColor(KnownColor.Chartreuse);
				tool.SharedProps.AppearancesSmall.HotTrackAppearance.BackGradientStyle = GradientStyle.VerticalBump;
			}
		}
	}
}
