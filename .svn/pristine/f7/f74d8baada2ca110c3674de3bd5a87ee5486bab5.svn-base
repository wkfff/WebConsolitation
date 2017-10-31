using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace Krista.FM.Client.Design.Editors
{
    public partial class AddSemanticForm : Form
    {
        private IDictionary<string, string> value;
        private string str;
        private string errStr = String.Empty;

        public AddSemanticForm(string str, IDictionary<string, string> value)
        {
            this.str = str;
            this.value = value;

            InitializeComponent();
        }

        public string Ret()
        {
            if (!String.IsNullOrEmpty(caption.Text) || !String.IsNullOrEmpty(name.Text))
                return caption.Text + " (" + name.Text + ")";
            else
            {
                return "none";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(caption.Text) || String.IsNullOrEmpty(name.Text))
            {
                errStr = String.Format("Оба поля являются обязательными для заполнения");
                MessageBox.Show(errStr, "Ошибка при добавлении", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (!value.ContainsKey(name.Text))
                {
                    value.Add(name.Text, caption.Text);
                }
                else
                {
                    errStr = String.Format("Запись с именим {0} уже существует", name.Text);
                    MessageBox.Show(errStr, "Ошибка при добавлении", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}