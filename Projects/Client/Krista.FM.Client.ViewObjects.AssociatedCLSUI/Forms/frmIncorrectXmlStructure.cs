using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;

using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    public partial class frmIncorrectXmlStructure : Form
    {
        public static bool ShowIncorrectXmlStructure(DataTable table, IWin32Window _IWin32Window, bool allowSaveData, string objectName)
        {
            frmIncorrectXmlStructure frm = new frmIncorrectXmlStructure();
            
            string caption = string.Format("Обнаружены различия в структуре {0} и XML документа", objectName);
            if (!allowSaveData)
            {
                caption = caption + ". Данные невозможно загрузить";
                frm.pbImages.Image = Properties.Resources.error2;
            }
            else
            {
                frm.pbImages.Image = Properties.Resources.Warning2;
            }
            frm.lCaption.Text = caption;
            InfragisticComponentsCustomize.CustomizeUltraGridParams(frm.ug);
            frm.ug.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(ug_InitializeRow);
            frm.ug.DataSource = table;
            frm.ug.DisplayLayout.Bands[0].Columns[2].Hidden = true;
            frm.ug.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.Horizontal;
            frm.ug.DisplayLayout.Bands[0].Columns[0].Width = 200;
            frm.ug.DisplayLayout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            frm.ug.DisplayLayout.Bands[0].Columns[1].Width = 200;
            frm.ug.DisplayLayout.Bands[0].Columns[1].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            return frm.ShowDialog(_IWin32Window) == DialogResult.OK;
        }

        static void ug_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;
            if (Convert.ToBoolean(row.Cells[2].Value))
            {
                row.Appearance.ForeColor = System.Drawing.Color.Red;
                row.ToolTipText = "Критическое отличие структуры";
            }
        }

        public frmIncorrectXmlStructure()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}