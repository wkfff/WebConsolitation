using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message
{
	public class UltraGridColumnValidationMessage : MasterDetailMessage
	{
		private string columnKey;

		public UltraGridColumnValidationMessage(string columnName, IEntityAssociation entityAssociation)
			: base(entityAssociation)
		{
			this.columnKey = columnName;
		}

		public UltraGridColumnValidationMessage(string columnName, IEntityAssociation entityAssociation, IEntityAssociation secondAssociation)
			: base(entityAssociation, secondAssociation)
		{
			this.columnKey = columnName;
		}

		public string ColumnKey
		{
			get { return columnKey; }
		}
	}
}