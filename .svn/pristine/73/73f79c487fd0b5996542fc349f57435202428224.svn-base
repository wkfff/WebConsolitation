using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Design.Editors
{
    public partial class SessionEditorForm : Form
    {
        IDictionary<string, ISession> sessions;

        public SessionEditorForm(IDictionary<string, ISession> sessions)
        {
            this.sessions = sessions;

            InitializeComponent();

            SessionGridControl sessionGridControl = new SessionGridControl(sessions);
            Controls.Add(sessionGridControl);
            sessionGridControl.Dock = DockStyle.Fill;

            sessionGridControl.Refresh();
        }
    }
}