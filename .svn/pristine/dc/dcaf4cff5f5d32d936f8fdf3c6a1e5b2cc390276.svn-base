using System.Collections;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface ISubsidyService
    {
        IList GetSubsidyListTable(int programId);

        void InsertFile(D_ExcCosts_ListPrg program, string fileName, byte[] fileBody, string subsidyName);

        void UpdateFile(int fileId, D_ExcCosts_ListPrg program, string fileName, byte[] fileBody, string subsidyName);

        void GetFile(int fileId, out byte[] fileBody, out string fileName, out string fileMimeType);

        D_ExcCosts_Subsidy GetSubsidy(int fileId);
        
        void DeleteSubsidy(int fileId);

        void DeleteAllSubsidy(int programId);

        void UpdateSubsidyAttributes(int fileId, string subsidyName);
    }
}