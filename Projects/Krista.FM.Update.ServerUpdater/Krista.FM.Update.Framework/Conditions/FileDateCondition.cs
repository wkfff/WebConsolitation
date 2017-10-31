using System;
using System.IO;

namespace Krista.FM.Update.Framework.Conditions
{
    [Serializable]
    public class FileDateCondition : UpdateCondition
    {
        #region IUpdateCondition Members

        public override bool IsMet(IUpdateTask task)
        {
            DateTime fileDateTime;
            if (!Attributes.ContainsKey("timestamp") || !DateTime.TryParse(Attributes["timestamp"], out fileDateTime))
                return true;

            string localPath = string.Empty;
            if (Attributes.ContainsKey("localPath"))
                localPath = Attributes["localPath"];
            else if (task != null && task.Attributes.ContainsKey("localPath"))
                localPath = task.Attributes["localPath"];

            if (!File.Exists(localPath))
                return true;

            DateTime localFileDateTime = File.GetLastWriteTime(localPath);
            if (Attributes.ContainsKey("what"))
            {
                switch (Attributes["what"])
                {
                    case "newer":
                        return localFileDateTime > fileDateTime;
                    case "is":
                        return localFileDateTime.Equals(fileDateTime);
                }
            }

            return localFileDateTime < fileDateTime; // == what="older"
        }

        #endregion
    }
}
