using Ext.Net.MVC;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance
{
    public interface IAnnualBalanceService : INewRestService
    {
        RestResult F0503730Read(int recId, int section);

        RestResult F0503730Save(string data, int recId, int section);

        RestResult F0503121Read(int recId, int section);

        RestResult F0503121Save(string data, int recId, int section);

        RestResult F0503127Read(int recId, int section);

        RestResult F0503127Save(string data, int recId, int section);

        RestResult F0503130Read(int recId, int section);

        RestResult F0503130Save(string data, int recId, int section);

        RestResult F0503137Read(int recId, int section);

        RestResult F0503137Save(string data, int recId, int section);
        
        RestResult F0503737Read(int recId, int section);

        RestResult F0503737Save(string data, int recId, int section);

        /// <summary>
        /// Удаление документа
        /// </summary>
        /// <param name="recId">ID документа</param>
        void DeleteDoc(int recId);

        /// <summary>
        /// Расчет сумм для заданных показателей
        /// </summary>
        /// <param name="docId">идентификатор документа</param>
        /// <param name="section">детализация в документе</param>
        void CalculateSumm(int docId, int section);

        /// <summary>
        /// Заполнение документа по умолчанию 
        /// </summary>
        void СheckDocContent(int recId);
    }
}