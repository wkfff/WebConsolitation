using System;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Grid.Style;

namespace Krista.FM.Client.MDXExpert.Grid.UserInterface
{
    /// <summary>
    /// Интерфейс работы пользователя с гридом
    /// </summary>
    public interface IGridUserInterface
    {
        #region behavoir(поведение)

        /// <summary>
        /// Автоматический расчет высоты ячеек у строк
        /// </summary>
        bool AutoSizeRows { get; set; }

        /// <summary>
        /// Задержка до появления комментария
        /// </summary>
        int CommentDisplayDelay { get; set; }

        /// <summary>
        /// Отображать комментарии
        /// </summary>
        bool IsDisplayComments { get; set;}

        /// <summary>
        /// Максимальная ширина комментария
        /// </summary>
        int CommentMaxWidth { get; set;}

        /// <summary>
        /// Отображать комментарий к ячейке, до ее смены
        /// </summary>
        bool DisplayCommentUntilControlChange { get; set;}

        /// <summary>
        /// Стиль отображения комментария
        /// </summary>
        CellStyle CommentStyle { get; set; }
        #endregion

        #region layout(расположение)

        /// <summary>
        /// Расположение таблицы(ее начальные координаты на форме)
        /// </summary>
        Point GridLocation { get; set; }
        /// <summary>
        /// Высота разделителя между областями таблицы
        /// </summary>
        int OriginalSeparatorHeight { get; set; }

        #endregion

        #region appearance(вид)

        /// <summary>
        /// Цвет выделенной ячейки
        /// </summary>
        Color HighLightColor { get; set; }

        //заголовки фильтров
        /// <summary>
        /// Шрифт заголовков фильтров
        /// </summary>
        Font FilterCaptionsFont {get; set; }
        /// <summary>
        /// Цвет фона заголовков фильтров
        /// </summary>
        Color FilterCaptionsStartBackColor {get; set; }
        /// <summary>
        /// Цвет фона заголовков фильтров
        /// </summary>
        Color FilterCaptionsEndBackColor { get; set; }
        /// <summary>
        /// Цвет текста заголовков фильтров
        /// </summary>
        Color FilterCaptionsForeColor { get; set; }
        /// <summary>
        /// Цвет границы заголовков фильтров
        /// </summary>
        Color FilterCaptionsBorderColor { get; set; }

        /// <summary>
        /// Шрифт значений фильтров
        /// </summary>
        Font FilterValuesFont { get; set; }
        /// <summary>
        /// Цвет фона значений фильтров
        /// </summary>
        Color FilterValuesStartBackColor { get; set; }
        /// <summary>
        /// Цвет фона значений фильтров
        /// </summary>
        Color FilterValuesEndBackColor { get; set; }
        /// <summary>
        /// Цвет текста значений фильтров
        /// </summary>
        Color FilterValuesForeColor { get; set; }
        /// <summary>
        /// Цвет границы значений фильтров
        /// </summary>
        Color FilterValuesBorderColor { get; set; }

        /// <summary>
        /// Видимость заголовков фильтров
        /// </summary>
        bool FilterCaptionsVisible { get; set; }
        /// <summary>
        /// Максимально количество элементов отображаемое в подсках к фильтрам
        /// </summary>
        int TipDisplayMaxMemberCount { get; set; }
        /// <summary>
        /// Максимальное количество элементов отображаемое в значении фильтра
        /// </summary>
        DisplayMemberCount DisplayMemberCount { get; set; }
        /// <summary>
        /// Показывать родителей в заголовках элементов фильтра
        /// </summary>
        bool IsCaptionIncludeParents { get; set; }
        //заголовки строк
        /// <summary>
        /// Шрифт заголовков строк
        /// </summary>
        Font RowCaptionsFont {get; set; }
        /// <summary>
        /// Цвет фона заголовков строк
        /// </summary>
        Color RowCaptionsStartBackColor { get; set; }
        /// <summary>
        /// Цвет фона заголовков строк
        /// </summary>
        Color RowCaptionsEndBackColor { get; set; }
        /// <summary>
        /// Цвет текста заголовков строк
        /// </summary>
        Color RowCaptionsForeColor { get; set; }
        /// <summary>
        /// Цвет границы заголовков строк
        /// </summary>
        Color RowCaptionsBorderColor { get; set; }
        /// <summary>
        /// Стиль заголовков строк
        /// </summary>
        CellStyle RowCaptionsStyle { get; set; }

        //заголовки колонок
        /// <summary>
        /// Шрифт заголовков колонок
        /// </summary>
        Font ColumnCaptionsFont {get; set; }
        /// <summary>
        /// Цвет фона заголовков колонок
        /// </summary>
        Color ColumnCaptionsStartBackColor { get; set; }
        /// <summary>
        /// Цвет фона заголовков колонок
        /// </summary>
        Color ColumnCaptionsEndBackColor { get; set; }
        /// <summary>
        /// Цвет текста заголовков колонок
        /// </summary>
        Color ColumnCaptionsForeColor { get; set; }
        /// <summary>
        /// Цвет границы заголовков колонок
        /// </summary>
        Color ColumnCaptionsBorderColor { get; set; }
        /// <summary>
        /// Стиль заголовков колонок
        /// </summary>
        CellStyle ColumnCaptionsStyle { get; set; }

        //заголовки показателей
        /// <summary>
        /// Шрифт заголовков показателей
        /// </summary>
        Font MeasureCaptionsFont {get; set; }
        /// <summary>
        /// Цвет фона заголовков показателей
        /// </summary>
        Color MeasureCaptionsStartBackColor { get; set; }
        /// <summary>
        /// Цвет фона заголовков показателей
        /// </summary>
        Color MeasureCaptionsEndBackColor { get; set; }
        /// <summary>
        /// Цвет текста заголовков показателей
        /// </summary>
        Color MeasureCaptionsForeColor { get; set; }
        /// <summary>
        /// Цвет границы заголовков показателей
        /// </summary>
        Color MeasureCaptionsBorderColor { get; set; }
        /// <summary>
        /// Стиль заголовков показателей
        /// </summary>
        CellStyle MeasureCaptionsStyle { get; set; }

        //ось строк
        /// <summary>
        /// Шрифт оси строк
        /// </summary>
        Font RowAxisFont {get; set; }
        /// <summary>
        /// Цвет фона оси строк
        /// </summary>
        Color RowAxisStartBackColor { get; set; }
        /// <summary>
        /// Цвет фона оси строк
        /// </summary>
        Color RowAxisEndBackColor { get; set; }
        /// <summary>
        /// Цвет текста оси строк
        /// </summary>
        Color RowAxisForeColor { get; set; }
        /// <summary>
        /// Цвет границы оси строк
        /// </summary>
        Color RowAxisBorderColor { get; set; }
        /// <summary>
        /// Стиль оси строк
        /// </summary>
        CellStyle RowAxisStyle { get; set; }

        //ось колонок
        /// <summary>
        /// Шрифт оси колонок
        /// </summary>
        Font ColumnAxisFont {get; set; }
        /// <summary>
        /// Цвет фона оси колонок
        /// </summary>
        Color ColumnAxisStartBackColor { get; set; }
        /// <summary>
        /// Цвет фона оси колонок
        /// </summary>
        Color ColumnAxisEndBackColor { get; set; }
        /// <summary>
        /// Цвет текста оси колонок
        /// </summary>
        Color ColumnAxisForeColor { get; set; }
        /// <summary>
        /// Цвет границы оси колонок
        /// </summary>
        Color ColumnAxisBorderColor { get; set; }
        /// <summary>
        /// Стиль оси колонок
        /// </summary>
        CellStyle ColumnAxisStyle { get; set; }

        //область данных
        /// <summary>
        /// Шрифт области данных
        /// </summary>
        Font DataAreaFont { get; set; }
        /// <summary>
        /// Цвет фона области данных
        /// </summary>
        Color DataAreaBackColor { get; set; }
        /// <summary>
        /// Цвет текста области данных
        /// </summary>
        Color DataAreaForeColor { get; set; }
        /// <summary>
        /// Цвет границы в области данных
        /// </summary>
        Color DataAreaBorderColor { get; set; }
        /// <summary>
        /// Стиль области данных
        /// </summary>
        CellStyle DataAreaStyle { get; set; }
        /// <summary>
        /// Шрифт итогов в области данных
        /// </summary>
        Font DataAreaTotalsFont { get; set; }
        /// <summary>
        /// Цвет фона итогов в области данных
        /// </summary>
        Color DataAreaTotalsBackColor { get; set; }
        /// <summary>
        /// Цвет текста итогов в области данных
        /// </summary>
        Color DataAreaTotalsForeColor { get; set; }
        /// <summary>
        /// Стиль итогов в области данных
        /// </summary>
        CellStyle DataTotalsAreaStyle { get; set; }
        /// <summary>
        /// Цвет фона фиктивных ячеек в области данных
        /// </summary>
        Color DataAreaDummyBackColor { get; set; }
        /// <summary>
        /// Цвет текста фиктивных ячеек в области данных
        /// </summary>
        Color DataAreaDummyForeColor { get; set; }
        /// <summary>
        /// Стиль фиктивных ячеек в области данных
        /// </summary>
        CellStyle DataDummyAreaStyle { get; set; }
        /// <summary>
        /// Условная раскраска
        /// </summary>
        ColorRuleCollection ColorRules { get; set; }

        /// <summary>
        /// Шрифт имени, свойства элемента оси строк
        /// </summary>
        Font RowMemberPropertiesNameFont { get; set; }
        /// <summary>
        /// Шрифт значения, свойства элемента оси строк
        /// </summary>
        Font RowMemberPropertiesValueFont { get; set; }
        /// <summary>
        /// Цвет имени, свойства элемента оси строк
        /// </summary>
        Color RowMemberPropertiesNameForeColor { get; set; }
        /// <summary>
        /// Цвет значения, свойства элемента оси строк
        /// </summary>
        Color RowMemberPropertiesValueForeColor { get; set; }
        /// <summary>
        /// Стиль свойства элемента оси строк
        /// </summary>
        CellStyle RowMemberPropertiesStyle { get; set; }

        /// <summary>
        /// Шрифт имени, свойства элемента оси столбцов
        /// </summary>
        Font ColumnMemberPropertiesNameFont { get; set; }
        /// <summary>
        /// Шрифт значения, свойства элемента оси столбцов
        /// </summary>
        Font ColumnMemberPropertiesValueFont { get; set; }
        /// <summary>
        /// Цвет имени, свойства элемента оси столбцов
        /// </summary>
        Color ColumnMemberPropertiesNameForeColor { get; set; }
        /// <summary>
        /// Цвет значения, свойства элемента оси столбцов
        /// </summary>
        Color ColumnMemberPropertiesValueForeColor { get; set; }
        /// <summary>
        /// Стиль свойства элемента оси колонок
        /// </summary>
        CellStyle ColumnMemberPropertiesStyle { get; set; }
        /// <summary>
        /// Масштаб таблицы
        /// </summary>
        int GridPercentScale { get; set; }
        #endregion

        #region События
        event EventHandler RecalculatedGrid;
        event EventHandler GridSizeChanged;
        event Krista.FM.Client.MDXExpert.Grid.ExpertGrid.ObjectSelectedHandler ObjectSelected;
        event Krista.FM.Client.MDXExpert.Grid.ExpertGrid.SortClickHandler SortClick;
        event Krista.FM.Client.MDXExpert.Grid.ExpertGrid.DropButtonClickHandler DropButtonClick;
        #endregion

        #region Экспорт
        void ExportToWorkbook(string bookPath, string sheetName, bool isPrintVersion, bool isSeparateProperties);
        #endregion
    }
}
