using System;
using System.IO;
using System.Text;
using System.Web;

namespace Krista.FM.Server.Dashboards.Core.HttpModules
{
    public class HTMLCleaner : Stream
    {
        private Stream _HTML;
        public HTMLCleaner(Stream HTML)
        {
            _HTML = HTML;
        }

        #region Стандартные методы и свойства
        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return _HTML.Length; }
        }

        public override long Position
        {
            get { return _HTML.Position; }
            set { _HTML.Position = value; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _HTML.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _HTML.SetLength(value);
        }

        public override void Flush()
        {
            byte[] outdata;
            if (page.Contains("WordExportMode"))
            {
                outdata = ResourceCompressor.PrepareToWord(page);
            }
            else
            {
                outdata = ResourceCompressor.CompressPage(page);
            }
            _HTML.Write(outdata, 0, outdata.Length);
            _HTML.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _HTML.Read(buffer, offset, count);
        }
        #endregion


        /// <summary>
        /// Обрабатываем данные поступающие в Response
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            //Преобразовываем массив байт в строку и дописываем к страничке.
            page += Encoding.UTF8.GetString(buffer);
        }

        // Хранит текст странички.
        private string page = string.Empty;
    }

    public class HTTPModulePageCleaner : IHttpModule
    {
        #region IHttpModule Members
        public void Dispose()
        {
        }
        /// <summary>
        /// Подключение обработчиков событий
        /// </summary>
        public void Init(HttpApplication context)
        {
            //Два обработчика необходимы для совместимости с библиотеками сжатия HTML-документов
            context.ReleaseRequestState += new EventHandler(context_Clear);
           // context.PreSendRequestHeaders += new EventHandler(context_Clear);
        }

        /// <summary>
        /// Обработчик события PostRequestHandlerExecute
        /// </summary>
        private static void context_Clear(object sender, EventArgs e)
        {
            //Получение HTTP Application
            HttpApplication app = (HttpApplication)sender;
            //Получаем имя файла который обрабатывается
            string realPath = app.Request.Path.Remove(0, app.Request.ApplicationPath.Length + 1);

            // Если это ссылка на ресурс сборки, или одна из основных страниц
            if (realPath.Contains("WebResource"))
            {
                return;
            }
            //Проверяем тип содержимого
            if (app.Response.ContentType == "text/html")
            {
                //Устанавливаем фильтр обработчик
                app.Context.Response.Filter = new HTMLCleaner(app.Context.Response.Filter);
            }
        }

        #endregion
    }
}