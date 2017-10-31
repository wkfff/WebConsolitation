using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ILinqRepository<D_InvProject_Reestr> projectsRepository;
        private readonly ILinqRepository<FX_InvProject_Part> invProjPartRepository;
        private readonly ILinqRepository<F_InvProject_Data> factRepository;
        private readonly ILinqRepository<D_InvProject_Vizual> filesRepository;

        public ProjectService(
                                ILinqRepository<D_InvProject_Reestr> projectsRepository,
                                ILinqRepository<FX_InvProject_Part> invProjPartRepository,
                                ILinqRepository<F_InvProject_Data> factRepository,
                                ILinqRepository<D_InvProject_Vizual> filesRepository)
        {
            this.projectsRepository = projectsRepository;
            this.invProjPartRepository = invProjPartRepository;
            this.factRepository = factRepository;
            this.filesRepository = filesRepository;
        }

        public IList<ProjectsViewModel> GetProjectsTable(InvProjPart partition, bool[] projStatusFilters)
        {
            var data = from p in projectsRepository.FindAll()
                select new ProjectsViewModel
                {
                    ID = p.ID,
                    Code = p.Code,
                    Name = p.Name,
                    Status = p.RefStatus.Code,
                    TerritoryName = p.RefTerritory.Name,
                    BeginYear = p.RefBeginDate.GetYear(),
                    EndYear = p.RefEndDate.GetYear(),
                    Exception = p.Exception,
                    RefPartId = p.RefPart.ID
                };
            
            if (partition != InvProjPart.Undefined)
            {
                data = data.Where(f => f.RefPartId == (int)partition);
            }

            if (projStatusFilters != null && projStatusFilters.Contains(false))
            {
                bool filterEdit = true;
                try
                {
                    filterEdit = projStatusFilters[0];
                } 
                catch (Exception)
                {
                    filterEdit = true;
                }

                bool filterExecuting = true;
                try
                {
                    filterExecuting = projStatusFilters[1];
                } 
                catch (Exception)
                {
                    filterExecuting = true;
                }

                bool filterExcluded = true; 
                try
                {
                    filterExcluded = projStatusFilters[2];
                } 
                catch (Exception)
                {
                    filterExcluded = true;
                }
                
                data = data.Where(f => (filterEdit && (f.Status == (int)InvProjStatus.Edit))
                                      || (filterExecuting && (f.Status == (int)InvProjStatus.Executing))
                                      || (filterExcluded && (f.Status == (int)InvProjStatus.Excluded)));
            }

            return new List<ProjectsViewModel>(data);
        }

        public void ChangeProjectPart(int projectId)
        {
            var project = projectsRepository.FindOne(projectId);
            if (project == null)
            {
                throw new KeyNotFoundException("Запись не найдена.");
            }

            if (project.RefPart.ID == (int)InvProjPart.Part1)
            {
                project.RefPart = GetRefPart((int)InvProjPart.Part2);
            }
            else if (project.RefPart.ID == (int)InvProjPart.Part2)
            {
                project.RefPart = GetRefPart((int)InvProjPart.Part1);
            }
            else
            {
                throw new Exception("Недостижимая ветка кода.");
            }

            projectsRepository.Save(project);
        }

        public void DeleteProject(int projectId)
        {
            var project = projectsRepository.FindOne(projectId);
            if (project == null)
            {
                throw new KeyNotFoundException("Запись не найдена.");
            }

            var facts = factRepository.FindAll().Where(f => f.RefReestr == project);
            foreach (var row in facts)
            {
                factRepository.Delete(row);
            }

            var files = filesRepository.FindAll().Where(f => f.RefReestr == project);
            foreach (var row in files)
            {
                filesRepository.Delete(row);
            }

            projectsRepository.Delete(project);
        }

        public D_InvProject_Reestr GetProject(int id)
        {
            var entity = projectsRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Проект не найден.");
            }

            return entity;
        }

        public ProjectDetailViewModel GetProjectModel(int projectId)
        {
            var data = new List<ProjectDetailViewModel>(
                            from p in projectsRepository.FindAll()
                            where p.ID == projectId
                            select new ProjectDetailViewModel
                            {
                                ID = p.ID,
                                IncomingDate = p.IncomingDate,
                                RosterDate = p.RosterDate,
                                Name = p.Name,
                                InvestorName = p.InvestorName,
                                LegalAddress = p.LegalAddress,
                                MailingAddress = p.MailingAddress,
                                Email = p.Email,
                                Phone = p.Phone,
                                Goal = p.Goal,
                                ExpectedOutput = p.ExpectedOutput,
                                Production = p.Production,
                                PaybackPeriod = p.PaybackPeriod,
                                DocBase = p.DocBase,
                                InvestAgreement = p.InvestAgreement,
                                AddMech = p.AddMech,
                                ExpertOpinion = p.ExpertOpinion,
                                Study = p.Study,
                                Effect = p.Effect,
                                Exception = p.Exception,
                                Contact = p.Contact,
                                Code = p.Code,
                                RefBeginDateVal = p.RefBeginDate.GetYear(),
                                RefEndDateVal = p.RefEndDate.GetYear(),
                                RefTerritoryId = p.RefTerritory.ID,
                                RefTerritoryName = p.RefTerritory.Name,
                                RefStatusId = p.RefStatus.ID,
                                RefPartId = p.RefPart.ID,
                                RefOKVEDId = p.RefOKVED.ID,
                                RefOKVEDName = p.RefOKVED.Name,
                            });
            if (data.Count > 0)
            {
                return data.First();
            }
            else
            {
                throw new KeyNotFoundException("Проект не найден.");
            }
        }

        public ProjectDetailViewModel GetInitialProjectModel(InvProjPart projPart)
        {
            var data = new ProjectDetailViewModel
            {
                Name = "Новый проект",
                RefStatusId = (int)InvProjStatus.Edit,
                RefPartId = (int)projPart,
                RefBeginDateVal = projPart == InvProjPart.Part1 ? DateTime.Now.Year : (int?)null,
                RefEndDateVal = projPart == InvProjPart.Part1 ? DateTime.Now.Year : (int?)null
            };
            return data;
        }

        public void Validate(D_InvProject_Reestr entity)
        {
            if (entity.RefBeginDate.GetYear() > entity.RefEndDate.GetYear())
            {
                throw new ArgumentException("Год начала реализации должен быть меньше года окончания! ");
            }

            if (entity.RefStatus.ID == (int)InvProjStatus.Excluded && String.IsNullOrEmpty(entity.Exception))
            {
                throw new ArgumentException("Не заполнено поле \"Причина исключения\"");
            }

            if (entity.RefStatus.ID == (int)InvProjStatus.Executing && (entity.RefBeginDate.ID <= 0 || entity.RefEndDate.ID <= 0))
            {
                throw new ArgumentException("Не корректно заполнены поля \"Гол начала/окончания реализации\"");
            }

            return;
        }

        public void SaveProject(D_InvProject_Reestr entityNew, D_InvProject_Reestr entityOld)
        {
            projectsRepository.Save(entityNew);

            // Если у проекта сдвинули дату запуска/окончания - удаляем из таблицы фактов то что не влезло в новый интервал дат
            if (entityOld != null
                && (entityNew.RefBeginDate.GetYear() != entityOld.RefBeginDate.GetYear() //// "прошлогодний год" у InvProjInvestType.TargetRatings может подосрать
                    || entityNew.RefEndDate.GetYear() < entityOld.RefEndDate.GetYear()))
            {
                DeleteIncorrectFactData(entityNew);
            }
        }

        public decimal GetSumInvestPlan(int refProjId)
        {
            var list = from p in factRepository.FindAll()
                       where p.RefReestr.ID == refProjId
                             && (p.RefIndicator.RefTypeI.ID == (int)InvProjInvestType.Investment
                                 || p.RefIndicator.RefTypeI.ID == (int)InvProjInvestType.GosSupport)
                       select p.Value;

            decimal result = 0;
            if (list.Count() > 0)
            {
                result = list.Sum();
            }

            return result;
        }

        public void DeleteIncorrectFactData(D_InvProject_Reestr entity)
        {
            ////var oldFactEntitys = from f in factRepository.FindAll()
            ////                     where f.RefReestr == entity
            ////                           && ((f.RefDate.ID < entity.RefBeginDate.ID) || (f.RefDate.ID > entity.RefEndDate.ID))
            ////                     select f;

            ////foreach (var row in oldFactEntitys)
            ////{
            ////    factRepository.Delete(row);
            ////}

            // TODO : корректная очистка таблицы фактов - непопавших в диапазон, и "предыдущий год"+-
            var factDataCount = factRepository.FindAll().Where(f => f.RefReestr == entity).Count();
            if (factDataCount > 0)
            {
                throw new Exception("Из-за имеющихся введенных данных нельзя изменять дату начала/окончание проекта.");
            }
        }

        public InvProjStatus GetProjectStatus(int projectId)
        {
            var entity = GetProject(projectId);
            return (InvProjStatus)entity.RefStatus.ID;
        }

        private FX_InvProject_Part GetRefPart(int id)
        {
            var entity = invProjPartRepository.FindOne(id);
            return entity;
        }
    }
}
