using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.OLAPAdmin
{
    /// <summary>
    /// Общий интерфейс для работы с утилитой администрирования многомерных баз
    /// </summary>
    interface IOLAPScheme
    {
        /// <summary>
        /// аутенфикация пользователя (в дальнейшем может использавться для определения прав на администрирование базы)
        /// </summary>
        string UserName { get; set; }
    }

    public class OLAPScheme : IOLAPScheme
    {
        /// <summary>
        /// Обеспечиваем единственный экземпляр схемы
        /// </summary>
        private OLAPScheme scheme = null;

        public OLAPScheme InstanceScheme()
        {
            if (scheme != null)
                return scheme;

            return new OLAPScheme();
        }

        #region IOLAPScheme Members

        public string UserName
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
