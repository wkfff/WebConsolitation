using System;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public interface IVersioningService : INewRestService
    {
        /// <summary>
        /// Проверяет закрыт ли документ.
        /// </summary>
        /// <param name="docId"> идентификатор документа</param>
        /// <returns> true если документ закрыт </returns>
        bool GetCloseState(int docId);

        DateTime GetOpenDate(int docId);

        F_F_ParameterDoc GetDocumentForCopy(int docId);

        int GetDocumentIdForOGS(int recId);

        bool CheckCloseDocs(int docId);

        bool CheckDocs(int docId);

        void CloseDocument(int docId);

        void OpenDocument(int docId);

        void CloseOgs(int recId, DateTime closeDate, string note);

        void OpenOgs(int recId);
    }
}