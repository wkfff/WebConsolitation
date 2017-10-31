using System;
using System.Windows.Forms;
using Infragistics.Win.UltraWinDock;

namespace Krista.FM.Client.MDXExpert
{
    // Форма редактирования MDX запроса
    public partial class MDXQueryForm : Form
    {
        #region Поля
        
        // Активная панель
        private CustomReportElement activeElement;

        #endregion

        public MDXQueryForm(CustomReportElement reportElement)
        {
            InitializeComponent();
            this.activeElement = reportElement;
            this.Text = "MDX запрос для " + this.activeElement.Title;
            rtbQuery.Text = this.activeElement.PivotData.MDXQuery;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            activeElement.PivotData.IsCustomMDX = true;
            activeElement.ExecuteMDXQuery(rtbQuery.Text);
            Close();
        }
    }
}