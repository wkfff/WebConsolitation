using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Controllers
{
    public class ProjectDetailController : SchemeBoundController
    {
        private readonly IProjectService projectService;
        private readonly IAdditionalDataService additionalService;

        public ProjectDetailController(
                                       IProjectService projectService,
                                       IAdditionalDataService additionalService)
        {
            this.projectService = projectService;
            this.additionalService = additionalService;
        }

        public ActionResult Load(int? projId, int refPartId)
        {
            ProjectDetailViewModel data;
            if (projId == null)
            {
                data = projectService.GetInitialProjectModel((InvProjPart)refPartId);
            }
            else
            {
                data = projectService.GetProjectModel((int)projId);
            }

            return new AjaxStoreResult(data, 1);
        }

        public ActionResult LoadSumInvestPlan(int? projId)
        {
            if (projId == null)
            {
                return new AjaxStoreResult(new List<object>(), 0);
            }
            else
            {
                var data = new { ID = 1, Value = projectService.GetSumInvestPlan((int)projId) };
                return new AjaxStoreResult(data, 1);
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult Save(int? projId, FormCollection values)
        {
            var result = new AjaxFormResult();
            StringBuilder script = new StringBuilder();

            // Заменяем - вместо null приходит 0
            if (projId == 0)
            {
                projId = null;
            }

            try
            {
                var model = ConvertToModel(values);
                
                D_InvProject_Reestr entityNew;
                D_InvProject_Reestr entityOld;

                if (projId != null)
                {
                    entityNew = projectService.GetProject((int)projId);
                    entityOld = (D_InvProject_Reestr)Copy(entityNew);
                }
                else
                {
                    entityNew = new D_InvProject_Reestr();
                    entityOld = null;
                }

                StuffProjectEntityFromModel(entityNew, model);

                // Проверяем корректность заполнения атрибутов
                projectService.Validate(entityNew);

                // Сохраняем
                projectService.SaveProject(entityNew, entityOld);

                // Если запись создавали как новую, то на форме необходимо выполнить манипуляции
                if (projId == null)
                {
                    script.AppendFormat("ID.setValue({0});\n", entityNew.ID);

                    if (entityNew.RefPart.ID == (int)InvProjPart.Part1)
                    {
                        script.AppendLine("btSaveNewProject.setVisible(false);");
                        script.AppendLine("ProjectDetailTabPanel.setDisabled(false);");
                        script.AppendFormat("InvestTab.autoLoad.params.projId={0};\n", entityNew.ID);
                        script.AppendFormat("GosTab.autoLoad.params.projId={0};\n", entityNew.ID);
                        script.AppendFormat("TargetRatingsTab.autoLoad.params.projId={0};\n", entityNew.ID);
                        script.AppendFormat("VisualizationTab.autoLoad.params.projId={0};\n", entityNew.ID);
                        script.AppendLine("InvestTab.reload();");
                    }
                }

                // Закладку со списком проектов нужно обновить
                switch (model.RefPartId)
                {
                    case (int)InvProjPart.Part1:
                        script.AppendLine("parent.RunningProjectsTab.getBody().dsProjects.reload();");
                        break;
                    case (int)InvProjPart.Part2:
                        script.AppendLine("parent.ProposedProjectsTab.getBody().dsProjects.reload();");
                        break;
                }
                
                // Подчиненные закладки нужно перезагрузить (изменится структура таблицы)
                if (projId != null && (entityNew.RefBeginDate != entityOld.RefBeginDate || entityNew.RefEndDate != entityOld.RefEndDate))
                {
                    // TODO: отимизация быстродейстыия - перезагружать только активную вкладку, остальные помечать на обновление, иначе лезет ошибка в js
                    script.AppendLine("if(InvestTab.iframe != undefined){InvestTab.reload();}");
                    script.AppendLine("if(GosTab.iframe != undefined){GosTab.reload();}");
                    script.AppendLine("if(TargetRatingsTab.iframe != undefined){TargetRatingsTab.reload();}");
                }
                else
                {
                    string visible;

                    // В зависимости от статуса меняем доступные кнопки сохранения на форме:
                    if (entityNew.RefStatus.ID == (int)InvProjStatus.Edit)
                    {
                        visible = "true";
                    }
                    else
                    {
                        visible = "false";
                    }

                    script.AppendFormat(
                        @"
var tab = Ext.getCmp('InvestTab');
if(tab != undefined && tab.iframe != undefined){{
  var tabBody = tab.getBody();
  tabBody.btnAdd.setVisible({0});
  tabBody.btnDelete.setVisible({0});
  tabBody.btnSave.setVisible({0});
}}
tab = Ext.getCmp('GosTab');
if(tab != undefined && tab.iframe != undefined){{
  var tabBody = tab.getBody();
  tabBody.btnAdd.setVisible({0});
  tabBody.btnDelete.setVisible({0});
  tabBody.btnSave.setVisible({0});
}}
tab = Ext.getCmp('TargetRatingsTab');
if(tab != undefined && tab.iframe != undefined){{
  var tabBody = tab.getBody();
  tabBody.btnAdd.setVisible({0});
  tabBody.btnDelete.setVisible({0});
  tabBody.btnSave.setVisible({0});
}}
tab = Ext.getCmp('VisualizationTab');
if(tab != undefined && tab.iframe != undefined){{
  var tabBody = tab.getBody();
  tabBody.btnAddFile.setVisible({0});
  tabBody.btnSave.setVisible({0});
}}
",
                        visible);
                }

                script.AppendLine("ResetDirtyAttributeOnFormItems(ProjectAttrForm);");

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

        private void StuffProjectEntityFromModel(D_InvProject_Reestr entity, ProjectDetailViewModel model)
        {
            // Если статус "Исключен", то позволяем менять только этот статус и коммент к нему
            if (model.RefStatusId == (int)InvProjStatus.Excluded)
            {
                entity.RefStatus = additionalService.GetRefStatus(model.RefStatusId);
                entity.Exception = model.Exception;
            }
            else
            {
                entity.IncomingDate = model.IncomingDate;
                entity.RosterDate = model.RosterDate;
                entity.Name = model.Name;
                entity.InvestorName = model.InvestorName;
                entity.LegalAddress = model.LegalAddress;
                entity.MailingAddress = model.MailingAddress;
                entity.Email = model.Email;
                entity.Phone = model.Phone;
                entity.Goal = model.Goal;
                entity.ExpectedOutput = model.ExpectedOutput;
                entity.Production = model.Production;
                entity.PaybackPeriod = model.PaybackPeriod;
                entity.DocBase = model.DocBase;
                entity.InvestAgreement = model.InvestAgreement;
                entity.AddMech = model.AddMech;
                entity.ExpertOpinion = model.ExpertOpinion;
                entity.Study = model.Study;
                entity.Effect = model.Effect;
                entity.Exception = null;
                entity.Contact = model.Contact;
                entity.Code = model.Code;
                entity.RefBeginDate = model.RefBeginDateVal != null
                                          ? additionalService.GetRefYear((int)model.RefBeginDateVal)
                                          : additionalService.GetRefYearDayUnvUndefined();
                entity.RefEndDate = model.RefEndDateVal != null
                                        ? additionalService.GetRefYear((int)model.RefEndDateVal)
                                        : additionalService.GetRefYearDayUnvUndefined();
                entity.RefTerritory = additionalService.GetRefTerritory(model.RefTerritoryId);
                entity.RefStatus = additionalService.GetRefStatus(model.RefStatusId);
                entity.RefPart = additionalService.GetRefPart(model.RefPartId);
                entity.RefOKVED = additionalService.GetRefOKVED(model.RefOKVEDId);
            }
        }

        private ProjectDetailViewModel ConvertToModel(FormCollection parameters)
        {
            CheckParameters(parameters);
            var model = new ProjectDetailViewModel();
            model.ID = Convert.ToInt32(parameters["ID"]);
            model.IncomingDate = Convert.ToDateTime(parameters["IncomingDate"]);
            model.RosterDate = parameters["RosterDate"] == String.Empty ? null : (DateTime?)Convert.ToDateTime(parameters["RosterDate"]);
            model.Name = parameters["Name"];
            model.InvestorName = parameters["InvestorName"];
            model.LegalAddress = parameters["LegalAddress"];
            model.MailingAddress = parameters["MailingAddress"];
            model.Email = parameters["Email"];
            model.Phone = parameters["Phone"];
            model.Goal = parameters["Goal"];
            model.ExpectedOutput = parameters["ExpectedOutput"];
            model.Production = parameters["Production"];
            model.PaybackPeriod = parameters["PaybackPeriod"];
            model.DocBase = parameters["DocBase"];
            model.InvestAgreement = parameters["InvestAgreement"];
            model.AddMech = parameters["AddMech"];
            model.ExpertOpinion = parameters["ExpertOpinion"];
            model.Study = parameters["Study"];
            model.Effect = parameters["Effect"];
            model.Exception = parameters["Exception"];
            model.Contact = parameters["Contact"];
            model.Code = parameters["Code"];
            model.RefBeginDateVal = String.IsNullOrEmpty(parameters["RefBeginDateVal"]) ? null : (int?)Convert.ToInt32(parameters["RefBeginDateVal"]);
            model.RefEndDateVal = String.IsNullOrEmpty(parameters["RefEndDateVal"]) ? null : (int?)Convert.ToInt32(parameters["RefEndDateVal"]);
            model.RefTerritoryId = Convert.ToInt32(parameters["RefTerritoryId"]);
            model.RefStatusId = Convert.ToInt32(parameters["comboRefStatus_Value"]);
            model.RefPartId = Convert.ToInt32(parameters["comboRefPart_Value"]);
            model.RefOKVEDId = Convert.ToInt32(parameters["RefOKVEDId"]);

            return model;
        }

        private void CheckParameters(FormCollection parameters)
        {
            // TODO: проверка входящих параметров на заполненность и соответствие типу
            if (String.IsNullOrEmpty(parameters["IncomingDate"]))
            {
                throw new NoNullAllowedException("Поле <Дата поступления сведений> должно быть заполнено");
            }

            return;
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
