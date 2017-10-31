using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // Модуль шаблонов наиболее употребимых функций этапа предпросмотра

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {
        /// <summary>
        /// Функция получения данных для предварительного просмотра
        /// </summary>
        /// <param name="dir">Каталог с файлами</param>
        protected virtual void PreviewFilesData(DirectoryInfo dir)
        {

        }

        /// <summary>
        /// Шаблон для функции предпросмотра данных источника "Год"
        /// </summary>
        protected void PreviewDataYTemplate()
        {
            // Список каталогов с годами
            DirectoryInfo[] dir_years = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
            if (dir_years.GetLength(0) == 0)
            {
                throw new PumpDataFailedException(string.Format(
                    "В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));
            }

            // Обходим все каталоги и обрабатываем файлы
            for (int i = 0; i < dir_years.GetLength(0); i++)
            {
                this.DataSource = null;
                if (!dir_years[i].Exists) continue;

                int sourceYear;
                try
                {
                    sourceYear = Convert.ToInt32(dir_years[i].Name);
                }
                catch
                {
                    this.DataPumpProtocol.WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeCriticalError, this.PumpID, this.SourceID,
                        "Год источника задан некорректно.");
                    continue;
                }

                // Добавляем источник
                SetDataSource(ParamKindTypes.Year, string.Empty, sourceYear, 0, string.Empty, 0, string.Empty);

                // Пишем в протокол о начале закачки из источника
                //this.DataPumpProtocol.WriteEventIntoDataPumpProtocol(
                //    DataPumpEventKind.dpeStart, this.PumpID, this.SourceID,
                //    string.Format("Старт закачки из источника {0} (ID источника {1}).",
                //        dir_years[i].FullName, this.SourceID));
                WriteToTrace(string.Format("Старт закачки из источника {0}.", dir_years[i].FullName), 
                    TraceMessageKind.Information);

                try
                {
                    // Проверяем, есть здесь что качать
                    if (dir_years[i].GetFiles("*.*", SearchOption.AllDirectories).GetLength(0) == 0)
                    {
                        throw new DataSourceIsCorruptException(string.Format(
                            "Каталог {0} пуст.", dir_years[i].FullName));
                    }

                    // Обработка файлов источника
                    PreviewFilesData(dir_years[i]);

                    //this.DataPumpProtocol.WriteEventIntoDataPumpProtocol(
                    //    DataPumpEventKind.dpeSuccefullFinished, this.PumpID, this.SourceID,
                    //    string.Format("Закачка из источника {0} успешно закончена.", dir_years[i].FullName));
                    WriteToTrace(string.Format("Закачка из источника {0} успешно закончена.", dir_years[i].FullName),
                        TraceMessageKind.Information);
                }
                catch (ThreadAbortException)
                {
                    //this.DataPumpProtocol.WriteEventIntoDataPumpProtocol(
                    //    DataPumpEventKind.dpeFinishedWithErrors, this.PumpID, this.SourceID, string.Format(
                    //        "Закачка из источника {0} закончена с ошибками: операция прервана пользователем.",
                    //        dir_years[i].FullName));

                    throw;
                }
                catch (Exception ex)
                {
                    //this.DataPumpProtocol.WriteEventIntoDataPumpProtocol(
                    //    DataPumpEventKind.dpeFinishedWithErrors, this.PumpID, this.SourceID,
                    //    string.Format("Закачка из источника {0} закончена с ошибками: {1}. \nДанные не сохранены.",
                    //        dir_years[i].FullName, ex.Message));

                    this.DataSourcesProcessingResult.AddToProcessedSources(this.SourceID, ex.Message);

                    continue;
                }
                finally
                {
                    this.DataSource = null;
                    CollectGarbage();
                }
            }
        }
    }
}