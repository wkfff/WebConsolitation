using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class SubsidyService : ISubsidyService
    {
        private readonly ILinqRepository<D_ExcCosts_Subsidy> subsidyRepository;

        public SubsidyService(ILinqRepository<D_ExcCosts_Subsidy> subsidyRepository)
        {
            this.subsidyRepository = subsidyRepository;
        }

        public IList GetSubsidyListTable(int programId)
        {
            var data = from p in this.subsidyRepository.FindAll()
                       where p.RefProg.ID == programId
                       select new
                                  {
                                      ID = p.ID,
                                      SubsidyName = p.Name
                                  };
            
            return data.ToList();
        }

        public void InsertFile(D_ExcCosts_ListPrg program, string fileName, byte[] fileBody, string subsidyName)
        {
            var file = new D_ExcCosts_Subsidy
                           {
                               RefProg = program,
                               Doc = fileBody,
                               DocName = fileName,
                               Name = subsidyName
                           };

            CheckEntity(file);

            subsidyRepository.Save(file);
            subsidyRepository.DbContext.CommitChanges();
        }

        public void UpdateFile(int fileId, D_ExcCosts_ListPrg program, string fileName, byte[] fileBody, string subsidyName)
        {
            var file = GetSubsidy(fileId);
            
            file.RefProg = program;
            file.Doc = fileBody;
            file.DocName = fileName;
            file.Name = subsidyName;
            
            CheckEntity(file);
            
            subsidyRepository.Save(file);
            subsidyRepository.DbContext.CommitChanges();
        }

        public void GetFile(int fileId, out byte[] fileBody, out string fileName, out string fileMimeType)
        {
            var file = GetSubsidy(fileId);
            
            fileBody = file.Doc;
            fileName = file.DocName;
            fileMimeType = GetContentMimeType(file.DocName);
        }

        public D_ExcCosts_Subsidy GetSubsidy(int fileId)
        {
            var entity = subsidyRepository.FindOne(fileId);
            if (entity == null)
            {
                throw new KeyNotFoundException("Субсидия не найдена");
            }

            return entity;
        }

        public void DeleteSubsidy(int fileId)
        {
            try
            {
                var file = GetSubsidy(fileId);
                subsidyRepository.Delete(file);
                subsidyRepository.DbContext.CommitChanges();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        public void DeleteAllSubsidy(int programId)
        {
            var files = from f in subsidyRepository.FindAll()
                        where f.RefProg.ID == programId
                        select f;
            foreach (var entity in files)
            {
                subsidyRepository.Delete(entity);
            }

            if (files.Count() > 0)
            {
                subsidyRepository.DbContext.CommitChanges();
            }
        }

        public void UpdateSubsidyAttributes(int fileId, string subsidyName)
        {
            var entity = GetSubsidy(fileId);
            entity.Name = subsidyName;
            
            CheckEntity(entity);
            
            subsidyRepository.Save(entity);
            subsidyRepository.DbContext.CommitChanges();
        }

        private void CheckEntity(D_ExcCosts_Subsidy entity)
        {
            if (String.IsNullOrEmpty(entity.Name))
            {
                throw new ArgumentNullException("Не заполнено наименование субсидии");
            }
        }

        private string GetContentMimeType(string fileName)
        {
            switch (Path.GetExtension(fileName))
            {
                case ".doc": return "application/msword";
                case ".dot": return "application/msword";
                case ".xls": return "application/vnd.ms-excel";
                case ".xla": return "application/vnd.ms-excel";
                case ".xlt": return "application/vnd.ms-excel";
                case ".xlc": return "application/vnd.ms-excel";
                case ".xlm": return "application/vnd.ms-excel";
                case ".xlw": return "application/vnd.ms-excel";
                case ".txt": return "text/plain";
                default: return "application/octet-stream";
            }
        }
    }
}
