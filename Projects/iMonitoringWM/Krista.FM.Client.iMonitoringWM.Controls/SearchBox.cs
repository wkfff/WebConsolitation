using System;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.iMonitoringWM.Common;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class SearchBox : UserControl
    {
        public new event EventHandler TextChanged
        {
            add { this.textBox.TextChanged += value; }
            remove { this.textBox.TextChanged -= value; }
        }

        public new event MouseEventHandler MouseDown
        {
            add { this.textBox.MouseDownEX += value; }
            remove { this.textBox.MouseDownEX -= value; }
        }

        public override string Text
        {
            get { return this.textBox.Text; }
            set { this.textBox.Text = value; }
        }

        public SearchBox()
        {
            InitializeComponent();
            if (Utils.ScreenSize == ScreenSizeMode.s240x320)
                this.textBox.Height--;
        }

        private void findLeftPart_MouseDown(object sender, MouseEventArgs e)
        {
            this.textBox.OnMouseDownEX(e);
        }

        private void findRightPart_MouseDown(object sender, MouseEventArgs e)
        {
            this.textBox.OnMouseDownEX(e);
        }
    }
}
