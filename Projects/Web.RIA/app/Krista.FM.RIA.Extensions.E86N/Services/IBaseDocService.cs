using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public interface IBaseDocService : INewRestService
    {
        /// <summary>
        /// ”даление документа
        /// </summary>
        /// <param name="docId">ID документа</param>
        void DeleteDoc(int docId);

        /// <summary>
        /// ѕроверка на пустой документ
        /// </summary>
        /// <param name="docId">ID документа</param>
        bool CheckDocEmpty(int docId);

        /// <summary>
        ///  опирование контента документа в новый документ
        /// </summary>
        /// <param name="docId">ID документа</param>
        void CopyContent(int docId);
    }
}