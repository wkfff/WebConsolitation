namespace Krista.FM.RIA.Core.Progress
{
    public interface IProgressDataProvider
    {
        void Set(string taskId, ProgressState progress, int durationInSeconds = 300);

        ProgressState Get(string taskId);
    }
}
