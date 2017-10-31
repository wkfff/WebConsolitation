using Infragistics.Win.UltraWinStatusBar;

namespace Krista.FM.Utils.DTSGenerator
{
    /// <summary>
    /// Свой статусбар
    /// </summary>
    public partial class SSISStatusBar : UltraStatusBar
    {
        /// <summary>
        /// Панель подключения к источнику
        /// </summary>
        private UltraStatusPanel panelSourceScheme;

        /// <summary>
        /// Панель подключения к преемнику
        /// </summary>
        private UltraStatusPanel panelDestinationScheme;

        /// <summary>
        /// Панель с именем пользователя
        /// </summary>
        private UltraStatusPanel panelUserName;

        public SSISStatusBar()
        {
            InitializeComponent();
        }

        public void CreateUltraStatusPanels()
        {
            Panels.Clear();

            panelSourceScheme = new UltraStatusPanel();
            panelSourceScheme.SizingMode = PanelSizingMode.Automatic;

            panelDestinationScheme = new UltraStatusPanel();
            panelDestinationScheme.SizingMode = PanelSizingMode.Automatic;

            panelUserName = new UltraStatusPanel();
            panelUserName.SizingMode = PanelSizingMode.Automatic;

            Panels.AddRange(new UltraStatusPanel[] {panelSourceScheme, panelDestinationScheme, panelUserName});
        }


        public UltraStatusPanel PanelUserName
        {
            get { return panelUserName; }
            set { panelUserName = value; }
        }

        public UltraStatusPanel PanelDestinationScheme
        {
            get { return panelDestinationScheme; }
            set { panelDestinationScheme = value; }
        }

        public UltraStatusPanel PanelSourceScheme
        {
            get { return panelSourceScheme; }
            set { panelSourceScheme = value; }
        }
    }
}