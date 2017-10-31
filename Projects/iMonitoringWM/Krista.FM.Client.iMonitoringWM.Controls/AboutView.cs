using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.iMonitoringWM.Common;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class AboutView : UserControl
    {
        #region События
        private event EventHandler _closeAboutView;

        public event EventHandler CloseAboutView
        {
            add { _closeAboutView += value; }
            remove { _closeAboutView -= value; }
        }
        #endregion

        public bool IsInitializedVisualComponent
        {
            get { return this.mainControlsPanel != null; }
        }

        public AboutView()
        {
        }

        public void InitVisualComponent()
        {
            InitializeComponent();
            this.lbVersion.Text = Consts.VersionNumber;
        }

        /// <summary>
        /// Извлекает из реестра путь к браузеру по умолчанию
        /// </summary>
        /// <returns></returns>
        private string GetBrowserDefault()
        {
            string result = Utils.GetRegValue(Microsoft.Win32.Registry.ClassesRoot,
                @"\htmlfile\Shell\Open\Command", "Default", "iexplore");
            result = result.Replace("\"", "");
            string extension = ".exe";
            int dotIndex = result.IndexOf(extension);
            if (dotIndex > 0)
                result = result.Remove(dotIndex + extension.Length, result.Length - dotIndex - extension.Length);
            return result;
        }

        private void ShowProductSite()
        {
            string defBrowser = this.GetBrowserDefault();
            Win32Helper.CreateProcess(defBrowser, Consts.wmPage, IntPtr.Zero,
                IntPtr.Zero, false, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        protected virtual void OnCloseAboutView()
        {
            if (this._closeAboutView != null)
                this._closeAboutView(this, new EventArgs());
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            this.OnCloseAboutView();
        }

        private void imCallSupport_Click(object sender, EventArgs e)
        {
            OpenNETCF.Phone.Phone.MakeCall(Consts.supportNumber1);
        }

        private void btSite_Click(object sender, EventArgs e)
        {
            string defBrowser = this.GetBrowserDefault();
            Win32Helper.CreateProcess(defBrowser, Consts.kristaPage, IntPtr.Zero,
                IntPtr.Zero, false, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        private void btProductName_Click(object sender, EventArgs e)
        {
            this.ShowProductSite();
        }

        private void btSolutionName2_Click(object sender, EventArgs e)
        {
            this.ShowProductSite();
        }

        private void btSolutionName_Click(object sender, EventArgs e)
        {
            this.ShowProductSite();
        }
    }
}
