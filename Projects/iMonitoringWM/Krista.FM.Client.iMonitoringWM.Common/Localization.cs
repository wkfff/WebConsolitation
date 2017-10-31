using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.iMonitoringWM.Common
{
    public class Localization
    {
        private Dictionary<string, string> dictionary;

        public Localization()
        {
            dictionary = new Dictionary<string, string>();
            this.FillDictionary();
        }

        private void FillDictionary()
        {
            dictionary.Add("Unable to connect to the remote server", "Не удается подключиться к удаленному серверу.");
        }

        public string GetValue(string key)
        {
            string value = string.Empty;
            if (dictionary.TryGetValue(key, out value))
                return value;
            else
                return key;
        }
    }
}
