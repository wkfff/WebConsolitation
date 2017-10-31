using Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.ResultsOfActivity
{
    public interface IResultsOfActivityService : INewRestService
    {
        FinNFinAssetsViewModel GetFinNFinAssetsItem(int parentId);

        void SaveValRowFinNFinAssets(int recId, string valueId, decimal? val, int refStateValue, int? refTypeIxm);

        /// <summary>
        /// ѕолучение даных дл€ формы "»спользование имущества"
        /// </summary>
        /// <param name="parentId">ID документа</param>
        /// <returns>возвращаетс€ запись</returns>
        PropertyUseViewModel GetPropertyUseItem(int parentId);

        /// <summary>
        /// —охранение значени€ формы "»спользование имущества"
        /// </summary>
        /// <param name="recId">ID документа</param>
        /// <param name="valueId">ID записи значени€</param>
        /// <param name="beginYearVal">значение на начало года</param>
        /// <param name="endYearVal">значение на конец года</param>
        /// <param name="refStateValue">ID типа значени€</param>
        void SaveValRowPropertyUse(int recId, string valueId, decimal beginYearVal, decimal endYearVal, int refStateValue);

        /// <summary>
        /// ”даление документа
        /// </summary>
        /// <param name="recId">ID документа</param>
        void DeleteDoc(int recId);
    }
}