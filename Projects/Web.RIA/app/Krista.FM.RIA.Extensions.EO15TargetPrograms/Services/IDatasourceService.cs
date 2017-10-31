namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public interface IDatasourceService
    {
        int GetDefaultDatasourceId();

        void CreateDefaultDatasource();

        int GetFactDatasourceId();

        void CreateFactDatasource();

        int GetCriteriasSourceId(ProgramStage stage);

        void CreateCriteriasDatasource(ProgramStage stage);
    }
}