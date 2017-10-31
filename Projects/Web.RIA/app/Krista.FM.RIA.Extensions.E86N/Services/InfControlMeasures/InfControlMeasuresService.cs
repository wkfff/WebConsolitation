using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.InfControlMeasures
{
    public class InfControlMeasuresService : NewRestService, IInfControlMeasuresService
    {
        #region IInfControlMeasures Members

        /// <summary>
        /// Удаление документа
        /// </summary>
        /// <param name="recId">ID документа</param>
        public void DeleteDoc(int recId)
        {
            try
            {
                var membersOfStaff = from p in GetItems<F_Fact_InspectionEvent>()
                                 where p.RefParametr.ID == recId
                                 select p;

                membersOfStaff.Each(x => Delete<F_Fact_InspectionEvent>(x.ID));

                var extHeader = from p in GetItems<T_Fact_ExtHeader>()
                                     where p.RefParametr.ID == recId
                                     select p;

                extHeader.Each(x => Delete<T_Fact_ExtHeader>(x.ID));
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления документа \"Сведения о проведенных контрольных мероприятиях и их результатах\": " + e.Message, e);
            }
        }

        #endregion
    }
}