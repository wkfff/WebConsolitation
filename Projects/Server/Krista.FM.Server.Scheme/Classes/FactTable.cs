using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.OLAP.Processor;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    internal class FactTable : DataSourceDividedClass, IFactTable
    {
        public FactTable(string key, ServerSideObject owner, string semantic, string name, SubClassTypes subClassType, ServerSideObjectStates state)
            : base(key, owner, semantic, name, ClassTypes.clsFactData, subClassType, true, state, SchemeClass.ScriptingEngineFactory.FactEntityScriptingEngine)
        {
            LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
            try
            {
                SessionContext.SetSystemContext();

                tagElementName = "DataTable";

                //this.Attributes[DataAttribute.IDColumnName].IsNullable = true;

                // ��������� ��������� ��������
                this.Attributes.Add(DataAttribute.SystemSourceID);

                if (subClassType == SubClassTypes.Pump)
                {
                    Attributes.Add(DataAttribute.SystemPumpID);
                    Attributes.Add(DataAttribute.SystemSourceKey);
                }
                else if (subClassType == SubClassTypes.Input)
                {
                    Attributes.Add(DataAttribute.SystemTaskID);
                }
                else if (subClassType == SubClassTypes.PumpInput)
                {
                    Attributes.Add(DataAttribute.SystemPumpIDDefault);
                    Attributes.Add(DataAttribute.SystemSourceKey);
                    Attributes.Add(DataAttribute.SystemTaskIDDefault);
                }
                else
                    throw new Exception("������������ �������� �������.");
            }
            finally
            {
                LogicalCallContextData.SetContext(callerContext);
            }
        }

        /// <summary>
        /// ������������� �������� �������, ������� ��� ���������
        /// </summary>
        /// <param name="value">��������������� ��������</param>
        protected override void ChangeSubClassType(SubClassTypes value)
        {
            // Input -> Pump
            if (value == SubClassTypes.Pump && SubClassType == SubClassTypes.Input)
            {
                Attributes.Remove(DataAttribute.TaskIDColumnName);

                Attributes.Add(DataAttribute.SystemPumpID);
                Attributes.Add(DataAttribute.SystemSourceKey);
            }
            // Input -> PumpInput
            else if (value == SubClassTypes.PumpInput && SubClassType == SubClassTypes.Input)
            {
                Attributes.Add(DataAttribute.SystemPumpIDDefault);
                Attributes.Add(DataAttribute.SystemSourceKey);

                Attributes.Remove(DataAttribute.TaskIDColumnName);
                Attributes.Add(DataAttribute.SystemTaskIDDefault);
            }
            // Pump -> Input
            else if (value == SubClassTypes.Input && SubClassType == SubClassTypes.Pump)
            {
                Attributes.Remove(DataAttribute.PumpIDColumnName);
                Attributes.Remove(DataAttribute.SourceKeyColumnName);

                Attributes.Add(DataAttribute.SystemTaskID);
            }
            // Pump -> PumpInput
            else if (value == SubClassTypes.PumpInput && SubClassType == SubClassTypes.Pump)
            {
                Attributes.Add(DataAttribute.SystemTaskIDDefault);
            }
            // PumpInput -> Pump
            else if (value == SubClassTypes.Pump && SubClassType == SubClassTypes.PumpInput)
            {
                Attributes.Remove(DataAttribute.TaskIDColumnName);

            }
            // PumpInput -> Input
            else if (value == SubClassTypes.Input && SubClassType == SubClassTypes.PumpInput)
            {
                Attributes.Remove(DataAttribute.PumpIDColumnName);
                Attributes.Remove(DataAttribute.SourceKeyColumnName);
            }
            else
                throw new ArgumentException("�������� �������� ��������. �������� ����� ��������� �������� Input, Pump ��� PumpInput.");
        }

        internal override void Create(Modifications.ModificationContext �ontext)
        {
            base.Create(�ontext);

            RegisterObject(GetKey(ObjectKey, FullName), FullCaption, SysObjectsTypes.FactTable);
        }

        internal override void Drop(Modifications.ModificationContext context)
        {
            base.Drop(context);
            UnRegisterObject(GetKey(ObjectKey, FullName));
        }

        public override string TablePrefix
        {
            get { return "f"; }
        }

        /// <summary>
        /// ������ ��� �������
        /// </summary>
        public override string FullName
        {
            [DebuggerStepThrough]
            get
            {
                return "f." + Semantic + "." + Name;
            }
        }

        protected override string GetOlapCaption()
        {
            if (this.ShortCaption != String.Empty)
                return ShortCaption;
            else
                return Caption;
        }
                
        /// <summary>
        /// down
        /// </summary>
        public new IAssociationCollection Associations
        {
            [DebuggerStepThrough()]
            get { return (IAssociationCollection)base.Associations; }
        }

        /// <summary>
        /// ������� ������ ������ ������� �� ����������
        /// </summary>
        public override bool IsDivided
        {
            get
            {
                return true;
            }
        }
        
#if ScriptingEngine
        /// <summary>
        /// ��������� ������ ��� �������� ���������
        /// </summary>
        /// <param name="withoutAttribute">�������, ������� �� ����� ����������� ��� �������� ��������� ��������</param>
        /// <returns>DDL-����������� ��������</returns>
        internal override string[] GetTriggersScript(DataAttribute withoutAttribute)
        {
            List<string> script = new List<string>(base.GetTriggersScript(withoutAttribute));

            // �������� �� ����������� � ������� ����������� � ���������������
            foreach (EntityAssociation association in this.Associations.Values)
            {
                //if (association.RoleDataAttribute.Name == withoutAttribute.Name)
                //    continue;

                string childTriggerScript = association.RoleB.GetCustomTriggerScriptForChildEntity(this, withoutAttribute);
                if (!String.IsNullOrEmpty(childTriggerScript))
                    script.Add(childTriggerScript);
            }

            return script.ToArray();
        }
#endif
        /// <summary>
        /// ��������� ����� �� �������� ������� ��� �������� ������������
        /// </summary>
        /// <returns>true - ���� � ������������ ���� ����� �� ��������</returns>
        public override bool CurrentUserCanViewThisObject()
        {
            try
            {
                return SchemeClass.Instance.UsersManager.CheckPermissionForSystemObject(ObjectKey, (int)FactTableOperations.ViewClassifier, false);
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// ���������� ������ ��� ���������� � ���������� ������
        /// </summary>
        public override IDataUpdater GetDataUpdater()
        {
            return GetDataUpdater(String.Empty, null, null);
        }

        /// <summary>
        /// ���������� ������ ��� ���������� � ���������� ������
        /// </summary>
        public override IDataUpdater GetDataUpdater(IDatabase db, string selectFilter, int? maxRecordCountInSelect, params IDbDataParameter[] selectFilterParameters)
        {
            if (SchemeDWH.Instance.FactoryName == ProviderFactoryConstants.OracleClient || 
				SchemeDWH.Instance.FactoryName == ProviderFactoryConstants.OracleDataAccess ||
				SchemeDWH.Instance.FactoryName == ProviderFactoryConstants.MSOracleDataAccess)
            {
                return base.GetDataUpdater(db, selectFilter, maxRecordCountInSelect, selectFilterParameters);
            }
            else
            {
                DataUpdater du;

                //// du = db.GetDataUpdater(this.FullDBName, this.Attributes, selectFilter, maxRecordCountInSelect, selectFilterParameters);
                IDbDataAdapter adapter = ((Database) db).GetDataAdapter();
                du = new DataUpdater(adapter, null, ((SchemeDWH) SchemeClass.Instance.SchemeDWH).GetFactory, true);
                ((Database) db).InitDataAdapter(adapter, FullDBName, Attributes, null, selectFilter,
                                                maxRecordCountInSelect, selectFilterParameters);
                du.Transaction = ((Database) db).Transaction;
                ////

                // ������� ID �� ������� �� �������
                adapter.InsertCommand.CommandText =
                    adapter.InsertCommand.CommandText.Replace("(ID, ", "(").Replace(
                        String.Format("({0}ID, ", _scriptingEngine.ParameterPrefixChar()), "(");
                adapter.InsertCommand.Parameters.RemoveAt(0);
                // ��������� SELECT ����� INSERT ��� ���������� �������� ID ��� ����������� ������� 
                StringBuilder selectQuery = new StringBuilder(";SELECT SCOPE_IDENTITY() as ID, ");
                foreach (IDataAttribute attribute in Attributes.Values)
                {
                    if (attribute.Name != "ID")
                        selectQuery.Append(attribute.Name).Append(',');
                }
                selectQuery.Remove(selectQuery.Length - 1, 1);
                selectQuery.Append(" FROM ")
                    .Append(FullDBName)
                    .Append(" WHERE (ID = SCOPE_IDENTITY())");
                adapter.InsertCommand.CommandText += selectQuery.ToString();

                // ������� ID �� ������� �� ����������
                adapter.UpdateCommand.CommandText =
                    adapter.UpdateCommand.CommandText.Replace(
                        String.Format("ID={0}ID, ", _scriptingEngine.ParameterPrefixChar()), "");
                adapter.UpdateCommand.Parameters.RemoveAt(0);
                du.OnAfterUpdate += new AfterUpdateEventDelegate(OnAfterUpdate);
                return du;
            }
        }

        /// <summary>
        /// ���������� ������ ��� ���������� � ���������� ������
        /// </summary>
        public override IDataUpdater GetDataUpdater(string selectFilter, int? maxRecordCountInSelect, params IDbDataParameter[] selectFilterParameters)
        {
            Database db = null;
            try
            {
                db = (Database)SchemeClass.Instance.SchemeDWH.DB;

                DataUpdater du = (DataUpdater)GetDataUpdater(db, selectFilter, maxRecordCountInSelect, selectFilterParameters);
                return du;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                throw new Exception(e.Message, e);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        protected override void OnAfterUpdate()
        {
            try
            {
                SchemeClass.Instance.Processor.InvalidatePartition(
                    this,
                    "Krista.FM.Server.Scheme.Classes.FactTable",
                    InvalidateReason.ClassifierChanged,
                    OlapName);
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "��� ��������� �������� ������������� ������� ��� ������� \"{0}\" ��������� ������: {1}",
                    this.FullName, e.Message);
            }
        }

        /// <summary>
        /// ���������� ������ ���������� �� ������� ����������� ���
        /// </summary>
        /// <returns>Key - ID ��������� ������; Value - �������� ���������</returns>
        public Dictionary<int, string> GetDataSourcesNames()
        {
            if (Attributes.ContainsKey(DataAttribute.SourceIDColumnName))
                return SchemeClass.Instance.DataSourceManager.GetDataSourcesNames(FullDBName);
            else
                throw new Exception(String.Format("���������� �������� ������ ���������� � ������� � ������� ��� �������� {0}.", DataAttribute.SourceIDColumnName));
        }
        
        /// <summary>
        /// ���������� �������������� �������� ���� �� �������������� ��������� ������ ��� ������ ������� ������.
        /// </summary>
        /// <param name="sourceID">������������� ��������� ������.</param>
        /// <returns>�������������� �������� ����.</returns>
        public List<string> GetPartitionsNameBySourceID(int sourceID)
        {
            List<string> partitionsIdList = new List<string>();

            OlapDBWrapper olapDBWrapper = ((OlapDBWrapper) SchemeClass.Instance.Processor.OlapDBWrapper);

            // �������� ������ �����, ������� ��������� �� ������ ������� ������
            List<string> cubesIdList = olapDBWrapper.GetOlapObjectsIdByFullName(OlapObjectType.Cube, this.FullName);

            // ��� ������� ���� �������� ����� ������� ��������������� ��������� sourceID
            foreach (string cubeId in cubesIdList)
            {
                partitionsIdList.AddRange(GetPartitionsNameBySourceIDForOneCube(cubeId, sourceID, olapDBWrapper));
            }

            return partitionsIdList;
        }

        /// <summary>
        /// ���������� �������������� �������� ���� �� �������������� ��������� ������.
        /// </summary>
        /// <param name="cubeId"></param>
        /// <param name="sourceID"></param>
        /// <param name="olapDBWrapper"></param>
        /// <returns></returns>
        // TODO: ��� ��������� ������ ���� ����� ���� ��������!!!
        private List<string> GetPartitionsNameBySourceIDForOneCube(string cubeId, int sourceID, OlapDBWrapper olapDBWrapper)
        {
            List<string> partitionsIdList = new List<string>();
            
            IProcessableObjectInfo poi = olapDBWrapper.GetProcessableObjectInfo(OlapObjectType.Cube, cubeId);
            string cubeName = poi.ObjectName;

            // 1. �������� ������ �������� ����
            Dictionary<string, IProcessableObjectInfo> partitions = olapDBWrapper.DS_GetCubePartitions(poi.ObjectID);

            // 2. ���� ������ ����, �� ���������� ��� ���
            if (partitions.Count == 1)
            {
                foreach (IProcessableObjectInfo objectInfo in partitions.Values)
                {
                    partitionsIdList.Add(objectInfo.ObjectID);
                    return partitionsIdList;
                }
                throw new ServerException("BUG");
            }
                // 3. ���� �������� ���������, �� �������� ����� ������
            else
            {
                List<string> partitionsNames = new List<string>();
                foreach (IProcessableObjectInfo item in partitions.Values)
                {
                    partitionsNames.Add(item.ObjectName);
                }
                
                // 3.1. ���� ���� �������� ����������� � ������ ����, 
                // �� ���������� �� - ��������� ����������
                if (partitionsNames.Contains(cubeName))
                {
                    partitionsIdList.Add(olapDBWrapper.GetPartitionIdByName(cubeName, cubeName));
                    return partitionsIdList;
                }

                // 3.1. ���������� ��������� ��������� ������
                IDataSource ds = SchemeClass.Instance.DataSourceManager.DataSources[sourceID];
                if (ds == null)
                {
                    return partitionsIdList;
                }

                try
                {
                    // 3.2. �� ���������� ��������� ������ ��������� ����� �������� �� ����������
                    switch (ds.ParametersType)
                    {
                        case ParamKindTypes.Year:
                        case ParamKindTypes.Budget:
                            foreach (string item in partitionsNames)
                            {
                                string[] parts = item.Split(new char[] { '_' });
                                // ������: ���_YYYY
                                if (parts[0].ToUpper() == "���" && 
                                    parts.GetLength(0) == 2 &&
                                    parts[1].Length == 4)
                                {
                                    int year = Convert.ToInt32(parts[1]);
                                    if (ds.Year == year)
                                    {
                                        partitionsIdList.Add(olapDBWrapper.GetPartitionIdByName(cubeName, item));       
                                    }
                                } else
                                    // ������: ���_YYYY_YYYY
                                    if (parts[0].ToUpper() == "���" &&
                                        parts.GetLength(0) == 3 &&
                                        parts[1].Length == 4 &&
                                        parts[2].Length == 4)
                                    {
                                        int startYear = Convert.ToInt32(parts[1]);
                                        int endYear = Convert.ToInt32(parts[2]);
                                        if (ds.Year >= startYear && ds.Year <= endYear)
                                        {
                                            partitionsIdList.Add(olapDBWrapper.GetPartitionIdByName(cubeName, item));
                                        }
                                    } else
                                        // ������: ���_YYYY_MM
                                        if (parts[0].ToUpper() == "���" &&
                                            parts.GetLength(0) == 3 &&
                                            parts[1].Length == 4 &&
                                            parts[2].Length == 2)
                                        {
                                            int year = Convert.ToInt32(parts[1]);
                                            if (ds.Year == year)
                                            {
                                                partitionsIdList.Add(olapDBWrapper.GetPartitionIdByName(cubeName, item));
                                            }
                                        }
                            }
                            break;
                        case ParamKindTypes.YearMonth:
                        case ParamKindTypes.YearMonthVariant:
                        case ParamKindTypes.YearMonthTerritory:
                            foreach (string item in partitionsNames)
                            {
                                string[] parts = item.Split(new char[] { '_' });
                                // ������: ���_YYYY
                                if (parts[0].ToUpper() == "���" &&
                                    parts.GetLength(0) == 2 &&
                                    parts[1].Length == 4)
                                {
                                    int year = Convert.ToInt32(parts[1]);
                                    if (ds.Year == year)
                                    {
                                        string partitionId = olapDBWrapper.GetPartitionIdByName(cubeName, item);
                                        if (!partitionsIdList.Contains(partitionId))
                                            partitionsIdList.Add(partitionId);
                                    }
                                }
                                else
                                    // ������: ���_YYYY_YYYY
                                    if (parts[0].ToUpper() == "���" &&
                                        parts.GetLength(0) == 3 &&
                                        parts[1].Length == 4 &&
                                        parts[2].Length == 4)
                                    {
                                        int startYear = Convert.ToInt32(parts[1]);
                                        int endYear = Convert.ToInt32(parts[2]);
                                        if (ds.Year >= startYear && ds.Year <= endYear)
                                        {
                                            partitionsIdList.Add(olapDBWrapper.GetPartitionIdByName(cubeName, item));
                                        }
                                    }
                                    else
                                        // ������: ���_YYYY_MM
                                        if (parts[0].ToUpper() == "���" &&
                                            parts.GetLength(0) == 3 &&
                                            parts[1].Length == 4 &&
                                            parts[2].Length == 2)
                                        {
                                            int year = Convert.ToInt32(parts[1]);
                                            int month = Convert.ToInt32(parts[2]);
                                            if (ds.Year == year && ds.Month == month)
                                            {
                                                partitionsIdList.Add(olapDBWrapper.GetPartitionIdByName(cubeName, item));
                                            }
                                        }
                            }
                            break;
                    }
                    return partitionsIdList;     
                }
                catch (Exception e)
                {
                    Trace.TraceError(
                        "��� ������ � ���� \"{0}\" ������ �� ��������� (ID={1}) ��������� ������. ����� ���c����� ���� ���.\n����� ������: {2}", 
                        OlapName, sourceID,
                        Krista.Diagnostics.KristaDiagnostics.ExpandException(e));                    
                }

                partitionsIdList.Add(olapDBWrapper.GetPartitionIdByName(cubeName, cubeName));
                
                return partitionsIdList;
            }
        }

        /// <summary>
        /// ����������� ����������� ������
        /// </summary>
        /// <param name="processTypes">��� ������� �������</param>
        public override void Process(ProcessTypes processTypes)
        {
            try
            {
                SchemeClass.Instance.Processor.ProcessCube(this, processTypes, this.GetOlapCaption());
            }
            catch (Exception e)
            {
                string message = String.Format("��� \"{0}\", ������ �������: {1}", this.GetOlapCaption(), e.Message);
                Trace.TraceError(message);
				// ����� ����� �������� ������ ���������� - � ������� ����� �� ���� �� ����������.
				// ����� ������ ������������ (�������� ����� �����) �������.
                // Re: ����� ���� - ����� �� �������������� ������� ����������, � ��������� �����
                // ���������������� ����, � ������� ���������� ��� ���������. 
                // ����� ������� �������� ����� ��������� �� ���� ��������.
                throw new ServerException(message, e.InnerException);
            }
        }
    }
}
