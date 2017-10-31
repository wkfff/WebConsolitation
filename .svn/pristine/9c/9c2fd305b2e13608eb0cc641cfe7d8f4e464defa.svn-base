using Krista.FM.Common.Validations.Messages;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations
{
	public abstract class ValidationResultVisualizator
	{
		private ValidationMessage validationMessage;

		public ValidationResultVisualizator(ValidationMessage validationMessage)
		{
			this.validationMessage = validationMessage;
		}

		public ValidationMessage ValidationMessage
		{
			get { return validationMessage; }
		}

		public abstract void Fire();
		public abstract void FireNotice();
		public abstract void Hide();
	}
}