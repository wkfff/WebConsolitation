using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.MobileReports.Common
{
    /// <summary>
    /// Режим скачивания скиптов
    /// </summary>
    public enum ScriptsDownloadType
    {
        NotDownload,
        DownloadSource,
        Custom
    }

    /// <summary>
    /// Состояние процесса обновления данных
    /// </summary>
    public enum UpdateState
    {
        /// <summary>
        /// Готов к обновлению
        /// </summary>
        ReadyForUpdate,
        /// <summary>
        /// Загрузка настроек
        /// </summary>
        LoadSettings,
        /// <summary>
        /// Запроc параметров отчетов у базы данных
        /// </summary>
        QueryReportsParams,
        /// <summary>
        /// Генерация отчетов
        /// </summary>
        BootloadReports,
        /// <summary>
        /// Формируем дамп базы данной для PHP сервиса
        /// </summary>
        BuildDataBaseDump,
        /// <summary>
        /// Архивация пакета данных
        /// </summary>
        ArchiveDataBurst,
        /// <summary>
        /// Загрузка пакета, на удаленныей сервер
        /// </summary>
        UploadDataBurst,
        /// <summary>
        /// Разворачивание пакета на удаленом сервере
        /// </summary>
        RollOutOnDistHost,
        /// <summary>
        /// Синхронизация даты обновления отчетов в базе данных с отчетами на удаленом хосте
        /// </summary>
        SynchronizeDeployDate,
        /// <summary>
        /// Конец обновления данных
        /// </summary>
        FinishUpdateData
    }

    /// <summary>
    /// Режимы генерации отчетов
    /// </summary>
    public enum MobileReportsSnapshotMode
    {
        /// <summary>
        /// New - новый режим сохранения отчета, когда после загрузки каждый отчет архивируется. 
        /// </summary>
        New,
        /// <summary>
        /// Old - старый режим сохраениния отчета, отчеты не архивируются, нужен для совместимостей старых версий.
        /// </summary>
        Old,
        /// <summary>
        /// Both - генерация и идет сразу в двух режимах. Те что генерятся по новой схеме помещаются 
        ///	в NewModeSnapshot, старые соответственно в OldModeSnapshot
        /// </summary>
        Both
    }
}
