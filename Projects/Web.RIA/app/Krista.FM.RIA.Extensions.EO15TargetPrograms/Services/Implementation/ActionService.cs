using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class ActionService : IActionService
    {
        private readonly ILinqRepository<D_ExcCosts_Events> actionRepository;

        public ActionService(ILinqRepository<D_ExcCosts_Events> actionRepository)
        {
            this.actionRepository = actionRepository;
        }

        public IList GetActionsTable(int programId)
        {
            var data = from p in this.actionRepository.FindAll()
                       where p.RefTask.RefGoal.RefProg.ID == programId
                       select new
                                  {
                                      ID = p.ID,
                                      TaskName = p.RefTask.Name,
                                      TaskId = p.RefTask.ID,
                                      ActionName = p.Name,
                                      ActionOwnerName = p.RefCreators.Name,
                                      ActionOwnerId = p.RefCreators.ID,
                                      ActionResults = p.Results,
                                      ActionNote = p.Note
                                  };
            return data.ToList();
        }

        public IList GetActionsTableForLookup(int programId)
        {
            var data = from p in this.actionRepository.FindAll()
                       where p.RefTask.RefGoal.RefProg.ID == programId
                       select new
                       {
                           ID = p.ID,
                           Name = p.Name
                       };
            return data.ToList();
        }

        public void Create(int programId, D_ExcCosts_Tasks task, string actionName, string actionNote, string actionResult, D_ExcCosts_Creators owner)
        {
            var entity = new D_ExcCosts_Events
                             {
                                 Name = actionName,
                                 Note = actionNote,
                                 Results = actionResult,
                                 RefTask = task,
                                 RefCreators = owner
                             };
            CheckEntity(entity);
            this.actionRepository.Save(entity);
            this.actionRepository.DbContext.CommitChanges();
        }

        public void Update(int programId, int actionId, D_ExcCosts_Tasks task, string actionName, string actionNote, string actionResult, D_ExcCosts_Creators owner)
        {
            var entity = GetAction(actionId);

            if (entity.RefTask.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Мероприятие относится к другой программе!");
            }

            if (task.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Задача относится к другой программе!");
            }

            entity.Name = actionName;
            entity.Note = actionNote;
            entity.Results = actionResult;
            entity.RefTask = task;
            entity.RefCreators = owner;
            CheckEntity(entity);
            this.actionRepository.Save(entity);
            this.actionRepository.DbContext.CommitChanges();
        }

        public void Delete(int actionId, int programId)
        {
            D_ExcCosts_Events entity;

            try
            {
                entity = GetAction(actionId);
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            if (entity.RefTask.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Мероприятие относится к другой программе!");
            }

            this.actionRepository.Delete(entity);
            this.actionRepository.DbContext.CommitChanges();
        }

        public D_ExcCosts_Events GetAction(int id)
        {
            var entity = actionRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Мероприятие не найдено.");
            }

            return entity;
        }

        public void DeleteAllAction(int taskId)
        {
            var actions = from f in actionRepository.FindAll()
                         where f.RefTask.ID == taskId
                        select f;
            foreach (var entity in actions)
            {
                actionRepository.Delete(entity);
            }

            try
            {
                actionRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления мероприятий", e);
            }
        }

        public bool IsUserInActionsOwners(D_ExcCosts_ListPrg program, string userLogin)
        {
            var actionsCount = actionRepository.FindAll()
                                               .Where(t => t.RefTask.RefGoal.RefProg == program
                                                          && t.RefCreators.Login == userLogin)
                                               .Count();
            if (actionsCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CheckEntity(D_ExcCosts_Events entity)
        {
            if (String.IsNullOrEmpty(entity.Name))
            {
                throw new ArgumentNullException("Поле Name не может быть пустым");
            }
        }
    }
}
