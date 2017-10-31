using System.Windows.Forms;

using Krista.FM.Update.Framework;

namespace Krista.FM.Update.NotifyIconApp
{
    public partial class NotifyIconForm : Form
    {
       public NotifyIconForm(IUpdateManager manager)
       {
           InitializeComponent();
           notifyIconControl.Manager = manager;
       }

       public NotifyIconForm(IUpdateManager manager, bool asRemote)
           :this(manager)
       {
           notifyIconControl.AsRemote = asRemote;
       }
    }
}
