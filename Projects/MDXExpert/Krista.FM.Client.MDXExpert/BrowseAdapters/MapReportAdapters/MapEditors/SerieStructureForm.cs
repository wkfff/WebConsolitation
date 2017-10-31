using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;

namespace Krista.FM.Client.MDXExpert
{
    public partial class SerieStructureForm : Form
    {
        #region поля

        private List<string> serieNames;
        private List<string> measureNames;

        #endregion

        #region свойства

        public List<string> SerieNames
        {
            get { return serieNames; }
        }

        public List<string> MeasureNames
        {
            get { return measureNames; }
        }

        #endregion
 
        public SerieStructureForm(List<string> serieNames, List<string> measureNames)
        {
            InitializeComponent();
            this.serieNames = serieNames;
            this.measureNames = measureNames;
            InitEditor();

        }

        private void InitEditor()
        {
            foreach (string serieName in this.serieNames)
                lvSeries.Items.Add(serieName, serieName);

            foreach (string measureName in this.measureNames)
                lvMeasures.Items.Add(measureName, measureName);
        }

        private void AddSerie()
        {
            if (teSerieName.Text == "")
                return;

            if (lvSeries.Items.IndexOf(teSerieName.Text) > -1)
                return;

            lvSeries.Items.Add(teSerieName.Text, teSerieName.Text);
        }

        private void DeleteSerie()
        {
            foreach (UltraListViewItem lvItem in lvSeries.SelectedItems)
                lvSeries.Items.Remove(lvItem);
        }

        private void AddMeasure()
        {
            if (teMeasureName.Text == "")
                return;

            if (lvMeasures.Items.IndexOf(teMeasureName.Text) > -1)
                return;

            lvMeasures.Items.Add(teMeasureName.Text, teMeasureName.Text);
        }

        private void DeleteMeasure()
        {
            foreach (UltraListViewItem lvItem in lvMeasures.SelectedItems)
                lvMeasures.Items.Remove(lvItem);
        }


        #region обработчики

        private void btAddSerie_Click(object sender, EventArgs e)
        {
            AddSerie();
        }

        private void btDeleteSerie_Click(object sender, EventArgs e)
        {
            DeleteSerie();
        }

        private void btAddMeasure_Click(object sender, EventArgs e)
        {
            AddMeasure();
        }

        private void btDeleteMeasure_Click(object sender, EventArgs e)
        {
            DeleteMeasure();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this.serieNames.Clear();
            foreach (UltraListViewItem lvItem in lvSeries.Items)
                this.serieNames.Add(lvItem.Text);

            this.measureNames.Clear();
            foreach (UltraListViewItem lvItem in lvMeasures.Items)
                this.measureNames.Add(lvItem.Text);

            Close();
        }


        private void teSerieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
                AddSerie();
        }

        private void teMeasureName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
                AddMeasure();
        }

        private void lvSeries_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
                DeleteSerie();
        }

        private void lvMeasures_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
                DeleteMeasure();
        }

        #endregion

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}