using System.Collections;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface INpaService
    {
        string GetNpaListCommaSeparated(int programId);

        IList GetNpaListTable(int programId);

        void InsertFile(D_ExcCosts_ListPrg program, string fileName, byte[] fileBody, string npaName, bool amendment);

        void UpdateFile(int fileId, D_ExcCosts_ListPrg program, string fileName, byte[] fileBody, string npaName, bool amendment);

        void GetFile(int fileId, out byte[] fileBody, out string fileName, out string fileMimeType);

        D_ExcCosts_NPA GetNpa(int fileId);
        
        void DeleteNpa(int fileId);

        void DeleteAllNpa(int programId);

        void UpdateNpaAttributes(int fileId, string npaName, bool amendment);
    }
}