using System;
using System.Collections.Generic;
using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoPresentationCollectionDesign : SmoDictionaryBaseDesign<string, IPresentation>, IPresentationCollection
    {
        public SmoPresentationCollectionDesign(IDictionaryBase<string, IPresentation> serverControl)
            : base(serverControl)
        { }

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            return typeof(SmoPresentationDesign);
        }

        #region IPresentationCollection Members

        [DisplayName(@"Представление по умолчанию")]
        [Description("Представление по умолчанию")]
        public virtual string DefaultPresentation
        {
            get
            {
                return
                    serverControl[
                        ((IPresentationCollection)serverControl).DefaultPresentation].Name;
            }
            set
            {
                ((IPresentationCollection)serverControl).DefaultPresentation = value;
            }
        }

        public IPresentation CreateItem(string key, string name, List<IDataAttribute> attributes, string levelNamingTemplate)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
