using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Krista.FM.Client.MDXExpert.Data
{
    public abstract class PivotObject 
    {
        #region поля

        protected PivotObjectType objectType;
        protected PivotData _parentPivotData;

        #endregion

        #region свойства

        [DisplayName("Тип объекта")]
        //[TypeConverter(typeof(EnumTypeConvertor))]
        [Browsable(true)]
        public PivotObjectType ObjectType
        {
            get { return objectType; }
        }

        [Browsable(false)]
        public PivotData ParentPivotData
        {
            get { return _parentPivotData; }
        }

        [Browsable(false)]
        public PivotDataType PivotDataType
        {
            get { return ParentPivotData.Type; }
        }

        #endregion 
    }
}
