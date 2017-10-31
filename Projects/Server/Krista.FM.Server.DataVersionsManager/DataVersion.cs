using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;
using Krista.FM.Common;

namespace Krista.FM.Server.DataVersionsManager
{
    /// <summary>
    /// Версия классификатора
    /// </summary>
    public class DataVersion : DisposableObject, IDataVersion
    {
        public DataVersion()
        {
        }

        #region IDataVersion Members

        /// <summary>
        /// Уникальный идентификатор версии структуры
        /// </summary>
        public string PresentationKey { get; set; }

        private string name;
        /// <summary>
        /// Имя версии
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value.Length > 100 ? value.Substring(0, 100) : value; }
        }

        /// <summary>
        /// Признак текущей версии
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// ID версии в базе
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Уникальный идентификатор объекта, для которого создана версия
        /// </summary>
        public string ObjectKey { get; set; }

        /// <summary>
        /// ID источника данных для версии
        /// </summary>
        public int SourceID { get; set; }

        #endregion
    }
}
