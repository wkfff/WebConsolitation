using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Krista.FM.Client.SchemeEditor.Gui;

namespace Krista.FM.Client.SchemeEditor.Services
{
    /// <summary>
    /// Набор сервисных функций для работы с панелью статуса
    /// </summary>
    public class StatusBarService
    {
        private static StatusBar statusBar = null;

        /// <summary>
        /// Конструктор типа
        /// </summary>
        static StatusBarService()
        {
            statusBar = new StatusBar();
        }

        /// <summary>
        /// Элемент управления панели статуса
        /// </summary>
        public static System.Windows.Forms.Control Control
        {
            get
            {
                System.Diagnostics.Debug.Assert(statusBar != null);
                return statusBar;
            }
        }

        /// <summary>
        /// Установка информации о схеме
        /// </summary>
        /// <param name="server">Имя машины</param>
        /// <param name="port">Порт</param>
        /// <param name="schemeName">Имя схемы</param>
        public static void SetSchemeInfo(string server, string port, string schemeName, string userName)
        {
            statusBar.ServerInfoPanel.Text = String.Format("Сервер: {0}:{1}", server, port);
            statusBar.SchemeInfoPanel.Text = String.Format("Схема: {0}", schemeName);
            statusBar.UserInfoPanel.Text = String.Format("Пользователь: {0}", userName);
        }

        static string lastMessage = "";

        public static void SetMessage(string message)
        {
            System.Diagnostics.Debug.Assert(statusBar != null);
            lastMessage = message;
            statusBar.SetMessage(message);
        }
    }
}
