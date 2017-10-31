using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Krista.FM.Client.Design.Editors
{
    public partial class SQLScriptEditorForm : Form
    {
        public SQLScriptEditorForm()
        {
            InitializeComponent();
            Krista.FM.Client.Common.DefaultFormState.Load(this);
        }

        public void SetText(List<string> text)
        {
            richTextBox.ResetText();
            richTextBox.Text = String.Join(Environment.NewLine + Environment.NewLine, text.ToArray());
        }

        private void SQLScriptEditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Krista.FM.Client.Common.DefaultFormState.Save(this);
        }
    }
}