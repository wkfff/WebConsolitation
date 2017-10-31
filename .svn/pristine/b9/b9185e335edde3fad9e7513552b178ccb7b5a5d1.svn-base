using System;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;

namespace Krista.FM.Client.Common.Configuration
{
    [Serializable]
    internal class UltraGridColumnSetting
    {
        // ----------------------------------------------------------------------
        public UltraGridColumnSetting(UltraGridColumn ultraGridColumn)
        {
            if (ultraGridColumn == null)
            {
                throw new ArgumentNullException("ultraGridColumn");
            }

            name = ultraGridColumn.Key;
            DisplayIndex = ultraGridColumn.Header.VisiblePosition;
            Width = ultraGridColumn.Width;
            Hidden = ultraGridColumn.Hidden;
            IsGroupByColumn = ultraGridColumn.IsGroupByColumn;
            SortIndicator = ultraGridColumn.SortIndicator;
            CellMultiline = ultraGridColumn.CellMultiLine;
        }

        // ----------------------------------------------------------------------
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        // ----------------------------------------------------------------------
        private int displayIndex;
        public int DisplayIndex 
        {
            get { return displayIndex; }
            set { displayIndex = value; } 
        }

        // ----------------------------------------------------------------------
        private int width;
        public int Width 
        {
            get { return width; }
            set { width = value; }
        }

        // ----------------------------------------------------------------------
        private bool hidden;
        public bool Hidden 
        {
            get { return hidden; }
            set { hidden = value; }
        }

        //----------------------------------------------------------------------
        private bool isGroupByColumn;
        public bool IsGroupByColumn
        {
            get { return isGroupByColumn; }
            set { isGroupByColumn = value; }
        }

        private SortIndicator sortIndicator;
        public SortIndicator SortIndicator
        {
            get { return sortIndicator; }
            set { sortIndicator = value; }
        }

        //----------------------------------------------------------------------
        private DefaultableBoolean cellMililine;
        public DefaultableBoolean CellMultiline
        {
            get { return cellMililine; }
            set { cellMililine = value; }
        }

        // ----------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;

            UltraGridColumnSetting compare = obj as UltraGridColumnSetting;
            if (compare == null)
            {
                return false;
            }

            return
                int.Equals(DisplayIndex, compare.DisplayIndex) &&
                int.Equals(Width, compare.Width) &&
                bool.Equals(Hidden, compare.Hidden) &&
                bool.Equals(IsGroupByColumn, compare.IsGroupByColumn) &&
                (sortIndicator == compare.SortIndicator) &&
                bool.Equals(CellMultiline, compare.CellMultiline);
        } // Equals

        // ----------------------------------------------------------------------
        public override int GetHashCode()
        {
            int hash = GetType().GetHashCode();
            hash = AddHashCode(hash, DisplayIndex);
            hash = AddHashCode(hash, Width);
            hash = AddHashCode(hash, Hidden);
            hash = AddHashCode(hash, IsGroupByColumn);
            return hash;
        } // GetHashCode

        // ----------------------------------------------------------------------
        private static int AddHashCode(int hash, object obj)
        {
            int combinedHash = obj != null ? obj.GetHashCode() : 0;
            if (hash != 0)
            {
                combinedHash += hash * 31;
            }
            return combinedHash;
        }
    }
}
