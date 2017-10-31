using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Krista.Diagnostics
{
	/// <summary>
    /// Трассировщик сообщений, выводящий протоктол в файл
    /// </summary>
	public class MultiFileTraceListener : TextWriterTraceListener
    {
		private DateTime? wtiterInitializedTime;
		private TimeSpan? historyAge;
		private TimeSpan[] newFileAtTime;

		private readonly string fileMask = String.Empty;
		private readonly Regex fileMaskRegex;
		private string dateFormat = "yyyyMMdd-HHmmss";
		private bool deleteOldFile;
		private Encoding encoding;
		private string currentLogFileName = string.Empty;

		private bool attributesInitialized;
		string delimiter = ";";
		//string _secondaryDelim = ",";

		protected override string[] GetSupportedAttributes()
		{
			return new string [] { "newFileAtTime", "historyAge", "delimiter", "encoding", "dateFormat", "deleteOldFile" };
		}

        /// <summary>
        /// Основной конструктор
        /// </summary>
		/// <param name="initializeData">Маска файла</param>
		public MultiFileTraceListener(string initializeData)
        {
			fileMask = initializeData;
			fileMaskRegex = new Regex(string.Format(@".*{0}([^\\])*\.log", fileMask.Replace(@"\", @"\\")), RegexOptions.Compiled | RegexOptions.IgnoreCase);
			Trace.Listeners.Add(this);
		}

        /// <summary>
        /// Вывод трассировочного сообщения
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
			Init();
			base.TraceEvent(eventCache, source, eventType, id);
			// Закомментировано для оптимизации
			//Writer.Flush();
        }
        
		/// <summary>
        /// Вывод трассировочного сообщения
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
			TraceEvent(eventCache, source, eventType, id, message, null);
		}
        
		/// <summary>
        /// Вывод трассировочного сообщения
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, format, args))
				return;

			lock (this)
			{
				Init();
				StringBuilder sb = new StringBuilder();
				WriteEscaped(sb, source);
				sb.Append(delimiter);
				WriteEscaped(sb, eventType.ToString());
				sb.Append(delimiter);
				WriteEscaped(sb, (args != null) ? String.Format(format, args) : format);
				sb.Append(delimiter);
				if (!String.IsNullOrEmpty(dateFormat))
				{
					WriteEscaped(sb, DateTime.Now.ToString(dateFormat));
				}
				WriteLine(sb.ToString());
				// Закомментировано для оптимизации
				//Flush();
			}

		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			TraceData(eventCache, source, eventType, id, data, null);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			throw new NotImplementedException();
			//base.TraceData(eventCache, source, eventType, id, data);
		}

		private static void WriteEscaped(StringBuilder sb, string message)
		{
			if (!String.IsNullOrEmpty(message))
			{
				sb.Append("\"");
				int index;
				int lastindex = 0;
				while ((index = message.IndexOf('"', lastindex)) != -1)
				{
					sb.Append(message, lastindex, index - lastindex);
					sb.Append("\"\"");
					lastindex = index + 1;
				}

				sb.Append(message, lastindex, message.Length - lastindex);
				sb.Append("\"");
			}
		}

		private void Init()
		{
			try
			{
				if (!attributesInitialized)
				{
					if (Attributes.ContainsKey("delimiter"))
					{
						delimiter = Attributes["delimiter"];
						if (string.IsNullOrEmpty(delimiter))
						{
							delimiter = ",";
						}
						else if (delimiter.ToLower() == "tab")
						{
							delimiter = "\t";
						}
					}

					if (Attributes.ContainsKey("encoding"))
					{
						try
						{
							encoding = Encoding.GetEncoding(Attributes["encoding"]);
						}
						catch (ArgumentException)
						{
						}
					}
					if (encoding == null) encoding = Encoding.GetEncoding("utf-8");

					if (Attributes.ContainsKey("dateFormat"))
					{
						dateFormat = Attributes["dateFormat"];
					}

					if (Attributes.ContainsKey("historyAge"))
					{
						try
						{
							historyAge = TimeSpan.Parse(Attributes["historyAge"]);
						}
						catch
						{
							historyAge = null;
						}
					}

					if (Attributes.ContainsKey("newFileAtTime"))
					{
						List<TimeSpan> timespanList = new List<TimeSpan>();
						foreach (string ts in Attributes["newFileAtTime"].Split(new char[] { ',' }))
						{
							try
							{
								timespanList.Add(TimeSpan.Parse(ts.Trim()));
							}
							catch
							{
							}
						}
						newFileAtTime = timespanList.ToArray();
					}

					if (Attributes.ContainsKey("deleteOldFile"))
					{
						try
						{
							deleteOldFile = Convert.ToBoolean(Attributes["deleteOldFile"]);
						}
						catch
						{
							deleteOldFile = false;
						}
					}

					attributesInitialized = true;
				}

				//Проверка необходимости создания нового лог-файла
				//Новый лог-файл создается если текущее время больше, чем одно из времени указанного
				//в перечне, а сам файл был создан ранее этого времени
				if ((wtiterInitializedTime != null) && (newFileAtTime != null))
				{
					foreach (TimeSpan checkTimeSpan in newFileAtTime)
					{
						DateTime checkTime = DateTime.Today.Add(checkTimeSpan);
						if (
							//Время наступило
							(checkTime.CompareTo(DateTime.Now) < 0) &&
							//Файл был создан до этого времени
							(wtiterInitializedTime.Value.CompareTo(checkTime) < 0)
							)
						{
							//Закрываем предыдущий лог, и выходим из цикла
							Writer.Close();
							Writer = null;
							wtiterInitializedTime = null;
							break;
						}
					}
				}

				//Если время создания лога не указано, значит он и не был создан
				if (wtiterInitializedTime == null)
				{
					string fileNameRoot = String.IsNullOrEmpty(fileMask) ? "logfile" : fileMask;
					Uri logfileUri = new Uri(
						new Uri(AppDomain.CurrentDomain.SetupInformation.ApplicationBase), string.Format(@"{0}.{1}.log", fileNameRoot, DateTime.Now.ToString("yyyyMMdd-HHmmss-FFFF")));

					currentLogFileName = logfileUri.LocalPath;

					string logDirectory = Path.GetDirectoryName(currentLogFileName);

					//Удаление из логов старых файлов
					if (historyAge != null)
					{
						foreach (string fileName in Directory.GetFiles(logDirectory))
						{
							//Файл соответсвует маске создаваемых файлов
							if (fileMaskRegex.Match(fileName).Success)
							{
								//Файл более старый, чем определено временем хранения
								if (File.GetCreationTime(fileName).Add(historyAge.Value).CompareTo(DateTime.Now) < 0 && deleteOldFile)
								{
									try
									{
										File.Delete(fileName);
									}
									catch
									{
									}
								}
							}
						}
					}

					Writer = new StreamWriter(currentLogFileName, true, encoding);
					wtiterInitializedTime = DateTime.Now;
				}
			}
			catch
			{
				//Все исключительные ситуации блокируем.
				//работа логера не должна сказываться на работоспособности системы
			}
		}
    }
}