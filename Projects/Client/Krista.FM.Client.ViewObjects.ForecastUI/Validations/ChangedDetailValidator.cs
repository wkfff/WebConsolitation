using System;
using System.Drawing;
using Infragistics.Win;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.ForecastUI.Validations
{
	public class ForecastMessagesVisualizator : MessagesVisualizator
	{
		public ForecastMessagesVisualizator(IValidatorMessageHolder vmh)
			: base(vmh)
		{
		}

		public override void AfterVisualizatorCreate(MasterDetailCompositeVisulizator visualizator)
		{
			if (visualizator is UltraGridCellValidationVisualizator)
			{
				UltraGridCellValidationVisualizator cellVis = visualizator as UltraGridCellValidationVisualizator;
				cellVis.ShowToolTip = true;
				cellVis.GradColor1 = Color.White;
				cellVis.FireStyle = FireStyles.Gradient;

				String s = cellVis.ValidationMessage.Message.Split('_')[2];
				if (s == "inc")
				{
					cellVis.GradColor2 = Color.LightGreen;
					cellVis.GradStyle = GradientStyle.ForwardDiagonal;
				}
				if (s == "dec")
				{
					cellVis.GradColor2 = Color.LightPink;
					cellVis.GradStyle = GradientStyle.BackwardDiagonal;
				}
			}

			if (visualizator is UltraGridRowValidationVisualizator)
			{
				UltraGridRowValidationVisualizator rowVis = visualizator as UltraGridRowValidationVisualizator;
				rowVis.ShowToolTip = true;
				rowVis.FireStyle = FireStyles.Color;
				rowVis.FlatColor = Color.SkyBlue;
			}
		}
	}

	
}
