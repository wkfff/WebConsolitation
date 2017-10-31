using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoAssociationDesign : SmoCommonDBObjectDesign, IAssociation
    {
        public SmoAssociationDesign(IEntityAssociation serverControl)
            : base(serverControl)
        {
        }

        public static SmoAssociationDesign CreateInstance(IEntityAssociation serverObject)
        {
            if (serverObject is IBridgeClassifier)
                return new SmoBridgeAssociationDesign(serverObject);
            else
                return new SmoAssociationDesign(serverObject);
        }

        [Browsable(false)]
        public new IEntityAssociation ServerControl
        {
            get { return (IEntityAssociation)serverControl; }
        }

        /*        [Category("CommonObject.Naming")]
                [DisplayName("Английское наименование (Name)")]
                [Description("Имя объекта")]
                [RefreshProperties(RefreshProperties.All)]*/
        [ReadOnly(true)] // Запрещием редактирование наименования
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        /*        [Category("CommonObject.Naming")]
                [DisplayName("Семантика (Semantic)")]
                [Description("Семантичаская принадлежность объекта")]
                [Browsable(true)]
                [TypeConverter(typeof(RelationConverter))]
                [RefreshProperties(RefreshProperties.All)]*/
        [ReadOnly(true)] // Запрещием редактирование наименования
        public override string Semantic
        {
            get { return base.Semantic; }
            set { base.Semantic = value; }
        }

        #region IAssociation Members

        [Browsable(false)]
        public IEntity RoleData
        {
            get
            {
                return ServerControl.RoleData;
            }
        }

        [Browsable(false)]
        public IEntity RoleBridge
        {
            get
            {
               return ServerControl.RoleBridge;
            }
        }

        public int Associate()
        {
            return ((IBridgeAssociation)serverControl).Associate();
        }

        public int Associate(int dataClsSourceID, int bridgeClsSourceID)
        {
            return ((IBridgeAssociation)serverControl).Associate(dataClsSourceID, bridgeClsSourceID);
        }

        public int Associate(int dataClsSourceID, int bridgeClsSourceID, int pumpID)
        {
            return ((IBridgeAssociation)serverControl).Associate(dataClsSourceID, bridgeClsSourceID, pumpID);
        }

        public int Associate(int dataClsSourceID, int bridgeClsSourceID, int pumpID, bool allowDigits, bool reAssociate)
        {
            return ((IBridgeAssociation)ServerControl).Associate(dataClsSourceID, bridgeClsSourceID, pumpID, allowDigits, reAssociate);
        }

        public int Associate(int dataClsSourceID, int bridgeClsSourceID, IAssociateRule associateRule)
        {
            return ((IBridgeAssociation)serverControl).Associate(dataClsSourceID, bridgeClsSourceID, associateRule);
        }

        public int Associate(int dataClsSourceID, int bridgeClsSourceID, string associateRuleObjectKey, StringElephanterSettings stringSettings, AssociationRuleParams ruleParams)
        {
            return ((IBridgeAssociation)serverControl).Associate(dataClsSourceID, bridgeClsSourceID, associateRuleObjectKey, stringSettings,
                                                                  ruleParams);
        }

        public int FormBridgeClassifier(int dataSourceID, int bridgeSourceID)
        {
            return ((IBridgeAssociation)serverControl).FormBridgeClassifier(dataSourceID, bridgeSourceID);
        }

        public int CopyAndAssociateRow(int rowID, int bridgeSourceID)
        {
            return ((IBridgeAssociation)serverControl).CopyAndAssociateRow(rowID, bridgeSourceID);
        }

        public void ClearAssociationReference(int sourceID)
        {
            ((IBridgeAssociation)serverControl).ClearAssociationReference(sourceID);
        }

        public int GetAllRecordsCount()
        {
            return ((IBridgeAssociation)serverControl).GetAllRecordsCount();
        }

        public int GetAllUnassociateRecordsCount()
        {
            return ((IBridgeAssociation)serverControl).GetAllUnassociateRecordsCount();
        }

        public int GetRecordsCountByCurrentDataSource(int sourceID)
        {
            return ((IBridgeAssociation)serverControl).GetRecordsCountByCurrentDataSource(sourceID);
        }

        public int GetUnassociateRecordsCountByCurrentDataSource(int sourceID)
        {
            return ((IBridgeAssociation)serverControl).GetUnassociateRecordsCountByCurrentDataSource(sourceID);
        }

        public void RefreshRecordsCount()
        {
            ((IBridgeAssociation)serverControl).RefreshRecordsCount();
        }

        #endregion

        [DisplayName(@"Класс (AssociationClassType)")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public virtual AssociationClassTypes AssociationClassType
        {
            get
            {
                return
                    ServerControl.AssociationClassType;
            }
        }

        [DisplayName(@"Составное имя (FullCaption)")]
        public virtual string FullCaption
        {
            get { return ServerControl.FullCaption; }
        }

        [Browsable(false)]
        public IDataAttribute RoleDataAttribute
        {
            get
            {
                return
                    ServerControl.RoleDataAttribute;
            }
        }
    }

    [ReadOnly(true)]
    public class SmoAssociationReadOnlyDesign : SmoAssociationDesign
    {
        public SmoAssociationReadOnlyDesign(IEntityAssociation serverObject)
            : base(serverObject)
        {
        }

        public static SmoAssociationDesign CreateInstance(IEntityAssociation serverObject)
        {
            if (serverObject is IBridgeClassifier)
                return new SmoAssociationReadOnlyDesign(serverObject);
            else
                return new SmoAssociationReadOnlyDesign(serverObject);
        }
    }
}
