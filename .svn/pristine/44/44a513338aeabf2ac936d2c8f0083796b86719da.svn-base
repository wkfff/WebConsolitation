using System;
using System.Data;
using System.Reflection;
using System.Runtime.Hosting;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
//test

namespace Krista.FM.Server.Logger
{
    /// <summary>
    /// Общий класс для объектов протоколирования, реализует все необходимые интерфейсы 
    /// </summary>
    public class Logger : DisposableObject, IBaseProtocol, IDataPumpProtocol, IBridgeOperationsProtocol,
        IMDProcessingProtocol, IUsersOperationProtocol, ISystemEventsProtocol, IReviseDataProtocol,
        IProcessDataProtocol, IDeleteDataProtocol, IPreviewDataProtocol, IClassifiersProtocol,
        IUpdateSchemeProtocol, IDataSourceProtocol, ITransferDBToNewYearProtocol,
        IMessageExchangeProtocol
    {
        // ИД операции пользователя, инициализируется при первой операции записи события
        private int userOperationID = -1;

        // Тип модуля, ведущего протоколирование
        //private ModulesTypes CallerModuleType;

        // Объект для работы с базой, инициализируется в конструкторе
        //private IDatabase DBObj;

        private string CallerAssemblyName = string.Empty;

        private IScheme _scheme = null;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="DataBaseItf">Объект для работы с БД</param>
        public Logger(/*IDatabase DataBaseItf*/IScheme scheme, string assemblyName)
        {
            // Если не задан объект для работы с БД - генерируем исключение
            //if (DataBaseItf == null) throw new InvalidOperationException("Не задан интерфейс для работы с базой данных");
            if (scheme == null)
                throw new Exception("Не задан интерфейс схемы");
            CallerAssemblyName = assemblyName;
            //DBObj = DataBaseItf;
            _scheme = scheme;
        }

        protected override void Dispose(bool disposing)
        {
            /*if (disposing)
            {
                lock (this)
                {
                    if (DBObj != null) DBObj.Dispose();
                    #region UNUSED
                    try
                    {
                        // Если были операции записи в лог (пользователь работал с объектом системы)
                        if (UserOperationID != -1)
                        {
                            UsersOperationEventKind UserEvent = UsersOperationEventKind.uoeStopWorkingWithModule_UsersOperations;
                            switch (CallerModuleType)
                            {
                                case ModulesTypes.DataPumpModule:
                                    UserEvent = UsersOperationEventKind.uoeStopWorkingWithModule_DataPump;
                                    break;
                                case ModulesTypes.BridgeOperationsModule:
                                    UserEvent = UsersOperationEventKind.uoeStopWorkingWithModule_BridgeOperations;
                                    break;
                                case ModulesTypes.MDProcessingModule:
                                    UserEvent = UsersOperationEventKind.uoeStopWorkingWithModule_MDProcessing;
                                    break;
                                case ModulesTypes.ReviseDataModule:
                                    UserEvent = UsersOperationEventKind.uoeStopWorkingWithModule_ReviseData;
                                    break;
                                case ModulesTypes.SystemEventsModule:
                                    UserEvent = UsersOperationEventKind.uoeStopWorkingWithModule_SystemEvents;
                                    break;
                                case ModulesTypes.UsersOperationsModule:
                                    UserEvent = UsersOperationEventKind.uoeStopWorkingWithModule_UsersOperations;
                                    break;
                                case ModulesTypes.ProcessDataModule:
                                    UserEvent = UsersOperationEventKind.uoeStopWorkingWithModule_ProcessData;
                                    break;
                                case ModulesTypes.DeleteDataModule:
                                    UserEvent = UsersOperationEventKind.uoeStopWorkingWithModule_DeleteData;
                                    break;
                            };

                            // Пишем событие о завершении работы с объектом системы
                            WriteEventIntoUsersOperationProtocol(CallerAssemblyName, UserEvent, "SYSTEM", "UNKNOWN", "STOP", "STOP");
                        }
                    }
                    finally
                    {
                    //}
                    #endregion
                }
            }*/
            base.Dispose(disposing);
        }

        /// <summary>
        /// ИД операции пользователя, инициализируется при первой операции записи события.
        /// </summary>
        public int UserOperationID
        {
            //TODO [DebuggerStepThrough()]
            get { return userOperationID; }
        }

        #region UNUSED
        /// <summary>
        /// Проверка начала работы пользователя с объектом системы (вызывается из функций записи в лог)
        /// </summary>
        /*protected void CheckUserOperation(ModulesTypes mt)
        {
            // Если это первая операция записи
            if (UserOperationID == -1)
            {
                // генерируем ИД новой записи
                UserOperationID = (int)DBObj.GetGenerator("g_HUB_EventProtocol");
                //
                CallerModuleType = mt;
                UsersOperationEventKind UserEvent = UsersOperationEventKind.uoeStartWorkingWithModule_UsersOperations;
                switch (mt)
                {
                    case ModulesTypes.DataPumpModule:
                        UserEvent = UsersOperationEventKind.uoeStartWorkingWithModule_DataPump;
                        break;
                    case ModulesTypes.BridgeOperationsModule:
                        UserEvent = UsersOperationEventKind.uoeStartWorkingWithModule_BridgeOperations;
                        break;
                    case ModulesTypes.MDProcessingModule:
                        UserEvent = UsersOperationEventKind.uoeStartWorkingWithModule_MDProcessing;
                        break;
                    case ModulesTypes.ReviseDataModule:
                        UserEvent = UsersOperationEventKind.uoeStartWorkingWithModule_ReviseData;
                        break;
                    case ModulesTypes.SystemEventsModule:
                        UserEvent = UsersOperationEventKind.uoeStartWorkingWithModule_SystemEvents;
                        break;
                    case ModulesTypes.UsersOperationsModule:
                        UserEvent = UsersOperationEventKind.uoeStartWorkingWithModule_UsersOperations;
                        break;
                    case ModulesTypes.ProcessDataModule:
                        UserEvent = UsersOperationEventKind.uoeStartWorkingWithModule_ProcessData;
                        break;
                    case ModulesTypes.DeleteDataModule:
                        UserEvent = UsersOperationEventKind.uoeStartWorkingWithModule_DeleteData;
                        break;
                };
                // Пишем событие про то что пользовтаель начал работать с объектом системы
                WriteEventIntoUsersOperationProtocol(UserOperationID, CallerAssemblyName, UserEvent, "не реализовано", "не реализовано", "старт", "старт");
            }
        }*/
        #endregion

        #region Константы для проверки корректности вводимых значений
        private const int MAX_INFOMSG_LENGTH = 4000;
        private const int MAX_BRIDGEROLE_LENGTH = 255;
        private const int MAX_MODULENAME_LENGTH = 255;
        private const int MAX_MDOBJECTNAME_LENGTH = 255;
        private const int MAX_OBJECTNAME_LENGTH = 255;
        private const int MAX_USERNAME_LENGTH = 255;
        private const int MAX_ACTIONNAME_LENGTH = 255;

        private static string CheckStrLen(string msg, int maxLength)
        {
            if (msg.Length > maxLength)
                return msg.Substring(0, maxLength - 3).PadRight(maxLength, '.');
            else
                return msg;
        }
        #endregion

        public void WriteEventIntoUsersOperationProtocol(UsersOperationEventKind eventKind, string infoMsg, params string[] UserHost)
        {
            IDatabase db = null;
            try
            {
                string host = string.Empty;
                if (UserHost.Length != 0)
                    host = UserHost[0];
                db = _scheme.SchemeDWH.DB;
                string userName = Authentication.UserName;
                if (String.IsNullOrEmpty(userName))
                    userName = WindowsIdentity.GetCurrent().Name; 
                //create or replace view UsersOperations
                // ID, EventDateTime, Module, KindsOfEvents, InfoMessage,
                // UserName, ObjectName, ActionName
                userOperationID = (int)db.GetGenerator("g_HUB_EventProtocol");
                db.ExecQuery(
                    "insert  into UsersOperations (ID, Module, KindsOfEvents, InfoMessage, UserName, ObjectName, ActionName, UserHost, SessionID) values (?, ?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", userOperationID),
                    db.CreateParameter("Module", (string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (int)eventKind),
                    db.CreateParameter("InfoMessage", CheckStrLen(infoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("UserName", CheckStrLen(userName, MAX_USERNAME_LENGTH)),
                    db.CreateParameter("ObjectName", "..."),
                    db.CreateParameter("ActionName", "..."),
                    db.CreateParameter("UserHost", host),
                    db.CreateParameter("SessionID", SessionContext.SessionId)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Записать событие в протокол действий пользователя
        /// </summary>
        /// <param name="ID">ИД новой записи</param>
        /// <param name="EvenKind">Тип события</param>
        /// <param name="UserName">Имя пользователя</param>
        /// <param name="ObjectName">Название объекта системы</param>
        /// <param name="ActionName">Действие</param>
        /// <param name="InfoMsg">Информационное сообщение</param>
        [Obsolete]
        public void WriteEventIntoUsersOperationProtocol(object ID, string Module, UsersOperationEventKind EventKind, string UserName, string ObjectName,
            string ActionName, string InfoMsg)
        {
            /*    string userName = Authentication.User;
                if (String.IsNullOrEmpty(userName))
                    userName = WindowsIdentity.GetCurrent().Name;
                //create or replace view UsersOperations
                // ID, EventDateTime, Module, KindsOfEvents, InfoMessage,
                // UserName, ObjectName, ActionName
                DBObj.ExecQuery(
                    "insert  into UsersOperations (ID, Module, KindsOfEvents, InfoMessage, UserName, ObjectName, ActionName) values (?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    DBObj.CreateParameter("ID", ID),
                    DBObj.CreateParameter("Module", (string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    DBObj.CreateParameter("KindsOfEvents", (int)EventKind),
                    DBObj.CreateParameter("InfoMessage", CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    DBObj.CreateParameter("UserName", CheckStrLen(userName, MAX_USERNAME_LENGTH)),
                    DBObj.CreateParameter("ObjectName", "..."),
                    DBObj.CreateParameter("ActionName", "...")
                );*/
        }

        /// <summary>
        /// Записать событие в протокол действий пользователя
        /// </summary>
        /// <param name="EvenKind">Тип события</param>
        /// <param name="UserName">Имя пользователя</param>
        /// <param name="ObjectName">Название объекта системы</param>
        /// <param name="ActionName">Действие</param>
        /// <param name="InfoMsg">Информационное сообщение</param>
        [Obsolete]
        public void WriteEventIntoUsersOperationProtocol(string Module, UsersOperationEventKind EvenKind, string UserName, string ObjectName,
            string ActionName, string InfoMsg)
        {
            /*WriteEventIntoUsersOperationProtocol((int)DBObj.GetGenerator("g_HUB_EventProtocol"), Module, EvenKind,
                UserName, ObjectName, ActionName, InfoMsg);*/
        }


        /// <summary>
        /// Записать событие в прокол закачки данных
        /// </summary>
        /// <param name="EventKind">Тип события</param>
        /// <param name="PumpHistoryID">ИД операции закачки</param>
        /// <param name="DataSourceID">ИД источника данных</param>
        /// <param name="InfoMsg">Информационное сообщение</param>
        public void WriteEventIntoDataPumpProtocol(DataPumpEventKind EventKind, int PumpHistoryID, int DataSourceID, string InfoMsg)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                //CheckUserOperation(ModulesTypes.DataPumpModule);
                //create or replace view DataPumpProtocol
                //	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations,
                //	PumpHistoryID, DataSourceID
                db.ExecQuery(
                    "insert  into DataPumpProtocol (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, PumpHistoryID, DataSourceID, SessionID) values (?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", (object)(string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)EventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("PumpHistoryID", (object)PumpHistoryID),
                    db.CreateParameter("DataSourceID", (object)DataSourceID),
                    db.CreateParameter("SessionID", SessionContext.SessionId)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Записать событие в протокол сопоставления классификаторов
        /// </summary>
        /// <param name="EventKind">Тип события</param>
        /// <param name="BridgeRoleA">Сопоставляемый классификатор</param>
        /// <param name="BridgeRoleB">Сопоставимый классификатор</param>
        /// <param name="InfoMsg">Информационное сообщение</param>
        public void WriteEventIntoBridgeOperationsProtocol(BridgeOperationsEventKind EventKind, string BridgeRoleA, string BridgeRoleB, string InfoMsg, int PumpHistoryID, int DataSourceID)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                //CheckUserOperation(ModulesTypes.BridgeOperationsModule);
                //create or replace view BridgeOperations
                //	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations,
                //	BridgeRoleA, BridgeRoleB
                db.ExecQuery(
                    "insert  into BridgeOperations (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID, SessionID) values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", (object)(string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)EventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("BridgeRoleA", (object)CheckStrLen(BridgeRoleA, MAX_BRIDGEROLE_LENGTH)),
                    db.CreateParameter("BridgeRoleB", (object)CheckStrLen(BridgeRoleB, MAX_BRIDGEROLE_LENGTH)),
                    db.CreateParameter("PumpHistoryID", (object)PumpHistoryID),
                    db.CreateParameter("DataSourceID", (object)DataSourceID),
                    db.CreateParameter("SessionID", SessionContext.SessionId)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Записать событие в протокол обработки многомерных хранилищ
        /// </summary>		
        public void WriteEventIntoMDProcessingProtocol(MDProcessingEventKind EventKind, string InfoMsg, string MDObjectName)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                //CheckUserOperation(ModulesTypes.MDProcessingModule);
                //create or replace view MDProcessing
                //	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations,
                //	MDObjectName
                db.ExecQuery(
                    "insert  into MDProcessing (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, MDObjectName, SessionID) values (?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", (object)(string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)EventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("MDObjectName", (object)CheckStrLen(MDObjectName, MAX_MDOBJECTNAME_LENGTH)),
                    db.CreateParameter("SessionID", SessionContext.SessionId)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        public void WriteEventIntoMDProcessingProtocol(string moduleName, MDProcessingEventKind EventKind, string InfoMsg, string MDObjectName,
            string ObjectIdentifier, Krista.FM.Server.ProcessorLibrary.OlapObjectType OlapObjectType, string batchId)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                //db.ExecQuery(
                //    "insert  into MDProcessing (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, MDObjectName, PumpHistoryID, DataSourceID, SessionID, ObjectIdentifier, InvalidateReason, OlapObjectType) values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                //    QueryResultTypes.NonQuery,
                //    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                //    db.CreateParameter("Module", (object)(string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                //    db.CreateParameter("KindsOfEvents", (object)(int)EventKind),
                //    db.CreateParameter("InfoMessage", (object)CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                //    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                //    db.CreateParameter("MDObjectName", (object)CheckStrLen(MDObjectName, MAX_MDOBJECTNAME_LENGTH)),
                //    db.CreateParameter("PumpHistoryID", (object)PumpHistoryID),
                //    db.CreateParameter("DataSourceID", (object)DataSourceID),
                //    db.CreateParameter("DataSourceID", (object)ObjectIdentifier),
                //    db.CreateParameter("DataSourceID", (object)(int)InvalidateReason),
                //    db.CreateParameter("DataSourceID", (object)(int)OlapObjectType)
                //);

                db.ExecQuery(
                    "insert  into MDProcessing (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, MDObjectName, SessionID, ObjectIdentifier, OlapObjectType, BatchId) values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", moduleName),
                    db.CreateParameter("KindsOfEvents", (int)EventKind),
                    db.CreateParameter("InfoMessage", CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", userOperationID),
                    db.CreateParameter("MDObjectName", CheckStrLen(MDObjectName, MAX_MDOBJECTNAME_LENGTH)),
                    db.CreateParameter("SessionID", SessionContext.SessionId),
                    db.CreateParameter("ObjectIdentifier", ObjectIdentifier),
                    db.CreateParameter("OlapObjectType", (int)OlapObjectType),
                    db.CreateParameter("BatchId", batchId)
                );

            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Записать событие в протокол системных событий
        /// </summary>
        public void WriteEventIntoSystemEventsProtocol(SystemEventKind EventKind,
            string InfoMsg, string ObjectName)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                //CheckUserOperation(ModulesTypes.SystemEventsModule);
                // create or replace view SystemEvents
                //	ID, EventDateTime, Module, KindsOfEvents, InfoMessage,
                //	ObjectName
                db.ExecQuery(
                    "insert into SystemEvents (Module, KindsOfEvents, InfoMessage, ObjectName, SessionID) values (?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("Module", (object)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)EventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("ObjectName", (object)CheckStrLen(ObjectName, MAX_OBJECTNAME_LENGTH)),
                    db.CreateParameter("SessionID", SessionContext.SessionId)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Записать событие в протокол сверки данных
        /// </summary>
        /// <param name="EventKind">Вид события</param>
        /// <param name="InfoMsg">Информационное сообщение</param>
        public void WriteEventIntoReviseDataProtocol(ReviseDataEventKind EventKind, string InfoMsg, int PumpHistoryID, int DataSourceID)
        {

            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                //CheckUserOperation(ModulesTypes.ReviseDataModule);
                //create or replace view ReviseDataProtocol
                //	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations
                db.ExecQuery(
                    "insert  into ReviseDataProtocol (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, PumpHistoryID, DataSourceID, SessionID) values (?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", (object)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)EventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("PumpHistoryID", (object)PumpHistoryID),
                    db.CreateParameter("DataSourceID", (object)DataSourceID),
                    db.CreateParameter("SessionID", SessionContext.SessionId)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Записать событие в протокол обработки данных
        /// </summary>
        /// <param name="EventKind">Вид событий</param>
        /// <param name="InfoMsg">Иформационное сообщение</param>
        /// <param name="PumpHistoryID">ИД закачки</param>
        /// <param name="DataSourceID">ИД источника данных</param>
        public void WriteEventIntoProcessDataProtocol(ProcessDataEventKind EventKind, string InfoMsg, int PumpHistoryID, int DataSourceID)
        {

            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                //CheckUserOperation(ModulesTypes.ProcessDataModule);
                //create or replace view ProcessDataProtocol
                //	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations,
                //	PumpHistoryID, DataSourceID
                db.ExecQuery(
                    "insert into ProcessDataProtocol (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, PumpHistoryID, DataSourceID, SessionID) values (?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", (object)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)EventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("PumpHistoryID", (object)PumpHistoryID),
                    db.CreateParameter("DataSourceID", (object)DataSourceID),
                    db.CreateParameter("SessionID", SessionContext.SessionId)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Записать событие в протокол удаления данных
        /// </summary>
        /// <param name="EventKind">Тип события</param>
        /// <param name="InfoMsg">Информационное сообщение</param>
        /// <param name="PumpHistoryID">ИД закачки</param>
        /// <param name="DataSourceID">ИД источника данных</param>	
        public void WriteEventIntoDeleteDataProtocol(DeleteDataEventKind EventKind, string InfoMsg, int PumpHistoryID, int DataSourceID)
        {

            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                //CheckUserOperation(ModulesTypes.DeleteDataModule);
                //create or replace view DeleteDataProtocol
                //	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations,
                //	PumpHistoryID, DataSourceID
                db.ExecQuery(
                    "insert into DeleteDataProtocol (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, PumpHistoryID, DataSourceID, SessionID) values (?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", (object)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)EventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("PumpHistoryID", (object)PumpHistoryID),
                    db.CreateParameter("DataSourceID", (object)DataSourceID),
                    db.CreateParameter("SessionID", SessionContext.SessionId)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Записать событие в прокол закачки данных
        /// </summary>
        /// <param name="EventKind">Тип события</param>
        /// <param name="PumpHistoryID">ИД операции закачки</param>
        /// <param name="DataSourceID">ИД источника данных</param>
        /// <param name="InfoMsg">Информационное сообщение</param>
        public void WriteEventIntoPreviewDataProtocol(PreviewDataEventKind EventKind, int PumpHistoryID, int DataSourceID, string InfoMsg)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;

                db.ExecQuery(
                    "insert into PreviewDataProtocol (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, PumpHistoryID, SessionID, DataSourceID) values (?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", (object)(string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)EventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(InfoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("PumpHistoryID", (object)PumpHistoryID),
                    db.CreateParameter("SessionID", SessionContext.SessionId),
                    db.CreateParameter("DataSourceID", (object)DataSourceID)
                    );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        public void WriteEventIntoClassifierProtocol(ClassifiersEventKind eventKind, string classifier, int pumpHistoryID, int dataSourceID, int dataOperationsObjectType, string infoMsg)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                db.ExecQuery(
                    "insert  into ClassifiersOperations (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, Classifier, PumpHistoryID, DataSourceID, SessionID, ObjectType) values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", (object)(string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)eventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(infoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("Classifier", (object)CheckStrLen(classifier, MAX_BRIDGEROLE_LENGTH)),
                    db.CreateParameter("PumpHistoryID", (object)pumpHistoryID),
                    db.CreateParameter("DataSourceID", (object)dataSourceID),
                    db.CreateParameter("SessionID", SessionContext.SessionId),
                    db.CreateParameter("ObjectType", (object)dataOperationsObjectType)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        public void WriteEventIntoUpdateSchemeProtocol(UpdateSchemeEventKind eventKind, string objectFullName, string objectFullCaption, ModificationTypes modificationType, string infoMsg)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeSchemeUpdate, "Запущена операция обновления схемы.");

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                db.ExecQuery(
                    "insert into UpdateSchemeProtocol (Module, KindsOfEvents, InfoMessage, RefUsersOperations, ModificationType, ObjectFullName, ObjectFullCaption, SessionID) values (?, ?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("Module", (object)(string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)eventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(infoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("ModificationType", (object)(int)modificationType),
                    db.CreateParameter("ObjectFullName", (object)CheckStrLen(objectFullName, 64)),
                    db.CreateParameter("ObjectFullCaption", (object)CheckStrLen(objectFullCaption, 255)),
                    db.CreateParameter("SessionID", SessionContext.SessionId)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }


        public void WriteEventIntoDataSourceProtocol(DataSourceEventKind eventKind, int dataSourceID, string infoMsg)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                db.ExecQuery(
                    "insert  into DataSourcesOperations (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, DataSourceID) values (?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object)(int)db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module", (object)(string)CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object)(int)eventKind),
                    db.CreateParameter("InfoMessage", (object)CheckStrLen(infoMsg, MAX_INFOMSG_LENGTH), DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object)userOperationID),
                    db.CreateParameter("SessionID", SessionContext.SessionId),
                    db.CreateParameter("DataSourceID", (object)dataSourceID)
                );
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Вставляет запись в протокол функции перехода на новый год
        /// </summary>
        /// <param name="eventKind"> Тип события</param>
        /// <param name="dataSourceID"> Источник данных</param>
        /// <param name="infoMsg"> Информационное сообщение</param>
        public void WriteEventIntoTransferDBToNewYearProtocol(TransferDBToNewYearEventKind eventKind, int dataSourceID, string infoMsg)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                db.ExecQuery(
                    "insert into NewYearOperations (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, DataSourceID) values (?, ?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object) (int) db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module",
                                       (object) (string) CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object) (int) eventKind),
                    db.CreateParameter("InfoMessage", (object) CheckStrLen(infoMsg, MAX_INFOMSG_LENGTH),
                                       DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object) userOperationID),
                    db.CreateParameter("SessionID", SessionContext.SessionId),
                    db.CreateParameter("DataSourceID", (object) dataSourceID));
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        public void WriteEventIntoMessageExchangeProtocol(MessagesEventKind eventKind, string infoMsg)
        {
            if (userOperationID < 0)
                WriteEventIntoUsersOperationProtocol(UsersOperationEventKind.uoeStartWorking_RefUserName, string.Empty);

            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                db.ExecQuery(
                    "insert into MessageExchangeOperations (ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID) values (?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", (object) (int) db.GetGenerator("g_HUB_EventProtocol")),
                    db.CreateParameter("Module",
                                       (object) (string) CheckStrLen(CallerAssemblyName, MAX_MODULENAME_LENGTH)),
                    db.CreateParameter("KindsOfEvents", (object) (int) eventKind),
                    db.CreateParameter("InfoMessage", (object) CheckStrLen(infoMsg, MAX_INFOMSG_LENGTH),
                                       DbType.AnsiString),
                    db.CreateParameter("RefUsersOperations", (object) userOperationID),
                    db.CreateParameter("SessionID", SessionContext.SessionId));
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

        #region Получение данных протокола
        /// <summary>
        /// Получение данных протокола (с фильтром)
        /// </summary>
        /// <param name="mt">Тип протокола</param>
        /// <param name="ProtocolData">Данные протокола</param>
        /// <param name="Filter">Условия фильтрации</param>
        /// <param name="parameters">Параметры фильтрации</param>
        public void GetProtocolData(ModulesTypes mt, ref DataTable ProtocolData, string Filter, params IDbDataParameter[] parameters)
        {
            string queryStr = string.Empty;
            switch (mt)
            {
                case ModulesTypes.DataPumpModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.PumpHistoryID, dpp.DataSourceID, uo.UserName, dpp.SessionID" +
                        " from DataPumpProtocol dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                case ModulesTypes.BridgeOperationsModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.BridgeRoleA, dpp.BridgeRoleB, dpp.PumpHistoryID, dpp.DataSourceID, uo.UserName, dpp.SessionID" +
                        " from BridgeOperations dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                    /*
                case ModulesTypes.MDProcessingModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.MDObjectName, dpp.PumpHistoryID, dpp.DataSourceID, uo.UserName, dpp.SessionID" +
                        " from MDProcessing dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                */
                 case ModulesTypes.MDProcessingModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.MDObjectName, dpp.OlapObjectType, uo.UserName, dpp.SessionID," +
                        " dpp.ObjectIdentifier, dpp.BatchId" +
                        " from v_MDProcessing dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                 
                case ModulesTypes.ReviseDataModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.PumpHistoryID, dpp.DataSourceID, uo.UserName, dpp.SessionID" +
                        " from ReviseDataProtocol dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                case ModulesTypes.ProcessDataModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.PumpHistoryID, dpp.DataSourceID, uo.UserName, dpp.SessionID" +
                        " from ProcessDataProtocol dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                case ModulesTypes.DeleteDataModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.PumpHistoryID, dpp.DataSourceID, uo.UserName, dpp.SessionID" +
                        " from DeleteDataProtocol dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                case ModulesTypes.UsersOperationsModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.UserName, dpp.ObjectName, dpp.ActionName, dpp.UserHost, dpp.SessionID" +
                        " from UsersOperations dpp " +
                        " where (1 = 1) and (dpp.KindsOfEvents < 40101 or dpp.KindsOfEvents > 40108)" +
                        " and (dpp.KindsOfEvents < 40201 or dpp.KindsOfEvents > 40208) and (dpp.KindsOfEvents <> 40100)";
                    break;
                case ModulesTypes.SystemEventsModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage, dpp.ObjectName, dpp.SessionID" +
                        " from SystemEvents dpp" +
                        " where (1 = 1)";
                    break;
                case ModulesTypes.PreviewDataModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.PumpHistoryID, dpp.DataSourceID, uo.UserName, dpp.SessionID" +
                        " from PreviewDataProtocol dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                        //" where (dpp.RefUsersOperations = uo.ID)";
                    break;
                case ModulesTypes.ClassifiersModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.Classifier, dpp.ObjectType, dpp.PumpHistoryID, dpp.DataSourceID, uo.UserName, dpp.SessionID" +
                        " from ClassifiersOperations dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                        //" where (dpp.RefUsersOperations = uo.ID)";
                    break;
                case ModulesTypes.UpdateModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, dpp.ModificationType, dpp.ObjectFullName, dpp.ObjectFullCaption, uo.UserName, dpp.SessionID" +
                        " from UpdateSchemeProtocol dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                case ModulesTypes.DataSourceModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, uo.UserName, dpp.SessionID, dpp.datasourceid" +
                        " from DataSourcesOperations dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                case ModulesTypes.TransferDBToNewYearModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, uo.UserName, dpp.SessionID, dpp.datasourceid" +
                        " from NewYearOperations dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
                case ModulesTypes.MessagesExchangeModule:
                    queryStr = "Select dpp.ID, dpp.EventDateTime, dpp.Module, dpp.KindsOfEvents, dpp.InfoMessage," +
                        " dpp.RefUsersOperations, uo.UserName, dpp.SessionID" +
                        " from MessageExchangeOperations dpp" +
                        " left outer join UsersOperations uo on dpp.RefUsersOperations = uo.ID";
                    break;
            }
            if (Filter != null & Filter != "")
            {
                if (queryStr.Contains("where"))
                    queryStr = queryStr + " and " + Filter;
                else
                    queryStr = queryStr + " where " + Filter;
            }
            queryStr = queryStr + " order by ID";
            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                ProtocolData = (DataTable)db.ExecQuery(queryStr, QueryResultTypes.DataTable, parameters);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }
        
        /// <summary>
        /// Получение граничных дат в протоколах
        /// </summary>
        public void GetProtocolsDate(ref DateTime MinDate, ref DateTime MaxDate)
        {
            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                string QueryStrMin = "select min(eventdatetime) from hub_eventprotocol t";
                string QueryStrMax = "select max(eventdatetime) from hub_eventprotocol t";
                MinDate = (DateTime)db.ExecQuery(QueryStrMin, QueryResultTypes.Scalar);
                MaxDate = (DateTime)db.ExecQuery(QueryStrMax, QueryResultTypes.Scalar);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Получить данные протокола (без фильтра)
        /// </summary>
        /// <param name="mt">Тип протокола</param>
        /// <param name="ProtocolData">Данные протокола</ProtocolData>
        public void GetProtocolData(ModulesTypes mt, ref DataTable ProtocolData)
        {
            GetProtocolData(mt, ref ProtocolData, "");
        }
        #endregion

        #region Удаление данных протокола
        public int DeleteProtocolData(ModulesTypes mt, int sourceID)
        {
            return DeleteProtocolData(mt, String.Format("(SourceID = {0})", sourceID.ToString()));
        }

        public int DeleteProtocolData(ModulesTypes mt, int sourceID, int pumpHistoryID)
        {
            string filterStr = string.Empty;
            if (sourceID != -1)
            {
                filterStr = string.Format("(DataSourceID = {0})", sourceID.ToString());
            }
            if (pumpHistoryID != -1)
            {
                if (filterStr != String.Empty) filterStr = filterStr + " and ";
                filterStr = string.Format("{0}(PumpHistoryID = {1})", filterStr, pumpHistoryID.ToString());
            }
            return DeleteProtocolData(mt, filterStr);
        }

        public int DeleteProtocolData(ModulesTypes mt, string filterStr)
        {
            string queryStr = string.Empty;
            switch (mt)
            {
                case ModulesTypes.DataPumpModule:
                    queryStr = "delete from DataPumpProtocol";
                    break;
                case ModulesTypes.BridgeOperationsModule:
                    queryStr = "delete from BridgeOperations";
                    break;
                case ModulesTypes.MDProcessingModule:
                    queryStr = "delete from MDProcessing";
                    break;
                case ModulesTypes.ReviseDataModule:
                    queryStr = "delete from ReviseDataProtocol";
                    break;
                case ModulesTypes.ProcessDataModule:
                    queryStr = "delete from ProcessDataProtocol";
                    break;
                case ModulesTypes.DeleteDataModule:
                    queryStr = "delete from DeleteDataProtocol";
                    break;
                case ModulesTypes.UsersOperationsModule:
                    queryStr = "delete from UsersOperations";
                    break;
                case ModulesTypes.SystemEventsModule:
                    queryStr = "delete from SystemEvents";
                    break;
                case ModulesTypes.PreviewDataModule:
                    queryStr = "delete from PreviewDataProtocol";
                    break;
                case ModulesTypes.ClassifiersModule:
                    queryStr = "delete from ClassifiersOperations";
                    break;
                case ModulesTypes.DataSourceModule:
                    queryStr = "delete from DataSourcesOperations";
                    break;
                case ModulesTypes.TransferDBToNewYearModule:
                    queryStr = "delete from NewYearOperations";
                    break;
            }
            if (filterStr != String.Empty) queryStr = queryStr + " where " + filterStr;
            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                db.ExecQuery(queryStr, QueryResultTypes.NonQuery);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return -1;
        }
        public bool DeleteProtocolArchive(string filterStr, params IDbDataParameter[] parameters)
        {
            IDatabase db = null;
            try
            {
                db = _scheme.SchemeDWH.DB;
                db.ExecQuery(filterStr, QueryResultTypes.NonQuery, parameters);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return true;
        }
        #endregion


        #region IBaseProtocol Members

        /// <summary>
        /// Получение минималный даты протокола
        /// </summary>
        public DateTime MinProtocolsDate
        {
            get
            {
                using (IDatabase db = _scheme.SchemeDWH.DB)
                {
                    return (DateTime)db.ExecQuery("select MIN(eventdatetime) from hub_eventprotocol", QueryResultTypes.Scalar); 
                }
            }
        }

        #endregion
    }
}
