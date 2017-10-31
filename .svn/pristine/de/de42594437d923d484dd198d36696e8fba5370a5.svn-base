using System;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert.FieldList
{
    /// <summary>
    /// Данные поля  
    /// </summary>
    public class ItemData
    {
        private ItemType itemType;
        private Object adomdObj;
        private PivotObject pivotObj;

        public ItemType ItemType
        {
            get
            {
                return itemType;
            }
            set
            {
                itemType = value;
            }
        }

        public Object AdomdObj
        {
            get { return adomdObj; }
            set { adomdObj = value; }
        }

        public PivotObject PivotObj
        {
            get { return pivotObj; }
            set { pivotObj = value; }
        }

        public ItemData(ItemType itemType, Object adomdObj, PivotObject pivotObj)
        {
            this.itemType = itemType;
            this.adomdObj = new Object();
            this.adomdObj = adomdObj;
            this.pivotObj = pivotObj;
        }
    }

}
