using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Krista.FM.Client.Design.Editors;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    [TypeConverter(typeof(SmoDataSourceDividedClassPropertyVisibleSwitchTypeConverter))]
    public class SmoDataSourceDividedClassDesign : SmoEntityDesign, IDataSourceDividedClass
    {
        public SmoDataSourceDividedClassDesign(IEntity serverControl)
            : base(serverControl)
        {
        }

        #region IDataSourceDividedClass Members


        public ParamKindTypes DataSourceParameter(int sourceID)
        {
            return ((IDataSourceDividedClass)serverControl).DataSourceParameter(sourceID);
        }

        internal const string IsDividedPropertyName = "IsDivided";
        [DisplayName(@"Деление по версиям (IsDivided)")]
        [Description("Определяет делится ли объект по версиям или нет")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool IsDivided
        {
            get
            {
                return
                    ((IDataSourceDividedClass)serverControl).IsDivided;
            }
        }

        internal const string DataSourceKindsPropertyName = "DataSourceKinds";
        [DisplayName(@"Коллекция видов источника (DataSourceKinds)")]
        [Description("Коллекция видов источника, по которым делятся данные")]
        [Editor(typeof(DataSourcesEditor), typeof(UITypeEditor))]
        public virtual string DataSourceKinds
        {
            get
            {
                return
                    ((IDataSourceDividedClass)serverControl).DataSourceKinds;
            }
            set
            {
                ((IDataSourceDividedClass)serverControl).DataSourceKinds = value;
                CallOnChange();
            }
        }

        #endregion
    }

    internal class SmoDataSourceDividedClassPropertyVisibleSwitchTypeConverter : SmoEntityPropertyVisibleSwitchTypeConverter<SmoDataSourceDividedClassDesign>
    {
        protected override Attribute[] GetPropertyAttributes(SmoDataSourceDividedClassDesign component, PropertyDescriptor property)
        {
            List<Attribute> attributes = new List<Attribute>(base.GetPropertyAttributes(component, property));

            if (property.Name == SmoClassifierDesign.IsDividedPropertyName ||
                property.Name == SmoClassifierDesign.DataSourceKindsPropertyName)
            {
                switch (component.ClassType)
                {
                    case ClassTypes.clsPackage:
                    case ClassTypes.clsFixedClassifier:
                    case ClassTypes.clsAssociation:
                        attributes.Add(BrowsableAttribute.No);
                        break;
                    case ClassTypes.clsDataClassifier:
                    case ClassTypes.clsBridgeClassifier:
                    case ClassTypes.clsFactData:
                        attributes.Add(BrowsableAttribute.Yes);
                        break;
                }
            }

            return attributes.ToArray();
        }
    }
}
