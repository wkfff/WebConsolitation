using System;
using System.IO;
using System.Net;
using System.Text;

namespace Krista.FM.Update.Framework.Utils
{
    public sealed class FileDownloader : FileDownloaderAbstract, IDisposable
    {
        private readonly WebClient client;

        public FileDownloader(string proxy, string proxyPort, string user, string password)
        {
            client = new WebClient {Encoding = Encoding.UTF8};
            client.Credentials = CredentialCache.DefaultCredentials;
            if (!String.IsNullOrEmpty(proxy))
            {
                WebProxy webProxy =
                    new WebProxy(String.Format("{0}:{1}", proxy, proxyPort));
                webProxy.Credentials = new NetworkCredential(user, password);
                client.Proxy = webProxy;
            }
        }

        public byte[] Download(Uri _uri)
        {
            return client.DownloadData(_uri);
        }

        public override string DownloadString(Uri _uri)
        {
            try
            {
                return client.DownloadString(_uri);
            }
            catch (WebException e)
            {
                throw new FileDownloaderException(e.Message);
            }
            catch (NotSupportedException e)
            {
                throw new FileDownloaderException(e.Message);
            }
        }

        public override bool DownloadToFile(string tempLocation, Uri _uri, bool needAddPostfix)
        {
            try
            {
                // Чтобы не было проблем с MIME-Типами, если клиенты обновляются через HTTP 
                if (needAddPostfix)
                {
                    _uri = new Uri(string.Format("{0}.deploy", _uri));
                }

                DateTime uriFileChangeTime = GetLastModifiedTime(_uri);

                client.DownloadFile(_uri, tempLocation);

                if (uriFileChangeTime.ToString() != "01.01.0001 0:00:00")
                {
                    File.SetLastWriteTime(tempLocation, uriFileChangeTime);
                }

                return true;
            }
            catch (WebException e)
            {
                try
                {
                    // не удалось загрузить файл, пытаемся по другому
                    byte[] bytes = client.DownloadData(_uri);
                    File.WriteAllBytes(tempLocation, bytes);
                    return true;
                }
                catch (IOException ex)
                {
                    throw new FileDownloaderException(ex.Message);
                }
                catch (WebException ex)
                {
                    try
                    {
                        string text = client.DownloadString(_uri);
                        File.WriteAllText(tempLocation, text);
                        return true;
                    }
                    catch (WebException webException)
                    {
                        throw new FileDownloaderException(webException.Message);
                    }
                }
            }
            catch(NotSupportedException e)
            {
                throw new FileDownloaderException(e.Message);
            }
        }

        public void DownloadAsync(Uri uri, Action<byte[]> finishedCallback)
        {
            DownloadAsync(uri, finishedCallback, null);
        }

        /// <summary>
        /// Асинхронная загрузка файла
        /// </summary>
        /// <param name="_uri"></param>
        /// <param name="finishedCallback"></param>
        /// <param name="progressChangedCallback"></param>
        public void DownloadAsync(Uri _uri, Action<byte[]> finishedCallback, Action<long, long> progressChangedCallback)
        {
            try
            {
                if (progressChangedCallback != null)
                    client.DownloadProgressChanged += (sender, args) => progressChangedCallback(args.BytesReceived, args.TotalBytesToReceive);

                client.DownloadDataCompleted += (sender, args) => finishedCallback(args.Result);
                client.DownloadDataAsync(_uri);
            }
            catch (WebException e)
            {
                throw new FileDownloaderException(e.Message);
            }
            catch (NotSupportedException e)
            {
                throw new FileDownloaderException(e.Message);
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}