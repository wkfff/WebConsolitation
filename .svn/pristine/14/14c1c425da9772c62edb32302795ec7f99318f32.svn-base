using System.Data;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Services
{
    public interface IFilesService
    {
        DataTable GetFileTable(int refProjId);

        void GetFile(int fileId, out byte[] fileBody, out string fileName, out string fileMimeType);
        
        D_InvProject_Vizual GetFile(int fileId);

        void InsertFile(int refProjId, string fileName, byte[] fileBody);

        void UpdateFile(int fileId, int refProjId, string fileName, byte[] fileBody);

        void UpdateFileName(int fileId, string fileName);

        void DeleteFile(int fileId);
    }
}
