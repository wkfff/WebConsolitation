using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.DiverseInfo;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.DiverseInfo;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    /// <summary>
    ///   Контроллер для представления "Иная информация об учреждении"
    /// </summary>
    public sealed class DiverseInfoViewController : SchemeBoundController
    {
        private readonly IAuthService auth;

        private readonly TofkListModel tofkListModel = new TofkListModel();
        private readonly PaymentDetailsModel paymentDetailsModel = new PaymentDetailsModel();
        private readonly PaymentDetailsTargetsModel paymentDetailsTargetsModel = new PaymentDetailsTargetsModel();
        private readonly LicenseDetailsModel licenseDetailsModel = new LicenseDetailsModel();
        private readonly AccreditationDetailsModel accreditationDetailsModel = new AccreditationDetailsModel();

        private readonly IDiverseInfoService service;
        private readonly IChangeLogService logService;

        public DiverseInfoViewController()
        {
            service = Resolver.Get<IDiverseInfoService>();
            logService = Resolver.Get<IChangeLogService>();
            auth = Resolver.Get<IAuthService>();
        }

        #region TofkList

        public RestResult TofkListRead(int docId)
        {
            var data = service.GetItems<T_F_TofkList>()
                .Where(p => p.RefParameterDoc.ID == docId)
                .Select(
                    p => new TofkListModel
                        {
                            ID = p.ID,
                            RefParameterDoc = p.RefParameterDoc.ID, 
                            TofkName = p.TofkName, 
                            TofkAddress = p.TofkAddress
                        });

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult TofkListSave(string data, int docId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = TofkListValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<T_F_TofkList>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParameterDoc = service.GetItem<F_F_ParameterDoc>(docId);

                service.Save(record);
                logService.WriteChangeDocDetail(record.RefParameterDoc);

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = service.GetItems<T_F_TofkList>()
                                .Where(p => p.ID == record.ID)
                                .Select(
                                    p => new TofkListModel
                                        {
                                            ID = p.ID,
                                            RefParameterDoc = p.RefParameterDoc.ID, 
                                            TofkName = p.TofkName, 
                                            TofkAddress = p.TofkAddress
                                        })
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult TofkListDelete(int id, int docId)
        {
            return service.DeleteDocDetailAction<T_F_TofkList>(id, docId);
        }

        #endregion

        #region PaymentDetails

        public RestResult PaymentDetailsRead(int docId)
        {
            var data = service.GetItems<T_F_PaymentDetails>()
                .Where(p => p.RefParameterDoc.ID == docId)
                .Select(
                    p => new PaymentDetailsModel
                    {
                        ID = p.ID,
                        RefParameterDoc = p.RefParameterDoc.ID,
                        TofkName = p.TofkName,
                        BankName = p.BankName,
                        BankCity = p.BankCity,
                        Bik = p.Bik,
                        CorAccountCode = p.CorAccountCode,
                        CalcAccountCode = p.CalcAccountCode,
                        RefPaymentDetailsType = p.RefPaymentDetailsType.ID,
                        RefPaymentDetailsTypeName = p.RefPaymentDetailsType.Name,
                        PersonalAccountCode = p.PersonalAccountCode,
                        AccountName = p.AccountName
                    });

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult PaymentDetailsSave(string data, int docId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = PaymentDetailsValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<T_F_PaymentDetails>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParameterDoc = service.GetItem<F_F_ParameterDoc>(docId);
                record.RefPaymentDetailsType = service.GetItem<FX_FX_PaymentDetailsType>(record.RefPaymentDetailsType.ID);

                service.Save(record);
                logService.WriteChangeDocDetail(record.RefParameterDoc);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = service.GetItems<T_F_PaymentDetails>()
                            .Where(p => p.ID == record.ID)
                            .Select(
                                p => new PaymentDetailsModel
                                {
                                    ID = p.ID,
                                    RefParameterDoc = p.RefParameterDoc.ID,
                                    TofkName = p.TofkName,
                                    BankName = p.BankName,
                                    BankCity = p.BankCity,
                                    Bik = p.Bik,
                                    CorAccountCode = p.CorAccountCode,
                                    CalcAccountCode = p.CalcAccountCode,
                                    RefPaymentDetailsType = p.RefPaymentDetailsType.ID,
                                    RefPaymentDetailsTypeName = p.RefPaymentDetailsType.Name,
                                    PersonalAccountCode = p.PersonalAccountCode,
                                    AccountName = p.AccountName
                                })
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult PaymentDetailsDelete(int id, int docId)
        {
            service.BeginTransaction();
            try
            {
                service.GetItems<T_F_PaymentDetailsTargets>().Where(x => x.RefPaymentDetails.ID == id).Each(service.Delete);
                service.Delete<T_F_PaymentDetails>(id);
                logService.WriteDeleteDocDetail(service.GetItem<F_F_ParameterDoc>(docId));

                // todo считаю что не нужно в IChangeLogService работать с транзакциями!!!
                if (service.HaveTransaction)
                {
                    service.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                service.RollbackTransaction();
                return new RestResult { Success = false, Message = e.Message };
            }
        }
        
        #endregion

        #region PaymentDetailsTargets

        public RestResult PaymentDetailsTargetsRead(int parentId)
        {
            var data = service.GetItems<T_F_PaymentDetailsTargets>()
                .Where(p => p.RefPaymentDetails.ID == parentId)
                .Select(
                    p => new PaymentDetailsTargetsModel
                    {
                        ID = p.ID,
                        RefPaymentDetails = p.RefPaymentDetails.ID,
                        PaymentType = p.PaymentType,
                        PaymentTargetName = p.PaymentTargetName,
                        Kbk = p.Kbk
                    });

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult PaymentDetailsTargetsSave(string data, int parentId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = PaymentDetailsTargetsValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<T_F_PaymentDetailsTargets>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefPaymentDetails = service.GetItem<T_F_PaymentDetails>(parentId);

                service.Save(record);
                logService.WriteChangeDocDetail(record.RefPaymentDetails.RefParameterDoc);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = service.GetItems<T_F_PaymentDetailsTargets>()
                            .Where(p => p.ID == record.ID)
                            .Select(
                                p => new PaymentDetailsTargetsModel
                                {
                                    ID = p.ID,
                                    RefPaymentDetails = p.RefPaymentDetails.ID,
                                    PaymentType = p.PaymentType,
                                    PaymentTargetName = p.PaymentTargetName,
                                    Kbk = p.Kbk
                                })
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult PaymentDetailsTargetsDelete(int id, int docId)
        {
            return service.DeleteDocDetailAction<T_F_PaymentDetailsTargets>(id, docId);
        }
        
        #endregion

        #region LicenseDetails

        public RestResult LicenseDetailsRead(int docId)
        {
            var data = service.GetItems<T_F_LicenseDetails>()
                .Where(p => p.RefParameterDoc.ID == docId)
                .Select(
                    p => new LicenseDetailsModel
                    {
                        ID = p.ID,
                        RefParameterDoc = p.RefParameterDoc.ID,
                        LicenseAgencyName = p.LicenseAgencyName,
                        LicenseName = p.LicenseName,
                        LicenseNum = p.LicenseNum,
                        LicenseDate = p.LicenseDate,
                        LicenseExpDate = p.LicenseExpDate
                    });

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult LicenseDetailsSave(string data, int docId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = LicenseDetailsValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<T_F_LicenseDetails>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParameterDoc = service.GetItem<F_F_ParameterDoc>(docId);

                service.Save(record);
                logService.WriteChangeDocDetail(record.RefParameterDoc);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = service.GetItems<T_F_LicenseDetails>()
                            .Where(p => p.ID == record.ID)
                            .Select(
                                p => new LicenseDetailsModel
                                {
                                    ID = p.ID,
                                    RefParameterDoc = p.RefParameterDoc.ID,
                                    LicenseAgencyName = p.LicenseAgencyName,
                                    LicenseName = p.LicenseName,
                                    LicenseNum = p.LicenseNum,
                                    LicenseDate = p.LicenseDate,
                                    LicenseExpDate = p.LicenseExpDate
                                })
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult LicenseDetailsDelete(int id, int docId)
        {
            return service.DeleteDocDetailAction<T_F_LicenseDetails>(id, docId);
        }
        
        #endregion

        #region AccreditationDetails

        public RestResult AccreditationDetailsRead(int docId)
        {
            var data = service.GetItems<T_F_AccreditationDetails>()
                .Where(p => p.RefParameterDoc.ID == docId)
                .Select(
                    p => new AccreditationDetailsModel
                    {
                        ID = p.ID,
                        RefParameterDoc = p.RefParameterDoc.ID,
                        AccreditationAgencyName = p.AccreditationAgencyName,
                        AccreditationName = p.AccreditationName,
                        AccreditationExpDate = p.AccreditationExpDate
                    });

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult AccreditationDetailsSave(string data, int docId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = AccreditationDetailsValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<T_F_AccreditationDetails>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParameterDoc = service.GetItem<F_F_ParameterDoc>(docId);

                service.Save(record);
                logService.WriteChangeDocDetail(record.RefParameterDoc);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = service.GetItems<T_F_AccreditationDetails>()
                            .Where(p => p.ID == record.ID)
                            .Select(
                                p => new AccreditationDetailsModel
                                {
                                    ID = p.ID,
                                    RefParameterDoc = p.RefParameterDoc.ID,
                                    AccreditationAgencyName = p.AccreditationAgencyName,
                                    AccreditationName = p.AccreditationName,
                                    AccreditationExpDate = p.AccreditationExpDate
                                })
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult AccreditationDetailsDelete(int id, int docId)
        {
            return service.DeleteDocDetailAction<T_F_AccreditationDetails>(id, docId);
        }
        
        #endregion

        #region CopyDoc

        [HttpPost]
        [Transaction]
        public RestResult CheckIfCanDocumentCopy(int recId)
        {
            if (!Resolver.Get<IVersioningService>().CheckDocs(recId))
            {
                return new RestResult
                {
                    Success = false,
                    Message = "Нет закрытых документов"
                };
            }

            if (service.CheckDocEmpty(recId))
            {
                return new RestResult
                {
                    Success = true,
                    Message = "Документ пуст"
                };
            }

            return new RestResult
            {
                Success = false,
                Message = "Документ не пуст"
            };
        }

        [Transaction]
        public RestResult CopyContent(int recId)
        {
            try
            {
                service.CopyContent(recId);
                logService.WriteChangeDocDetail(service.GetItem<F_F_ParameterDoc>(recId));

                return new RestResult
                {
                    Success = true,
                    Message = "Данные скопированы"
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        #endregion

        public ActionResult ExportToXml(int recId)
        {
            return File(
                ExportDiverseInfoService.Serialize(auth, service.Load<F_F_ParameterDoc>(recId)), 
                "application/xml",
                string.Concat("diverseInfo", DateTime.Now.ToString("yyyymmddhhmmss"), ".xml"));
        }
        
        private string TofkListValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => tofkListModel.TofkName))
            {
                message.Append(Msg.FormatWith(tofkListModel.DescriptionOf(() => tofkListModel.TofkName)));
            }

            if (record.CheckNull(() => tofkListModel.TofkAddress))
            {
                message.Append(Msg.FormatWith(tofkListModel.DescriptionOf(() => tofkListModel.TofkAddress)));
            }

            return message.ToString();
        }

        private string PaymentDetailsValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => paymentDetailsModel.RefPaymentDetailsType))
            {
                message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsModel.RefPaymentDetailsTypeName)));
            }

            if (record.CheckNull(() => paymentDetailsModel.BankName))
            {
                message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsModel.BankName)));
            }

            if (record.CheckNull(() => paymentDetailsModel.Bik))
            {
                message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsModel.Bik)));
            }

            if (record.CheckNull(() => paymentDetailsModel.CalcAccountCode))
            {
                message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsModel.CalcAccountCode)));
            }

            var paymentDetailsType = record.GetValueToIntOrDefault(() => paymentDetailsModel.RefPaymentDetailsType, -1);

            if (paymentDetailsType != 0)
            {
                if (record.CheckNull(() => paymentDetailsModel.PersonalAccountCode))
                {
                    message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsModel.PersonalAccountCode)));
                }
                else
                {
                    if (paymentDetailsType == 1)
                    {
                        if (!Regex.IsMatch(record.GetValue(() => paymentDetailsModel.PersonalAccountCode), @"^(\p{L}|\d){11}$"))
                        {
                            message.Append("Значение поля \"{0}\" не соответвтует требованиям форматов данных<br>"
                                .FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsModel.PersonalAccountCode)));
                        }
                    }
                }

                if (record.CheckNull(() => paymentDetailsModel.AccountName))
                {
                    message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsModel.AccountName)));
                }
            }
            else
            {
                if (record.CheckNull(() => paymentDetailsModel.CorAccountCode))
                {
                    message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsModel.CorAccountCode)));
                }
            }

            return message.ToString();
        }

        private string PaymentDetailsTargetsValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => paymentDetailsTargetsModel.PaymentType))
            {
                message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsTargetsModel.PaymentType)));
            }

            if (record.CheckNull(() => paymentDetailsTargetsModel.PaymentTargetName))
            {
                message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsTargetsModel.PaymentTargetName)));
            }

            if (record.CheckNull(() => paymentDetailsTargetsModel.Kbk))
            {
                message.Append(Msg.FormatWith(paymentDetailsModel.DescriptionOf(() => paymentDetailsTargetsModel.Kbk)));
            }

            return message.ToString();
        }

        private string LicenseDetailsValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => licenseDetailsModel.LicenseAgencyName))
            {
                message.Append(Msg.FormatWith(tofkListModel.DescriptionOf(() => licenseDetailsModel.LicenseAgencyName)));
            }

            if (record.CheckNull(() => licenseDetailsModel.LicenseName))
            {
                message.Append(Msg.FormatWith(tofkListModel.DescriptionOf(() => licenseDetailsModel.LicenseName)));
            }

            if (record.CheckNull(() => licenseDetailsModel.LicenseNum))
            {
                message.Append(Msg.FormatWith(tofkListModel.DescriptionOf(() => licenseDetailsModel.LicenseNum)));
            }

            if (record.CheckNull(() => licenseDetailsModel.LicenseDate))
            {
                message.Append(Msg.FormatWith(tofkListModel.DescriptionOf(() => licenseDetailsModel.LicenseDate)));
            }

            return message.ToString();
        }

        private string AccreditationDetailsValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = new StringBuilder(string.Empty);

            if (record.CheckNull(() => accreditationDetailsModel.AccreditationAgencyName))
            {
                message.Append(Msg.FormatWith(accreditationDetailsModel.DescriptionOf(() => accreditationDetailsModel.AccreditationAgencyName)));
            }

            if (record.CheckNull(() => accreditationDetailsModel.AccreditationName))
            {
                message.Append(Msg.FormatWith(accreditationDetailsModel.DescriptionOf(() => accreditationDetailsModel.AccreditationName)));
            }

            if (record.CheckNull(() => accreditationDetailsModel.AccreditationExpDate))
            {
                message.Append(Msg.FormatWith(accreditationDetailsModel.DescriptionOf(() => accreditationDetailsModel.AccreditationExpDate)));
            }

            return message.ToString();
        }
    }
}
