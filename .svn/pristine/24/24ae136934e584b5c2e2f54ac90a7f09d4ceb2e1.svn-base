using System;
using System.IO;
using System.Net;

namespace Krista.FM.Update.Framework.Utils
{
    /// <summary>
    /// Работа через FTP
    /// </summary>
    public class FtpDownloader : FileDownloaderAbstract
    {
        public override string DownloadString(Uri uri)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Credentials = CredentialCache.DefaultCredentials;
            if (!String.IsNullOrEmpty(UpdateManager.Instance.Proxy))
            {
                WebProxy webProxy =
                    new WebProxy(String.Format("{0}:{1}", UpdateManager.Instance.Proxy, UpdateManager.Instance.ProxyPort));
                webProxy.Credentials = new NetworkCredential(UpdateManager.Instance.User, UpdateManager.Instance.Password);
                request.Proxy = webProxy;
            }

            request.UsePassive = false;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            using (StreamReader reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        public override bool DownloadToFile(string tempLocation, Uri uri, bool needAddPostfix)
        {
            // Чтобы не было проблем с MIME-Типами, если клиенты обновляются через HTTP 
            if (needAddPostfix)
            {
                uri = new Uri(string.Format("{0}.deploy", uri));
            }

            DateTime uriFileChangeTime = GetLastModifiedTime(uri);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            if (!String.IsNullOrEmpty(UpdateManager.Instance.Proxy))
            {
                WebProxy webProxy =
                    new WebProxy(String.Format("{0}:{1}", UpdateManager.Instance.Proxy, UpdateManager.Instance.ProxyPort));
                webProxy.Credentials = new NetworkCredential(UpdateManager.Instance.User, UpdateManager.Instance.Password);
                request.Proxy = webProxy;
            }
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UsePassive = false;

            byte[] bFileWriteData;
            using (FtpWebResponse response = (FtpWebResponse) request.GetResponse())
            {
                int iContentLength = Convert.ToInt32(response.ContentLength);
                bFileWriteData = new byte[iContentLength];
                int iContentLengthCounter = 0;
                while (iContentLengthCounter < iContentLength)
                {
                    iContentLengthCounter += response.GetResponseStream().Read(bFileWriteData, iContentLengthCounter, (iContentLength - iContentLengthCounter));
                }
            }
            using (FileStream oFileStream = new System.IO.FileStream(tempLocation, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                oFileStream.Write(bFileWriteData, 0, bFileWriteData.Length);
            }

            if (uriFileChangeTime.ToString() != "01.01.0001 0:00:00")
            {
                File.SetLastWriteTime(tempLocation, uriFileChangeTime);
            }

            return true;
        }
    }
}
