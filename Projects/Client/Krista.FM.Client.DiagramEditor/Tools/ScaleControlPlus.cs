using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.DiagramEditor.Tools
{
    public partial class ScaleControl_ : UserControl
    {
        /// <summary>
        /// Таймер,запускается при нажатии ну кнопки +/-
        /// </summary>
        Timer timer = new Timer();

        /// <summary>
        /// Нажата
        /// </summary>
        bool downbuttonDown = false;

        bool upbuttonDown = false;

        public ScaleControl_()
        {
            InitializeComponent();

            timer.Enabled = false;
            timer.Interval = 100;

            timer.Tick += new EventHandler(timer_Tick);

            this.down.MouseDown += new MouseEventHandler(downup_MouseDown);
            this.up.MouseDown += new MouseEventHandler(downup_MouseDown);

            this.down.MouseUp += new MouseEventHandler(downup_MouseUp);
            this.up.MouseUp += new MouseEventHandler(downup_MouseUp);
        }

        private void FinalizeOperation()
        {
            downbuttonDown = false;
            upbuttonDown = false;
            timer.Enabled = false;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (downbuttonDown)
                    zoomControl.ScaleDown();
                else if (upbuttonDown)
                    zoomControl.ScaleUp();
            }
            // Перехватываем исключения.. и глушим
            catch
            {
                FinalizeOperation();
            }
        }

        void downup_MouseUp(object sender, MouseEventArgs e)
        {
            FinalizeOperation();
        }

        void downup_MouseDown(object sender, MouseEventArgs e)
        {
            timer.Enabled = true;

            Button button = sender as Button;

            if (button != null)
            {
                switch (button.Name)
                {
                    case "down" :
                        downbuttonDown = true;
                        break;
                    case "up":
                        upbuttonDown = true;
                        break;
                }
            }
        }

        public new ZoomControl ScaleControl
        {
            get { return zoomControl; }
        }
    }
}
