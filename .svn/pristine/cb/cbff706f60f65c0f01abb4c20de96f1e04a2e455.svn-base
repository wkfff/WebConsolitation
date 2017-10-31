using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;

using Krista.FM.Common;

namespace Krista.FM.Server.Common
{
    //
    // Generic AsyncReplyHelperSink class - delegates calls to
    // SyncProcessMessage to delegate instance passed in ctor.
    internal class AsyncReplyHelperSink : IMessageSink
    {
        // Define a delegate to the callback method.
        public delegate IMessage AsyncReplyHelperSinkDelegate(IMessage msg);

        IMessageSink _NextSink;
        AsyncReplyHelperSinkDelegate _delegate;

        public IMessageSink NextSink
        {
            get
            {
                return _NextSink;
            }
        }

        public AsyncReplyHelperSink(IMessageSink next,
                                     AsyncReplyHelperSinkDelegate d)
        {
            _NextSink = next;
            _delegate = d;
        }

        public virtual IMessage SyncProcessMessage(IMessage msg)
        {
            if (_delegate != null)
            {
                // Notify delegate of reply message. The delegate
                // can modify the message, so save the result and 
                // pass it down the chain.
                IMessage msg2 = _delegate(msg);
                return _NextSink.SyncProcessMessage(msg2);
            }
            else
            {
                return new ReturnMessage(
                    new Exception(
                        "AsyncProcessMessage _delegate member is null!"),
                        (IMethodCallMessage)msg);
            }
        }

        public virtual IMessageCtrl AsyncProcessMessage(
            IMessage msg,
            IMessageSink replySink)
        {
            // This should not be called in the reply sink chain. The
            // runtime processes reply messages to asynchronous calls
            // synchronously. Someone must be trying to use us in a
            // different chain!
            return null;
        }
    }

    internal class ExceptionLoggingMessageSink : IMessageSink
    {
        IMessageSink _NextSink;
        static FileStream _stream;

        public IMessageSink NextSink
        {
            get
            {
                return _NextSink;
            }
        }

        public ExceptionLoggingMessageSink(IMessageSink next,
                                            string filename)
        {
            Trace.TraceVerbose("ExceptionLoggingMessageSink ctor");
            _NextSink = next;
            try
            {
                lock (this)
                {
                    if (_stream == null)
                    {
                        _stream = new FileStream(filename,
                                                FileMode.Append);
                    }
                }
            }
            catch (IOException e)
            {
                Trace.TraceError(e.Message);
            }
        }

        public IMessage SyncProcessMessage(IMessage msg)
        {
            try
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    bool showTrace = true;
                    try
                    {
                        //
                        object o = msg.Properties["__CallContext"];
                        LogicalCallContext lcc = (LogicalCallContext)o;
                        LogicalCallContextData lccd = lcc.GetData("Authorization") as LogicalCallContextData;
                        if (lccd != null)
                        {
                            Session session = lccd["Session"] as Session;
                            if (session != null)
                            {
                                showTrace = session.Principal.Identity.Name != "SYSTEM";
                                if (showTrace)
                                    Console.WriteLine("{0} {1}", session.Principal.Identity.Name, session.SessionId);
                            }
                            else
                                Console.WriteLine("Нет сессии");
                        }
                        else
                            Console.WriteLine("Нет авторизации");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Нет сессии {0}", e.Message);
                    }
                    if (showTrace)
                        TraceObjectClall(msg);
                }
                finally
                {
                    Console.ResetColor();
                }

                // Pass to next sink.
                IMessage retmsg = _NextSink.SyncProcessMessage(msg);

                // Inspect return message and log an exception if needed.
                InspectReturnMessageAndLogException(retmsg);

                return retmsg;
            }
            catch 
            {
                return null;
            }
        }

        static void TraceObjectClall(IMessage msg)
        {
            if (msg.GetType().ToString() == "System.Runtime.Remoting.Messaging.ConstructorCallMessage")
            {
                ConstructionCall cc = new ConstructionCall(msg);
                Trace.TraceVerbose("Конструктор {0}",cc.ActivationType);
            }
            else if (msg.GetType().ToString() == "System.Runtime.Remoting.Messaging.Message")
            {
                MethodCallMessageWrapper mcmw = new MethodCallMessageWrapper((IMethodCallMessage)msg);
                List<string> args = new List<string>();
                for (int i = 0; i < mcmw.InArgCount; i++)
                {
                    string val = Convert.ToString(mcmw.InArgs[i]);
                    args.Add(String.Format("{0} {1}", ((Type[])mcmw.MethodSignature)[i].Name, val));
                }
                Trace.TraceVerbose("Метод {0}.{1}({2})", 
                    mcmw.MethodBase.ReflectedType.FullName, mcmw.MethodName,
                    String.Join(", ", args.ToArray()));
                

            }
            else if (msg.GetType().ToString() == "System.Runtime.Remoting.Messaging.MethodCall")
            {
                MethodCallMessageWrapper mcmw = new MethodCallMessageWrapper((IMethodCallMessage)msg);
                List<string> args = new List<string>();
                for (int i = 0; i < mcmw.InArgCount; i++)
                {
                    try
                    {
                        string val = Convert.ToString(mcmw.InArgs[i]);
                        args.Add(String.Format("{0} {1}", ((Type[])mcmw.MethodSignature)[i].Name, val));
                    }
                    catch (Exception e)
                    {
                        args.Add(e.Message);
                    }
                }
                Trace.TraceVerbose("Метод {0}.{1}({2})",
                    mcmw.MethodBase.ReflectedType.FullName, mcmw.MethodName,
                    String.Join(", ", args.ToArray()));
            }   
            else
            {
                Trace.TraceVerbose("Это что-то неизвестное.");
            }
        }

        static void InspectReturnMessageAndLogException(IMessage retmsg)
        {
            MethodReturnMessageWrapper mrm =
               new MethodReturnMessageWrapper((IMethodReturnMessage)retmsg);

            if (mrm.Exception != null)
            {
                lock (_stream)
                {
                    Exception e = mrm.Exception;
                    StreamWriter w = new StreamWriter(_stream,
                                                      Encoding.ASCII);
                    w.WriteLine();
                    w.WriteLine("========================");
                    w.WriteLine();
                    w.WriteLine(String.Format("Exception: {0}",
                                              DateTime.Now));
                    w.WriteLine(String.Format("Application Name: {0}",
                                              e.Source));
                    w.WriteLine(String.Format("Method Name: {0}",
                                              e.TargetSite));
                    w.WriteLine(String.Format("Description: {0}",
                                              e.Message));
                    w.WriteLine(String.Format("More Info: {0}",
                                              e));
                    w.Flush();
                }
            }
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg,
                                            IMessageSink replySink)
        {
            try
            {
                //
                // Set up our reply sink with a delegate
                // to our callback method.
                AsyncReplyHelperSink.AsyncReplyHelperSinkDelegate rsd =
                    new AsyncReplyHelperSink.AsyncReplyHelperSinkDelegate(
                    this.AsyncProcessReplyMessage);

                // We want to trace the response when we get it, so add
                // a sink to the reply sink.
                replySink =
                    new AsyncReplyHelperSink(replySink,
                                             rsd);

                return _NextSink.AsyncProcessMessage(msg, replySink);
            }
            catch 
            {
                return null;
            }
        }

        //
        // Trace the reply message and return it.
        public IMessage AsyncProcessReplyMessage(IMessage msg)
        {
            // Inspect reply message and log an exception if needed.
            InspectReturnMessageAndLogException(msg);
            return msg;
        }

    }

    [Serializable]
    internal class ExceptionLoggingProperty : IContextProperty,
                                            IContributeClientContextSink,
                                            IContributeServerContextSink
    {
        private string _Name;

        IMessageSink _ServerSink;
        IMessageSink _ClientSink;
        string _FileName;

        public ExceptionLoggingProperty(string name, string FileName)
        {
            _Name = name;
            _FileName = FileName;
        }

        public void Freeze(Context
                             newContext)
        {
            // When this is called, we can't add any more properties
            // to the context.
        }

        public Boolean IsNewContextOK(Context newCtx)
        {
            return true;
        }

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public IMessageSink GetClientContextSink(IMessageSink nextSink)
        {
            Console.WriteLine("GetClientContextSink()");
            lock (this)
            {
                if (_ClientSink == null)
                {
                    _ClientSink =
                        new ExceptionLoggingMessageSink(nextSink,
                                                        _FileName);
                }
            }
            return _ClientSink;
        }

        public IMessageSink GetServerContextSink(IMessageSink nextSink)
        {
            Console.WriteLine("GetServerContextSink()");
            lock (this)
            {
                if (_ServerSink == null)
                {
                    _ServerSink =
                        new ExceptionLoggingMessageSink(nextSink,
                                                        _FileName);
                }
            }
            return _ServerSink;
        }
    } // End class ExceptionLoggingProperty

    [AttributeUsage(AttributeTargets.Class)]
    public class ExceptionLoggingContextAttribute : ContextAttribute
    {
        string _FileName;

        public ExceptionLoggingContextAttribute(string filepath)
            : base("ExceptionLoggingContextAttribute")
        {
            _FileName = filepath;
        }

        public override void GetPropertiesForNewContext(IConstructionCallMessage msg)
        {
            // Add our property to the context properties.
            msg.ContextProperties.Add(new ExceptionLoggingProperty(this.AttributeName, _FileName));
        }

        public override Boolean IsContextOK(Context ctx, IConstructionCallMessage msg)
        {
            // Does the context already have
            // an instance of this property?
            return (ctx.GetProperty(this.AttributeName) != null);
        }
    } // End class ExceptionLoggingContextAttribute

    [ExceptionLoggingContext(@"C:\exceptions.log")]
    public class SecurityContextBoundObject : 
        //ContextBoundObject
        DisposableObject
        //, IDisposable
    {
/*        private bool disposed = false;

        public SecurityContextBoundObject()
        {
        }

        ~SecurityContextBoundObject()
		{
            if (!this.disposed)
                Dispose(false);
        }

        /// <summary>
        /// Вызывается для детерменированного уничтожения объекта
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                // Вызываем метод, реально выполняющий очистку.
                Dispose(true);

                // Поскольку очистка объекта выполняется явно,
                // запрещаем сборщику мусора вызов метода Finalize
                //GC.SuppressFinalize(this);

                this.disposed = true;
            }
        }

        /// <summary>
        /// Этот открытый метод можно вызывать вместо Dispose
        /// </summary>
        public virtual void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Общий метод, реально выполняющий очистку.
        /// Его вызывают методы Finalize, Dispose и Close
        /// </summary>
        /// <param name="disposing">
        /// true - явное уничтожение/закрытие объекта; 
        /// false - неявное уничтожение при сборке мусора
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            //Trace.WriteLine("Dispose(" + Convert.ToString(disposing)  + ") вызван", "DisposableObject");
            // Синхронизируем потоки для запрета одновременного вызова Dispose/Close
            lock (this)
            {

                GC.SuppressFinalize(ListenerContainer.textWriter);
                //                if (ListenerContainer.textWriter != null)
                //                    if (Debug.Listeners.Contains(ListenerContainer.textWriter))
                //                    {
                //Debug.WriteLine(String.Format("~{0}({1})", GetType().FullName, disposing));
                //                    }
                //                    else
                //                        throw new Exception("Debug.Listeners.Contains(ListenerContainer.textWriter)");
                if (disposing)
                {
                    // Здесь еще можно обращаться к полям, ссылающимся 
                    // на другие объекты - это безопасно для кода, так как для
                    // этих объектов метод Finalize еще не вызван
                }
                else
                {
                }

                // Здесь должно выполняться уничножение/закрытие неупровляемых ресурсов
            }
            //Trace.WriteLine("Dispose(" + Convert.ToString(disposing) + ") завершон", "DisposableObject");
        }


        /// <summary>
        /// Возвращает true если объект уже был уничтожен
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public bool Disposed
        {
            get { return this.disposed; }
        }


        /// <summary>
        /// Инициализация срока лицензии
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            //ILease lease = (ILease)base.InitializeLifetimeService();
            //if (lease.CurrentState == LeaseState.Initial)
            //{
            //    lease.InitialLeaseTime = TimeSpan.FromSeconds(30);
            //    lease.RenewOnCallTime = TimeSpan.FromSeconds(5);
            //    lease.SponsorshipTimeout = TimeSpan.FromSeconds(3);
            //}
            //return lease;
            return null;
        }
        */
    }
}
