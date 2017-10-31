using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinDock;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common
{
    #region Прототипы наиболее часто используемых делегатов для событий
    public delegate int GetIntDelegate();
    public delegate void SetIntDelegate(int Val);
    public delegate string GetStringDelegate();
    public delegate void SetStringDelegate(string Val);
    public delegate bool GetBoolDelegate();
    public delegate void SetBoolDelegate(bool Val);
    public delegate void VoidDelegate();
    #endregion

    #region перечисления, которые используются в разных объектах просмотра

    public enum NavigationNodeKind
    {
        ndUnknown,
        ndAllUsers,
        ndAllGroups,
        ndAllObjects,
        ndAllDirectoryes,
        ndOrganizations,
        ndDivisions,
        ndTasksTypes,
        ndSessions
    };

    public enum AuditShowObjects
    {
        ClsObject,
        RowObject,
        TaskObject,
        ConstObject
    }

    #endregion

    #region Интерфейсы объектов клиентской части
    /// <summary>
    /// Интерфейс оболочки Workplace, доступный в объектах просмотра
    /// </summary>
    public interface IWorkplace : IServiceProvider
    {
        /// <summary>
        /// Интерфейс схемы к которой подключен Workplace
        /// </summary>
        IScheme ActiveScheme { get; }
        /// <summary>
        /// Объект - форма прогресса (создается при первом обращении)
        /// </summary>
        Progress ProgressObj { get; }
        /// <summary>
        /// Объект для индикации длительных операций (создается при первом обращении)
        /// </summary>
        Operation OperationObj { get; }
        /// <summary>
        /// Указатель на главный мэнэджер тулбаров Workplace
        /// </summary>
        UltraToolbarsManager MainToolbar { get; }
        /// <summary>
        /// указатель на главный статусбар Workplace
        /// </summary>
        UltraStatusBar MainStatusBar { get; }
        /// <summary>
        /// Объект-просмотрщик протоколов
        /// </summary>
        IInplaceProtocolView ProtocolsInplacer { get; }

        object ActiveContent { get; }

        IModalClsManager ClsManager { get; }

        string ViewObjectCaption { set; }

        void ChangePasswordAdm(int userID);

        void SwitchTo(string uiModuleName, params object[] moduleParams);

        IInplaceClsView GetClsView(IEntity cls);

        IInplaceTasksPermissionsView GetTasksPermissions();

        int WndHandle { get; }

        bool IsDeveloperMode { get; }

        IWin32Window WindowHandle { get; }

        ITimedMessageManager TimedMessageManager { get; }
    }

    public interface ITimedMessageManager
    {
        // Таблица с сообщениями
        DataTable MessagesTable { get; }
        // Время последнего обновления
        DateTime LastUpdate { get; }
        // Количество всех новых сообщений
        int NewMessagesCount { get; set; }
        // Количество новых важных сообщений
        int NewImportanceMessages { get; set; }

        void Activate();
        // Получить вложение для письма
        MessageAttachmentDTO GetMessageAttachment(int messageId);
        // Обновить статус сообщения
        void UpdateMessage(int messageId, MessageStatus status);
        // Удалить сообщение
        void DeleteMessage(int messageId);
        // Получить сообщения
        void ReceiveMessages();

        // Событие при получении сообщений
        event EventHandler OnReciveMessages;
        // Событие при запуске получения сообщений
        event EventHandler OnStartReciveMessages;
    }

    /// <summary>
    /// Интерфейс объекта для внедрения протоколов
    /// </summary>
    public interface IInplaceProtocolView
    {
        /// <summary>
        /// Метод для внедрения грида с протоколом в произвольный элемент управления
        /// </summary>
        /// <param name="mt">Тип протокола</param>
        /// <param name="ParentArea">Элемент управления</param>
        /// <param name="Filter">Строка фильтра</param>
        /// <param name="parameters">Параметры фильтрации</param>
        void AttachViewObject(ModulesTypes mt, Control ParentArea, string Filter, params IDbDataParameter[] parameters);

        /// <summary>
        /// Метод для внедрения грида с протоколом в произвольный элемент управления
        /// </summary>
        /// <param name="mt">Тип протокола</param>
        /// <param name="ParentArea">Элемент управления</param>
        /// <param name="protocolFileName">Название файла по умолчанию, куда будет сохраняться лог</param>
        /// <param name="Filter">Строка фильтра</param>
        /// <param name="parameters">Параметры фильтрации</param>
        void AttachViewObject(ModulesTypes mt, Control ParentArea, string protocolFileName, string Filter, params IDbDataParameter[] parameters);

        /// <summary>
        ///	 Метод для обновления данных во внедренном гриде
        /// </summary>
        /// <param name="Filter">Фильтр запроса</param>
        /// <param name="FilterParams">Параметры в запросе</param>
        void RefreshAttachData(string protocolFileName, string Filter, params IDbDataParameter[] FilterParams);

        /// <summary>
        /// Метод для обновления данных во внедренном гриде
        /// </summary>
        void RefreshAttachData();

        /// <summary>
        /// метод для внедрения грида с аудитом в произвольный элемент управления
        /// </summary>
        /// <param name="parentArea"></param>
        /// <param name="fileName"></param>
        /// <param name="auditObject"></param>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        void AttachAudit(Control parentArea, string fileName, AuditShowObjects auditObject, string filter, params IDbDataParameter[] filterParams);

        /// <summary>
        /// метод для внедрения грида с аудитом в произвольный элемент управления с учетом истории закачки
        /// </summary>
        /// <param name="parentArea"></param>
        /// <param name="fileName"></param>
        /// <param name="classType"></param>
        /// <param name="objectName"></param>
        /// <param name="pumpId"></param>
        /// <param name="auditFilter"></param>
        /// <param name="auditParams"></param>
        void AttachAudit(Control parentArea, string fileName, ClassTypes classType, string objectName,
                         int pumpId, string auditFilter, params IDbDataParameter[] auditParams);

        /// <summary>
        /// метод для обновления данных аудита во внедренном гриде
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterParams"></param>
        void RefreshAttachAuditData(string filter, params IDbDataParameter[] filterParams);

        /// <summary>
        /// обновление данных аудита с учетом данных из истории закачек
        /// </summary>
        void RefreshAttachAuditData(int pumpId, ClassTypes classType, string objectName, string filter,
                                    params IDbDataParameter[] filterParams);

        /// <summary>
        /// метод для обновления данных аудита во внедренном гриде
        /// </summary>
        void RefreshAttachAuditData();

        UltraGridEx GridComponent
        { get; }

    }

    public interface IInplaceTasksPermissionsView
    {
        void AttachViewObject(Control groupsPermissionsArea, Control usersPermissionsArea);
        void DetachViewObject();
        void Close();
        void RefreshAttachedData(int mainID, SysObjectsTypes tp, bool isUsers);
        void ClearAttachedData(bool isUsers);
        IUsersModal IUserModalForm { get; }
    }

    /// <summary>
    /// Интерфейс объекта для внедрения протоколов
    /// </summary>
    public interface IInplaceClsView
    {
        /// <summary>
        /// Метод для внедрения классификатора в объект просмотра
        /// </summary>		
        void AttachViewObject(Control parentControl);

        void InitModalCls(int oldID);
        /// <summary>
        ///	 Метод для обновления данных во внедренном гриде
        /// </summary>
        object RefreshAttachedData();

        object RefreshAttachedData(int sourceID);

        void TrySetDataSource(int sourceId);

        int GetSelectedID();

        List<int> GetSelectedIDs();

        string GetClsRusName();

        DataSet GetClsDataSet();

        UltraGridEx UltraGridExComponent { get; }

        IEntity ActiveDataObj { get; set; }

        int CurrentSourceID { get; }

        int CurrentDataSourceYear { get; set; }

        string AdditionalFilter { get; set; }

        HierarchyInfo GetHierarchyInfo(object sender);

        void AttachCls(Control ctrl, ref IInplaceClsView attCls);

        void DetachViewObject();

        void FinalizeViewObject();

        void SaveChanges();

        UltraToolbarsManager GetClsToolBar();

        void DataSourceSelected(object sender, ToolDropdownEventArgs e);

        event VoidDelegate RefreshData;

        event VoidDelegate SelectDataSource;

        IInplaceClsView ProtocolsInplacer { get; }

        void GetColumnsValues(string[] getColumns, ref object[] columnsValues);

        bool IsCurrentVersion { get; set; }

        bool SaveLastSelectedDataSource { get; set; }

        bool InAssociateMode { get; set; }
    }

    public interface IModalClsManager
    {
        void Clear();
        bool ShowClsModal(string clsName, int oldClsID, int sourceID, ref object clsID);
        bool ShowClsModal(string clsName, int oldClsID, int sourceID, int sourceYear, ref object clsID);
        bool ShowClsModal(string clsName, int oldClsID, int sourceID, int sourceYear, ref object clsID, bool singleId);
        bool ShowClsModal(string clsName, int oldClsID, int sourceID, int sourceYear, ref object clsID, ref DataTable selectedData);
    }

    public interface IUsersModal
    {
        bool ShowModal(NavigationNodeKind kind, ref int mainID, ref string name);
    }

    public interface ILookupManager : IDisposable
    {
        void Clear();
        string GetLookupValue(string objName, bool needFoolValue, int ID);
        bool CheckLookupValue(string objName, int ID);
        void InitLookupsCash(IEntity obj,
            DataSet sourceData//,
            //string clientFilter,         
            //List<Krista.FM.Client.Components.UltraGridEx.FilterParamInfo> filterParameters
            );
    }

    public interface IBaseClsNavigation
    {
        IInplaceClsView GetClsView(IEntity cls);
        IModalClsManager ClsManager();
    }

    public interface IProtocolNavigation
    {
        IInplaceProtocolView ProtocolsInplacer();
    }

    public interface IAdministrationNavigation
    {
        IInplaceTasksPermissionsView GetTasksPermissions();
    }

    #endregion

    #region Общие методы
    public struct StringHelper
    {

        private const string Space = " ";
        //private const string Enter = "/r";
        private const string NewLine = "\r\n";
        private const string Tab = "\t";

        private const string spaceBeforeEnter = " \r\n";
        private const string spaceAfterEnter = "\r\n ";

        private static char[] trimSymbols = { '\r', '\n', ' ' };
        private static char[] textTrimSymbols = { '\r', '\n' };

        /// <summary>
        /// Нормализует строку, убирает двойные пробелы, табуляции и новые строки
        /// </summary>
        /// <param name="normalizingString"></param>
        /// <returns></returns>
        public static string GetNormalizeString(string normalizingString)
        {
            // очищаем строку от лишних пробелов
            string returnStr = normalizingString;
            // убираем лишние пробелы
            string[] strParts = returnStr.Split(new string[] { Space }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i <= strParts.Length - 1; i++)
            {
                strParts[i] = strParts[i].Replace(Space, string.Empty);
            }
            returnStr = String.Join(Space, strParts);
            // убираем лишние новые строки
            strParts = returnStr.Split(new string[] { NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i <= strParts.Length - 1; i++)
            {
                strParts[i] = strParts[i].Replace(NewLine, string.Empty);
            }
            returnStr = String.Join(NewLine, strParts);
            // убираем лишние табуляции
            strParts = returnStr.Split(new string[] { Tab }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i <= strParts.Length - 1; i++)
            {
                strParts[i] = strParts[i].Replace(Tab, string.Empty);
            }
            returnStr = String.Join(Tab, strParts);
            returnStr = returnStr.Trim(trimSymbols);
            return returnStr;
        }

        public static string GetNormalizeTextString(string normalizingString)
        {
            string returnString = normalizingString.TrimEnd(trimSymbols);
            return returnString.TrimStart(textTrimSymbols);
        }
    }

    public struct DataTableHelper
    {
        public static bool CopyDataTable(DataTable sourceTable, ref DataTable destinationTable)
        {
            return CopyDataTable(sourceTable, ref destinationTable, String.Empty);
        }

        public static bool CopyDataTable(DataTable sourceTable, ref DataTable destinationTable, string sortOrder)
        {
            return CopyDataTable(sourceTable, ref destinationTable, String.Empty, String.Empty);
        }

        public static bool CopyDataTable(DataTable sourceTable, ref DataTable destinationTable, string sortOrder, string filter)
        {
            // если исходная таблица не задана - создать копию невозможно
            if (sourceTable == null)
                return false;
            // если не задана результриующая таблица - делаем клон исходной
            if (destinationTable == null)
            {
                destinationTable = sourceTable.Clone();
            }
            try
            {
                DataRow[] rows;
                if (!String.IsNullOrEmpty(sortOrder))
                {
                    rows = sourceTable.Select(filter, sortOrder);
                }
                else
                {
                    rows = sourceTable.Select(filter);
                }
                destinationTable.BeginLoadData();
                foreach (DataRow row in rows)
                {
                    destinationTable.Rows.Add(row.ItemArray);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                destinationTable.EndLoadData();
            }
        }

        public static bool CopyDataSet(DataSet sourceDataSet, ref DataSet destinationDataSet)
        {
            return CopyDataSet(sourceDataSet, ref destinationDataSet, string.Empty);
        }

        public static bool CopyDataSet(DataSet sourceDataSet, ref DataSet destinationDataSet, string sortOrder)
        {
            // если исходная таблица не задана - создать копию невозможно
            if (sourceDataSet == null)
                return false;
            // если не задана результриующая таблица - делаем клон исходной
            if (destinationDataSet == null)
            {
                destinationDataSet = sourceDataSet.Clone();
            }
            try
            {
                DataRow[] rows;
                foreach (DataTable table in sourceDataSet.Tables)
                {
                    if (!String.IsNullOrEmpty(sortOrder))
                    {
                        rows = table.Select(String.Empty, sortOrder);
                    }
                    else
                    {
                        rows = table.Select();
                    }
                    destinationDataSet.Tables[table.TableName].BeginLoadData();
                    foreach (DataRow row in rows)
                    {
                        destinationDataSet.Tables[table.TableName].Rows.Add(row.ItemArray);
                    }
                    destinationDataSet.Tables[table.TableName].EndLoadData();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Удаление пустых дат из таблицы.
        /// </summary>
        /// <param name="table"></param>
        public static void RemoveEmptyData(ref DataTable table)
        {
            try
            {
                DateTime dateTimeMinValue = System.Data.SqlTypes.SqlDateTime.MinValue.Value.AddHours(3);

                table.BeginLoadData();
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        if (column.DataType == typeof(DateTime))
                        {
                            if (!String.IsNullOrEmpty(row[column].ToString()) && ((DateTime)row[column]) <= dateTimeMinValue)
                            {
                                row[column] = DBNull.Value;
                            }
                        }
                    }
                }
            }
            finally
            {
                table.EndLoadData();
            }
            table.AcceptChanges();
        }

        internal static int MAX_PARAMS_IN_CONDITION = 1000;

        public static List<string> GetDistinctFieldValuesConstraints(DataTable dt, string columnName)
        {
            List<string> res = new List<string>();
            int clmnIndex = dt.Columns.IndexOf(columnName);
            if (clmnIndex < 0)
                throw new ArgumentException(String.Format("Колонка '{0}' не найдена", columnName));

            DataColumn clmn = dt.Columns[clmnIndex];
            SortedList<object, object> dictinctValues = new SortedList<object, object>();
            foreach (DataRow row in dt.Rows)
            {
                object curVal = row[clmn];
                if ((curVal == DBNull.Value) || (curVal == null))
                    continue;
                if (dictinctValues.IndexOfKey(curVal) == -1)
                {
                    dictinctValues.Add(curVal, null);
                }
            }

            if (dictinctValues.Count == 0)
                return res;

            StringBuilder sb = new StringBuilder(dictinctValues.Count * 10);
            for (int i = 0; i < dictinctValues.Count; i++)
            {
                if ((i % MAX_PARAMS_IN_CONDITION == 0) && (sb.Length > 0))
                {
                    sb.Remove(sb.Length - 2, 2);
                    res.Add(sb.ToString());
                    sb.Length = 0;
                }
                sb.Append(dictinctValues.Keys[i].ToString());
                sb.Append(", ");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 2, 2);
                res.Add(sb.ToString());
            }

            /*StringBuilder sb = new StringBuilder(dictinctValues.Count * 10);
            foreach (object curVal in dictinctValues.Keys)
            {
                sb.Append(curVal.ToString());
                sb.Append(", ");
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);*/

            return res;
        }
    }

    #endregion
}
