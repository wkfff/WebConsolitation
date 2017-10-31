using System;
using System.Web;
using System.Web.Caching;

namespace Krista.FM.RIA.Core.Progress
{
    public class AspnetProgressProvider : IProgressDataProvider
    {
        public void Set(string taskId, ProgressState progress, int durationInSeconds = 5)
        {
            HttpContext.Current.Cache.Insert(
                taskId,
                progress,
                null,
                DateTime.Now.AddSeconds(durationInSeconds),
                Cache.NoSlidingExpiration);
        }

        public ProgressState Get(string taskId)
        {
            var o = HttpContext.Current.Cache[taskId];
            if (o == null)
            {
                return null;
            }

            return (ProgressState)o;
        }
    }
}
