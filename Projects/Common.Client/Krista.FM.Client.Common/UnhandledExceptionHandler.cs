using System;
using System.Runtime.Remoting;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.IO;

using System.Text;

using Krista.FM.Common;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common
{
    #region Класс для обработки необработанных исключений
    public static class UnhandledExceptionHandler
    {
        // название папки с логами
        private const string CRASH_LOGS_FOLDER_NAME = "CrashLogs";
        // шаблон имени лог-файла
        private const string CRASH_FILE_NAME_TEMPLATE = "{0}_{1}_CrashLog.txt";

        /// <summary>
        /// Создать лог-файл с информацией об исключении
        /// </summary>
        /// <param name="e">Объект Exception</param>
        /// <returns>Названия созданного лог-файла</returns>
        private static string CreateCrashLog(Exception e)
        {
            string crashFolder = AppDomain.CurrentDomain.BaseDirectory + CRASH_LOGS_FOLDER_NAME;
            string crashDate = DateTime.Now.ToString("dd/MM/yyyy_HH.mm.ss");
            string appName = Assembly.GetExecutingAssembly().ManifestModule.Name;
            string crashFileName = crashFolder + "\\" + String.Format(CRASH_FILE_NAME_TEMPLATE, crashDate, appName);
            string res = String.Format("Ошибка при формировании файла '{0}'", crashFileName);
            try
            {
                if (!Directory.Exists(crashFolder))
                    Directory.CreateDirectory(crashFolder);
                File.WriteAllText(crashFileName, ExceptionHelper.DumpException(e), Encoding.GetEncoding(1251));
                res = crashFileName;
            }
            catch 
            {
            }
            return res;
        }

        private static IScheme _scheme = null;
        /// <summary>
        /// Указатель на схему. Если не задан - запись в лог действий пользователей не ведется
        /// </summary>
        public static IScheme Scheme
        {
            get { return _scheme; }
            set { _scheme = value; }
        }

        // Обработчик исключений нити
        public static void OnThreadException(object sender, ThreadExceptionEventArgs t)
        {
            #region запись в лог
            if (_scheme != null)
            {
                try
                {
                    // получаем имя сборки в которой произошло исключение
                    string callerName = t.Exception.Source;
                    // получаем инетерфейс протокола действий пользователей
                    using (IUsersOperationProtocol protocol = (IUsersOperationProtocol)_scheme.GetProtocol(callerName))
                    {
                        // записываем в протокол действий пользователей сообщение о необработанном исключении
                        //string moduleName = callerMethod.Module.Name;
                        string hostName = System.Windows.Forms.SystemInformation.ComputerName;
						string errorMessage = t.Exception.Message + Environment.NewLine + Krista.Diagnostics.KristaDiagnostics.ExpandException(t.Exception);
                        protocol.WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeUntilledExceptionsEvent, errorMessage, hostName);

                    }
                }
                catch
                {
                }
            }
            #endregion

            string errStr = String.Empty;
            // если произошла необработанная ошибка доступа - показываем специальную форму
            if (t.Exception is PermissionException)
            {
                FormPermissionException.ShowErrorForm((PermissionException)t.Exception);
                return;
            }

            // в случае отваливания сервера надо сразу все закрывать иначе повиснет наглухо
            if ((t.Exception is RemotingException) || (t.Exception is SocketException))
            {
                string fileName = CreateCrashLog(t.Exception);
                errStr =
                    "Произошла критическая ошибка при обращении к объекту сервера." + Environment.NewLine +
                    "Приложение будет закрыто." + Environment.NewLine +
                    "Лог-файл с подробной иформацией об исключении сохранен по адресу:" + Environment.NewLine + Environment.NewLine +
                    fileName;

                MessageBox.Show(errStr, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.ExitThread();
                return;
            }

            if (t.Exception is System.IO.IOException)
            {
                errStr = "Приложение не может получить доступ к файлу. Возможно он используется другим процессом.";
                MessageBox.Show(errStr, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (t.Exception is System.Exception && t.Exception.Message == "Сессия заблокирована.")
            {
                errStr = "Текущая сессия заблокирована." + Environment.NewLine
                    + "Для разблокирования сессии обратитесь к администратору системы";
                MessageBox.Show(errStr, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ErrorFormResult result = ErrorFormResult.efrContinue;
            try
            {
                result = ShowThreadExceptionDialog(t.Exception);
            }
            catch (Exception e)
            {
                try
                {
                    MessageBox.Show(
                        String.Format("Необработанное программное исключение: {0}", e.Message), 
                        "Необработанное программное исключение",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Application.Exit();
                }
            }
            switch (result)
            {
                case ErrorFormResult.efrClose:
                    Application.ExitThread();
                    break;
                case ErrorFormResult.efrContinue:
                    break;
                case ErrorFormResult.efrRestart:
                    Application.Restart();
                    break;
            }
        }

        // Диалог с ообщением об ошибке
        private static ErrorFormResult ShowThreadExceptionDialog(Exception e)
        {
            return FormException.ShowErrorForm(e);
        }
    }
    #endregion

}