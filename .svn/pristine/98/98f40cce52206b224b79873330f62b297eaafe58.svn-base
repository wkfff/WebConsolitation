using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;
using System.IO;

namespace Krista.FM.Client.MDXExpert.Common
{
    /// <summary>
    /// Обертка на AdomdCommand, может вести журнал выполняемых запросов
    /// </summary>
    public class MDXCommand
    {
        private AdomdCommand _adomdCommand;
        private bool _isKeepLog;
        private string _logPath;

        public MDXCommand()
        {
            this.ADOMDCommand = new AdomdCommand();
        }

        public MDXCommand(string commandText)
        {
            this.ADOMDCommand = new AdomdCommand(commandText);
        }

        public MDXCommand(string commandText, AdomdConnection connection)
        {
            this.ADOMDCommand = new AdomdCommand(commandText, connection);
        }

        public void ExecuteNonQuery()
        {
            this.ExecuteNonQuery(string.Empty, null);
        }

        public void ExecuteNonQuery(string commandText)
        {
            this.ExecuteNonQuery(commandText, null);
        }

        public void ExecuteNonQuery(string commandText, AdomdConnection connection)
        {
            this.Execute(commandText, connection, true);
        }

        public CellSet Execute()
        {
            return this.Execute(string.Empty, null);
        }

        public CellSet Execute(string commandText)
        {
            return this.Execute(commandText, null);
        }

        public CellSet Execute(string commandText, AdomdConnection connection)
        {
            return this.Execute(commandText, connection, false);
        }

        public CellSet Execute(string commandText, AdomdConnection connection, bool isNonQuery)
        {
            this.ADOMDCommand.CommandText = commandText;
            if (connection != null)
                this.ADOMDCommand.Connection = connection;

            if (this.IsKeepLog)
                this.AddLogText(commandText);

            object result = null; 
            
            if (isNonQuery)
                this.ADOMDCommand.ExecuteNonQuery();
            else
                result = this.ADOMDCommand.Execute();

            //Если результат не равен нулл и является CellSet вернем его
            if ((result != null) && (result is CellSet))
                return (CellSet)result;
            else
                return null;
        }

        /// <summary>
        /// Вести журнал запросов, сохранять их в указанном файле
        /// </summary>
        /// <param name="logPath"></param>
        public void KeepLog(string logPath)
        {
            if (logPath != string.Empty)
            {
                this.IsKeepLog = true;
                this.LogPath = logPath;
            }
        }

        /// <summary>
        /// Удалить лог
        /// </summary>
        public void DeleteLog()
        {
            if (this.LogPath != string.Empty)
            {
                if (File.Exists(this.LogPath))
                    File.Delete(this.LogPath);
            }
        }

        /// <summary>
        /// Добавить текст в лог
        /// </summary>
        /// <param name="text"></param>
        private void AddLogText(string text)
        {
            const string separator = "==================================";
            if (this.LogPath != string.Empty)
            {
                using (StreamWriter writer = new StreamWriter(this.LogPath, true, Encoding.GetEncoding("Windows-1251")))
                {
                    writer.WriteLine(string.Empty);
                    writer.WriteLine(DateTime.Now.ToString() + separator);
                    writer.WriteLine(text);
                    writer.WriteLine(separator);
                }
            }
        }

        /// <summary>
        /// Вести журнал запросов
        /// </summary>
        public bool IsKeepLog
        {
            get { return _isKeepLog; }
            set { _isKeepLog = value; }
        }

        /// <summary>
        /// Путь к файлу с журналом запросов
        /// </summary>
        public string LogPath
        {
            get { return _logPath; }
            set { _logPath = value; }
        }

        public AdomdCommand ADOMDCommand
        {
            get { return _adomdCommand; }
            set { _adomdCommand = value; }
        }
    }
}
