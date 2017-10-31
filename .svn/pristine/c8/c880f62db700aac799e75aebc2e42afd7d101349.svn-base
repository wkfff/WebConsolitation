using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    /// <summary>
    /// Разборщик текстовых файлов.
    /// </summary>
    public sealed class TXTSorcerer : DisposableObject
    {
        #region Константы

        private const int constMaxRows = 10000;
        private const int constMaxCols = 2000;

        // Процент разделителей в горизонтальной линии, по котрому можно судить, что это разделитель
        private const double constHorzMarginDelimsPercent = 0.95;
        // Процент разделителей в вертикальной линии, по котрому можно судить, что это разделитель
        private const double constVertMarginDelimsPercent = 0.5;
        // Процент разделителей в линии, по котрому можно судить, что это неполный разделитель
        private const double constShortMarginDelimsPercent = 0.1;

        // Элементы и атрибуты хмл-файла настроек
        private const string nodeGlobalSettings = "//GlobalSettings";
        private const string attrVertDelimiters = "VertDelimiters";
        private const string attrHorzDelimiters = "HorzDelimiters";
        private const string attrCharacterSet = "CharacterSet";
        private const string attrPartsWildCards = "PartsWildCards";
        private const string attrReportWildCards = "ReportWildCards";
        private const string attrPartsUnionType = "PartsUnionType";
        private const string attrExcludeFilesByString = "ExcludeFilesByString";

        private const string nodeFixedParams = "//FixedParams";
        private const string nodeFixedParam = ".//FixedParam";
        private const string attrFixedParam = "FixedParam";
        private const string attrName = "Name";
        private const string attrBeginPosX = "BeginPosX";
        private const string attrBeginPosY = "BeginPosY";
        private const string attrEndPosX = "EndPosX";
        private const string attrEndPosY = "EndPosY";
        private const string attrPartNo = "PartNo";
        private const string attrCaption = "Caption";
        private const string attrFixedParamKind = "FixedParamKind";

        private const string nodeTables = "//Tables";
        private const string attrTablesAmount = "TablesAmount";

        private const string nodeTable = "//Table";
        private const string attrLocation = "Location";

        private const string nodeTableColumns = ".//TableColumns";
        private const string nodeColumns = ".//Column";
        private const string attrDivisionKind = "DivisionKind";
        private const string attrType = "Type";
        private const string attrWidth = "Width";
        private const string attrDBField = "DBField";
        private const string attrNullable = "Nullable";
        private const string attrDefaultValue = "DefaultValue";
        private const string attrDataPrecision = "DataPrecision";
        private const string attrExcludedValues = "ExcludedValues";
        private const string attrExactCheck = "ExactCheck";

        private const string nodeTableEntry = ".//TableEntry";
        private const string attrEndTableMarker = "EndTableMarker";
        private const string attrHeaderAmidTable = "HeaderAmidTable";
        private const string attrTotalSumAcrossRow = "TotalSumAcrossRow";
        private const string attrHeaderHeight = "HeaderHeight";
        private const string attrIncludeEndTableMarker = "IncludeEndTableMarker";
        private const string attrInnerEndTableMarker = "InnerEndTableMarker";
        private const string attrTableDelimited = "TableDelimited";
        private const string attrEndTableMarkerSpace = "EndTableMarkerSpace";

        // Тип кодировки
        private const string cstANSI = "ANSI";
        private const string cstOEM = "OEM";

        // Виды фиксированных параметров
        private const string fxpReportDate = "ReportDate";
        private const string fxpReportDateEx = "ReportDateEx";
        private const string fxpFormNo = "FormNo";
        private const string fxpPeriod = "Period";
        private const string fxpTaxOrgan = "TaxOrgan";
        private const string fxpCls = "Cls";
        private const string fxpBalance = "Balance";
        private const string fxpAccount = "Account";
        private const string fxpOrganization = "Organization";
        private const string fxpCodeOnly = "CodeOnly";
        private const string fxpNameOnly = "NameOnly";

        // Тип переноса в ячейках столбца
        private const string tclNone = "None";
        private const string tclUpper = "Upper";
        private const string tclLower = "Lower";
        private const string tclDivisionSign = "DivisionSign";

        #endregion Константы


        #region Структуры

        #region Структуры данных хмл-настроек

        /// <summary>
        /// Тип объединения частей отчета
        /// </summary>
        private enum PartsUnionType
        {
            /// <summary>
            /// Oбъединение по горизонтальным границам файлов
            /// </summary>
            Horizontal,

            /// <summary>
            /// Oбъединение по вертикальным границам файлов
            /// </summary>
            Vertical,

            /// <summary>
            /// Неуказанный тип
            /// </summary>
            Unknown
        }


        /// <summary>
        /// Общие Параметры отчета
        /// </summary>
        private class GlobalSettings : DisposableObject
        {
            #region Инициализация

            /// <summary>
            /// Конструктор
            /// </summary>
            public GlobalSettings()
            {
                this.VertDelimiters = new List<int>(20);
                this.HorzDelimiters = new List<int>(20);
                this.ReportWildCards = new List<string>(20);
                this.PartsWildCards = new List<string>(20);
                this.ExcludeFilesByString = new List<string>(20);
            }

            /// <summary>
            /// Освобождение ресурсов
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.VertDelimiters != null)
                        this.VertDelimiters.Clear();
                    if (this.HorzDelimiters != null)
                        this.HorzDelimiters.Clear();
                    if (this.ReportWildCards != null)
                        this.ReportWildCards.Clear();
                    if (this.PartsWildCards != null)
                        this.PartsWildCards.Clear();
                    if (this.ExcludeFilesByString != null)
                        this.ExcludeFilesByString.Clear();
                }
                
                base.Dispose(disposing);
            }

            #endregion Инициализация


            /// <summary>
            /// вертикальный разделитель
            /// </summary>
            public List<int> VertDelimiters;

            /// <summary>
            /// горизонтальный разделитель
            /// </summary>
            public List<int> HorzDelimiters;

            /// <summary>
            /// Кодировка текста отчета
            /// </summary>
            public CharacterSet CharSet;

            /// <summary>
            /// Список масок файлов отчета
            /// </summary>
            public List<string> ReportWildCards;

            /// <summary>
            /// Маски файлов - частей отчета
            /// </summary>
            public List<string> PartsWildCards;

            /// <summary>
            /// Тип объединения частей отчета
            /// </summary>
            public PartsUnionType PartsUnionType;

            /// <summary>
            /// Если файл содержит одну из указанных строк, он будет пропущен
            /// </summary>
            public List<string> ExcludeFilesByString;
        }


        /// <summary>
        /// Тип фиксированного параметра
        /// </summary>
        private enum FixedParamKind
        {
            /// <summary>
            /// Располагается в единственном месте отчета.
            /// </summary>
            Single,

            /// <summary>
            /// Располагается непосредственно перед таблицей отчета (перед верхней границей). 
            /// В случае наличия в отчете нескольких таблиц таких параметров тоже может быть несколько.
            /// </summary>
            BeforeTable
        }


        /// <summary>
        /// Параметры, которые всегда находятся в одном и том же месте (даты и т.п.).
        /// Координаты
        /// </summary>
        private class FixedParamPos : DisposableObject
        {
            /// <summary>
            /// Название фиксированного параметра. Определяет его содержание и алгоритм обработки разборщиком.
            /// </summary>
            public string Name;

            /// <summary>
            /// начальная позиция по X
            /// </summary>
            public int BeginPosX;

            /// <summary>
            /// начальная позиция по Y
            /// </summary>
            public int BeginPosY;

            /// <summary>
            /// конечная позиция по X
            /// </summary>
            public int EndPosX;

            /// <summary>
            /// конечная позиция по Y
            /// </summary>
            public int EndPosY;

            /// <summary>
            /// Порядковый номер файла отчета, в котором находится параметр
            /// </summary>
            public int PartNo;

            /// <summary>
            /// Человеческое название параметра, выводимое в форме предварительного просмотра
            /// </summary>
            public string Caption;

            /// <summary>
            /// Тип фиксированного параметра
            /// </summary>
            public FixedParamKind FixedParamKind;

            /// <summary>
            /// Наименование поля в итоговой таблице, куда будет записано значение фиксированного параметра.
            /// </summary>
            public string DBField;

            /// <summary>
            /// Значение по умолчанию. Если оно определено, то в случае отсутствия значения 
            /// в параметре будет подставлено значение по умолчанию
            /// </summary>
            public string DefaultValue;
        }


        /// <summary>
        /// Таблица отчета
        /// </summary>
        private class Table : DisposableObject
        {
            #region Инициализация

            /// <summary>
            /// Конструктор
            /// </summary>
            public Table()
            {
                this.TableIndexes = new List<int>(20);
                this.Files = new List<string>(20);
                this.TableColumns = new TableColumns();
                this.TableEntry = new TableEntry();
            }

            /// <summary>
            /// Освобождение ресурсов
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.TableIndexes != null) this.TableIndexes.Clear();
                    if (this.Files != null) this.Files.Clear();
                }
                
                base.Dispose(disposing);
            }

            #endregion Инициализация


            /// <summary>
            /// Список индексов таблиц отчета (с нуля, через запятую), к которым относятся данные параметры
            /// </summary>
            public List<int> TableIndexes;

            /// <summary>
            /// Список масок файлов, к таблицам из которых относятся данные параметры
            /// </summary>
            public List<string> Files;

            /// <summary>
            /// Параметры столбцов таблицы
            /// </summary>
            public TableColumns TableColumns;

            /// <summary>
            /// Параметры внутренней области таблицы
            /// </summary>
            public TableEntry TableEntry;
        }

        /// <summary>
        /// Список таблиц отчета
        /// </summary>
        private class Tables : DisposableObject
        {
            #region Инициализация

            /// <summary>
            /// Конструктор
            /// </summary>
            public Tables()
            {
                this.TablesList = new List<Table>(20);
            }

            /// <summary>
            /// Освобождение ресурсов
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.TablesList != null) this.TablesList.Clear();
                }
                
                base.Dispose(disposing);
            }

            #endregion Инициализация


            /// <summary>
            /// Количество таблиц в отчете. Если количество таблиц известно и фиксированно, то лучше указывать.
            /// </summary>
            public int TablesAmount;

            /// <summary>
            /// Список таблиц отчета
            /// </summary>
            public List<Table> TablesList;
        }


        /// <summary>
        /// Тип данных, хранимых в столбце
        /// </summary>
        private enum ColumnDataType
        {
            /// <summary>
            /// Целочисленное значение
            /// </summary>
            Integer,

            /// <summary>
            /// Строковое значение
            /// </summary>
            String,

            /// <summary>
            /// Дробное значение
            /// </summary>
            Double
        }


        /// <summary>
        /// Точность данных столбца
        /// </summary>
        private class DataPrecision : DisposableObject
        {
            /// <summary>
            /// Число знаков значащей части числа
            /// </summary>
            public int Significand;

            /// <summary>
            /// Число знаков дробной части числа
            /// </summary>
            public int Fraction;
        }


        /// <summary>
        /// Тип переноса данных в ячейках столбца. 
        /// </summary>
        private enum DivisionKind
        {
            /// <summary>
            /// Нет переносов.
            /// </summary>
            None,

            /// <summary>
            /// Ячейка с переносом. Значения в других ячейках строки располагаются по верхней границе переноса.
            /// </summary>
            Upper,

            /// <summary>
            /// Ячейка с переносом. Значения в других ячейках строки располагаются по нижней границе переноса.
            /// </summary>
            Lower,

            /// <summary>
            /// Нет переносов. Ячейка выступает в роли признака переноса: если в этой ячейке нет значения, 
            /// то имеет место перенос в ячейках с переносом.
            /// </summary>
            DivisionSign
        }


        /// <summary>
        /// Параметры колонки таблицы
        /// </summary>
        private class TableColumn : DisposableObject
        {
            #region Инициализация

            /// <summary>
            /// Конструктор
            /// </summary>
            public TableColumn()
            {
                this.ExcludedValues = new List<string>(20);
            }

            /// <summary>
            /// Освобождение ресурсов
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.ExcludedValues != null) this.ExcludedValues.Clear();
                }
                
                base.Dispose(disposing);
            }

            #endregion Инициализация


            /// <summary>
            /// Тип переноса данных в ячейках столбца
            /// </summary>
            public DivisionKind DivisionKind;

            /// <summary>
            /// тип хранимой информации (для контроля правильности данных)
            /// </summary>
            public ColumnDataType DataType;

            /// <summary>
            /// Ширина столбца
            /// </summary>
            public int Width;

            /// <summary>
            /// поле таблицы БД, в которое будут закачиваться данные столбца
            /// </summary>
            public string DBField;

            /// <summary>
            /// Порядковый номер файла отчета, в котором находится параметр
            /// </summary>
            public int PartNo;

            /// <summary>
            /// Заголовок столбца, используемый в форме предварительного просмотра
            /// </summary>
            public string Caption;

            /// <summary>
            /// false - Все строки столбца обязяны иметь значение.
            /// </summary>
            public bool Nullable;

            /// <summary>
            /// Значение по умолчанию. Если оно определено и Nullable == false, то в случае отсутствия значения 
            /// в ячейке столбца будет подставлено значение по умолчанию, в противном случае строка будет пропущена.
            /// </summary>
            public string DefaultValue;

            /// <summary>
            /// Точность данных столбца (если это число). Формат: n.m, где n - число знаков до запятой, 
            /// m - число знаков после запятой.
            /// </summary>
            public DataPrecision DataPrecision;

            /// <summary>
            /// Значения ячеек столбца, строки с которыми будут исключены
            /// </summary>
            public List<string> ExcludedValues;

            /// <summary>
            /// При несоответствии значения ячейки типу или точности генерится исключение, иначе подставляется значение по умолчанию.
            /// </summary>
            public bool ExactCheck;
        }


        /// <summary>
        /// Параметры столбцов таблицы
        /// </summary>
        private class TableColumns : DisposableObject
        {
            #region Инициализация

            /// <summary>
            /// Конструктор
            /// </summary>
            public TableColumns()
            {
                this.Columns = new List<TableColumn>(20);
            }

            /// <summary>
            /// Освобождение ресурсов
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.Columns != null) this.Columns.Clear();
                }
                
                base.Dispose(disposing);
            }

            #endregion Инициализация


            /// <summary>
            /// список столбцов TableColumn
            /// </summary>
            public List<TableColumn> Columns;
        }


        /// <summary>
        /// Параметры внутренней области таблицы
        /// </summary>
        private class TableEntry : DisposableObject
        {
            /// <summary>
            /// признак окончания таблицы - в данном случае пустая строка
            /// </summary>
            public string EndTableMarker;

            /// <summary>
            /// признак того, что в середине таблицы есть какая-то часть шапки (обычно с номерами строк)
            /// </summary>
            public bool HeaderAmidTable;

            /// <summary>
            /// в таблице отчета каждая вторая строка - "итого"
            /// </summary>
            public bool TotalSumAcrossRow;

            /// <summary>
            /// Количество горизонтальных разделителей в заголовке.
            /// </summary>
            public int HeaderHeight;

            /// <summary>
            /// Включать строку с признаком конца таблицы в датасет с данными.
            /// </summary>
            public bool IncludeEndTableMarker;

            /// <summary>
            /// Признак конца таблицы находится внутри самой таблицы или нет (важно, если отчет содержит несколько таблиц).
            /// </summary>
            public bool InnerEndTableMarker;

            /// <summary>
            /// Таблица имеет разделители вверху и внизу
            /// </summary>
            public bool TableDelimited;

            /// <summary>
            /// Количество строк от признака конца таблицы до нижней границы таблицы
            /// </summary>
            public int EndTableMarkerSpace;
        }


        /// <summary>
        /// Параметры отчета
        /// </summary>
        private class TXTReportSettings : DisposableObject
        {
            #region Инициализация

            /// <summary>
            /// Конструктор
            /// </summary>
            public TXTReportSettings()
            {
                this.Tables = new Tables();
            }

            /// <summary>
            /// Освобождение ресурсов
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.Tables != null) this.Tables.Dispose();
                }
                
                base.Dispose(disposing);
            }

            #endregion Инициализация


            /// <summary>
            /// Общие Параметры отчета
            /// </summary>
            public GlobalSettings GlobalSettings;

            /// <summary>
            /// координаты параметров, которые всегда находятся в одном и том же месте (даты и т.п.) FixedParamPos
            /// </summary>
            public List<FixedParamPos> FixedParamsPos;

            /// <summary>
            /// Список таблиц отчета
            /// </summary>
            public Tables Tables;

            /// <summary>
            /// Ищет описание таблицы по ее индексу в отчете
            /// </summary>
            /// <param name="files">Файлы отчета</param>
            /// <param name="index">Индекс</param>
            /// <param name="repFilesDir">Каталог с отчетами</param>
            /// <returns>Описание</returns>
            public Table GetTable(FileInfo[] files, int index, DirectoryInfo repFilesDir)
            {
                if (Tables == null)
                {
                    throw new TextRepAnalysisFailedException(
                        string.Format("Не найдено описание параметров для таблицы {0}", index + 1));
                }

                Table result = null;

                for (int i = 0; i < Tables.TablesList.Count; i++)
                {
                    Table table = Tables.TablesList[i];
                    // Проверка на соответствие текущего файла списку найденного описания
                    bool checkTableResult = CheckTableFile(files, repFilesDir, table);

                    // Если нашли структуру с описанием таблицы с указанными индексом - ее и возвращаем
                    if (table.TableIndexes.Contains(index) && checkTableResult)
                    {
                        return table;
                    }
                    // На случай если не найдем - вернем универсальное описание таблиц текущего файла, если есть
                    else if (checkTableResult && table.TableIndexes.Count == 0)
                    {
                        result = table;
                    }
                    // На случай если не найдем - вернем универсальное описание для всех файлов, если есть
                    else if (table.TableIndexes.Count == 0 && result == null)
                    {
                        result = table;
                    }
                }

                if (result == null)
                {
                    throw new TextRepAnalysisFailedException(
                        string.Format("Не найдено описание параметров для таблицы {0}", index + 1));
                }

                return result;
            }

            /// <summary>
            /// Проверяет соответствие файлов из списка Table обрабатываемым файлам
            /// </summary>
            /// <param name="files">Файлы отчета</param>
            /// <param name="repFilesDir">Каталог с отчетами</param>
            /// <param name="table">Описание параметров таблицы</param>
            /// <returns>Соответствует или нет</returns>
            private bool CheckTableFile(FileInfo[] files, DirectoryInfo repFilesDir, Table table)
            {
                for (int i = 0; i < table.Files.Count; i++)
                {
                    // Ищем файл в каталоге отчетов
                    FileInfo[] tableFiles = repFilesDir.GetFiles(table.Files[i], SearchOption.AllDirectories);

                    // Проверяем, есть ли такой среди файлов текущего отчета
                    for (int j = 0; j < tableFiles.GetLength(0); j++)
                    {
                        for (int k = 0; k < files.GetLength(0); k++)
                        {
                            if (String.Compare(tableFiles[j].FullName, files[k].FullName, true) == 0)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        #endregion Структуры данных хмл-настроек


        #region Структуры данных результатов разбора

        /// <summary>
        /// Восставший из текста столбец таблицы
        /// </summary>
        private class ArisedColumn : DisposableObject
        {
            /// <summary>
            /// ИД столбца
            /// </summary>
            public int ID;

            // Заголовок столбца
            //public string Caption;
            // Позиция левой границы
            public int LeftPos;

            /// <summary>
            /// Позиция правой границы
            /// </summary>
            public int RightPos;

            /// <summary>
            /// Позиция верхней границы
            /// </summary>
            public int TopPos;

            /// <summary>
            /// Позиция нижней границы
            /// </summary>
            public int BottomPos;

            /// <summary>
            /// Верхняя граница заголовка
            /// </summary>
            //public int HeaderTopPos;
        }


        /// <summary>
        /// Результат анализа
        /// </summary>
        private class TXTAnalysisResult : DisposableObject
        {
            #region Инициализация

            /// <summary>
            /// Конструктор
            /// </summary>
            public TXTAnalysisResult()
            {
                this.ArisedColumns = new List<ArisedColumn>(20);
            }

            /// <summary>
            /// Освобождение ресурсов
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.ArisedColumns != null) this.ArisedColumns.Clear();
                }
                
                base.Dispose(disposing);
            }

            #endregion Инициализация


            /// <summary>
            /// Массив начальным границ частей отчета
            /// </summary>
            public int[] PartStartMargins;

            /// <summary>
            /// Массив конечных границ частей отчета
            /// </summary>
            public int[] PartEndMargins;

            /// <summary>
            /// Массив количества строк частей отчета
            /// </summary>
            public int[] PartRowsCount;

            /// <summary>
            /// Восставшие из текста столбцы таблицы ArisedColumn
            /// </summary>
            public List<ArisedColumn> ArisedColumns;
        }


        /// <summary>
        /// Информация об одной вертикальной или горизонтальной линии
        /// </summary>
        private class LineInfo : DisposableObject
        {
            /// <summary>
            /// Индекс линии в массиве текстовых данных
            /// </summary>
            public int Index;

            /// <summary>
            /// Количество вертикальных/горизонтальных разделителей
            /// </summary>
            public int DelimitersCount;

            /// <summary>
            /// Это разделитель или нет
            /// </summary>
            public bool IsMargin;

            /// <summary>
            /// Это укороченный разделитель (не на всю длину строки) или нет
            /// </summary>
            public bool IsShortMargin;
        }


        /// <summary>
        /// Параметры всех горизонтальных/вертикальных линий файла
        /// </summary>
        private class LineInfoCollection : DisposableObject
        {
            #region Инициализация

            /// <summary>
            /// Конструктор
            /// </summary>
            public LineInfoCollection()
            {
                this.line2TextData = new Dictionary<int, int>(constMaxRows);
                this.textData2Line = new Dictionary<int, int>(constMaxRows);
                this.textLines = new List<LineInfo>(constMaxRows);
            }

            /// <summary>
            /// Освобождение ресурсов
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.line2TextData != null) this.line2TextData.Clear();
                    if (this.textData2Line != null) this.textData2Line.Clear();
                    if (this.textLines != null) this.textLines.Clear();
                }
                
                base.Dispose(disposing);
            }

            #endregion Инициализация


            /// <summary>
            /// Массив с параметрами всех горизонтальных (вертикальных) линий файла
            /// </summary>
            private List<LineInfo> textLines;

            /// <summary>
            /// Коллекция соответствия значения ключа TextRows номеру строки (столбца) в массиве данных отчета
            /// </summary>
            private Dictionary<int, int> line2TextData;

            /// <summary>
            /// Коллекция соответствия номера строки (столбца) в массиве данных отчета значению ключа TextRows 
            /// </summary>
            private Dictionary<int, int> textData2Line;

            /// <summary>
            /// Возвращает информацию о строке (столбце) по индексу строки исходных данных
            /// </summary>
            /// <param name="index">Индекс строки (столбца) исходных данных</param>
            /// <returns>Информация о строке (столбце)</returns>
            public LineInfo this[int index]
            {
                get
                {
                    if (this.textData2Line.ContainsKey(index))
                    {
                        return this.textLines[this.textData2Line[index]];
                    }
                    else
                    {
                        LineInfo li = new LineInfo();
                        li.Index = index;
                        this.textLines.Add(li);

                        this.textData2Line.Add(index, this.textLines.Count - 1);

                        return li;
                    }
                }
            }

            /// <summary>
            /// Количество линий в списке
            /// </summary>
            public int Count
            {
                get
                {
                    return this.textLines.Count;
                }
            }
        }


        /// <summary>
        /// Информация о вертикальных и горизонтальных линиях одной таблицы отчета
        /// </summary>
        private class ParsedTableInfo : DisposableObject
        {
            #region Инициализация

            /// <summary>
            /// Конструктор
            /// </summary>
            public ParsedTableInfo()
            {
                this.TextRows = new LineInfoCollection();
                this.TextColumns = new LineInfoCollection();
            }

            /// <summary>
            /// Освобождение ресурсов
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.TextRows != null) this.TextRows.Dispose();
                    if (this.TextColumns != null) this.TextColumns.Dispose();
                }
                
                base.Dispose(disposing);
            }

            #endregion Инициализация


            /// <summary>
            /// Индекс таблицы
            /// </summary>
            public int Index;

            /// <summary>
            /// Структура с описанием параметров разобранной таблицы
            /// </summary>
            public Table Table;

            /// <summary>
            /// Массив с параметрами всех горизонтальных линий файла
            /// </summary>
            public LineInfoCollection TextRows;

            /// <summary>
            /// Массив с параметрами всех вертикальных линий файла
            /// </summary>
            public LineInfoCollection TextColumns;

            /// <summary>
            /// Верхняя граница области данных таблицы
            /// </summary>
            public int DataAreaTopMargin;

            /// <summary>
            /// Нижняя граница области данных таблицы
            /// </summary>
            public int DataAreaBottomMargin;

            /// <summary>
            /// Верхняя граница таблицы
            /// </summary>
            public int TopMargin;

            /// <summary>
            /// Нижняя граница таблицы
            /// </summary>
            public int BottomMargin;

            /// <summary>
            /// Возвращает количество вертикальных разделителей таблицы
            /// </summary>
            /// <returns>Количество вертикальных разделителей таблицы</returns>
            public int GetVertDelimitersCount()
            {
                int result = 0;

                for (int i = 0; i < this.TextColumns.Count; i++)
                {
                    if (this.TextColumns[i].IsMargin)
                    {
                        result++;
                    }
                }

                return result;
            }
        }

        #endregion Структуры данных результатов разбора

        #endregion Структуры


        #region Поля

        // Информация о вертикальнвх и горизонтальных линиях таблиц отчета
        private List<ParsedTableInfo> tableInfoList;
        // Непосредственно текстовые данные
        private List<string> textDataStrings;
        // Список массивов файлов (один массив на маску файла из параметров хмл)
        private List<FileInfo[]> repFilesLists;

        private FileInfo xmlSettingsFile;
        private DirectoryInfo repFilesDir;
        private TXTReportSettings txtReportSettings;
        private TXTAnalysisResult txtAnalysisResult;
        private DataSet resultDS;
        private string fileIndFieldName;
        private string tableIndFieldName;
        private Dictionary<int, Dictionary<string, FixedParameter>> fixedParams =
            new Dictionary<int, Dictionary<string, FixedParameter>>(100);
        private DataPumpModuleBase dataPumpModule;
        private int firstTableTopMargin = 0;

        private int totalColsCount;
        private int totalRowsCount;

        // Порядковый номер обрабатываемого отчета (отчет может состоять из нескольких файлов)
        private int currentReportIndex = 0;
        // Порядковый номер обрабатываемого файла
        private int currentFileIndex = -1;
        // Номер текущей таблицы с данными отчета
        private int currentReportTableIndex;

        private Encoding encoding866 = Encoding.GetEncoding(866);

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="settingsFileName">Файл с хмл-настройками</param>
        /// <param name="repFilesPath">Путь к текстовым файлам отчетов</param>
        /// <param name="fileIndexFieldName">Наименование столбца в ResultTable, по которому определяется 
        /// принадлежность данных конкретному файлу</param>
        /// <param name="tableIndexFieldName">Наименование столбца в таблице с данными отчета, 
        /// по которому определяется принадлежность данных таблице файла отчета</param>
        public TXTSorcerer(string settingsFileName, string repFilesDir, string fileIndexFieldName,
           string tableIndexFieldName)
        {
            if (!File.Exists(settingsFileName))
                throw new TextRepAnalysisFailedException(string.Format("Файл {0} не найден.", settingsFileName));

            if (!Directory.Exists(repFilesDir))
                throw new TextRepAnalysisFailedException(string.Format("Каталог {0} не найден.", repFilesDir));

            this.xmlSettingsFile = new FileInfo(settingsFileName);
            this.repFilesDir = new DirectoryInfo(repFilesDir);
            this.fileIndFieldName = fileIndexFieldName;
            this.tableIndFieldName = tableIndexFieldName;
        }

        #endregion Инициализация


        #region Инициализация структур

        /// <summary>
        /// Инициализация структур параметров отчета
        /// </summary>
        private void InitTXTReportSettings()
        {
            this.txtReportSettings = new TXTReportSettings();
            this.txtReportSettings.FixedParamsPos = new List<FixedParamPos>(20);

            this.txtReportSettings.GlobalSettings = new GlobalSettings();
        }

        /// <summary>
        /// Инициализация структур результатов анализа файлов
        /// </summary>
        private void InitTXTAnalysisResult(int repPartsCount)
        {
            this.txtAnalysisResult = new TXTAnalysisResult();
            this.txtAnalysisResult.PartStartMargins = new int[repPartsCount];
            this.txtAnalysisResult.PartEndMargins = new int[repPartsCount];
            this.txtAnalysisResult.PartRowsCount = new int[repPartsCount];
            this.txtAnalysisResult.ArisedColumns = new List<ArisedColumn>(50);
        }

        /// <summary>
        /// Инициализация датасета с результатами анализа
        /// </summary>
        private void InitResultDataSet()
        {
            if (this.resultDS != null) this.resultDS.Dispose();

            this.resultDS = new DataSet();
            this.resultDS.Tables.Add("ReportFiles");

            this.resultDS.Tables["ReportFiles"].Columns.Add("FILES", typeof(string));
            this.resultDS.Tables["ReportFiles"].Columns["FILES"].Caption = "Файлы отчета";
            this.resultDS.Tables["ReportFiles"].Columns.Add("INFO", typeof(string));
            this.resultDS.Tables["ReportFiles"].Columns["INFO"].Caption = "Результат анализа";
            this.resultDS.Tables["ReportFiles"].Columns.Add("ROWSCOUNT", typeof(int));
            this.resultDS.Tables["ReportFiles"].Columns["ROWSCOUNT"].Caption = "Всего строк";
            this.resultDS.Tables["ReportFiles"].Columns.Add("STARTTIME", typeof(string));
            this.resultDS.Tables["ReportFiles"].Columns["STARTTIME"].Caption = "Время начала";
            this.resultDS.Tables["ReportFiles"].Columns.Add("ENDTIME", typeof(string));
            this.resultDS.Tables["ReportFiles"].Columns["ENDTIME"].Caption = "Время окончания";
            this.resultDS.Tables["ReportFiles"].Columns.Add(fileIndFieldName, typeof(int));
            this.resultDS.Tables["ReportFiles"].Columns.Add(tableIndFieldName, typeof(int));
        }

        /// <summary>
        /// Создает таблицу данных отчета по параметрам хмл
        /// </summary>
        private void AddFileDataTable(Table table)
        {
            DataTable dt = this.resultDS.Tables.Add();
            this.currentReportTableIndex++;

            // Добавляем столбцы
            for (int i = 0; i < table.TableColumns.Columns.Count; i++)
            {
                TableColumn tcol = table.TableColumns.Columns[i];

                switch (tcol.DataType)
                {
                    case ColumnDataType.Double:
                        dt.Columns.Add(tcol.DBField, typeof(double));
                        break;

                    case ColumnDataType.Integer:
                        dt.Columns.Add(tcol.DBField, typeof(int));
                        break;

                    case ColumnDataType.String:
                        dt.Columns.Add(tcol.DBField, typeof(string));
                        break;
                }

                if (tcol.Caption != string.Empty)
                {
                    dt.Columns[tcol.DBField].Caption = tcol.Caption;
                }
                else
                {
                    dt.Columns[tcol.DBField].Caption = tcol.DBField;
                }
            }

            // Добавляем служебный столбец для идентификации принадлежности данных конкретным файлам
            dt.Columns.Add(fileIndFieldName, typeof(int));
            // Добавляем служебный столбец для идентификации принадлежности данных конкретным таблицам отчета
            dt.Columns.Add(tableIndFieldName, typeof(int));

            // Добавляем столбцы для данных фиксированных параметров
            for (int i = 0; i < this.txtReportSettings.FixedParamsPos.Count; i++)
            {
                if (this.txtReportSettings.FixedParamsPos[i].DBField != string.Empty)
                {
                    dt.Columns.Add(this.txtReportSettings.FixedParamsPos[i].DBField, typeof(string));
                }
            }

            //this.resultDS.Tables["ReportFiles"].Rows[this.currentReportIndex]["fileIndFieldName"] = this.currentReportIndex;

            this.resultDS.Relations.Add(new DataRelation(
                this.currentReportTableIndex.ToString(),
                this.resultDS.Tables["ReportFiles"].Columns[fileIndFieldName],
                dt.Columns[fileIndFieldName]));
        }

        /// <summary>
        /// Инициализация структур для хранения данных текстовых файлов
        /// </summary>
        private void InitLists()
        {
            // Инициализация структуры описания таблиц отчета
            if (this.tableInfoList != null)
            {
                this.tableInfoList.Clear();
            }
            else
            {
                this.tableInfoList = new List<ParsedTableInfo>(100);
            }
        }

        /// <summary>
        /// Добавляет структуру описания таблицы в список описаний
        /// </summary>
        /// <param name="index">Индекс таблицы</param>
        /// <param name="table">Описание структуры таблицы</param>
        /// <returns>Структура описания таблицы</returns>
        private ParsedTableInfo AddParsedTableInfo(int index, Table table)
        {
            ParsedTableInfo tableLinesInfo = new ParsedTableInfo();
            tableLinesInfo.Index = index;
            tableLinesInfo.Table = table;

            this.tableInfoList.Add(tableLinesInfo);

            return tableLinesInfo;
        }

        /// <summary>
        /// Инициализация структур для хранения данных текстовых файлов
        /// </summary>
        private void InitTextDataArray()
        {
            if (this.textDataStrings== null)
            {
                this.textDataStrings = new List<string>(constMaxRows);
            }
            else
            {
                this.textDataStrings.Clear();
            }
        }

        /// <summary>
        /// Освобождает память
        /// </summary>
        private void DisposeLists()
        {
            if (this.tableInfoList != null)
            {
                for (int i = 0; i < this.tableInfoList.Count; i++)
                {
                    if (this.tableInfoList[i] != null) this.tableInfoList[i].Dispose();
                }

                this.tableInfoList.Clear();
            }

            if (this.repFilesLists != null) this.repFilesLists.Clear();

            if (this.textDataStrings != null) this.textDataStrings.Clear();
        }

        #endregion Инициализация структур


        #region Загрузка параметров из XML

        /// <summary>
        /// Преобразует строку в PartsUnionType
        /// </summary>
        /// <param name="value">Строка</param>
        /// <returns>PartsUnionType</returns>
        private PartsUnionType StringToPartsUnionType(string value)
        {
            if (String.Compare(value, "HORIZONTAL", true) == 0)
            {
                return PartsUnionType.Horizontal;
            }
            else if (String.Compare(value, "VERTICAL", true) == 0)
            {
                return PartsUnionType.Vertical;
            }

            return PartsUnionType.Unknown;
        }

        /// <summary>
        /// Загружает секцию глобальных настроек хмл
        /// </summary>
        /// <param name="xd">хмл-документ с настройками</param>
        private void LoadGlobalSettings(XmlDocument xd)
        {
            XmlNode xn = xd.SelectSingleNode(nodeGlobalSettings);
            if (xn == null)
            {
                throw new TextRepAnalysisFailedException(
                    string.Format("В файле настроек {0} отсутствует элемент GlobalSettings.", xmlSettingsFile.Name));
            }

            this.txtReportSettings.GlobalSettings.VertDelimiters = CommonRoutines.StringArrayToIntList(
                XmlHelper.GetStringAttrValue(xn, attrVertDelimiters, string.Empty).Split(';'));
            if (txtReportSettings.GlobalSettings.VertDelimiters.Count == 0)
            {
                throw new TextRepAnalysisFailedException("Отсутствует описание вертикальных разделителей.");
            }

            this.txtReportSettings.GlobalSettings.HorzDelimiters = CommonRoutines.StringArrayToIntList(
                XmlHelper.GetStringAttrValue(xn, attrHorzDelimiters, string.Empty).Split(';'));
            if (txtReportSettings.GlobalSettings.HorzDelimiters.Count == 0)
            {
                throw new TextRepAnalysisFailedException("Отсутствует описание горизотальных разделителей.");
            }

            string attrValue = XmlHelper.GetStringAttrValue(xn, attrCharacterSet, "OEM");
            if (attrValue.ToUpper() == "OEM")
            {
                this.txtReportSettings.GlobalSettings.CharSet = CharacterSet.OEM;
            }
            else if (attrValue.ToUpper() == "ANSI")
            {
                this.txtReportSettings.GlobalSettings.CharSet = CharacterSet.ANSI;
            }
            else
            {
                throw new TextRepAnalysisFailedException(string.Format(
                    "В файле настроек {0} указана неизвестня кодировка.", xmlSettingsFile.Name));
            }

            this.txtReportSettings.GlobalSettings.ReportWildCards = CommonRoutines.StringArrayToStringList(
                XmlHelper.GetStringAttrValue(xn, attrReportWildCards, string.Empty).Split(';'));

            string str = XmlHelper.GetStringAttrValue(xn, attrPartsWildCards, string.Empty);
            if (str != string.Empty)
            {
                this.txtReportSettings.GlobalSettings.PartsWildCards = CommonRoutines.StringArrayToStringList(
                    str.Split(';'));
            }

            this.txtReportSettings.GlobalSettings.PartsUnionType = StringToPartsUnionType(
                XmlHelper.GetStringAttrValue(xn, attrPartsUnionType, string.Empty));

            str = XmlHelper.GetStringAttrValue(xn, attrExcludeFilesByString, string.Empty);
            if (str != string.Empty)
            {
                this.txtReportSettings.GlobalSettings.ExcludeFilesByString = CommonRoutines.StringArrayToStringList(
                    str.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        /// <summary>
        /// Преобразует строку в тип фиксированного параметра
        /// </summary>
        /// <param name="value">Строка</param>
        /// <returns>Тип фиксированного параметра</returns>
        private FixedParamKind StringToFixedParamKind(string value)
        {
            if (value.ToUpper() == "SINGLE")
            {
                return FixedParamKind.Single;
            }
            else if (value.ToUpper() == "BEFORETABLE")
            {
                return FixedParamKind.BeforeTable;
            }
            return FixedParamKind.Single;
        }

        /// <summary>
        /// Загружает секцию фиксированных параметров отчета
        /// </summary>
        /// <param name="xd">хмл-документ с настройками</param>
        private void LoadFixedParams(XmlDocument xd)
        {
            XmlNodeList xnl = xd.SelectNodes(nodeFixedParam);
            for (int i = 0; i < xnl.Count; i++)
            {
                FixedParamPos fp = new FixedParamPos();
                fp.BeginPosX = XmlHelper.GetIntAttrValue(xnl[i], attrBeginPosX, 0);
                fp.BeginPosY = XmlHelper.GetIntAttrValue(xnl[i], attrBeginPosY, 0);
                fp.EndPosX = XmlHelper.GetIntAttrValue(xnl[i], attrEndPosX, 0);
                fp.EndPosY = XmlHelper.GetIntAttrValue(xnl[i], attrEndPosY, 0);
                fp.Name = XmlHelper.GetStringAttrValue(xnl[i], attrName, string.Empty);
                fp.PartNo = XmlHelper.GetIntAttrValue(xnl[i], attrPartNo, 0);
                fp.Caption = XmlHelper.GetStringAttrValue(xnl[i], attrCaption, string.Empty);
                fp.FixedParamKind = StringToFixedParamKind(
                    XmlHelper.GetStringAttrValue(xnl[i], attrFixedParamKind, string.Empty));
                fp.DBField = XmlHelper.GetStringAttrValue(xnl[i], attrDBField, string.Empty);
                fp.DefaultValue = XmlHelper.GetStringAttrValue(xnl[i], attrDefaultValue, string.Empty);
                txtReportSettings.FixedParamsPos.Add(fp);
            }
        }

        /// <summary>
        /// Преобразует строку в ColumnDataType
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>ColumnDataType</returns>
        private ColumnDataType StringToColumnDataType(string str)
        {
            if (str.ToUpper() == "DOUBLE")
            {
                return ColumnDataType.Double;
            }
            else if (str.ToUpper() == "INTEGER")
            {
                return ColumnDataType.Integer;
            }
            else if (str.ToUpper() == "STRING")
            {
                return ColumnDataType.String;
            }
            return ColumnDataType.String;
        }

        /// <summary>
        /// Преобразует строку в тип переноса в ячейках столбца
        /// </summary>
        /// <param name="value">Строка</param>
        /// <returns>Тип переноса</returns>
        private DivisionKind StringToDivisionKind(string value)
        {
            if (value == tclUpper)
            {
                return DivisionKind.Upper;
            }
            else if (value == tclLower)
            {
                return DivisionKind.Lower;
            }
            else if (value == tclDivisionSign)
            {
                return DivisionKind.DivisionSign;
            }

            return DivisionKind.None;
        }

        /// <summary>
        /// Загружает секцию параметров столбцов в файле отчета
        /// </summary>
        /// <param name="node">хмл-элемент с настройками</param>
        /// <param name="tableColumns">Коллекция столбцов</param>
        private void LoadTableColumns(XmlNode node, TableColumns tableColumns)
        {
            XmlNode xn = node.SelectSingleNode(nodeTableColumns);
            if (xn == null)
            {
                throw new TextRepAnalysisFailedException(
                    string.Format("В файле настроек {0} отсутствует элемент TableColumns.", xmlSettingsFile.Name));
            }

            XmlNodeList xnl = node.SelectNodes(nodeColumns);
            if (xnl.Count == 0)
            {
                throw new TextRepAnalysisFailedException(
                    string.Format("В файле настроек {0} отсутствует описание столбцов отчета.", xmlSettingsFile.Name));
            }

            tableColumns.Columns = new List<TableColumn>(xnl.Count);

            for (int i = 0; i < xnl.Count; i++)
            {
                TableColumn tc = new TableColumn();
                tc.DataType = StringToColumnDataType(
                    XmlHelper.GetStringAttrValue(xnl[i], attrType, "string"));
                tc.DivisionKind = StringToDivisionKind(
                    XmlHelper.GetStringAttrValue(xnl[i], attrDivisionKind, string.Empty));
                tc.PartNo = XmlHelper.GetIntAttrValue(xnl[i], attrPartNo, -1);
                tc.Width = XmlHelper.GetIntAttrValue(xnl[i], attrWidth, 80);
                tc.DBField = XmlHelper.GetStringAttrValue(xnl[i], attrDBField, string.Empty);
                tc.Caption = XmlHelper.GetStringAttrValue(xnl[i], attrCaption, string.Empty);
                tc.Nullable = XmlHelper.GetBoolAttrValue(xnl[i], attrNullable, true);
                tc.DefaultValue = XmlHelper.GetStringAttrValue(xnl[i], attrDefaultValue, string.Empty);

                // Загружаем данные о контроле разрядности данных столбца
                string prec = XmlHelper.GetStringAttrValue(xnl[i], attrDataPrecision, string.Empty);
                tc.DataPrecision = new DataPrecision();
                if (prec != string.Empty)
                {
                    string[] values = prec.Split('.');
                    if (values.GetLength(0) != 2)
                    {
                        throw new TextRepAnalysisFailedException("Формат значения атрибута DataPrecision не соответствует стандартному.");
                    }

                    tc.DataPrecision.Significand = Convert.ToInt32(values[0]);
                    tc.DataPrecision.Fraction = Convert.ToInt32(values[1]);
                }
                else
                {
                    tc.DataPrecision.Significand = -1;
                    tc.DataPrecision.Fraction = -1;
                }

                // Загруажем исключаемые значения столбца
                string[] exValues = XmlHelper.GetStringAttrValue(xnl[i], attrExcludedValues,
                    string.Empty).Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                tc.ExcludedValues = new List<string>(exValues.GetLength(0));
                for (int j = 0; j < exValues.GetLength(0); j++)
                {
                    tc.ExcludedValues.Add(exValues[j]);
                }

                tc.ExactCheck = XmlHelper.GetBoolAttrValue(xnl[i], attrExactCheck, true);

                tableColumns.Columns.Add(tc);
            }
        }

        /// <summary>
        /// Загружает секцию параметров области таблицы
        /// </summary>
        /// <param name="node">хмл-элемент с настройками</param>
        /// <param name="tableEntry">Стурктура с описанием</param>
        private void LoadTableEntry(XmlNode node, TableEntry tableEntry)
        {
            XmlNode xn = node.SelectSingleNode(nodeTableEntry);
            if (xn == null)
            {
                throw new TextRepAnalysisFailedException(
                    string.Format("В файле настроек {0} отсутствует элемент TableEntry.", xmlSettingsFile.Name));
            }

            tableEntry.EndTableMarker = XmlHelper.GetStringAttrValue(xn, attrEndTableMarker, "       ");
            
            tableEntry.HeaderAmidTable = XmlHelper.GetBoolAttrValue(xn, attrHeaderAmidTable, true);

            tableEntry.TotalSumAcrossRow = XmlHelper.GetBoolAttrValue(xn, attrTotalSumAcrossRow, true);

            tableEntry.HeaderHeight = XmlHelper.GetIntAttrValue(xn, attrHeaderHeight, 2);

            tableEntry.IncludeEndTableMarker = XmlHelper.GetBoolAttrValue(xn, attrIncludeEndTableMarker, false);

            tableEntry.InnerEndTableMarker = XmlHelper.GetBoolAttrValue(xn, attrInnerEndTableMarker, true);

            tableEntry.TableDelimited = XmlHelper.GetBoolAttrValue(xn, attrTableDelimited, true);

            tableEntry.EndTableMarkerSpace = XmlHelper.GetIntAttrValue(xn, attrEndTableMarkerSpace, 1);
        }

        /// <summary>
        /// Загружает параметры таблиц отчета
        /// </summary>
        /// <param name="xd">хмл-документ с настройками</param>
        private void LoadTables(XmlDocument xd)
        {
            XmlNode xn = xd.SelectSingleNode(nodeTables);
            if (xn == null)
            {
                throw new TextRepAnalysisFailedException(
                    string.Format("В файле настроек {0} отсутствует элемент Tables.", xmlSettingsFile.Name));
            }

            XmlNodeList xnl = xn.SelectNodes(nodeTable);
            if (xnl.Count == 0)
            {
                throw new TextRepAnalysisFailedException(
                    string.Format("В файле настроек {0} отсутствуют элементы Table.", xmlSettingsFile.Name));
            }

            this.txtReportSettings.Tables.TablesAmount = XmlHelper.GetIntAttrValue(xn, attrTablesAmount, -1);
            if (this.txtReportSettings.Tables.TablesAmount == 0)
            {
                this.txtReportSettings.Tables.TablesAmount = -1;
            }

            for (int i = 0; i < xnl.Count; i++)
            {
                Table table = new Table();

                string location = XmlHelper.GetStringAttrValue(xnl[i], attrLocation, string.Empty);
                if (location != string.Empty)
                {
                    string[] str = location.Split('|');
                    if (str.GetLength(0) == 0)
                    {
                        throw new Exception("Некорректный формат значения атрибута Location " + location);
                    }

                    table.Files = CommonRoutines.StringArrayToStringList(str[0].Split(';'));
                    table.TableIndexes = CommonRoutines.ParseParamsStringToList(str[1]);
                }

                table.TableColumns = new TableColumns();
                LoadTableColumns(xnl[i], table.TableColumns);

                table.TableEntry = new TableEntry();
                LoadTableEntry(xnl[i], table.TableEntry);

                this.txtReportSettings.Tables.TablesList.Add(table);
            }
        }

        /// <summary>
        /// Загружает хмл-настройки для текстовых файлов
        /// </summary>
        private void LoadTXTReportSettings()
        {
            // Загрузка хмл
            XmlDocument xdSettings = new XmlDocument();
            xdSettings.Load(xmlSettingsFile.FullName);

            // Инициализация структур
            InitTXTReportSettings();

            // Загружаем секцию глобальных настроек хмл
            LoadGlobalSettings(xdSettings);

            // Загружаем секцию фиксированных параметров отчета
            LoadFixedParams(xdSettings);

            // Загружаем параметры таблиц отчета
            LoadTables(xdSettings);
        }

        #endregion Загрузка параметров из XML


        #region Обработка файлов

        #region Общая организация обработки файлов

        /// <summary>
        /// Возвращает данные текстовых файлов
        /// </summary>
        /// <param name="dpm">Модуль закачки (для установки позиции прогресса)</param>
        /// <param name="resultDataSet">Датасет с данными файлов</param>
        /// <param name="filesLists">Список наименований файлов для поиска их данных в таблице</param>
        /// <param name="fixedParameters">Список фиксированных параметров</param>
        /// <param name="charSet">Кодировка файлов</param>
        public void GetDataFromFiles(DataPumpModuleBase dpm, ref DataSet resultDataSet, ref List<FileInfo[]> filesLists,
            ref Dictionary<int, Dictionary<string, FixedParameter>> fixedParameters, out CharacterSet charSet)
        {
            this.dataPumpModule = dpm;
            charSet = CharacterSet.ANSI;

            try
            {
                // Получаем данные хмл-настроек
                LoadTXTReportSettings();

                // Обработка файлов
                PumpFiles();

                // Формирование результата разбора файлов

                // Датасет с данными отчетов
                resultDataSet = this.resultDS;

                // Список файлов
                filesLists = GetFilesLists();

                // Фиксированные параметры
                fixedParameters = this.fixedParams;

                // Кодировка
                charSet = this.txtReportSettings.GlobalSettings.CharSet;
            }
            finally
            {
                DisposeLists();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        /// <summary>
        /// Формирует список обработанных файлов отчетов для клиента
        /// </summary>
        private List<FileInfo[]> GetFilesLists()
        {
            List<FileInfo[]> filesLists = new List<FileInfo[]>(this.repFilesLists.Count);

            for (int i = 0; i < this.repFilesLists.Count; i++)
            {
                filesLists.Add(new FileInfo[this.repFilesLists[i].GetLength(0)]);
                Array.Copy(this.repFilesLists[i], filesLists[i], this.repFilesLists[i].GetLength(0));
            }

            return filesLists;
        }

        /// <summary>
        /// Заполняет список файлов для закачки
        /// </summary>
        private void FillRepFilesList()
        {
            int filesCount = -1;

            // Если отчет состоит из одного файла, то выбираем все файлы
            if (txtReportSettings.GlobalSettings.PartsWildCards.Count == 0)
            {
                FileInfo[] files = new FileInfo[0];
                for (int i = 0; i < this.txtReportSettings.GlobalSettings.ReportWildCards.Count; i++)
                {
                    files = (FileInfo[])CommonRoutines.ConcatArrays(files, repFilesDir.GetFiles(
                        this.txtReportSettings.GlobalSettings.ReportWildCards[i],
                        SearchOption.AllDirectories));
                }
                this.repFilesLists.Add(files);
            }
            else
            {
                // Для каждой маски части файла формируем отдельный список файлов
                for (int i = 0; i < txtReportSettings.GlobalSettings.PartsWildCards.Count; i++)
                {
                    this.repFilesLists.Add(repFilesDir.GetFiles(
                        txtReportSettings.GlobalSettings.PartsWildCards[i], SearchOption.AllDirectories));

                    if (filesCount < 0)
                    {
                        filesCount = this.repFilesLists[i].GetLength(0);
                    }

                    if (this.repFilesLists[i].GetLength(0) != filesCount)
                    {
                        if (filesCount >= 0)
                        {
                            throw new FilesNotFoundException(
                                "Количество файлов одной части отчета не совпадает с количеством файлов " +
                                "другой части отчета");
                        }
                    }
                    if (filesCount == 0)
                    {
                        throw new FilesNotFoundException("Отсутствуют файлы, удовлетворяющие одной из масок");
                    }
                }
            }

            if (this.repFilesLists[0].GetLength(0) == 0)
            {
                throw new FilesNotFoundException(string.Format(
                    "В указанном каталоге файлы для закачки не обнаружены. " +
                    "Файлы должны удовлетворять одной из следующих масок: {0}",
                    string.Join(", ", this.txtReportSettings.GlobalSettings.ReportWildCards.ToArray())));
            }
        }

        /// <summary>
        /// Возвращает список, построенный из файлов - частей отчета
        /// </summary>
        /// <param name="index">Номер файла в списке</param>
        /// <returns>Список файлов честей отчета</returns>
        private FileInfo[] GetSiblingFiles(int index)
        {
            FileInfo[] result = new FileInfo[this.repFilesLists.Count];

            for (int i = 0; i < this.repFilesLists.Count; i++)
            {
                result[i] = this.repFilesLists[i][index];
            }

            return result;
        }

        /// <summary>
        /// Проверяет, все ли файлы - части отчета еще есть, т.е. удалили их или нет
        /// </summary>
        /// <param name="repPartsList">Список файлов</param>
        /// <returns>true - все на месте</returns>
        private bool CheckFilesExists(FileInfo[] repPartsList)
        {
            for (int i = 0; i < repPartsList.GetLength(0); i++)
            {
                if (!File.Exists(repPartsList[i].FullName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Закачивает файлы отчета
        /// </summary>
        /// <param name="files">Файлы отчета</param>
        private void PumpFiles()
        {
            // Загрузка списка файлов
            repFilesLists = new List<FileInfo[]>(this.txtReportSettings.GlobalSettings.PartsWildCards.Count);
            FillRepFilesList();

            this.fixedParams.Clear();

            // Создаем датасет для хранения данных отчетов
            InitResultDataSet();
            this.currentReportIndex = 0;
            this.currentReportTableIndex = 0;

            int filesCount = this.repFilesLists[0].GetLength(0);

            // Обходим все файлы
            for (int i = 0; i < filesCount; i++)
            {
                bool pumpFile = false;
                try
                {
                    // Получаем список файлов - частей одного отчета
                    FileInfo[] repPartsList = GetSiblingFiles(i);

                    // Проверяем наличие всех файлов отчета
                    if (!CheckFilesExists(repPartsList))
                    {
                        continue;
                    }
                    Trace.WriteLine(string.Format("Обработка файлов {0}.", MakeReportFielsList(i)), "TXTSorcerer");

                    // Инициализация внутренних структур для хранения результатов анализа файлов
                    InitTXTAnalysisResult(repPartsList.GetLength(0));
                    InitLists();

                    // Анализируем содержимое полученных файлов и записываем результат в структуры
                    pumpFile = AnalyzeFiles(repPartsList);
                    if (!pumpFile)
                        continue;

                    // В датасете создаем таблицу для хранения данных отчета и строку с информацией о закачиваемых файлах
                    AddRowToRepFilesTable(i);

                    // Переносим данные из структур в таблицу
                    MoveFilesDataToDataTable();

                    if (this.dataPumpModule != null)
                    {
                        this.dataPumpModule.SetProgress(filesCount, i + 1,
                            string.Format("Источник {0}. Анализ файлов отчетов...", repFilesDir.FullName),
                            string.Format("Отчет {0} из {1}", i + 1, filesCount), true);
                    }
                }
                catch (Exception ex)
                {
                    this.resultDS.Tables["ReportFiles"].Rows[currentFileIndex]["INFO"] = ex.Message;
                    if (this.resultDS.Tables.Count > this.currentReportIndex + 1)
                    {
                        this.resultDS.Tables[this.currentReportIndex + 1].Clear();
                    }
                    Trace.WriteLine("ERROR: " + ex.ToString(), "TXTSorcerer");
                }
                finally
                {
                    if (pumpFile)
                    {
                        this.currentReportIndex++;
                        this.resultDS.Tables["ReportFiles"].Rows[currentFileIndex]["ENDTIME"] = DateTime.Now;
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает строку с названиями закачиваемых файлов
        /// </summary>
        /// <returns>Строка с названиями закачиваемых файлов</returns>
        private string MakeReportFielsList(int fileIndex)
        {
            string result = string.Empty;

            for (int i = 0; i < this.repFilesLists.Count; i++)
            {
                result += this.repFilesLists[i][fileIndex].Name + "; ";
            }

            if (result != string.Empty)
            {
                result = result.Remove(result.Length - 2);
            }

            return result;
        }

        /// <summary>
        /// Добавляет строку в таблицу сведений о закачанных отчетах
        /// </summary>
        private void AddRowToRepFilesTable(int fileIndex)
        {
            DataRow row = this.resultDS.Tables["ReportFiles"].NewRow();

            row[fileIndFieldName] = this.currentReportIndex;
            row["FILES"] = MakeReportFielsList(fileIndex);
            row["STARTTIME"] = DateTime.Now;
            row["ROWSCOUNT"] = 0;

            this.resultDS.Tables["ReportFiles"].Rows.Add(row);

            currentFileIndex++;
        }

        #endregion Общая организация обработки файлов


        #region Разбор текстового файла, формирование внутренних структур данных

        /// <summary>
        /// Анализирует содержимое полученных файлов и записывает результат в структуры
        /// </summary>
        /// <param name="repPartList">Список файлов - частей одного отчета</param>
        private bool AnalyzeFiles(FileInfo[] repPartsList)
        {
            if (repPartsList.GetLength(0) == 0)
            {
                throw new FilesNotFoundException("Отсутствуют файлы для анализа.");
            }

            // Загрузка файла во внутренние структуры
            if (!LoadFiles(repPartsList))
                return false;
             
            // Анализ содержимого файла.
            // Определение позиций разделителей таблицы
            ScanDelimiters(repPartsList);

            // Загрузка фиксированных параметров
            LoadFixedParameters();

            return true;
        }

        /// <summary>
        /// Возвращает массив строк указанного файла
        /// </summary>
        /// <param name="files"></param>
        /// <returns>Массив строк указанного файла</returns>
        private string[] GetFileContent(FileInfo file)
        {
            string[] fileContent = new string[0];

            FileStream fs = file.OpenRead();
            StreamReader sr = null;

            try
            {
                // Определяемся с кодировкой файла
                if (this.txtReportSettings.GlobalSettings.CharSet == CharacterSet.ANSI)
                {
                    sr = new StreamReader(fs);
                }
                else if (this.txtReportSettings.GlobalSettings.CharSet == CharacterSet.OEM)
                {
                    sr = new StreamReader(fs, Encoding.GetEncoding(866));
                }

                // Читаем содержимое файла
                fileContent = sr.ReadToEnd().Split(new string[] { "\r" }, StringSplitOptions.None);
                if (fileContent.GetLength(0) == 0)
                {
                    fileContent = sr.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                }

                if (fileContent.GetLength(0) > constMaxRows)
                {
                    throw new TextRepAnalysisFailedException(string.Format(
                        "Количество строк {0} файла {1} превышает максимальный размер буфера {2}",
                        fileContent.GetLength(0), file.Name, constMaxRows));
                }
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }

            return fileContent;
        }

        /// <summary>
        /// Определяет, пропускать файл или нет
        /// </summary>
        /// <param name="fileContentStr">Строка файла</param>
        /// <returns>Пропускать или нет</returns>
        private bool ExcludeFile(FileInfo file, string fileContentStr)
        {
            foreach (string str in this.txtReportSettings.GlobalSettings.ExcludeFilesByString)
            {
                if (fileContentStr.ToUpper().Contains(str.ToUpper()))
                {
                    Trace.WriteLine(string.Format(
                        "Файл {0} пропущен, т.к. в нем найдено вхождение подстроки \"{1}\".",
                        file.Name, str), "TXTSorcerer");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Загружает файлы отчета во внутренние структуры
        /// </summary>
        /// <param name="files">Файлы отчета</param>
        private bool LoadFiles(FileInfo[] files)
        {
            InitTextDataArray();

            int colsCnt = 0;
            this.totalColsCount = 0;
            this.totalRowsCount = 0;

            // Загружаем все части отчета в один массив символов
            for (int i = 0; i < files.GetLength(0); i++)
            {
                string[] fileContent = GetFileContent(files[i]);
                if (ExcludeFile(files[i], string.Join(" ", fileContent)))
                    return false;

                int length = fileContent.GetLength(0);

                // Определяем номер строки массива данных отчета, с которой будет производиться загрузка
                int dataArrayIndex = 0;
                if (this.txtReportSettings.GlobalSettings.PartsUnionType == PartsUnionType.Horizontal)
                {
                    dataArrayIndex = this.totalRowsCount;
                    this.txtAnalysisResult.PartStartMargins[i] = this.totalRowsCount;
                }
                else
                {
                    this.txtAnalysisResult.PartStartMargins[i] = this.totalColsCount;
                }

                this.txtAnalysisResult.PartRowsCount[i] = length;

                // Перегоняем массив строк в двумерный массив символов
                for (int j = 0; j < length; j++)
                {
                    string fileContentStr = fileContent[j].Trim('\n');

                    if (this.txtReportSettings.GlobalSettings.PartsUnionType == PartsUnionType.Horizontal)
                    {
                        textDataStrings.Add(fileContentStr);
                    }
                    else
                    {
                        if (textDataStrings.Count == j)
                        {
                            textDataStrings.Add(fileContentStr);
                        }
                        else
                        {
                            textDataStrings[j].PadRight(this.totalColsCount, ' ');
                            textDataStrings[j] += fileContentStr;
                        }
                    }

                    // Запоминаем максмальное количество столбцов
                    if (fileContentStr.Length > colsCnt && fileContentStr.Length <= constMaxCols)
                    {
                        colsCnt = fileContentStr.Length;
                    }

                    // Ищем верхнюю границу таблицы (чтобы отсеять мелкие вспомогательные таблички вверху отчета)
                    if (this.firstTableTopMargin == 0)
                    {
                        if (CheckStringForMargin(fileContentStr))
                        {
                            this.firstTableTopMargin = j;
                        }
                    }

                    dataArrayIndex++;
                }

                // Запоминаем количество строк файла
                if (this.txtReportSettings.GlobalSettings.PartsUnionType == PartsUnionType.Horizontal)
                {
                    this.totalRowsCount += length;
                }
                else
                {
                    if (length > this.totalRowsCount)
                        this.totalRowsCount = length;
                }

                // Запоминаем количество столбцов файла
                switch (this.txtReportSettings.GlobalSettings.PartsUnionType)
                {
                    case PartsUnionType.Horizontal:
                        if (colsCnt > this.totalColsCount)
                            this.totalColsCount = colsCnt;
                        break;

                    default:
                        this.totalColsCount += colsCnt;
                        break;
                }

                colsCnt = 0;

                this.txtAnalysisResult.PartEndMargins[i] = this.totalColsCount;
            }

            return true;
        }

        /// <summary>
        /// Ищет верхнюю границу таблицы отчета
        /// </summary>
        /// <returns>true - данная строка содержит разделитель</returns>
        private bool CheckStringForMargin(string str)
        {
            Encoding en = Encoding.GetEncoding(866);

            // Считаем количество разделителей в строке
            double delimsCount = 0;
            for (int i = 0; i < str.Length; i++)
            {
                byte[] bb = en.GetBytes(new char[] { str[i] });
                if (bb.GetLength(0) == 0)
                {
                    throw new TextRepAnalysisFailedException("Невозможно перекодировать символ.");
                }

                if (this.txtReportSettings.GlobalSettings.HorzDelimiters.Contains(bb[0]))
                {
                    delimsCount++;
                }
            }
            if (delimsCount / str.Length > 0.8) return true;

            return false;
        }

        /// <summary>
        /// Проверяет количество столбцов таблицы, полученное в результате разбора, с указанным файле настроек
        /// </summary>
        /// <param name="tableInfo">Данные, полученные в результате разбора</param>
        private void VerifyColumnsAmount(ParsedTableInfo tableInfo)
        {
            int count = tableInfo.GetVertDelimitersCount();

            // Надо учесть, что количество вертикальных разделителей на 1 больше количества столбцов
            bool a = (this.txtReportSettings.GlobalSettings.PartsWildCards.Count == 0 || 
                this.txtReportSettings.GlobalSettings.PartsUnionType != PartsUnionType.Vertical) &&
                count != tableInfo.Table.TableColumns.Columns.Count + 1;

            bool b = this.txtReportSettings.GlobalSettings.PartsWildCards.Count > 0 &&
                this.txtReportSettings.GlobalSettings.PartsUnionType == PartsUnionType.Vertical &&
                count != tableInfo.Table.TableColumns.Columns.Count +
                    this.txtReportSettings.GlobalSettings.PartsWildCards.Count;

            if (a || b)
            {
                throw new TextRepAnalysisFailedException(string.Format(
                    "Ошибка при определении количества столбцов таблицы {0} отчета.", tableInfo.Index + 1));
            }
        }

        /// <summary>
        /// Ищет границы таблицы по данным счетчиков разделителей
        /// </summary>
        /// <param name="startIndex">Индекс начальной строки таблицы</param>
        /// <param name="endIndex">Индекс конечной строки</param>
        /// <param name="maxHeight">Позиция последнего вертикального разделителя</param>
        /// <param name="maxWidth">Позиция последнего горизонтального разделителя</param>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        private void FindTableMargins(int startIndex, int endIndex, int maxHeight, int maxWidth, ParsedTableInfo tableInfo)
        {
            // Поиск горизонтальных границ
            for (int i = startIndex; i <= endIndex; i++)
            {
                double d = (double)tableInfo.TextRows[i].DelimitersCount / maxWidth;
                SetRowMarginValue(tableInfo.TextRows[i], d);
            }

            // Поиск вертикальных границ
            for (int i = 0; i < this.totalColsCount; i++)
            {
                double d = (double)tableInfo.TextColumns[i].DelimitersCount / maxHeight;
                SetColumnMarginValue(tableInfo.TextColumns[i], d);
            }
        }

        /// <summary>
        /// Ищет верхнюю границу столбца
        /// </summary>
        /// <param name="startRow">Строка, от которой начинается поиск</param>
        /// <returns>Индекс строки - верхней границы</returns>
        private void FindTableTopMargin(int startRow, ref ParsedTableInfo tableInfo)
        {
            // Заголовок может состоять из нескольких разделителей
            int tmp = tableInfo.Table.TableEntry.HeaderHeight;

            for (int i = startRow; i < this.totalRowsCount; i++)
            {
                if (tableInfo.TextRows[i].IsMargin)
                {
                    tmp--;
                }
                if (tmp == 0)
                {
                    tableInfo.DataAreaTopMargin = i + 1;
                    return;
                }
            }
        }

        /// <summary>
        /// Ищет границы таблицы по данным счетчиков разделителей
        /// </summary>
        /// <param name="startIndex">Индекс начальной строки таблицы</param>
        /// <param name="maxHeight">Максимальная высота столбца - нужно для определения столбцов таблицы</param>
        /// <param name="maxWidth">Максимальная ширина строки - нужно для определения границ таблицы</param>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        private int FindNextHorizMargin(int startIndex, ref int maxHeight, ref int maxWidth, ParsedTableInfo tableInfo)
        {
            // Поиск горизонтальных границ
            for (int i = startIndex; i < this.totalRowsCount; i++)
            {
                // Oпределяем вертикальные и горизонтальные разделители в текущей строке
                DetermineDelimiters(tableInfo, i, ref maxHeight, ref maxWidth);

                double d = (double)tableInfo.TextRows[i].DelimitersCount / maxWidth;
                SetRowMarginValue(tableInfo.TextRows[i], d);

                if (tableInfo.TextRows[i].IsMargin)
                {
                    return i;
                }
            }

            return this.totalRowsCount;
        }

        /// <summary>
        /// Ищет границы таблицы по данным счетчиков разделителей
        /// </summary>
        /// <param name="startIndex">Индекс начальной строки таблицы</param>
        private int FindNextHorizMargin(int startIndex)
        {
            int maxWidth = 0;

            // Поиск горизонтальных границ
            for (int i = startIndex; i < this.totalRowsCount; i++)
            {
                string row = this.textDataStrings[i];
                int delimsCount = 0;

                for (int j = 0; j < this.totalColsCount; j++)
                {
                    if (row.Length <= j)
                        break;

                    byte[] bb = encoding866.GetBytes(new char[] { this.textDataStrings[i][j] });
                    if (bb.GetLength(0) == 0)
                    {
                        throw new TextRepAnalysisFailedException(string.Format(
                            "Невозможно перекодировать символ {0} (строка {1}, столбец {2})", this.textDataStrings[i][j], i, j));
                    }

                    // Если символ входит в список горизонтальных разделителей
                    if (this.txtReportSettings.GlobalSettings.HorzDelimiters.Contains(bb[0]))
                    {
                        delimsCount++;
                        if (delimsCount > maxWidth)
                        {
                            maxWidth = delimsCount;
                        }
                    }
                }

                if (maxWidth > 0)
                {
                    double d = (double)delimsCount / maxWidth;
                    if (d >= constHorzMarginDelimsPercent)
                    {
                        return i;
                    }
                }
            }

            return this.totalRowsCount;
        }

        /// <summary>
        /// Устанавливает нижнюю границу таблицы
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="rowIndex">Индекс строки</param>
        private void SetTableDataAreaBottomMargin(ref ParsedTableInfo tableInfo, int rowIndex)
        {
            // Включать строку с признаком конца таблицы в датасет с данными или нет
            if (tableInfo.Table.TableEntry.IncludeEndTableMarker)
            {
                tableInfo.DataAreaBottomMargin = rowIndex;
            }
            else
            {
                tableInfo.DataAreaBottomMargin = rowIndex - 1;
            }
        }

        /// <summary>
        /// Поиск нижней границы текущей таблицы
        /// </summary>
        /// <param name="startIndex">Индекс начальной строки таблицы</param>
        /// <param name="rowIndex">Индекс текущей строки</param>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="maxHeight">Максимальная высота столбца - нужно для определения столбцов таблицы</param>
        /// <param name="maxWidth">Максимальная ширина строки - нужно для определения границ таблицы</param>
        private void FindTableBottomMargin(ref int startIndex, ref int rowIndex, ref ParsedTableInfo tableInfo, ref int maxHeight, ref int maxWidth)
        {
            // Если признак окончания находится внутри таблцы, то просто берем предыдущую строку
            if (tableInfo.Table.TableEntry.InnerEndTableMarker)
            {
                SetTableDataAreaBottomMargin(ref tableInfo, rowIndex);
                rowIndex = FindNextHorizMargin(rowIndex, ref maxHeight, ref maxWidth, tableInfo);

                tableInfo.BottomMargin = rowIndex;
            }
            else
            {
                // Если признак окончания таблицы находится вне ее, то ищем нижнюю границу как
                // разделитель, предшествующий найденной позиции
                if (tableInfo.Table.TableEntry.TableDelimited)
                {
                    int prevMargin = FindPrevHorizMargin(tableInfo, rowIndex);
                    SetTableDataAreaBottomMargin(ref tableInfo, prevMargin);

                    tableInfo.BottomMargin = prevMargin;
                }
                else
                {
                    tableInfo.DataAreaBottomMargin = rowIndex - tableInfo.Table.TableEntry.EndTableMarkerSpace;
                    tableInfo.BottomMargin = rowIndex;
                }
            }
        }

        /// <summary>
        /// Определяет верхнюю и нижнюю границы текущей таблицы
        /// </summary>
        /// <param name="startIndex">Индекс начальной строки таблицы</param>
        /// <param name="rowIndex">Индекс текущей строки</param>
        /// <param name="colIndex">Индекс текущего столбца</param>
        /// <param name="maxHeight">Позиция последнего вертикального разделителя</param>
        /// <param name="maxWidth">Позиция последнего горизонтального разделителя</param>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        private bool CheckTableMargins(ref int startIndex, ref int rowIndex, ref int maxHeight, ref int maxWidth, ref ParsedTableInfo tableInfo)
        {
            // Если текущая строка является маркером конца таблицы, то определяем все разделители таблицы и границы ее области данных
            if (this.textDataStrings[rowIndex].ToUpper().Contains(tableInfo.Table.TableEntry.EndTableMarker.ToUpper()))
            {
                // Определяем разделители таблицы
                FindTableMargins(startIndex, rowIndex, maxHeight, maxWidth, tableInfo);

                // Проверяем количество столбцов таблицы, полученное в результате разбора, с указанным файле настроек
                VerifyColumnsAmount(tableInfo);

                // Определяем верхнюю границу
                FindTableTopMargin(startIndex, ref tableInfo);

                // Определяем нижнюю границу
                FindTableBottomMargin(ref startIndex, ref rowIndex, ref tableInfo, ref maxHeight, ref maxWidth);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Определяет, является ли символ горизонтальным разделителем
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="maxWidth">Максимальная ширина строки - нужно для определения границ таблицы</param>
        /// <param name="rowIndex">Индекс строки</param>
        /// <param name="bb">Символ</param>
        private void DetermineHorizDelimiter(ParsedTableInfo tableInfo, ref int maxWidth, int rowIndex, byte[] bb)
        {
            // Если символ входит в список горизонтальных разделителей
            if (this.txtReportSettings.GlobalSettings.HorzDelimiters.Contains(bb[0]))
            {
                tableInfo.TextRows[rowIndex].DelimitersCount++;
                int count = tableInfo.TextRows[rowIndex].DelimitersCount;

                if (count > maxWidth)
                {
                    maxWidth = count;
                }
            }
        }

        /// <summary>
        /// Определяет, является ли символ вертикальным разделителем
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="maxHeight">Максимальная высота столбца - нужно для определения столбцов таблицы</param>
        /// <param name="colIndex">Индекс столбца</param>
        /// <param name="bb">Символ</param>
        private void DetermineVertDelimiter(ParsedTableInfo tableInfo, ref int maxHeight, int colIndex, byte[] bb)
        {
            // Если символ входит в список вертикальных разделителей
            if (this.txtReportSettings.GlobalSettings.VertDelimiters.Contains(bb[0]))
            {
                tableInfo.TextColumns[colIndex].DelimitersCount++;
                int count = tableInfo.TextColumns[colIndex].DelimitersCount;

                if (count > maxHeight)
                {
                    maxHeight = count;
                }
            }
        }

        /// <summary>
        /// Определяет вертикальные и горизонтальные разделители в текущей строке
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="rowIndex">Номер строки</param>
        /// <param name="maxHeight">Максимальная высота столбца - нужно для определения столбцов таблицы</param>
        /// <param name="maxWidth">Максимальная ширина строки - нужно для определения границ таблицы</param>
        private void DetermineDelimiters(ParsedTableInfo tableInfo, int rowIndex, ref int maxHeight, ref int maxWidth)
        {
            string row = this.textDataStrings[rowIndex];

            for (int j = 0; j < this.totalColsCount; j++)
            {
                if (row.Length <= j) return;

                byte[] bb = encoding866.GetBytes(new char[] { this.textDataStrings[rowIndex][j] });
                if (bb.GetLength(0) == 0)
                {
                    throw new TextRepAnalysisFailedException(string.Format(
                        "Невозможно перекодировать символ {0} (строка {1}, столбец {2})", this.textDataStrings[rowIndex][j], rowIndex, j));
                }

                // Границы таблицы здесь не определяются, т.к. еще не известны общая ширина и высота таблицы

                // Определяем, является ли символ горизонтальным разделителем
                DetermineHorizDelimiter(tableInfo, ref maxWidth, rowIndex, bb);

                // Определяем, является ли символ вертикальным разделителем
                DetermineVertDelimiter(tableInfo, ref maxHeight, j, bb);
            }
        }

        /// <summary>
        /// Проверяет количество оставшихся таблиц отчета для разбора
        /// </summary>
        /// <param name="tablesAmount">Количество оставшихся таблиц</param>
        /// <returns>Что-то осталось или нет</returns>
        private bool CheckTablesAmount(ref int tablesAmount)
        {
            if (tablesAmount < 0) return true;

            tablesAmount--;
            if (tablesAmount == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Сканирование границ и разделителей таблицы
        /// </summary>
        /// <param name="files">Файлы отчета</param>
        private void ScanDelimiters(FileInfo[] files)
        {
            // Максимальная высота столбца - нужно для определения столбцов таблицы
            int maxHeight = 0;
            // Максимальная ширина строки - нужно для определения границ таблицы
            int maxWidth = 0;

            // Индекс текущей таблицы
            int tableIndex = 0;
            // Настройки таблицы
            Table table = this.txtReportSettings.GetTable(files, tableIndex, this.repFilesDir);

            // Данные разбора таблицы
            ParsedTableInfo tableInfo = AddParsedTableInfo(tableIndex, table);
            tableInfo.TopMargin = this.firstTableTopMargin;
            int startIndex = this.firstTableTopMargin;

            int tablesAmount = this.txtReportSettings.Tables.TablesAmount;

            // Описание алгоритма. 
            // Бежим по всем символам строк файла и определяем горизонтальные и вертикальные разделители.
            // При обнаружении строки-маркера окончания таблицы запускаем определение вертикальных и горизонтальных границ
            // в найденной области (между маркером и самым верхним горизонтальным разделителем). Таблица определена,
            // идем дальше - повторяем алгоритм.

            // Считаем количество разделителей в каждой строке и столбце и определяем границы
            for (int i = this.firstTableTopMargin; i < this.totalRowsCount; i++)
            {
                // Oпределяем вертикальные и горизонтальные разделители в текущей строке
                DetermineDelimiters(tableInfo, i, ref maxHeight, ref maxWidth);

                // Находим границы таблицы
                if (CheckTableMargins(ref startIndex, ref i, ref maxHeight, ref maxWidth, ref tableInfo))
                {
                    // Если нижняя граница найдена - текущая таблица определена, ищем описание параметров для следующей

                    // Ищем начало следующей таблицы
                    i++;

                    // Ищем верхнюю границу следующей таблицы
                    startIndex = FindNextHorizMargin(i);
                    // Если нашли, то начинаем со строки, слудующей за границей
                    i = startIndex - 1;

                    // Если есть следующая граница, то добавляем еще одну таблицу
                    if (CheckTablesAmount(ref tablesAmount) && FindNextHorizMargin(startIndex) < this.totalRowsCount)
                    {
                        tableIndex++;
                        table = this.txtReportSettings.GetTable(files, tableIndex, this.repFilesDir);
                        tableInfo = AddParsedTableInfo(tableIndex, table);
                        tableInfo.TopMargin = startIndex;
                    }
                    else
                    {
                        return;
                    }

                    maxHeight = 0;
                    maxWidth = 0;
                }
            }
        }

        /// <summary>
        /// Возвращает значение фиксированного параметра
        /// </summary>
        /// <param name="fp">Фиксированный параметр</param>
        /// <param name="tableTopMargin">Верхняя граница таблицы для загрузки относительных параметров</param>
        /// <returns>Значение</returns>
        private string GetFixedParamValue(FixedParamPos fp, int tableTopMargin)
        {
            switch (fp.FixedParamKind)
            {
                case FixedParamKind.Single:
                    return GetFileFragment(
                        fp.BeginPosX + txtAnalysisResult.PartStartMargins[fp.PartNo],
                        fp.BeginPosY,
                        fp.EndPosX + txtAnalysisResult.PartStartMargins[fp.PartNo],
                        fp.EndPosY);

                case FixedParamKind.BeforeTable:
                    if (tableTopMargin >= 0)
                    {
                        return GetFileFragment(
                            fp.BeginPosX + txtAnalysisResult.PartStartMargins[fp.PartNo],
                            tableTopMargin - fp.EndPosY - 1,
                            fp.EndPosX + txtAnalysisResult.PartStartMargins[fp.PartNo],
                            tableTopMargin - fp.BeginPosY - 1);
                    }
                    break;
            }

            return string.Empty;
        }

        /// <summary>
        /// Загружает значения фиксированных параметров (FixedParamKind == FixedParamKind.Single)
        /// </summary>
        private void LoadFixedParameters()
        {
            FixedParameter fixedParameter;

            for (int i = 0; i < this.txtReportSettings.FixedParamsPos.Count; i++)
            {
                FixedParamPos fp = this.txtReportSettings.FixedParamsPos[i];

                // Добавляем новый элемент списка фиксированных параметров
                if (!this.fixedParams.ContainsKey(this.currentReportIndex))
                {
                    this.fixedParams.Add(this.currentReportIndex, new Dictionary<string, FixedParameter>(20));
                }

                // Ищем значение фиксированных параметров
                switch (fp.FixedParamKind)
                {
                    case FixedParamKind.Single:
                        if (fp.DBField == string.Empty)
                        {
                            fixedParameter = new FixedParameter(fp.Caption, GetFixedParamValue(fp, -1));

                            if (fixedParameter.Value == string.Empty)
                            {
                                fixedParameter.Value = fp.DefaultValue;
                            }
                            else
                            {
                                // Приводим значение параметра к стандартному виду
                                fixedParameter.Value = CorrectFixedParamValue(fp.Name, fixedParameter.Value);
                            }

                            if (this.fixedParams[this.currentReportIndex].ContainsKey(fp.Name))
                            {
                                this.fixedParams[this.currentReportIndex][fp.Name] = fixedParameter;
                            }
                            else
                            {
                                this.fixedParams[this.currentReportIndex].Add(fp.Name, fixedParameter);
                            }
                        }
                        break;

                    case FixedParamKind.BeforeTable:
                        break;
                }
            }
        }

        /// <summary>
        /// Приводит значение параметра к стандартному виду
        /// </summary>
        /// <param name="name">Параметр</param>
        private string CorrectFixedParamValue(string name, string value)
        {
            if (name == fxpReportDate)
            {
                // Преобразуем дату
                string tempStr = value.Split('-')[0].Replace("_", string.Empty);
                tempStr = CommonRoutines.TrimLetters(tempStr);
                if (tempStr.Length > 10)
                    tempStr = tempStr.Remove(10);
                return CommonRoutines.ShortDateToNewDate(tempStr).ToString();
            }
            else if (name == fxpReportDateEx)
            {
                // Преобразуем дату
                return CommonRoutines.LongDateToNewDate(
                    CommonRoutines.TrimLetters(value.Replace("_", string.Empty)));
            }
            else if (name == fxpTaxOrgan)
            {
                // Убираем лишние знаки
                return value.Replace("_", string.Empty).Replace("  ", string.Empty);
            }
            else if (name == fxpCodeOnly)
            {
                // Убираем все буквы справа и слева - оставляем только код
                return CommonRoutines.TrimLetters(value);
            }
            else if (name == fxpNameOnly)
            {
                // Убираем все цифры справа и слева - оставляем только наименование
                return CommonRoutines.TrimNumbers(value);
            }
            else
            {
                return value.Trim((char)0).Trim();
            }
        }

        #endregion Разбор текстового файла, формирование внутренних структур данных


        #region Анализ внутренних структур, перенос данных в DataTable

        /// <summary>
        /// Переносит данные из структур в таблицу
        /// </summary>
        private void MoveFilesDataToDataTable()
        {
            for (int i = 0; i < this.tableInfoList.Count; i++)
            {
                // Добавляем таблицу для данных отчета
                AddFileDataTable(tableInfoList[i].Table);

                // Ищем верхнюю и нижнюю границы столбцов
                AriseTableColumns(tableInfoList[i]);

                // Переносим данные в DataTable
                MoveDataToDataTable(tableInfoList[i]);

                // Корректируем строки с переносами
                CorrectRowsWithDivisions(tableInfoList[i]);
            }
        }

        /// <summary>
        /// Ищет верхнюю и нижнюю границы столбцов
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        private void AriseTableColumns(ParsedTableInfo tableInfo)
        {
            int leftPos = 0;
            int marginsCount = 0;
            int columnIndex = 0;

            int partLastColumnIndex = GetPartLastColumn(tableInfo, 0);
            this.txtAnalysisResult.ArisedColumns.Clear();

            for (int i = 0; i < tableInfo.TextColumns.Count; i++)
            {
                // Анализируем разделители
                if (tableInfo.TextColumns[i].IsMargin)
                {
                    marginsCount++;

                    if (marginsCount == 1)
                    {
                        leftPos = i;
                    }

                    if (i > leftPos && marginsCount > 1)
                    {
                        // Добавляем столбец
                        ArisedColumn column = new ArisedColumn();
                        column.ID = columnIndex;
                        column.LeftPos = leftPos;
                        column.TopPos = tableInfo.DataAreaTopMargin;
                        column.RightPos = i;
                        column.BottomPos = tableInfo.DataAreaBottomMargin;
                        this.txtAnalysisResult.ArisedColumns.Add(column);

                        // Если это последний столбец части отчета, то объединяем части
                        if (this.txtReportSettings.GlobalSettings.PartsWildCards.Count > 0 && (columnIndex == partLastColumnIndex))
                        {
                            JoinParts(tableInfo, partLastColumnIndex);
                            partLastColumnIndex = GetPartLastColumn(tableInfo, partLastColumnIndex + 1);
                        }

                        leftPos = i;
                        columnIndex++;
                    }
                }
            }

            if (this.txtAnalysisResult.ArisedColumns.Count != tableInfo.Table.TableColumns.Columns.Count)
            {
                throw new TextRepAnalysisFailedException(
                    "Количество столбцов, полученных в результате разбора файла, не совпадает с заявленным в XML.");
            }
        }

        /// <summary>
        /// Ищет столбец, последний в текущей части отчета
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="index">Индекс столбца, от которого начинать поиск</param>
        /// <returns>Индекс последнего столбца в текущей части отчета</returns>
        private int GetPartLastColumn(ParsedTableInfo tableInfo, int index)
        {
            // Среди всех столбцов ищем последний в той части отчета, в которой находится текущий столбцец с индексом index
            if (index <= tableInfo.Table.TableColumns.Columns.Count)
            {
                int partNo = tableInfo.Table.TableColumns.Columns[index].PartNo;

                for (int i = index; i < tableInfo.Table.TableColumns.Columns.Count; i++)
                {
                    if (tableInfo.Table.TableColumns.Columns[i].PartNo != partNo)
                    {
                        return i - 1;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Корректирует коррдинаты столбцов и разделителей после объединения частей отчета
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="startIndex">Индекс столбца, с которого начинается коррекция</param>
        /// <param name="length">Величина, на которую изменяются координаты</param>
        private void CorrectMargins(ParsedTableInfo tableInfo, int startIndex, int length)
        {
            for (int i = startIndex; i < this.totalColsCount; i++)
            {
                if (tableInfo.TextColumns[i].IsMargin)
                {
                    // Удаляем старые разделители
                    tableInfo.TextColumns[i].IsMargin = false;

                    // Добавляем новые разделители
                    tableInfo.TextColumns[i - length].IsMargin = true;
                }
            }
        }

        /// <summary>
        /// Объединяет части отчета
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="partLastColumn">Индекс последнего столбца в текущей части отчета</param>
        private void JoinParts(ParsedTableInfo tableInfo, int partLastColumn)
        {
            int startIndex = this.txtAnalysisResult.ArisedColumns[partLastColumn].RightPos + 1;
            // Длина вырезаемой части (промежуток между частями отчета)
            int length = FindNextVertMargin(tableInfo, startIndex) - startIndex + 1;

            if (length > 0)
            {
                for (int i = 0; i < this.textDataStrings.Count; i++)
                {
                    // Вырезаем часть строки - промежуток между частями отчета
                    string row = this.textDataStrings[i];

                    if (row.Length > startIndex + length)
                    {
                        row = row.Remove(startIndex, length);
                        this.textDataStrings[i] = row;
                    }
                }

                // Коррекция границ стобцов и разделителей
                CorrectMargins(tableInfo, startIndex, length);
            }
        }

        /// <summary>
        /// Инициализирует служебные поля для строки результирующей таблицы
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        private object[] PrepareServiceFields(ParsedTableInfo tableInfo)
        {
            object[] result = new object[4];
            // Устанавливаем значение поля индекса файла
            result[0] = this.fileIndFieldName;
            result[1] = this.currentReportIndex;
            // Устанавливаем значение поля индекса таблицы в отчете
            result[2] = this.tableIndFieldName;
            result[3] = tableInfo.Index;

            // Ищем все фиксированные параметры, подлежащие записи в строки таблицы и счиатываем их значения
            for (int i = 0; i < this.txtReportSettings.FixedParamsPos.Count; i++)
            {
                FixedParamPos fp = this.txtReportSettings.FixedParamsPos[i];

                switch (fp.FixedParamKind)
                {
                    case FixedParamKind.Single:
                        if (fp.DBField != string.Empty)
                        {
                            Array.Resize(ref result, result.GetLength(0) + 2);
                            result[result.GetLength(0) - 2] = fp.DBField;
                            result[result.GetLength(0) - 1] =
                                CorrectFixedParamValue(fp.Name, GetFixedParamValue(fp, -1));
                        }
                        break;

                    case FixedParamKind.BeforeTable:
                        if (tableInfo.TopMargin >= 0 && fp.DBField != string.Empty)
                        {
                            Array.Resize(ref result, result.GetLength(0) + 2);
                            result[result.GetLength(0) - 2] = fp.DBField;
                            result[result.GetLength(0) - 1] =
                                CorrectFixedParamValue(fp.Name, GetFixedParamValue(fp, tableInfo.TopMargin));
                        }
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Переносит данные из внутренних структур в DataTable
        /// </summary>
        /// <param name="tableInfo">Информация о вертикальных и горизонтальных линиях одной таблицы отчета</param>
        private void MoveDataToDataTable(ParsedTableInfo tableInfo)
        {
            // Т.к. верхняя и нижняя граница у всех столбцов одинакова, то берем их у первого столбца
            int topPos = this.txtAnalysisResult.ArisedColumns[0].TopPos;
            int bottomPos = this.txtAnalysisResult.ArisedColumns[0].BottomPos;
            object[] serviceFields = PrepareServiceFields(tableInfo);

            DataTable dt = this.resultDS.Tables[this.currentReportTableIndex];

            for (int i = topPos; i <= bottomPos; i++)
            {
                // Если текущая строка - разделитель или заголовок (такой с циферками посреди таблицы), пропускаем ее
                if (!CheckRow(tableInfo, topPos, bottomPos, ref i)) continue;

                // Признак пустой строки
                bool skipRow = true;

                // Массив значений ячеек строки
                object[] objArray = (object[])CommonRoutines.ConcatArrays(
                    new object[this.txtAnalysisResult.ArisedColumns.Count * 2], serviceFields);

                for (int j = 0; j < this.txtAnalysisResult.ArisedColumns.Count; j++)
                {
                    ArisedColumn arisedColumn = this.txtAnalysisResult.ArisedColumns[j];
                    TableColumn tableColumn = tableInfo.Table.TableColumns.Columns[j];

                    objArray[j * 2] = tableColumn.DBField;

                    // Получаем значение ячейки строки
                    string value = GetFileFragment(arisedColumn.LeftPos + 1, i, arisedColumn.RightPos - 1, i).Trim();

                    if (value != string.Empty)
                    {
                        skipRow = false;
                    }

                    // Проверка значения
                    if (!ValidateCellValue(i, ref tableColumn, ref value))
                    {
                        skipRow = true;
                        break;
                    }

                    if (value != string.Empty)
                    {
                        objArray[j * 2 + 1] = value;
                    }
                }

                // Если строка не пуста, то добавляем ее в таблицу
                if (!skipRow)
                {
                    PumpRow(dt, objArray);
                }
            }

            int count = Convert.ToInt32(this.resultDS.Tables["ReportFiles"].Rows[this.currentFileIndex]["ROWSCOUNT"]); 
            count += this.resultDS.Tables[this.currentReportIndex + 1].Rows.Count;
            this.resultDS.Tables["ReportFiles"].Rows[this.currentFileIndex]["ROWSCOUNT"] = count;
        }

        /// <summary>
        /// Проверяет значение ячейки отчета
        /// </summary>
        /// <param name="strNo">Номер строки отчета</param>
        /// <param name="tableColumn">TableColumn текущего столбца</param>
        /// <param name="value">Значение ячейки</param>
        private bool ValidateCellValue(int strNo, ref TableColumn tableColumn, ref string value)
        {
            try
            {
                if (tableColumn.ExcludedValues.Contains(value)) return false;

                // Подставляем значение по умолчанию, если ячейка пуста
                if (value == string.Empty)
                {
                    value = tableColumn.DefaultValue;

                    // Проверка поля nullable
                    if (!tableColumn.Nullable && value == string.Empty)
                    {
                        throw new Exception("поле должно иметь значение");
                    }
                }

                if (value == string.Empty) return true;

                // В зависимости от типа данных столбца проверяем формат его данных
                switch (tableColumn.DataType)
                {
                    case ColumnDataType.Double:
                        value = Convert.ToString(CommonRoutines.ReduceDouble(value));

                        // Для дробных чисел проверям длину значащей и дробной частей
                        if (tableColumn.DataPrecision.Significand > 0)
                        {
                            string[] parts = value.Insert(value.Length, ",").Split(',');

                            if (parts[0].Length > tableColumn.DataPrecision.Significand ||
                                parts[1].Length > tableColumn.DataPrecision.Fraction)
                            {
                                throw new TextRepAnalysisFailedException();
                            }
                        }
                        break;

                    case ColumnDataType.Integer:
                        value = Convert.ToString(CommonRoutines.ReduceInt(value));

                        if (tableColumn.DataPrecision.Significand > 0)
                        {
                            if (value.Contains(".") || value.Length > tableColumn.DataPrecision.Significand)
                            {
                                throw new TextRepAnalysisFailedException();
                            }
                        }
                        break;

                    case ColumnDataType.String:
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                // При несоответствии значения ячейки типу или точности генерится исключение, иначе подставляется значение по умолчанию, 
                // если оно есть.
                if (tableColumn.ExactCheck || tableColumn.DefaultValue == string.Empty)
                {
                    throw new TextRepAnalysisFailedException(string.Format(
                        "Ошибка при проверке значения ячейки отчета. Часть {0}, строка {1}, столбец \"{2}\" ({3}).",
                        tableColumn.PartNo + 1, strNo + 1, tableColumn.DBField, ex.Message));
                }
                else
                {
                    value = tableColumn.DefaultValue;
                    return true;
                }
            }
        }

        /// <summary>
        /// Определяет тип строки: если это разделитель или заголовок (такой с циферками посреди таблицы),
        /// то возвращает индекс строки, следующей за этим безобразием
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="topPos">Индекс первой строки</param>
        /// <param name="index">Индекс проверяемой строки</param>
        /// <returns>true - нормальная строка со значениями</returns>
        private bool CheckRow(ParsedTableInfo tableInfo, int topPos, int bottomPos, ref int index)
        {
            bool result = true;

            if (index == bottomPos && (tableInfo.TextRows[index].IsMargin || tableInfo.TextRows[index].IsShortMargin))
            {
                return false;
            }

            // В середине таблицы может встретиться какой-нить левый заголовок (номера столбов, например)
            if (tableInfo.Table.TableEntry.HeaderAmidTable && tableInfo.TextRows[index].IsMargin)
            {
                index++;
                while (!tableInfo.TextRows[index].IsMargin && index < constMaxRows - 2) index++;
                index++;
            }

            // Следует также учесть, что строки могут быть разделены
            if (tableInfo.TextRows[index].IsMargin || tableInfo.TextRows[index].IsShortMargin)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Возвращает массив столбцов с указанным типом переноса
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        /// <param name="divisionKind">Тип переноса</param>
        /// <returns>Массив столбцов</returns>
        private TableColumn[] GetTableColumnsByDivisionKind(ParsedTableInfo tableInfo, DivisionKind divisionKind)
        {
            TableColumn[] result = new TableColumn[0];

            for (int i = 0; i < tableInfo.Table.TableColumns.Columns.Count; i++)
            {
                if (tableInfo.Table.TableColumns.Columns[i].DivisionKind == divisionKind)
                {
                    Array.Resize(ref result, result.GetLength(0) + 1);
                    result[result.GetLength(0) - 1] = tableInfo.Table.TableColumns.Columns[i];
                }
            }

            return result;
        }

        /// <summary>
        /// Корректирует строки с переносами в ячейках
        /// </summary>
        /// <param name="tableInfo">Структура результатов разбора таблицы</param>
        private void CorrectRowsWithDivisions(ParsedTableInfo tableInfo)
        {
            DataTable dt = this.resultDS.Tables[this.currentReportTableIndex];

            // Получаем массив столбцов, в которых возможны переносы
            TableColumn[] divisionColumns = (TableColumn[])CommonRoutines.ConcatArrays(
                GetTableColumnsByDivisionKind(tableInfo, DivisionKind.Lower), GetTableColumnsByDivisionKind(tableInfo, DivisionKind.Upper));
            if (divisionColumns.GetLength(0) == 0) return;

            // Получаем массив столбцов-признаков переноса, по которым можно судить о переносах в других столбцах
            TableColumn[] divisionSignColumns = GetTableColumnsByDivisionKind(tableInfo, DivisionKind.DivisionSign);
            if (divisionSignColumns.GetLength(0) == 0) return;

            // Идем по всем записям таблицы и смотрим по ячейкам столбцов-признаков переноса. Если ячейки этих
            // столбцов пусты в данной строке, то кое-где есть перенос.
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool isDivision = true;

                for (int j = 0; j < divisionSignColumns.GetLength(0); j++)
                {
                    if (!dt.Rows[i].IsNull(divisionSignColumns[j].DBField))
                    {
                        isDivision = false;
                        break;
                    }
                }

                // Если это перенос, то анализируем тип переноса и корректируем значения ячеек 
                // соответствующих столбцов.
                if (isDivision)
                {
                    DataRow row = null;

                    for (int j = 0; j < divisionColumns.GetLength(0); j++)
                    {
                        TableColumn divisionColumn = divisionColumns[j];

                        switch (divisionColumn.DivisionKind)
                        {
                            case DivisionKind.Lower:
                                // Берем предыдущую строку и объединяем ячейки в столбцах с переносами
                                if (i < dt.Rows.Count - 1)
                                {
                                    row = dt.Rows[i];
                                    dt.Rows[i + 1][divisionColumn.DBField] =
                                        Convert.ToString(row[divisionColumn.DBField]).Trim('-') + " " +
                                        Convert.ToString(dt.Rows[i + 1][divisionColumn.DBField]).Trim('-');
                                }
                                break;

                            case DivisionKind.Upper:
                                // Берем следующую строку и объединяем ячейки в столбцах с переносами
                                if (i > 0)
                                {
                                    row = dt.Rows[i + 1];
                                    dt.Rows[i][divisionColumn.DBField] =
                                        Convert.ToString(dt.Rows[i][divisionColumn.DBField]).Trim('-') + " " +
                                        Convert.ToString(row[divisionColumn.DBField]).Trim('-');
                                }
                                break;
                        }
                    }

                    dt.Rows.Remove(row);
                    if (i >= 0) i--;
                }
            }
        }

        #endregion Анализ внутренних структур, перенос данных в DataTable

        #endregion Обработка файлов


        #region Общие функции

        /// <summary>
        /// Возвращает строку, содержащую данные указанной области файла
        /// </summary>
        /// <param name="left">Левая граница области</param>
        /// <param name="top">Верхняя граница области</param>
        /// <param name="right">Правая граница области</param>
        /// <param name="bottom">Нижняя граница области</param>
        /// <returns>Строка с данными фрагмента файла</returns>
        private string GetFileFragment(int left, int top, int right, int bottom)
        {
            string result = string.Empty;

            for (int i = top; i <= bottom; i++)
            {
                string row = this.textDataStrings[i];
                if (left >= row.Length) continue;

                if (right < row.Length)
                {
                    result += " " + row.Substring(left, right - left + 1);
                }
                else
                {
                    result += " " + row.Substring(left);
                }
            }

            return result.Trim();
        }

        /// <summary>
        /// Ищет следующий горизонтальный разделитель после указанной позиции
        /// </summary>
        /// <param name="startIndex">Начальная позиция поиска</param>
        /// <returns>Позиция разделителя (-1 - не найден)</returns>
        private int FindNextHorizMargin(ParsedTableInfo tableInfo, int startIndex)
        {
            for (int i = startIndex; i < this.totalRowsCount; i++)
            {
                if (tableInfo.TextRows[i].IsMargin)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Ищет предыдущий горизонтальный разделитель от указанной позиции
        /// </summary>
        /// <param name="startIndex">Начальная позиция поиска</param>
        /// <returns>Позиция разделителя (-1 - не найден)</returns>
        private int FindPrevHorizMargin(ParsedTableInfo tableInfo, int startIndex)
        {
            for (int i = startIndex; i >= 0; i--)
            {
                if (tableInfo.TextRows[i].IsMargin)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Ищет следующий вертикальный разделитель после указанной позиции
        /// </summary>
        /// <param name="startIndex">Начальная позиция поиска</param>
        /// <returns>Позиция разделителя (-1 - не найден)</returns>
        private int FindNextVertMargin(ParsedTableInfo tableInfo, int startIndex)
        {
            for (int i = startIndex; i < this.totalColsCount; i++)
            {
                if (tableInfo.TextColumns[i].IsMargin)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// ДЛЯ ОТЛАДКИ. Сохраняет массив данных отчета в файл
        /// </summary>
        private void SaveTXTDataToFile()
        {
            FileStream fs = File.Create(@"y:\server\temp\_1111.txt");
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(866));

            try
            {
                for (int i = 0; i < this.textDataStrings.Count; i++)
                {
                    sw.WriteLine(this.textDataStrings[i]);
                }
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        /// Устанавливает признак разделителя у столбца
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        private void SetColumnMarginValue(LineInfo col, double delimsPercent)
        {
            col.IsMargin = delimsPercent >= constVertMarginDelimsPercent;
            col.IsShortMargin = delimsPercent >= constShortMarginDelimsPercent;
        }

        /// <summary>
        /// Устанавливает признак разделителя у строки
        /// </summary>
        /// <param name="index">Индекс строки</param>
        private void SetRowMarginValue(LineInfo row, double delimsPercent)
        {
            row.IsMargin = delimsPercent >= constHorzMarginDelimsPercent;
            row.IsShortMargin = delimsPercent >= constShortMarginDelimsPercent;
        }

        /// <summary>
        /// Закачивает строку
        /// </summary>
        /// <param name="ds">Таблица классификатора</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        private void PumpRow(DataTable dt, object[] valuesMapping)
        {
            DataRow row = dt.NewRow();

            for (int i = 0; i < valuesMapping.GetLength(0) - 1; i += 2)
            {
                if (valuesMapping[i] == null) continue;

                object value = valuesMapping[i + 1];

                if (value == null)
                {
                    row[Convert.ToString(valuesMapping[i])] = DBNull.Value;
                }
                else
                {
                    row[Convert.ToString(valuesMapping[i])] = value;
                }
            }

            dt.Rows.Add(row);
        }

        #endregion Общие функции
    }
}