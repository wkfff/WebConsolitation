using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public class FilesService : IFilesService
    {
        private readonly ILinqRepository<D_InvProject_Vizual> filesRepository;
        private readonly IProjectService projectService;

        public FilesService(
                                     ILinqRepository<D_InvProject_Vizual> filesRepository,
                                     IProjectService projectService)
        {
            this.filesRepository = filesRepository;
            this.projectService = projectService;
        }

        public DataTable GetFileTable(int refProjId)
        {
            DataTable table = new DataTable();

            // Инициализируем поля таблицы
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("FileType", typeof(string));
            table.Columns.Add("FileName", typeof(string));
            
            var files = from f in filesRepository.FindAll()
                        where f.RefReestr.ID == refProjId
                        select f;
            foreach (var file in files)
            {
                DataRow row = table.NewRow();
                row["ID"] = file.ID;
                row["FileType"] = GetFileType(file.Name);
                row["FileName"] = file.Name;
                table.Rows.Add(row);
            }

            return table;
        }

        public void GetFile(int fileId, out byte[] fileBody, out string fileName, out string fileMimeType)
        {
            var file = GetFile(fileId);
            fileBody = file.Document;
            fileName = file.Name;
            fileMimeType = GetContentMimeType(file.Name);
        }

        public D_InvProject_Vizual GetFile(int fileId)
        {
            var file = filesRepository.FindOne(fileId);
            if (file == null)
            {
                throw new KeyNotFoundException("Файл не найден");
            }

            return file;
        }

        public void InsertFile(int refProjId, string fileName, byte[] fileBody)
        {
            var file = new D_InvProject_Vizual();
            file.RefReestr = projectService.GetProject(refProjId);
            file.Name = fileName;
            file.Document = fileBody;

            filesRepository.Save(file);
            filesRepository.DbContext.CommitChanges();
        }

        public void UpdateFile(int fileId, int refProjId, string fileName, byte[] fileBody)
        {
            var file = GetFile(fileId);
            file.RefReestr = projectService.GetProject(refProjId);
            file.Name = fileName;
            file.Document = fileBody;

            filesRepository.Save(file);
            filesRepository.DbContext.CommitChanges();
        }

        public void DeleteFile(int fileId)
        {
            try
            {
                var file = GetFile(fileId);
                filesRepository.Delete(file);
                filesRepository.DbContext.CommitChanges();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        public void UpdateFileName(int fileId, string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("Не заполнено имя файла");
            }

            var file = GetFile(fileId);
            file.Name = fileName;
            filesRepository.Save(file);
            filesRepository.DbContext.CommitChanges();
        }

        private string GetFileType(string fileName)
        {
            switch (Path.GetExtension(fileName.ToLower()))
            {
                case ".jpg": return "фото";
                case ".jpeg": return "фото";
                case ".bmp": return "фото";
                case ".tiff": return "фото";
                case ".gif": return "фото";

                case ".avi": return "видео";
                case ".mpeg": return "видео";
                case ".mpg": return "видео";
                case ".3gp": return "видео";
                case ".mp4": return "видео";
                
                case ".mp3": return "аудио";
                case ".ac3": return "аудио";
                case ".wmf": return "аудио";
                case ".wma": return "аудио";
                
                case ".rar": return "архив";
                case ".zip": return "архив";
                
                default: return "документ";
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
