using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Core.HttpModules
{
    public class ResourceCompressor
    {
        public static bool UseGZip
        {
            get
            {
                HttpRequest Request = HttpContext.Current.Request;
                string AcceptEncoding = Request.Headers["Accept-Encoding"];
                if (!string.IsNullOrEmpty(AcceptEncoding) && AcceptEncoding.ToLower().IndexOf("gzip") > -1)
                {
                    return true;
                }
                return false;
            }
        }

        public static byte[] GetOutput(string textResource, Encoding encoding)
        {
            HttpContext context = HttpContext.Current;
            byte[] output;
            // если больше 8кб
            if (UseGZip && textResource.Length > 6000)
            {
                // упакуем
                output = GZipMemory(textResource, encoding);
                context.Response.AppendHeader("Content-Encoding", "gzip");
            }
            else
            {
                output = encoding.GetBytes(textResource);
            }
            return output;
        }

        /// <summary>
        /// Кодирует строку в массив байт со сжатием.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static byte[] GZipMemory(string Input, Encoding encoding)
        {
            byte[] Buffer = encoding.GetBytes(Input);
            MemoryStream ms = new MemoryStream();
            GZipStream GZip = new GZipStream(ms, CompressionMode.Compress);
            GZip.Write(Buffer, 0, Buffer.Length);
            GZip.Close();

            byte[] Result = ms.ToArray();
            ms.Close();

            return Result;
        }

        public static byte[] CompressPage(string htmlPage)
        {            
            string customScriptRef = PrepareScriptsRef(htmlPage);
            htmlPage = ReplaceScriptsRef(htmlPage, customScriptRef);
            htmlPage = OptimizeHTML(htmlPage);
            return GetOutput(htmlPage, Encoding.UTF8);
        }

        /// <summary>
        /// Формирует ссылки на необходимые ресурсы.
        /// </summary>
        /// <param name="htmlPage">Текст страницы.</param>
        /// <returns></returns>
        private static string PrepareScriptsRef(string htmlPage)
        {
            string replacement = string.Empty;
            string appPath = GetAppPath();
            // Добавляем ссылку на общие скрипты.
            string resourceName = string.Format("{0}/Common.axd", appPath);
            replacement += string.Format(
                "<script  src=\"{0}\" type=\"text/javascript\"></script>{1}", resourceName, Environment.NewLine);
            foreach (string pattern in ScriptPatterns.PatternResourceDictionary.Keys)
            {
                if (pattern != null)
                {
                    Regex regex = new Regex(pattern);
                    MatchCollection scripts = regex.Matches(htmlPage);
                    // Если такие скрипты есть
                    if (scripts.Count > 0)
                    {
                        resourceName =
                            string.Format("{0}/{1}.axd", appPath, ScriptPatterns.PatternResourceDictionary[pattern]);
                        replacement += string.Format(
                            "<script  src=\"{0}\" type=\"text/javascript\"></script>{1}", resourceName, Environment.NewLine);
                    }
                }
            }
            return replacement;
        }

        private static string GetAppPath()
        {
            string appPath = HttpContext.Current.Request.ApplicationPath;
            if (appPath == "/")
            {
                appPath = string.Empty;
            }
            return appPath;
        }

        private static string OptimizeHTML(string documentText)
        {
            // Лишние пробелы убираем
            documentText = Regex.Replace(documentText, ">(\r\n){0,10} {0,20}\t{0,10}(\r\n){0,10}\t{0,10}(\r\n){0,10} {0,20}(\r\n){0,10} {0,20}<", "><", RegexOptions.Compiled);
            documentText = Regex.Replace(documentText, ";(\r\n){0,10} {0,20}\t{0,10}(\r\n){0,10}\t{0,10}", ";",
                              RegexOptions.Compiled);
            documentText = Regex.Replace(documentText, "{(\r\n){0,10} {0,20}\t{0,10}(\r\n){0,10}\t{0,10}", "{",
                              RegexOptions.Compiled);
            documentText = Regex.Replace(documentText, ">(\r\n){0,10}\t{0,10}<", "><", RegexOptions.Compiled);
            documentText = Regex.Replace(documentText, ">\r{0,10}\t{0,10}<", "><", RegexOptions.Compiled);

            return documentText;
        }

        /// <summary>
        ///  Удаляет ссылки на скрипты
        /// </summary>
        /// <param name="documentText"></param>
        /// <returns></returns>
        static private string ReplaceScriptsRef(string documentText, string replacements)
        {
            MatchCollection scripts = Regex.Matches(documentText, "<script src=[\\s\\S]*?</script>");
            if (scripts.Count > 0 && scripts[0].Captures.Count > 0)
            {
                int index = documentText.IndexOf(scripts[0].Captures[0].Value);
                documentText = Regex.Replace(documentText, "<script src=[\\s\\S]*?</script>", string.Empty);
                // придумать нормальное регулярное выражение
                documentText = Regex.Replace(documentText, "<script language=\"javascript\" src=[\\s\\S]*?</script>", string.Empty);
                documentText = documentText.Insert(index, replacements);
            }

            string imageUlr = Regex.Match(documentText,
                                          "<script type=\"text/javascript\">[^>]*?ig_pi_imageUrl='(?<imageUrl>[\\s\\S]*?)'[\\s\\S]*?</script>").Groups["imageUrl"].Value;
            if (!String.IsNullOrEmpty(imageUlr))
            {
                documentText = documentText.Replace(imageUlr, string.Format("{0}/CustomResources/ImagePiResource.gif", GetAppPath()));
            }
            return documentText;
        }

        public static byte[] PrepareToWord(string documentText)
        {
            string output = documentText;
            output = output.Replace("WordExportMode", String.Empty);
            Dictionary<string, string> grids = new Dictionary<string, string>(); 
            // достаем ультрагрид
            Match ugMatches = Regex.Match(output,
                            "<table border='0' cellpadding='0' cellspacing='0'[\\s\\S]*?<img src='[../]*?ig_res/Office2007Blue/images/ig_tblPimgDn.gif' border='0px' width='10px' height='12px'[\\s\\S]*?style=\"overflow:hidden;position:absolute;left:0px;top:0px;display:none;\" />");
            for (int i = 0; i < ugMatches.Captures.Count; i++)
            {
                string ultraGrid = ugMatches.Captures[i].Value;
                output = output.Replace(ultraGrid, "ультрагрид" + i);
                // почистим
                ultraGrid = ConvertUltraGrid(ultraGrid);
                grids.Add("ультрагрид" + i, ultraGrid);
            }

           // Чистим остальную страничку
            
            output = Regex.Replace(output, "<script src=[\\s\\S]*?</script>", string.Empty);
            output = Regex.Replace(output, "<script type=[\\s\\S]*?</script>", string.Empty);
            output = Regex.Replace(output, "<link href=\"[\\s\\S]*?/>", string.Empty);
            output = Regex.Replace(output, "<input[\\s\\S]*?/>", string.Empty);           
            output = Regex.Replace(output, "<map onmousemove=\"[\\s\\S]*?</map>", string.Empty);
            output = Regex.Replace(output, "<div[\\s\\S]*?>", string.Empty);
            output = Regex.Replace(output, "<table[\\s\\S]*?>", string.Empty);
            output = Regex.Replace(output, "<td[\\s\\S]*?>", string.Empty);
            output = Regex.Replace(output, "<tr[\\s\\S]*?>", string.Empty);
            output = Regex.Replace(output, "id=\"[\\s\\S]*?\"", string.Empty);
            output = Regex.Replace(output, "<a  href[\\s\\S]*?</a>", string.Empty);
            // Отрезаем картинку, относительный урл заменяем на абсолютный
            MatchCollection matches = Regex.Matches(output, "<img[^>]*?src=[^>]*\\./(?<imageUrl>[^>]*?).png[^>]*?>");
           
            foreach (Match capture in matches)
            {
               string imageUrl = capture.Groups["imageUrl"].Value;
               output =
                    output.Replace(capture.Value, string.Format(
                                             "<img src=\"{0}{1}.png\">",
                                             GetSiteAbsoluteUrl(), imageUrl));
            }

            // Приписываем абсолютные пути таблицам стилей
            matches = Regex.Matches(output, "<link href=\"[^>]*\\./(?<cssUrl>[^>]*?).css[^>]*?>");

            foreach (Match capture in matches)
            {
                string cssUrl = capture.Groups["cssUrl"].Value;
                output =
                     output.Replace(capture.Value, string.Format(
                                              "<link href=\"{0}{1}.css\" type=\"text/css\" rel=\"stylesheet\"/>",
                                              GetSiteAbsoluteUrl(), cssUrl));
            }

             output = output.Replace("</div>", string.Empty);
            output = output.Replace("</table>", string.Empty);
            output = output.Replace("</td>", string.Empty);
            output = output.Replace("</tr>", string.Empty);
            output = output.Replace("</span>", "</span><br/>");
            output = output.Replace("Tooltip", string.Empty);
            output = output.Replace("&nbsp;", " ");
            output = output.Replace("&nbsp", " ");
            output = output.Replace("TBODY", String.Empty);
            output = output.Replace("TR", String.Empty);
            output = output.Replace("TD", String.Empty);
            output = output.Replace("THEAD", String.Empty);
            output = output.Replace("TH", String.Empty);
            output = output.Replace("class=\"backgroundGray\"", String.Empty);

            // Вставляем ультрагрид обратно.
            for (int i = 0; i < ugMatches.Captures.Count; i++)
            {
                output = output.Replace("ультрагрид" + i, grids["ультрагрид" + i]);
            }

            output = output.Replace("с", "&#1089");
            output = output.Replace("я", "&#1103");
            
            MakeHttpHeader();
            
            Chilkat.Mht mht = new Chilkat.Mht();
            mht.UnlockComponent("MHTT34MB34N_5DB469D2oR9M");
            output = mht.HtmlToMHT(output);
                       
            return GetOutput(output, Encoding.UTF8);
        }

        private static string ConvertUltraGrid(string ultraGrid)
        {
            ultraGrid = Regex.Replace(ultraGrid, "id='[\\s\\S]*?'", string.Empty);
            ultraGrid = ultraGrid.Replace("<nobr>", string.Empty);
            ultraGrid = ultraGrid.Replace("</nobr>", string.Empty);
            ultraGrid = ultraGrid.Replace("</div>", string.Empty);
            ultraGrid = Regex.Replace(ultraGrid, "<div>[\\s\\S]*/>", string.Empty);

            int theadStart = ultraGrid.IndexOf("<thead");
            int theadLength = ultraGrid.IndexOf("</thead>") - theadStart;
            int tbodyStart = ultraGrid.IndexOf("<tbody");
            int tbodyLength = ultraGrid.IndexOf("</tbody>") - tbodyStart;

            // вырежем заголовок и тело
            string thead = ultraGrid.Substring(theadStart, theadLength);
            string tbody = ultraGrid.Substring(tbodyStart, tbodyLength);

            thead = Regex.Replace(thead, "<thead class=[\\s\\S]*?</thead>", string.Empty);
            thead = Regex.Replace(thead, "columnno='[\\s\\S]*?'", string.Empty);
            thead = Regex.Replace(thead, "<th[\\s\\S]*?img src=[\\s\\S]*?/></th>", string.Empty);
            thead = Regex.Replace(thead, "<th[^>]*?display:none[\\s\\S]*?</th>", string.Empty);

            //  thead = Regex.Replace(thead, "<colgroup>[\\s\\S]*?</colgroup>", string.Empty);
            //  thead = Regex.Replace(thead, "coloffs='[\\s\\S]*?'", string.Empty);
            //  thead = Regex.Replace(thead, "title=\"[\\s\\S]*?\"", string.Empty);
            //  thead = Regex.Replace(thead, "<img src='[\\s\\S]*?/>", string.Empty);
            
            tbody = Regex.Replace(tbody, "<th[\\s\\S]*?class=\"[\\s\\S]*?</th>", string.Empty);
            tbody = Regex.Replace(tbody, "<table[\\s\\S]*?</table>", string.Empty);
            tbody = Regex.Replace(tbody, "<tbody[\\s\\S]*?style=\"[\\s\\S]*?>", string.Empty);
            tbody = Regex.Replace(tbody, "<tr>[\\s\\S]*?</thead>", string.Empty);
            tbody = Regex.Replace(tbody, "uV=\"[\\s\\S]*?\"", string.Empty);
            tbody = Regex.Replace(tbody, "<thead class=[\\s\\S]*?</thead>", string.Empty);

            //   tbody = Regex.Replace(tbody, "<colgroup>[\\s\\S]*?</colgroup>", string.Empty);
            //  tbody = Regex.Replace(tbody, "<tr style='[\\s\\S]*?>", "<tr>");

            tbody = MakeCellsIndicators(tbody);
            // Формируем таблицу из ультрагрида
            ultraGrid = "<table style=\"border-collapse: collapse\">" + thead + tbody + "</table>";
            ultraGrid = Regex.Replace(ultraGrid, "<td[^>]*?display:none[\\s\\S]*?</td>", string.Empty);
            return ultraGrid;
        }

        /// <summary>
        /// Конвертирует индикаторы в ячейках в картинки.
        /// </summary>
        /// <param name="tbody">Исходное тело грида.</param>
        /// <returns>Сконвертированное тело грида.</returns>
        private static string MakeCellsIndicators(string tbody)
        {
            MatchCollection matches = Regex.Matches(tbody, "background-image:url\\((?<imageUrl>[\\s\\S]*?)\\);[\\s\\S]*?>");
            while (matches.Count > 0)
            {
                matches = Regex.Matches(tbody, "background-image:url\\((?<imageUrl>[\\s\\S]*?)\\);[\\s\\S]*?>");
                foreach (Match capture in matches)
                {
                    string imageUrl = capture.Groups["imageUrl"].Value;
                    tbody =
                         tbody.Replace(capture.Value, string.Format(
                                                  "\"><img src=\"{0}{1}\">",
                                                  GetSiteHostUrl(), imageUrl.TrimStart('/')));
                }
            }
            return tbody;
        }

        private static string GetSiteAbsoluteUrl()
        {
            return String.Format("http://{0}{1}",
                                 HttpContext.Current.Request.Url.Authority,
                                 HttpContext.Current.Request.ApplicationPath).Trim('/') + "/";
        }

        private static string GetSiteHostUrl()
        {
            return String.Format("http://{0}",
                                 HttpContext.Current.Request.Url.Authority).Trim('/') + "/";
        }

        /// <summary>
        /// Устанавливает ContentType = "application/msword"
        /// </summary>
        private static void MakeHttpHeader()
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            StringBuilder builder = new StringBuilder();
            builder.Append("attachment; ");
            builder.Append("filename=" + "Report.doc");
            HttpContext.Current.Response.AddHeader("Content-Disposition", builder.ToString());
            HttpContext.Current.Response.ContentType = "application/msword";
        }
    }
}