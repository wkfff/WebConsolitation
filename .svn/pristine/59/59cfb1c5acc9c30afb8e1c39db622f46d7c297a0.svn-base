using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public interface IMarksOmsuRepository : IRepository<F_OMSU_Reg16> 
    {
        /// <summary>
        /// Возвращает значение показателя для заданного района.
        /// </summary>
        /// <param name="markId">Id показателя.</param>
        /// <param name="regionId">Id района.</param>
        F_OMSU_Reg16 GetFactForMarkRegion(int markId, int regionId);

        /// <summary>
        /// Возвращает данные показателей для Ответственного ОИВ.
        /// </summary>
        /// <param name="markId">Id показателя, данные по которому нужно вернуть.</param>
        IList<F_OMSU_Reg16> GetForOIV(int markId);

        /// <summary>
        /// Возвращает данные показателей прошлого года, соответствующие указанному через сопоставление.
        /// </summary>
        IList<F_OMSU_Reg16> GetForOIVPrevious(int markId);

        /// <summary>
        /// Возвращает данные по всем показателям для выбранного района (МО).
        /// </summary>
        /// <param name="region">Район, данные по которому нужно вернуть.</param>
        IList<F_OMSU_Reg16> GetForMO(D_Regions_Analysis region);
        
        /// <summary>
        /// Возвращает данные по дочерним показателям указанного показателя для выбранного района (МО).
        /// </summary>
        /// <param name="region">Район, данные по которому нужно вернуть.</param>
        /// <param name="markId">Id родительского показателя.</param>
        IList<F_OMSU_Reg16> GetForMO(D_Regions_Analysis region, int markId);
        
        /// <summary>
        /// Возвращает данные прошлого года по дочерним показателям указанного показателя для выбранного района (МО).
        /// </summary>
        IList<F_OMSU_Reg16> GetForMOPrevious(D_Regions_Analysis region, int markId);
        
        /// <summary>
        /// Возвращает данные по всем показателям для выбранного района (МО) без учета иерархии.
        /// </summary>
        /// <param name="region">Район, данные по которому нужно вернуть.</param>
        IList<F_OMSU_Reg16> GetAllMarksForMO(D_Regions_Analysis region);

        /// <summary>
        /// Возвращает данные прошлого года по всем показателям для выбранного района (МО) без учета иерархии.
        /// </summary>
        IList<F_OMSU_Reg16> GetAllMarksForMOPrevious(D_Regions_Analysis region);

        /// <summary>
        /// Возвращает данные заданного показателя по всем районам для текущего года без учета иерархии.
        /// </summary>
        IEnumerable<F_OMSU_Reg16> GetCurrentYearFactsOfAllRegions(int markId);
    }
}