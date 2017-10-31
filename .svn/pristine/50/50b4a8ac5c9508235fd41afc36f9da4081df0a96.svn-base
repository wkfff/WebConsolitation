using System.Collections;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Models;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Servises
{
    public interface IAreaService
    {
        IList GetAreasTable(bool filterEdit, bool filterReview, bool filterAccepted, IUserCredentials userCredentials);

        AreaDetailViewModel GetInitialAreaModel();

        AreaDetailViewModel GetAreaModel(int areaId);

        D_InvArea_Reestr GetProject(int id);

        void SaveProject(D_InvArea_Reestr entityNew, D_InvArea_Reestr entityOld, IUserCredentials userCredentials);

        D_Territory_RF GetRefTerritory(int id);

        FX_InvArea_Status GetRefStatus(int id);

        void DeleteProject(int id, IUserCredentials userCredentials);
    }
}