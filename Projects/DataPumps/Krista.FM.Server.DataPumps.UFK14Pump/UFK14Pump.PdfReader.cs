using System;
using System.Collections.Generic;
using Krista.FM.Server.DataPumps.DataAccess;

namespace Krista.FM.Server.DataPumps.UFK14Pump
{

    /// <summary>
    /// Парсер отчётов в Pdf-формате для Москвы
    /// </summary>
    public class PdfReader
    {

        #region Поля

        /// <summary>
        /// Объект Pdf-документа
        /// </summary>
        private PdfHelper pdf = null;

        /// <summary>
        /// Количество страниц в документе
        /// </summary>
        private int pagesCount = 0;
        /// <summary>
        /// Номер текущей страницы документа
        /// </summary>
        private int currentPageNumber = 0;
        /// <summary>
        /// Текущая открытая страница документа
        /// </summary>
        private List<PdfTextToken[]> currentPage = null;

        /// <summary>
        /// Количество строк на странице
        /// </summary>
        private int rowsCount = 0;
        /// <summary>
        /// Номер текущей строки на странице
        /// </summary>
        private int currentRowNumber = 0;
        /// <summary>
        /// Текущая строка страницы
        /// </summary>
        private PdfTextToken[] currentRow = null;

        /// <summary>
        /// Признак, что текущая строка последняя на текущей странице
        /// </summary>
        private bool isLastRowAtPage = false;
        /// <summary>
        /// Признак, что текущую строку следует пропустить
        /// </summary>
        private bool isSkipRow = false;
        /// <summary>
        /// Признак, что в тукещей строке содержится номер сводного реестра
        /// </summary>
        private bool isSvodRow = false;
        /// <summary>
        /// Признак, что в текущей строке содержится дата отчета
        /// </summary>
        private bool isDateRow = false;
        /// <summary>
        /// Признак, что данные предназначены для закачки
        /// </summary>
        private bool toPump = false;

        #endregion

        #region Свойства

        /// <summary>
        /// Количество страниц в документе
        /// </summary>
        public int PagesCount
        {
            get
            {
                return pagesCount;
            }
        }

        /// <summary>
        /// Номер текущей страницы документа
        /// </summary>
        public int CurrentPageNumber
        {
            get
            {
                return currentPageNumber;
            }
        }

        /// <summary>
        /// Текущая открытая страница документа
        /// </summary>
        public List<PdfTextToken[]> CurrentPage
        {
            get
            {
                return currentPage;
            }
        }

        /// <summary>
        /// Количество строк на странице
        /// </summary>
        public int RowsCount
        {
            get
            {
                return rowsCount;
            }
        }

        /// <summary>
        /// Номер текущей строки на странице
        /// </summary>
        public int CurrentRowNumber
        {
            get
            {
                return currentRowNumber;
            }
        }

        /// <summary>
        /// Текущая строка страницы
        /// </summary>
        public PdfTextToken[] CurrentRow
        {
            get
            {
                return currentRow;
            }
        }

        /// <summary>
        /// Признак, что текущая строка последняя на текущей странице
        /// </summary>
        public bool IsLastRowAtPage
        {
            get
            {
                return isLastRowAtPage;
            }
        }

        /// <summary>
        /// Признак, что текущую строку следует пропустить
        /// </summary>
        public bool IsSkipRow
        {
            get
            {
                return isSkipRow;
            }
        }

        /// <summary>
        /// Признак, что в тукещей строке содержится номер сводного реестра
        /// </summary>
        public bool IsSvodRow
        {
            get
            {
                return isSvodRow;
            }
        }

        /// <summary>
        /// Признак, что в текущей строке содержится дата отчета
        /// </summary>
        public bool IsDateRow
        {
            get
            {
                return isDateRow;
            }
        }

        /// <summary>
        /// Признак, что данные предназначены для закачки
        /// </summary>
        public bool ToPump
        {
            get
            {
                return toPump;
            }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="filename">Имя pdf-файла</param>
        public PdfReader(string filename)
        {
            pdf = new PdfHelper(filename);
            pagesCount = pdf.PagesCount;
            currentPageNumber = 0;
            ReadNextPage();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Считать следующую страницу из документа
        /// </summary>
        /// <returns>True - успешно считано, False - достигнут конец документа</returns>
        private bool ReadNextPage()
        {
            if (currentPageNumber >= pagesCount)
                return false;

            currentPage = pdf.ExtractTokenTextFromPage(currentPageNumber);
            currentPageNumber++;

            rowsCount = currentPage.Count;
            currentRowNumber = 0;

            return true;
        }

        /// <summary>
        /// Считать следующую строку из документа
        /// </summary>
        /// <returns>True - успешно считано, False - достигнут конец документа</returns>
        public bool ReadNextRow()
        {
            if (currentPage == null)
                return false;

            // если предыдущая строка была последней на странице,
            // то открываем следующую страницу
            if (isLastRowAtPage)
            {
                bool result = ReadNextPage();
                if (!result)
                    return false;
            }

            currentRow = currentPage[currentRowNumber];
            RecognizeRow();
            currentRowNumber++;
            isLastRowAtPage = (currentRowNumber >= rowsCount);

            return true;
        }

        /// <summary>
        /// Распознать строку и установить соответствующие ей признаки
        /// </summary>
        private void RecognizeRow()
        {
            isSkipRow = false;
            isSvodRow = false;
            isDateRow = false;
            string value = currentRow[0].Text.Trim().ToUpper();

            if (value.StartsWith("СВОДНЫЙ") && !toPump)
            {
                isSkipRow = true;
                isSvodRow = true;
            }
            else if (value.StartsWith("ЗА"))
            {
                isSkipRow = true;
                // дата отчета находится в строке, начинающейся на "ЗА..." и содержащей слово "ДАТА"
                if (!toPump)
                {
                    foreach (PdfTextToken token in currentRow)
                        if (token.Text.ToUpper().Contains("ДАТА"))
                        {
                            isDateRow = true;
                            break;
                        }
                }
            }
            else if (value == "1")
            {
                isSkipRow = true;
                toPump = true;
            }
            else if (value.Contains("СТРАНИЦ") || value.StartsWith("ФОРМА"))
            {
                isSkipRow = true;
            }
            else if (value == "ВСЕГО")
            {
                isSkipRow = true;
                toPump = false;
            }
        }

        #endregion

    }

}
