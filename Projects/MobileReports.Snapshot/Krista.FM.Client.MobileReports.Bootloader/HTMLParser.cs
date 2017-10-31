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
        /// ���������� ��������� � ����������
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
        /// �������� ���� � ��������
        /// </summary>
        /// <param name="documentText">�������� �����</param>
        /// <param name="resourcesInfo">���������� � ��������</param>
        /// <param name="includeResourcePath">�������� �� � ����, ����� � ���������</param>
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
		                echo '������������ �� ����������������';
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
                        echo '������������ �� ����� ������ � ������: $reportID';
		                exit;
                    }
                ?>";
            header = header.Replace("{0}", reportID);
            return header + documentText;
        }

        /// <summary>
        /// �������� xml �������
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
            // �������� �����������;
            return Regex.Replace(documentText, "<!--.*?-->", string.Empty);
        }

        /// <summary>
        /// ������ ������ ��������, �������� ����
        /// </summary>
        /// <param name="htmlPage">���� ��������</param>
        /// <param name="customScriptsPath">���� � ����� �� ���������, ������� ����� ��������</param>
        /// <param name="reportResourcePath">���� � ������� � �������</param>
        /// <returns></returns>
        static public string SetCustomScripts(string htmlPage, string customScriptsPath, 
            string reportResourcePath)
        {
            //infragistics 2008
            //Regex regex = new Regex("<script\\stype='text/javascript'><!--\\s*var[\\s\\S]*?=\\sigtbl_initGrid[\\s\\S]*?</script>");
            // ���� ������ �������������
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

            //�� ������� ���� �������� ��� �����
            string resourceFolderName = Path.GetFileName(reportResourcePath);

            #region �����
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            int patternNum = 0;
            string gridKey = string.Empty;
            foreach(Match script in gridScripts)
            {
                string patternName = string.Format("<!%GridScript{0}%!>", patternNum);
                // ��������� ����� ������
                replacements.Add(patternName, HTMLParser.ReduceGridScript(script.Captures[0].Value));
                // ������� �����
                htmlPage = htmlPage.Replace(script.Captures[0].Value, patternName);
                if (patternNum == 0)
                    //�������� ������ ����, ����� ����������
                    gridKey = patternName;
                patternNum++;
            }
            #endregion

            #region ���������
            Dictionary<string, string> chartReplacements = new Dictionary<string, string>();
            
            string chartKey = string.Empty;
            for(int i = 0; i < chartScripts.Count; i++)
            {
                string patternName = string.Format("<!%ChartScript{0}%!>", i);
                // ��������� ����� ������
                chartReplacements.Add(patternName, ReduceChartScript(chartScripts[i].Captures[0].Value));
                // ������� �����
                htmlPage = htmlPage.Replace(chartScripts[i].Captures[0].Value, patternName);
                if (i == 0)
                    //�������� ������ ����, ����� ����������
                    chartKey = patternName;                
            }
            #endregion

            #region ������
            
            string tagCloudKey = string.Empty;
            // ������ �� ��������
            if (tagCloudScripts.Count > 0)
                    //�������� ������ ����, ����� ����������
                    tagCloudKey = tagCloudScripts[0].Captures[0].Value;
            
            #endregion

            // ��� ���� �� �������
            htmlPage = HTMLParser.RemoveScripts(htmlPage);

            if (gridKey != string.Empty)
            {
                string gridResourcePath = String.Format("{0}\\{1}", customScriptsPath, Consts.gridScripts);
                //�������� � ����� � ��������� ���������������� �������
                File.Copy(gridResourcePath, string.Format("{0}\\{1}", reportResourcePath, Consts.gridScripts));
                //��������� � ������ �������� ������ �� ���������������� ������
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

            // ���� ���� � ������ ��������� �� ����� ��������
            if (chartKey != string.Empty &&
                tagCloudKey != string.Empty)
            {
                //������ �� ������� ������ �������� ����
                chartScriptIndex = Math.Min(chartScriptIndex, tagCloudScriptIndex);

                string chartResourcePath = String.Format("{0}\\{1}", customScriptsPath, Consts.chartScripts);
                //�������� � ����� � ��������� ���������������� �������
                File.Copy(chartResourcePath, string.Format("{0}\\{1}", reportResourcePath, Consts.chartScripts));
                //��������� � ������ �������� ������ �� ���������������� ������
                htmlPage = htmlPage.Insert(chartScriptIndex, string.Format("<script src=\"{0}/{1}\" type=\"text/javascript\"></script>{2}",
                    resourceFolderName, Consts.chartScripts, Environment.NewLine));
            }
            else
            {
                if (chartKey != string.Empty)
                {
                    string chartResourcePath = String.Format("{0}\\{1}", customScriptsPath, Consts.chartScripts);
                    //�������� � ����� � ��������� ���������������� �������
                    File.Copy(chartResourcePath, string.Format("{0}\\{1}", reportResourcePath, Consts.chartScripts));
                    //��������� � ������ �������� ������ �� ���������������� ������
                    htmlPage = htmlPage.Insert(htmlPage.IndexOf(chartKey), string.Format("<script src=\"{0}/{1}\" type=\"text/javascript\"></script>{2}",
                        resourceFolderName, Consts.chartScripts, Environment.NewLine));
                }
                if (tagCloudKey != string.Empty)
                {
                    string chartResourcePath = String.Format("{0}\\{1}", customScriptsPath, Consts.chartScripts);
                    //�������� � ����� � ��������� ���������������� �������
                    File.Copy(chartResourcePath, string.Format("{0}\\{1}", reportResourcePath, Consts.chartScripts));
                    //��������� � ������ �������� ������ �� ���������������� ������
                    htmlPage = htmlPage.Insert(htmlPage.IndexOf(tagCloudKey), string.Format("<script src=\"{0}/{1}\" type=\"text/javascript\"></script>{2}",
                        resourceFolderName, Consts.chartScripts, Environment.NewLine));
                }
            }
            // �������� ������ �� ����� ������
            foreach(string key in replacements.Keys)
            {
                htmlPage = htmlPage.Replace(key, replacements[key]);
            }

            // �������� ������ �� ����� ������
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

            // ���� ������ ����������� ����������.
            while (c != '(')
            {
                pos++;
                c = htmlPage[pos];
            }

            // ������ � ��� ������ ���� �������
            pos++;
            c = htmlPage[pos];
            // ����� �� � �������� ������ � ������ ������
            output += c;
            pos++;
            c = htmlPage[pos];

            // ���� �� ��������� �� ��������� �������
            while (c != '"')
            {
                output += c;
                pos++;
                c = htmlPage[pos];
            }
            // ���������� ������ �������
            output += c;
            output += ',';

            // ���� ������ ������������� �����
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

            // ���� ������ ������������� ������.
            // ��� ������ ��������.
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }
            // ������ ������ ����� �����
            output += c;
            pos++;
            c = htmlPage[pos];

            // ������ �������� ����� ��� � ������
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
            // ��������� ������ ������
            output += ']';
            // �����������
            output += ',';

            count = 0;
            // ���� ������ ������������� �������.
            // ��� ������ �������� ��������.
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }
            // ������ ������ ����� �����
            output += c;
            pos++;
            c = htmlPage[pos];

            // ������ �������� ��� � ������
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }
            // ������ ������ ����� �����
            output += c;
            pos++;
            c = htmlPage[pos];

            // ������ �������� ��� � ������
            while (c != '[')
            {
                pos++;
                c = htmlPage[pos];
            }

            while (count != -1)
            {
                if (c == '[')
                {
                    // ������� �������� ������
                    count++;
                    if (count == 1)
                    {
                        output += "[";
                        pos++;
                        c = htmlPage[pos];
                        // ���� ��� ������
                        while (c != ',')
                        {
                            pos++;
                            c = htmlPage[pos];
                        }
                        pos++;
                        c = htmlPage[pos];
                        // ���� ��� ������
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
                    // �������� �������� ������
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
            // ��������� ������ ������
            output += "]]";
            // ���������� ������ ������� � ���������
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
            // ��������������
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
            // ������ ������� �������
            documentText = documentText.Replace("> ", ">");
            documentText = documentText.Replace(" >", ">");
            documentText = documentText.Replace("< ", "<");
            documentText = documentText.Replace(" <", "<");
            documentText = documentText.Replace("{ ", "{");
            documentText = documentText.Replace(" {", "{");
            documentText = documentText.Replace("} ", "}");
            documentText = documentText.Replace(" }", "}");
            documentText = documentText.Replace(" ;", ";");

            // ��������� ���� �� �����
            documentText = Regex.Replace(documentText, "<input[\\s]*type=\"hidden\"[\\s]*name=\"__VIEWSTATE\".*?/>", string.Empty);

            // ������������ 
            documentText = Utils.ReplaceAll(documentText, "\r\r", "\r");
            documentText = Utils.ReplaceAll(documentText, "\n\n", "\n");
            documentText = Utils.ReplaceAll(documentText, "\t\t", "\t");
            documentText = Utils.ReplaceAll(documentText, "\t ", "\t");
            documentText = Utils.ReplaceAll(documentText, "\r\n\r\n", "\r\n");
            // ���� ���� ��������� ��������� ������, � ������������� �������� �������� ���� �����
            documentText = Utils.ReplaceAll(documentText, "          ", " ");
            documentText = Utils.ReplaceAll(documentText, "        ", " ");
            documentText = Utils.ReplaceAll(documentText, "      ", " ");
            documentText = Utils.ReplaceAll(documentText, "    ", " ");
            documentText = Utils.ReplaceAll(documentText, "  ", " ");

            return documentText;
        }

        /// <summary>
        /// ���� ����������� �������� � ������ �������, ������� WM, ������ �������������� 
        /// ������� ������ ������, �� ���� ������� ������ �����������, ����� ��������� ���� ������
        /// � �������� ������, ��� �������� ������������ ������� �����������
        /// </summary>
        /// <param name="appendingValue">����������� �������� � �������</param>
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