using System;
using System.Data;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations
{
	/// <summary>
	/// Выполняет проверки для связки мастер(строка)-деталь(таблица).
	/// </summary>
	public abstract class AbstractMasterDetailValidator
	{
		protected string errorMessage = String.Empty;
		private IEntityAssociation entityAssociation;

		public abstract IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable);

		public virtual string ErrorMessage
		{
			get { return errorMessage; }
			set { errorMessage = value; }
		}

		public IEntityAssociation Association
		{
			get { return entityAssociation; }
			set { entityAssociation = value; }
		}
	}

	public abstract class AbstractMasterDetailRowValidator
	{
		protected string errorMessage = String.Empty; 
		private IEntityAssociation entityAssociation;

		public abstract IValidatorMessageHolder Validate(DataRow masterRow, DataRow detailRow);

		public virtual string ErrorMessage
		{
			get { return errorMessage; }
			set { errorMessage = value; }
		}

		public IEntityAssociation Association
		{
			get { return entityAssociation; }
			set { entityAssociation = value; }
		}
	}
}