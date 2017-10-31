using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    public partial class SelectCommentForm : Form
    {
        /// <summary>
        /// Вызов формы с выбором комментария к данным
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static bool ShowSaveCalcResultsForm(List<string> comments, ref string comment)
        {
            SelectCommentForm form = new SelectCommentForm();
            form.cbCommentsList.Items.Add("Новый комментарий");
            foreach (string comm in comments)
                form.cbCommentsList.Items.Add(comm);
            form.cbCommentsList.SelectedIndex = 0;
            if (form.ShowDialog() == DialogResult.OK)
            {
                comment = form.tbComment.Text;
                return true;
            }
            return false;
        }

        public SelectCommentForm()
        {
            InitializeComponent();
        }


        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbComment.Text))
            {
                MessageBox.Show("Для сохранения результата расчета необходимо ввести комментарий", "Сохранение данных",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cbCommentsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCommentsList.SelectedIndex == 0)
                tbComment.Text = string.Empty;
            else
                tbComment.Text = cbCommentsList.SelectedItem.ToString();
        }

        private void tbComment_TextChanged(object sender, EventArgs e)
        {
            ultraButton1.Enabled = !string.IsNullOrEmpty(tbComment.Text);
        }
    }
}