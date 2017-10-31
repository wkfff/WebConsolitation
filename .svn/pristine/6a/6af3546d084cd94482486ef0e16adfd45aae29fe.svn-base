using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.FinSourcePlanning.Services
{
    internal class CurrencyRateService
    {
        internal static void SetCurrencyRatesReferences(IScheme scheme)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                IEntity currencyRates = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_ExchangeRate_Key);
                IEntity currencyList = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_OKV_Currency_Key);
                using (IDataUpdater duCurrency = currencyList.GetDataUpdater())
                {
                    DataTable dtCurrency = new DataTable();
                    duCurrency.Fill(ref dtCurrency);
                    using (IDataUpdater duCurrencyRates = currencyRates.GetDataUpdater())
                    {
                        DataTable dtCurrencyRates = new DataTable();
                        duCurrencyRates.Fill(ref dtCurrencyRates);
                        foreach (DataRow row in dtCurrencyRates.Select())
                        {
                            DataRow[] rows = dtCurrency.Select(string.Format("Code = {0}", row["CurrencyCode"]));
                            if (rows.Length > 0)
                                row["RefOkv"] = rows[0]["ID"];
                        }
                        DataTable dtChanges = dtCurrencyRates.GetChanges();
                        if (dtChanges != null)
                            duCurrencyRates.Update(ref dtChanges);
                    }
                }
            }
        }
    }
}
