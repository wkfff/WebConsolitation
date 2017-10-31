using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Krista.FM.Client.iMonitoringWM.Common
{
    public static class ConnectionHelper
    {
        private static Dictionary<string, string> _cookies;

        /// <summary>
        /// Cookies
        /// </summary>
        public static Dictionary<string, string> Cookies
        {
            get
            {
                if (_cookies == null)
                    _cookies = new Dictionary<string, string>();
                return _cookies;
            }
            set { _cookies = value; }
        }

        /// <summary>
        /// Создать веб запрос
        /// </summary>
        /// <param name="uri">uri</param>
        /// <returns>запрос</returns>
        public static HttpWebRequest GetHttpWebRequest(string uri)
        {
            return GetHttpWebRequest(uri, string.Empty);
        }

        /// <summary>
        /// Создать веб запрос
        /// </summary>
        /// <param name="uri">uri</param>
        /// <param name="queryContent">содержимое запроса (если строка не пустая, то тип запроса будет POST иначе GET)</param>
        /// <returns>запрос</returns>
        public static HttpWebRequest GetHttpWebRequest(string uri, string queryContent)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Proxy = new WebProxy(uri);
            request.UserAgent = Consts.userAgent;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.AllowWriteStreamBuffering = true;
            request.AllowAutoRedirect = false;

            //запишем в куку сессии клиента
            string sessionsId = GetSessionsIdForCookie(uri);
            if (!string.IsNullOrEmpty(sessionsId))
                request.Headers.Add(Consts.headerCookieName, sessionsId);

            if (queryContent != string.Empty)
            {
                request.ContentType = "application/x-www-form-urlencoded";

                request.Method = "POST";
                byte[] SomeBytes = Encoding.UTF8.GetBytes(queryContent);
                request.ContentLength = SomeBytes.Length;
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(SomeBytes, 0, SomeBytes.Length);
                }
            }
            else
            {
                request.Method = "GET";
            }

            return request;
        }

        /// <summary>
        /// Вернет значение для куки с сессиями клиента
        /// </summary>
        /// <returns></returns>
        private static string GetSessionsIdForCookie(string uriStr)
        {
            uriStr = Utils.GetUriWithoutQuery(uriStr);
            string sessionId = string.Empty;
            while (uriStr.Contains("/"))
            {
                if (Cookies.TryGetValue(uriStr, out sessionId))
                {
                    return string.Format("PHPSESSID={0}; ", sessionId);
                }
                //если такого ури нет в коллекции, будем смотреть может есть для родителя
                uriStr = uriStr.TrimEnd(new char[] { '/' });
                uriStr = Utils.CutEndString(uriStr, '/', false);
            }
            return string.Empty;
        }

        /// <summary>
        /// Вытащит из куки сессии клиента
        /// </summary>
        public static void ParseSessionsIdFromCookie(HttpWebResponse response)
        {
            if (response != null)
            {
                string cookies = response.Headers.Get("Set-Cookie");
                if (!string.IsNullOrEmpty(cookies))
                {
                    cookies.TrimEnd(new char[] { ',' });
                    cookies += ",";
                    //Если включены соседние сервера испульзуем данное регулярное выражение
                    Regex rx = new Regex(@"PHPSESSID=(?<sessionID>[\S\s]*?); path=(?<path>[\S\s]*?); domain=(?<domain>[\S]*?),", 
                        RegexOptions.IgnoreCase);
                    MatchCollection matches = rx.Matches(cookies);
                    if (matches.Count == 0)
                    {
                        //Если работаем с одним сервером, то испульзуем ниже указанное регулярное выражение
                        rx = new Regex(@"PHPSESSID=(?<sessionID>[\S\s]*?); path=(?<path>[\S\s]*?),", 
                            RegexOptions.IgnoreCase);
                        matches = rx.Matches(cookies);
                    }
                    foreach (Match match in matches)
                    {
                        string sessionId = match.Groups["sessionID"].Value;

                        string path = match.Groups["path"].Value;
                        path = path.Replace("//", string.Empty);

                        string domain = string.Empty;
                        if (match.Groups["domain"].Value != string.Empty)
                        {
                            domain = match.Groups["domain"].Value;
                            domain = domain.Replace("https", string.Empty);
                            domain = domain.Replace("http", string.Empty);
                        }

                        string uri = string.Empty;
                        string responseUri = response.ResponseUri.ToString();

                        //Если в куках не указан домен к которому отнисится ID сессии, 
                        //то урл = урл с которого пришел ответ
                        if (domain == string.Empty)
                        {
                            uri = responseUri;
                        }
                        //если домен не пустой то формируем урл, для которого извлекали ID сессии
                        else
                        {
                            path = path.Replace(domain, string.Empty);
                            //протокол передачи данных
                            string protocol = responseUri.Contains("https") ? "https://" : "http://";

                            uri = domain + path;
                            uri = protocol + uri;
                        }

                        uri = Utils.GetUriWithoutQuery(uri);

                        //Добавим пару Урл и ID сессии для него
                        if (!Cookies.ContainsKey(uri))
                            Cookies.Add(uri, sessionId);
                    }
                }
            }
        }

        #region Тестовые методы
        public static void Test(Uri URL, string user, string password)
        {
            HttpWebRequest WRequest;
            HttpWebResponse WResponse;
            //preAuth the request
            // You can add logic so that you only pre-authenticate the very first request.
            // You should not have to pre-authenticate each request.
            WRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            // Set the username and the password.
            WRequest.Credentials = new NetworkCredential(user, password);
            WRequest.PreAuthenticate = true;
            WRequest.UserAgent = "Upload Test";
            WRequest.Method = "HEAD";
            WRequest.Timeout = 10000;
            WResponse = (HttpWebResponse)WRequest.GetResponse();
            WResponse.Close();
            // Make the real request.
            WRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            // Set the username and the password.
            WRequest.Credentials = new NetworkCredential(user, password);
            WRequest.PreAuthenticate = true;
            WRequest.UserAgent = "Upload Test";
            WRequest.Method = "POST";
            WRequest.AllowWriteStreamBuffering = false;
            WRequest.Timeout = 10000;
            FileStream ReadIn = new FileStream("c:\\testuploadfile.txt", FileMode.Open, FileAccess.Read);
            ReadIn.Seek(0, SeekOrigin.Begin); // Move to the start of the file.
            WRequest.ContentLength = ReadIn.Length; // Set the content length header to the size of the file.
            Byte[] FileData = new Byte[ReadIn.Length]; // Read the file in 2 KB segments.
            int DataRead = 0;
            Stream tempStream = WRequest.GetRequestStream();
            do
            {
                DataRead = ReadIn.Read(FileData, 0, 2048);
                if (DataRead > 0) //we have data
                {
                    tempStream.Write(FileData, 0, DataRead);
                    Array.Clear(FileData, 0, 2048); // Clear the array.
                }
            } while (DataRead > 0);

            WResponse = (HttpWebResponse)WRequest.GetResponse();
            // Read your response data here.
            // Close all streams.
            ReadIn.Close();
            tempStream.Close();
            WResponse.Close();
        }

        public static string GetCookieInOut(string uri, string input_cookie_value, out string out_cookie_values)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "GET";
            webRequest.UserAgent = "Mozilla/4.0(compatible;MSIE 7.0;Windows NT 6.0; SLCC1;.NET CLR 2.0.50727;Media Center PC 5.0; .NET CLR 3.0.04506;.NET CLR 1.1.4322)";
            webRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-ms-application, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, app";
            webRequest.Headers.Add("Accept-Language", "en-gb");

            if (input_cookie_value != "")
                webRequest.Headers.Add("Cookie", input_cookie_value);

            webRequest.AllowAutoRedirect = false;

            WebResponse webResponse = webRequest.GetResponse();
            if (webResponse == null)
            {
                out_cookie_values = null;
                return null;
            }
            out_cookie_values = webResponse.Headers.Get("Set-Cookie");
            StreamReader sr = new StreamReader(webResponse.GetResponseStream());
            string responce = sr.ReadToEnd().Trim();
            sr.Close();
            return responce;
        }
        #endregion
    }
}
