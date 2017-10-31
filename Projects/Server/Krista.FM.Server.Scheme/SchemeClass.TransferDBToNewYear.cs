using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.Common;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme
{
    /// <summary>
    /// Перевод базы на новый год
    /// </summary>
    internal partial class SchemeClass
    {
        #region Функция перевода базы на новый год

        private readonly Object objLock = new Object();

        public TransferDBToNewYearState TransferDBToNewYear(int currentYear)
        {
            lock (objLock)
            {
                NewYearTransferService newYearTransferService = new NewYearTransferService();
                return newYearTransferService.TransferDBToNewYear(currentYear);
            }
        }

        #endregion Функция перевода базы на новый год
    }
}
