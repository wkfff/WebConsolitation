using System.Windows.Forms;
using Krista.FM.Update.Framework.Controls;

namespace Krista.FM.Update.Framework.Forms
{
    public partial class NotifyIconForm : Form
    {
        public NotifyIconForm(UpdateManager manager)
        {
            InitializeComponent();
            notifyIconControl.Manager = manager;
        }

        public NotifyIconControl NotifyIconControl
        {
            get { return notifyIconControl; }
            set { notifyIconControl = value; }
        }
    }
}
