using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;


namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Пул подключений к базе данных
    /// </summary>
    internal class ConnectionPool : IDisposable
    {
		private List<DbConnectionWrapper> pool;

        public ConnectionPool()
        {
			pool = new List<DbConnectionWrapper>(10);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IDbConnection GetConnection()
        {
        	Session session = SessionContext.Session;

			foreach (DbConnectionWrapper conn in pool)
            {
                if (conn.State == ConnectionState.Closed && conn.Available)
                {
                    if (DisposableObject.ShowTrace)
                    {
                        Trace.TraceVerbose("Извлечение из пула коннектов: количество в пуле {0}, сессия {1}, User={2}", pool.Count, session.SessionId, session.Principal.Identity.Name);
                    }

                	conn.Available = false;
                    return conn;
                }
            }
			DbConnectionWrapper newConnection = (DbConnectionWrapper)((IConnectionProvider)session.Scheme.SchemeDWH).Connection;
        	newConnection.Available = false;
            pool.Add(newConnection);

            if (DisposableObject.ShowTrace)
            {
                Trace.TraceVerbose("Добавление в пул коннектов: количество в пуле {0}, сессия {1}, User={2}", pool.Count, session.SessionId, session.Principal.Identity.Name);
            }

            return newConnection;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Session session = (Session)LogicalCallContextData.GetContext()["Session"];
            if (session == null)
                return;

            if (DisposableObject.ShowTrace)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Trace.TraceVerbose("Удаление пула коннектов: количество в пуле {0}, сессия {1}, User={2}", pool.Count, session.SessionId, session.Principal.Identity.Name);
                Console.ResetColor();
            }

            foreach (IDbConnection conn in pool)
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Dispose();
                else
                {
                    Trace.TraceError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Trace.TraceError("При удалении пула коннектов: количество в пуле {0}, сессия {1}, User={2}", pool.Count, session.SessionId, session.Principal.Identity.Name);
                    Trace.TraceError("Встретился коннект имеющий состояние отличное от ConnectionState.Closed: State == {0}", conn.State.ToString());
                    Trace.TraceError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
            }
            pool.Clear();
        }

        #endregion
    }
}
