using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Grid
{
    class GridConsts
    {
        //Цветовые константы для раскраски таблицы
        static public Color gridRowsBackColorStart = Color.FromArgb(228, 236, 247);
        static public Color gridRowsBackColorEnd = Color.FromArgb(228, 236, 247);

        static public Color gridColumnsBackColorStart = gridRowsBackColorStart;
        static public Color gridColumnsBackColorEnd = gridRowsBackColorEnd;

        static public Color gridCaptionsBackColorStart = Color.FromArgb(228, 236, 247);
        static public Color gridCaptionsBackColorEnd = Color.FromArgb(228, 236, 247);

        static public Color gridCommonBorderColor = Color.FromArgb(158, 182, 206);
        static public Color gridAxisForeColor = Color.FromArgb(0, 0, 0);
        static public Color gridMesuresForeColor = Color.FromArgb(0, 0, 0);

        //отклонение от реальных координат ячейки, начиная с которого возможно ее перетаскивание 
        //(появление курсора - сплитера)
        public const int boundsDeflection = 5;

        /// <summary>
        /// Наименование ячейки итога
        /// </summary>
        public const string totalCaption = "Итоги";

        /// <summary>
        /// Если название элеменента из селсета пустое, будем писать в нем empty
        /// </summary>
        public const string empty = "(Пусто)";

        /// <summary>
        /// Заголовок для элемента "Среднее значение"
        /// </summary>
        public const string average = "Среднее значение";

        /// <summary>
        /// Заголовок для элемента "Медиана"
        /// </summary>
        public const string median = "Медиана";

        /// <summary>
        /// Заголовок для элемента "Стандартное отклонение"
        /// </summary>
        public const string stdev = "Стандартное отклонение";


        /// <summary>
        /// Не правильное значение в ячейке
        /// </summary>
        public const string errorValue = "#ЗНАЧ!";
        
        /// <summary>
        /// значение в ячейке - результат деления на 0
        /// </summary>
        public const string errorDivZero = "#ДЕЛ/0!";

        /// <summary>
        /// Обший итог
        /// </summary>
        public const string grandTotal = "grandTotal";
        /// <summary>
        /// Итог
        /// </summary>
        public const string total = "total";
        /// <summary>
        /// Элемент измерения
        /// </summary>
        public const string member = "member";
        /// <summary>
        /// Схлопнутый элемент измерения
        /// </summary>
        public const string collapsedMember = "collapsed_member";
        /// <summary>
        /// Дубликат
        /// </summary>
        public const string duplicate = "duplicate";

        //
        //Именование узлов в Xml
        //

        /// <summary>
        /// свойства таблицы
        /// </summary>
        public const string gridPropertys = "gridPropertys";
        /// <summary>
        /// рисовальщик
        /// </summary>
        public const string painter = "painter";
        /// <summary>
        /// заголовок таблицы
        /// </summary>
        public const string gridCaption = "gridCaption";
        /// <summary>
        /// заголовки фильтров
        /// </summary>
        public const string filtersCaptions = "filtersCaptions";
        /// <summary>
        /// заголовки колонок
        /// </summary>
        public const string columnsCaptions = "columnsCaptions";
        /// <summary>
        /// заголовки строк
        /// </summary>
        public const string rowsCaptions = "rowsCaptions";
        /// <summary>
        /// строки
        /// </summary>
        public const string rows = "rows";
        /// <summary>
        /// колонки
        /// </summary>
        public const string columns = "columns";
        /// <summary>
        /// заголовки мер
        /// </summary>
        public const string measuresCaptions = "measuresCaptions";
        /// <summary>
        /// область данных
        /// </summary>
        public const string measuresData = "measuresData";
        /// <summary>
        /// комментарии
        /// </summary>
        public const string comments = "comments";
        /// <summary>
        /// дополнительные свойства
        /// </summary>
        public const string addinPropertys = "addinPropertys";
        /// <summary>
        /// стиль
        /// </summary>
        public const string style = "style";
        /// <summary>
        /// стиль имен MemberProperties
        /// </summary>
        public const string memPropNameStyle = "memPropNameStyle";
        /// <summary>
        /// стиль значений MemberProperties
        /// </summary>
        public const string memPropValueStyle = "memPropValueStyle";
        /// <summary>
        /// состояние раскрытости элементов оси
        /// </summary>
        public const string stateMembersExpand = "stateMembersExpand";
        /// <summary>
        /// стиль итогов
        /// </summary>
        public const string totalsStyle = "totalsStyle";
        /// <summary>
        /// стиль фиктивных ячеек мер
        /// </summary>
        public const string dummyStyle = "dummyStyle";
        /// <summary>
        /// стиль опциональня ячейки заголовка
        /// </summary>
        public const string optionalCellStyle = "optionalCellStyle";
        /// <summary>
        /// цвета
        /// </summary>
        public const string colors = "colors";
        /// <summary>
        /// шрифт
        /// </summary>
        public const string font = "font";
        /// <summary>
        /// свойства отдельного элемента
        /// </summary>
        public const string propertys = "propertys";
        /// <summary>
        /// формат строки
        /// </summary>
        public const string stringFormat = "stringFormat";

        //
        //Именование атрибутов в Xml
        //

        /// <summary>
        /// начальный цвет фона
        /// </summary>
        public const string backColorStart = "backColorStart";
        /// <summary>
        /// конечный цвет фона
        /// </summary>
        public const string backColorEnd = "backColorEnd";
        /// <summary>
        /// цвет бордюра
        /// </summary>
        public const string borderColor = "borderColor";
        /// <summary>
        /// цвет шрифта
        /// </summary>
        public const string foreColor = "foreColor";
        /// <summary>
        /// цвет шрифта
        /// </summary>
        public const string gradient = "gradient";
        /// <summary>
        /// подсветка выделенных кнопок
        /// </summary>
        public const string highlightColor = "highlightColor";
        /// <summary>
        /// шрифт конвертированный в строку
        /// </summary>
        public const string sfont = "sfont";
        /// <summary>
        /// размеры элементов
        /// </summary>
        public const string sizes = "size";
        /// <summary>
        /// высота элемента
        /// </summary>
        public const string widths = "widths";
        /// <summary>
        /// ширина элементов
        /// </summary>
        public const string heights = "heights";
        /// <summary>
        /// интервал времени
        /// </summary>
        public const string timerInterval = "timerInterval";
        /// <summary>
        /// максимальная ширина комментариев
        /// </summary>
        public const string maxWidth = "maxWidth";
        /// <summary>
        /// отображать
        /// </summary>
        public const string isDisplay = "isDisplay";
        /// <summary>
        /// показывать
        /// </summary>
        public const string visible = "visible";
        /// <summary>
        /// отображать до смены контрола
        /// </summary>
        public const string displayUntilConrolChange = "displayUntilConrolChange";
        /// <summary>
        /// выравнивание (по вертикале)
        /// </summary>
        public const string vAligment = "vAligment";
        /// <summary>
        /// выравнивание (по горизонтале)
        /// </summary>
        public const string hAligment = "hAligment";
        /// <summary>
        /// обрезание строки
        /// </summary>
        public const string trimming = "trimming";
        /// <summary>
        /// дополнительные флаги форматирования строки
        /// </summary>
        public const string formatFlags = "formatFlags";
        /// <summary>
        /// значения чего либо
        /// </summary>
        public const string value = "value";
        /// <summary>
        /// автоматичекий расчет высоты ячеек у строк
        /// </summary>
        public const string autoSizeRows = "autoSizeRows";
        /// <summary>
        /// высота разделителя между областями таблицы
        /// </summary>
        public const string separatorHeight = "separatorHeight";
        /// <summary>
        /// текст
        /// </summary>
        public const string text = "text";
        /// <summary>
        /// пропорции
        /// </summary>
        public const string proportion = "proportion";
        /// <summary>
        /// место расположения
        /// </summary>
        public const string place = "place";
        /// <summary>
        /// максимально количество элементов отображаемое в подсках к фильтрам
        /// </summary>
        public const string tipDisplayMaxMemberCount = "tipDisplayMaxMemberCount";
        /// <summary>
        /// Количество отображаемых элементов в значении фильтра
        /// </summary>
        public const string displayMemberCount = "displayMemberCount";
        /// <summary>
        /// отображать предков в заголовках элементов фильтра
        /// </summary>
        public const string isCaptionIncludeParents = "isCaptionIncludeParents";
        /// <summary>
        /// имя меры
        /// </summary>
        public const string measureName = "measureName";
        /// <summary>
        /// условие для правила раскраски
        /// </summary>
        public const string colorRuleCondition = "condition";
        /// <summary>
        /// значение 1 для правила раскраски
        /// </summary>
        public const string colorRuleValue1 = "value1";
        /// <summary>
        /// значение 2 для правила раскраски
        /// </summary>
        public const string colorRuleValue2 = "value2";
        /// <summary>
        /// область применения правила раскраски
        /// </summary>
        public const string colorRuleArea = "area";
        /// <summary>
        /// правило раскраски
        /// </summary>
        public const string colorRule = "colorRule";
        /// <summary>
        /// коллекция правил раскраски
        /// </summary>
        public const string colorRules = "colorRules";
        /// <summary>
        /// масштаб таблицы
        /// </summary>
        public const string gridScale = "scale";
        /// <summary>
        /// значение масштаба
        /// </summary>
        public const string scaleValue = "value";
        /// <summary>
        /// выделенные ячейки
        /// </summary>
        public const string selectedCells = "selectedCells";
        /// <summary>
        /// идентификатор выделенной ячейки
        /// </summary>
        public const string selectedCellItem = "item";

    }
}
