using System;

namespace Krista.FM.Update.Framework.Sources
{
    public class MemorySource : IUpdateSource
    {
        public MemorySource(string feedXml)
        {
            this.FeedXml = feedXml;
        }

        public string FeedXml { get; set; }

        #region IUpdateSource Members

        public string GetUpdatesFeed()
        {
            return FeedXml;
        }

        public string GetUpdatesFeed(string feedPath)
        {
            return String.Empty;
        }

        public bool GetData(string filePath, string basePath, ref string tempFile, bool needAddPostfix)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
