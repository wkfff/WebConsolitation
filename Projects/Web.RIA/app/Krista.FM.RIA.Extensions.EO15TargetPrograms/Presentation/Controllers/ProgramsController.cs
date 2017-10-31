using System;
using System.Data;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Models;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Views;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Controllers
{
    public class ProgramsController : SchemeBoundController
    {
        private readonly IProgramService programService;
        private readonly INpaService npaService;
        private readonly IAdditionalService additionalService;

        public ProgramsController(
                                   IProgramService programService,
                                   INpaService npaService,
                                   IAdditionalService additionalService)
        {
            this.programService = programService;
            this.npaService = npaService;
            this.additionalService = additionalService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult GetProgramsTable(bool[] filters)
        {
            bool filterTypeDCP = filters[0];
            bool filterTypeVCP = filters[1];
            bool filterUnapproved = filters[2];
            bool filterApproved = filters[3];
            bool filterRunning = filters[4];
            bool filterFinished = filters[5]; 
            
            var data = this.programService.GetProgramsTable(filterTypeDCP, filterTypeVCP, filterUnapproved, filterApproved, filterRunning, filterFinished);

            return new RestResult { Success = true, Data = data };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetParentProgramListForLookup()
        {
            var data = this.programService.GetParentProgramListForLookup();
            return new AjaxStoreResult(data, data.Count);
        }
        
        [AcceptVerbs(HttpVerbs.Delete)]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult DeleteProgram(int id)
        {
            var result = new RestResult();
            try
            {
                if (!User.IsInRole(ProgramRoles.Creator))
                {
                    throw new SecurityException("Недостаточно привилегий");
                }

                programService.DeleteProgram(id);
                
                return new RestResult { Success = true, Message = "Программа удалена." };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.Message = String.Format("Ошибка удаления: {0}", e.Message);
                return result;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LoadProgram(int? programId)
        {
            ProgramViewModel data;
            if (programId == null)
            {
                data = programService.GetInitialProgramModel();
            }
            else
            {
                data = programService.GetProgramModel((int)programId);
            }

            return new AjaxStoreResult(data, 1);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult SaveProgram(int? programId, FormCollection values)
        {
            var result = new AjaxFormResult();
            StringBuilder script = new StringBuilder();

            // Заменяем - вместо null приходит 0
            if (programId == 0)
            {
                programId = null;
            }

            try
            {
                if (!User.IsInRole(ProgramRoles.Creator))
                {
                    throw new SecurityException("Недостаточно привилегий");
                }

                var model = ConvertToModel(values);

                D_ExcCosts_ListPrg entityNew;
                D_ExcCosts_ListPrg entityOld;

                if (programId != null)
                {
                    entityNew = programService.GetProgram((int)programId);
                    entityOld = (D_ExcCosts_ListPrg)Copy(entityNew);

                    var editable = new PermissionSettings(User, entityNew).CanDeleteProgram;
                    if (!editable)
                    {
                        throw new SecurityException("Недостаточно привилегий");
                    }
                }
                else
                {
                    entityNew = new D_ExcCosts_ListPrg();
                    entityOld = null;
                }

                StuffEntityFromModel(entityNew, model);

                programService.SaveProject(entityNew, entityOld);
               
                // Если запись создавали как новую, то на форме необходимо выполнить манипуляции
                if (programId == null)
                {
                    script.AppendFormat("ID.setValue({0});\n", entityNew.ID)
                            .AppendLine("btSaveNewProgram.setVisible(false);")
                            .AppendFormat("{0}.setBaseParam('programId',{1});", ProgramView.DatasourceProgramID, entityNew.ID)
                            .AppendLine("ProgramDetailTabPanel.setDisabled(false);")
                            .AppendFormat("TabNPA.autoLoad.params.programId={0};\n", entityNew.ID)
                            .AppendFormat("TabTargets.autoLoad.params.programId={0};\n", entityNew.ID)
                            .AppendFormat("TabTasks.autoLoad.params.programId={0};\n", entityNew.ID)
                            .AppendFormat("TabActions.autoLoad.params.programId={0};\n", entityNew.ID)
                            .AppendFormat("TabFinances.autoLoad.params.programId={0};\n", entityNew.ID)
                            .AppendFormat("TabTargetRatings.autoLoad.params.programId={0};\n", entityNew.ID)
                            .AppendFormat("TabSubsidy.autoLoad.params.programId={0};\n", entityNew.ID)
                            .AppendLine("TabNPA.reload();");
                }

                script.AppendLine("ResetDirtyAttributeOnFormItems(programForm);");
                script.AppendLine("parent.dsPrograms.reload();");
                
                result.Success = true;
                result.ExtraParams["msg"] = "Запись изменена.";
                result.Script = script.ToString();
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка сохранения.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ApproveProgram(int programId, string approveDate, string fileName, string npaName)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                if (!User.IsInRole(ProgramRoles.Creator))
                {
                    throw new SecurityException("Привилегий недостаточно.");
                }

                HttpPostedFileBase uploadFile = Request.Files[0];
                if (uploadFile.ContentLength <= 0)
                {
                    throw new ArgumentNullException("Файл пустой!");
                }

                byte[] fileBody = new byte[uploadFile.ContentLength];
                uploadFile.InputStream.Read(fileBody, 0, uploadFile.ContentLength);

                DateTime approvementDate = Convert.ToDateTime(approveDate);

                var program = programService.GetProgram(programId);

                var approveAvailable = new PermissionSettings(User, program).CanApprove;
                if (approveAvailable)
                {
                   npaService.InsertFile(program, fileName, fileBody, npaName, false);
                   programService.ApproveProgram(program, approvementDate);
                }
                else
                {
                    throw new SecurityException("Утверждение невозможно");
                }

                StringBuilder script = new StringBuilder();
                script.AppendLine("btnApprove.setDisabled(true);");
                script.AppendFormat("ApproveDate.setValue('{0}.{1}.{2}');\n", approvementDate.Day, approvementDate.Month, approvementDate.Year);
                script.AppendLine("TabNPA.reload();");

                result.Success = true;
                result.ExtraParams["msg"] = "Программа утверждена.";
                result.IsUpload = true;
                result.Script = script.ToString();
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка утверждения.";
                result.ExtraParams["responseText"] = e.Message;
                result.IsUpload = true;
                return result;
            }
        }

        /// <summary>
        /// Преобразует параметры, полученные с формы, в модель данной вьюхи
        /// </summary>
        private ProgramViewModel ConvertToModel(FormCollection parameters)
        {
            CheckParameters(parameters);

            var model = new ProgramViewModel();
            model.ID = Convert.ToInt32(parameters["ID"]);
            model.Name = parameters["Name"];
            model.ShortName = parameters["ShortName"];
            model.RefTypeProgId = Convert.ToInt32(parameters["comboRefPart_Value"]);
            model.RefBeginDateVal = Convert.ToInt32(parameters["RefBeginDateVal"]);
            model.RefEndDateVal = Convert.ToInt32(parameters["RefEndDateVal"]);
            model.ParentId = String.IsNullOrEmpty(parameters["ParentId"]) ? null : (int?)Convert.ToInt32(parameters["ParentId"]);
            return model;
        }

        private void CheckParameters(FormCollection parameters)
        {
            if (String.IsNullOrEmpty(parameters["Name"]))
            {
                throw new NoNullAllowedException("Поле <Наименование проекта> должно быть заполнено");
            }

            if (String.IsNullOrEmpty(parameters["RefBeginDateVal"]))
            {
                throw new NoNullAllowedException("Поле <Год начала реализации> должно быть заполнено");
            }

            if (String.IsNullOrEmpty(parameters["RefEndDateVal"]))
            {
                throw new NoNullAllowedException("Поле <Год окончания реализации> должно быть заполнено");
            }

            return;
        }

        private void StuffEntityFromModel(D_ExcCosts_ListPrg entity, ProgramViewModel model)
        {
            entity.Name = model.Name;
            entity.ShortName = model.ShortName;
            ////entity.Note = String.Empty;
            entity.ParentID = model.ParentId;
            entity.RefCreators = additionalService.GetCreator(User.DbUser.Name);
            entity.RefTypeProg = additionalService.GetRefTypeProg(model.RefTypeProgId);
            entity.RefApYear = null;
            entity.RefBegDate = additionalService.GetRefYear(model.RefBeginDateVal);
            entity.RefEndDate = additionalService.GetRefYear(model.RefEndDateVal);
            entity.RefTerritory = additionalService.GetRefTerritory(-1);
        }
        
        private object Copy(object obj)
        {
            var result = obj.GetType().GetConstructor(new Type[0]).Invoke(null);
            foreach (var property in obj.GetType().GetProperties())
            {
                property.SetValue(result, property.GetValue(obj, null), null);
            }

            return result;
        }
    }
}
