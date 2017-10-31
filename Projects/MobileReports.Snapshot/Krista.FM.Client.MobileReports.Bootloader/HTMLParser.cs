using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Krista.FM.Client.MobileReports.Common;

namespace Krista.FM.Client.MobileReports.Bootloader
{
    class HTMLParser
    {
        /// <summary>
        /// Возвращает коллекцию с рессурсами
        /// </summary>
        /// <param name="documetText"></param>
        /// <returns></returns>
        static public MatchCollection GetResources(string documetText)
        {
            MatchCollection result = null;
            if (string.IsNullOrEmpty(documetText))
                return result;
            string pattern = string.Format("(url\\((?<{0}>[\\S\\s]*?)\\))|(\\ssrc=[\\\"|'](?<{1}>[\\S\\s]*?)[\\\"|'])|(<link\\shref=\\\"(?<hrefResources>[\\S\\s]*?)\\\")", 
                Consts.urlResources, Consts.srcResources);
            result = Regex.Matches(documetText, pattern, RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        /// Заменить пути к ресурсам
        /// </summary>
        /// <param name="documentText">исходный текст</param>
        /// <param name="resourcesInfo">информация о ресурсах</param>
        /// <param name="includeResourcePath">включать ли в путь, папку с ресурсами</param>
        /// <returns></returns>
        static public string SetResources(string documentText, List<ResourceInfo> resourcesInfo,
            bool includeResourcePath)
        {
            string result = documentText;
            int offSet = 0;
            foreach (ResourceInfo info in resourcesInfo)
            {
                string resourceName = includeResourcePath ? info.NewValue : info.Name;
                result = result.Remove(info.StartIndex + offSet, info.Lenght);
                result = result.Insert(info.StartIndex + offSet, resourceName);
                offSet += resourceName.Length - info.Lenght;
            }
            return result;
        }

        static public string AppendPHPHeader(string documentText, string reportID)
        {
            string header =
                @"<?php
	                error_reporting(0);
	                session_start();
	                if (!isset($_SESSION['userID']))
	                {
		                echo 'Пользователь не аутентифицирован';
		                exit();
	                }
                		
	                $reportID = '{0}';

	                $accessReports = $_SESSION['accessReports'];
	                $isContain = false;
	                foreach ($accessReports as $item)
	                {
		                if (strpos($reportID, $item) > -1)
		                {
			                $isContain = true;
			                break;
		                }
	                }
	                if (!$isContain)
                    {
                        echo 'Пользователь не имеет доступ к отчету: $reportID';
		                exit;
                    }
                ?>";
            header = header.Replace("{0}", reportID);
            return header + documentText;
        }

        /// <summary>
        /// Вырезаем xml вставки
        /// </summary>
        /// <param name="documentText"></param>
        /// <returns></returns>
        static public string RemoveXML(string documentText)
        {
            return Regex.Replace(documentText, @"<\?xml[\s\S].*?\?>", string.Empty);
        }

        static public string RemoveScripts(string documentText)
        {
            documentText = Regex.Replace(documentText, "<script[\\s\\S]*?</script>", string.Empty);
            // Вырезаем комментарии;
            return Regex.Replace(documentText, "<!--.*?-->", string.Empty);
        }

        /// <summary>
        /// Вместо родных скриптов, внедряем свои
        /// </summary>
        /// <param name="htmlPage">тело страницы</param>
        /// <param name="customScriptsPath">путь к папке со скриптами, которые будем внедрять</param>
        /// <param name="reportResourcePath">путь к ресурсу с отчетом</param>
        /// <returns></returns>
        static public string SetCustomScripts(string htmlPage, string customScriptsPath, 
            string reportResourcePath)
        {
            //infragistics 2008
            //Regex regex = new Regex("<script\\stype='text/javascript'><!--\\s*var[\\s\\S]*?=\\sigtbl_initGrid[\\s\\S]*?</script>");
            // ищем скрипт инициализации
            //ifragistics 2009.1
            Regex regex = new Regex("<script\\stype='text/javascript'>\\s*//<![CDDATA[\\s*var[\\s\\S]*?=\\sigtbl_initGrid[\\s\\S]*?</script>");
            MatchCollection gridScripts = regex.Matches(htmlPage);

            regex = new Regex("<script type=\"text/javascript\">[^(script)]*?InitilizeScrollbar[\\s\\S]*?InitializeUltraChart[\\s\\S]*?</script>");
            MatchCollection chartScripts = regex.Matches(htmlPage);

            regex = new Regex("<SCRIPT type='text/javascript'>[^(script)]*?InitilizeScrollbar[\\s\\S]*?InitializeTagCloud[\\s\\S]*?</SCRIPT>");
            MatchCollection tagCloudScripts = regex.Matches(htmlPage);

            if (((gridScripts.Count == 0) && (chartScripts.Count == 0) && (tagCloudScripts.Count == 0)) || (customScriptsPath == string.Empty) 
                || (reportResourcePath == string.Empty))
            {
                return HTMLParser.RemoveScripts(htmlPage);
            }

            //из полного пути извлечем имя папки
            string resourceFolderName = Path.GetFileName(reportResourcePath);

            #region Гриды
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            int patternNum = 0;
            string gridKey = string.Empty;
            foreach(Match script in gridScripts)
            {
                string patternName = string.Format("<!%GridScript{0}%!>", patternNum);
                // формируем новый скрипт
                replacements.Add(patternName, HTMLParser.ReduceGridScript(script.Captures[0].Value));
                // Готовим место
                htmlPage = htmlPage.Replace(script.Captures[0].Value, patternName);
                if (patternNum == 0)
                    //сохраним первый ключ, потом пригодится
                    gridKey = patternName;
                patternNum++;
            }
            #endregion

            #region Диаграммы
            Dictionary<string, string> chartReplacements = new Dictionary<string, string>();
            
            string chartKey = string.Empty;
            for(int i = 0; i < chartScripts.Count; i++)
            {
                string patternName = string.Format("<!%ChartScript{0}%!>", i);
                // формируем новый скрипт
                chartReplacements.Add(patternName, ReduceChartScript(chartScripts[i].Captures[0].Value));
                // Готовим место
                htmlPage = htmlPage.Replace(chartScripts[i].Captures[0].Value, patternName);
                if (i == 0)
                    //сохраним первый ключ, потом пригодится
                    chartKey = patternName;                
            }
            #endregion

            #region Облака
            
            string tagCloudKey = string.Empty;
            // облака не заменяем
            if (tagCloudScripts.Count > 0)
                    //сохраним первый ключ, потом пригодится
                    tagCloudKey = tagCloudScripts[0].Captures[0].Value;
            
            #endregion

            // тут жмем по всякому
            htmlPage = HTMLParser.RemoveScripts(htmlPage);

            if (gridKey != string.Empty)
            {
                string gridResourcePath = String.Format("{0}\\{1}", customScriptsPath, Consts.gridScripts);
                //копируем в папку с ресурсами пользовательские скрипты
                File.Copy(gridResourcePath, string.Format("{0}\\{1}", reportResourcePath, Consts.gridScripts));
                //добавляем к тексту страницы ссылку на пользовательский ресурс
                htmlPage = htmlPage.Insert(htmlPage.IndexOf(gridKey), string.Format("<script src=\"{0}/{1}\" type=\"text/javascript\"></script>{2}",
                    resourceFolderName, Consts.gridScripts, Environment.NewLine));
            }

            int chartScriptIndex = 0;
            if (chartKey != string.Empty)
            {
                chartScriptIndex = htmlPage.IndexOf(chartKey);
            }

            int tagCloudScriptIndex = 0;
            if (tagCloudKey != string.Empty)
            {
                tagCloudScriptIndex = htmlPage.IndexOf(tagCloudKey);
            }

            // Если чарт и облако оказались на одной странице
            if (chartKey != string.Empty &&
                tagCloudKey != string.Empty)
            {
                //ссылку на внешний скрипт поставим одну
                chartScriptIndex = Math.Min(chartScriptIndex, tagCloudScriptIndex);

                string chartResourcePath = String.Format("{0}\\{1}", customScriptsPath, Consts.chartScripts);
                //копируем в папку с ресурсами пользовательские скрипты
                File.Copy(chartResourcePath, string.Format("{0}\\{1}", reportResourcePath, Consts.chartScripts));
                //добавляем к тексту страницы ссылку на пользовательский ресурс
                htmlPage = htmlPage.Insert(chartScriptIndex, string.Format("<script src=\"{0}/{1}\" type=\"text/javascript\"></script>{2}",
                    resourceFolderName, Consts.chartScripts, Environment.NewLine));
            }
            else
            {
                if (chartKey != string.Empty)
                {
                    string chartResourcePath = String.Format("{0}\\{1}", customScriptsPath, Consts.chartScripts);
                    //копируем в папку с ресурсами пользовательские скрипты
                    File.Copy(chartResourcePath, string.Format("{0}\\{1}", reportResourcePath, Consts.chartScripts));
                    //добавляем к тексту страницы ссылку на пользовательский ресурс
                    htmlPage = htmlPage.Insert(htmlPage.IndexOf(chartKey), string.Format("<script src=\"{0}/{1}\" type=\"text/javascript\"></script>{2}",
                        resourceFolderName, Consts.chartScripts, Environment.NewLine));
                }
                if (tagCloudKey != string.Empty)
                {
                    string chartResourcePath = String.Format("{0}\\{1}", customScriptsPath, Consts.chartScripts);
                    //копируем в папку с ресурсами пользовательские скрипты
                    File.Copy(chartResourcePath, string.Format("{0}\\{1}", reportResourcePath, Consts.chartScripts));
                    //добавляем к тексту страницы ссылку на пользовательский ресурс
                    htmlPage = htmlPage.Insert(htmlPage.IndexOf(tagCloudKey), string.Format("<script src=\"{0}/{1}\" type=\"text/javascript\"></script>{2}",
                        resourceFolderName, Consts.chartScripts, Environment.NewLine));
                }
            }
            // заменяем шаблон на новый скрипт
            foreach(string key in replacements.Keys)
            {
                htmlPage = htmlPage.Replace(key, replacements[key]);
            }

            // заменяем шаблон на новый скрипт
            foreach (string key in chartReplacements.Keys)
            {
                htmlPage = htmlPage.Replace(key, chartReplacements[key]);
            }

            return htmlPage;
        }

        private static string ReduceGridScript(string htmlPage)
        {
            string output = "<script type=text/javascript><!--";
            output += Environment.NewLine;
            output += " igtbl_initGrid(";
            int count = 0;
            int pos = 0;
            
            char c = htmlPage[pos];

            // ищем начало фактических параметров.
            while (c != '(')
            {
                pos++;
                c = htmlPage[pos];
            }

            // сейчас у нас должна быть кавычка
            pos++;
            c = htmlPage[pos];
            // пишем ее в выходную строку и читаем дальше
            output += c;
            pos++;
            c = htmlPage[pos];

            // пока не наткнемся на следующую кавычку
            while (c != '"')
            {
                output += c;
                pos++;
                c = htmlPage[pos];
            }
            // дописываем сторую кавычку
            output += c;
            output += ',';

            // ищем массив инициализации грида
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }
            count++;
            pos++;
            c = htmlPage[pos];

            while (!(c == ',' && count <= 0))
            {
                if (c == '[')
                {
                    count++;
                }
                if (c == ']')
                {
                    count--;
                }
                if (count == 0)
                {
                    output += "[]";
                }
                pos++;
                c = htmlPage[pos];
            }
            output += c;

            // ищем массив инициализации бандов.
            // это массив массивов.
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }
            // первую скобку пишем сразу
            output += c;
            pos++;
            c = htmlPage[pos];

            // дальше работаем почти как с гридом
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }
            count++;
            pos++;
            c = htmlPage[pos];

            while (!(c == ',' && count <= 0))
            {
                if (c == '[')
                {
                    count++;
                }
                if (c == ']')
                {
                    count--;
                }
                if (count == 0)
                {
                    output += "[]";
                }
                pos++;
                c = htmlPage[pos];
            }
            // закрываем первую скобку
            output += ']';
            // разделитель
            output += ',';

            count = 0;
            // ищем массив инициализации колонок.
            // это массив массивов массивов.
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }
            // первую скобку пишем сразу
            output += c;
            pos++;
            c = htmlPage[pos];

            // дальше работаем как с бандом
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }
            // первую скобку пишем сразу
            output += c;
            pos++;
            c = htmlPage[pos];

            // дальше работаем как с гридом
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }

            while (count != -1)
            {
                if (c == '[')
                {
                    // плюсуем открытую скобку
                    count++;
                    if (count == 1)
                    {
                        output += "[";
                        pos++;
                        c = htmlPage[pos];
                        // ищем тип данных
                        while (c != ',')
                        {
                            pos++;
                            c = htmlPage[pos];
                        }
                        pos++;
                        c = htmlPage[pos];
                        // ищем тип данных
                        while (c != ',')
                        {
                            pos++;
                            c = htmlPage[pos];
                        }
                        output += "\"\",\"\",";
                        pos++;
                        c = htmlPage[pos];
                        while (c != ',')
                        {
                            output += c;
                            pos++;
                            c = htmlPage[pos];
                        }
                    }
                }
                if (c == ']')
                {
                    // минусуем открытую скобку
                    count--;
                }
                if (count == 0 && c != ',')
                {
                    output += "]";
                }
                if (count == 0 && c == ',')
                {
                    output += ",";
                }
                pos++;
                c = htmlPage[pos];
            }
            // закрываем первые скобки
            output += "]]";
            // дописываем массив событий и закрываем
            output += ",[]);";
            output += Environment.NewLine;
            output += "//--></script>";
            output += Environment.NewLine;
           
            return output;
        }

        private static string ReduceChartScript(string chartScript)
        {
            string chartName = Regex.Match(chartScript, "(?<chartName>[^\\s].*?) = new IGUltraChart\\(.*?\\);").Groups["chartName"].Value;

            string output = "<SCRIPT type=text/javascript><!--";
            output += Environment.NewLine;
            output += "function " + chartName + "_pRcEv(event, this_ref,row,column,event_name, layer_id) {";
            output += Environment.NewLine;
            output += string.Format("if ({0}_IsLoaded)", chartName);
            output += Environment.NewLine;
            output += string.Format("Bounce(event, \"{0}\", \"onallevent\", [this_ref, row, column, event_name, layer_id])", chartName);
            output += " }";
            output += Environment.NewLine;
            output += "function " + chartName + "_BaseMove(e) {";
            output += Environment.NewLine;
            output += string.Format("if ({0}_IsLoaded)", chartName);
            output += Environment.NewLine;
            output += string.Format("Bounce(e, '{0}');", chartName);
            output += " }";
            output += Environment.NewLine;
            output += string.Format("var {0} = null;", chartName);
            output += Environment.NewLine;
            output += "function Initialize" + chartName + "() {";
            output += Environment.NewLine;
            output += Regex.Match(chartScript, "var TD = new Array\\(.*?\\);").Captures[0].Value;
            output += Environment.NewLine;
            MatchCollection tooltipData = Regex.Matches(chartScript, "TD\\[\".*?\"\\] = \".*?\";");
            foreach (Match data in tooltipData)
            {
                output += data.Captures[0].Value;
                output += Environment.NewLine;
            }

            output += Regex.Match(chartScript, "var ED = new Array\\(.*?\\);").Captures[0].Value;
            output += Environment.NewLine;
            MatchCollection eventData = Regex.Matches(chartScript, "ED\\[\".*?\"\\] = \".*?\";");
            foreach (Match data in eventData)
            {
                output += data.Captures[0].Value;
                output += Environment.NewLine;
            }

            output += string.Format("var TOOLTIP = IGB.GetObject(\"{0}_IGTooltip\");", chartName);
            output += "if (TOOLTIP != null) if (TOOLTIP.style != null) with (TOOLTIP.style){";
            output += "align = \"left\";";
            output += "borderBottomWidths = \"1,1,1,1\";";
            output += "border = \"Black 1px outset\";";
            output += "backgroundColor = \"AntiqueWhite\";";
            output += "color = \"Black\";";
            output += "margins = \"1,1,1,1\";";
            output += "padding=\"0\";";
            output += "height = \"auto\";";
            output += "width = \"auto\";";
            output += "}";


            output += Environment.NewLine;
            output += string.Format("{0} = new IGUltraChart(\"{0}\", \"ChartImages\", \"{0}\");", chartName);
            output += Environment.NewLine;
            output += chartName + ".TooltipData = TD;";
            output += Environment.NewLine;
            output += chartName + ".EventData = ED;";
            output += Environment.NewLine;
            output += chartName + ".TooltipDisplay = 1;";
            output += Environment.NewLine;
            // Прослушиватели
            MatchCollection listeners = Regex.Matches(chartScript, ".AddListener\\(.*?\\);");
            foreach (Match data in listeners)
            {
                output += chartName + data.Captures[0].Value;
                output += Environment.NewLine;
            }
            output += chartName + "_IsLoaded = true; }";
            output += Environment.NewLine;
            output += string.Format("Initialize{0}();", chartName);
            output += Environment.NewLine;
            output += "//--></script>";
            output += Environment.NewLine;

            return output;
        }

        static public string OptimizeHTML(string documentText)
        {
            // Лишние пробелы убираем
            documentText = documentText.Replace("> ", ">");
            documentText = documentText.Replace(" >", ">");
            documentText = documentText.Replace("< ", "<");
            documentText = documentText.Replace(" <", "<");
            documentText = documentText.Replace("{ ", "{");
            documentText = documentText.Replace(" {", "{");
            documentText = documentText.Replace("} ", "}");
            documentText = documentText.Replace(" }", "}");
            documentText = documentText.Replace(" ;", ";");

            // Вьюстейты тоже не нужны
            documentText = Regex.Replace(documentText, "<input[\\s]*type=\"hidden\"[\\s]*name=\"__VIEWSTATE\".*?/>", string.Empty);

            // Оптимизирует 
            documentText = Utils.ReplaceAll(documentText, "\r\r", "\r");
            documentText = Utils.ReplaceAll(documentText, "\n\n", "\n");
            documentText = Utils.ReplaceAll(documentText, "\t\t", "\t");
            documentText = Utils.ReplaceAll(documentText, "\t ", "\t");
            documentText = Utils.ReplaceAll(documentText, "\r\n\r\n", "\r\n");
            // Лень было отдельную процедуру писать, с оптимизировал удаление пробелов прям здесь
            documentText = Utils.ReplaceAll(documentText, "          ", " ");
            documentText = Utils.ReplaceAll(documentText, "        ", " ");
            documentText = Utils.ReplaceAll(documentText, "      ", " ");
            documentText = Utils.ReplaceAll(documentText, "    ", " ");
            documentText = Utils.ReplaceAll(documentText, "  ", " ");

            return documentText;
        }

        /// <summary>
        /// Если изображение помещено в ячейку таблицы, браузер WM, делает дополнительные 
        /// отступы внутри ячейки, от чего урезает размер изображения, будем добавлять этот размер
        /// к размерам ячейки, что сохранит оригинальные размеры изображения
        /// </summary>
        /// <param name="appendingValue">добавляемое значение к размеру</param>
        /// <returns></returns>
        static public string AppendImageTdSize(string htmlPage, int appendingValue)
        {
            Regex regex = new Regex("<td style=\"(?<style>[^<]*?)\"[\\s]*><div");
            MatchCollection imageTdStyles = regex.Matches(htmlPage);

            if (imageTdStyles.Count > 0)
            {
                int offSet = 0;
                foreach (Match math in imageTdStyles)
                {
                    Group styleGroup = math.Groups["style"];
                    string oldStyle = styleGroup.Value;
                    string newStyle = oldStyle;
                    newStyle = AppendAtributeValue(newStyle, "width", appendingValue);
                    newStyle = AppendAtributeValue(newStyle, "height", appendingValue);

                    if (newStyle != oldStyle)
                    {
                        htmlPage = htmlPage.Remove(styleGroup.Index + offSet, oldStyle.Length);
                        htmlPage = htmlPage.Insert(styleGroup.Index + offSet, newStyle);

                        offSet += newStyle.Length - oldStyle.Length;
                    }
                }
            }
            return htmlPage;
        }

        static private string AppendAtributeValue(string style, string attributeName, int appendingValue)
        {
            string strRegex = string.Format(@"{0}:(?<value>[\S\s]*?)px;", attributeName);
            Regex regex = new Regex(strRegex);
            MatchCollection values = regex.Matches(style);
            if (values.Count > 0)
            {
                Group group = values[0].Groups["value"];
                string strValue = group.Value;
                int value;
                if (int.TryParse(strValue, out value))
                {
                    value += appendingValue;

                    style = style.Remove(group.Index, group.Length);
                    style = style.Insert(group.Index, value.ToString());
                }
            }
            return style;
        }
    }
}