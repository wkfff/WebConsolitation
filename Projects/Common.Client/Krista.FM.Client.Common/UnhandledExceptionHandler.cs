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
    #region ����� ��� ��������� �������������� ����������
    public static class UnhandledExceptionHandler
    {
        // �������� ����� � ������
        private const string CRASH_LOGS_FOLDER_NAME = "CrashLogs";
        // ������ ����� ���-�����
        private const string CRASH_FILE_NAME_TEMPLATE = "{0}_{1}_CrashLog.txt";

        /// <summary>
        /// ������� ���-���� � ����������� �� ����������
        /// </summary>
        /// <param name="e">������ Exception</param>
        /// <returns>�������� ���������� ���-�����</returns>
        private static string CreateCrashLog(Exception e)
        {
            string crashFolder = AppDomain.CurrentDomain.BaseDirectory + CRASH_LOGS_FOLDER_NAME;
            string crashDate = DateTime.Now.ToString("dd/MM/yyyy_HH.mm.ss");
            string appName = Assembly.GetExecutingAssembly().ManifestModule.Name;
            string crashFileName = crashFolder + "\\" + String.Format(CRASH_FILE_NAME_TEMPLATE, crashDate, appName);
            string res = String.Format("������ ��� ������������ ����� '{0}'", crashFileName);
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
        /// ��������� �� �����. ���� �� ����� - ������ � ��� �������� ������������� �� �������
        /// </summary>
        public static IScheme Scheme
        {
            get { return _scheme; }
            set { _scheme = value; }
        }

        // ���������� ���������� ����
        public static void OnThreadException(object sender, ThreadExceptionEventArgs t)
        {
            #region ������ � ���
            if (_scheme != null)
            {
                try
                {
                    // �������� ��� ������ � ������� ��������� ����������
                    string callerName = t.Exception.Source;
                    // �������� ���������� ��������� �������� �������������
                    using (IUsersOperationProtocol protocol = (IUsersOperationProtocol)_scheme.GetProtocol(callerName))
                    {
                        // ���������� � �������� �������� ������������� ��������� � �������������� ����������
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
            // ���� ��������� �������������� ������ ������� - ���������� ����������� �����
            if (t.Exception is PermissionException)
            {
                FormPermissionException.ShowErrorForm((PermissionException)t.Exception);
                return;
            }

            // � ������ ����������� ������� ���� ����� ��� ��������� ����� �������� �������
            if ((t.Exception is RemotingException) || (t.Exception is SocketException))
            {
                string fileName = CreateCrashLog(t.Exception);
                errStr =
                    "��������� ����������� ������ ��� ��������� � ������� �������." + Environment.NewLine +
                    "���������� ����� �������." + Environment.NewLine +
                    "���-���� � ��������� ���������� �� ���������� �������� �� ������:" + Environment.NewLine + Environment.NewLine +
                    fileName;

                MessageBox.Show(errStr, "����������� ������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.ExitThread();
                return;
            }

            if (t.Exception is System.IO.IOException)
            {
                errStr = "���������� �� ����� �������� ������ � �����. �������� �� ������������ ������ ���������.";
                MessageBox.Show(errStr, "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (t.Exception is System.Exception && t.Exception.Message == "������ �������������.")
            {
                errStr = "������� ������ �������������." + Environment.NewLine
                    + "��� ��������������� ������ ���������� � �������������� �������";
                MessageBox.Show(errStr, "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        String.Format("�������������� ����������� ����������: {0}", e.Message), 
                        "�������������� ����������� ����������",
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

        // ������ � ��������� �� ������
        private static ErrorFormResult ShowThreadExceptionDialog(Exception e)
        {
            return FormException.ShowErrorForm(e);
        }
    }
    #endregion

}