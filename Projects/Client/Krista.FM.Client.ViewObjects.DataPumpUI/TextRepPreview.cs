using System.Windows.Forms;

using Krista.FM.Client.Common;


namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
    public partial class TextRepPreview : Form
    {
        /// <summary>
        /// �����������
        /// </summary>
        public TextRepPreview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ����������� �����������
        /// </summary>
        public void Localize()
        {
            InfragisticsRusification.LocalizeAll();
        }
    }
}