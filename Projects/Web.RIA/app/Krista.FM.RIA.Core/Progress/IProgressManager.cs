namespace Krista.FM.RIA.Core.Progress
{
    public interface IProgressManager
    {
        void SetCompleted(double percentage, string taskId = null);

        void SetCompleted(string step, string taskId = null);

        void SetCompleted(string step, double percentage, string taskId = null);

        ProgressState GetStatus(string taskId);
    }
}
