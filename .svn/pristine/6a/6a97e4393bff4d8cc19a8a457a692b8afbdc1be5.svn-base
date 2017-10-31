using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Krista.FM.Expert.PivotData
{
    public class CalcTotal : PivotObject
    {
        #region поля

        private string caption;
        private string uniqueName;
        private ValueFormat format;
        private SortType sortType = SortType.None;
        private string expression = "0";
        
        #endregion 

        #region свойства

        public string Caption
        {
            get 
            { 
                return caption; 
            }
            set 
            { 
                caption = value;
                this.ParentPivotData.DoAppearanceChanged(false);
            }
        }

        public string UniqueName
        {
            get 
            { 
                return uniqueName; 
            }
        }

        public ValueFormat Format
        {
            get 
            { 
                return format; 
            }
            set 
            { 
                format = value;
                this.ParentPivotData.DoAppearanceChanged(false);
            }
        }

        public SortType SortType
        {
            get { return sortType; }
            set
            {
                sortType = value;
                RefreshTotalsSort();
                this.ParentPivotData.DoDataChanged();
            }
        }

        public string Expression
        {
            get
            {
                return expression;
            }
            set
            {
                expression = value;
                this.ParentPivotData.DoDataChanged();
            }
        }

        #endregion

        public CalcTotal(PivotData pivotData, string uName, string caption, string expression)
        {
            this.uniqueName = uName;
            this.caption = caption;
            this.expression = expression;

            this._parentPivotData = pivotData;
            this.objectType = PivotObjectType.poTotal;

            this.format = new ValueFormat();
            this.format.Changed += new ValueFormatEventHandler(ValueFormatChanged);
        }

        private void ValueFormatChanged()
        {
            this.ParentPivotData.DoAppearanceChanged(false);
        }

        private void RefreshTotalsSort()
        {
            if (SortType == SortType.None)
            {
                return;
            }

            foreach(CalcTotal total in this.ParentPivotData.TotalAxis.CalcTotals)
            {
                if (total != this)
                {
                    total.SortType = SortType.None;
                }
            }
        }
    }
}
