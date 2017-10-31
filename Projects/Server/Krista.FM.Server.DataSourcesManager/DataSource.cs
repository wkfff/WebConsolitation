using System;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataSourcesManager
{
	/// <summary>
	/// �������� ������
	/// </summary>
	internal class DataSource : DisposableObject, IDataSource
	{
		private int _ID = -1;
		private string supplierCode;
		private string dataCode;
        private string dataName;
        private ParamKindTypes parametersType;
		private DataSourceManager dataSourceManager;
		private string name = String.Empty;
        private string terrirory = String.Empty;
		private int year = 0;
		private int month = 0;
		private string variant = String.Empty;
		private int quarter = 0;
	    private int locked = 0;
	    private int deleted = 0;


		public DataSource(DataSourceManager dataSourceManager)
		{
			if (dataSourceManager == null)
				throw new ArgumentNullException("dataSourceManager");
			this.dataSourceManager = dataSourceManager;
		}

        private void CheckWriteAccess()
        {
            if (_ID != -1)
                throw new InvalidOperationException("�������� ������ ��������.");
        }

		#region ���������� ���������� IDataSource

		#region ���������� �������

		/// <summary>
		/// ������� ��� ������ ���������� �� ����� ���������
		/// </summary>
		public void DeleteData()
		{
			// ��� ���� ����������� ������� ��������� ����� �������� �������� �������	
		}
        
        /// <summary>
        /// ����������(��������) ���������.
        /// </summary>
        public void LockDataSource()
        {
            locked = 1;
            UpdateDataSourceState();
            WriteIntoProtocol(DataSourceEventKind.ceSourceLock, String.Format("�������� ID={0} ������ �� ���������", ID));
        }

        /// <summary>
        /// �������� ���������.
        /// </summary>
        public void UnlockDataSource()
        {
            locked = 0;
            UpdateDataSourceState();
            WriteIntoProtocol(DataSourceEventKind.ceSourceUnlock, String.Format("�������� ID={0} ������ ��� ���������", ID));
        }

        /// <summary>
        /// ������� ������ �� ��������� � ������ ��������� ������� ��������.
        /// </summary>
        /// <param name="dependedObjects">������� ��������� ��������.</param>
        public DataTable RemoveWithData(DataTable dependedObjects)
        {
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;
            DataTable dtResult = CreateDtResult();
            try
            {
                db.BeginTransaction();
                // �������� �� ��������� �������� � ������� ������ �� ���������.
                foreach (DataRow row in dependedObjects.Rows)
                {
                    // ������� �� �������� ������.
                    if ((row["ObjectType"].ToString()).Equals("������� ������"))
                    {
                        DataTable innerResult = ProcessLockedVariant(db, row["FullDBName"].ToString());
                        foreach (DataRow innerRow in innerResult.Rows)
                        {
                            AddIfNoExistsResultRow(ref dtResult, innerRow);
                       }
                    }
                }

                // ����� �� ���� ���������.
                foreach (DataRow row in dependedObjects.Rows)
                {
                    if (!(row["ObjectType"].ToString()).Equals("������� ������"))
                    {
                        DataTable innerResult = ProcessDeleteChildRecord(db, row["FullDBName"].ToString());
                        foreach (DataRow innerRow in innerResult.Rows)
                        {
                            AddIfNoExistsResultRow(ref dtResult, innerRow);
                        }
                    }
                }
                if (dtResult.Rows.Count > 0)
                {
                    db.Rollback();
                    db.Dispose();
                    return dtResult;
                } 

                // ������ ��������� ������� ��������.
                deleted = 1;
                db.ExecQuery(string.Format("update HUB_DataSources set DELETED = ? where ID = ?"),
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("Deleted", deleted),
                    db.CreateParameter("ID", ID));

                db.Commit();
                WriteIntoProtocol(DataSourceEventKind.ceSourceDelete, String.Format("�������� ID={0} ������", ID));
                return null;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw new Exception("������ ��� �������� ��������� ������", ex);
            }
            finally
            {
                db.Dispose();
            }
        }

        // ��������� � ������� ����������� ������, ���� ����� ��� �� ����.
        private static void AddIfNoExistsResultRow(ref DataTable dtResult, DataRow innerRow)
        {
            foreach (DataRow row in dtResult.Rows)
            {
                if (row["FullDBName"].ToString() == innerRow["FullDBName"].ToString() &&
                    row["ID"].ToString() == innerRow["ID"].ToString() )
                {
                    return;
                }
                
            }
            dtResult.ImportRow(innerRow);
        }

	    private DataTable ProcessDeleteChildRecord(IDatabase db, string clsName)
	    {
            // ���� ������������� �� �����.
	        IClassifier cls = null;
	        foreach (IClassifier item in this.dataSourceManager.Scheme.Classifiers.Values)
	        {
	            if (item.FullDBName.Equals(clsName))
	            {
	                cls = item;
	                break;
	            }
	        }

	        // �������� ������, ������� ������ ���� �������
	        DataTable dtRefKdID = (DataTable)db.ExecQuery(string.Format
                  ("select ID from {0} where sourceID = ?", cls.FullDBName),
                  QueryResultTypes.DataTable, db.CreateParameter("ID", ID));

            DataTable dtResult = CreateDtResult();

	        // ��������� ��� ������� ID
            foreach (DataRow refKdIDRow in dtRefKdID.Rows)
	        {
                // ������������� ����������.
                foreach (IEntityAssociation item in cls.Associated.Values)
	            {
                    // ������� ��������� �� ������ ����������
                    DataTable dtOtherSourceData = new DataTable();
                    if (item.RoleData.ClassType != ClassTypes.Table)
                    {
                        dtOtherSourceData = (DataTable)(db.ExecQuery
                            (String.Format(
                            "select ID from {0} where {1} = ? and sourceID <> ?", item.RoleData.FullDBName, item.RoleDataAttribute.Name),
                            QueryResultTypes.DataTable,
                            db.CreateParameter("ID", Convert.ToInt32(refKdIDRow[0])),
                            db.CreateParameter("sourceID", ID)));
                    }

                    // ���� ��� ����
	                foreach (DataRow otherSourceDataRow  in dtOtherSourceData.Rows)
	                {
                        // ������� ��� �������������� � ID ������ � ������ ������
                        DataRow resultRow = dtResult.NewRow();
                        resultRow["FullCaption"] = item.RoleData.FullCaption;
                        resultRow["FullDBName"] = item.RoleData.FullDBName;
                        resultRow["ID"] = otherSourceDataRow[0];
                        resultRow["ObjectType"] = item.RoleData.GetObjectType();
                        dtResult.Rows.Add(resultRow);
	                }

                    // ���� � ��������� ������� ������
                    if (item.RoleData is IFactTable)
                    {
                        // ����� ��������� ���������, �� ��������� �� ��� �� ��������������� �������
                        DataTable innerResult = ProcessLockedVariant(db, item.RoleData.FullDBName);
                        foreach (DataRow innerRow in innerResult.Rows)
                        {
                            dtResult.ImportRow(innerRow);
                        }

                    }
                    if (item.RoleData.ClassType != ClassTypes.Table)
                    {
                        if (dtResult.Rows.Count == 0)
                        {
                            // �������� ������� ������
                            db.ExecQuery(
                                String.Format("delete from {0} where {1} = ? and sourceID = ?",
                                              item.RoleData.FullDBName, item.RoleDataAttribute.Name),
                                QueryResultTypes.NonQuery,
                                db.CreateParameter("RefKd", Convert.ToInt32(refKdIDRow[0])),
                                db.CreateParameter("sourceID", ID));
                        }
                    }
	            }
	        }
            if (dtResult.Rows.Count == 0)
            // ������� ������ �� ���������
            {
                db.ExecQuery(String.Format("delete from {0} where sourceID = ?", cls.FullDBName),
                             QueryResultTypes.NonQuery,
                             db.CreateParameter("sourceID", ID));
            }
            return dtResult;
	    }

	    /// <summary>
        /// ��������� ���������� � ��������������� ���������.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="row"></param>
	    private DataTable ProcessLockedVariant(IDatabase db, string factTableName)
	    {
            // ���� ������� �� �����.
	        IFactTable factTable = null;
	        foreach (IFactTable item in this.dataSourceManager.Scheme.FactTables.Values)
	        {
	            if (item.FullDBName.Equals(factTableName))
	            {
	                factTable = item;
	                break;
	            }
	        }

	        DataTable dtResult = CreateDtResult();

	        //������������� ���������� ������� ������
	        foreach (IAssociation association in factTable.Associations.Values)
	        {
	            // ���� � ��� �������
	            if (association.RoleBridge is IVariantDataClassifier)
	            {
	                IEntity variantCls = association.RoleBridge;
	                // �������� ID ������� ��������, �� ������� ��������� �������
	                DataTable dtRefVariantID = (DataTable)db.ExecQuery(string.Format
                       ("select distinct {0} from {1} where sourceID = ?", association.RoleDataAttribute.Name, factTable.FullDBName),
                       QueryResultTypes.DataTable, db.CreateParameter("ID", ID));

                    // ������� ������� �� ���� ������� �������������
	                foreach (DataRow dataRow in dtRefVariantID.Rows)
	                {
                        int count = Convert.ToInt32(db.ExecQuery
                            (String.Format("select count (*) from {0} where ID = ? and variantcompleted = 1", variantCls.FullDBName),
                            QueryResultTypes.Scalar, db.CreateParameter("ID", Convert.ToInt32(dataRow[0]))));
                        if (count > 0)
                        {
                            // ������� ��� �������� � ID ������ � ������ ������
                            DataRow resultRow = dtResult.NewRow();
                            resultRow["FullCaption"] = variantCls.FullCaption;
                            resultRow["FullDBName"] = variantCls.FullDBName;
                            resultRow["ID"] = dataRow[0];
                            resultRow["ObjectType"] = variantCls.GetObjectType();
                            dtResult.Rows.Add(resultRow);
                        }
	                }
	            }
	        }
            // ���� ������ �� ����
            if (dtResult.Rows.Count == 0)
            {
                // �������� ������� ������.
                db.ExecQuery(String.Format("delete from {0} where sourceID = ?", factTable.FullDBName),
                     QueryResultTypes.NonQuery,
                     db.CreateParameter("sourceID", ID));
            }
            return dtResult;
	    }

	    private static DataTable CreateDtResult()
	    {
	        DataTable dtResult = new DataTable();
	        DataColumn colFullCaption = new DataColumn("FullCaption", Type.GetType("System.String"));
	        DataColumn colFullDBName = new DataColumn("FullDBName", Type.GetType("System.String"));
	        DataColumn colID = new DataColumn("ID", Type.GetType("System.Int32"));
            DataColumn colObjectType = new DataColumn("ObjectType", Type.GetType("System.String"));
	        dtResult.Columns.Add(colFullCaption);
	        dtResult.Columns.Add(colFullDBName);
	        dtResult.Columns.Add(colID);
	        dtResult.Columns.Add(colObjectType);
	        return dtResult;
	    }

        /// <summary>
        /// ���������� ����� ������� ��������� ������ � ���������
        /// </summary>
        /// <returns></returns>
        public int? FindInDatabase()
        {
            return dataSourceManager.DataSources.FindDataSource(this);
        }

        /// <summary>
        /// ��������� ������ �������� ������ � ��������� � � ���� ������
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            return dataSourceManager.DataSources.Add(this);
        }

	    public void ConfirmDataSource()
	    {
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;
	        try
	        {
                UpdateDataSourceConfirmed(db, 1);
                WriteIntoProtocol(DataSourceEventKind.ceSourceLock, String.Format("�������� ID={0} ��������� � ��������� ���������", ID));
	        }
	        finally
	        {
	            db.Dispose();
	        }
	    }

	    public void UnConfirmDataSource()
	    {
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;
            try
            {
                UpdateDataSourceConfirmed(db, 0);
                WriteIntoProtocol(DataSourceEventKind.ceSourceLock, String.Format("�������� ID={0} ��������� � ��������� �� ��������", ID));
            }
            finally
            {
                db.Dispose();
            }
	    }

	    #endregion ���������� �������


		#region ���������� �������

		/// <summary>
		/// ID ��������� ������
		/// </summary>
		public int ID 
		{ 
			get { return _ID; }
			set { _ID = value; }
		}

		/// <summary>
		/// ��� ���������� ������
		/// </summary>
		public string SupplierCode 
		{ 
			get { return supplierCode; }
			set {
                CheckWriteAccess();
                supplierCode = value; 
            }
		}

		/// <summary>
		/// ���������� ����� ����������� ����������
		/// </summary>
		public string DataCode	
		{ 
			get	{ return dataCode; }
            set
            {
                CheckWriteAccess();
                dataCode = value;
            }
		}

        /// <summary>
        /// ������������ ����������� ����������
        /// </summary>
        public String DataName
        {
            get { return dataName; }
            set
            {
                CheckWriteAccess();
                dataName = value;
            }
        }
        
        /// <summary>
		/// ��� ���������� ��������� ������
		/// </summary>
		public ParamKindTypes ParametersType
		{ 
			get	{ return parametersType; }
            set
            {
                CheckWriteAccess();
                parametersType = value;
            }
		}

		/// <summary>
		/// ��� ����������: ������������ �������
		/// </summary>
		public string BudgetName
		{ 
			get	{ return name; }
            set
            {
                CheckWriteAccess();
                name = value;
            }
		}

        /// <summary>
        /// ��� ����������: ����������
        /// </summary>
        public string Territory
        {
            get { return terrirory; }
            set
            {
                CheckWriteAccess();
                terrirory = value;
            }
        }

        /// <summary>
		/// ��� ����������: ���
		/// </summary>
		public int Year 
		{ 
			get	{ return year; }
            set
            {
                CheckWriteAccess();
                year = value;
            }
		}

		/// <summary>
		/// ��� ����������: �����
		/// </summary>
		public int Month
		{ 
			get	{ return month; }
            set
            {
                CheckWriteAccess();
                month = value;
            }
		}

		/// <summary>
		/// ��� ����������: �������
		/// </summary>
		public string Variant 
		{ 
			get	{ return variant; }
            set
            {
                CheckWriteAccess();
                variant = value;
            }
		}

		/// <summary>
		/// ��� ����������: �������
		/// </summary>
		public int Quarter
		{
			get { return quarter; }
            set
            {
                CheckWriteAccess();
                quarter = value;
            }
		}

	    /// <summary>
	    /// ������� ���������� ���������.
	    /// </summary>
        public int Locked
	    {
            get { return locked; }
	    }

	    /// <summary>
	    /// ������� �������� ���������.
	    /// </summary>
	    public int Deleted
	    {
            get { return deleted; }
	    }
        
		#endregion ���������� �������

		#endregion ���������� ���������� IDataSource

        
        private void UpdateDataSourceState()
        {
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;
            try
            {
                UpdateDataSourceState(db);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// �������� ���� ��������� ��������� � ���� ������.
        /// </summary>
        /// <param name="db">������ ������� � ���� ������.</param>
        private void UpdateDataSourceState(IDatabase db)
        {
            db.ExecQuery(String.Format("update HUB_DataSources set locked = ? where ID = ?"),
                QueryResultTypes.NonQuery,
                db.CreateParameter("Locked", locked),
                db.CreateParameter("SourceID", ID));
        }

        /// <summary>
        /// �������� ���� �������� ��������� ��� ���
        /// </summary>
        /// <param name="db">������ ������� � ���� ������.</param>
        private void UpdateDataSourceConfirmed(IDatabase db, int confirmed)
        {
            db.ExecQuery(String.Format("update HUB_DataSources set confirmed = ? where ID = ?"),
                QueryResultTypes.NonQuery,
                db.CreateParameter("Locked", confirmed),
                db.CreateParameter("SourceID", ID));
        }

        /// <summary>
        /// ������ ������� � �������� �������� ������������
        /// </summary>
        /// <param name="kind">��� �������</param>
        /// <param name="eventMsg">����� ���������</param>
        private void WriteIntoProtocol(DataSourceEventKind kind, string eventMsg)
        {
            IDataSourceProtocol log = null;
            try
            {
               log = (IDataSourceProtocol)dataSourceManager.Scheme.GetProtocol("Krista.FM.Server.Scheme.dll");
               log.WriteEventIntoDataSourceProtocol(kind, this.ID, eventMsg);
            }
            finally
            {
                if (log != null)
                    log.Dispose();
            }
        }
	}
}
