﻿using System;
using System.Data;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations
{
	public abstract class AbstractMasterDetailDetailValidator
	{
		protected string errorMessage = String.Empty;
		private IEntityAssociation entityAssociation;
		private IEntityAssociation secondAssociation;

		public abstract IValidatorMessageHolder Validate(DataRow masterRow, DataTable detailTable, DataTable boundDetailTable);

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

		public IEntityAssociation SecondAssociation
		{
			get { return secondAssociation; }
			set { secondAssociation = value; }
		}
	}
}