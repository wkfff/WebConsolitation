using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Krista.FM.Common.Constants;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Models;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class ProgramService : IProgramService
    {
        private readonly ILinqRepository<D_ExcCosts_ListPrg> programsRepository;
        private readonly ILinqRepository<D_ExcCosts_Events> actionsRepository;
        private readonly INpaService npaService;
        private readonly ITargetService targetService;
        private readonly IAdditionalService additionalService;
        private readonly IExtension extension;

        public ProgramService(
                               ILinqRepository<D_ExcCosts_ListPrg> programsRepository,
                               ILinqRepository<D_ExcCosts_Events> actionsRepository,
                               INpaService npaService,
                               ITargetService targetService,
                               IAdditionalService additionalService, 
                               IExtension extension)
        {
            this.programsRepository = programsRepository;
            this.actionsRepository = actionsRepository;
            this.npaService = npaService;
            this.targetService = targetService;
            this.additionalService = additionalService;
            this.extension = extension;
        }

        public IList GetProgramsTable(
                                        bool filterTypeDCP, 
                                        bool filterTypeVCP, 
                                        bool filterUnapproved, 
                                        bool filterApproved, 
                                        bool filterRunning, 
                                        bool filterFinished)
        {
            var userCredentials = (BasePrincipal)System.Web.HttpContext.Current.User;

            if (!userCredentials.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer))
            {
                return new List<object>();
            }

            var data = from p in this.programsRepository.FindAll()
                       where p.ID > 0
                       orderby p.RefTypeProg.ID descending 
                       select new
                       {
                           ID = p.ID,
                           Name = p.Name,
                           ParentId = p.ParentID,
                           ParentName = p.ParentID == null ? String.Empty : this.programsRepository.FindOne((int)p.ParentID).Name,
                           NPAListCommaSeparated = npaService.GetNpaListCommaSeparated(p.ID),
                           BeginYearString = p.RefBegDate.GetYear().ToString(),
                           EndYearString = p.RefEndDate.GetYear().ToString(),
                           RefCreatorId = p.RefCreators.ID,
                           RefCreatorName = p.RefCreators.Name,
                           RefCreatorLogin = p.RefCreators.Login,
                           Status = (int)GetStatus(p.RefApYear, p.RefBegDate, p.RefEndDate),
                           RefTypeProgId = p.RefTypeProg.ID
                       };

            if (userCredentials.IsInRole(ProgramRoles.Creator))
            {
                data = data.Where(f => f.RefCreatorLogin == userCredentials.DbUser.Name
                                      || this.actionsRepository.FindAll().Any(x => x.RefCreators.Login == userCredentials.DbUser.Name && x.RefTask.RefGoal.RefProg.ID == f.ID));
            }
            else if (userCredentials.IsInRole(ProgramRoles.Viewer))
            {
            }
            else
            {
                throw new Exception("Неизвестная роль пользователя");
            }

            if (!filterTypeDCP)
            {
                data = data.Where(f => f.RefTypeProgId != (int)ProgramType.Longterm);
            }

            if (!filterTypeVCP)
            {
                data = data.Where(f => f.RefTypeProgId != (int)ProgramType.Department);
            }

            if (extension.OKTMO == OKTMO.Sakhalin)
            {
                data = data.Where(f => f.RefTypeProgId == (int)ProgramType.Municipal);
            }

            var result = data.ToList();

            if (!filterUnapproved)
            {
                result = result.FindAll(f => f.Status != (int)ProgramStatus.Unapproved);
            }

            if (!filterApproved)
            {
                result = result.FindAll(f => f.Status != (int)ProgramStatus.Approved);
            }
            
            if (!filterRunning)
            {
                result = result.FindAll(f => f.Status != (int)ProgramStatus.Running);
            }
            
            if (!filterFinished)
            {
                result = result.FindAll(f => f.Status != (int)ProgramStatus.Finished);
            }
            
            return result;
        }

        public IList GetParentProgramListForLookup()
        {
            var data = from p in this.programsRepository.FindAll()
                       where p.ParentID == null
                             && p.ID > 0
                       select new
                       {
                           ID = p.ID,
                           Name = p.Name
                       };
            return data.ToList();
        }

        public D_ExcCosts_ListPrg GetProgram(int id)
        {
            var entity = programsRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Программа не найдена.");
            }

            return entity;
        }

        public ProgramViewModel GetProgramModel(int id)
        {
            var data = (from p in programsRepository.FindAll()
                        where p.ID == id
                        select new ProgramViewModel
                                  {
                                      ID = p.ID,
                                      Name = p.Name,
                                      ShortName = p.ShortName,
                                      RefTypeProgId = p.RefTypeProg.ID,
                                      Creator = p.RefCreators.Name,
                                      ApproveDate = p.RefApYear == null ? String.Empty : String.Format("{0}.{1}.{2}", p.RefApYear.GetDay(), p.RefApYear.GetMonth(), p.RefApYear.GetYear()),
                                      RefBeginDateVal = p.RefBegDate.GetYear(),
                                      RefEndDateVal = p.RefEndDate.GetYear(),
                                      NpaListCommaSeparated = npaService.GetNpaListCommaSeparated(p.ID),
                                      ParentName = p.ParentID == null ? String.Empty : this.programsRepository.FindOne((int)p.ParentID).Name,
                                      ParentId = p.ParentID
                                  }).ToList();
            if (data.Count > 0)
            {
                return data.First();
            }
            else
            {
                throw new KeyNotFoundException("Программа не найдена.");
            }
        }

        public ProgramViewModel GetInitialProgramModel()
        {
            var userCredentials = (BasePrincipal)System.Web.HttpContext.Current.User;
            var creator = additionalService.GetCreator(userCredentials.DbUser.Name);

            var data = new ProgramViewModel
                           {
                               RefTypeProgId = (int)ProgramType.Longterm,
                               RefBeginDateVal = DateTime.Now.Year,
                               RefEndDateVal = DateTime.Now.Year,
                               Creator = creator.Name
                           };

            if (extension.OKTMO == OKTMO.Sakhalin)
            {
                data.RefTypeProgId = (int)ProgramType.Municipal;
            }

            return data;
        }

        public ProgramStatus GetStatus(FX_Date_YearDayUNV approveDate, FX_Date_YearDayUNV startDate, FX_Date_YearDayUNV finishDate)
        {
           if (approveDate == null)
           {
               return ProgramStatus.Unapproved;
           }
           else if (DateTime.Now.Year < startDate.GetYear())
           {
               return ProgramStatus.Approved;
           }
           else if (DateTime.Now.Year >= startDate.GetYear()
                    && DateTime.Now.Year <= finishDate.GetYear())
           {
               return ProgramStatus.Running;
           }
           else if (DateTime.Now.Year > finishDate.GetYear())
           {
               return ProgramStatus.Finished;
           }
           else
           {
               throw new Exception("Ошибка определения статуса");
           }
        }

        public void SaveProject(D_ExcCosts_ListPrg entityNew, D_ExcCosts_ListPrg entityOld)
        {
            CopyNotChangableParameters(entityOld, entityNew);

            Validate(entityNew);

            if (entityNew.ParentID == null && entityOld != null && entityOld.ParentID != null)
            {
                ClearReferenceOnChildPrograms(entityNew.ID);
            }

            this.programsRepository.Save(entityNew);
            try
            {
                this.programsRepository.DbContext.CommitChanges();
            }
            catch (Exception)
            {
                throw new Exception("Ошибка сохранения данных");
            }
        }

        public void DeleteProgram(int id)
        {
            var entity = GetProgram(id);

            // Удаление зависимостей(подчиненных записей)
            npaService.DeleteAllNpa(id);
            targetService.DeleteAllTarget(id);

            ClearReferenceOnChildPrograms(entity.ID);
            
            programsRepository.Delete(entity);
            programsRepository.DbContext.CommitChanges();
        }

        public IList<int> GetYears(int programId)
        {
            var program = GetProgram(programId);
            return program.GetYears();
        }

        public IList<int> GetYearsWithPreviousAndFollowing(int programId)
        {
            var program = GetProgram(programId);
            return program.GetYearsWithPreviousAndFollowing();
        }

        public void ApproveProgram(D_ExcCosts_ListPrg program, DateTime approvementDate)
        {
            program.RefApYear = additionalService.GetRefYearMonthDay(approvementDate.Year, approvementDate.Month, approvementDate.Day);
            programsRepository.Save(program);
            programsRepository.DbContext.CommitChanges();
        }

        /// <summary>
        /// Копирование параметров, которые не должны изменяться при некоторых условиях
        /// </summary>
        internal void CopyNotChangableParameters(D_ExcCosts_ListPrg entityOld, D_ExcCosts_ListPrg entityNew)
        {
            if (entityOld == null)
            {
                return;
            }

            // Нельзя менять некоторые параметры
            entityNew.RefApYear = entityOld.RefApYear;
            entityNew.RefCreators = entityOld.RefCreators;
        }

        private void Validate(D_ExcCosts_ListPrg entity)
        {
            // Если назначают головную задачу, то проверяем наличие родителя
            if (entity.ParentID != null)
            {
                if (entity.ParentID == entity.ID)
                {
                    throw new Exception("Программа не может ссылаться сама на себя в качестве родительской.");
                }

                var patentProgram = GetProgram((int)entity.ParentID);
                if (patentProgram.ParentID != null)
                {
                    throw new Exception("Данная программа уже имеет вложенные. Допускается только один уровень вложенности.");
                }
            }
        }

        private void ClearReferenceOnChildPrograms(int parentId)
        {
            var childPrograms = from p in programsRepository.FindAll()
                                where p.ParentID == parentId
                                select p;
            foreach (D_ExcCosts_ListPrg program in childPrograms)
            {
                program.ParentID = null;
                programsRepository.Save(program);
            }

            programsRepository.DbContext.CommitChanges();
        }
    }
}
