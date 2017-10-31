using System;
using System.ComponentModel;
using Krista.FM.Common;
using Microsoft.AnalysisServices;

namespace Krista.FM.Server.OLAP.Processor
{
     public delegate void BatchComplitedDelegate(object sender, RunWorkerCompletedEventArgs e);

    /// <summary>
    /// Класс подключается к серверу SSAS2005, генерирует и отправляет скрипт для расчета.
    /// </summary>
    public class BatchProcessor : DisposableObject
    {
        private readonly Microsoft.AnalysisServices.Server server;

        private BackgroundWorker backgroundWorker;

        public event BatchComplitedDelegate BatchComplitedEvent;

        public BatchProcessor(Microsoft.AnalysisServices.Server server)
        {
            this.server = server;
        }

        public string StartSync(string script)
        {
            Process(script);
            return server.SessionID;
        }

        public void StartAsync(string script)
        {            
            InitWorker();
            backgroundWorker.RunWorkerAsync(script);
        }

        public string SessionId
        {
            get
            {
                if (server.Connected)
                {
                    return server.SessionID;
                }

                return string.Empty;
            }
        }

        private void InitWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += processWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += processWorker_RunWorkerCompleted;
        }

        private void processWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {   
            if (BatchComplitedEvent != null)
            {
                BatchComplitedEvent(this, e);
            }            
            if (e.Cancelled)
            {                
                //MessageBox.Show("Отменили!!!");
            }
            else
            {                
                //MessageBox.Show("Рассчитал!!!");
            }
        }

        public void CancelProcess()
        {
            server.CancelCommand(SessionId);
        }

        private void processWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Process((string)e.Argument);
        }

        /// <summary>
        /// Собственно выполнение скрипта на сервере SSAS2005.
        /// </summary>
        /// <param name="script">Скрипт, который необходимо выполнить.</param>
        private void Process(string script)
        {
            try
            {
                server.Execute(script);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                throw new Exception(e.Message);
            }
        }        

        // Implement the override Dispose method that will contain common cleanup functionality
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (server != null && server.Connected)
                {
                    server.Disconnect();
                    server.Dispose();
                }

                if (backgroundWorker != null)
                {
                    backgroundWorker.Dispose();
                    backgroundWorker = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
