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
    /// Контекст в котором выполняются изменения структуры схемы.
    /// </summary>
    internal class ModificationContext : DisposableObject, IModificationContext
    {
        private int indentLevel;
        private int userOperationID;
        private Database database;
        private IUpdateSchemeProtocol logger;

        /// <summary>
        /// Инициализация экземпляра.
        /// </summary>
        internal ModificationContext(Database database)
        {
            this.database = database;
        }

        /// <summary>
        /// Деструктор.
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
        /// Объект доступа к базе данных.
        /// </summary>
        internal Database Database
        {
            get { return database; }
        }

        /// <summary>
        /// Уровень вложенности операции.
        /// </summary>
        public int IndentLevel
        {
            [DebuggerStepThrough]
            get { return indentLevel; }
        }

        /// <summary>
        /// Объект для записи логов в базу.
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
                Trace.TraceError("Ошибка при вызове делегата OnModificationMessage: {0}", e.Message);
            }
        }

        /// <summary>
        /// Индикация начала операции модификации объекта.
        /// </summary>
        /// <param name="item">Модифицируемый объект.</param>
        internal void StartOperation(ModificationItem item)
        {
            SendModificationMessage(new ModificationMessageEventArgs(
                String.Format("Начало операции: {0}", item.Name),
                item,
                ModificationEventTypes.StartOperation,
                indentLevel));

            try
            {
                Logger.WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind.Information, item.Name, item.Name, item.Type, item.Key);
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка записи в протокол: {0}", e.Message);
            }

            indentLevel++;
        }

        /// <summary>
        /// Посылает клиенту текстовое сообщение.
        /// </summary>
        /// <param name="message">Текстовое сообщение.</param>
        internal void SendMessage(string message)
        {
            SendModificationMessage(new ModificationMessageEventArgs(message));
        }

        /// <summary>
        /// Индикация успешного завершения операции модификации объекта.
        /// </summary>
        /// <param name="item">Модифицируемый объект.</param>
        /// <param name="isAppliedPartially">Признак частичной-успешной модификации.</param>
        internal void EndOperation(ModificationItem item, bool isAppliedPartially)
        {
            indentLevel--;

            if (isAppliedPartially)
            {
                item.State = ModificationStates.AppliedPartially;
                SendModificationMessage(new ModificationMessageEventArgs(
                String.Format("Операция выполнена частично: {0}", item.Name),
                item,
                ModificationEventTypes.EndOperation,
                indentLevel));
            }
            else
            {
                item.State = ModificationStates.Applied;
                SendModificationMessage(new ModificationMessageEventArgs(
                String.Format("Конец операции: {0}", item.Name),
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
                Trace.TraceError("Ошибка записи в протокол: {0}", e.Message);
            }
        }

        /// <summary>
        /// Индикация ошибочного завершения операции модификации объекта.
        /// </summary>
        /// <param name="item">Модифицируемый объект.</param>
        /// <param name="e"></param>
        internal void EndOperation(ModificationItem item, Exception e)
        {
            indentLevel--;

            item.State = ModificationStates.AppliedWithErrors;

            SendModificationMessage(new ModificationMessageEventArgs(
                String.Format("Ошибка выполнения операции: {0}{1}Сообщение: {2}{1}Содержимое стека: {3}",
                    item.Name, Environment.NewLine, e.Message, e.StackTrace),
                item, ModificationEventTypes.EndOperation, indentLevel));

            try
            {
                Logger.WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind.EndUpdateWithError, item.Name, item.Name, item.Type,
                    String.Format("Ошибка выполнения операции: {0}{1}Сообщение: {2}{1}Содержимое стека: {3}",
                        item.Name,
                        Environment.NewLine,
                        e.Message,
                        e.StackTrace));
            }
            catch (Exception exp)
            {
                Trace.TraceError("Ошибка записи в протокол: {0}", exp.Message);
            }
        }

        /// <summary>
        /// Сигнализирует о начале процесса обновления.
        /// </summary>
        /// <returns>ID записи протокола "Операции пользователя".</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int BeginUpdate()
        {
            // Синхронизация потоков переделана на семафор, поскольку он не обеспечивает потоковой идентификации.
            // При использовании Mutex ReleaseMutex иногда вызывается не из того же thread что и вызов WaitOne,
            // так как никто не гарантирует, что отдельные remoting вызовы будут работать в том же thread.
            // В результате чего возникала ошибка  "Object synchronization method was called from an unsynchronized block of code"
            SchemeClass.SemaphoreSchemeAutoUpdate.WaitOne();
            Logger.WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind.BeginUpdate, SchemeClass.Instance.Name, SchemeClass.Instance.Name, ModificationTypes.Modify, "Начало операции обновления");
            userOperationID = Logger.UserOperationID;
            return userOperationID;
        }

        /// <summary>
        /// Сигнализирует о завершении процесса обновления.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void EndUpdate()
        {
            // Сброс коллекций глобальных констант и реестра закачек
            ((DataPumpManagement.DataPumpInfo)SchemeClass.Instance.DataPumpManager.DataPumpInfo).ResetCache();
            ((GlobalConsts.GlobalConstsManager)SchemeClass.Instance.GlobalConstsManager).ResetCache();

            SchemeClass.Instance.RunTestDatabase();
            SchemeClass.SemaphoreSchemeAutoUpdate.Release();
            Logger.WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind.Information, SchemeClass.Instance.Name, SchemeClass.Instance.Name, ModificationTypes.Modify, "Операция обновления завершена");
        }

        public event ModificationMessageEventHandler OnModificationMessage;
    }
}
