using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.Core.HttpModules
{
    public class ScriptPatterns
    {
        private static Dictionary<string, string> patternResourceDictionary;
        
        public static Dictionary<string, string> PatternResourceDictionary
        {
            get
            {
                if (patternResourceDictionary == null || patternResourceDictionary.Count == 0)
                {
                    FillScriptsDictionary();
                }
                return patternResourceDictionary;
            }
        }

        private static void FillScriptsDictionary()
        {
            patternResourceDictionary = new Dictionary<string, string>();

            // Добавляем обрабатываемые ресурсы

            //Грид
            patternResourceDictionary.Add(
                    "igtbl_initGrid", "GridResource");
            // Чарт
            patternResourceDictionary.Add(
                    "InitilizeScrollbar", "ChartResource");
            // Кнопка
            patternResourceDictionary.Add(
                    "catch\\(e\\){status=\"Can not init button\";}", "ImageButtonResource");
            // Панель
            patternResourceDictionary.Add(
                    "ig_CreateWebPanel", "WebPanelResource");
            // Дерево
            patternResourceDictionary.Add(
                    "ig_tree", "WebTreeResource");
            // Календарь
            patternResourceDictionary.Add(
                    "WebCalendar", "CalendarResource");
            // Гейдж
            patternResourceDictionary.Add("ig_CreateWebGauge", "UltraGauge");
            // Аутентификация
            patternResourceDictionary.Add("ValidatorOnSubmit", "LoginResource");
            // Общие скрипты Infragistics
            patternResourceDictionary.Add("CommonScriptResource", "Common");
            // ДундасМап
            patternResourceDictionary.Add("MapManager", "DundasMap");
        }
    }
}
