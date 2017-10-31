using System.Collections.Generic;
using System.Data;
using Ext.Net;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public interface IDataService
    {
        ////Store StoreData { get; set; }

        void Initialize();

        void FillData(int variantId);

        void LoadData(int variantId);

        void FillDataForNewComplexMethod(string xmlString);

        ////List<Criteria> GetCriteria();

        DataTable GetProgData();

        DataTable GetStaticData();

        DataTable GetRegulatorData();

        SortedList<string, double> GetCoeff();

        SortedList<int, bool> GetArrYears();

        List<ForecastStruct> GetForecastList();

        void NewData(int paramId);

        void LoadRegulators();
    }
}