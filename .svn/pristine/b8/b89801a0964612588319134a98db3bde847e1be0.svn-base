using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.Diagnostics;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для представления "Бухгалтерская отчетность"
    /// </summary>
    public class AnnualBalanceViewController : SchemeBoundController
    {
        private readonly IAnnualBalanceService annualBalanceService;
        private readonly IAuthService auth;
        private readonly IChangeLogService logService;

        private readonly AnnualBalanceF0503721ViewModel annualBalanceF0503721ViewModel = new AnnualBalanceF0503721ViewModel();

        public AnnualBalanceViewController()
        {
            annualBalanceService = Resolver.Get<IAnnualBalanceService>();
            auth = Resolver.Get<IAuthService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        #region HeadAttribute

        public ActionResult HeadAttributeRead(int docId)
        {
            var data = from p in annualBalanceService.GetItems<F_Report_HeadAttribute>()
                       where p.RefParametr.ID == docId
                       select new
                                  {
                                      HeadAttributeID = p.ID,
                                      Datedata = p.Datedata.Date.ToString("dd.MM.yyyy"),
                                      RefParametr = p.RefParametr.ID,
                                      RefPeriodic = p.RefPeriodic.ID,
                                      RefPeriodicName = p.RefPeriodic.Name,
                                      FounderAuthorityOkpo = p.founderAuthorityOkpo
                                  };

            return new AjaxStoreResult(data, 1);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult HeadAttributeSave(FormCollection values, int docId)
        {
            var result = new AjaxFormResult();

            try
            {
                var fieldID = values[HeadAttributeFields.HeadAttributeID.ToString()];
                F_Report_HeadAttribute record;
                if (string.IsNullOrEmpty(fieldID))
                {
                    record = new F_Report_HeadAttribute
                    {
                        ID = 0,
                        TaskID = 0,
                        SourceID = 0,
                        RefParametr = annualBalanceService.GetItem<F_F_ParameterDoc>(docId)
                    };
                }
                else
                {
                    record = annualBalanceService.GetItem<F_Report_HeadAttribute>(Convert.ToInt32(fieldID));
                }

                var date = Convert.ToDateTime(values[HeadAttributeFields.Datedata.ToString()]);
                if (date > DateTime.Now)
                {
                    throw new InvalidDataException(
                        "\"{0}\" должна быть меньше или равна текущей дате".FormatWith(AnnualBalanceHelpers.HeadAttributeFieldsNameMapping(HeadAttributeFields.Datedata)));
                }

                record.Datedata = date;
                
                record.founderAuthorityOkpo = Convert.ToString(values[HeadAttributeFields.FounderAuthorityOkpo.ToString()]);

                if (values[HeadAttributeFields.RefPeriodic + "_SelIndex"] != "-1")
                {
                    record.RefPeriodic = annualBalanceService.GetItem<FX_FX_Periodic>(Convert.ToInt32(values[HeadAttributeFields.RefPeriodic + "_Value"]));
                }

                annualBalanceService.Save(record);
                logService.WriteChangeDocDetail(record.RefParametr);

                result.Success = true;
                result.ExtraParams["msg"] = "Сохранено";
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }

        #endregion

        #region NonfinancialAssets

        public RestResult NonfinancialAssetsRead(int docId, int section)
        {
            return annualBalanceService.F0503730Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult NonfinancialAssetsSave(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_Report_BalF0503730>(docId).RefParametr);
            return annualBalanceService.F0503730Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult NonfinancialAssetsDelete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_BalF0503730>(id, docId);
        }

        #endregion

        #region FinancialAssets

        public RestResult FinancialAssetsRead(int docId, int section)
        {
            return annualBalanceService.F0503730Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult FinancialAssetsSave(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_Report_BalF0503730>(docId).RefParametr);
            return annualBalanceService.F0503730Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult FinancialAssetsDelete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_BalF0503730>(id, docId);
        }

        #endregion

        #region Liabilities

        public RestResult LiabilitiesRead(int docId, int section)
        {
            return annualBalanceService.F0503730Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult LiabilitiesSave(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_Report_BalF0503730>(docId).RefParametr);
            return annualBalanceService.F0503730Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult LiabilitiesDelete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_BalF0503730>(id, docId);
        }

        #endregion

        #region FinancialResult

        public RestResult FinancialResultRead(int docId, int section)
        {
            return annualBalanceService.F0503730Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult FinancialResultSave(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_Report_BalF0503730>(docId).RefParametr);
            return annualBalanceService.F0503730Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult FinancialResultDelete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_BalF0503730>(id, docId);
        }

        #endregion

        #region Information

        public RestResult InformationRead(int docId, int section)
        {
            return annualBalanceService.F0503730Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult InformationSave(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_Report_BalF0503730>(docId).RefParametr);
            return annualBalanceService.F0503730Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult InformationDelete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_BalF0503730>(id, docId);
        }

        #endregion

        #region F0503121

        public RestResult F0503121Read(int docId, int section)
        {
            return annualBalanceService.F0503121Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult F0503121Save(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_F_ParameterDoc>(docId));
            return annualBalanceService.F0503121Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult F0503121Delete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_Bal0503121>(id, docId);
        }

        #endregion

        #region F0503127

        public RestResult F0503127Read(int docId, int section)
        {
            return annualBalanceService.F0503127Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult F0503127Save(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_F_ParameterDoc>(docId));
            return annualBalanceService.F0503127Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult F0503127Delete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_Bal0503127>(id, docId);
        }

        #endregion

        #region F0503130

        public RestResult F0503130Read(int docId, int section)
        {
            return annualBalanceService.F0503130Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult F0503130Save(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_F_ParameterDoc>(docId));
            return annualBalanceService.F0503130Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult F0503130Delete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_BalF0503130>(id, docId);
        }

        #endregion

        #region F0503137

        public RestResult F0503137Read(int docId, int section)
        {
            return annualBalanceService.F0503137Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult F0503137Save(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_F_ParameterDoc>(docId));
            return annualBalanceService.F0503137Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult F0503137Delete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_BalF0503137>(id, docId);
        }

        #endregion

        #region F0503721
        
        public RestResult F0503721Read(int docId)
        {
            try
            {
                annualBalanceService.СheckDocContent(docId);
                return new RestResult { Success = true, Data = annualBalanceF0503721ViewModel.GetModelData(Request.QueryString) };
            }
            catch (Exception e)
            {
                Trace.TraceError("ReadAction: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        #endregion

        #region F0503737

        public RestResult F0503737Read(int docId, int section)
        {
            return annualBalanceService.F0503737Read(docId, section);
        }

        [HttpPost]
        [Transaction]
        public RestResult F0503737Save(string data, int docId, int section)
        {
            logService.WriteChangeDocDetail(annualBalanceService.GetItem<F_F_ParameterDoc>(docId));
            return annualBalanceService.F0503737Save(data, docId, section);
        }

        [HttpDelete]
        public virtual RestResult F0503737Delete(int id, int docId)
        {
            return annualBalanceService.DeleteDocDetailAction<F_Report_BalF0503737>(id, docId);
        }

        #endregion

        #region Indicators

        public RestResult IndicatorsRead()
        {
            var data = annualBalanceService.GetItems<D_Line_Indicators>();

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult IndicatorsSave(string data)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = IndicatorsValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<D_Line_Indicators>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                annualBalanceService.Save(record);

                return new RestResult
                           {
                               Success = true,
                               Message = msg,
                               Data = annualBalanceService.GetItem<D_Line_Indicators>(record.ID)
                           };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult IndicatorsDelete(int id)
        {
            return annualBalanceService.DeleteAction<D_Line_Indicators>(id);
        }

        public string IndicatorsValidateData(JsonObject record)
        {
            const string Msg = "Показатель с кодом {0} уже заведен<br>";

            var message = string.Empty;

            var code = Convert.ToInt32(record[IndicatorsFields.Code.ToString()]);

            if (annualBalanceService.GetItems<D_Line_Indicators>().Any(x => x.Code == code))
            {
                message += Msg.FormatWith(code);
            }
            
            return message;
        }

        #endregion

        #region Settings

        public RestResult SettingsRead()
        {
            var data = from p in annualBalanceService.GetItems<D_Marks_ItfSettings>()
                       select new
                       {
                           p.ID,
                           RefPartDoc = p.RefPartDoc.ID,
                           RefPartDocName = p.RefPartDoc.Name,
                           p.Section,
                           SectionName = GetDetailsNameByPartDoc(p.RefPartDoc.ID, p.Section),
                           p.RefIndicators.LineCode,
                           RefIndicators = p.RefIndicators.ID,
                           RefIndicatorsName = p.RefIndicators.LineCode + ";" + p.RefIndicators.Name,
                           p.Additional,
                           p.StartYear,
                           p.EndYear
                       };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult SettingsSave(string data)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = SettingsValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<D_Marks_ItfSettings>.DeserializeSingle(data);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefPartDoc = annualBalanceService.GetItem<FX_FX_PartDoc>(record.RefPartDoc.ID);
                record.RefIndicators = annualBalanceService.GetItem<D_Line_Indicators>(record.RefIndicators.ID);

                annualBalanceService.Save(record);

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = from p in annualBalanceService.GetItems<D_Marks_ItfSettings>()
                           where p.ID == record.ID
                           select new
                           {
                               p.ID,
                               RefPartDoc = p.RefPartDoc.ID,
                               RefPartDocName = p.RefPartDoc.Name,
                               p.Section,
                               SectionName = GetDetailsNameByPartDoc(p.RefPartDoc.ID, p.Section),
                               p.RefIndicators.LineCode,
                               RefIndicators = p.RefIndicators.ID,
                               RefIndicatorsName = p.RefIndicators.LineCode + ";" + p.RefIndicators.Name,
                               p.Additional
                           }
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult SettingsDelete(int id)
        {
            return annualBalanceService.DeleteAction<D_Marks_ItfSettings>(id);
        }

        public string SettingsValidateData(JsonObject record)
        {
            const string Msg = "Показатель с кодом строки {0} для детализации {1}, документа {2} уже заведен<br>";

            var message = string.Empty;

            var id = Convert.ToInt32(record[SettingsFields.ID.ToString()]);
            var code = Convert.ToInt32(record[SettingsFields.RefIndicators.ToString()]);
            var partdoc = Convert.ToInt32(record[SettingsFields.RefPartDoc.ToString()]);
            var section = Convert.ToInt32(record[SettingsFields.Section.ToString()]);

            if (annualBalanceService.GetItems<D_Marks_ItfSettings>().Any(x => x.RefIndicators.ID == code
                                                                         && x.RefPartDoc.ID == partdoc
                                                                         && x.Section == section
                                                                         && x.ID != id))
            {
                message += Msg.FormatWith(
                    annualBalanceService.GetItems<D_Line_Indicators>().Single(x => x.Code == code).LineCode,
                    GetDetailsNameByPartDoc(partdoc, section),
                    annualBalanceService.GetItem<FX_FX_PartDoc>(partdoc).Name);
            }

            return message;
        }

        public string GetDetailsNameByPartDoc(int partDoc, int section)
        {
            switch (partDoc)
            {
                case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                    return AnnualBalanceHelpers.F0503130F0503730DetailsNameMapping((F0503130F0503730Details)section);

                case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                    return AnnualBalanceHelpers.F0503130F0503730DetailsNameMapping((F0503130F0503730Details)section);

                case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                    return AnnualBalanceHelpers.F0503121DetailsNameMapping((F0503121Details)section);

                case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                    return AnnualBalanceHelpers.F0503127DetailsNameMapping((F0503127Details)section);

                case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                    return AnnualBalanceHelpers.F0503137DetailsNameMapping((F0503137Details)section);

                case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                    return AnnualBalanceHelpers.F0503737DetailsNameMapping((F0503737Details)section);

                case FX_FX_PartDoc.AnnualBalanceF0503721Type:
                    return AnnualBalanceHelpers.F0503721DetailsNameMapping((F0503721Details)section);
            }

            return string.Empty;
        }

        public RestResult SettingsGetDetails(int partDoc)
        {
            switch (partDoc)
            {
                case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                    return new RestResult
                    {
                        Success = true,
                        Data = (from int datail in Enum.GetValues(typeof(F0503130F0503730Details))
                                select new
                                {
                                    ID = datail,
                                    Name = AnnualBalanceHelpers.F0503130F0503730DetailsNameMapping((F0503130F0503730Details)datail)
                                }).ToList()
                    };

                case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                    return new RestResult
                    {
                        Success = true,
                        Data = (from int datail in Enum.GetValues(typeof(F0503130F0503730Details))
                                select new
                                {
                                    ID = datail,
                                    Name = AnnualBalanceHelpers.F0503130F0503730DetailsNameMapping((F0503130F0503730Details)datail)
                                }).ToList()
                    };

                case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                    return new RestResult
                    {
                        Success = true,
                        Data = (from int datail in Enum.GetValues(typeof(F0503121Details))
                                select new
                                {
                                    ID = datail,
                                    Name = AnnualBalanceHelpers.F0503121DetailsNameMapping((F0503121Details)datail)
                                }).ToList()
                    };

                case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                    return new RestResult
                    {
                        Success = true,
                        Data = (from int datail in Enum.GetValues(typeof(F0503127Details))
                                select new
                                {
                                    ID = datail,
                                    Name = AnnualBalanceHelpers.F0503127DetailsNameMapping((F0503127Details)datail)
                                }).ToList()
                    };

                case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                    return new RestResult
                    {
                        Success = true,
                        Data = (from int datail in Enum.GetValues(typeof(F0503137Details))
                                select new
                                {
                                    ID = datail,
                                    Name = AnnualBalanceHelpers.F0503137DetailsNameMapping((F0503137Details)datail)
                                }).ToList()
                    };

                case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                    return new RestResult
                    {
                        Success = true,
                        Data = (from int datail in Enum.GetValues(typeof(F0503737Details))
                                select new
                                {
                                    ID = datail,
                                    Name = AnnualBalanceHelpers.F0503737DetailsNameMapping((F0503737Details)datail)
                                }).ToList()
                    };

                case FX_FX_PartDoc.AnnualBalanceF0503721Type:
                    return new RestResult
                    {
                        Success = true,
                        Data = (from int datail in Enum.GetValues(typeof(F0503721Details))
                                select new
                                {
                                    ID = datail,
                                    Name = AnnualBalanceHelpers.F0503721DetailsNameMapping((F0503721Details)datail)
                                }).ToList()
                    };
            }

            return new RestResult
            {
                Success = true,
                Data = new List<object> { new { ID = -1, Name = string.Empty } }
            };
        }

        #endregion

        public ActionResult ExportToXml(int docId)
        {
            var doc = annualBalanceService.Load<F_F_ParameterDoc>(docId);
            var typeDoc = doc.RefPartDoc.ID;

            switch (typeDoc)
            {
                case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                    {
                        return File(
                            ExportAnnualBalanceF0503121Service.Serialize(auth, doc),
                            "application/xml",
                            "AnnualBalanceF0503121_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xml");
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                    {
                        return File(
                            ExportAnnualBalanceF0503127Service.Serialize(auth, doc),
                            "application/xml",
                            "AnnualBalanceF0503127_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xml");
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                    {
                        return File(
                            ExportAnnualBalanceF0503130Service.Serialize(auth, doc),
                            "application/xml",
                            "AnnualBalanceF0503130_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml");
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                    {
                        return File(
                            ExportAnnualBalanceF0503137Service.Serialize(auth, doc),
                            "application/xml",
                            "AnnualBalanceF0503137_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml");
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503721Type:
                    {
                        return File(
                            ExportAnnualBalanceF0503721Service.Serialize(auth, doc),
                            "application/xml",
                            "AnnualBalanceF0503721_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml");
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                    {
                        return File(
                            ExportAnnualBalanceF0503730Service.Serialize(auth, doc),
                            "application/xml",
                            "AnnualBalanceF0503730_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml");
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                    {
                        return File(
                            ExportAnnualBalanceF0503737Service.Serialize(auth, doc),
                            MediaTypeNames.Text.Xml,
                            "AnnualBalanceF0503737_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml");
                    }
            }

            var result = new AjaxFormResult { Success = false };

            result.ExtraParams["responseText"] = "Не реализовано";
            result.IsUpload = true;
            return result;
        }

        [Transaction]
        public ActionResult CalculateSumm(int docId, int section)
        {
            try
            {
                annualBalanceService.CalculateSumm(docId, section);

                return new RestResult
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new RestResult
                {
                    Success = false,
                    Message = e.Message
                };
            }
        }

        [HttpPost]
        [Transaction]
        public RestResult CheckIfCanDocumentCopy(int recId)
        {
            if (!Resolver.Get<IVersioningService>().CheckDocs(recId))
            {
                return new RestResult
                {
                    Success = false,
                    Message = "Нет документов"
                };
            }

            var paramDoc = annualBalanceService.GetItem<F_F_ParameterDoc>(recId);
            var detal = paramDoc.ReportHeadAttribute.Count == 0;

            switch (paramDoc.RefPartDoc.ID)
            {
                case 4:
                    detal = detal && !paramDoc.AnnualBalanceF0503137.Any(
                                p =>
                                p.approveEstimateAssign != 0 || 
                                p.execFinancAuthorities != 0 || 
                                p.execBankAccounts != 0 || 
                                p.execNonCashOperation != 0 || 
                                p.execTotal != 0 || 
                                p.unexecAssignments != 0);
                    break;
                case 9:
                    detal = detal && !paramDoc.AnnualBalanceF0503730.Any(
                                p =>
                                p.servicesBegin.GetValueOrDefault() != 0 ||
                                p.servicesEnd.GetValueOrDefault() != 0 ||
                                p.targetFundsBegin != 0 ||
                                p.targetFundsEnd != 0 ||
                                p.temporaryFundsBegin.GetValueOrDefault() != 0 ||
                                p.temporaryFundsEnd.GetValueOrDefault() != 0 ||
                                p.totalEndYear != 0 ||
                                p.totalStartYear != 0);
                    break;
                case 10:
                    detal = detal && !paramDoc.AnnualBalanceF0503721.Any(
                                p =>
                                p.services.GetValueOrDefault() != 0 ||
                                p.targetFunds != 0 ||
                                p.temporaryFunds.GetValueOrDefault() != 0 ||
                                p.total != 0);
                    break;
                case 11:
                    detal = detal && !paramDoc.AnnualBalanceF0503737.Any(
                                p =>
                                p.approvePlanAssign != 0 ||
                                p.execBankAccounts != 0 ||
                                p.execCashAgency != 0 ||
                                p.execNonCashOperation != 0 ||
                                p.execPersonAuthorities != 0 ||
                                p.execTotal != 0 ||
                                p.unexecPlanAssign != 0);
                    break;
                case 12:
                    detal = detal && !paramDoc.AnnualBalanceF0503130.Any(
                                p =>
                                p.availableMeansBegin != 0 ||
                                p.availableMeansEnd != 0 ||
                                p.budgetActivityBegin != 0 ||
                                p.budgetActivityEnd != 0 ||
                                p.incomeActivityBegin != 0 ||
                                p.incomeActivityEnd != 0 ||
                                p.totalBegin != 0 ||
                                p.totalEnd != 0);
                    break;
                case 13:
                    detal = detal && !paramDoc.AnnualBalanceF0503121.Any(
                                p =>
                                p.availableMeans != 0 ||
                                p.budgetActivity != 0 ||
                                p.incomeActivity != 0 ||
                                p.total != 0);
                    break;
                case 14:
                    detal = detal && !paramDoc.AnnualBalanceF0503127.Any(
                                p =>
                                p.ApproveBudgAssign != 0 ||
                                p.budgObligatLimits != 0 ||
                                p.execBankAccounts != 0 ||
                                p.execFinAuthorities != 0 ||
                                p.execNonCashOperation != 0 ||
                                p.execTotal != 0 ||
                                p.unexecAssignments != 0 ||
                                p.unexecBudgObligatLimit != 0);
                    break;
            }

            if (!detal)
            {
                return new RestResult
                {
                    Success = false,
                    Message = "Документ не пуст"
                };
            }

            return new RestResult
            {
                Success = true,
                Message = "Документ пуст"
            };
        }

        [HttpPost]
        [Transaction]
        public RestResult CopyContent(int recId)
        {
            try
            {
                var newParamDoc = annualBalanceService.GetItem<F_F_ParameterDoc>(recId);
                var oldParamDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(recId);

                if (oldParamDoc.ReportHeadAttribute.Count > 0)
                {
                    var hed = oldParamDoc.ReportHeadAttribute.First();
                    annualBalanceService.Save(
                        new F_Report_HeadAttribute
                            {
                                Datedata = hed.Datedata,
                                RefParametr = newParamDoc,
                                RefPeriodic = hed.RefPeriodic,
                                founderAuthorityOkpo = hed.founderAuthorityOkpo
                            });
                }

                switch (oldParamDoc.RefPartDoc.ID)
                {
                    case 4:
                        annualBalanceService.GetItems<F_Report_BalF0503137>().Where(p => p.RefParametr == newParamDoc).Each(p => annualBalanceService.Delete<F_Report_BalF0503137>(p.ID));
                        oldParamDoc.AnnualBalanceF0503137.Each(
                            p => annualBalanceService.Save(
                                new F_Report_BalF0503137
                                    {
                                        Name = p.Name,
                                        RefParametr = newParamDoc,
                                        Section = p.Section,
                                        SourceID = p.SourceID,
                                        TaskID = p.TaskID,
                                        approveEstimateAssign = p.approveEstimateAssign,
                                        budgClassifCode = p.budgClassifCode,
                                        execBankAccounts = p.execBankAccounts,
                                        execFinancAuthorities = p.execFinancAuthorities,
                                        execNonCashOperation = p.execNonCashOperation,
                                        execTotal = p.execTotal,
                                        lineCode = p.lineCode,
                                        unexecAssignments = p.unexecAssignments
                                    }));
                        break;
                    case 9:
                        annualBalanceService.GetItems<F_Report_BalF0503730>().Where(p => p.RefParametr == newParamDoc).Each(p => annualBalanceService.Delete<F_Report_BalF0503730>(p.ID));
                        oldParamDoc.AnnualBalanceF0503730.Each(
                            p => annualBalanceService.Save(
                                new F_Report_BalF0503730
                                    {
                                        Name = p.Name,
                                        RefParametr = newParamDoc,
                                        Section = p.Section,
                                        SourceID = p.SourceID,
                                        TaskID = p.TaskID,
                                        lineCode = p.lineCode,
                                        servicesBegin = p.servicesBegin,
                                        servicesEnd = p.servicesEnd,
                                        targetFundsBegin = p.targetFundsBegin,
                                        targetFundsEnd = p.targetFundsEnd,
                                        temporaryFundsBegin = p.temporaryFundsBegin,
                                        temporaryFundsEnd = p.temporaryFundsEnd,
                                        totalEndYear = p.totalEndYear,
                                        totalStartYear = p.totalStartYear
                                    }));
                        break;
                    case 10:
                        annualBalanceService.GetItems<F_Report_BalF0503721>().Where(p => p.RefParametr == newParamDoc).Each(p => annualBalanceService.Delete<F_Report_BalF0503721>(p.ID));
                        oldParamDoc.AnnualBalanceF0503721.Each(
                            p => annualBalanceService.Save(
                                new F_Report_BalF0503721
                                    {
                                        Name = p.Name,
                                        RefParametr = newParamDoc,
                                        Section = p.Section,
                                        SourceID = p.SourceID,
                                        TaskID = p.TaskID,
                                        lineCode = p.lineCode,
                                        analyticCode = p.analyticCode,
                                        services = p.services,
                                        targetFunds = p.targetFunds,
                                        temporaryFunds = p.temporaryFunds,
                                        total = p.total
                                    }));
                        break;
                    case 11:
                        annualBalanceService.GetItems<F_Report_BalF0503737>().Where(p => p.RefParametr == newParamDoc).Each(p => annualBalanceService.Delete<F_Report_BalF0503737>(p.ID));
                        oldParamDoc.AnnualBalanceF0503737.Each(
                            p => annualBalanceService.Save(
                                new F_Report_BalF0503737
                                    {
                                        Name = p.Name,
                                        RefParametr = newParamDoc,
                                        Section = p.Section,
                                        SourceID = p.SourceID,
                                        TaskID = p.TaskID,
                                        execBankAccounts = p.execBankAccounts,
                                        execNonCashOperation = p.execNonCashOperation,
                                        execTotal = p.execTotal,
                                        lineCode = p.lineCode,
                                        RefTypeFinSupport = p.RefTypeFinSupport,
                                        analyticCode = p.analyticCode,
                                        approvePlanAssign = p.approvePlanAssign,
                                        execCashAgency = p.execCashAgency,
                                        execPersonAuthorities = p.execPersonAuthorities,
                                        unexecPlanAssign = p.unexecPlanAssign
                                    }));
                        break;
                    case 12:
                        annualBalanceService.GetItems<F_Report_BalF0503130>().Where(p => p.RefParametr == newParamDoc).Each(p => annualBalanceService.Delete<F_Report_BalF0503130>(p.ID));
                        oldParamDoc.AnnualBalanceF0503130.Each(
                            p => annualBalanceService.Save(
                                new F_Report_BalF0503130
                                    {
                                        Name = p.Name,
                                        RefParametr = newParamDoc,
                                        Section = p.Section,
                                        SourceID = p.SourceID,
                                        TaskID = p.TaskID,
                                        lineCode = p.lineCode,
                                        availableMeansBegin = p.availableMeansBegin,
                                        availableMeansEnd = p.availableMeansEnd,
                                        budgetActivityBegin = p.budgetActivityBegin,
                                        budgetActivityEnd = p.budgetActivityEnd,
                                        incomeActivityBegin = p.incomeActivityBegin,
                                        incomeActivityEnd = p.incomeActivityEnd,
                                        totalBegin = p.totalBegin,
                                        totalEnd = p.totalEnd
                                    }));
                        break;
                    case 13:
                        annualBalanceService.GetItems<F_Report_Bal0503121>().Where(p => p.RefParametr == newParamDoc).Each(p => annualBalanceService.Delete<F_Report_Bal0503121>(p.ID));
                        oldParamDoc.AnnualBalanceF0503121.Each(
                            p => annualBalanceService.Save(
                                new F_Report_Bal0503121
                                    {
                                        Name = p.Name,
                                        RefParametr = newParamDoc,
                                        Section = p.Section,
                                        SourceID = p.SourceID,
                                        TaskID = p.TaskID,
                                        lineCode = p.lineCode,
                                        RefKosgy = p.RefKosgy,
                                        availableMeans = p.availableMeans,
                                        budgetActivity = p.budgetActivity,
                                        incomeActivity = p.incomeActivity,
                                        total = p.total
                                    }));
                        break;
                    case 14:
                        annualBalanceService.GetItems<F_Report_Bal0503127>().Where(p => p.RefParametr == newParamDoc).Each(p => annualBalanceService.Delete<F_Report_Bal0503127>(p.ID));
                        oldParamDoc.AnnualBalanceF0503127.Each(
                            p => annualBalanceService.Save(
                                new F_Report_Bal0503127
                                    {
                                        Name = p.Name,
                                        RefParametr = newParamDoc,
                                        Section = p.Section,
                                        SourceID = p.SourceID,
                                        TaskID = p.TaskID,
                                        budgClassifCode = p.budgClassifCode,
                                        execBankAccounts = p.execBankAccounts,
                                        execNonCashOperation = p.execNonCashOperation,
                                        execTotal = p.execTotal,
                                        lineCode = p.lineCode,
                                        unexecAssignments = p.unexecAssignments,
                                        ApproveBudgAssign = p.ApproveBudgAssign,
                                        budgObligatLimits = p.budgObligatLimits,
                                        execFinAuthorities = p.execFinAuthorities,
                                        unexecBudgObligatLimit = p.unexecBudgObligatLimit
                                    }));
                        break;
                }

                logService.WriteChangeDocDetail(newParamDoc);

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
    }
}
