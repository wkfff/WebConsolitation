using System;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO.Design
{
    public class SmoDataKindCollectionDesign : SmoDictionaryBaseDesign<string, IDataKind>, IDataKindCollection
    {
        public SmoDataKindCollectionDesign(IDataKindCollection serverObject)
            : base(serverObject)
        {
        }

        #region IDataKindCollection Members

        public IDataKind New()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Add(IDataKind dataKind)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
