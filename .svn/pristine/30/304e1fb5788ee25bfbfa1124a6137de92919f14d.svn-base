using System;
using Krista.FM.Common;

namespace Krista.FM.Client.DiagramEditor
{
    /// <summary>
    /// Реализует функционал хранения информации о версии
    /// </summary>
    public struct VersionInfo
    {
        public DateTime CreationDate { get; set; }

        public string Creator { get; set; }

        public DateTime ModificationDate { get; set; }

        public string Modifier { get; set; }

        /// <summary>
        /// Обновляет о последнем изменении
        /// </summary>
        public void Modify()
        {
            ModificationDate = DateTime.Now;
            Modifier = ClientAuthentication.UserName;
        }
    }
}
