using System.Data;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Servises
{
    public interface IFilesService
    {
        DataTable GetFileTable(int areaId);

        void GetFile(int fileId, out byte[] fileBody, out string fileName, out string fileMimeType);

        D_InvArea_Visual GetFile(int fileId);

        void InsertFile(int areaId, string fileName, byte[] fileBody);

        void UpdateFile(int fileId, int areaId, string fileName, byte[] fileBody);

        void UpdateFileName(int fileId, string fileName);

        void DeleteFile(int fileId);
    }
}
