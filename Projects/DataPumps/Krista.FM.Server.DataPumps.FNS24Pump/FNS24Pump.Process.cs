using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS24Pump
{
    public partial class FNS24PumpModule : CorrectedPumpModuleBase
    {

        #region Обработка

        private bool toFillOkatoField = false;
        private bool toCorrectAddress = false;

        #region Работа с базой и кэшами

        private void FillProcessCaches()
        {
            FillRowsCache(ref cacheEGRULOgrn, dsEGRUL.Tables[0], "Id", "OGRN");
            FillRowsCache(ref cacheOKKladrCode, dsOKKladr.Tables[0], "Code1", "OKATO");
            FillRowsCache(ref cacheOKKladrIndex, dsOKKladr.Tables[0], "Indeks", "OKATO");
            FillRowsCache(ref cacheOrgAdress, dsOrgAdress.Tables[0], "Id");
        }

        protected override void QueryDataForProcess()
        {
            InitDataSet(ref daEGRUL, ref dsEGRUL, clsEGRUL, false, string.Empty, string.Empty);
            InitDataSet(ref daEGRIP, ref dsEGRIP, clsEGRIP, false, string.Empty, string.Empty);
            InitDataSet(ref daOKKladr, ref dsOKKladr, clsOKKladr, false, string.Empty, string.Empty);
            InitDataSet(ref daOrgAdress, ref dsOrgAdress, clsOrgAdress, false, string.Empty, string.Empty);
            InitDataSet(ref daFounderOrgDet, ref dsFounderOrgDet, clsFounderOrgDet, false, string.Empty, string.Empty);
            InitDataSet(ref daAncestorDet, ref dsAncestorDet, clsAncestorDet, false, string.Empty, string.Empty);
            InitDataSet(ref daAssignDet, ref dsAssignDet, clsAssignDet, false, string.Empty, string.Empty);
            InitDataSet(ref daSubdivisionDet, ref dsSubdivisionDet, clsSubdivisionDet, false, string.Empty, string.Empty);
            FillProcessCaches();
        }

        protected override void ProcessFinalizing()
        {
            ClearDataSet(ref dsEGRUL);
            ClearDataSet(ref dsEGRIP);
            ClearDataSet(ref dsOKKladr);
            ClearDataSet(ref dsOrgAdress);
            ClearDataSet(ref dsFounderOrgDet);
            ClearDataSet(ref dsAncestorDet);
            ClearDataSet(ref dsAssignDet);
            ClearDataSet(ref dsSubdivisionDet);
        }

        protected override void UpdateProcessedData()
        {
            UpdateDataSet(daEGRUL, dsEGRUL, clsEGRUL);
            UpdateDataSet(daEGRIP, dsEGRIP, clsEGRIP);
            UpdateDataSet(daFounderOrgDet, dsFounderOrgDet, clsFounderOrgDet);
            UpdateDataSet(daAncestorDet, dsAncestorDet, clsAncestorDet);
            UpdateDataSet(daAssignDet, dsAssignDet, clsAssignDet);
            UpdateDataSet(daSubdivisionDet, dsSubdivisionDet, clsSubdivisionDet);
        }

        #endregion Работа с базой и кэшами

        private object GetOkatoFromKladr(Dictionary<string, string> cache, string key)
        {
            key = key.TrimStart(new char[] { '0' });
            if (cache.ContainsKey(key) && !string.IsNullOrEmpty(cache[key]))
                return cache[key];
            return DBNull.Value;
        }

        private void FillOkatoField(IEntity cls, DataTable dt, string okatoField, string ogrnField, string adressField)
        {
            int count = dt.Rows.Count;
            for (int curRow = 0; curRow < count; curRow++)
            {
                DataRow orgRow = dt.Rows[curRow];

                if ((TryGetLongValue(orgRow[okatoField]) != 0) || (TryGetLongValue(orgRow[adressField]) <= 0))
                    continue;

                int refAdress = Convert.ToInt32(orgRow[adressField]);
                if (!cacheOrgAdress.ContainsKey(refAdress))
                    continue;

                DataRow adressRow = cacheOrgAdress[refAdress];
                if (TryGetLongValue(adressRow["KodKLNasPunkt"]) != 0)
                {
                    orgRow[okatoField] = GetOkatoFromKladr(cacheOKKladrCode, adressRow["KodKLNasPunkt"].ToString());
                    if ((orgRow[okatoField] != null) && (orgRow[okatoField] != DBNull.Value))
                        continue;
                }
                if (TryGetLongValue(adressRow["KodKLGorod"]) != 0)
                {
                    orgRow[okatoField] = GetOkatoFromKladr(cacheOKKladrCode, adressRow["KodKLGorod"].ToString());
                    if ((orgRow[okatoField] != null) && (orgRow[okatoField] != DBNull.Value))
                        continue;
                }
                if (TryGetLongValue(adressRow["KodKLRaion"]) != 0)
                {
                    orgRow[okatoField] = GetOkatoFromKladr(cacheOKKladrCode, adressRow["KodKLRaion"].ToString());
                    if ((orgRow[okatoField] != null) && (orgRow[okatoField] != DBNull.Value))
                        continue;
                }
                if (TryGetLongValue(adressRow["Indeks"]) != 0)
                {
                    orgRow[okatoField] = GetOkatoFromKladr(cacheOKKladrIndex, adressRow["Indeks"].ToString());
                    if ((orgRow[okatoField] != null) && (orgRow[okatoField] != DBNull.Value))
                        continue;
                }
                if (TryGetLongValue(adressRow["KodKLRegion"]) != 0)
                {
                    orgRow[okatoField] = GetOkatoFromKladr(cacheOKKladrCode, adressRow["KodKLRegion"].ToString().PadRight(11, '0'));
                    if ((orgRow[okatoField] != null) && (orgRow[okatoField] != DBNull.Value))
                        continue;
                }
                string ogrn = string.Empty;
                if (ogrnField == string.Empty)
                    ogrn = cacheEGRULOgrn[Convert.ToInt32(orgRow["RefOrgEGRUL"])];
                else
                    ogrn = orgRow[ogrnField].ToString();
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                    "Для организации ОГРН - {0} не заполнено поле '{1}' таблицы '{2}' ({3}).",
                    ogrn, okatoField, cls.OlapName, cls.FullDBName));
            }
        }

        // регулярное выражение для удаления из адреса фиксированных записей
        private Regex regExCorrectAdress = new Regex(
            "(Индекс: |Регион: |Город: |Район: |Населенный пункт: |Улица: |Номер дома, владения: |Номер корпуса, строения: |Номер квартиры, офиса: )",
            RegexOptions.IgnoreCase);
        private void CorrectAddress(DataTable dt, string adressField)
        {
            int rowsCount = dt.Rows.Count;
            for (int curRow = 0; curRow < rowsCount; curRow++)
            {
                string adress = Convert.ToString(dt.Rows[curRow][adressField]);
                adress = regExCorrectAdress.Replace(adress, string.Empty);
                dt.Rows[curRow][adressField] = adress;
            }
        }

        protected override void ProcessDataSource()
        {
            if (toFillOkatoField)
            {
                FillOkatoField(clsEGRUL, dsEGRUL.Tables[0], "OKATO", "OGRN", "RefAdress");
                FillOkatoField(clsEGRUL, dsEGRUL.Tables[0], "OKATODirect", "OGRNDirect", "RefAdressDirect");
                FillOkatoField(clsFounderOrgDet, dsFounderOrgDet.Tables[0], "OKATO", "OGRN", "RefAdress");
                FillOkatoField(clsAncestorDet, dsAncestorDet.Tables[0], "OKATO", "OGRN", "RefAdress");
                FillOkatoField(clsAssignDet, dsAssignDet.Tables[0], "OKATO", "OGRN", "RefAdress");
                FillOkatoField(clsSubdivisionDet, dsSubdivisionDet.Tables[0], "OKATO", string.Empty, "RefAdress");
                FillOkatoField(clsEGRIP, dsEGRIP.Tables[0], "OKATO", "OGRNIP", "RefAdress");
            }
            if (toCorrectAddress)
            {
                toCorrectAddress = false;
                CorrectAddress(dsEGRUL.Tables[0], "Adress");
                CorrectAddress(dsEGRUL.Tables[0], "AdressDirect");
                CorrectAddress(dsFounderOrgDet.Tables[0], "Adress");
                CorrectAddress(dsAncestorDet.Tables[0], "Adress");
                CorrectAddress(dsAssignDet.Tables[0], "Adress");
                CorrectAddress(dsSubdivisionDet.Tables[0], "Adress");
                CorrectAddress(dsEGRIP.Tables[0], "Adress");
            }
        }

        protected override void DirectProcessData()
        {
            toFillOkatoField = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbFillOkatoField", "False"));
            toCorrectAddress = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbCorrectAddress", "False"));
            string message = string.Empty;
            if (toFillOkatoField)
            {
                message = "Заполнение поля 'ОКАТО' по классификатору 'ОК.КЛАДР'";
            }
            if (toCorrectAddress)
            {
                if (message != string.Empty)
                    message += "; ";
                message += "Корректировка поля 'Адрес'";
            }
            ProcessDataSourcesTemplate(message);
        }

        #endregion Обработка

        #region Сопоставление

        // Организации.ЕГРЮЛ -> Районы.Сопоставимый (a_Org_EGRUL_RefRegion)
        private const string A_ORG_EGRUL_GUID = "77e520f3-f4be-4128-b414-44cf0e674db7";
        private void AssociateClsUL()
        {
            IEntityAssociation association = clsEGRUL.Associations[A_ORG_EGRUL_GUID];
            if (!(association is IBridgeAssociation))
                return;

            try
            {
                ((IBridgeAssociation)association).Associate();
            }
            catch (ThreadAbortException)
            {
                this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeFinishedWithError, "Нет данных", "Нет данных", string.Format(
                    "Сопоставление {0} закончено с ошибками: операция прервана пользователем.", association.FullCaption),
                    this.PumpID, -1);
                WriteToTrace(string.Format(
                    "Сопоставление {0} закончено с ошибками: операция прервана пользователем.", association.FullCaption),
                    TraceMessageKind.Error);
                throw;
            }
            catch (Exception ex)
            {
                WriteToTrace(string.Format(
                    "Сопоставление {0} закончено с ошибками: {1}",
                    association.FullCaption, ex.ToString()), TraceMessageKind.Error);
            }
        }

        // сопоставление классификатора "Организации.ЕГРЮЛ" -> "Районы.Сопоставимый"
        protected override void DirectAssociateData()
        {
            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(BridgeOperationsEventKind.dpeStartDataSourceProcessing,
                "Нет данных", "Нет данных", "Старт сопоставления данных.", this.PumpID, -1);
            WriteToTrace("Старт сопоставления данных.", TraceMessageKind.Information);

            
            // Сопоставление данных

            clsEGRUL = this.Scheme.Classifiers[B_ORG_EGRUL_GUID];
            string semantic = clsEGRUL.FullCaption;
            WriteToTrace(string.Format("Сопоставление данных {0}...", semantic), TraceMessageKind.Information);

            AssociateClsUL();

            WriteToTrace(string.Format("Данные {0} сопоставлены.", semantic), TraceMessageKind.Information);


            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(BridgeOperationsEventKind.dpeSuccessfullFinishDataSourceProcess,
                "Нет данных", "Нет данных", "Сопоставление данных закончено.", this.PumpID, -1);
            WriteToTrace("Сопоставление данных закончено.", TraceMessageKind.Information);
        }

        #endregion Сопоставление

    }
}
