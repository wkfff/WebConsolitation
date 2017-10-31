using System.Collections.Generic;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoSemanticsCollection : SmoSimpleDictionary<string, string>, ISemanticsCollection
    {
        private readonly SMOSerializationInfo cache;

        readonly Dictionary<string, string> innerDictionary = new Dictionary<string, string>();

        public SmoSemanticsCollection(SMOSerializationInfo cache)
        {
            this.cache = cache;

            FillCache();
        }

        private void FillCache()
        {
            foreach (KeyValuePair<string, object> pair in cache)
            {
                innerDictionary.Add(pair.Key, pair.Value.ToString());
            }
        }

        #region ISemanticsCollection Members

        public bool ReadWrite
        {
            get { return Krista.FM.Common.FileUtils.FileHelper.IsFileReadOnly("Semantics.xml"); }
            set { Krista.FM.Common.FileUtils.FileHelper.SetFileReadAccess("Semantics.xml", value); }
        }

        #endregion

        #region ISMOSerializable Members

        public SMOSerializationInfo GetSMOObjectData()
        {
            return cache;
        }

        public SMOSerializationInfo GetSMOObjectData(LevelSerialization level)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public override string ToString()
        {
            return string.Format("SmoSemanticsIdentifier - {0} : {1}", cache["Identifier"], base.ToString());
        }

    }
}
