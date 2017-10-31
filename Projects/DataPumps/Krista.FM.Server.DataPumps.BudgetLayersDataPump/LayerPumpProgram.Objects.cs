// ******************************************************************
// ������ �������� ���������� �������� �������� ��������� �������
// ******************************************************************

using System;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    #region �������

    #region Layer
    /// <summary>
    /// ����� ��� ������������� ������ ����
    /// </summary>
    public sealed class Layer : ProcessObjectWithSynonym
    {
        // ������ ����
        internal DataTable layerData;

        public Layer(PumpProgram parentProgram)
            : base(parentProgram, String.Empty)
        {
        }

        /// <summary>
        /// ��������� ������ ���� �� ��������
        /// </summary>
        /// <param name="dir">������ ���� � ��������</param>
        internal void LoadFromDir(string dir)
        {
            // ���� ��� ���� �� ��� ��� �� ������ - ������
            if (String.IsNullOrEmpty(this.name))
                throw new Exception(String.Format("�� ������ ��� ����"));
            // ��������� ������ ���� � ������ ����
            string layerFileName = dir + "\\" + this.name + ".xml";
            // �������� ��������� ������ ����
            ADODBRecordsetReader.LoadRecordset(ref layerData, layerFileName);
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        internal override void Clear()
        {
            base.Clear();
            layerData.Clear();
            layerData = null;
        }
    }
    #endregion

    #region FMObject
    /// <summary>
    /// ����� ��� ������������ ������ ������ �������
    /// </summary>
    public sealed class FMObject : ProcessObjectWithSynonym
    {
        // ��� ������� (������������� ��� ������� ������)
        internal FmObjectsTypes objectType;
        // ���������� ������ �� ������ �����
        internal object obj;
        // ������� ��� ����������� � ������� �������
        internal IDbDataAdapter adapter;
        // ���������� ������ �������
        internal DataSet objData;
        // ID ��������� ���������� ������ �� �������, ������������ ��� ������������ ������ �� ������
        // � ��������� �������� ������
        internal object pumpedValueForCurrentLayerRow;
        // ������ ������ ��������� ���������� �������. ������� ����������� � ������ ����� ������
        internal ProcessRulesList processRules;
        // �������� ������ �� ������� ������������ ����� �������
        internal DataSource dataSource = null;

        
        public FMObject(PumpProgram parentProgram) : base(parentProgram, String.Empty)
        {
            processRules = new ProcessRulesList(this.parentProgram);
        }

        /// <summary>
        /// ������������ ������������ ��������
        /// </summary>
        internal override void Clear()
        {
            base.Clear();
            if (obj != null)
            {
                obj = null;
            }
            if (adapter != null)
            {
                adapter = null;
            }
            if (objData != null)
            {
                objData.Clear();
                objData = null;
            }
            processRules.Clear();
        }

        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            this.objectType = XmlConstsToEnumConverter.StringToFmObjectsTypes(
                XmlHelper.GetStringAttrValue(node, XmlConsts.fmObjectTypeAttr, String.Empty));

            XmlNode xn = node.SelectSingleNode(XmlConsts.dataSourceNodeTag);
            if (xn != null)
            {
                dataSource = new DataSource(this.parentProgram);
                dataSource.LoadFromXml(xn);
            }
            XmlNode rulesNode = node.SelectSingleNode(XmlConsts.processRulesTag);
            if (rulesNode != null)
                processRules.LoadFromXml(rulesNode);
        }

        private const string OBJECT_NOT_FIND_TEMPLATE = "{0} '{1}' �� ������ � ��������� �������� �����'";

        //internal Dictionary<string, FMObject> referencesMapping = new Dictionary<string, FMObject>();

        /// <summary>
        /// �������� ������������ ������� (������ � ����� ������� ������ �������������� � �����)
        /// </summary>
        /// <returns>����� ������ (���� ��� ����)</returns>
        internal override string Validate()
        {
            obj = null;
            try
            {
                DataPumpModuleBase pumpModule = parentProgram.pumpModule;
                IScheme scheme = pumpModule.Scheme;
                switch (objectType)
                {
                    case FmObjectsTypes.cls:
                    case FmObjectsTypes.fixedCls:
                        if (!scheme.Classifiers.ContainsKey(this.name))
                            return String.Format(OBJECT_NOT_FIND_TEMPLATE, "�������������", this.name);
                        else
                            obj = scheme.Classifiers[this.name];
                        break;
                    case FmObjectsTypes.factTable:
                        if (!scheme.FactTables.ContainsKey(this.name))
                            return String.Format(OBJECT_NOT_FIND_TEMPLATE, "������� ������", this.name);
                        else
                            obj = scheme.FactTables[this.name];
                        break;
                }
                return base.Validate();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        /// ������ ���������� ������ �������
        /// </summary>
        internal void QueryData()
        {
            int sourceIdInt = parentProgram.pumpModule.SourceID;
            if (dataSource != null)
                sourceIdInt = dataSource.id;
            switch (objectType)
            {
                case FmObjectsTypes.fixedCls:
                    // ???
                    //pumpModule.InitClsDataSet(ref adapter, ref objData, (IClassifier)obj);
                    break;
                case FmObjectsTypes.cls:
                    parentProgram.pumpModule.InitClsDataSet(ref adapter, ref objData, (IClassifier)obj, false, string.Empty, sourceIdInt);
                    break;
                case FmObjectsTypes.factTable:
                    parentProgram.pumpModule.InitFactDataSet(ref adapter, ref objData, (IFactTable)obj);
                    break;
            }
        }
    }
    #endregion

    #region DataSource
    /// <summary>
    /// ����� ��� ������������� ��������� ������. � ������ ������ ����������� �� ������������. 
    /// ����� ������������
    /// </summary>
    public sealed class DataSource : ProcessObject
    {
        internal string supplierCode;
        internal string dataCode;
        internal string kindsOfParams;
        internal string year;
        internal string month;
        internal string quarter;
        internal string variant;
        internal int id;

        public DataSource(PumpProgram parentProgram) : base(parentProgram, String.Empty)
        {
        }

        internal override void LoadFromXml(XmlNode node)
        {
            supplierCode = XmlHelper.GetStringAttrValue(node, XmlConsts.supplierCodeAttr, String.Empty);
            dataCode = XmlHelper.GetStringAttrValue(node, XmlConsts.dataCodeAttr, String.Empty);
            kindsOfParams = XmlHelper.GetStringAttrValue(node, XmlConsts.kindsOfParamsAttr, String.Empty);
            year = XmlHelper.GetStringAttrValue(node, XmlConsts.yearAttr, "0");
            month = XmlHelper.GetStringAttrValue(node, XmlConsts.monthAttr, "0");
            quarter = XmlHelper.GetStringAttrValue(node, XmlConsts.quarterAttr, "0");
            variant = XmlHelper.GetStringAttrValue(node, XmlConsts.variantAttr, String.Empty);

            id = parentProgram.pumpModule.AddDataSource(supplierCode, dataCode, (ParamKindTypes)Convert.ToInt32(kindsOfParams),
                string.Empty, Convert.ToInt32(year), Convert.ToInt32(month), variant, Convert.ToInt32(quarter), string.Empty).ID;
        }

        internal override string Validate()
        {
            return base.Validate();
        }
    }
    #endregion

    #region ProcessRule
    /// <summary>
    /// ����� ��� ������������� ������� ���������
    /// </summary>
    public sealed class ProcessRule : ProcessObject
    {
        internal AttributeProcessRules processRule;
        internal object dataValue;

        public ProcessRule(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.attributeNameAttr)
        {
        }

        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            processRule = XmlConstsToEnumConverter.StringToAttributeProcessRules(
                XmlHelper.GetStringAttrValue(node, XmlConsts.attributeProcessRuleTag, String.Empty)
            );
            dataValue = XmlHelper.GetStringAttrValue(node, XmlConsts.dataValueAttr, String.Empty);
        }

        /// <summary>
        /// �������������� ������� ��������� (��������� � DataValue ����������� ��������)
        /// </summary>
        internal void Build()
        {
            DataPumpModuleBase pumpModule = parentProgram.pumpModule;
            switch (processRule)
            {
                case AttributeProcessRules.constant:
                    //  � DataValue � ��� ���������� ������ ��������
                    break;
                case AttributeProcessRules.globalConstant:
                    // �������� �������� ���������� ���������
                    IGlobalConst cnst = pumpModule.Scheme.GlobalConstsManager.Consts.ConstByName(dataValue.ToString());
                    dataValue = cnst.Value;
                    //cnst.Dispose(); // ���� ��?
                    break;
                case AttributeProcessRules.param:
                    // �������� �������� ��������� �������
                    string prmValue = pumpModule.GetParamValueByName(pumpModule.ProgramConfig, dataValue.ToString(), String.Empty);
                    // �������� �������� �������� ���� ��������� � ���� �������� ������� �����
                    //DataColumn clmn = fmObject.objData.Tables[0].Columns[pr.name];
                    //if (clmn == null)
                    //    throw new Exception(String.Format("�������� '{0}' �� ������", this.name));
                    //dataValue = Convert.ChangeType(prmValue, clmn.DataType);
                    dataValue = prmValue;
                    break;
                case AttributeProcessRules.refCls:
                    // ��, �� ������� ������ �� ������������� ������
                    // �������� ����� ������� ������������ �� ������� ������� ��� ����� ������ ����
                    break;
            }
        }

        /// <summary>
        /// ��������� ������� ��������� � ������ ������
        /// </summary>
        /// <param name="row">������ ������</param>
        internal void Applay(DataRow row)
        {
            switch (processRule)
            {
                case AttributeProcessRules.constant:
                case AttributeProcessRules.globalConstant:
                case AttributeProcessRules.param:
                    row[this.name] = dataValue;
                    break;
                case AttributeProcessRules.generatorValue:
                    row[this.name] = parentProgram.pumpModule.GetGeneratorNextValue((string)dataValue);
                    break;
                case AttributeProcessRules.fmObjectRef:
                    row[this.name] = ((FMObject)dataValue).pumpedValueForCurrentLayerRow;
                    break;
            }
        }
    }
    #endregion

    #region Step
    /// <summary>
    /// ����� ��� ������������� ������ ���� �������
    /// </summary>
    public sealed class PumpStep : ProcessObjectWithSynonym
    {
        internal FieldsGroupsList fieldsGroups;

        public PumpStep(PumpProgram parentProgram)
            : base(parentProgram, String.Empty)
        {
            fieldsGroups = new FieldsGroupsList(this.parentProgram);
        }

        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            this.synonym = XmlHelper.GetStringAttrValue(node, XmlConsts.layerSynonymAttr, String.Empty);

            fieldsGroups.LoadFromXml(node);
        }

        internal override void Clear()
        {
            base.Clear();
            fieldsGroups.Clear();
        }
    }
    #endregion

    #region LayerField
    /// <summary>
    /// ����� ���� ������������� ������ ���� ����
    /// </summary>
    public sealed class LayerField : ProcessObject
    {
        internal ProcessTypes processType;
        internal string destinationObjectSynonym;
        internal string destinationAttributeName;
        internal bool destinationAttributeUseInSearch;
        internal ConvertValueModes convertValueMode = ConvertValueModes.none;
        internal string convertValueModeParam1;
        internal string convertValueModeParam2;
        internal string refLayerSynonym;
        internal string refLayerFieldName;

        internal ProcessRulesList processRules;

        public LayerField(PumpProgram parentProgram)
            : base(parentProgram, String.Empty)
        {
            processRules = new ProcessRulesList(this.parentProgram);
        }

        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            this.processType = XmlConstsToEnumConverter.StringToProcessTypes(
                XmlHelper.GetStringAttrValue(node, XmlConsts.processTypeAttr, String.Empty));
            this.destinationObjectSynonym = XmlHelper.GetStringAttrValue(node, XmlConsts.destinationObjectSynonymAttr, String.Empty);
            this.destinationAttributeName = XmlHelper.GetStringAttrValue(node, XmlConsts.destinationAttributeAttr, String.Empty);
            this.destinationAttributeUseInSearch = XmlHelper.GetBoolAttrValue(node, XmlConsts.destinationAttributeUseInSearchAttr, false);
            string convValModeStr = XmlHelper.GetStringAttrValue(node, XmlConsts.convertValueModeAttr, String.Empty);
            if (!String.IsNullOrEmpty(convValModeStr))
            {
                this.convertValueMode = XmlConstsToEnumConverter.StringToConvertValueVodes(convValModeStr);
            }
            refLayerSynonym = XmlHelper.GetStringAttrValue(node, XmlConsts.refLayerSynonymAttr, String.Empty);
            refLayerFieldName = XmlHelper.GetStringAttrValue(node, XmlConsts.refLayerFieldNameAttr, String.Empty);
            convertValueModeParam1 = XmlHelper.GetStringAttrValue(node, XmlConsts.convertValueModeParam1Attr, String.Empty);
            convertValueModeParam2 = XmlHelper.GetStringAttrValue(node, XmlConsts.convertValueModeParam2Attr, String.Empty);

            XmlNode rulesNode = node.SelectSingleNode(XmlConsts.processRulesTag);
            if (rulesNode != null)
                processRules.LoadFromXml(rulesNode);
        }

        internal override void Clear()
        {
            base.Clear();
            processRules.Clear();
        }
    }
    #endregion

    #region FieldsGroup
    /// <summary>
    /// ����� ��� ������������� ������ ����� ����
    /// </summary>
    public sealed class FieldsGroup : ProcessObject
    {
        internal FieldsGroupsType groupType;

        internal LayerFieldsList layerFields;

        public FieldsGroup(PumpProgram parentProgram)
            : base(parentProgram, String.Empty)
        {
            layerFields = new LayerFieldsList(this.parentProgram);
        }

        internal override void LoadFromXml(XmlNode node)
        {
            groupType = XmlConstsToEnumConverter.StringToFieldsGroupsType(
                XmlHelper.GetStringAttrValue(node, XmlConsts.fieldsGroupTypeAttr, String.Empty)
            );

            layerFields.LoadFromXml(node);
        }

        internal override void Clear()
        {
            base.Clear();
            layerFields.Clear();
        }

        internal override string Validate()
        {
            StringBuilder sb = new StringBuilder();
            string firstDestinationObject = ((LayerField)layerFields[0]).destinationObjectSynonym;
            foreach (LayerField field in layerFields)
            {
                // ��������� ������������ ������� �������� ������� �����
                string syn = field.destinationObjectSynonym;
                try
                {
                    FMObject fo = parentProgram.fmObjects[syn] as FMObject;
                }
                catch
                {
                    sb.AppendLine(String.Format("�� ������ ������ ������� � ��������� '{0}'", syn));
                }
                // ��������� �������������� ��� ����� ������ ������ ������� �����
                if (field.destinationObjectSynonym != firstDestinationObject)
                    sb.AppendLine("� ������ ����� ������ ��� ���� ������ ������������ ������ ������� �������");
            }
            return sb.ToString();
        }
    }
    #endregion
    #endregion

}