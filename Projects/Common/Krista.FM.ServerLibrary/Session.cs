using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;

namespace Krista.FM.ServerLibrary
{
    /// <summary>
    /// ���� ���������� ������
    /// </summary>
    public enum SessionClientType
    {
        /// <summary>
        /// �������������� ��������
        /// </summary>
        [Description("�������������� ��������")]
        Undefined,
        /// <summary>
        /// ��������� ������
        /// </summary>
        [Description("������")]
        Server,

        /// <summary>
        /// ������ ������� ������
        /// </summary>
        [Description("������� ������")]
        DataPump,

        /// <summary>
        /// ������ ���-�������
        /// </summary>
        [Description("���-������")]
        WebService,

        /// <summary>
        /// Windows Net managed ������� (Workplace � ��.)
        /// </summary>
        [Description("������ .Net")]
        WindowsNetClient,

        /// <summary>
        /// ������ RIA-����������
        /// </summary>
        [Description("RIA-���������")]
        RIA,

        /// <summary>
        /// ������ ����� ���
        /// </summary>
        [Description("���� ���")]
        Dashboadrds
    }

    #region Server.Common

    /// <summary>
    /// ������
    /// </summary>
    public interface ISession : IDisposable
    {
        /// <summary>
        /// ID ������
        /// </summary>
        string SessionId { get; }

        /// <summary>
        /// IPrincipal ������������
        /// </summary>
        IPrincipal Principal { get; }

        /// <summary>
        /// ����� �����������
        /// </summary>
        DateTime LogonTime { get; }

        /// <summary>
        /// ��� ���������� ������
        /// </summary>
        SessionClientType ClientType { get; }

        /// <summary>
        /// ������������ �� �������� ������������
        /// </summary>
        string Application { get; }

        /// <summary>
        /// ��� ������ � ������� ������������ ���������� ����������
        /// </summary>
        string Host { get; }

        /// <summary>
        /// ������� ����������������� ������
        /// </summary>
        bool IsBlocked { get; set; }

        /// <summary>
        /// ���������� ���������� ��������
        /// </summary>
        int ResourcesCount { get; }

        /// <summary>
        /// ����������� ������� � ��������� ������
        /// </summary>
        /// <param name="obj">�������������� ������</param>
        void Register(IDisposable obj);

        /// <summary>
        /// �������� ������� �� ��������� ������
        /// </summary>
        /// <param name="obj">��������� ������</param>
        void Unregister(IDisposable obj);

        /// <summary>
        /// ������ � ������� ���������� ��������
        /// </summary>
        /// <param name="actionText">�������� ����������� ��������</param>
        void PostAction(string actionText);

        string ExecutedActions { get; }
    }

    /// <summary>
    /// �������� ������
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// ����������
        /// </summary>
        ISession this[string sessionId] { get; }

        /// <summary>
        /// ��������� ������
        /// </summary>
        IDictionary<string, ISession> Sessions { get; }

        void ClientSessionIsAlive(string sessionID);

        TimeSpan MaxClientResponseDelay { get; }
    }

    #endregion Server.Common

}