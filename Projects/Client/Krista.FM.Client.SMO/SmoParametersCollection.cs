using System;
using System.Collections.Generic;
using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoParametersCollection : SmoDictionaryBase<string, string>, IParametersCollection
    {
        public SmoParametersCollection(IDictionaryBase<string, string> serverObject)
            : base(serverObject)
        {
        }

        #region IParametersCollection Members

        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool ReadWrite
        {
            get
            {
                return ((IParametersCollection)serverControl).ReadWrite;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
