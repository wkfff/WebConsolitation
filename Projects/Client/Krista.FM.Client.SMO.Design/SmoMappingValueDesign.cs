using System;
using System.ComponentModel;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SmoAssociateMappingValueDesign : SmoMappingValueDesign
    {
        public SmoAssociateMappingValueDesign(IMappingValue serverControl, bool isBridge)
            : base(serverControl, isBridge)
        {
        }

        [DisplayName(@"Атрибут")]
        [Description("Поле таблицы.")]
        [TypeConverter(typeof(EntityAttributesListConverter))]
        public override string AttributeName
        {
            get { return SmoAttributeDesign.GetAttributeCaption(serverControl.Attribute); }
            set { serverControl.Attribute = DataAttributeHelper.GetByName(Parent.Attributes, EntityAttributesListConverter.ExtractName(value)); CallOnChange(); }
        }

    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SmoMappingValueDesign : SmoServerSideObjectDesign<IMappingValue>, IMappingValue
    {
        /// <summary>
        /// True - значение со стороны сопоставимого, False - значение со стороны классификатора данных
        /// </summary>
        private bool isBridge;

        public SmoMappingValueDesign(IMappingValue serverControl, bool isBridge)
            : base(serverControl)
        {
            this.isBridge = isBridge;
        }

        #region IMappingValue Members

        [DisplayName(@"Наименование поля")]
        [Description("Наименование поля из которого(в которое) будет браться(записываться) значение.")]
        public string Name
        {
            get { return serverControl.Name; }
        }

        [DisplayName(@"Вычисляемое поле")]
        [Description("Нет - значение берется из хранимого атрибута таблицы, Да - значение берется из вычисляемого поля.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool IsSample
        {
            get { return !serverControl.IsSample; }
        }

        [Browsable(false)]
        public IDataAttribute Attribute
        {
            get
            {
                if (serverControl.Attribute == null)
                    return null;

                return
                    serverControl.Attribute;
            }
            set { serverControl.Attribute = value; CallOnChange(); }
        }

        [Browsable(false)]
        public IEntity Parent
        {
            get
            {
                try
                {
                    // Получаем предка ассоциацию
                    IServerSideObject sso = OwnerObject;
                    while (!(sso is IAssociation))
                    {
                        sso = sso.OwnerObject;
                        if (sso is IPackage)
                            throw new Exception("Коллекция правил формирования не имеет предка ассоциацию.");
                    }
                    IAssociation association = (IAssociation)sso;
                    return isBridge ? association.RoleBridge : association.RoleData;
                }
                catch
                {
                    return null;
                }
            }
        }

        [DisplayName(@"Атрибут")]
        [Description("Поле таблицы.")]
        [TypeConverter(typeof(AttributesListConverter))]
        public virtual string AttributeName
        {
            get { return SmoAttributeDesign.GetAttributeCaption(serverControl.Attribute); }
            set { serverControl.Attribute = DataAttributeHelper.GetByName(Parent.Attributes, AttributesListConverter.ExtractName(value)); CallOnChange(); }
        }

        [Browsable(false)]
        public string[] SourceAttributes
        {
            get { return serverControl.SourceAttributes; }
        }

        #endregion

        public override string ToString()
        {
            if (serverControl.Attribute != null)
                return serverControl.Attribute.Caption;
            else
                return serverControl.Name;
        }

        public SMOSerializationInfo GetSMOObjectData()
        {
            throw new NotImplementedException();
        }

        public SMOSerializationInfo GetSMOObjectData(LevelSerialization level)
        {
            throw new NotImplementedException();
        }
    }
}
