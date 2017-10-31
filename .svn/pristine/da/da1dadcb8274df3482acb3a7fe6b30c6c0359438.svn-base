using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public interface IFactRepository : IRepository<F_OIV_REG10Qual> 
    {
        /// <summary>
        /// Возвращает значение показателя для заданного района
        /// При отсутствии в фактах возвращает проинициализированную запись
        /// </summary>
        /// <param name="markId">Id показателя.</param>
        /// <param name="territoryId">Id территории</param>
        F_OIV_REG10Qual GetFactForMarkTerritory(int markId, int territoryId);

        /// <summary>
        /// Возвращает данные показателей для Ответственного ОИВ.
        /// </summary>
        IList<F_OIV_REG10Qual> GetMarksForOiv();

        IList<F_OIV_REG10Qual> GetMarksForIMA();

        /// <summary>
        /// Возвращает данные показателей для ОМСУ.
        /// </summary>
        IList<F_OIV_REG10Qual> GetMarks(D_Territory_RF territory);

        /// <summary>
        /// Возвращает данные по территориям по указанному показателю
        /// </summary>
        IList<F_OIV_REG10Qual> GetTerritories(int markId);
    }
}