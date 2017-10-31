using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTabbedMdi;


namespace Krista.FM.Client.Workplace.Services
{
    public class TabbedMdiService
    {
        private static UltraTabbedMdiManager tabbedMdiManager;

        public static void Attach(Form parentForm)
        {
            tabbedMdiManager = new UltraTabbedMdiManager(parentForm.Container);

            ((System.ComponentModel.ISupportInitialize)(tabbedMdiManager)).BeginInit();

            tabbedMdiManager.ImageTransparentColor = System.Drawing.Color.Magenta;
            tabbedMdiManager.MdiParent = parentForm;
            tabbedMdiManager.TabGroupSettings.ShowTabListButton = Infragistics.Win.DefaultableBoolean.True;
            tabbedMdiManager.TabSettings.DisplayFormIcon = Infragistics.Win.DefaultableBoolean.True;

            ((System.ComponentModel.ISupportInitialize)(tabbedMdiManager)).EndInit();
        }

        public static UltraTabbedMdiManager Control
        {
            get
            {
                System.Diagnostics.Debug.Assert(tabbedMdiManager != null);
                return tabbedMdiManager;
            }
        }

        public static Form ActiveWindow
        {
            get
            {
                System.Diagnostics.Debug.Assert(tabbedMdiManager != null);
                return tabbedMdiManager.ActiveTab != null ? tabbedMdiManager.ActiveTab.Form : null;
            }
        }
    }
}
