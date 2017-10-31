using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Xml;

using Krista.Diagnostics;
using Krista.FM.Common;


namespace Krista.FM.Server.WriteBack
{
    /// <summary>
    /// —ервер обработки запросов на обратную запись данных. —одержит очередь запросов и 
    /// распредел€ет выполнение по отдельным потокам учитыва€ лимит
    /// параллельно выполн€ющихс€ запросов
    /// </summary>
    internal sealed class WriteBackServerProcess
    {
        /// <summary>
        /// ќсновной поток сервера, обслуживающий очередь запросов
        /// </summary>
        private static Thread serverThread;

        /// <summary>
        /// очередь запросов
        /// </summary>
        private static Queue requestQueue = Queue.Synchronized(new Queue());
/*
        private static ArrayList runingQueries = ArrayList.Synchronized(new ArrayList());
*/

        private static AutoResetEvent newRequestEvent = new AutoResetEvent(true);
        private static ManualResetEvent stopServiceEvent = new ManualResetEvent(false);

        internal static int requestThreadsCount = 0;
        private static int maxThreadsCount = 5;

        internal static void Run()
        {
            stopServiceEvent.Reset();			// —брос флага остановки сервера
            if (serverThread == null)
            {
                Trace.TraceVerbose("«апуск сервера обратной записи...");
                serverThread = new Thread(new ParameterizedThreadStart(EntryPoint));
                serverThread.Start(LogicalCallContextData.GetContext());
            }
        }

        internal static void Stop()
        {
            if (serverThread != null)
            {
                stopServiceEvent.Set();			// ѕросигналим потоку о том, что надо завершитьс€		
                Trace.TraceVerbose("«акрытие сервера обратрой записи...");
                serverThread.Join();			// ƒождемс€ его завершени€
                serverThread = null;			// ”далим объект потока
            }
        }

        private static XmlDocument StdResponse(int requesrID, Guid batchId)
        {
            XmlDocument response = WriteBackServerClass.CreateXMLDocument();
            response.InnerXml = String.Format("<Response><RequestID>{0}</RequestID><BatchID>{1}</BatchID></Response>", requesrID, batchId);
            return response;
        }

        private static XmlDocument StdResponse(Exception exception, int requestID)
        {
            XmlDocument response = WriteBackServerClass.CreateXMLDocument();
			Trace.TraceError("ќшибка обратной записи: {0}", KristaDiagnostics.ExpandException(exception));
            response.InnerXml = String.Format(
				"<Exception><Message><![CDATA[{0}]]></Message><Source><![CDATA[{1}]]></Source><StackTrace><![CDATA[{2}]]></StackTrace><RequestID>{3}</RequestID></Exception>",
				Krista.FM.Common.Exceptions.FriendlyExceptionService.GetFriendlyMessage(exception).Message, 
				exception.Source, 
				KristaDiagnostics.ExpandException(exception), 
				requestID);
            return response;
        }

        internal static XmlDocument Request(RequestData request)
        {
            Debug.WriteLine("New writeback request Queue:" + (requestQueue.Count + 1) + " Queries:" + requestThreadsCount);
            try
            {
                if (Krista.FM.Server.Common.Authentication.UserID == null)
                {
                    throw new ServerException(
                        "ќбратна€ запись без аутентификации клиента невозможна. " +
                        "«апретите анонимный доступ к веб-сервису, оставив режим встроенной проверки подленности Windows.");
                }
                if (request.Async)
                {
                    requestQueue.Enqueue(request);		// ƒополним очередь запросов			
                    newRequestEvent.Set();				// и просигналим потоку о новом запросе
                    return StdResponse(request.GetHashCode(), request.BatchID);
                }
                else
                {
                    RequestShell requestShell = new RequestShell(request, WriteBackServerClass.Scheme);
                    requestShell.Run(LogicalCallContextData.GetContext(), true);
                    if (requestShell.exception != null)
                        return StdResponse(requestShell.exception, requestShell.RequestID);
                    else
                        return StdResponse(request.GetHashCode(), request.BatchID);
                }
            }
            catch (Exception e)
            {
                return StdResponse(e, -1);
            }
        }

        private static void EntryPoint(object args)
        {
            Thread.CurrentThread.Name = "WriteBack";

            LogicalCallContextData.SetContext((LogicalCallContextData)args);

            //Debug.WriteLine("Writeback Server Started at " + DateTime.Now.ToString());
            WaitHandle[] wait_objects = new WaitHandle[2];
            
            wait_objects[0] = newRequestEvent;
            wait_objects[1] = stopServiceEvent;
            
            while (WaitHandle.WaitAny(wait_objects) == 0) //ѕросигналил объект событи€ обработки нового запроса или остановки сервера 
            {
                {
                    // ќбрабатываем очередь, если не превышено число параллельно вычисл€емых запросов
                    if (
                        (requestThreadsCount < maxThreadsCount) &&
                        (requestQueue.Count > 0)
                    )
                    {
                        // начинаем обработку нового запроса
                        RequestData requestData = (RequestData)(requestQueue.Dequeue());
                        RequestShell requestShell = new RequestShell(requestData, WriteBackServerClass.Scheme);
                        requestThreadsCount++;
                        requestShell.Run(LogicalCallContextData.GetContext());
                    }
                }
            }
            // ¬ыход из цикла возможен только по сигналу событи€ остановки сервера
            // TODO: ѕринудительно завершить выполнение текущих запросов или ждать их завершени€
            // Wait(runingQueries);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestShell"></param>
        internal static void QueryComplete(RequestShell requestShell)
        {
            if (requestShell.request.Async)
                requestThreadsCount--;		//”меньшим счетчик числа работающих очтеров
            Debug.WriteLine("Writeback request complete. Queue:" + (requestQueue.Count) + " Queries:" + requestThreadsCount);
            newRequestEvent.Set();		//и запустим заново цикл обработки новых запросов
        }
    }
}
