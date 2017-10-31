using System;
using System.Linq;
using System.Text;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateSysyem
{
    public class StateTaskStateSysController : StateSysBaseController
    {
        /// <summary>
        ///   Экшен выполнения перехода на рассмотрение
        /// </summary>
        public virtual AjaxFormResult TransitionWithValidate(int docId, int transitionID)
        {
            try
            {
                string msg;
                if (DataValidations(docId, out msg))
                {
                    return DefaultTransition(docId, transitionID);
                }

                return GetScriptResult(GetShowMessageScript(msg));
            }
            catch (Exception e)
            {
                return GetScriptResult(GetShowMessageScript(e.Message));
            }
        }

        /// <summary>
        ///   Экшен выполнения перехода на завершение
        /// </summary>
        public virtual AjaxFormResult FinishWithValidate(int docId, int transitionID)
        {
            try
            {
                string msg;
                if (DataValidations(docId, out msg))
                {
                    var result = DefaultTransition(docId, transitionID);
                    
                    if (GetNotBringState(docId))
                    {
                        // закрываем документ если галочка(Не доводить ГЗ) стоит
                        VersioningService.CloseDocument(docId);
                        
                        var resultScript = new StringBuilder();

                        // закрываем документ
                        resultScript.AppendLine(StateSysScope + ".SetCloseDoc({0}, {1});".FormatWith(docId, Auth.IsAdmin().ToString().ToLower()));
                        resultScript.AppendLine("dsParamDoc{0}.reload();".FormatWith(docId));

                        result.Script = resultScript.ToString();
                    }

                    return result;
                }

                return GetScriptResult(GetShowMessageScript(msg));
            }
            catch (Exception e)
            {
                return GetScriptResult(GetShowMessageScript(e.Message));
            }
        }

        /// <summary>
        ///   настраиваем вместе с кнопками и редактируемость интерфейса
        ///   в зависимости от состояния и пользователя
        /// </summary>
        public override StringBuilder InitComponents(int docId)
        {
            StringBuilder resultScript = base.InitComponents(docId);

            if (Auth.IsSpectator())
            {
                // для наблюдателя только чтение
                resultScript.Append(ReadOnlyScript(true, docId));
                resultScript.Append(DisableNotBringCheckBox(true, docId));
                
                return resultScript;
            }

            if (!Auth.IsAdmin())
            {
                var stateCode = StateSystemService.GetCurrentStateID(docId);

                if (Auth.IsPpoUser() && !Auth.IsGrbsUser())
                {
                    // если ФО то только чтение
                    resultScript.Append(ReadOnlyScript(true, docId));
                    resultScript.Append(DisableNotBringCheckBox(true, docId));
                }
                else
                {
                    // если ГРБС или учреждение
                    switch (stateCode)
                    {
                        case (int)StatesType.UnderConsideration:
                            resultScript.Append(Auth.IsGrbsUser() ? ReadOnlyScript(false, docId) : ReadOnlyScript(true, docId));
                            resultScript.Append(DisableNotBringCheckBox(true, docId));
                            break;
                        case (int)StatesType.Completed:
                            resultScript.Append(ReadOnlyScript(true, docId));
                            resultScript.Append(DisableNotBringCheckBox(true, docId));
                            break;
                        case (int)StatesType.Exported:
                            resultScript.Append(ReadOnlyScript(true, docId));
                            resultScript.Append(DisableNotBringCheckBox(true, docId));
                            break;
                        default:
                            resultScript.Append(ReadOnlyScript(false, docId));
                            resultScript.Append(DisableNotBringCheckBox(false, docId));
                            break;
                    }
                }
            }
            else
            {
                resultScript.Append(ReadOnlyScript(false, docId));
                resultScript.Append(DisableNotBringCheckBox(VersioningService.GetCloseState(docId), docId));
            }

            return resultScript;
        }

        /// <summary>
        ///   Валидация данных
        /// </summary>
        /// <returns> true - если валидация прошла </returns>
        private bool DataValidations(int docId, out string message)
        {
            if (GetNotBringState(docId))
            {
                message = string.Empty;
                return true;
            }

            if (StateSystemService.GetItem<F_F_ParameterDoc>(docId).RefYearForm.ID < 2016)
            {
                return Validate(docId, out message);
            }

            return Validate2016(docId, out message);
        }

        private bool Validate(int docId, out string message)
        {
            const string Msg = "Нет данных {0}.<p/>";
            const string Msg1 = "Нет показателя {0} для услуги \"{1}\".<p/>";
            const string Msg2 = "Не задано поле \"{0}\" в НПА для услуги \"{1}\".<p/>";
            const string Msg3 = "Не прикреплен документ типа \"{0}\".<p/>";
            const string Msg4 = "Не задана \"Средневзвешенная цена за единицу услуги\" для услуги \"{0}\".<p/>";
            const string Msg5 = "Отсутствуют категории потребителей для услуги \"{0}\". Обратитесь к {1} <br>";
            const string Msg6 = "У услуги \"{0}\" не указана платность. Обратитесь к {1} <br>";
            const string Msg7 = "\"Средневзвешенная цена за единицу услуги\" для бесплатной услуги(работы)" +
                                " \"{0}\" не должна иметь значения.<br>";
            const string Msg8 = "Не задано поле \"Очередной год\" в показателях оказания услуг для услуги" +
                                " \"{0}\" <br>";
            const string Msg9 = "Фактическое значение отличается от планового.Необходимо заполнить поле \"{0}\" для показателя \"{1}\" услуги \"{2}\".<br>";
            const string Msg10 = "Поле \"Причина отклонения\" не должно быть заполнено для показателя \"{0}\" услуги \"{1}\".<br>";

            message = string.Empty;

            var docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                       where (p.RefParametr.ID == docId) &&
                             (p.Url != "НетФайла") &&
                             (p.RefTypeDoc.Code == "C")
                       select new { p.ID };

            if (!docs.Any())
            {
                message += Msg3.FormatWith(GetNameTypeDoc("C"));
            }

            IQueryable<F_F_GosZadanie> gosZadanie =
                StateSystemService.GetItems<F_F_GosZadanie>().Where(p => (p.RefParametr.ID == docId));

            if (!gosZadanie.Any())
            {
                message += Msg.FormatWith("по услугам");
            }
            else
            {
                foreach (F_F_GosZadanie rec in gosZadanie)
                {
                    F_F_GosZadanie rec2 = rec;
                    IQueryable<F_F_PNRZnach> indicators = StateSystemService
                        .GetItems<F_F_PNRZnach>().Where(p => (p.RefFactGZ.ID == rec2.ID));

                    // проверяем Причину отклонения и соответствие плана факту у показателей
                    foreach (F_F_PNRZnach item in indicators)
                    {
                        // если фактическое значение указано
                        if (!(item.ActualValue.IsNullOrEmpty() || item.ActualValue.Trim().IsNullOrEmpty()))
                        {
                            // фактическое значение отличное от очередного года
                            if (item.ActualValue.Trim() != item.ComingYear.Trim())
                            {
                                // если Причина отклонения не заполнено
                                if (item.Protklp.IsNullOrEmpty() || item.Protklp.Trim().IsNullOrEmpty())
                                {
                                    message += Msg9.FormatWith("Причина отклонения", item.RefIndicators.Name, rec2.RefVedPch.Name);
                                }
                            }
                            else
                            {
                                // если фактическое значение указано и оно идентично очередному году
                                // если Причина отклонения заполнено
                                if (!(item.Protklp.IsNullOrEmpty() || item.Protklp.Trim().IsNullOrEmpty()))
                                {
                                    message += Msg10.FormatWith(item.RefIndicators.Name, rec2.RefVedPch.Name);
                                }
                            }
                        }
                        else
                        {
                            // если фактическое значение не указано
                            // если Причина отклонения заполнена
                            if (!(item.Protklp.IsNullOrEmpty() || item.Protklp.Trim().IsNullOrEmpty()))
                            {
                                message += Msg10.FormatWith(item.RefIndicators.Name, rec2.RefVedPch.Name);
                            }
                        }
                    }

                    // проверять только для услуг и работ
                    // для бесплатных работ услуг
                    if (rec.RefVedPch.RefPl.Code == 1
                        && rec.CenaEd != null && rec.CenaEd != 0)
                    {
                        message += Msg7.FormatWith(rec.RefVedPch.Name);
                    }

                    // проверять только для услуг
                    if (rec.RefVedPch.RefTipY.ID == 1)
                    {
                        string ppo = rec.RefParametr.RefUchr.RefOrgPPO.Code;

                        F_F_GosZadanie rec1 = rec;
                        IQueryable<F_F_PotrYs> consumers =
                            StateSystemService.GetItems<F_F_PotrYs>().Where(p => (p.RefVedPP.ID == rec1.RefVedPch.ID));

                        if (!consumers.Any())
                        {
                            switch (ppo)
                            {
                                case "78000000000":
                                    message += Msg5.FormatWith(rec.RefVedPch.Name, "ГРБС");
                                    break;
                                default:
                                    message += Msg5.FormatWith(rec.RefVedPch.Name, "ФО");
                                    break;
                            }
                        }

                        if (!indicators.Any())
                        {
                            message +=
                                Msg.FormatWith(
                                    "по показателям оказания услуги для услуги \"{0}\""
                                        .FormatWith(rec.RefVedPch.Name));
                        }
                        else
                        {
                            if (!indicators.Any(x => x.RefIndicators.RefCharacteristicType.Code.Equals(FX_FX_CharacteristicType.VolumeIndex)))
                            {
                                message += Msg1.FormatWith("объема", rec.RefVedPch.Name);
                            }

                            if (!indicators.Any(x => x.RefIndicators.RefCharacteristicType.Code.Equals(FX_FX_CharacteristicType.QualityIndex)))
                            {
                                message += Msg1.FormatWith("качества", rec.RefVedPch.Name);
                            }

                            if (indicators.Any(x => (x.ComingYear == null) || (x.ComingYear == string.Empty)))
                            {
                                message += Msg8.FormatWith(rec.RefVedPch.Name);
                            }
                        }

                        if (rec.RefVedPch.RefPl == null)
                        {
                            switch (ppo)
                            {
                                case "78000000000":
                                    message += Msg6.FormatWith(rec.RefVedPch.Name, "ГРБС");
                                    break;
                                default:
                                    message += Msg6.FormatWith(rec.RefVedPch.Name, "ФО");
                                    break;
                            }
                        }
                        else
                        {
                            // если услуга платная то проверяем заполнение НПА и цены
                            if (rec.RefVedPch.RefPl.Code == 2 || rec.RefVedPch.RefPl.Code == 3)
                            {
                                if (rec.CenaEd == null || rec.CenaEd == 0)
                                {
                                    message += Msg4.FormatWith(rec.RefVedPch.Name);
                                }

                                F_F_GosZadanie rec3 = rec;
                                IQueryable<F_F_NPACena> npa = StateSystemService
                                    .GetItems<F_F_NPACena>().Where(p => (p.RefGZPr.ID == rec3.ID));

                                if (!npa.Any())
                                {
                                    message += Msg.FormatWith(
                                        "по НПА, устанавливающим цены для услуги \"{0}\""
                                            .FormatWith(rec.RefVedPch.Name));
                                }
                                else
                                {
                                    foreach (F_F_NPACena recNpa in npa)
                                    {
                                        if (string.IsNullOrEmpty(recNpa.Name))
                                        {
                                            message += Msg2.FormatWith("Наименование НПА", rec.RefVedPch.Name);
                                        }

                                        if (string.IsNullOrEmpty(recNpa.VidNPAGZ))
                                        {
                                            message += Msg2.FormatWith("Вид НПА", rec.RefVedPch.Name);
                                        }

                                        if (string.IsNullOrEmpty(recNpa.NumNPA))
                                        {
                                            message += Msg2.FormatWith("Номер НПА", rec.RefVedPch.Name);
                                        }

                                        if (recNpa.DataNPAGZ == null)
                                        {
                                            message += Msg2.FormatWith("Дата НПА", rec.RefVedPch.Name);
                                        }

                                        if (string.IsNullOrEmpty(recNpa.OrgUtvDoc))
                                        {
                                            message += Msg2.FormatWith(
                                                "Орган, утвердивший НПА",
                                                rec.RefVedPch.Name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return string.IsNullOrEmpty(message);
        }

        private bool Validate2016(int docId, out string message)
        {
            const string Msg = "Нет данных {0}.<p/>";
            const string Msg1 = "Нет показателя {0} для услуги \"{1}\".<p/>";
            const string Msg2 = "Не задано поле \"{0}\" в НПА для услуги \"{1}\".<p/>";
            const string Msg3 = "Не прикреплен документ типа \"{0}\".<p/>";
            const string Msg4 = "Не задана \"Средневзвешенная цена за единицу услуги\" для услуги \"{0}\".<p/>";
            const string Msg5 = "Отсутствуют категории потребителей для услуги \"{0}\". Обратитесь к {1} <br>";
            const string Msg6 = "У услуги \"{0}\" не указана платность. Обратитесь к {1} <br>";
            const string Msg7 = "\"Средневзвешенная цена за единицу услуги\" для бесплатной услуги(работы)" +
                                " \"{0}\" не должна иметь значения.<br>";
            const string Msg8 = "Не задано поле \"Очередной год\" в показателях оказания услуг для услуги" +
                                " \"{0}\" <br>";
            const string Msg9 = "Фактическое значение отличается от планового.Необходимо заполнить поле \"{0}\" для показателя \"{1}\" услуги \"{2}\".<br>";
            const string Msg10 = "Поле \"Причина отклонения\" не должно быть заполнено для показателя \"{0}\" услуги \"{1}\".<br>";
            const string Msg11 = "Не указан отчет для показателя \"{0}\" в услуге \"{1}\"<br>";
            const string Msg12 = "Не указано фактическое значение для показателя \"{0}\" в услуге \"{1}\"<br>";
            const string Msg13 = "Не указано ни одного показателя для отчета \"{0}\".<br>";
            const string Msg14 = "Не указан орган утвердивший НПА в услуге \"{0}\".<br>";

            message = string.Empty;

            var docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                       where (p.RefParametr.ID == docId) &&
                             (p.Url != "НетФайла") &&
                             (p.RefTypeDoc.Code == "C")
                       select new { p.ID };

            if (!docs.Any())
            {
                message += Msg3.FormatWith(GetNameTypeDoc("C"));
            }
            
            var gosZadanie =
                StateSystemService.GetItems<F_F_GosZadanie2016>().Where(p => (p.RefParameter.ID == docId));

            if (!gosZadanie.Any())
            {
                message += Msg.FormatWith("по услугам");
            }
            else
            {
                // проверяем отчеты
                var reports = StateSystemService.GetItem<F_F_ParameterDoc>(docId).StateTasksReports;
                foreach (var report in reports)
                {
                    if (!gosZadanie.Any(x => x.Indicators.Any(ind => ind.RefReport.ID == report.ID)))
                    {
                        message += Msg13.FormatWith(report.NameReport);
                    }
                }
                
                foreach (var rec in gosZadanie)
                {
                    // НПА регулирующий порядок оказания услуги
                    foreach (var ro in rec.RenderOrders)
                    {
                        // Поле Автор было добавлено позднее, поэтому нужна проверка при отправке на рассмотрение
                        if (ro.Author == null || ro.Author.Trim().IsNullOrEmpty())
                        {
                            message += Msg14.FormatWith(rec.RefService.Regrnumber);
                        }
                    }

                    var rec2 = rec;
                    var indicators = StateSystemService
                        .GetItems<F_F_PNRZnach2016>().Where(p => (p.RefFactGZ.ID == rec2.ID));

                    var recServiceName = rec.RefService.NameName;
                    var rec2ServiceName = rec2.RefService.NameName;
                    
                    // проверяем Причину отклонения и соответствие плана факту у показателей
                    foreach (var item in indicators)
                    {
                        // если фактическое значение указано (или отчет)
                        if ((item.ActualValue != null && item.ActualValue.Trim().IsNotNullOrEmpty()) || item.RefReport != null)
                        {
                            // Должно быть указано фактическое значение
                            if (item.ActualValue == null || item.ActualValue.Trim().IsNullOrEmpty())
                            {
                                message += Msg12.FormatWith(item.RefIndicators.Name, rec.RefService.Regrnumber);
                                continue;
                            }
                            
                            // Должна быть ссылка на отчет
                            if (item.RefReport == null)
                            {
                                message += Msg11.FormatWith(item.RefIndicators.Name, rec.RefService.Regrnumber);
                            }

                            // фактическое значение отличное от очередного года
                            if (item.ActualValue.Trim() != item.ComingYear.Trim())
                            {
                                // если Причина отклонения не заполнено
                                if (item.Protklp.IsNullOrEmpty() || item.Protklp.Trim().IsNullOrEmpty())
                                {
                                    message += Msg9.FormatWith("Причина отклонения", item.RefIndicators.Name, rec2ServiceName);
                                }
                            }
                            else
                            {
                                // если фактическое значение указано и оно идентично очередному году
                                // если Причина отклонения заполнено
                                if (!(item.Protklp.IsNullOrEmpty() || item.Protklp.Trim().IsNullOrEmpty()))
                                {
                                    message += Msg10.FormatWith(item.RefIndicators.Name, rec2ServiceName);
                                }
                            }
                        }
                        else
                        {
                            // если фактическое значение не указано
                            // если Причина отклонения заполнена
                            if (!(item.Protklp.IsNullOrEmpty() || item.Protklp.Trim().IsNullOrEmpty()))
                            {
                                message += Msg10.FormatWith(item.RefIndicators.Name, rec2ServiceName);
                            }
                        }
                    }
                    
                    // проверять только для услуг и работ
                    // для бесплатных работ услуг
                    if (rec.RefService.RefPay.Code.Equals(FX_FX_ServicePayType2.CodeOfFree)
                            && rec.AveragePrice != null && rec.AveragePrice != 0)
                    {
                        message += Msg7.FormatWith(recServiceName);
                    }

                    // проверять только для услуг
                    if (rec.RefService.RefType.Code.Equals(FX_FX_ServiceType.CodeOfService))
                    {
                        var ppo = rec.RefParameter.RefUchr.RefOrgPPO.Code;

                        var rec1 = rec;
                        var consumers = StateSystemService.GetItems<F_F_GZYslPotr2016>()
                            .Where(p => p.RefFactGZ.ID.Equals(rec1.ID));

                        if (!consumers.Any())
                        {
                            switch (ppo)
                            {
                                case "78000000000":
                                    message += Msg5.FormatWith(recServiceName, "ГРБС");
                                    break;
                                default:
                                    message += Msg5.FormatWith(recServiceName, "ФО");
                                    break;
                            }
                        }

                        if (!indicators.Any())
                        {
                            message +=
                                Msg.FormatWith(
                                    "по показателям оказания услуги для услуги \"{0}\""
                                        .FormatWith(recServiceName));
                        }
                        else
                        {
                            if (!indicators.Any(
                                x => x.RefIndicators.RefType.Code
                                    .Equals(FX_FX_CharacteristicType.VolumeIndex)))
                            {
                                message += Msg1.FormatWith("объема", recServiceName);
                            }
                            
                            if (indicators.Any(x => (x.ComingYear == null) || (x.ComingYear == string.Empty)))
                            {
                                message += Msg8.FormatWith(recServiceName);
                            }
                        }

                        if (rec.RefService.RefPay == null)
                        {
                            switch (ppo)
                            {
                                case "78000000000":
                                    message += Msg6.FormatWith(recServiceName, "ГРБС");
                                    break;
                                default:
                                    message += Msg6.FormatWith(recServiceName, "ФО");
                                    break;
                            }
                        }
                        else
                        {
                            // если услуга платная то проверяем заполнение НПА и цены
                            if (rec.RefService.RefPay.Code.Equals(FX_FX_ServicePayType2.CodeOfPayable))
                            {
                                if (rec.AveragePrice == null || rec.AveragePrice == 0)
                                {
                                    message += Msg4.FormatWith(recServiceName);
                                }

                                var rec3 = rec;
                                var npa = StateSystemService
                                    .GetItems<F_F_NPACena2016>().Where(p => (p.RefFactGZ.ID == rec3.ID));

                                if (!npa.Any())
                                {
                                    message += Msg.FormatWith(
                                        "по НПА, устанавливающим цены для услуги \"{0}\""
                                            .FormatWith(recServiceName));
                                }
                                else
                                {
                                    foreach (var recNpa in npa)
                                    {
                                        if (string.IsNullOrEmpty(recNpa.Name))
                                        {
                                            message += Msg2.FormatWith("Наименование НПА", recServiceName);
                                        }

                                        if (string.IsNullOrEmpty(recNpa.VidNPAGZ))
                                        {
                                            message += Msg2.FormatWith("Вид НПА", recServiceName);
                                        }

                                        if (string.IsNullOrEmpty(recNpa.NumNPA))
                                        {
                                            message += Msg2.FormatWith("Номер НПА", recServiceName);
                                        }

                                        if (recNpa.DataNPAGZ == null)
                                        {
                                            message += Msg2.FormatWith("Дата НПА", recServiceName);
                                        }

                                        if (string.IsNullOrEmpty(recNpa.OrgUtvDoc))
                                        {
                                            message += Msg2.FormatWith(
                                                "Орган, утвердивший НПА",
                                                recServiceName);
                                        }
                                    }
                                }
                                
                                foreach (var volumeIndex in indicators
                                    .Where(x => x.RefIndicators.RefType.Code.Equals(FX_FX_CharacteristicType.VolumeIndex)))
                                {
                                    if (volumeIndex.AveragePrices.Count == 0)
                                    {
                                        message += Msg.FormatWith(
                                            "по Среднегодовому размеру платы для показателя \"{0}\""
                                                .FormatWith(volumeIndex.RefIndicators.Name));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return string.IsNullOrEmpty(message);
        }

        private string ReadOnlyScript(bool readOnly, int docId)
        {
            var flag = GetNotBringState(docId) || readOnly;
            var scope = GetScope(docId);

            var resultScript = new StringBuilder();
            resultScript.Append(scope + "SetReadOnlyStateTask({0}, {1});".FormatWith(flag.ToString().ToLower(), docId));

            return resultScript.ToString();
        }

        private string GetScope(int docId)
        {
            var doc = StateSystemService.GetItem<F_F_ParameterDoc>(docId);
            var scope = doc.RefYearForm.ID < 2016 ? "E86n.View.StateTask." : "E86n.View.StateTask2016.";
            return scope;
        }

        /// <summary>
        /// Проверка на наличие признака
        /// todo Данное решение это огромный кастыль. Надо переделать галочку в ГЗ и КНМ на состояния чтобы избавиться от кучи гимороя на клиенте
        /// todo Также переделать систему состояний чтобы не возвращать с сервера JS скрипты. Сделать все через единый контроллер с возвратом параметров для обработки на клиенте 
        /// </summary>
        private string DisableNotBringCheckBox(bool disable, int docId)
        {
            var scope = GetScope(docId);

            var resultScript = new StringBuilder();
            resultScript.Append(scope + "DisableNotBringCheckBox({0});".FormatWith(disable.ToString().ToLower()));

            return resultScript.ToString();
        }

        /// <summary>
        /// Проверяем стоит ли галочка(Не доводить ГЗ) у документа
        /// </summary>
        /// <param name="docId"> идентификатор документа</param>
        /// <returns> true - если галочка присуцтствует</returns>
        private bool GetNotBringState(int docId)
        {
            return StateSystemService.GetItem<F_F_ParameterDoc>(docId).StateTasksExtHeader.Any()
                    && StateSystemService.GetItem<F_F_ParameterDoc>(docId).StateTasksExtHeader.First().NotBring;
        }
    }
}