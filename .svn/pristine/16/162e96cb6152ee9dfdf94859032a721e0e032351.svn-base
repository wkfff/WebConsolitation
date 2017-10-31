using System;
using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoDimensionLevelCollectionDesign : SmoDictionaryBaseDesign<string, IDimensionLevel>, IDimensionLevelCollection
    {
        public SmoDimensionLevelCollectionDesign(IDictionaryBase<string, IDimensionLevel> serverObject)
            : base(serverObject)
        {
        }
        
        protected override Type GetItemValueSmoObjectType(object obj)
        {
            if (HierarchyType == HierarchyType.Regular)
                return typeof(SmoDimensionLevelRegularDesign);
            return typeof(SmoDimensionLevelDesign);
        }


        #region IDimensionLevelCollection Members

        public HierarchyType HierarchyType
        {
            get
            {
                return
                    ((IDimensionLevelCollection)serverControl).HierarchyType;
            }
            set { ((IDimensionLevelCollection)serverControl).HierarchyType = value; CallOnChange(); }
        }

        public IDimensionLevel CreateItem(string name, LevelTypes levelType)
        {
            return ((IDimensionLevelCollection)serverControl).CreateItem(name, levelType);
        }

        [Browsable(true)]
        [DisplayName("All - фиктивный")]
        [Description("¬ Ёксперте если дл€ измерени€ стоит свойство \"ALL - фиктивный\" - отключать ALL.  ¬ листах если дл€ измерени€ стоит свойство \"ALL - фиктивный\" - отключать ALL.")]
        [TypeConverter(typeof(BooleanConverter))]
        public bool AllHidden
        {
            get 
            {
                return ((IDimensionLevelCollection)serverControl).AllHidden;
            }

            set
            {
                ((IDimensionLevelCollection)serverControl).AllHidden = value;
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
