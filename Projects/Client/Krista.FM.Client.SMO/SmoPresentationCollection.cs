using System;
using System.Collections.Generic;

using Krista.FM.ServerLibrary;
using System.ComponentModel;

namespace Krista.FM.Client.SMO
{
    public class SmoPresentationCollection : SmoDictionaryBase<string, IPresentation>, IPresentationCollection
    {
         public SmoPresentationCollection(SMOSerializationInfo cache) : 
            base(cache)
        {
        }

        public SmoPresentationCollection(IDictionaryBase<string, IPresentation> serverObject)
            : base(serverObject)
        {}

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            return typeof(SmoPresentation);
        }

        #region IPresentationCollection Members

        public virtual string DefaultPresentation
        {
            get
            {
                return
                    ((IPresentationCollection) serverControl)[
                        ((IPresentationCollection) serverControl).DefaultPresentation].Name;
            }
            set
            {
                ((IPresentationCollection)serverControl).DefaultPresentation = value;
            }
        }

        public IPresentation CreateItem(string key, string name, List<IDataAttribute> attributes, string levelNamingTemplate)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
