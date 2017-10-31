using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.DiverseInfo
{
    public class DiverseInfoService : NewRestService, IDiverseInfoService
    {
        /// <summary>
        ///   Удаление документа
        /// </summary>
        /// <param name="docId">ID документа</param>
        public void DeleteDoc(int docId)
        {
            try
            {
                GetItems<T_F_TofkList>().Where(x => x.RefParameterDoc.ID == docId).Each(Delete);
                GetItems<T_F_PaymentDetailsTargets>().Where(x => x.RefPaymentDetails.RefParameterDoc.ID == x.ID).Each(Delete);
                GetItems<T_F_PaymentDetails>().Where(x => x.RefParameterDoc.ID == docId).Each(Delete);
                GetItems<T_F_LicenseDetails>().Where(x => x.RefParameterDoc.ID == docId).Each(Delete);
                GetItems<T_F_AccreditationDetails>().Where(x => x.RefParameterDoc.ID == docId).Each(Delete);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления документа \"Иная информация об учреждении\": " + e.Message, e);
            }
        }

        /// <summary>
        /// Проверка на пустой документ
        /// </summary>
        /// <param name="docId">ID документа</param>
        public bool CheckDocEmpty(int docId)
        {
            if (GetItems<T_F_TofkList>().Any(x => x.RefParameterDoc.ID == docId))
            {
                return false;
            }

            if (GetItems<T_F_PaymentDetails>().Any(x => x.RefParameterDoc.ID == docId))
            {
                return false;
            }

            if (GetItems<T_F_LicenseDetails>().Any(x => x.RefParameterDoc.ID == docId))
            {
                return false;
            }

            if (GetItems<T_F_AccreditationDetails>().Any(x => x.RefParameterDoc.ID == docId))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Копирование контента документа в новый документ
        /// </summary>
        /// <param name="docId">ID документа</param>
        public void CopyContent(int docId)
        {
            var parameterDoc = GetItem<F_F_ParameterDoc>(docId);
            var idOfLastDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(docId).ID;
                
            GetItems<T_F_TofkList>().Where(x => x.RefParameterDoc.ID == idOfLastDoc)
                   .Each(x => Save(
                                    new T_F_TofkList
                                    {
                                        RefParameterDoc = parameterDoc,
                                        TofkName = x.TofkName,
                                        TofkAddress = x.TofkAddress
                                    }));

            GetItems<T_F_PaymentDetails>().Where(x => x.RefParameterDoc.ID == idOfLastDoc)
                    .Each(x =>
                          {
                            var record = new T_F_PaymentDetails
                            {
                                RefParameterDoc = parameterDoc,
                                TofkName = x.TofkName,
                                BankName = x.BankName,
                                BankCity = x.BankCity,
                                Bik = x.Bik,
                                CorAccountCode = x.CorAccountCode,
                                CalcAccountCode = x.CalcAccountCode,
                                RefPaymentDetailsType = x.RefPaymentDetailsType,
                                PersonalAccountCode = x.PersonalAccountCode
                            };

                            Save(record);

                            GetItems<T_F_PaymentDetailsTargets>().Where(d => d.RefPaymentDetails.ID == x.ID)
                                .Each(d => Save(new T_F_PaymentDetailsTargets
                                                {
                                                    RefPaymentDetails = record,
                                                    Kbk = d.Kbk,
                                                    PaymentType = d.PaymentType,
                                                    PaymentTargetName = d.PaymentTargetName
                                                })); 
                          });

            GetItems<T_F_LicenseDetails>().Where(x => x.RefParameterDoc.ID == idOfLastDoc)
                  .Each(x => Save(
                                   new T_F_LicenseDetails
                                   {
                                       RefParameterDoc = parameterDoc,
                                       LicenseAgencyName = x.LicenseAgencyName,
                                       LicenseDate = x.LicenseDate,
                                       LicenseExpDate = x.LicenseExpDate,
                                       LicenseName = x.LicenseName,
                                       LicenseNum = x.LicenseNum
                                   }));

            GetItems<T_F_AccreditationDetails>().Where(x => x.RefParameterDoc.ID == idOfLastDoc)
                  .Each(x => Save(
                                   new T_F_AccreditationDetails
                                   {
                                       RefParameterDoc = parameterDoc,
                                       AccreditationAgencyName = x.AccreditationAgencyName,
                                       AccreditationExpDate = x.AccreditationExpDate,
                                       AccreditationName = x.AccreditationName
                                   }));
        }
    }
}
