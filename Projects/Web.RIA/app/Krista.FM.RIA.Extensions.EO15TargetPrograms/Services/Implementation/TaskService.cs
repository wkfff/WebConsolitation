using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class TaskService : ITaskService
    {
        private readonly ILinqRepository<D_ExcCosts_Tasks> taskRepository;
        private readonly IActionService actionService;
        private readonly ITargetRatingService targetRatingService;

        public TaskService(
                           ILinqRepository<D_ExcCosts_Tasks> taskRepository,
                           IActionService actionService,
                           ITargetRatingService targetRatingService)
        {
            this.taskRepository = taskRepository;
            this.actionService = actionService;
            this.targetRatingService = targetRatingService;
        }

        public IList GetTasksTable(int programId)
        {
            var data = from p in this.taskRepository.FindAll()
                       where p.RefGoal.RefProg.ID == programId
                       select new
                                  {
                                      ID = p.ID,
                                      TaskName = p.Name,
                                      TaskNote = p.Note,
                                      TargetName = p.RefGoal.Name,
                                      TargetId = p.RefGoal.ID
                                  };
            
            return data.ToList();
        }

        public IList GetTasksTableForLookup(int programId)
        {
            var data = from p in this.taskRepository.FindAll()
                       where p.RefGoal.RefProg.ID == programId
                       select new
                       {
                           ID = p.ID,
                           Name = p.Name
                       };

            return data.ToList();
        }

        public void Create(int programId, D_ExcCosts_Goals target, string taskName, string taskNote)
        {
            if (target.RefProg.ID != programId)
            {
                throw new InvalidConstraintException("Указанная цель не относится к данной программе");
            } 

            var entity = new D_ExcCosts_Tasks
                             {
                                 Name = taskName,
                                 Note = taskNote,
                                 RefGoal = target
                             };
            CheckEntity(entity);
            this.taskRepository.Save(entity);
            this.taskRepository.DbContext.CommitChanges();
        }

        public void Update(int programId, D_ExcCosts_Goals target, int taskId, string taskName, string taskNote)
        {
            var entity = GetTask(taskId);
            
            if (entity.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Задача относится к другой программе!");
            }

            if (target.RefProg.ID != programId)
            {
                throw new Exception("Программа у цели и у задачи различаются!");
            }

            entity.Name = taskName;
            entity.Note = taskNote;
            entity.RefGoal = target;
            CheckEntity(entity);
            this.taskRepository.Save(entity);
            this.taskRepository.DbContext.CommitChanges();
        }

        public void Delete(int taskId, int programId)
        {
            D_ExcCosts_Tasks entity;

            try
            {
                entity = GetTask(taskId);
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            if (entity.RefGoal.RefProg.ID != programId)
            {
                throw new Exception("Задача относится к другой программе!");
            }

            this.taskRepository.Delete(entity);
            this.taskRepository.DbContext.CommitChanges();
        }

        public D_ExcCosts_Tasks GetTask(int id)
        {
            var entity = this.taskRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Задача не найдена");
            }

            return entity;
        }

        public void DeleteAllTask(int targetId)
        {
            var tasks = from f in taskRepository.FindAll()
                       where f.RefGoal.ID == targetId
                      select f;
            foreach (var entity in tasks)
            {
                actionService.DeleteAllAction(entity.ID);
                targetRatingService.DeleteAllRate(entity.ID);
                taskRepository.Delete(entity);
            }

            taskRepository.DbContext.CommitChanges();
        }

        private void CheckEntity(D_ExcCosts_Tasks entity)
        {
            if (String.IsNullOrEmpty(entity.Name))
            {
                throw new Exception("Поле <Название задачи> не может быть пустым");
            }
        }
    }
}
