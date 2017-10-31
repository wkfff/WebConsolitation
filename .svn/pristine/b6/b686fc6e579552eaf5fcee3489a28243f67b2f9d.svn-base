using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.DataWarning
{
    public class DataWarningNotifier
    {
        public Control DataControl
        {
            get; set;
        }

        public string WarningMessage
        {
            get; set;
        }

        private PictureBox WarningPicture
        {
            get; set;
        }

        public DataWarningNotifier(Control control, string message)
        {
            WarningMessage = message;
            WarningPicture = new PictureBox();
            WarningPicture.Size = new Size(16, 16);
            WarningPicture.Image = Resources.ru.error_small;
            WarningPicture.MouseEnter += WarningPicture_MouseEnter;
            WarningPicture.MouseLeave += WarningPicture_MouseLeave;
            control.Parent.Controls.Add(WarningPicture);
            WarningPicture.Location = new Point(control.Location.X - 18, control.Location.Y + 3);
            WarningPicture.Location.Offset(-18, 0);
            WarningPicture.Show();

            ToolTip = new Infragistics.Win.ToolTip(WarningPicture);
            ToolTip.DisplayShadow = true;
            ToolTip.AutoPopDelay = 0;
            ToolTip.InitialDelay = 0;
        }

        void WarningPicture_MouseLeave(object sender, EventArgs e)
        {
            ToolTip.Hide();
        }

        void WarningPicture_MouseEnter(object sender, EventArgs e)
        {
            ToolTip.ToolTipText = WarningMessage;
            var tooltipPos = new Point(WarningPicture.ClientRectangle.Left, WarningPicture.ClientRectangle.Bottom);
            tooltipPos = WarningPicture.PointToScreen(tooltipPos);
            ToolTip.Show(tooltipPos);
        }

        #region тултипы для компонентов

        private Infragistics.Win.ToolTip ToolTip
        { 
            get; set;
        }

        #endregion

        public void Hide()
        {
            WarningPicture.Hide();
        }
    }
}
