using System;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoDimensionLevelCollection : SmoDictionaryBase<string, IDimensionLevel>, IDimensionLevelCollection, IMinorModifiable
    {
        public SmoDimensionLevelCollection(IDictionaryBase<string, IDimensionLevel> serverObject)
            : base(serverObject)
        {
        }

        public SmoDimensionLevelCollection(SMOSerializationInfo info)
            : base(info)
        {
        }

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            if (this.HierarchyType == HierarchyType.Regular)
                return typeof(SmoDimensionLevelRegular);
            return typeof (SmoDimensionLevel);
        }
        

        #region IDimensionLevelCollection Members

        public HierarchyType HierarchyType
        {
            get {
                return
                    cached
                        ? (HierarchyType) GetCachedValue("HierarchyType")
                        : ((IDimensionLevelCollection) serverControl).HierarchyType; }
            set { ((IDimensionLevelCollection)serverControl).HierarchyType = value; CallOnChange(); }
        }

        public IDimensionLevel CreateItem(string name, LevelTypes levelType)
        {
            return ((IDimensionLevelCollection)serverControl).CreateItem(name, levelType);
        }

        public bool AllHidden
        {
            get 
            {
                return cached ? (bool)GetCachedValue("AllHidden") :
                ((IDimensionLevelCollection) serverControl).AllHidden;
            }

            set
            {
                ((IDimensionLevelCollection) serverControl).AllHidden = value;
            }
        }

        #endregion

        #region IMinorModifiable Members

        public void Update(IModifiable toObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IModifiable Members

        public IModificationItem GetChanges(IModifiable toObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
