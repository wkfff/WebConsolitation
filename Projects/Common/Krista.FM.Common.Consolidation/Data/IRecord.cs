namespace Krista.FM.Common.Consolidation.Data
{
    public interface IRecord
    {
        int MetaRowId { get; }

        ReportDataRecordState State { get; }

        object Value { get; }

        bool IsMultiplicity();

        object Get(string column);

        void Set(string column, object value);

        void AssignRecord(object assignRecord);

        void Delete();
    }
}
