#define KristaDiagnostics

using System;
using System.Diagnostics;
using System.Configuration;
using System.Globalization;
using System.Runtime.Remoting;
using System.Security.Permissions;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.AppServer
{
    /*
    public class MyTrackingHandler : ITrackingHandler
    {

        #region ITrackingHandler Members

        public void DisconnectedObject(object obj)
        {
            Trace.TraceWarning("DisconnectedObject: obj = {0}", obj);
        }

        public void MarshaledObject(object obj, ObjRef or)
        {
            Trace.TraceWarning("MarshaledObject: obj = {0}; or = {1}", obj, or.TypeInfo);
        }

        public void UnmarshaledObject(object obj, ObjRef or)
        {
            if (obj.GetType().FullName == "Krista.FM.Common.ClientSession" ||
                obj.GetType().FullName == "Krista.FM.Server.Common.Session")
            {
                return;
            }
            Trace.TraceWarning("UnmarshaledObject: obj = {0}; or = {1}", obj.GetType(), or.TypeInfo);
        }

        #endregion
    }
    */
	/// <summary>
	/// Консольное приложение, которое создает и инициализирует 
	/// основной объект сервера приложений IServer.
	/// </summary>
	public sealed class AppServerClass
	{
		private AppServerClass()
		{
			
		}

        static void GCStatistic()
        {
            Trace.TraceEvent(TraceEventType.Verbose, "TotalMemory   = {0}", GC.GetTotalMemory(true));
            Trace.TraceEvent(TraceEventType.Verbose, "CollectionCount(0) = {0}", GC.CollectionCount(0));
            Trace.TraceEvent(TraceEventType.Verbose, "CollectionCount(1) = {0}", GC.CollectionCount(1));
            Trace.TraceEvent(TraceEventType.Verbose, "CollectionCount(2) = {0}", GC.CollectionCount(2));
        }

        /// <summary>
		/// Точка входа приложения.
		/// </summary>
		[MTAThread]
		[SecurityPermission(SecurityAction.LinkDemand)]
		public static void Main()
		{
			Trace.TraceEvent(TraceEventType.Critical, "TraceCritical");
			Trace.TraceEvent(TraceEventType.Error, "TraceError");
			Trace.TraceEvent(TraceEventType.Warning, "TraceWarning");
			Trace.TraceEvent(TraceEventType.Information, "TraceInformation");
			Trace.TraceEvent(TraceEventType.Verbose, "TraceVerbose");

            System.Threading.Thread.CurrentThread.Name = AppDomain.CurrentDomain.FriendlyName;

            System.Collections.Specialized.NameValueCollection appSettings = 
                ConfigurationManager.AppSettings;

#if !KristaDiagnostics
            // вывод в консоль
            Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Out, "ServerConsoleLog"));

            // вывод в текстовый лог в кодировке win1251
            TextWriter wr = new StreamWriter(appSettings["ServerLog"], true, Encoding.GetEncoding(1251));
            TextWriterTraceListener textWriter = new TextWriterTraceListener(wr, "ServerLog");
            Trace.Listeners.Add(textWriter);

			Trace.AutoFlush = true;
#endif
            Trace.TraceEvent(TraceEventType.Information, "----------------------------------------------------------------------");
            Trace.TraceEvent(TraceEventType.Information, "Server startup at " + DateTime.Now.ToString());

            try
            {
                // URL объекта сервера приложений.
                string URL = String.Format(CultureInfo.InvariantCulture, "tcp://{0}:{1}/FMServer/Server.rem", Environment.MachineName, appSettings["ServerPort"]);

				// Настройка среды .NET Remoting
                RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);

#if CUSTOM_REMOUTING
                int port = Convert.ToInt32(appSettings["ServerPort"]);
                TcpChannel serverChannel = new TcpChannel(port);
                ChannelServices.RegisterChannel(serverChannel, true);
                Trace.WriteLine(String.Format("Port number {0}", port));
#endif

                //TrackingServices.RegisterTrackingHandler(new MyTrackingHandler());

                Stopwatch sw = new Stopwatch();
                sw.Start();
                // Необходимо чтобы серверный объект создавался и настраивался при запуске
                // сервера приложений, а не при первом обращении клиента. 
                // Именно это здесь и делаем - создаем удаленный объект средствами .NET Remoting
                IServer Server = (IServer)Activator.GetObject(typeof(IServer), URL);

                // Прокси объект сервера создан. Теперь надо произвести первое обащение
                // к удаленному объекту для того, чтобы он реально был создан на стороне сервера,
                // :) т.е. здесь же в текущем домене :)
                // Собственно делаем первое обащение к объекту сервера
                Trace.Indent();
                Server.Activate();
                Trace.Unindent();

                // А вот теперь серверный объект создан и ни куда не денется от нас,
                // если конечно мы ему корректно настроим таймаут
                sw.Stop();
                Trace.TraceEvent(TraceEventType.Information, "Инициализация сервера завершена. Затрачено {0} мс", sw.ElapsedMilliseconds);

                //DisposableObject.ShowTrace = true;

                // Собственно преостанавливаем выполнение текущего треда
                // Надо бы сделать какой-нибудь цикл с ожиданием событий, но пока похуй на это
                Console.ReadLine();

                DisposableObject.ShowTrace = false;

                Trace.TraceEvent(TraceEventType.Information, "Server shutdown at " + DateTime.Now.ToString());

                GCStatistic();

                Server.Dispose();
                Server = null;

                Trace.TraceEvent(TraceEventType.Verbose, "Сбор мусора");
                Trace.Indent();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GCStatistic();
                Trace.Unindent();
                Trace.TraceEvent(TraceEventType.Verbose, "Сбор мусора завершен.");
            }
            catch (Exception e)
            {
                Trace.TraceEvent(TraceEventType.Critical, "Ошибка запуска сервера: {0}", e);
                throw;
            }
            finally
            {
            	foreach (TraceListener listener in System.Diagnostics.Trace.Listeners)
            	{
					listener.Flush();
            		listener.Close();
            	}
#if !KristaDiagnostics
                Trace.WriteLine("Удаление TextWriterTraceListener");
                Trace.Indent();
                Trace.Listeners.Remove(textWriter);
                textWriter.Close();
                textWriter = null;
                Trace.Unindent();
                Trace.WriteLine("TextWriterTraceListener удален");
#endif
            }
        }
	}
}
