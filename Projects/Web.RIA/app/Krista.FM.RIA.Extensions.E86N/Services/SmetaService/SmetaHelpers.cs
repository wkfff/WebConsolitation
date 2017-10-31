using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models;

namespace Krista.FM.RIA.Extensions.E86N.Services.SmetaService
{
    static class SmetaHelpers
    {
        static public SmetaViewModel ConvertToViewModel(F_Fin_Smeta item)
        {
            return
                new SmetaViewModel
                    {
                        RefBudgetIDName = item.RefBudget.Code + "; " + item.RefBudget.Name + ";",
                        RefParameterID = item.RefParametr.ID,
                        RefBudgetID = item.RefBudget.ID,
                        
                        ID = item.ID,
                        Funds = item.Funds,
                        FundsOneYear = item.FundsOneYear,
                        FundsTwoYear = item.FundsTwoYear,
                        CelStatya = item.CelStatya,
                        Event = item.Event
                    }
                    .Do(
                        model => item.RefKosgy.Do(
                            kosgy =>
                                {
                                    model.RefKosgyID = kosgy.ID;
                                    model.RefKosgyIDName = kosgy.Code + "; " + kosgy.Name + ";";
                                }))
                    .Do(
                        model => item.RefRazdPodr.Do(
                            podr =>
                                {
                                    model.RefRazdPodrID = podr.ID;
                                    model.RefRazdPodrIDName = podr.Code + "; " + podr.Name + ";";
                                }))
                    .Do(
                        model => item.RefVidRash.Do(
                            rash =>
                                {
                                    model.RefVidRashID = rash.ID;
                                    model.RefVidRashIDName = rash.Code + "; " + rash.Name + ";";
                                }));
        }

        static public void ExportMetadataTo(JsonReader jsonReader)
        {
            jsonReader.Fields.Clear();
            jsonReader.Fields.Add("ID");
            jsonReader.Fields.Add("RefParameterID");

            jsonReader.Fields.Add("RefBudgetIDName");
            
            jsonReader.Fields.Add("RefKosgyIDName");
            jsonReader.Fields.Add("RefRazdPodrIDName");
            jsonReader.Fields.Add("RefVidRashIDName");

            jsonReader.Fields.Add("RefBudgetID");
            jsonReader.Fields.Add("RefRazdPodrID");
            jsonReader.Fields.Add("RefKosgyID");
            jsonReader.Fields.Add("RefVidRashID");

            jsonReader.Fields.Add("Funds");
            jsonReader.Fields.Add("FundsOneYear");
            jsonReader.Fields.Add("FundsTwoYear");
            jsonReader.Fields.Add("CelStatya");
            jsonReader.Fields.Add("Event");
        }
    }
}
