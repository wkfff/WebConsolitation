using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations
{
	public class MessagesVisualizator
	{
		private readonly List<ValidationResultVisualizator> visualizators;

		public MessagesVisualizator(IValidatorMessageHolder vmh)
		{
			visualizators = new List<ValidationResultVisualizator>();
			Visualize(vmh);

		}

		public List<ValidationResultVisualizator> Visualizators
		{
			get { return visualizators; }
		}

		private void Visualize(IValidatorMessageHolder vmh)
		{
			if (vmh is ValidationMessages)
			{
				foreach (IValidatorMessageHolder item in ((ValidationMessages)vmh))
				{
					if (item != null)
						Visualize(item);
				}
			}
			else
			{
				// собираем все визуализаторы в список
				string className = vmh.GetType().FullName;
				MasterDetailCompositeVisulizator visualizator = null;
				if (typeof(UltraGridColumnValidationMessage).FullName == className)
					visualizator = new UltraGridColumnValidationVisualizator((UltraGridColumnValidationMessage)vmh);
				else if (typeof(UltraGridCellValidationMessage).FullName == className)
					visualizator = new UltraGridCellValidationVisualizator((UltraGridCellValidationMessage)vmh);
				else if (typeof(UltraGridRowCellsValidationMessage).FullName == className)
					visualizator = new UltraGridRowCellsValidationVisualizator((UltraGridRowCellsValidationMessage)vmh);
				else if (typeof(UltraGridColumnCellsValidationMessage).FullName == className)
					visualizator = new UltraGridColumnCellsValidationVisualizator((UltraGridColumnCellsValidationMessage)vmh);
				else if (typeof(UltraGridRowValidationMessage).FullName == className)
					visualizator = new UltraGridRowValidationVisualizator((UltraGridRowValidationMessage)vmh);										
				if (visualizator != null)
				{
					AfterVisualizatorCreate(visualizator);
					Visualizators.Add(visualizator);
					visualizator.Fire();
				}
			}
		}

		public virtual void AfterVisualizatorCreate(MasterDetailCompositeVisulizator visualizator)
		{
			
		}

		public void GetReport(IWin32Window parent, String fileName)
		{
			ValidationMessagesReporter vmr = new ValidationMessagesReporter(Visualizators);
			vmr.FileName = fileName;
			vmr.GetValidationReport(parent);
		}

		public void GetReport(IWin32Window parent)
		{
			GetReport(parent, "none");
		}

		public void Hide()
		{
			// освобождаем визуализаторы при переходе с одного договора на другой
			foreach (ValidationResultVisualizator visualizator in Visualizators)
			{
				visualizator.Hide();
			}
			Visualizators.Clear();
		}
	}
}