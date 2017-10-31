using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Metadata;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsFormController : SchemeBoundController
    {
        private readonly ILinqRepository<D_CD_Templates> templatesRepository;
        private readonly IFormImportService importService;
        private readonly IFormExportService exportService;

        public ConsFormController(ILinqRepository<D_CD_Templates> templatesRepository, IFormImportService importService, IFormExportService exportService)
        {
            this.templatesRepository = templatesRepository;
            this.importService = importService;
            this.exportService = exportService;
        }

        public ActionResult Export(int formId)
        {
            try
            {
                D_CD_Templates form = templatesRepository.FindOne(formId);
                var stream = exportService.Export(form);

                return File(stream, "application/zip", form.InternalName + ".zip");
            }
            catch (Exception e)
            {
                AjaxFormResult result = new AjaxFormResult();
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка экспорта формы.";
                result.ExtraParams["responseText"] = String.Format("Ошибка экспорта формы: {0}", e.Message);
                result.IsUpload = true;
                return result;
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult Import()
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                HttpPostedFileBase file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    D_CD_Templates form = importService.Import(file.InputStream);

                    var formValidator = new FormValidator();
                    var errors = formValidator.Validate(form);

                    if (errors.Test(form.TemplateMarkup != null, "У формы незадано сопоставление структуры с шаблоном отображения."))
                    {
                        var validator = new FormLayoutMarkupValidator();
                        errors.AddRange(validator.Validate(form, form.GetFormMarkupMappings()));

                        var mappingValidator = new FormMappingValidator();
                        errors.AddRange(mappingValidator.Validate(form));
                    }

                    if (templatesRepository.FindAll().Any(x => x.Code == form.Code && x.InternalName == form.InternalName && x.FormVersion == form.FormVersion))
                    {
                        errors.Add("Форма с кодом {0}, внутренним именем {1} и версией {2} уже существует.".FormatWith(form.Code, form.InternalName, form.FormVersion));
                    }

                    if (errors.Count == 0)
                    {
                        form.Status = 0;
                        templatesRepository.Save(form);
                        templatesRepository.DbContext.CommitChanges();
                    }
                    else
                    {
                        var errorWnd = new ErrorWindow
                        {
                            Title = "Результаты проверки",
                            Text = String.Join("<br/>", errors.ToArray())
                        };

                        result.Success = true;
                        result.IsUpload = true;
                        result.Script = errorWnd.Build(new ViewPage())[0].ToScript();
                        return result;
                    }
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Импорт формы успешно выполнен.";
                result.IsUpload = true;
                result.Script = "dsForms.reload();";
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка импорта формы.";
                result.ExtraParams["responseText"] = e.Message;
                result.IsUpload = true;
                return result;
            }
        }
    }
}
