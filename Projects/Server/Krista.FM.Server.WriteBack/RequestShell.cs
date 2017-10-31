using System;
using System.Threading;
using System.Xml;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.WriteBack
{
    /// <summary>
    /// Запрос с занными
    /// </summary>
    internal sealed class RequestData
    {
        static object nextID = 0;

        // Идентификатор
        public Int64 ID;
        public Guid BatchID;
        // Способ выполнения запроса
        public bool Async = false;
        // Данные
        public XmlDocument Data;

        public RequestData()
        {
            lock (nextID)
            {
                nextID = ID = Convert.ToInt64(nextID) + 1;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class RequestShell
    {
        internal RequestData request;
        private readonly IScheme scheme;
        private Thread queryThread;
        RequestDataProcess requestDataProcess;
        internal Exception exception = null;
        internal int RequestID = -1;


        public RequestShell(RequestData request, IScheme scheme)
        {
            this.request = request;
            this.scheme = scheme;
            requestDataProcess = new RequestDataProcess(request, new SchemeService(scheme));
        }

        public void Run(LogicalCallContextData userContext)
        {
            Run(userContext, false);
        }
        
        public void Run(LogicalCallContextData userContext, bool waitResult)
        {
            queryThread = new Thread(new ParameterizedThreadStart(this.EntryPoint));
            queryThread.Start(userContext);
            if (waitResult)
            {
                queryThread.Join();
            }
        }

        public void EntryPoint(object args)
        {
            try
            {
                LogicalCallContextData.SetContext((LogicalCallContextData)args);

                requestDataProcess.Process();
            }
            catch (Exception e)
            {
                exception = e;
                RequestID = requestDataProcess.CurrentRequestID;
            }

            //Сообщим серверу о том, что запрос завершен
            WriteBackServerProcess.QueryComplete(this);
        }

    }
}
