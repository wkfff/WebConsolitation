using System;
using System.Collections.Generic;
using System.ComponentModel;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    [TypeConverter(typeof(SmoEntityPropertyVisibleSwitchTypeConverter<SmoEntityDesign>))]
    public class SmoEntityDesign : SmoCommonDBObjectDesign, IEntity
    {
        public SmoEntityDesign(IEntity serverControl)
            : base(serverControl)
        {
        }

        #region IEntity Members

        [Browsable(false)]
        public IDataAttributeCollection Attributes
        {
            get
            {
                LogicalCallContextData context = LogicalCallContextData.GetContext();
                if (context[String.Format("{0}.Presentation", serverControl.FullDBName)] != null)
                    if (((IEntity)serverControl).Presentations.ContainsKey(Convert.ToString(context[String.Format("{0}.Presentation", serverControl.FullDBName)])))
                        return
                            ((IEntity) serverControl).Presentations[
                                Convert.ToString(context[String.Format("{0}.Presentation", serverControl.FullDBName)])].
                                Attributes;

                return
                    ((IEntity)serverControl).Attributes;
            }
        }


        
        [Category("CommonObject")]
        [DisplayName(@"Класс объекта (ClassType)")]
        [Description("Класс объекта")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(false)]
        public ClassTypes ClassType
        {
            get { return ((IEntity)serverControl).ClassType; }
        }

        internal const string SubClassTypePropertyName = "SubClassType";

        [Category("CommonObject")]
        [DisplayName(@"Подкласс объекта (SubClassType)")]
        [Description("Подкласс объекта")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public SubClassTypes SubClassType
        {
            get { return ((IEntity)serverControl).SubClassType; }
            set { ((IEntity)serverControl).SubClassType = value; CallOnChange(); }
        }

        [Category("CommonObject.Naming")]
        [DisplayName(@"ShortCaption")]
        [Description("Не истользуйте свойство ShortCaption при создании новых объектов. Это свойство было выведено в интерфейс для совсестимости со старыми версиями метаданных.")]
        public string ShortCaption
        {
            get { return ((IEntity)serverControl).ShortCaption; }
            set { ((IEntity)serverControl).ShortCaption = value; CallOnChange(); }
        }

        [Category("CommonObject.Naming")]
        [DisplayName(@"Наименование многомерного объекта (OlapName)")]
        [Description("Возвращает имя соответствующего OLAP-объекта")]
        public string OlapName
        {
            get { return ((IEntity)serverControl).OlapName; }
        }

        [Category("CommonObject.Naming")]
        [DisplayName(@"Полное русское наименование объекта, состоящее из семантического имени и названия объекта разделенных точкой.")]
        [Description("Полное русское наименование объекта, состоящее из семантического имени и названия объекта разделенных точкой.")]
        public string FullCaption
        {
            get { return ((IEntity)serverControl).FullCaption; }
        }

        [Browsable(false)]
        public string MacroSet
        {
            get { return ((IEntity)serverControl).MacroSet; }
            set { ((IEntity)serverControl).MacroSet = value; CallOnChange(); }
        }

        [Browsable(false)]
        public IEntityAssociationCollection Associations
        {
            get
            {
                return
                    ((IEntity)serverControl).Associations;
            }
        }

        [Browsable(false)]
        public IEntityAssociationCollection Associated
        {
            get
            {
                return
                    ((IEntity)serverControl).Associated;
            }
        }

        [Browsable(false)]
        public IUniqueKeyCollection UniqueKeys
        {
            get
            {
                return ((IEntity)serverControl).UniqueKeys;
            }
        }

        [Browsable(false)]
        public bool UniqueKeyAvailable
        {
            get { return ((IEntity)serverControl).UniqueKeyAvailable; }
        }

        [Browsable(false)]
        public string GeneratorName
        {
            get { return ((IEntity)serverControl).GeneratorName; }
        }

        [Browsable(false)]
        public string DeveloperGeneratorName
        {
            get { return ((IEntity)serverControl).DeveloperGeneratorName; }
        }

        [Browsable(false)]
        public int GetGeneratorNextValue
        {
            get { return ((IEntity)serverControl).GetGeneratorNextValue; }
        }

        [Browsable(false)]
        public IDataUpdater GetDataUpdater()
        {
            return ((IEntity)serverControl).GetDataUpdater();
        }

        [Browsable(false)]
        public IDataUpdater GetDataUpdater(string selectFilter, int? maxRecordCountInSelect, params System.Data.IDbDataParameter[] selectFilterParameters)
        {
            return ((IEntity)serverControl).GetDataUpdater(selectFilter, maxRecordCountInSelect, selectFilterParameters);
        }

        [Browsable(false)]
        public int DeleteData(string whereClause, params object[] parameters)
        {
            return ((IEntity)serverControl).DeleteData(whereClause, parameters);
        }

        public int DeleteData(string whereClause, bool disableTriggerAudit, params object[] parameters)
        {
            return ((IEntity)serverControl).DeleteData(whereClause, disableTriggerAudit, parameters);
        }

        [Browsable(false)]
        public int RecordsCount(int sourceID)
        {
            return ((IEntity)serverControl).RecordsCount(sourceID);
        }

        public System.Data.DataSet GetDependedData(int rowID, bool recursive)
        {
            return ((IEntity)serverControl).GetDependedData(rowID, recursive);
        }

        public bool ProcessObjectData()
        {
            return ((IEntity)serverControl).ProcessObjectData();
        }

        public void MergingDuplicates(int mainRecordID, List<int> duplicateRecordID, MergeDuplicatesListener listener)
        {
            ((IEntity)serverControl).MergingDuplicates(mainRecordID, duplicateRecordID, listener);
        }

        public string GetObjectType()
        {
            return ((IEntity)serverControl).GetObjectType();
        }

        /// <summary>
        /// Коллекция представлений
        /// </summary>
        [Browsable(false)]
        public IPresentationCollection Presentations
        {
            get
            {
                return ((IEntity)serverControl).Presentations;

            }
        }

        [Browsable(false)]
        IDataAttributeCollection IEntity.GroupedAttributes
        {
            get { return ((IEntity)serverControl).GroupedAttributes; }
        }

        #endregion
    }

    [ReadOnly(true)]
    public class SmoEntityReadOnlyDesign : SmoEntityDesign
    {
        public SmoEntityReadOnlyDesign(IEntity serverControl)
            : base(serverControl)
        {
        }
    }

    internal class SmoEntityPropertyVisibleSwitchTypeConverter<TComponent> : CustomPropertiesTypeConverter<TComponent> where TComponent : SmoEntityDesign
    {
        protected override Attribute[] GetPropertyAttributes(TComponent component, PropertyDescriptor property)
        {
            if (property.Name == SmoEntityDesign.SubClassTypePropertyName)
            {
                switch (component.ClassType)
                {
                    case ClassTypes.clsPackage:
                    case ClassTypes.clsFixedClassifier:
                    case ClassTypes.clsBridgeClassifier:
                    case ClassTypes.clsAssociation:
                        return new Attribute[] { BrowsableAttribute.No };
                    case ClassTypes.clsDataClassifier:
                    case ClassTypes.clsFactData:
                        return new Attribute[] { BrowsableAttribute.Yes, };
                }
            }
            return EmptyAttributes;
        }
    }
}
