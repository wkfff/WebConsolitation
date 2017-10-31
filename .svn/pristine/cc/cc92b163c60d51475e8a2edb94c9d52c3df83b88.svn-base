using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert
{
    public partial class ColorRuleCollectionForm : Form
    {
        private ColorRuleCollection colorRules;
        private bool isUpdating = false;

        public ColorRuleCollection ColorRules
        {
            get { return colorRules; }
            set { colorRules = value; }
        }

        public ColorRuleCollectionForm(ColorRuleCollection colorRules)
        {
            InitializeComponent();
            AdjustColors();

            this.ColorRules = colorRules;
            InitControls();
            
        }

        private void AdjustColors()
        {
            this.propertyGrid.BackColor = MainForm.PanelColor;
            this.propertyGrid.HelpBackColor = MainForm.PanelColor;
            this.splitter1.BackColor = MainForm.DarkPanelColor;
        }


        private void InitControls()
        {
            if (this.colorRules == null)
                return;

            if (this.colorRules.PivotData == null)
                return;

            this.ceMeasure.Items.Clear();

            try
            {
                
                foreach (Measure measure in this.colorRules.PivotData.Cube.Measures)
                {
                    this.ceMeasure.Items.Add(measure.UniqueName).DisplayText = measure.Caption;
                }

            }

            catch (Exception e)
            {
                if (e is AdomdException)
                {
                    if (AdomdExceptionHandler.ProcessOK((AdomdException)e))
                    {
                        AdomdExceptionHandler.IsRepeatedProcess = true;
                        this.InitControls();
                        AdomdExceptionHandler.IsRepeatedProcess = false;
                        return;
                    }
                }

                Common.CommonUtils.ProcessException(e);
            }



            foreach (PivotTotal total in this.colorRules.PivotData.TotalAxis.Totals)
            {
                if (total.IsCustomTotal)
                {
                    this.ceMeasure.Items.Add(total.UniqueName).DisplayText = total.Caption;
                }
            }


            if (this.ceMeasure.Items.Count > 0)
                this.ceMeasure.SelectedIndex = 0;
            
            this.lvColorRules.Items.Clear();
            foreach(ColorRule rule in this.ColorRules)
            {
                this.lvColorRules.Items.Add(this.lvColorRules.Items.Count.ToString(), rule);
            }

            if (this.lvColorRules.Items.Count > 0)
            {
                this.lvColorRules.SelectedItems.Add(this.lvColorRules.Items[0]);
            }
        }

        private void InitEditor(ColorRule rule)
        {
            this.isUpdating = true;
            this.ceMeasure.Value = rule.MeasureName;
            this.ceCells.Checked = ((rule.Area & ColorRuleArea.Cells) == ColorRuleArea.Cells);
            this.ceTotals.Checked = ((rule.Area & ColorRuleArea.Totals) == ColorRuleArea.Totals);

            this.propertyGrid.SelectedObject = new ColorRuleBrowseClass(rule);
            this.isUpdating = false;
        }

        private ColorRule GetSelectedColorRule()
        {
            return (this.lvColorRules.SelectedItems.Count > 0) ? (ColorRule)this.lvColorRules.SelectedItems[0].Value : null;
        }

        /// <summary>
        /// Обновить заголовки правил
        /// </summary>
        private void RefreshRulesCaptions()
        {
            //для того чтоб обновились заголовки, нужно заново выделить правило
            if (this.lvColorRules.SelectedItems.Count > 0)
            {
                UltraListViewItem selectedItem = this.lvColorRules.SelectedItems[0];
                this.lvColorRules.SelectedItems.Clear();
                this.lvColorRules.SelectedItems.Add(selectedItem);
            }
        }

        private void btAddRule_Click(object sender, EventArgs e)
        {
            if (this.ceMeasure.Items.Count == 0)
                return;

            ColorRule rule = new ColorRule((string)this.ceMeasure.Value, ColorCondition.Equal, 0, 0);
            this.ColorRules.Add(rule);
            UltraListViewItem newItem = this.lvColorRules.Items.Add(Guid.NewGuid().ToString(), rule);

            this.lvColorRules.SelectedItems.Clear();
            this.lvColorRules.SelectedItems.Add(newItem);
        }



        private void lvColorRules_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count > 0)
                InitEditor((ColorRule)e.SelectedItems[0].Value);
        }

        private void ceMeasure_ValueChanged(object sender, EventArgs e)
        {
            if (isUpdating)
                return;

            ColorRule rule = GetSelectedColorRule();
            if ((rule != null) && (this.ceMeasure.Value != null))
            {
                rule.MeasureName = (string)this.ceMeasure.Value;
                RefreshRulesCaptions();
            }
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ColorRule rule = GetSelectedColorRule();

            if (rule != null)
            {
                RefreshRulesCaptions();
            }
        }

        private void ceCells_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdating)
                return;

            ColorRule rule = GetSelectedColorRule();
            if (rule != null)
            {
                rule.Area = this.ceCells.Checked ? rule.Area | ColorRuleArea.Cells : (ColorRuleArea) (rule.Area - ColorRuleArea.Cells);
            }
        }

        private void ceTotals_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdating)
                return;

            ColorRule rule = GetSelectedColorRule();
            if (rule != null)
            {
                rule.Area = this.ceTotals.Checked ? rule.Area | ColorRuleArea.Totals : (ColorRuleArea)(rule.Area - ColorRuleArea.Totals);
            }
        }

        private void btDeleteRule_Click(object sender, EventArgs e)
        {
            if (this.lvColorRules.SelectedItems.Count > 0)
            {
                UltraListViewItem item = this.lvColorRules.SelectedItems[0];
                ColorRule rule = (ColorRule)item.Value;
                this.lvColorRules.Items.Remove(item);
                this.ColorRules.Delete(rule);
                
                if (this.lvColorRules.Items.Count > 0)
                {
                    this.lvColorRules.SelectedItems.Add(this.lvColorRules.Items[0]);
                }
            }
        }


    }
}
