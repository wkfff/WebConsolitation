using System.Runtime.InteropServices;

namespace Krista.FM.PlaningProviderCOMWrapper 
{
    [Guid("8E723ADA-D71E-39C6-A370-7EF57EF1F29F"), ComVisible(true)]
	public interface IPlaningProviderCOMWrapper
    {
        bool Connect(string serverNameNPort, string userName, string password,
                     int authType, bool withinTaskContext, ref string errStr);

        void Disconnect();

        bool Connected { get; }

        string GetSchemeName(string providerId);

        string Writeback(string data);

        object GetObjectRecordset(string objectName, string filter);

        string GetMetadataDate();

        string GetMetaData();

        string GetMembers(string providerId, string cubeName, string dimensionName, string hierarchyName, string levelNames, string memberPropertiesNames);
        
        string GetRecordsetData(string providerId, string queryText);

        string GetCellsetData(string providerId, string queryText);

        void RefreshDimension(string providerId, string[] names);

        void RefreshCube(string providerId, string[] names);

        string RefreshMetaData();

        string UpdateTaskParams(int taskId, string paramsText, string sectionDivider, string valuesDivider);

        string UpdateTaskConsts(int taskId, string constsText, string sectionDivider, string valuesDivider);

        string GetTaskContext(int taskId);

        void Dispose();
    }

    

    
}
