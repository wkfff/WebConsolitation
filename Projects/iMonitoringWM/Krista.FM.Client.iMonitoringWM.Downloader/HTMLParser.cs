using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Krista.FM.Client.iMonitoringWM.Common;
using System.IO;

namespace Krista.FM.Client.iMonitoringWM.Downloader
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

            string pattern = string.Format("(url\\((?<{0}>[\\S\\s]*?)\\))|(\\ssrc=\\\"(?<{1}>[\\S\\s]*?)\\\")|(<link\\shref=\\\"(?<hrefResources>[\\S\\s]*?)\\\")",
                Consts.urlResources, Consts.srcResources);
            result = Regex.Matches(documetText, pattern, RegexOptions.IgnoreCase);
            return result;
        }

        static public string SetResources(string documentText, List<ResourceInfo> resourcesInfo)
        {
            string result = documentText;
            int offSet = 0;
            
            /*for (int i = 0; i < resourcesInfo.Count; i++)
            {
                ResourceInfo info = resourcesInfo[i];
                result = result.Remove(info.StartIndex + offSet, info.Lenght);
                result = result.Insert(info.StartIndex + offSet, info.NewValue);
                offSet += info.NewValue.Length - info.Lenght;
            }
            */
            
            foreach (ResourceInfo info in resourcesInfo)
            {
                result = result.Remove(info.StartIndex + offSet, info.Lenght);
                result = result.Insert(info.StartIndex + offSet, info.NewValue);
                offSet += info.NewValue.Length - info.Lenght;
            }
            return result;
        }
    }
}
