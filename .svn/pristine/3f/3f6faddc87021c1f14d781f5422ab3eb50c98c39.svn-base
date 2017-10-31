using System;
using System.Collections.Generic;
using System.Text;

using Infragistics.Win.UltraWinStatusBar;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// Класс инкапсулирующий работу с панелью статуса
    /// </summary>
    public class StatusBar : UltraStatusBar
    {
        /// <summary>
        /// Информационная панель сервера 
        /// </summary>
        private UltraStatusPanel serverInfoPanel = new UltraStatusPanel();

        /// <summary>
        /// Информационная панель схемы
        /// </summary>
        private UltraStatusPanel schemeInfoPanel = new UltraStatusPanel();
        
        /// <summary>
        /// Информационная панель о пользователе
        /// </summary>
        private UltraStatusPanel userInfoPanel = new UltraStatusPanel();

        /// <summary>
        /// Панель для отображения текста.
        /// </summary>
        private UltraStatusPanel txtStatusBarPanel = new UltraStatusPanel();

        /// <summary>
        /// Конструктор экземпляра класса
        /// </summary>
        public StatusBar()
        {
            //
            // ServerInfoPanel
            //
            serverInfoPanel.SizingMode = PanelSizingMode.Automatic;

            //
            // SchemeInfo
            //
            schemeInfoPanel.SizingMode = PanelSizingMode.Automatic;

            //
            // UserInfoPanel
            //
            userInfoPanel.SizingMode = PanelSizingMode.Automatic;

            txtStatusBarPanel.SizingMode = PanelSizingMode.Automatic;

            //
            // StatusBar
            //
            this.Name = "StatusBar";
            this.Size = new System.Drawing.Size(0, 23);
            this.ViewStyle = ViewStyle.VisualStudio2005;
            this.ResizeStyle = ResizeStyle.Deferred;
            this.BorderStylePanel = Infragistics.Win.UIElementBorderStyle.Solid;
            this.Panels.AddRange(new UltraStatusPanel[] { serverInfoPanel, schemeInfoPanel, userInfoPanel, txtStatusBarPanel });
        }

        /// <summary>
        /// Информационная панель схемы
        /// </summary>
        public UltraStatusPanel SchemeInfoPanel
        {
            get { return schemeInfoPanel; }
        }

        /// <summary>
        /// Информационная панель о пользователе
        /// </summary>
        public UltraStatusPanel UserInfoPanel
        {
            get { return userInfoPanel; }
        }

        /// <summary>
        /// Информационная панель о сервере
        /// </summary>
        public UltraStatusPanel ServerInfoPanel
        {
            get { return serverInfoPanel; }
        }

        private string currentMessage;

        void UpdateText()
        {
            txtStatusBarPanel.Text = currentMessage;
        }

        public void SetMessage(string message)
        {
            currentMessage = message;
            if (this.IsHandleCreated)
                BeginInvoke(new MethodInvoker(UpdateText));
        }

    }
}
