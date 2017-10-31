using System;
using System.Data;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations
{
	public abstract class AbstractMasterValidator
	{
		protected string errorMessage = String.Empty;

		public abstract IValidatorMessageHolder Validate(DataRow masterRow, string masterKey);

		public virtual string ErrorMessage
		{
			get { return errorMessage; }
			set { errorMessage = value; }
		}
	}
}