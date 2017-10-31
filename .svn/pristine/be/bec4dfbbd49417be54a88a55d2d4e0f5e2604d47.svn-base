using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class TargetService : ITargetService
    {
        private readonly ILinqRepository<D_ExcCosts_Goals> targetRepository;
        private readonly ITaskService taskService;

        public TargetService(
                             ILinqRepository<D_ExcCosts_Goals> targetRepository,
                             ITaskService taskService)
        {
            this.targetRepository = targetRepository;
            this.taskService = taskService;
        }

        public IList GetTargetsTable(int programId)
        {
            var data = from p in this.targetRepository.FindAll()
                       where p.RefProg.ID == programId
                       select new
                                  {
                                      ID = p.ID,
                                      Name = p.Name,
                                      Note = p.Note
                                  };
            
            return data.ToList();
        }

        public IList GetTargetsTableForLookup(int programId)
        {
            var data = from p in this.targetRepository.FindAll()
                       where p.RefProg.ID == programId
                       select new
                       {
                           ID = p.ID,
                           Name = p.Name
                       };

            return data.ToList();
        }

        public void Create(D_ExcCosts_ListPrg program, string name, string note)
        {
            var entity = new D_ExcCosts_Goals
                             {
                                 Name = name,
                                 Note = note,
                                 RefProg = program
                             };
            CheckEntity(entity);
            this.targetRepository.Save(entity);
            this.targetRepository.DbContext.CommitChanges();
        }

        public void Update(D_ExcCosts_ListPrg program, int targetId, string name, string note)
        {
            var entity = GetTarget(targetId);
            
            if (entity.RefProg.ID != program.ID)
            {
                throw new Exception("Цель относится к другой программе!");
            }

            entity.Name = name;
            entity.Note = note;
            CheckEntity(entity);
            this.targetRepository.Save(entity);
            this.targetRepository.DbContext.CommitChanges();
        }

        public void Delete(int targetId, int programId)
        {
            D_ExcCosts_Goals entity;

            try
            {
                entity = GetTarget(targetId);
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            if (entity.RefProg.ID != programId)
            {
                throw new Exception("Цель относится к другой программе!");
            }

            this.targetRepository.Delete(entity);
            this.targetRepository.DbContext.CommitChanges();
        }

        public D_ExcCosts_Goals GetTarget(int id)
        {
            var entity = targetRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Цель не найдена.");
            }

            return entity;
        }

        public void DeleteAllTarget(int programId)
        {
            var targets = from f in targetRepository.FindAll()
                         where f.RefProg.ID == programId
                        select f;
            foreach (var entity in targets)
            {
                taskService.DeleteAllTask(entity.ID); 
                targetRepository.Delete(entity);
            }
            
            targetRepository.DbContext.CommitChanges();
        }

        private void CheckEntity(D_ExcCosts_Goals entity)
        {
            if (String.IsNullOrEmpty(entity.Name))
            {
                throw new ArgumentNullException("Поле Name не может быть пустым");
            }
        }
    }
}
