using System;
using System.Collections.Generic;
using Krista.FM.Common.Validations.Messages;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Message
{
	public class UltraGridCellValidationMessage : MasterDetailMessage
	{
		private int rowID = -1;
		private string columnKey;

		public UltraGridCellValidationMessage(int rowID, string columnKey, IEntityAssociation entityAssociation)
			: base(entityAssociation)
		{
			this.rowID = rowID;
			this.columnKey = columnKey;
		}

		public UltraGridCellValidationMessage(int rowID, string columnKey, String masterObjectKey, IEntityAssociation entityAssociation)
			: base(masterObjectKey, entityAssociation)
		{
			this.rowID = rowID;
			this.columnKey = columnKey;
		}

		public UltraGridCellValidationMessage(int rowID, string columnKey, String masterObjectKey)
			: base(masterObjectKey)
		{
			this.rowID = rowID;
			this.columnKey = columnKey;
		}


		public UltraGridCellValidationMessage(int rowID, string columnKey,
		                                      IEntityAssociation entityAssociation, IEntityAssociation secondAssociation)
			: base(entityAssociation, secondAssociation)
		{
			this.rowID = rowID;
			this.columnKey = columnKey;
		}

		/*public UltraGridCellValidationMessage(int rowID, string columnKey, string masterKey)
			: base(masterKey)
		{
			this.rowID = rowID;
			this.columnKey = columnKey;
		}*/

		public int RowID
		{
			get { return rowID; }
		}

		public string ColumnKey
		{
			get { return columnKey; }
		}
	}

	public class UltraGridColumnCellsValidationMessage : MasterDetailMessage
	{
		private List<int> _IDs;
		private string columnKey;

		public UltraGridColumnCellsValidationMessage(List<int> IDs, string columnKey, IEntityAssociation entityAssociation)
			: base(entityAssociation)
		{
			_IDs = IDs;
			this.columnKey = columnKey;
		}

		public UltraGridColumnCellsValidationMessage(List<int> IDs, string columnKey, string masterKey, IEntityAssociation entityAssociation)
			: base(masterKey, entityAssociation)
		{
			_IDs = IDs;
			this.columnKey = columnKey;
		}

		public List<int> IDs
		{
			get { return _IDs; }
		}

		public string ColumnKey
		{
			get { return columnKey; }
		}
	}

	public class UltraGridRowCellsValidationMessage : MasterDetailMessage
	{
		private int _rowID;
		private List<string> columnsKeys;

		public UltraGridRowCellsValidationMessage(int rowID, List<string> columnsKeys, IEntityAssociation entityAssociation)
			: base(entityAssociation)
		{
			_rowID = rowID;
			this.columnsKeys = columnsKeys;
		}

		public UltraGridRowCellsValidationMessage(int rowID, List<string> columnsKeys, string masterKey, IEntityAssociation entityAssociation)
			: base(masterKey, entityAssociation)
		{
			_rowID = rowID;
			this.columnsKeys = columnsKeys;
		}

		public UltraGridRowCellsValidationMessage(int rowID, List<string> columnsKeys, string masterKey)
			: base(masterKey)
		{
			_rowID = rowID;
			this.columnsKeys = columnsKeys;
		}

		public int RowID
		{
			get { return _rowID; }
		}

		public List<string> ColumnsKeys
		{
			get { return columnsKeys; }
		}
	}
}