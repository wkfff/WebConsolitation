using System;

namespace Krista.FM.Common
{
    [System.Diagnostics.DebuggerStepThrough()]
    public class OlapConnectionString : ConnectionString
    {
        /// <summary>
        /// Значение по умолчанию для параметра выбора алгоритма генерации уникальных имен
        /// </summary>
        private const string defaultMDXUniqueNameStyle = "0";

        public string ClientCacheSize;
        public string AutoSynchPeriod;
        public string MDXUniqueNameStyle = defaultMDXUniqueNameStyle;

        public OlapConnectionString()
            : base()
        {
        }

        public override void Parse(String cs)
        {
            base.Parse(cs);

            string[] parameters = cs.Split(';');
            foreach (string prm in parameters)
            {
                string[] keyValue = prm.Split('=');
                switch (keyValue[0])
                {
                    case "Client Cache Size":
                        ClientCacheSize = keyValue[1]; break;
                    case "Auto Synch Period":
                        AutoSynchPeriod = keyValue[1]; break;
                    case "MDX Unique Name Style":
                        MDXUniqueNameStyle = keyValue[1]; break;
                }
            }
        }

        public override string ToString()
        {
            return String.Format(
                "{0};Client Cache Size={1};Auto Synch Period={2}",
                base.ToString(), ClientCacheSize, AutoSynchPeriod);
        }

        public string OleDbConnectionString
        {
            get
            {
                return string.Format("Provider={0};Data Source={1}", this.Provider, this.DataSource);
            }
        }
    }
}
