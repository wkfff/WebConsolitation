using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using Infragistics.Win.UltraWinListView;

namespace Krista.FM.Client.MDXExpert
{
    public partial class MapIntervalNameForm : Form
    {
        private bool isSymbols = true;
        private PredefinedSymbolCollection symbols;
        private MapReportElement mapElement;
        private CustomColorCollection colors;

        public MapIntervalNameForm()
        {
            InitializeComponent();
        }

        public MapIntervalNameForm(PredefinedSymbolCollection symbols, MapReportElement mapElement)
        {
            InitializeComponent();
            this.mapElement = mapElement;

            this.symbols = symbols;
            this.isSymbols = true;
            Init();
        }

        public MapIntervalNameForm(CustomColorCollection colors, MapReportElement mapElement)
        {
            InitializeComponent();

            this.isSymbols = false;
            this.colors = colors;
            this.mapElement = mapElement;
            Init();
        }
        

        private void Init()
        {
            lvIntervals.Items.Clear();
            string intervalName = String.Empty;

            if (this.isSymbols)
            {
                int i = 0;
                foreach (PredefinedSymbol symbol in this.symbols)
                {
                    i++;
                    intervalName = String.Format("Интервал {0}", i);
                    UltraListViewItem item  = lvIntervals.Items.Add(intervalName, intervalName);
                    item.Tag = new SymbolIntervalBrowseClass(symbol);
                }
            }
            else
            {
                int i = 0;
                foreach (CustomColor color in this.colors)
                {
                    i++;
                    intervalName = String.Format("Интервал {0}", i);
                    UltraListViewItem item = lvIntervals.Items.Add(intervalName, intervalName);
                    item.Tag = new ColorIntervalBrowseClass(color, this.mapElement);
                }
            }

            if (lvIntervals.Items.Count > 0)
            {
                lvIntervals.SelectedItems.Clear();
                lvIntervals.SelectedItems.Add(lvIntervals.Items[0]);
            }

        }

        private void lvIntervals_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            this.propertyGrid.SelectedObject = lvIntervals.SelectedItems[0].Tag;
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (this.mapElement != null)
                this.mapElement.MainForm.Saved = false;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (this.mapElement != null) 
                this.mapElement.RefreshMapAppearance();
        }


    }
}
