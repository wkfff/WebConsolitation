using System;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    [Serializable]
    public class ExpertVersion
    {
        #region Поля

        private string number;
        private VersionType type;
        private int format;

        #endregion

        #region Свойства

        public string Number
        {
            get { return number; }
            set { number = value; }
        }

        public VersionType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Версия формата
        /// </summary>
        public int Format
        {
            get { return this.format; }
            set { this.format = value; }
        }

        #endregion 

        public ExpertVersion()
        {}
    }
}
