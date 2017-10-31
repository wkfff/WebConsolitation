using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class FeedbackView : View
    {
        private readonly IAuthService auth;

        public FeedbackView(IAuthService auth)
        {
            this.auth = auth;
        }

        public override List<Component> Build(ViewPage page)
        {
            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.PUT;
            restActions.Destroy = HttpMethod.DELETE;

            var emailText = string.Empty;
            var fioText = string.Empty;
            var phoneText = string.Empty;
            var innText = string.Empty;
            var nameText = string.Empty;

            if (auth.Profile != null)
            {
                var passports = auth.ProfileOrg.Documents.SelectMany(doc => doc.Passports).OrderByDescending(passport => passport.ID).ToList();
                var passportWithMail = passports.FirstOrDefault(passport => passport.Mail.IsNotNullOrEmpty());
                var passportWithFio = passports.FirstOrDefault(passport => passport.NameRuc.IsNotNullOrEmpty() || passport.Fam.IsNotNullOrEmpty() || passport.Otch.IsNotNullOrEmpty());
                var passportWithPhone = passports.FirstOrDefault(passport => passport.Phone.IsNotNullOrEmpty());

                if (passportWithMail != null)
                {
                    emailText = passportWithMail.Mail;
                }

                if (passportWithFio != null)
                {
                    fioText = passportWithFio.Fam + " " + passportWithFio.NameRuc + " " + passportWithFio.Otch;
                }

                if (passportWithPhone != null)
                {
                    phoneText = passportWithPhone.Phone;
                }

                innText = auth.ProfileOrg.INN;
                nameText = auth.ProfileOrg.ShortName;
            }

            var email = new TextField
                            {
                                ID = "FeedbackEmail",
                                FieldLabel = @"E-mail",
                                Text = emailText,
                                Vtype = "email",
                                AllowBlank = false,
                                IndicatorIcon = Icon.BulletRed,
                                IndicatorTip = "Это обязательное поле",
                                Anchor = "-20"
                            };

            var fio = new TextField
                          {
                              ID = "FeedbackFio",
                              FieldLabel = @"ФИО",
                              Text = fioText,
                              AllowBlank = false,
                              BlankText = @"Введите ФИО",
                              IndicatorIcon = Icon.BulletRed,
                              IndicatorTip = "Это обязательное поле",
                              Anchor = "-20"
                          };

            var inn = new TextField
                          {
                              ID = "FeedbackInn",
                              FieldLabel = @"ИНН",
                              Text = innText,
                              AllowBlank = false,
                              BlankText = @"Введите ИНН",
                              IndicatorIcon = Icon.BulletRed,
                              IndicatorTip = "Это обязательное поле",
                              Anchor = "-20"
                          };

            var name = new TextField
                           {
                               ID = "FeedbackName",
                               FieldLabel = @"Наименование учреждения",
                               Text = nameText,
                               Anchor = "-20"
                           };

            var phonenumber = new TextField
                                  {
                                      ID = "FeedbackPhonenumber",
                                      FieldLabel = @"Телефон",
                                      Text = phoneText,
                                      AllowBlank = false,
                                      BlankText = @"Введите телефон для связи",
                                      IndicatorIcon = Icon.BulletRed,
                                      IndicatorTip = "Это обязательное поле",
                                      Anchor = "-20"
                                  };

            var caption = new TextField
                              {
                                  ID = "FeedbackCaption",
                                  FieldLabel = @"Тема",
                                  AllowBlank = false,
                                  BlankText = @"Введите тему для обращения",
                                  IndicatorIcon = Icon.BulletRed,
                                  IndicatorTip = "Это обязательное поле",
                                  Anchor = "-20"
                              };

            var message = new TextArea
                              {
                                  ID = "FeedbackMessage",
                                  FieldLabel = @"Сообщение",
                                  AllowBlank = false,
                                  BlankText = @"Введите сообщение",
                                  IndicatorIcon = Icon.BulletRed,
                                  IndicatorTip = "Это обязательное поле",
                                  Anchor = "-20"
                              };

            var sendButton = new Button
                                 {
                                     ID = "FeedbackSendbtn",
                                     Text = @"Отправить",
                                     Disabled = true,
                                     DirectEvents =
                                         {
                                             Click =
                                                 {
                                                     Url =
                                                         UiBuilders.GetUrl<FeedbackController>("SendFeedback"),
                                                     Method = HttpMethod.GET,
                                                     ExtraParams =
                                                         {
                                                             new Parameter("caption", "'Обращение ' + FeedbackInn.getValue() + ' : ' + FeedbackCaption.getValue()", ParameterMode.Raw),

                                                             new Parameter(
                                                                 "message",
                                                                 "'E-mail : ' +                     FeedbackEmail.getValue() + '\\n' + " +
                                                                 "'ФИО : ' +                        FeedbackFio.getValue() + '\\n' + " +
                                                                 "'ИНН : ' +                        FeedbackInn.getValue() + '\\n' + " +
                                                                 "'Наименование учреждения : ' +    FeedbackName.getValue() + '\\n' + " +
                                                                 "'Телефон : ' +                    FeedbackPhonenumber.getValue() + '\\n\\n' + " +
                                                                 "'Тема : ' +                       FeedbackCaption.getValue() + '\\n' + " +
                                                                 "'Сообщение : ' +                  FeedbackMessage.getValue()",
                                                                 ParameterMode.Raw)
                                                         },
                                                     Success = @"alert('Письмо отправлено, спасибо за обращение в техподдержку.')
                                                                top.HBWnd.hide();",
                                                     Failure = @"alert(result.extraParams.responseText);
                                                                top.window.open(result.extraParams.redirectTo);"
                                                 }
                                         }
                                 };

            var form = new FormPanel
                           {
                               Padding = 5,
                               Items =
                                   {
                                       email,
                                       fio,
                                       inn,
                                       name,
                                       phonenumber,
                                       caption,
                                       message
                                   },
                               Buttons =
                                   {
                                       sendButton
                                   },
                               MonitorValid = true,
                               Listeners =
                                   {
                                       ClientValidation =
                                           {
                                               Handler = "FeedbackSendbtn.setDisabled(!valid);"
                                           }
                                   }
                           };

            return new List<Component>
                       {
                           new Viewport
                               {
                                   ID = "viewportMain",
                                   Layout = LayoutType.Fit.ToString(),
                                   Items =
                                       {
                                           form
                                       }
                               }
                       };
        }
    }
}
