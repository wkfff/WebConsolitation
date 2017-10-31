using System;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;
using Infragistics.Win.UltraWinListView;
using System.Windows.Forms.Design;

namespace Krista.FM.Client.MDXExpert
{
    public partial class AxisComponentListControl : UserControl
    {
        private Axis ax = null;
    
        public AxisComponentListControl()
        {
            InitializeComponent();
        }
        

        public AxisComponentListControl(Axis initialAxis)
        {
            InitializeComponent();
            if (initialAxis != null)
            {
                ax = initialAxis;
                InitAxisElements();
            }            
        }        
        
        private void InitAxisElements()
        {
            UltraListViewItem lvItem;
            lvAxisElems.Items.Clear();


            foreach (FieldSet fs in ax.FieldSets)
            {
                lvItem = lvAxisElems.Items.Add(fs.UniqueName);
                lvItem.Value = fs.UniqueName;
                if (fs.UsedInChartLabels)
                {
                    lvItem.CheckState = CheckState.Checked;
                }
                else
                {
                    lvItem.CheckState = CheckState.Unchecked;
                }
            }        
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lvAxisElems.Items.Count; i++)
            {
                ax.FieldSets[i].UsedInChartLabels = (lvAxisElems.Items[i].CheckState == CheckState.Checked);
            }

            if (Tag != null)
            {
                ((IWindowsFormsEditorService)Tag).CloseDropDown();
            }
            else
            {
                this.Hide();
            }
            
        }
    }
}
