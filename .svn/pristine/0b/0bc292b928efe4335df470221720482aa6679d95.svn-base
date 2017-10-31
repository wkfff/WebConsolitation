using System;
using System.Collections.Generic;
using System.Drawing;
using org.pdfclown.documents;
using org.pdfclown.documents.contents;
using org.pdfclown.documents.contents.objects;
using org.pdfclown.files;

namespace Krista.FM.Server.DataPumps.DataAccess
{

    /// <summary>
    /// Текстовый блок
    /// </summary>
    public struct PdfTextToken
    {
        /// <summary>
        /// Текст
        /// </summary>
        public string Text;

        /// <summary>
        /// Координата X текстового блока
        /// </summary>
        public float X;

        /// <summary>
        /// Координата Y текстового блока
        /// </summary>
        public float Y;

        /// <summary>
        /// Ширина текстового блока
        /// </summary>
        public float Width;

        /// <summary>
        /// Высота текстового блока
        /// </summary>
        public float Height;
    }

    /// <summary>
    /// Класс для работы с PDF-файлами
    /// </summary>
    public class PdfHelper
    {

        #region Поля

        /// <summary>
        /// Объект PDF-документа
        /// </summary>
        private Document document = null;

        #endregion

        #region Свойства

        /// <summary>
        /// Количество страниц в документе
        /// </summary>
        public int PagesCount
        {
            get
            {
                return this.document.Pages.Count;
            }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Констуктор
        /// </summary>
        /// <param name="filename">Имя PDF-файла</param>
        public PdfHelper(string filename)
        {
            File file = new File(filename);
            this.document = file.Document;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Извлечь текст как массив строк
        /// </summary>
        /// <param name="scanner">Сканер объектов содержимого страницы</param>
        /// <returns>Массив извлеченных строк</returns>
        private string[] ExtractText(ContentScanner scanner)
        {
            List<string> textList = new List<string>();

            while (scanner.MoveNext())
            {
                ContentObject content = scanner.Current;
                if (content is Text)
                {
                    ContentScanner.TextWrapper text = (ContentScanner.TextWrapper)scanner.CurrentWrapper;
                    foreach (ContentScanner.TextStringWrapper textString in text.TextStrings)
                    {
                        textList.Add(textString.Text);
                    }
                }
            }

            return textList.ToArray();
        }

        /// <summary>
        /// Извлечь содержимое из страницы в виде массива строк
        /// </summary>
        /// <param name="pageNumber">Номер страницы в документе</param>
        /// <returns>Массив строк</returns>
        public string[] ExtractTextFromPage(int pageNumber)
        {
            Page page = this.document.Pages[pageNumber];
            return ExtractText(new ContentScanner(page.Contents));
        }

        /// <summary>
        /// Извлечь текст как список текстовых блоков
        /// </summary>
        /// <param name="scanner">Сканер объектов содержимого страницы</param>
        /// <returns>Массив извлеченных текстовых блоков</returns>
        private List<PdfTextToken[]> ExtractTokenText(ContentScanner scanner)
        {
            List<PdfTextToken[]> rows = new List<PdfTextToken[]>();

            float lastX = 0;
            List<PdfTextToken> row = new List<PdfTextToken>();
            while (scanner.MoveNext())
            {
                ContentObject content = scanner.Current;
                if (content is Text)
                {
                    ContentScanner.TextWrapper text = (ContentScanner.TextWrapper)scanner.CurrentWrapper;
                    foreach (ContentScanner.TextStringWrapper textString in text.TextStrings)
                    {
                        RectangleF textStringBox = textString.Box.Value;

                        PdfTextToken textToken = new PdfTextToken();
                        textToken.X = textStringBox.X;
                        textToken.Y = textStringBox.Y;
                        textToken.Width = textStringBox.Width;
                        textToken.Height = textStringBox.Height;
                        textToken.Text = textString.Text;

                        if (lastX > textToken.X)
                        {
                            rows.Add(row.ToArray());
                            row.Clear();
                        }
                        row.Add(textToken);
                        lastX = textToken.X;
                    }
                }
            }
            rows.Add(row.ToArray());

            return rows;
        }

        /// <summary>
        /// Извлечь содержимое из страницы в виде массива текстовых блоков
        /// </summary>
        /// <param name="pageNumber">Номер страницы в документе</param>
        /// <returns>Массив текстовых блоков</returns>
        public List<PdfTextToken[]> ExtractTokenTextFromPage(int pageNumber)
        {
            Page page = this.document.Pages[pageNumber];
            return ExtractTokenText(new ContentScanner(page.Contents));
        }

        #endregion

    }

}
