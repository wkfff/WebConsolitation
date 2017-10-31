using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.iMonitoringWM.Common
{
    /// <summary>
    /// Режим работы приложения
    /// </summary>
    public enum WorkMode
    {
        /// <summary>
        /// Запрашиваем данные с сервера
        /// </summary>
        Online,
        /// <summary>
        /// Используем данные из кэша
        /// </summary>
        Cache
    }

    /// <summary>
    /// Виды отображения отчетов
    /// </summary>
    public enum ReportViewMode
    {
        Original,
        Vertical,
        Horizontal
    }

    /// <summary>
    /// Состояние отчета
    /// </summary>
    public enum ReportViewState
    {
        NeedUpdate,
        LoadedOfCache,
        LoadedOfWeb,
        FailedLoad,
        LoadedOfCacheAndFailedLoadWeb
    }

    /// <summary>
    /// Размер экрана устройства
    /// </summary>
    public enum ScreenSizeMode
    {
        s240x320,
        s480x640,
        s480x800,
        unsupportedSize,
    }

    /// <summary>
    /// Место размещения кэша
    /// </summary>
    public enum CachePlace
    {
        storagePhone,
        storageCard
    }

    /// <summary>
    /// Кнопки телефона
    /// </summary>
    public enum HardwareButtons
    {
        Up,
        Left,
        Right,
        Bottom,
        Center,
        None
    }
}
