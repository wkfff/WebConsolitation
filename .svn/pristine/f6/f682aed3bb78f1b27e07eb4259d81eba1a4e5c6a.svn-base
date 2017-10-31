using System;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Helpers;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsFormActivatorController : SchemeBoundController
    {
        private readonly ILinqRepository<D_CD_Templates> templatesRepository;
        private readonly IFormActivatorService activatorService;
        private readonly IFormValidationService validationService;

        public ConsFormActivatorController(ILinqRepository<D_CD_Templates> templatesRepository, IFormActivatorService activatorService, IFormValidationService validationService)
        {
            this.templatesRepository = templatesRepository;
            this.activatorService = activatorService;
            this.validationService = validationService;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activate(int formId)
        {
            var form = templatesRepository.FindOne(formId);

            var errors = validationService.Validate(form);
            if (errors.Count > 0)
            {
                var errorWnd = new ErrorWindow
                {
                    Title = "Результаты проверки",
                    Text = String.Join("<br/>", errors.ToArray())
                };

                return new AjaxResult(errorWnd.Build(new ViewPage())[0].ToScript());
            }

            // Активируем форму
            templatesRepository.DbContext.BeginTransaction();
            try
            {
                activatorService.Activate(formId);

                templatesRepository.DbContext.CommitChanges();
                templatesRepository.DbContext.CommitTransaction();
            }
            catch (Exception e)
            {
                templatesRepository.DbContext.RollbackTransaction();

                var errorWnd = new ErrorWindow
                {
                    Title = "Ошибка",
                    Text = e.ExpandException(new HtmlExceptionFormatter())
                };

                return new AjaxResult(errorWnd.Build(new ViewPage())[0].ToScript());
            }

            // Перестраиваем маппинг
            try
            {
                activatorService.RebuildSession(form);
            }
            catch (Exception e)
            {
                var errorWnd = new ErrorWindow
                {
                    Title = "Ошибка маппинга",
                    Text = e.ExpandException(new HtmlExceptionFormatter())
                };

                return new AjaxResult(errorWnd.Build(new ViewPage())[0].ToScript());
            }

            return new AjaxResult(ExtNet.Msg.Alert("Сообщение", "Форма активирована успешно.").ToScript());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public ActionResult Validate(int formId)
        {
            var form = templatesRepository.FindOne(formId);

            var errors = validationService.Validate(form);
            if (errors.Count > 0)
            {
                var errorWnd = new ErrorWindow
                {
                    Title = "Результаты проверки",
                    Text = String.Join("<br/>", errors.ToArray())
                };

                return new AjaxResult(errorWnd.Build(new ViewPage())[0].ToScript());
            }

            return new AjaxResult(ExtNet.Msg.Alert("Сообщение", "Форма проверена успешно.").ToScript());
        }
    }
}
