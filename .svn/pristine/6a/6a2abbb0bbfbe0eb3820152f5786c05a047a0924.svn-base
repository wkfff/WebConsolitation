using System;
using System.Diagnostics;
using System.IO;

namespace Krista.FM.Update.Framework.Conditions
{
    public static class ConditionHelper
    {
        public static string GetLocalPath(IUpdateTask task, string localPath)
        {
            try
            {
                if (UpdateManager.Instance.IsServerMode && UpdateManager.InstallerFeed.Equals(task.Owner.Feed.Name))
                    return Path.Combine(Path.GetDirectoryName(UpdateManager.GetProcessModule().FileName), localPath);

                if (UpdateManager.Instance.IsServerMode && !UpdateManager.InstallerFeed.Equals(task.Owner.Feed.Name))
                    localPath =
                        Path.Combine(
                            Path.Combine(Path.Combine(UpdateManager.Instance.DestBaseUrl, task.Owner.BaseUrl),
                                         task.Owner.Name), localPath);
                return Path.Combine(UpdateManager.Instance.DestBaseUrl, localPath);
            }
            catch (ArgumentException e)
            {
                Trace.TraceError(e.Message);
                return String.Empty;
            }
            catch (NotSupportedException e)
            {
                Trace.TraceError(e.Message);
                return string.Empty;
            }
        }
    }
}
