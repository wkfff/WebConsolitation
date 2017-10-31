using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Utils;
using LinqKit;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateSysyem
{
    public class PassportStateSysController : StateSysBaseController
    {
        /// <summary>
        ///   Экшен выполнения перехода на рассмотрение и на завершение
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
                return resultScript;
            }

            if (!Auth.IsAdmin())
            {
                var stateCode = StateSystemService.GetCurrentStateID(docId);

                if (Auth.IsPpoUser() && !Auth.IsGrbsUser())
                {
                    // если ФО то только чтение
                    resultScript.Append(ReadOnlyScript(true, docId));
                }
                else
                {
                    // если ГРБС или учреждение
                    switch (stateCode)
                    {
                        case (int)StatesType.UnderConsideration:
                            resultScript.Append(Auth.IsGrbsUser() ? ReadOnlyScript(false, docId) : ReadOnlyScript(true, docId));
                            break;
                        case (int)StatesType.Completed:
                            resultScript.Append(ReadOnlyScript(true, docId));
                            break;
                        case (int)StatesType.Exported:
                            resultScript.Append(ReadOnlyScript(true, docId));
                            break;
                        default:
                            resultScript.Append(ReadOnlyScript(false, docId));
                            break;
                    }
                }
            }

            return resultScript;
        }

        /// <summary>
        ///   Экшен выполнения перехода
        /// </summary>
        public override AjaxFormResult DefaultTransition(int docId, int transitionID)
        {
            try
            {
                StateSystemService.Jump(docId, transitionID);
                StringBuilder resultScript = InitComponents(docId);
                resultScript.AppendLine("InstitutionInfoStore.reload();");
                return GetScriptResult(resultScript.ToString());
            }
            catch (Exception e)
            {
                return GetScriptResult(GetShowMessageScript(e.Message));
            }
        }

        /// <summary>
        ///   Проверка заполнения данных на интерфейсе
        /// </summary>
        public string CheckData(int docId)
        {
            const string Msg = "Не заполнено поле \"{0}\"<br>";
            const string Msg1 = "Контактный телефон должен иметь формат 8-ХХХХ-ХХХХХХ<br>";

            string result = string.Empty;

            StateSystemService.GetItems<F_Org_Passport>().FirstOrDefault(x => x.RefParametr.ID == docId).Do(
                x =>
                {
                    if (x.RefVid == null)
                    {
                        result += Msg.FormatWith("Вид учреждения*");
                    }

                    if (x.RefOKATO == null)
                    {
                        result += Msg.FormatWith("Код по ОКАТО (по месту регистрации учреждения)*");
                    }

                    if (x.OGRN.IsNullOrEmpty())
                    {
                        result += Msg.FormatWith("ОГРН*");
                    }

                    if (x.RefOKFS == null)
                    {
                        result += Msg.FormatWith("Код по ОКФС*");
                    }

                    if (x.RefOKTMO == null)
                    {
                        result += Msg.FormatWith("Код по ОКТМО (по месту регистрации учреждения)*");
                    }

                    if (x.OKPO.IsNullOrEmpty())
                    {
                        result += Msg.FormatWith("Код по ОКПО*");
                    }

                    if (x.Indeks.IsNullOrEmpty())
                    {
                        result += Msg.FormatWith("Индекс*");
                    }

                    if (x.Adr.IsNullOrEmpty())
                    {
                        result += Msg.FormatWith("Адрес местонахождения");
                    }

                    if (x.Phone.IsNullOrEmpty())
                    {
                        result += Msg.FormatWith("Контактный телефон*");
                    }

                    if (x.Fam.IsNullOrEmpty())
                    {
                        result += Msg.FormatWith("Фамилия");
                    }

                    if (x.NameRuc.IsNullOrEmpty())
                    {
                        result += Msg.FormatWith("Имя");
                    }

                    if (x.Otch.IsNullOrEmpty())
                    {
                        result += Msg.FormatWith("Отчество");
                    }

                    if (x.Ordinary.IsNullOrEmpty())
                    {
                        result += Msg.FormatWith("Должность");
                    }

                    if (!Regex.IsMatch(x.Phone, @"^8-\d{4}-\d{6}$"))
                    {
                        result += Msg1;
                    }
                });

            return result;
        }

        /// <summary>
        /// Проверка закрытых ОКВЕДОВ
        /// </summary>
        /// <param name="docId"> идентификатор паспорта</param>
        /// <returns> сообщение об ошибке если есть закрытые окведы</returns>
        public string ValidateOKVED(int docId)
        {
            const string Msg = "В паспорте присутствуют закрытые ОКВЭД:<br> ";

            var result = string.Empty;

            StateSystemService.GetItems<F_Org_Passport>().FirstOrDefault(x => x.RefParametr.ID == docId)
                .Do(
                    x =>
                        {
                            x.Activity.ForEach(
                                okved =>
                                    {
                                        if (okved.RefOKVED.CloseDate.HasValue)
                                        {
                                            result += "{0} {1}<br>".FormatWith(okved.RefOKVED.Code, okved.RefOKVED.Name);
                                        }
                                    });
                        });

            return result.IsNotNullOrEmpty() ? Msg + result : result;
        }

        /// <summary>
        ///   Валидация данных
        /// </summary>
        /// <returns> true - если валидация прошла </returns>
        private bool DataValidations(int docId, out string message)
        {
            const string Msg = "Необходимо заполнить закладку \"Филиалы и представительства\"<p/>";
            const string Msg1 = "Не прикреплен документ типа \"{0}\"<p/>";
            const string Msg2 = "Паспорт не заполнен<br>";

            message = string.Empty;

            var docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                       where (p.RefParametr.ID == docId) &&
                             (p.Url != "НетФайла") &&
                             (p.RefTypeDoc.Code == "X")
                       select new { p.ID };

            if (!docs.Any())
            {
                message += Msg1.FormatWith(GetNameTypeDoc("X"));
            }

            docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                   where (p.RefParametr.ID == docId) &&
                         (p.Url != "НетФайла") &&
                         (p.RefTypeDoc.Code == "F")
                   select new { p.ID };

            if (!docs.Any())
            {
                message += Msg1.FormatWith(GetNameTypeDoc("F"));
            }

            docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                   where (p.RefParametr.ID == docId) &&
                         (p.Url != "НетФайла") &&
                         (p.RefTypeDoc.Code == "S")
                   select new { p.ID };

            if (!docs.Any())
            {
                message += Msg1.FormatWith(GetNameTypeDoc("S"));
            }

            docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                   where (p.RefParametr.ID == docId) &&
                         (p.Url != "НетФайла") &&
                         (p.RefTypeDoc.Code == "E")
                   select new { p.ID };

            if (!docs.Any())
            {
                message += Msg1.FormatWith(GetNameTypeDoc("E"));
            }

            var passport = from p in StateSystemService.GetItems<F_Org_Passport>()
                           where p.RefParametr.ID == docId
                           select new
                           {
                               RefTipYc = p.RefParametr.RefUchr != null ? p.RefParametr.RefUchr.RefTipYc.ID : -1,
                               RefCatYh = p.RefCateg != null ? p.RefCateg.Code : -1
                           };

            if (passport.Any())
            {
                message += CheckData(docId);

                if (passport.First().RefTipYc == 10)
                {
                    // Автономное учреждение
                    docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                           where (p.RefParametr.ID == docId) &&
                                 (p.Url != "НетФайла") &&
                                 (p.RefTypeDoc.Code == "I")
                           select new { p.ID };

                    if (!docs.Any())
                    {
                        message += Msg1.FormatWith(GetNameTypeDoc("I"));
                    }
                }

                docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                       where (p.RefParametr.ID == docId) &&
                             (p.Url != "НетФайла") &&
                             (p.RefTypeDoc.Code == "L")
                       select new { p.ID };

                bool branchesFile = docs.Count() != 0;

                // если не прикрепелн файл но заполнена закладка или учреждение имеет категорию 5 или 3 выводить сообщение
                if (
                    ((StateSystemService.GetItems<F_F_Filial>().Count(x => x.RefPassport.RefParametr.ID == docId) != 0) ||
                     (passport.First().RefCatYh == 3) || (passport.First().RefCatYh == 5)) && !branchesFile)
                {
                    message += Msg1.FormatWith(GetNameTypeDoc("L"));
                }

                // если прикрепелн файл но не заполнена закладка филиалов выводить сообщение
                if (
                    (StateSystemService.GetItems<F_F_Filial>().Count(x => x.RefPassport.RefParametr.ID == docId) == 0) &&
                     branchesFile)
                {
                    message += Msg;
                }

                // проверка закрытых ОКВЭДОВ
                message += ValidateOKVED(docId);
            }
            else
            {
                message += Msg2;
            }

            return string.IsNullOrEmpty(message);
        }

        private string ReadOnlyScript(bool readOnly, int docId)
        {
            const string ScopePassport = "E86n.View.PassportView.";

            var resultScript = new StringBuilder();

            resultScript.Append("window.E86n.Control.StateToolBar.ReadOnlyDocHandler({0}, {1});".FormatWith(readOnly.ToString().ToLower(), docId));

            var restrict = Convert.ToBoolean(Convert.ToInt32(ConfigurationManager.AppSettings["RestrictPassportEdit"]));
            if (!restrict)
            {
                resultScript.Append(
                    ScopePassport + "RestrictFields({0});"
                                        .FormatWith(readOnly.ToString().ToLower()));
            }
            else
            {
                resultScript.Append(ScopePassport + "RestrictFields(true);");
            }

            return resultScript.ToString();
        }
    }
}