using System.Windows.Forms;

using Krista.FM.Client.Common;


namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
    public partial class TextRepPreview : Form
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public TextRepPreview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Локализация компонентов
        /// </summary>
        public void Localize()
        {
            InfragisticsRusification.LocalizeAll();
        }
    }
}