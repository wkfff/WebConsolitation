using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class NpaService : INpaService
    {
        private readonly ILinqRepository<D_ExcCosts_NPA> npaRepository;
        
        public NpaService(ILinqRepository<D_ExcCosts_NPA> npaRepository)
        {
            this.npaRepository = npaRepository;
        }

        public string GetNpaListCommaSeparated(int programId)
        {
            var data = from p in this.npaRepository.FindAll()
                       where p.RefProg.ID == programId
                             && !p.Amendment
                       select p.Name;
            
            var result = String.Join(", ", data.ToArray());
            return result;
        }

        public IList GetNpaListTable(int programId)
        {
            var data = from p in this.npaRepository.FindAll()
                       where p.RefProg.ID == programId
                       select new
                                  {
                                      ID = p.ID,
                                      NPAName = p.Name,
                                      Amendment = p.Amendment
                                  };
            
            return data.ToList();
        }

        public void InsertFile(D_ExcCosts_ListPrg program, string fileName, byte[] fileBody, string npaName, bool amendment)
        {
            var file = new D_ExcCosts_NPA
                           {
                               RefProg = program,
                               Amendment = amendment,
                               Doc = fileBody,
                               DocName = fileName,
                               Name = npaName
                           };

            npaRepository.Save(file);
            npaRepository.DbContext.CommitChanges();
        }

        public void UpdateFile(int fileId, D_ExcCosts_ListPrg program, string fileName, byte[] fileBody, string npaName, bool amendment)
        {
            var file = npaRepository.FindOne(fileId);
            if (file == null)
            {
                throw new KeyNotFoundException("НПА не найден");
            }

            file. RefProg = program;
            file.Amendment = amendment;
            file.Doc = fileBody;
            file.DocName = fileName;
            file.Name = npaName;

            npaRepository.Save(file);
            npaRepository.DbContext.CommitChanges();
        }

        public void GetFile(int fileId, out byte[] fileBody, out string fileName, out string fileMimeType)
        {
            var file = GetNpa(fileId);
            
            fileBody = file.Doc;
            fileName = file.DocName;
            fileMimeType = GetContentMimeType(file.DocName);
        }

        public D_ExcCosts_NPA GetNpa(int fileId)
        {
            var file = npaRepository.FindOne(fileId);
            if (file == null)
            {
                throw new KeyNotFoundException("Файл не найден");
            }

            return file;
        }

        public void DeleteNpa(int fileId)
        {
            try
            {
                var file = GetNpa(fileId);
                npaRepository.Delete(file);
                npaRepository.DbContext.CommitChanges();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        public void DeleteAllNpa(int programId)
        {
            var files = from f in npaRepository.FindAll()
                        where f.RefProg.ID == programId
                        select f;
            foreach (var entity in files)
            {
                npaRepository.Delete(entity);
            }

            if (files.Count() > 0)
            {
                npaRepository.DbContext.CommitChanges();
            }
        }

        public void UpdateNpaAttributes(int fileId, string npaName, bool amendment)
        {
            if (String.IsNullOrEmpty(npaName))
            {
                throw new ArgumentNullException("Не заполнено название НПА");
            }

            var entity = GetNpa(fileId);
            entity.Name = npaName;
            entity.Amendment = amendment;
            npaRepository.Save(entity);
            npaRepository.DbContext.CommitChanges();
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
