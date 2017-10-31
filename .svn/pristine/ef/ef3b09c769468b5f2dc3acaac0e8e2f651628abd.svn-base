using System;
using System.Text;
using System.Net;
using System.IO;

using Krista.FM.Update.Framework.Utils;

namespace Krista.FM.Update.Framework.Sources
{
    public class SimpleWebSource : IUpdateSource
    {
        private readonly FileDownloaderAbstract fileDownloader;
        public SimpleWebSource(ConnectionType connectionType, string proxy, string proxyPort, string user, string password)
        {
            fileDownloader = SourceFactory.CreateSourceFactory(connectionType, proxy, proxyPort, user, password);
        }
        private readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        #region IUpdateSource Members

        public string GetUpdatesFeed(string feedPath)
        {
            string data = null;

            try
            {
                data = fileDownloader.DownloadString(new Uri(feedPath));
            }
            catch (WebException e)
            {
                Trace.TraceError(String.Format("Канал не найден: {0}", feedPath));
                return data;
            }
            catch(NotSupportedException e)
            {
                throw new FileDownloaderException(e.Message);
            }

            if (String.IsNullOrEmpty(data))
            {
                Trace.TraceWarning(String.Format("Файл {0} не загружен или пустой!", feedPath));
                return data;
            }

            if (data.StartsWith(_byteOrderMarkUtf8) && data[0] != '<')
                data = data.Remove(0, _byteOrderMarkUtf8.Length);

            return data;
        }

        /// <summary>
        /// Загружает файл из url в tempLocation
        /// </summary>
        /// <param name="url"></param>
        /// <param name="baseUrl"></param>
        /// <param name="tempLocation"></param>
        /// <returns></returns>
        public bool GetData(string url, string baseUrl, ref string tempLocation, bool needAddPostfix)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                uri = new Uri(url);
            }
            else if (Uri.TryCreate(Path.Combine(baseUrl, url), UriKind.Absolute, out uri))
            {
                uri = new Uri(new Uri(baseUrl, UriKind.Absolute), url);
            }

            if (uri == null)
                throw new FileDownloaderException(String.Format("Задан некоррекктный URI {0} к файлу {1}", baseUrl,
                                                                url));
            try
            {
               
                if (string.IsNullOrEmpty(tempLocation) || !Directory.Exists(Path.GetDirectoryName(tempLocation)))
                    // если временный файл не указан, создаем на диске временный файл с уникальным именем
                    tempLocation = Path.GetTempFileName();

                return fileDownloader.DownloadToFile(tempLocation, uri, needAddPostfix);
            }
            catch (FileDownloaderException e)
            {
                Trace.TraceError(String.Format("При загрузке файла {0} произошла ошибка: {1}. Загружаемый файл: {2}", url,
                                                                  e.Message, uri));
                throw new PreparetaskException(e.Message);
            }
        }

        #endregion
    }
}
