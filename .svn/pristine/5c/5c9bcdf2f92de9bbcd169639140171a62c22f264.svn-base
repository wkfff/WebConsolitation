using System;
using System.IO;

namespace Krista.FM.Update.Framework.Conditions
{
    [Serializable]
    public class FileSizeCondition : UpdateCondition
    {
        #region IUpdateCondition Members

        public override bool IsMet(IUpdateTask task)
        {
            long fileLength = 0;
            if (!Attributes.ContainsKey("size") || !long.TryParse(Attributes["size"], out fileLength))
                return true;

            string localPath = string.Empty;
            if (Attributes.ContainsKey("localPath"))
                localPath = Attributes["localPath"];
            else if (task != null && task.Attributes.ContainsKey("localPath"))
                localPath = task.Attributes["localPath"];

            if (!File.Exists(localPath))
                return true;

            FileInfo fi = new FileInfo(localPath);

            if (Attributes.ContainsKey("what"))
            {
                switch (Attributes["what"])
                {
                    case "above":
                        return fileLength < fi.Length;
                    case "is":
                        return fileLength == fi.Length;
                }
            }
            return fileLength > fi.Length;
        }

        #endregion
    }
}
