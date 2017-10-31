using System.ComponentModel;
using System.Security.Principal;
using Infragistics.Win.UltraWinStatusBar;

namespace Krista.FM.Client.OLAPAdmin
{
    public partial class StatusBar : UltraStatusBar
    {
        UltraStatusPanel userPanel = new UltraStatusPanel();

        public StatusBar()
        {
            InitializeComponent();

            InitializePanels();
        }

        private void InitializePanels()
        {
            userPanel.SizingMode = PanelSizingMode.Automatic;

            #region InitializePanels

            userPanel.Text = WindowsIdentity.GetCurrent().Name;

            #endregion

            this.Panels.AddRange(new UltraStatusPanel[] { userPanel });
        }

        public StatusBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            InitializePanels();
        }
    }
}
