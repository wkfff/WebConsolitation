using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SymbolSizeBrowseAdapter 
    {
        #region Поля

        private MapReportElement mapElement;

        #endregion

        #region Свойства

        [Description("Минимальный размер")]
        [DisplayName("Минимальный размер")]
        [Browsable(true)]
        public int MinSize
        {
          get { return SymbolSize.SymbolMinSize; }
          set 
          {
              if (value < MaxSize)
              {
                  SymbolSize.SymbolMinSize = value;
                  mapElement.RefreshMapAppearance();
              }
          }
        }

        [Description("Максимальный размер")]
        [DisplayName("Максимальный размер")]
        [Browsable(true)]
        public int MaxSize
        {
          get { return SymbolSize.SymbolMaxSize; }
          set 
          {
              if (value > MinSize)
              {
                  SymbolSize.SymbolMaxSize = value;
                  mapElement.RefreshMapAppearance();
              }
          }
        }


        #endregion

        public SymbolSizeBrowseAdapter(MapReportElement mapElement)
        {
            this.mapElement = mapElement;
        }

        public override string ToString()
        {
            return "";
        }


    }
}
