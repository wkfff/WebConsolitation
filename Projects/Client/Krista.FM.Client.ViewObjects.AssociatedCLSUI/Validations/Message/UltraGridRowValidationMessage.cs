using System;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message
{
	public class UltraGridRowValidationMessage : MasterDetailMessage
	{
		private int rowID = -1;

		public UltraGridRowValidationMessage(int rowID, IEntityAssociation entityAssociation)
			: base(entityAssociation)
		{
			this.rowID = rowID;
		}

		public UltraGridRowValidationMessage(int rowID, String masterObjectKey, IEntityAssociation entityAssociation)
			: base(masterObjectKey, entityAssociation)
		{
			this.rowID = rowID;
		}

		public UltraGridRowValidationMessage(int rowID, IEntityAssociation entityAssociation, IEntityAssociation secondAssociation)
			: base(entityAssociation, secondAssociation)
		{
			this.rowID = rowID;
		}

		public int RowID
		{
			get { return rowID; }
		}
	}
}