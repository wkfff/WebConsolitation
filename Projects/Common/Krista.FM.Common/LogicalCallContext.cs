using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;

namespace Krista.FM.Common
{
    /// <summary>
    /// Объект для хранения вспомогательной информации в контексте вызова методов
    /// </summary>
    [Serializable]
    public class LogicalCallContextData : ILogicalThreadAffinative
    {
        //public static bool ShowTrace = false;
        int _nAccesses;
        IPrincipal _principal;

        private SortedList items = new SortedList();

        /// <summary>
        /// Общее кол-во обращений к вспомогательной информации контекста вызова
        /// </summary>
        public string numOfAccesses
        {
            get
            {
                return String.Format("The identity of {0} has been accessed {1} times.",
                    _principal.Identity.Name,
                    _nAccesses);
            }
        }

        /// <summary>
        /// IPrincipal
        /// </summary>
        public IPrincipal Principal
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get
            {
                _nAccesses++;
                return _principal;
            }
        }

        /// <summary>
        /// Индексатор для доступа к свойствам
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get
            {
                if (items.ContainsKey(key))
                    return items[key];
                else
                    return null;
            }
            //[System.Diagnostics.DebuggerStepThrough()]
            set
            {
                if (!items.ContainsKey(key))
                    items.Add(key, value);
                else
                {
                    if (value != null)
                        items[key] = value;
                    else
                        items.Remove(key);
                }
            }
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="p">IPrincipal</param>
        public LogicalCallContextData(IPrincipal p)
        {
            _nAccesses = 0;
            _principal = p;
        }

        /*public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1000);
            sb.AppendLine("***");
            foreach (object key in items.Keys)
            {
                sb.AppendLine(String.Format("{0} = {1}", key.ToString(), items[key].ToString()));
            }
            sb.AppendLine("***");
            System.Diagnostics.StackTrace tr = new System.Diagnostics.StackTrace();
            sb.AppendLine(tr.ToString());
            sb.AppendLine("***");
            return sb.ToString();
        }*/

        /// <summary>
        /// Получить вспомогательную информацию из контекста вызова
        /// </summary>
        /// <returns>Вспомогательная информация ("Authorization")</returns>
        [System.Diagnostics.DebuggerStepThrough()]
        public static LogicalCallContextData GetContext()
        {
            return (LogicalCallContextData)CallContext.GetData("Authorization");
        }

        /// <summary>
        /// Установить вспомогательную информацию для контекста вызова
        /// </summary>
        /// <param name="context">Вспомогательная информация ("Authorization")</param>
        [System.Diagnostics.DebuggerStepThrough()]
        public static void SetContext(LogicalCallContextData context)
        {
            //if (ShowTrace)
            //    System.Diagnostics.Trace.WriteLine(context.ToString());
            CallContext.SetData("Authorization", context);
        }

        /// <summary>
        /// Установить в качестве авторизационной информации имя текущего пользователя
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough()]
        public static void SetAuthorization()
        {
            SetAuthorization(WindowsIdentity.GetCurrent().Name);            
        }

        /// <summary>
        /// Установить в качестве авторизационной информации заданное имя пользователя
        /// </summary>
        /// <param name="userName">имя пользователя</param>
        [System.Diagnostics.DebuggerStepThrough()]
        public static void SetAuthorization(string userName)
        {
            GenericIdentity ident = new GenericIdentity(userName);
            GenericPrincipal prpal = new GenericPrincipal(ident, new string[] { "Level1" });
            System.Threading.Thread.CurrentPrincipal = prpal;
            LogicalCallContextData data = new LogicalCallContextData(prpal);
            CallContext.SetData("Authorization", data);
        }
    }

    /// <summary>
    /// Структура для доступа к вспомогательной информации в контексте вызова
    /// </summary>
    public struct ClientAuthentication
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public static string UserName
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return LogicalCallContextData.GetContext().Principal.Identity.Name; }
        }

        /// <summary>
        /// ID пользователя
        /// </summary>
        public static int UserID
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get
            {
				return LogicalCallContextData.GetContext()["UserID"] == null ? -1 : (int)LogicalCallContextData.GetContext()["UserID"];
            }
        }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public static string SessionID
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get
            {
                return
                    (string) LogicalCallContextData.GetContext()["SessionID"] ?? string.Empty;
            }
        }
    }
}
