using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Modifications
{
    /// <summary>
    /// �������� � ������� ����������� ��������� ��������� �����.
    /// </summary>
    internal class ModificationContext : DisposableObject, IModificationContext
    {
        private int indentLevel;
        private int userOperationID;
        private Database database;
        private IUpdateSchemeProtocol logger;

        /// <summary>
        /// ������������� ����������.
        /// </summary>
        internal ModificationContext(Database database)
        {
            this.database = database;
        }

        /// <summary>
        /// ����������.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (database != null)
                    database.Dispose();
                
                if (logger != null)
                    logger.Dispose();
                
                database = null;
                logger = null;
            }
        }

        /// <summary>
        /// ������ ������� � ���� ������.
        /// </summary>
        internal Database Database
        {
            get { return database; }
        }

        /// <summary>
        /// ������� ����������� ��������.
        /// </summary>
        public int IndentLevel
        {
            [DebuggerStepThrough]
            get { return indentLevel; }
        }

        /// <summary>
        /// ������ ��� ������ ����� � ����.
        /// </summary>
        private IUpdateSchemeProtocol Logger
        {
            [DebuggerStepThrough]
            get
            {
                if (logger == null)
                {
                    logger = SchemeClass.Instance.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name) as IUpdateSchemeProtocol;
                }
                return logger;
            }
        }

        private void SendModificationMessage(ModificationMessageEventArgs args)
        {
            try
            {
                if (OnModificationMessage != null && args.Item is MajorObjectModificationItem) 
                {
                    if (args.Item is SchemeModificationItem || ((MajorObjectModificationItem)args.Item).FromObject is IPackage || ((MajorObjectModificationItem)args.Item).ToObject is IPackage)
                    {
                        OnModificationMessage(this, args);
                    }
                }
            }
            catch (Exception e)
            {
                OnModificationMessage = null;
                Trace.TraceError("������ ��� ������ �������� OnModificationMessage: {0}", e.Message);
            }
        }

        /// <summary>
        /// ��������� ������ �������� ����������� �������.
        /// </summary>
        /// <param name="item">�������������� ������.</param>
        internal void StartOperation(ModificationItem item)
        {
            SendModificationMessage(new ModificationMessageEventArgs(
                String.Format("������ ��������: {0}", item.Name),
                item,
                ModificationEventTypes.StartOperation,
                indentLevel));

            try
            {
                Logger.WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind.Information, item.Name, item.Name, item.Type, item.Key);
            }
            catch (Exception e)
            {
                Trace.TraceError("������ ������ � ��������: {0}", e.Message);
            }

            indentLevel++;
        }

        /// <summary>
        /// �������� ������� ��������� ���������.
        /// </summary>
        /// <param name="message">��������� ���������.</param>
        internal void SendMessage(string message)
        {
            SendModificationMessage(new ModificationMessageEventArgs(message));
        }

        /// <summary>
        /// ��������� ��������� ���������� �������� ����������� �������.
        /// </summary>
        /// <param name="item">�������������� ������.</param>
        /// <param name="isAppliedPartially">������� ���������-�������� �����������.</param>
        internal void EndOperation(ModificationItem item, bool isAppliedPartially)
        {
            indentLevel--;

            if (isAppliedPartially)
            {
                item.State = ModificationStates.AppliedPartially;
                SendModificationMessage(new ModificationMessageEventArgs(
                String.Format("�������� ��������� ��������: {0}", item.Name),
                item,
                ModificationEventTypes.EndOperation,
                indentLevel));
            }
            else
            {
                item.State = ModificationStates.Applied;
                SendModificationMessage(new ModificationMessageEventArgs(
                String.Format("����� ��������: {0}", item.Name),
                item,
                ModificationEventTypes.EndOperation,
                indentLevel));
            }

            

            try
            {
                Logger.WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind.EndUpdateSuccess, item.Name, item.Name, item.Type, item.Key);
            }
            catch (Exception e)
            {
                Trace.TraceError("������ ������ � ��������: {0}", e.Message);
            }
        }

        /// <summary>
        /// ��������� ���������� ���������� �������� ����������� �������.
        /// </summary>
        /// <param name="item">�������������� ������.</param>
        /// <param name="e"></param>
        internal void EndOperation(ModificationItem item, Exception e)
        {
            indentLevel--;

            item.State = ModificationStates.AppliedWithErrors;

            SendModificationMessage(new ModificationMessageEventArgs(
                String.Format("������ ���������� ��������: {0}{1}���������: {2}{1}���������� �����: {3}",
                    item.Name, Environment.NewLine, e.Message, e.StackTrace),
                item, ModificationEventTypes.EndOperation, indentLevel));

            try
            {
                Logger.WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind.EndUpdateWithError, item.Name, item.Name, item.Type,
                    String.Format("������ ���������� ��������: {0}{1}���������: {2}{1}���������� �����: {3}",
                        item.Name,
                        Environment.NewLine,
                        e.Message,
                        e.StackTrace));
            }
            catch (Exception exp)
            {
                Trace.TraceError("������ ������ � ��������: {0}", exp.Message);
            }
        }

        /// <summary>
        /// ������������� � ������ �������� ����������.
        /// </summary>
        /// <returns>ID ������ ��������� "�������� ������������".</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int BeginUpdate()
        {
            // ������������� ������� ���������� �� �������, ��������� �� �� ������������ ��������� �������������.
            // ��� ������������� Mutex ReleaseMutex ������ ���������� �� �� ���� �� thread ��� � ����� WaitOne,
            // ��� ��� ����� �� �����������, ��� ��������� remoting ������ ����� �������� � ��� �� thread.
            // � ���������� ���� ��������� ������  "Object synchronization method was called from an unsynchronized block of code"
            SchemeClass.SemaphoreSchemeAutoUpdate.WaitOne();
            Logger.WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind.BeginUpdate, SchemeClass.Instance.Name, SchemeClass.Instance.Name, ModificationTypes.Modify, "������ �������� ����������");
            userOperationID = Logger.UserOperationID;
            return userOperationID;
        }

        /// <summary>
        /// ������������� � ���������� �������� ����������.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void EndUpdate()
        {
            // ����� ��������� ���������� �������� � ������� �������
            ((DataPumpManagement.DataPumpInfo)SchemeClass.Instance.DataPumpManager.DataPumpInfo).ResetCache();
            ((GlobalConsts.GlobalConstsManager)SchemeClass.Instance.GlobalConstsManager).ResetCache();

            SchemeClass.Instance.RunTestDatabase();
            SchemeClass.SemaphoreSchemeAutoUpdate.Release();
            Logger.WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind.Information, SchemeClass.Instance.Name, SchemeClass.Instance.Name, ModificationTypes.Modify, "�������� ���������� ���������");
        }

        public event ModificationMessageEventHandler OnModificationMessage;
    }
}
