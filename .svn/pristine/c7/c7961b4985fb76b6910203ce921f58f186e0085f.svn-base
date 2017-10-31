using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Krista.FM.Common.Exceptions
{
    /// <summary>
    /// Типы ошибок.
    /// </summary>
    public enum ErrType
    {
        Users,
        Application
    }

    /// <summary>
    /// Дружелюбное сообщение.
    /// </summary>
    public struct FrMessage
    {
        /// <summary>
        /// Тело сообщения.
        /// </summary>
        public string Message;

        /// <summary>
        /// Тип ошибки.
        /// </summary>
        public ErrType erType;
    }


    /// <summary>
    /// Класс для получения дружелюбного сообщения о исключении.
    /// </summary>
    public class FriendlyExceptionService
    {
        /// <summary>
        /// Ключи -- шаблоны, значения -- дружелюбные сообщения.
        /// </summary>
        private static Dictionary<string, FrMessage> serverExceptionMessages = MakeServerExcTable();
        private static Dictionary<string, FrMessage> exceptionMessages = MakeExcTable();
        private static Dictionary<string, FrMessage> systemExceptionMessages = MakeSystemExcTable();
        private static Dictionary<string, FrMessage> typeLoadExceptionMessages = MakeTypeLoadExcTable();
        private static Dictionary<string, FrMessage> invalidOperationExceptionMessages = MakeInvalidOperationExcTable();
        private static Dictionary<string, FrMessage> COMExceptionMessages = MakeCOMExcTable();
        private static Dictionary<string, FrMessage> oracleExceptionMessages = MakeOracleExcTable();
        private static Dictionary<string, FrMessage> SocketExceptionMessages = MakeSocketExcTable();
        private static Dictionary<string, FrMessage> sqlExceptionMessages = MakeSQLExcTable();
        
        private static Dictionary<string, FrMessage> systemServerExceptionMessages = new Dictionary<string, FrMessage>();
        private static Dictionary<string, FrMessage> externalExceptionMessages = new Dictionary<string, FrMessage>();
        private static Dictionary<string, FrMessage> oleDbExceptionExceptionMessages = new Dictionary<string, FrMessage>();
        private static Dictionary<string, FrMessage> dbExceptionExceptionMessages = new Dictionary<string, FrMessage>();

        /// <summary>
        /// Возвращает дружелюбное сообщение об исключении.
        /// </summary>
        /// <param name="e">Исключение для которого нужно получить дружелюбное сообщение об ошибке.</param>
        /// <returns>Дружелюбное сообщение.</returns>
        public static FrMessage GetFriendlyMessage(Exception e)
        {
            Dictionary<string, FrMessage> friendlyMessages = GetMessagesTable(e);

            FrMessage friendlyMessage = new FrMessage();
            friendlyMessage.Message = e.Message;

            string innerExceptionMessage = GetInnerExceptionMessage(e);

            // Если не найдем соответствие, будем считать что ошибка приложения.
            friendlyMessage.erType = ErrType.Application;

            foreach (string pattern in friendlyMessages.Keys)
            {
                Regex regExp = new Regex(pattern);
                if (regExp.Match(innerExceptionMessage).Success)
                {
                    friendlyMessages.TryGetValue(pattern, out friendlyMessage);
                }
            }
            return friendlyMessage;
        }

        private static string GetInnerExceptionMessage(Exception e)
        {
            if (e.InnerException == null)
            {
                return e.Message;
            }
            else
            {
                return GetInnerExceptionMessage(e.InnerException);
            }
        }

        #region Выбор нужной таблицы сообщений.

        private static Dictionary<string, FrMessage> GetMessagesTable(Exception e)
        {
            switch (ClassifyByInner(e))
            {
                case "ServerException":
                    return serverExceptionMessages;

                case "Exception":
                    return exceptionMessages;

                case "TypeLoadException":
                    return typeLoadExceptionMessages;

                case "InvalidOperationException":
                    return invalidOperationExceptionMessages;

                case "System.Runtime.Remoting.ServerException":
                    return systemServerExceptionMessages;

                case "SystemException":
                    return systemExceptionMessages;

                case "COMException":
                    return COMExceptionMessages;

                case "ExternalException":
                    return externalExceptionMessages;

                case "OleDbException":
                    return oleDbExceptionExceptionMessages;

                case "SqlException":
                    return sqlExceptionMessages;

                case "OracleException":
                    return oracleExceptionMessages;

                case "DbException":
                    return dbExceptionExceptionMessages;

                case "SocketException":
                    return SocketExceptionMessages;
                
                default :
                    return new Dictionary<string, FrMessage>();
            }
        }
        #endregion

        #region Классификация по типу внутренних исключений

        private static string ClassifyByInner(Exception e)
        {
            if (e.InnerException == null)
            {
                return ClassifyByType(e);
            }
            else
            {
                return ClassifyByInner(e.InnerException);
            }
        }

        #endregion

        #region Классификация по типу исключения

        private static bool SeachInExc(Exception e)
        {
            foreach (string pattern in exceptionMessages.Keys)
            {
                Regex regExp = new Regex(pattern);
                if (regExp.Match(e.Message).Success)
                {
                    return true;
                }
            }
            return false;
        }
            
        private static string ClassifyByType(Exception e)
        {
            if (e is SystemException)
                return ClassifySystemException(e);
            else if (e is ServerException)
                return "ServerException";

            return "Exception";
        }
        
        private static string ClassifySystemException(Exception e)
        {
            if (e is ExternalException)
                return ClassifyExternalException(e);
            else if (e is TypeLoadException)
                return "TypeLoadException";
            else if (e is InvalidOperationException)
                return "InvalidOperationException";
            else if (e is System.Runtime.Remoting.ServerException)
                return "System.Runtime.Remoting.ServerException";

            return "SystemException";
        }

        private static string ClassifyExternalException(Exception e)
        {
            if (e is DbException)
                return ClassifyDbException(e);
            else if (e is Win32Exception)
                return ClassifyByWin32Exception(e);
            else if (e is COMException)
                return "COMException";

            return "ExternalException";
        }

        private static string ClassifyByWin32Exception(Exception e)
        {
            if (e is SocketException)
                return "SocketException";

            return "Win32Exception";
        }

        private static string ClassifyDbException(Exception e)
        {
            if (e is OleDbException)
                return "OleDbException";
            else if (e is SqlException)
                return "SqlException";
            else if (e is OracleException)
                return "OracleException";

            return "DbException";
        }

        #endregion

        #region TypeLoadException

        private static Dictionary<string, FrMessage> MakeTypeLoadExcTable()
        {
            Dictionary<string, FrMessage> typeLoadExceptions = new Dictionary<string, FrMessage>();
            FrMessage message = new FrMessage();

            message.Message = "Не найдена реализация вызываемого метода.";
            message.erType = ErrType.Application;
            typeLoadExceptions.Add(
                "Method '.*' in type '.*' from assembly '.*' does not have an implementation",
                message);

            return typeLoadExceptions;
        }
        #endregion

        #region SocketException

        private static Dictionary<string, FrMessage> MakeSocketExcTable()
        {
            Dictionary<string, FrMessage> socketExceptions = new Dictionary<string, FrMessage>();
            FrMessage message = new FrMessage();

            message.Message = "Удаленный хост принудительно разорвал существующее подключение.";
            message.erType = ErrType.Users;
            socketExceptions.Add("Удаленный хост принудительно разорвал существующее подключение", message);

            return socketExceptions;
        }
        #endregion

        #region ServerException

        private static Dictionary<string, FrMessage> MakeServerExcTable()
        {
            Dictionary<string, FrMessage> serverExceptions = new Dictionary<string, FrMessage>();
            FrMessage message = new FrMessage();

            message.Message = "Невозможно выполнить предварительную обработку таблицы базы данных, т.к. для многомерного объекта не найден соответствующий объект в схеме.";
            message.erType = ErrType.Users;
            serverExceptions.Add(
                "Невозможно выполнить предварительную обработку таблицы базы данных, т.к. для многомерного объекта \".*\" не найден соответствующий объект \".*\" в схеме",
                message);

            return serverExceptions;
        }
        #endregion

        #region OracleException

        private static Dictionary<string, FrMessage> MakeOracleExcTable()
        {
            Dictionary<string, FrMessage> oracleExceptions = new Dictionary<string, FrMessage>();
            FrMessage message = new FrMessage();

            message.Message = "Невозможно удалить запись из таблицы данных, т.к. есть записи, которые на нее ссылаются. Попробуйте удалить зависимые записи и повторите операцию";
            message.erType = ErrType.Users;
            oracleExceptions.Add(
                "ORA-[0-9]+: нарушено ограничение целостности .*- обнаружена порожденная запись",
                message);

            message.Message = "Недопустимый идентификатор.";
            message.erType = ErrType.Users;
            oracleExceptions.Add("ORA-[0-9]+: \".*\": недопустимый идентификатор", message);

            message.Message = "Некорректно настроено подключение к базе данных.";
            message.erType = ErrType.Users;
            oracleExceptions.Add("ORA-[0-9]+: TNS:нет прослушивателя", message);

            message.Message = "Вариант закрыт от изменений. Запись и изменение данных варианта запрещены";
            message.erType = ErrType.Users;
            oracleExceptions.Add("ORA-20101.*", message);

            message.Message = "Источник данных закрыт от изменений.Запись и изменение данных по источнику запрещены";
            message.erType = ErrType.Users;
            oracleExceptions.Add("ORA-20102.*", message);

            return oracleExceptions;
        }
        #endregion

        #region InvalidOperationException

        private static Dictionary<string, FrMessage> MakeInvalidOperationExcTable()
        {
            Dictionary<string, FrMessage> invalidOperationExceptions = new Dictionary<string, FrMessage>();
            FrMessage message = new FrMessage();

            message.Message = "Ошибка при выполнении запроса";
            message.erType = ErrType.Application;
            invalidOperationExceptions.Add("Запрос \".*\" вызвал ошибку .*", message);

            message.Message = "При логарифмическом типе оси у отметок, все данные должны быть положительными. Поменяйте тип оси на линейный, или удалите из данных отрицательные значения.";
            message.erType = ErrType.Users;
            invalidOperationExceptions.Add("Invalid data for chart: For Logarithmic axes the data and the range of data should greater than zero.  To enable plotting of zero values on a logarithmic axis, set its LogZero property to a positive value.", message);

            return invalidOperationExceptions;
        }

        #endregion

        #region SystemException
        private static Dictionary<string, FrMessage> MakeSystemExcTable()
        {
            Dictionary<string, FrMessage> exceptions = new Dictionary<string, FrMessage>();
            FrMessage message = new FrMessage();

            //Ошибки MDXExpert3 
            message.Message = "Отчет не может быть сохранен (файл «Только для чтения» или нет прав на запись).";
            message.erType = ErrType.Users;
            exceptions.Add("Access to the path '.*' is denied.", message);

            message.Message = "Размер выносок диаграммы, равен недопустимо малому значению. Попробуйте отключить видимость выносок, или уменьшить количество отображаемых данных.";
            message.erType = ErrType.Users;
            exceptions.Add("Value of '.*' is not valid for 'emSize'. 'emSize' should be greater than 0 and less than or equal to System.Single.MaxValue.\r\nParameter name: emSize", message);            
            
            return exceptions;
        }

        #endregion

        #region Exception
        private static Dictionary<string, FrMessage> MakeExcTable()
        {
            Dictionary<string, FrMessage> exceptions = new Dictionary<string, FrMessage>();
            FrMessage message = new FrMessage();

            message.Message = "Неизвестрый тип данных.";
            message.erType = ErrType.Users;
            exceptions.Add("Неизвестрый тип данных", message);
                        
            message.Message = "Некорректное содержание элемента.";
            message.erType = ErrType.Application;
            exceptions.Add(
                "The element '.*' in namespace '.*' has incomplete content. List of possible elements expected: .*", message);

            message.Message = "Не все ссылки на атрибут были удалены. Вы можете удалить оставшиеся ссылки вручную.";
            message.erType = ErrType.Users;
            exceptions.Add("При удалении ссылок на атрибут возникли следующие ошибки:\n.+", message);
            
            message.Message = "В таблице перекодировок присутствуют записи-дубликаты";
            message.erType = ErrType.Users;
            exceptions.Add("Вставляемая запись в таблице перекодировок уже присутствует.", message);

            message.Message = "Вариант закрыт от изменений. Запись и изменение данных варианта запрещены";
            message.erType = ErrType.Users;
            exceptions.Add("ORA-20101.*", message);

            message.Message = "Источник данных закрыт от изменений.Запись и изменение данных по источнику запрещены";
            message.erType = ErrType.Users;
            exceptions.Add("ORA-20102.*", message);
            
            //Ошибки MDXExpert3            
            message.Message = "Не удалось установить подключение. Убедитесь, что сервер работает.";
            message.erType = ErrType.Users;
            exceptions.Add("Unable to connect to the Analysis server. The server name '.*' was not found. Please verify that the name you entered is correct, and then try again", message);

            message.Message = "Не удалось установить соединение c указанной базой данных.";
            message.erType = ErrType.Users;
            exceptions.Add("Database '.*' does not exist.", message);

            message.Message = "Не удалось установить подключение. Убедитесь, что сервер работает.";
            message.erType = ErrType.Users;
            exceptions.Add("A connection cannot be made. Ensure that the server is running.", message);

            message.Message = "Потеряно соединение с сервером. Запрос не может быть выполнен из-за проблем в сети.";
            message.erType = ErrType.Users;
            exceptions.Add("Connection to the server is lost - The operation requested failed due to network problems.", message);

            message.Message = "При логарифмическом типе оси у отметок в пользовательском диапазоне максимальное и минимальное значения этого диапазона должны быть больше нуля.";
            message.erType = ErrType.Users;
            exceptions.Add("One or more of the following properties were invalid for NumericAxisType of Logarithmic scale.  Please check when AxisRangeType is Custom that both RangeMin and RangeMax are greater than zero.", message);

            message.Message = "Неверно заданы границы диапазона. Для диаграммы типа \"Поверхность\" границы диапазонов осей не должны превышать количество рядов/категорий.";
            message.erType = ErrType.Users;
            exceptions.Add("Index was out of range. Must be non-negative and less than the size of the collection.", message);

            //PropertyGrid
            message.Message = "Значение должно быть цифрой от 0 до 20";
            message.erType = ErrType.Users;
            exceptions.Add("MDXExpert-PropertyGrid-DigitCount.", message);

            message.Message = "Значение должно быть цифрой от 0 до 10000";
            message.erType = ErrType.Users;
            exceptions.Add("MDXExpert-PropertyGrid-CommentDisplayDellay.", message);

            message.Message = "Значение должно быть цифрой от 0 до 10000";
            message.erType = ErrType.Users;
            exceptions.Add("MDXExpert-PropertyGrid-CommentMaxWidth.", message);

            message.Message = "Не найден куб для элемента отчета. Данный элемент не будет загружен.";
            message.erType = ErrType.Users;
            exceptions.Add("Куб \".*\" не найден.", message);

            message.Message = "Отсутствует подключение к серверу.";
            message.erType = ErrType.Users;
            exceptions.Add("MDXExpert-AdomdConnectionIsNull", message);

            message.Message = "Значение должно быть числом от 0 до 255";
            message.erType = ErrType.Users;
            exceptions.Add("MDXExpert-PropertyGrid-SeparatorHeight", message);

            message.Message = "Невозможно подключиться к серверу. Сервер не запущен или занят.";
            message.erType = ErrType.Users;
            exceptions.Add("Cannot connect to the server '.*'. The server is either not started or too busy.", message);
            
            message.Message = "Истекло время выполнения запроса. Измените способ скрытия пустых на NECJ или увеличьте допустимое время выполнения запроса (менять значение свойства Timeout в Mas.udl, в этом случае приложение требует перезагрузки).";
            message.erType = ErrType.Users;
            exceptions.Add("The operation requested failed due to timeout", message);

            message.Message = "Невозможно загрузить отчет. Отчет имеет более новую версию по отношению к текущей версии MDX Эксперт.";
            message.erType = ErrType.Users;
            exceptions.Add("Отчет имеет более новую версию (.*) по отношению к текущей версии MDX Эксперт (.*).", message);

            message.Message = "Ошибка вставки содержимого в таблицу. Количество вставляемых столбцов больше максимально возможного для данной ячейки.";
            message.erType = ErrType.Users;
            exceptions.Add("Error performing Paste operation. Further information: Contents being pasted have more columns than what's available starting from the anchor cell. Paste contents have .* columns where as the available columns starting from the anchor cell are .*.", message);

            message.Message = "Ошибка вставки содержимого в таблицу. Количество вставляемых строк больше максимально возможного для данной ячейки.";
            message.erType = ErrType.Users;
            exceptions.Add("Error performing Paste operation. Further information: Contents being pasted have more rows than what's available starting from the anchor cell. Paste contents have .* rows where as the available rows starting from the anchor cell are .*.", message);

            message.Message = "Ошибка вставки содержимого в таблицу. Ячейка защищена от записи.";
            message.erType = ErrType.Users;
            exceptions.Add("Error performing Paste operation. Further information: .* cell is read-only.", message);

            message.Message = "Ошибка в MDX-запросе.";
            message.erType = ErrType.Users;
            exceptions.Add("Formula error.*", message);

            return exceptions;
        }

        #endregion

        #region COMException

        private static Dictionary<string, FrMessage> MakeCOMExcTable()
        {
            Dictionary<string, FrMessage> COMExceptions = new Dictionary<string, FrMessage>();
            FrMessage message = new FrMessage();

            message.Message = "Не найден элемент, к которому происходит обращение.";
            message.erType = ErrType.Users;
            COMExceptions.Add("Элемент \".*\" не найден", message);

            message.Message = "Уровень данного типа может быть только верхним уровнем иерархии.";
            message.erType = ErrType.Users;
            COMExceptions.Add("Level of type '.*' can be only the top level in the dimension", message);

            message.Message = "Превышена максимальная разрешенная длина строки.";
            message.erType = ErrType.Users;
            COMExceptions.Add("The .* is too long. The maximum length is [0-9]* characters", message);

            message.Message = "Объектная переменная, или переменная блока WITH не установлена.";
            message.erType = ErrType.Application;
            COMExceptions.Add("Object variable or With block variable not set", message);

            message.Message = "Не найдена база данных SourseSafe. Выберите другую базу данных.";
            message.erType = ErrType.Users;
            COMExceptions.Add(
                "The SourceSafe database path .* does not exist. Please select another database", message);

            message.Message = "База данных SourseSafe заблокирована администратором.";
            message.erType = ErrType.Users;
            COMExceptions.Add("The SourceSafe database has been locked by the Administrator", message);

            message.Message = "Некорректное имя уровня.";
            message.erType = ErrType.Users;
            COMExceptions.Add("The level name '' is not valid", message);

            return COMExceptions;
        }

        #endregion

        #region SQLException
        private static Dictionary<string, FrMessage> MakeSQLExcTable()
        {
            Dictionary<string, FrMessage> SQLExceptions = new Dictionary<string, FrMessage>();
            FrMessage message = new FrMessage();

            message.Message = "Невозможно удалить запись из таблицы данных, т.к. есть записи, которые на нее ссылаются. Попробуйте удалить зависимые записи и повторите операцию."; ;
            message.erType = ErrType.Users;
            SQLExceptions.Add("DELETE statement conflicted with the REFERENCE constraint.*", message);

            message.Message = "Источник данных закрыт от изменений.";
            message.erType = ErrType.Users;
            SQLExceptions.Add("Источник заблокирован.*", message);

            message.Message = "Вариант закрыт от изменений. Запись и изменение данных варианта запрещены.";
            message.erType = ErrType.Users;
            SQLExceptions.Add("Вариант заблокирован.*", message);

            return SQLExceptions;
        }
        #endregion
    }
}

#region Вот такие исключения
/* System.Runtime.InteropServices.COMException (0x80004005): Элемент "Packages/" не найден.
 * "Элемент \".*\" не найден"
 * 
 * System.Data.OracleClient.OracleException: 
 * ORA-02292: нарушено ограничение целостности (DV.JI_HUIHI_VGFFDG)- обнаружена порожденная запись
 * "ORA-[0-9]+: нарушено ограничение целостности .*- обнаружена порожденная запись"
 * 
 * Krista.FM.Server.Scheme Information: 0 : System.Runtime.InteropServices.COMException (0x800A0016): Level of type 'All' can be only the top level in the dimension
 * "Level of type '.*' can be only the top level in the dimension"
 * 
 * Krista.FM.Server.Scheme Information: 0 : System.Runtime.InteropServices.COMException (0x80040004): The  is too long. The maximum length is 50 characters.
 * "The .* is too long. The maximum length is [0-9]* characters"
 *   
Krista.FM.Server.Scheme Information: 0 : System.Runtime.InteropServices.COMException (0x800A005B): Object variable or With block variable not set
 * "Object variable or With block variable not set"

System.Runtime.InteropServices.COMException (0x8004D117): The SourceSafe database path SchemeDesigner does not exist. Please select another database.
 * "The SourceSafe database path .* does not exist. Please select another database"
 * 
 * System.TypeLoadException: Method 'CopyAndAssociateRow' in type 'Krista.FM.Server.Scheme.Classes.Fact2BridgeAssociation' from assembly 'Krista.FM.Server.Scheme, Version=2.3.0.13, Culture=neutral, PublicKeyToken=null' does not have an implementation.
 * "Method '.*' in type '.*' from assembly '.*' does not have an implementation"
 * 
System.Data.OracleClient.OracleException: ORA-12541: TNS:нет прослушивателя
 * "ORA-[0-9]+: TNS:нет прослушивателя"

 *System.Exception: Неизвестрый тип данных
 * "Неизвестрый тип данных"
 * 
 * System.Runtime.InteropServices.COMException (0x80040004): The level name '' is not valid. The  cannot be an empty string.
 * "The level name '' is not valid. The  cannot be an empty string"
 * 
System.Exception: The element 'BridgeMapping' in namespace 'xmluml' has incomplete content. List of possible elements expected: 'xmluml:Mapping'.
 * "The element '.*' in namespace '.*' has incomplete content. List of possible elements expected: .*"
 * 
System.Runtime.InteropServices.COMException (0x8004D728): The SourceSafe database has been locked by the Administrator.
"The SourceSafe database has been locked by the Administrator"
 *
 *System.InvalidOperationException: Запрос "select NeedUpdate from DatabaseVersions where ID = (select max(ID) from DatabaseVersions)" вызвал ошибку ORA-00904: "NEEDUPDATE": недопустимый идентификатор
 * "Запрос \".*\" вызвал ошибку .*"
 * 
 ---> System.Data.OracleClient.OracleException: ORA-00904: "NEEDUPDATE": недопустимый идентификатор  
 * "ORA-[0-9]+: \".*\": недопустимый идентификатор" * 
 * 
 * Krista.FM.Common.ServerException ----------
Message=Невозможно выполнить предварительную обработку таблицы базы данных, т.к. для многомерного объекта "ФО_МесОтч_КонсДефицит Профицит" не найден соответствующий объект "" в схеме.

 */
#endregion