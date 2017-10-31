using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Krista.FM.Client.MDXExpert.Common
{
    public struct Consts
    {
        /* Временные поля для подключения */
        public static string tmpCatalogName = "";
        public static string tmpServerName = "";
        //версия сервера
        public static string tmpConnectTo = "8.0";
        public static string tmpTimeout = string.Empty;
        public static string tmpPassword = string.Empty;
        public static string tmpUserID = string.Empty;
        public static string tmpProvider = "MSOLAP";

        public static string TmpConnStr
        {
            get
            {
                if (string.IsNullOrEmpty(tmpUserID) || string.IsNullOrEmpty(tmpPassword))
                    return
                        String.Format(
                            "Provider={4};Initial Catalog={0};MDX Unique Name Style=2;MDX Compatibility=2;Client Cache Size=25;Auto Synch Period=0;Data Source={1};ConnectTo={2};Timeout={3};",
                            tmpCatalogName, tmpServerName, tmpConnectTo, tmpTimeout, tmpProvider);
                else
                    return
                        String.Format(
                            "Provider={6};Password={0};User ID={1};Initial Catalog={2};MDX Unique Name Style=2;MDX Compatibility=2;Client Cache Size=25;Auto Synch Period=0;Data Source={3};ConnectTo={4};Timeout={5};",
                            tmpPassword, tmpUserID, tmpCatalogName, tmpServerName, tmpConnectTo, tmpTimeout, tmpProvider);
            }
        }

        public static string ReadConnectionString(string file)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(file))
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "[oledb]")
                            continue;
                        if (line == "; Everything after this line is an OLE DB initstring")
                            continue;

                        if (line.Contains("Provider"))
                            return line;
                    }
                    throw new Exception("Неверная строка подключения.");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);
                throw new Exception(e.Message, e);
            }
            return "";
        }

        /// <summary>
        /// Уровень All
        /// </summary>
        public const string allLevel = "[(All)]";

        /// <summary>
        /// Наименование общего итога
        /// </summary>
        public const string grandTotalCaption = "Общие итоги";

        /// <summary>
        /// Сообщение об отсутствии подключения
        /// </summary>
        public const string disconnection = " - нет подключения";

        /// <summary>
        /// Разделитель имен элементов измерения (160 - это неразрывный отдел)
        /// </summary>
        public static string memberNameSeparate = ((Char)160).ToString() + "-" + ((Char)160).ToString();

        /// <summary>
        /// Название столбца объектов для таблицы - источника данных карты
        /// </summary>
        public const string objectsColumn = "Объекты";

        /// <summary>
        /// Имя легенды для списка объектов карты
        /// </summary>
        public const string objectList = "Список объектов";

        /// <summary>
        /// Название столбца для кода объекта карты
        /// </summary>
        public const string objCodeColumn = "Код";

        /// <summary>
        /// Имя заголовка по умолчанию для новой вычисляемой меры
        /// </summary>
        public const string defaultTotalCaption = "Новый итог";

        /// <summary>
        /// Имя свойства для кода объекта на карте 
        /// </summary>
        public const string mapObjectCode = "Код объекта карты"; //"Идентификационный номер";//

        /// <summary>
        /// Имя свойства для имени объекта на карте 
        /// </summary>
        public const string mapObjectName = "Краткое наименование"; //"Сокращенное наименование";//
        /// <summary>
        /// Имя столбца в таблице данных для карты для краткого наименования объекта 
        /// </summary>
        public const string mapObjectShortName = "Краткое наименование"; 



        #region Текстовые сообщения
        /// <summary>
        /// Предупреждение показывается пользователю в случае использования сортировки
        /// с разрушением иерархии в таблицах
        /// </summary>
        public const string sortWarning = "Данный вид сортировки не рекомендуется использовать в таблицах, это\n" +
            "может повлечь некорректное отображение данных. Применить сортировку?";
        #endregion

        #region Путь к данным приложения
        /// <summary>
        /// Имя журнала запросов к многомерке
        /// </summary>
        public const string queryLogName = "Query.txt";

        /// <summary>
        /// Имя папки с журналами
        /// </summary>
        public const string logFolderName = "Log";

        /// <summary>
        /// Имя папки с шаблонами карт
        /// </summary>
        public const string mapsFolderName = "ESRIMaps";

        /// <summary>
        /// Имя текущего подключения
        /// </summary>
        public static string connectionName = "MAS.udl";



        /// <summary>
        /// Путь к данным приложений пользователя
        /// </summary>
        static public string UserDataPath
        {
            get
            {
                string result = string.Empty;
                string[] str = Application.UserAppDataPath.Split('\\');
                for (int i = 0; i < str.Length - 3; i++)
                {
                    result += str[i] + "\\";
                }
                return result;
            }
        }


        /// <summary>
        /// Путь к данным приложения
        /// </summary>
        static public string ConnectionFolderPath
        {
            //get { return Application.CommonAppDataPath + "\\Connections"; }
            get
            {
                return String.Format("{0}\\{1}",
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Krista\\MDX Эксперт 3\\Connections");
            }
        }

        /// <summary>
        /// Вернет путь к данным MDXExperta локального пользователя
        /// </summary>
        static public string UserAppDataPath
        {
            get
            {
                return UserDataPath + "Krista\\FM\\MDXExpert3";
            }
        }

        /// <summary>
        /// Папка созданая приложением.
        /// </summary>
        static public string UserAppDataPathForDelete
        {
            get
            {
                return Application.UserAppDataPath.Remove(Application.UserAppDataPath.LastIndexOf(Application.ProductName) - 1);
            }
        }

        /// <summary>
        /// Путь к журналу приложения
        /// </summary>
        static public string UserAppLogPath
        {
            get
            {
                return UserAppDataPath + "\\" + logFolderName;
            }
        }

        /// <summary>
        /// Путь к журналу запросов приложения
        /// </summary>
        static public string UserAppQueryLogPath
        {
            get
            {
                return UserAppLogPath + "\\" + queryLogName;
            }
        }

        #endregion

        #region Информация о приложении
        /// <summary>
        /// Версия MDX Эксперта
        /// </summary>
        public const string applicationVersion = "3.14.0.0";
        /// <summary>
        /// Из скольких частей состоит версия
        /// </summary>
        public const int versionPartNumber = 2;
        /// <summary>
        /// Тип версии (тестовая или релиз)
        /// </summary>
        public const VersionType versionType = VersionType.Test;
        /// <summary>
        /// Версия формата
        /// </summary>
        public const int formatVersion = 1;
        /// <summary>
        /// Имя нового отчета
        /// </summary>
        public const string newReportName = "Отчет";
        /// <summary>
        /// Расширение отчета
        /// </summary>
        public const string reportExt = ".exd3";
        /// <summary>
        /// MIME Type
        /// </summary>
        public const string mimeType = "application/mdxexpert";
        /// <summary>
        /// Имя приложения
        /// </summary>
        public const string applicationName = "MDX Эксперт";
        /// <summary>
        /// Имя приложения с номером
        /// </summary>
        public const string applicationNameWithNumber = applicationName + " 3";
        /// <summary>
        /// Телефоны службы поддержки
        /// </summary>
        public const string departamentTelefons = "8-800-200-20-72\n8-4855-29-17-50";
        /// <summary>
        /// Информация отображающаяся в сплывающей подсказке у файлов с разрешением reportExt
        /// </summary>
        public const string reportInform = "Отчет MDX Эксперт 3";
        #endregion

        #region Ключи реестра
        /// <summary>
        /// Если асоциация была пользователем, требуется удалить данный узел, с 
        /// автоматически сгенерированными значениями
        /// </summary>
        public const string autoExtensionKey = "exd3_auto_file";
        /// <summary>
        /// Имя приложения, используется при ассоциации расширения
        /// </summary>
        public const string applicationNameRegKey = "MDXExpert3";
        /// <summary>
        /// Путь к журналу с запросами
        /// </summary>
        public const string queryLogPathRegKey = "QueryLogPath";
        /// <summary>
        /// Вести журнал запросов
        /// </summary>
        public const string isKeepQueryLogRegKey = "IsKeepQueryLog";
        /// <summary>
        /// Список последних использованных шаблонов
        /// </summary>
        public const string mruListKey = "MRUList";
        /// <summary>
        /// Отображать итоги по умолчанию
        /// </summary>
        public const string isHideTotalsByDefaultKey = "IsHideTotalsByDefault";
        /// <summary>
        /// Имя подключения
        /// </summary>
        public const string connectionNameKey = "ConnectionName";
        /// <summary>
        /// Путь к стилю приложения
        /// </summary>
        public const string styleSheetPathRegKey = "StyleSheetPath";
        /// <summary>
        /// Путь к репозиторию карт
        /// </summary>
        public const string mapRepositoryPathRegKey = "MapRepositoryPath";
        /// <summary>
        /// Имя шаблона карты
        /// </summary>
        public const string mapTemplateNameRegKey = "MapTemplateName";
        #endregion

        #region Именование узлов и атрибутов Xml
        /// <summary>
        /// Узел со свойствами
        /// </summary>
        public const string properties = "Properties";
        /// <summary>
        /// PivotData
        /// </summary>
        public const string pivotData = "PivotData";
        /// <summary>
        /// заголовок элемента
        /// </summary>
        public const string elementCaption = "ElementCaption";
        /// <summary>
        /// комментарий заголовка
        /// </summary>
        public const string elementComment = "ElementComment";
        /// <summary>
        /// Установленные свойства (диаграммы, таблицы)
        /// </summary>
        public const string presets = "Presets";
        /// <summary>
        /// Легенда
        /// </summary>
        public const string legend = "legend";
        /// <summary>
        /// Размещение индикаторов
        /// </summary>
        public const string gaugesLocation = "gaugesLocation";
        /// <summary>
        /// Данные для диаграммы, введенные пользователем вручную
        /// </summary>
        public const string sourceDT = "SourceDT";
        /// Уникальный ключ элемента
        /// </summary>
        public const string uniqName = "UniqueName";
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
        /// значения чего либо
        /// </summary>
        public const string value = "value";
        /// <summary>
        /// выравнивание (по вертикале)
        /// </summary>
        public const string vAligment = "vAligment";
        /// <summary>
        /// выравнивание (по горизонтале)
        /// </summary>
        public const string hAligment = "hAligment";
        /// <summary>
        /// отображать
        /// </summary>
        public const string isDisplay = "isDisplay";
        /// <summary>
        /// высота элемента
        /// </summary>
        public const string widths = "widths";
        /// <summary>
        /// ширина элементов
        /// </summary>
        public const string heights = "heights";
        /// <summary>
        /// шрифт конвертированный в строку
        /// </summary>
        public const string sfont = "sfont";
        /// <summary>
        /// цвет фона
        /// </summary>
        public const string backColor = "backColor";
        /// <summary>
        /// цвет текста
        /// </summary>
        public const string foreColor = "fontColor";
        /// <summary>
        /// цвет бордюров
        /// </summary>
        public const string borderColor = "borderColor";
        /// <summary>
        /// стиль бордюров
        /// </summary>
        public const string borderStyle = "borderStyle";
        /// <summary>
        /// заголовок элемента
        /// </summary>
        public const string titleElement = "titleElement";
        /// <summary>
        /// имя куба
        /// </summary>
        public const string cubeName = "cubeName";
        /// <summary>
        /// тип элемента отчета
        /// </summary>
        public const string reportElemetType = "reportElemetType";
        /// <summary>
        /// режим скрытия пустых
        /// </summary>
        public const string hideEmptyMode = "hideEmptyMode";
        /// <summary>
        /// активно ли что либо
        /// </summary>
        public const string isActive = "isActive";
        /// <summary>
        /// размер элемента
        /// </summary>
        public const string elementSize = "elementSize";
        /// <summary>
        /// обновляемость
        /// </summary>
        public const string isUpdatable = "isUpdatable";
        /// <summary>
        /// положение легенды композитной диаграммы
        /// </summary>
        public const string compositeLegendLocation = "CompositeLegendLocation";
        /// <summary>
        /// размер легенды композитной диаграммы
        /// </summary>
        public const string compositeLegendExtent = "CompositeLegendExtent";
        /// <summary>
        ///синхронизация элемента
        /// </summary>
        public const string synchronization = "sync";
        /// <summary>
        ///уникальное имя элемента к которому привязан элемент
        /// </summary>
        public const string boundTo = "boundTo";
        /// <summary>
        ///получать значения только из текущего столбца
        /// </summary>
        public const string isCurrentColumnValues = "isCurrentColumnValues";
        /// <summary>
        ///признак, что меры идут в строки(ряды) при синхронизации
        /// </summary>
        public const string measureInRows = "measureInRows";
        /// <summary>
        ///признак, что объекты карты берутся из строк таблицы при синхронизации
        /// </summary>
        public const string objectsInRows = "objectsInRows";
        /// <summary>
        /// тип стыковки элемента
        /// </summary>
        public const string elementDock = "elementDock";
        /// <summary>
        /// максимальное количестов строк на странице в таблице
        /// </summary>
        public const string tablePageSize = "tablePageSize";
        /// <summary>
        /// номер текущей страницы в таблице
        /// </summary>
        public const string tablePageNumber = "tablePageNumber";
        /// <summary>
        /// элементы, синхронизируемые с таблицей
        /// </summary>
        public const string anchoredElems = "anchoredElems";
        /// <summary>
        /// отложить обновление данных
        /// </summary>
        public const string deferDataUpdating = "deferDataUpdating";
        /// <summary>
        /// подсчитывать итоги только по видимым (выбраным) элементам
        /// </summary>
        public const string isVisualTotals = "isVisualTotals";
        /// <summary>
        /// признак того, что пользователь ввел МДХ запрос в ручную
        /// </summary>
        public const string isCustomMDX = "isCustomMDX";
        /// <summary>
        /// MDX запрос
        /// </summary>
        public const string mdxQuery = "mdxQuery";
        /// <summary>
        /// Разворачивать элементы оси
        /// </summary>
        public const string dynamicLoadData = "dynamicLoadData";
        /// <summary>
        /// сбрасывать ли при каждом получечении данных соединение с многомеркой
        /// </summary>
        public const string isResetAdomdConnection = "isResetAdomdConnection";
        /// <summary>
        /// отображать ли итог
        /// </summary>
        public const string isVisibleTotal = "isVisibleTotal";
        /// <summary>
        /// отображать ли дата мемберы
        /// </summary>
        public const string isVisibleDataMember = "isVisibleDataMember";
        /// <summary>
        /// Включать ли уровень в запрос, учитывается при динамической загрузке данных в таблице
        /// </summary>
        public const string isIncludeToQuery = "isIncludeToQuery";
        /// <summary>
        /// имя шаблона для карты
        /// </summary>
        public const string templateName = "templateName";
        /// <summary>
        /// Автоматический расчет интервалов на индикаторе
        /// </summary>
        public const string autoTickmarkCalculation = "autoTickmarkCalculation";
        /// <summary>
        /// тип источника данных
        /// </summary>
        public const string dataSourceType = "dataSourceType";
        /// <summary>
        /// максимальный размер значка для карты
        /// </summary>
        public const string symMaxSize = "symMaxSize";
        /// <summary>
        /// минимальный размер значка для карты
        /// </summary>
        public const string symMinSize = "symMinSize";
        /// <summary>
        /// пропорциональный размер значков на карте
        /// </summary>
        public const string isPropSymbolSize = "isPropSymbolSize";
        /// <summary>
        /// список показателей, к которым не применяется пропорциональный размер
        /// </summary>
        public const string notProportionalMeasures = "notProportionalMeasures";
        /// <summary>
        /// настройка слоев карты
        /// </summary>
        public const string mapLayers = "layers";
        /// <summary>
        /// секторные диаграммы на карте
        /// </summary>
        public const string mapPieCharts = "pieCharts";
        /// <summary>
        /// показывать/скрывать пустые имена объектов с пустыми данными
        /// </summary>
        public const string displayEmptyShapeNames = "displayEmptyShapeNames";
        /// <summary>
        /// метка в Экселе для последней строки экспортированного элемента
        /// </summary>
        public const string elementLastRow = "elementLastRow";
        /// <summary>
        /// тип подписи объектов на карте (код или наименование)
        /// </summary>
        public const string shapeCaptionType = "shapeCaptionType";
        /// <summary>
        /// ширина легенды
        /// </summary>
        public const string legendSize = "size";
        /// <summary>
        /// расположение легенды
        /// </summary>
        public const string legendLocation = "location";
        /// <summary>
        /// строка формата
        /// </summary>
        public const string formatString = "formatString";
        /// <summary>
        /// Тип отображения единиц измерения
        /// </summary>
        public const string unitDisplayType = "unitDisplayType";
        /// <summary>
        /// Цвет легенды
        /// </summary>
        public const string legendColor = "backColor";
        /// <summary>
        /// Цвет границы легенды
        /// </summary>
        public const string legendBorderColor = "borderColor";
        /// <summary>
        /// Шрифт легенды
        /// </summary>
        public const string legendFont = "font";
        /// <summary>
        /// Видимость легенды
        /// </summary>
        public const string legendVisible = "visible";
        /// <summary>
        /// Видимость значений интервалов в легенде
        /// </summary>
        public const string legendValuesVisible = "valuesVisible";
        /// <summary>
        /// настройка вида отображения интервалов в легенде
        /// </summary>
        public const string legendContent = "content";
        public const string legendContents = "legendContents";
        /// <summary>
        /// имя интервала
        /// </summary>
        public const string intervalName = "intervalName";
        /// <summary>
        /// цвет интервала
        /// </summary>
        public const string intervalColor = "color";
        /// <summary>
        /// начальное значение
        /// </summary>
        public const string startValue = "startValue";
        /// <summary>
        /// конечное значение
        /// </summary>
        public const string endValue = "endValue";
        /// <summary>
        /// Цветовой интервал
        /// </summary>
        public const string colorRange = "range";
        /// <summary>
        /// Цветовые интервалы
        /// </summary>
        public const string colorRanges = "colorRanges";
        /// <summary>
        /// Настройки среднего значения
        /// </summary>
        public const string averageSettings = "averageSettings";
        /// <summary>
        /// Настройки медианы
        /// </summary>
        public const string medianSettings = "medianSettings";
        /// <summary>
        /// Настройки k-первых
        /// </summary>
        public const string topCountSettings = "topCountSettings";
        /// <summary>
        /// Настройки k-последних
        /// </summary>
        public const string bottomCountSettings = "bottomCountSettings";
        /// <summary>
        /// Тип среднего значения
        /// </summary>
        public const string averageType = "averageType";
        /// <summary>
        /// Разделение среднего значения на выше/ниже
        /// </summary>
        public const string isAverageSeparate = "isLowHighSeparate";
        /// <summary>
        /// Расчет обычного отклонения от среднего
        /// </summary>
        public const string isAverageDeviationCalculate = "isAverageCalculate";
        /// <summary>
        /// Расчет медианы
        /// </summary>
        public const string isMedianCalculate = "isMedianCalculate";
        /// <summary>
        /// Раздление на выше и ниже значения
        /// </summary>
        public const string isLowerHigherSeparate = "isLowerHigherSeparate";
        /// <summary>
        /// Расчет стандартного отклонения
        /// </summary>
        public const string isStdDevCalculate = "isStdDevCalculate";
        /// <summary>
        /// Цвет - ниже среднего
        /// </summary>
        public const string lowerAverageColor = "lowerAverageColor";
        /// <summary>
        /// Цвет - выше среднего
        /// </summary>
        public const string higherAverageColor = "higherAverageColor";
        /// <summary>
        /// Цвет - ниже медианы
        /// </summary>
        public const string lowerMedianColor = "lowerMedianColor";
        /// <summary>
        /// Цвет - выше медианы
        /// </summary>
        public const string higherMedianColor = "higherMedianColor";
        /// <summary>
        /// Цвет - ниже границы отклонения
        /// </summary>
        public const string lowerDeviationColor = "lowerDeviationColor";
        /// <summary>
        /// Цвет - выше границы отклонения
        /// </summary>
        public const string higherDeviationColor = "higherDeviationColor";
        /// <summary>
        /// Расчет k-первых
        /// </summary>
        public const string isTopCountCalculate = "isTopCountCalculate";
        /// <summary>
        /// кол-во k-первых
        /// </summary>
        public const string topCount = "topCount";
        /// <summary>
        /// цвет k-первых
        /// </summary>
        public const string topCountColor = "topCountColor";
        /// <summary>
        /// Расчет k-последних
        /// </summary>
        public const string isBottomCountCalculate = "isBottomCountCalculate";
        /// <summary>
        /// кол-во k-последних
        /// </summary>
        public const string bottomCount = "bottomCount";
        /// <summary>
        /// цвет k-последних
        /// </summary>
        public const string bottomCountColor = "bottomCountColor";

        //минимальное значение интервала
        public static double minRangeValue = -9e23;
        //максимальное значение интервала
        public static double maxRangeValue = 9e23;



        #endregion
    }
}
