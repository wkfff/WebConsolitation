namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public interface IReportService
    {
        void Accept(int regionId);
        
        void Reject(int regionId);
    }
}