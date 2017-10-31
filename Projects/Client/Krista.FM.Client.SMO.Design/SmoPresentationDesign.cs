using System;
using System.ComponentModel;
using System.Drawing.Design;
using Krista.FM.Client.Design.Editors;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    /// <summary>
    /// Smo объект для представления объекта
    /// </summary>
    [TypeConverter(typeof(SmoPresentationPropertyVisibleSwitchTypeConverter))]
    public class SmoPresentationDesign : SmoKeyIdentifiedObjectDesign<IPresentation>, IPresentation
    {
        public SmoPresentationDesign(IPresentation serverObject)
            : base(serverObject)
        {
        }

        #region IPresentation Members

        [Browsable(false)]
        public new IPresentation ServerControl
        {
            get { return (IPresentation)serverControl; }
        }

        public string Name
        {
            get
            {
                return ServerControl.Name;
            }
            set
            {
                ServerControl.Name = value;
            }
        }


        public IDataAttributeCollection Attributes
        {
            get
            {
                return
                    ServerControl.Attributes;
            }
        }


        #endregion

        #region IPresentation Members

        public const string levelNamingConst = "LevelNamingTemplate";

        /// <summary>
        /// 
        /// </summary>
        [DisplayName(@"Наименование уровней иерархии")]
        [Description("Наименование уровней иерархии")]
        public string LevelNamingTemplate
        {
            get
            {
                return ServerControl.LevelNamingTemplate;
            }
            set
            {
                ServerControl.LevelNamingTemplate = value;
            }
        }

        #endregion

        #region IPresentation Members

        [DisplayName(@"Конфигурация (OuterXml)")]
        [Description("XML конфигурация объекта")]
        public virtual string Configuration
        {
            get
            {
                return ServerControl.Configuration;
            }
        }

        [Browsable(false)]
        public IDataAttributeCollection GroupedAttributes
        {
            get { return serverControl.GroupedAttributes; }
        }

        #endregion
    }

    internal class SmoPresentationPropertyVisibleSwitchTypeConverter : CustomPropertiesTypeConverter<SmoPresentationDesign>
    {
        protected override Attribute[] GetPropertyAttributes(SmoPresentationDesign component, PropertyDescriptor property)
        {
            if (property.Name == SmoPresentationDesign.levelNamingConst)
            {
                if (!(component.OwnerObject is IClassifier))
                    return new Attribute[] { BrowsableAttribute.No };

                return new Attribute[] { BrowsableAttribute.Yes };
            }

            return EmptyAttributes;
        }
    }
}
