using Infragistics.Win.UltraWinExplorerBar;

using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;


namespace Krista.FM.Client.Workplace.Gui
{
    public class ExplorerPane : AbstractPaneContent
    {
        private static ExplorerPane instance;

        public static ExplorerPane Instance
        {
            get { return instance; }
        }

        private UltraExplorerBarEx navigationControl;

        public ExplorerPane()
        {
            instance = this;

            navigationControl = new UltraExplorerBarEx();
            UltraExplorerBarGroup ultraExplorerBarGroup1 = new UltraExplorerBarGroup();
            UltraExplorerBarContainerControl ultraExplorerBarContainerControl = new UltraExplorerBarContainerControl();

            ((System.ComponentModel.ISupportInitialize)(navigationControl)).BeginInit();
            navigationControl.SuspendLayout();

            // 
            // ultraExplorerBarContainerControl
            // 
            ultraExplorerBarContainerControl.Location = new System.Drawing.Point(1, 26);
            ultraExplorerBarContainerControl.Name = "ultraExplorerBarContainerControl";
            ultraExplorerBarContainerControl.Size = new System.Drawing.Size(256, 408);
            ultraExplorerBarContainerControl.TabIndex = 0;
            // 
            // ultraExplorerBarGroup1
            // 
            ultraExplorerBarGroup1.Container = ultraExplorerBarContainerControl;
            ultraExplorerBarGroup1.Text = "Пустая";
            // 
            // navigationControl
            // 
            navigationControl.AnimationEnabled = false;
            navigationControl.AnimationSpeed = AnimationSpeed.Fast;
            navigationControl.Controls.Add(ultraExplorerBarContainerControl);
            navigationControl.Dock = System.Windows.Forms.DockStyle.Left;
            navigationControl.Groups.AddRange(new UltraExplorerBarGroup[] {ultraExplorerBarGroup1});
            navigationControl.GroupSettings.ItemSort = ItemSortType.None;
            navigationControl.GroupSettings.Style = GroupStyle.ControlContainer;
            navigationControl.ImageSizeLarge = new System.Drawing.Size(24, 24);
            navigationControl.ImageTransparentColor = System.Drawing.Color.Magenta;
            navigationControl.Location = new System.Drawing.Point(0, 25);
            navigationControl.Name = "navigationControl";
            navigationControl.Size = new System.Drawing.Size(258, 525);
            navigationControl.Style = UltraExplorerBarStyle.OutlookNavigationPane;
            navigationControl.TabIndex = 15;
            navigationControl.ViewStyle = UltraExplorerBarViewStyle.Office2003;
            navigationControl.ActiveGroupChanging += OnActiveGroupChanging;
            navigationControl.ActiveGroupChanged += OnActiveGroupChanged;

            ((System.ComponentModel.ISupportInitialize)(navigationControl)).EndInit();
            navigationControl.ResumeLayout(false);
        }

        public override System.Windows.Forms.Control Control
        {
	        get { return navigationControl; }
        }

        public event ActiveGroupChangingEventHandler ActiveGroupChanging;

        private void OnActiveGroupChanging(object sender, CancelableGroupEventArgs e)
        {
            if (ActiveGroupChanging != null)
            {
                ActiveGroupChanging(navigationControl, e);
            }
        }

        public event ActiveGroupChangedEventHandler ActiveGroupChanged;

        private void OnActiveGroupChanged(object sender, GroupEventArgs e)
        {
            if (ActiveGroupChanged != null)
            {
                ActiveGroupChanged(navigationControl, e);
            }
        }
    }
}
