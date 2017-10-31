// ******************************************************************
// ������ �������� ���������� ������� � ��������� ������������ 
// ��� ������������� �������� �������� �������
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Data;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    #region ��������� � ������

    #region LayersCollection
    /// <summary>
    /// ��������� ������������ �����
    /// </summary>
    public sealed class LayersCollection : ObjectsCollection
    {
        public LayersCollection(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.layerNodeTag, typeof(Layer))
        {
        }

        public void LoadLayersFromDir(string dir)
        {
            foreach (Layer lay in this)
            {
                lay.LoadFromDir(dir);
            }
        }
    }
    #endregion

    #region FMObjectsCollection
    /// <summary>
    /// ��������� ������������ �������� ������� (�����)
    /// </summary>
    public sealed class FMObjectsCollection : ObjectsCollection
    {
        public FMObjectsCollection(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.fmObjectTag, typeof(FMObject))
        {
        }

        /// <summary>
        /// �������������� �������� ������������ ����������� ������ ������������ ��������
        /// </summary>
        /// <returns>����� ������ (���� ��� ����)</returns>
        public override string Validate()
        {
            string errors = base.Validate();
            if (!String.IsNullOrEmpty(errors))
                return errors;
            DataPumpModuleBase pumpModule = parentProgram.pumpModule;
            // �������������� ������� �������� ������������� ������
            List<string> classifiers = new List<string>();
            List<string> factTables = new List<string>();
            // �������� �� ������ �������� � ��������� ������ �� ������� � �����
            foreach (FMObject obj in this)
            {
                switch (obj.objectType)
                {
                    case FmObjectsTypes.cls:
                        //case FmObjectsTypes.fixedCls:
                        if (pumpModule.Scheme.Classifiers[obj.name].IsDivided)
                            classifiers.Add(obj.name);
                        break;
                    case FmObjectsTypes.factTable:
                        if (pumpModule.Scheme.FactTables[obj.name].IsDivided)
                            factTables.Add(obj.name);
                        break;
                }
            }
            // ��������� ������� ������������ �������� ��� ������������� ������
            // ��� ��� ������������ ��� �������, ���������� � �������� ������
            if (classifiers.Count > 0)
            {
                pumpModule.UsedClassifiers = new IClassifier[classifiers.Count];
                for (int i = 0; i < classifiers.Count; i++)
                {
                    string clsName = classifiers[i];
                    pumpModule.UsedClassifiers[i] = pumpModule.Scheme.Classifiers[clsName];
                }
            }
            if (factTables.Count > 0)
            {
                pumpModule.UsedFacts = new IFactTable[factTables.Count];
                for (int i = 0; i < factTables.Count; i++)
                {
                    string clsName = factTables[i];
                    pumpModule.UsedFacts[i] = pumpModule.Scheme.FactTables[clsName];
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// ����� ������������� �� ������� ��������� �������. ������, ����� ����� ������ - � ����� �������,
        /// � ��� ����� - ������� � ���������� ��������������� (�� � ������ �� ���� ����������) �����
        /// ��� �������������������� ������
        /// </summary>
        /// <param name="dataObj"></param>
        /// <param name="refName"></param>
        /// <returns></returns>
        private static IEntity GetBridgeClsByRefName(IEntity dataObj, string refName)
        {
            IAssociationCollection assCol = (IAssociationCollection)(dataObj).Associations;
            string fullName = String.Concat(dataObj.Name, ".", refName);
            IEntity bridgeCls = null;
            foreach (IAssociation item in assCol.Values)
            {
                if (String.Compare(item.Name, fullName, true) == 0)
                {
                    bridgeCls = item.RoleBridge;
                    break;
                }
            }
            return bridgeCls;
        }

        /// <summary>
        /// �������� ������ ��������� ���������� - �������������� �������� ������������� 
        /// ������ � �������� ��� � �������� ������ ����������� ������ (���������� PumpID � �.�.)
        /// </summary>
        private void BuildProcessRules()
        {
            // ��������� ������� ��� ������� ������������� ������� �������
            foreach (FMObject fmObject in this)
            {
                IEntity ent = fmObject.obj as IEntity;
                #region �� �������� � XML �������� ��������� �������� �� ����������� ��������
                // ������������ ������� ���������, �������� � XML
                fmObject.processRules.Build();
                // ��������� ��� AttributeProcessRules.refCls - ������������ ������������ ID, ��������
                // �������� ������ � ������
                foreach (ProcessRule pr in fmObject.processRules)
                {
                    if (pr.processRule != AttributeProcessRules.refCls)
                        continue;
                    string attrName = pr.name;
                    IDataAttribute attr = ent.Attributes[attrName];

                    if (attr.Class != DataAttributeClassTypes.Reference)
                        throw new Exception("������� ��������� ������ ��� ��������� ����������");
                    // �������� �������� �� ������� ��������� ��������
                    IEntity bridgeCls = GetBridgeClsByRefName(ent, attrName);
                    // ��������� ����� �������-�����������
                    string queryText = String.Format("select ID from {0} where {1}", bridgeCls.FullDBName, pr.dataValue);
                    // �������� ID �������, ��������������� ��������
                    DataTable ids = (DataTable)parentProgram.pumpModule.DB.ExecQuery(queryText, 
                        QueryResultTypes.DataTable);
                    if ((ids != null) && (ids.Rows.Count > 0))
                    {
                        // ����� ID ������ ������ � �������� ������� ���������
                        pr.dataValue = ids.Rows[0][0];
                    }
                    else
                    {
                        // ���� �� ����� ������ �� ������� - ����� ���������� ������� � �������
                        throw new Exception(
                            String.Format("�� ������� �������� ������. �� ������� ������, ��������������� ������� '{0}'", queryText)
                        );
                    }
                }
                #endregion

                #region ������ �� ���������� ������� ����� ��������� �������������� �������
                #region ��������� ID ��� ���������������
                if (fmObject.objectType == FmObjectsTypes.cls)
                {
                    // ��� ������� ��������������� ��������� ������� ��������� ��
                    ProcessRule pr = new ProcessRule(parentProgram);
                    pr.name = "ID";
                    pr.processRule = AttributeProcessRules.generatorValue;
                    pr.dataValue = ent.GeneratorName;
                    fmObject.processRules.AddNew(pr);
                }
                #endregion
                #region ������������ ������ �� �������������� ��� ������ ������
                // ��� ������ ������ - ����������� ������ �� ��������� ��������������
                if (fmObject.objectType == FmObjectsTypes.factTable)
                {
                    IDataAttributeCollection attrs = ent.Attributes;
                    foreach (IDataAttribute attr in attrs.Values)
                    {
                        // ���� �������� - ������..
                        if (attr.Class == DataAttributeClassTypes.Reference)
                        {
                            string attrName = attr.Name;
                            IEntity bridgeCls = GetBridgeClsByRefName(ent, attrName);
                            string bridgeName = bridgeCls.ObjectKey;
                            //  � �������� �������� ������� ��������� ����� �������������� ID 
                            // ��������� ������ ������ �������
                            FMObject fo = ObjectByName(bridgeName) as FMObject;
                            if (fo != null)
                            {
                                ProcessRule pr = new ProcessRule(parentProgram);
                                pr.processRule = AttributeProcessRules.fmObjectRef;
                                pr.name = attrName;
                                pr.dataValue = fo;
                                fmObject.processRules.AddNew(pr);
                            }
                        }
                    }
                }
                #endregion
                #region ������������ PumpID, SourceID, TaskID ��� �����
                DataColumnCollection cl = fmObject.objData.Tables[0].Columns;
                if (cl.Contains("PumpID"))
                {
                    ProcessRule pr = new ProcessRule(parentProgram);
                    pr.name = "PumpID";
                    pr.processRule = AttributeProcessRules.constant;
                    pr.dataValue = parentProgram.pumpModule.PumpID;
                    fmObject.processRules.AddNew(pr);
                }
                if (cl.Contains("SourceID"))
                {
                    ProcessRule pr = new ProcessRule(parentProgram);
                    pr.name = "SourceID";
                    pr.processRule = AttributeProcessRules.constant;
                    // ���� �������� ������� ���� �� ������ - ����� �������� �������
                    if (fmObject.dataSource != null)
                        pr.dataValue = fmObject.dataSource.id;
                    else
                        pr.dataValue = parentProgram.pumpModule.SourceID;
                    fmObject.processRules.AddNew(pr);
                }
                if (cl.Contains("TaskID"))
                {
                    ProcessRule pr = new ProcessRule(parentProgram);
                    pr.name = "TaskID";
                    pr.processRule = AttributeProcessRules.constant;
                    pr.dataValue = -1;
                    fmObject.processRules.AddNew(pr);
                }
                #endregion
                #endregion

            }

        }

        /// <summary>
        /// ������ ������ �������
        /// </summary>
        internal void QueryData()
        {
            foreach (FMObject obj in this)
            {
                obj.QueryData();
            }
            // ����� ������������� ��������� � ��������� ����� ������������ ������� ���������
            BuildProcessRules();
        }
    }
    #endregion

    #region ProcessRulesList
    /// <summary>
    /// ������ ������ ���������
    /// </summary>
    public sealed class ProcessRulesList : ObjectsList
    {
        public ProcessRulesList(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.processRuleTag, typeof(ProcessRule))
        {
        }

        /// <summary>
        /// ������������� ������� � ��������� ����
        /// </summary>
        internal void Build()
        {
            foreach (ProcessRule pr in this)
            {
                pr.Build();
            }
        }

        /// <summary>
        /// ��������� ������� ��������� � ������ ������
        /// </summary>
        /// <param name="row">������ ������</param>
        internal void Applay(DataRow row)
        {
            foreach (ProcessRule pr in this)
            {
                pr.Applay(row);
            }
        }
    }
    #endregion

    #region StepsCollection
    /// <summary>
    /// ��������� ����� �������
    /// </summary>
    public sealed class StepsCollection : ObjectsCollection
    {
        public StepsCollection(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.stepNodeTag, typeof(PumpStep))
        {
        }

        internal void BuildProcessRules()
        {
            foreach (PumpStep ps in this)
                ps.fieldsGroups.BuildProcessRules();
        }
    }
    #endregion

    #region LayerFieldsList
    /// <summary>
    /// ������ ����� ����
    /// </summary>
    public sealed class LayerFieldsList : ObjectsList
    {
        public LayerFieldsList(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.layerFieldNodeTag, typeof(LayerField))
        {
        }

        internal void BuildProcessRules()
        {
            foreach (LayerField lf in this)
            {
                if (lf.processRules.Count != 0)
                    lf.processRules.Build();
            }
        }

    }
    #endregion

    #region FieldsGroupsList
    /// <summary>
    /// ������ ����� �����
    /// </summary>
    public sealed class FieldsGroupsList : ObjectsList
    {
        public FieldsGroupsList(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.fieldsGroupTag, typeof(FieldsGroup))
        {
        }

        internal void BuildProcessRules()
        {
            foreach (FieldsGroup fp in this)
                fp.layerFields.BuildProcessRules();
        }

    }
    #endregion
    #endregion

}