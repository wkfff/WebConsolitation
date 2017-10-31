using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Generic;

using Krista.FM.Common;
using Krista.FM.Server.DataPumpManagement;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpManagement
{
    /// <summary>
    /// Менеджер программ закачек
    /// </summary>
    public sealed class DataPumpManager : DisposableObject, IDataPumpManager
    {
        #region Поля

        private IScheme scheme;
        // Менеджер расписания запуска закачек
        private PumpScheduler pumpScheduler;
        private DataPumpInfo dataPumpInfo;

        private List<Process> runningProcesses = new List<Process>();

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Менеджер программ закачек
        /// </summary>
        /// <param name="scheme">Ссылка на интерфейс объекта схемы</param>
        public DataPumpManager(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (pumpScheduler != null) pumpScheduler.Dispose();
                if (dataPumpInfo != null) dataPumpInfo.Dispose();
                // на этом этапе уже не должно быть запущенных процессов закачки
                // если они таки есть - принудительно закрываем
                foreach (Process prc in runningProcesses)
                {
                    try
                    {
                        if (!prc.HasExited)
                        {
                            prc.EnableRaisingEvents = false;
                            prc.Kill();
                        }
                    }
                    catch { }
                }
                runningProcesses.Clear();
            }
        }

        #endregion Инициализация


        #region Реализация IDataPumpManager

        /// <summary>
        /// Проверяет наличие модулей закачик по указанному пути
        /// </summary>
        /// <param name="path">Путь</param>
        private bool CheckDataPumpModules(string path)
        {
            return Directory.GetFiles(path, "Krista.FM.Server.DataPumps*.dll").GetLength(0) > 0;
        }

        /// <summary>
        /// Возвращает путь к хосту программ закачки
        /// </summary>
        /// <returns>путь к хосту программ закачки</returns>
        private string GetDataPumpPath()
        {
            System.Collections.Specialized.NameValueCollection appSettings = ConfigurationManager.AppSettings;
            string path = appSettings["DataPumpPath"];

            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                return path.TrimEnd('\\') + "\\";
            }

            return AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\";
        }

        /// <summary>
        /// Обработчик корректного завершения процесса закачки (т.е. если он сам закрылся, по собственной воле)
        /// </summary>
        /// <param name="sender">Процесс закачки</param>
        /// <param name="e"></param>
        private void PumpProcessExitHandler(object sender, EventArgs e)
        {
            // если вызван не процессом закачки - ничего не делаем
            if (!(sender is Process))
                return;
            // удаляем процесс из нашего внутренненго списка
            Process prc = sender as Process;
            if (runningProcesses.Contains(prc))
                runningProcesses.Remove(prc);
            // отцепляем обработчик события чтобы объект мог спокойно умереть
            prc.Exited -= new EventHandler(PumpProcessExitHandler);
        }

        /// <summary>
        /// Создание, загрузка и запуск программы закачки
        /// </summary>
        /// <param name="programIdentifier">Идентификатор программы звкачки данных</param>
        /// <param name="startState">Стартовое состояние закачки</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private string StartPumpProgram(string programIdentifier, PumpProcessStates startState, int pumpID, int sourceID, string userParams)
        {
            try
            {
                System.Collections.Specialized.NameValueCollection appSettings = ConfigurationManager.AppSettings;
                string url = String.Format("tcp://{0}:{1}/FMServer/Server.rem", Environment.MachineName, 
                    appSettings["ServerPort"]);

                string dataPumpPath = GetDataPumpPath();
                if (!Directory.Exists(dataPumpPath))
                {
                    throw new Exception(string.Format("Каталог с программами закачки {0} не найден.", dataPumpPath));
                }
                if (!CheckDataPumpModules(dataPumpPath))
                {
                    throw new Exception(string.Format("В каталоге {0} модули закачки не найдены.", dataPumpPath));
                }

                ProcessStartInfo psi = new ProcessStartInfo();

                if (startState == PumpProcessStates.DeleteData)
                {
                    psi.Arguments = string.Format(
                        "\"{0}\" \"{1}\" {2} {3} {4} {5}",
                        url, this.scheme.Name, programIdentifier, startState, pumpID, sourceID);
                }
                else
                {
                    psi.Arguments = string.Format(
                        "\"{0}\" \"{1}\" {2} {3} \"{4}\"",
                        url, this.scheme.Name, programIdentifier, startState, userParams);
                }

                psi.FileName = dataPumpPath + "Krista.FM.Server.DataPumpHost.exe";
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                psi.WorkingDirectory = dataPumpPath;

                Process prc = Process.Start(psi);
                // запоминаем процесс во вспомогательном списке чтобы можно было 
                // его принудительно закрыть в случае перезагрузки сервиса
                prc.Exited += new EventHandler(PumpProcessExitHandler);
                prc.EnableRaisingEvents = true;
                runningProcesses.Add(prc);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Создание, загрузка и запуск программы закачки
        /// </summary>
        /// <param name="programIdentifier">Идентификатор программы звкачки данных</param>
        /// <param name="startState">Стартовое состояние закачки</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string StartPumpProgram(string programIdentifier, PumpProcessStates startState, string userParams)
        {
            return StartPumpProgram(programIdentifier, startState, -1, -1, userParams);
        }

        /// <summary>
        /// Запуск удаления закачанных данных
        /// </summary>
        /// <param name="programIdentifier">Идентификатор программы звкачки данных</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <returns>Результат выполнения</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string DeleteData(string programIdentifier, int pumpID, int sourceID)
        {
            return StartPumpProgram(programIdentifier, PumpProcessStates.DeleteData, pumpID, sourceID, string.Empty);
        }

        /// <summary>
        /// Интерфейс менеджера запуска закачек по расписанию
        /// </summary>
        public IPumpScheduler PumpScheduler
        {
            get
            {
                return pumpScheduler;
            }
        }

        /// <summary>
        /// Запускает шедулер
        /// </summary>
        public void StartScheduler()
        {
            pumpScheduler.StartScheduler();
        }

        /// <summary>
        /// Интерфейс информационной части закачки
        /// </summary>
        public IDataPumpInfo DataPumpInfo
        {
            get
            {
                return dataPumpInfo;
            }
            set
            {
                dataPumpInfo = value as DataPumpInfo;
            }
        }

        /// <summary>
        /// Инициализация полей
        /// </summary>
        public void Initialize()
        {
            this.pumpScheduler = new PumpScheduler(scheme);
            this.dataPumpInfo = new DataPumpInfo(scheme);
            this.dataPumpInfo.Initialize();
        }

        #endregion Реализация IDataPumpManager
    }
}