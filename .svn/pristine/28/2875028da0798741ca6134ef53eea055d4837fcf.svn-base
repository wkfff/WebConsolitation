using System;
using System.IO;
using System.Net;

namespace Krista.FM.Update.Framework.Utils
{
    public abstract class FileDownloaderAbstract
    {
        public abstract string DownloadString(Uri _uri);
        public abstract bool DownloadToFile(string tempLocation, Uri _uri, bool needAddPostfix);
        protected static DateTime GetLastModifiedTime(Uri _uri)
        {
            DateTime uriFileChangeTime = new DateTime();

            switch (_uri.Scheme)
            {
                case "file":
                    uriFileChangeTime = File.GetLastWriteTime(_uri.LocalPath);
                    break;
                case "http":
                case "https":
                    {
                        var request = (HttpWebRequest)WebRequest.Create(_uri);
                        request.Credentials = CredentialCache.DefaultCredentials;
                        request.Method = "HEAD";
                        HttpWebResponse response = null;
                        try
                        {
                            response = (HttpWebResponse)request.GetResponse();
                            uriFileChangeTime = response.LastModified;
                        }
                        finally
                        {
                            if (response != null)
                                response.Close();
                        }
                    }
                    break;
                case "ftp":
                    {
                        var request = (FtpWebRequest)WebRequest.Create(_uri);
                        request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                        request.UsePassive = false;
                        FtpWebResponse response = null;
                        try
                        {
                            response = (FtpWebResponse)request.GetResponse();
                            uriFileChangeTime = response.LastModified;
                        }
                        finally
                        {
                            if (response != null)
                                response.Close();
                        }
                        break;
                    }
            }

            return uriFileChangeTime;
        }
    }
}
