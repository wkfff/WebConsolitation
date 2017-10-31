using System;
using System.IO;
using Krista.FM.Update.Framework.Utils;

namespace Krista.FM.Update.Framework.Conditions
{
    [Serializable]
    public class FileChecksumCondition : UpdateCondition
    {
        #region IUpdateCondition Members

        public override bool IsMet(IUpdateTask task)
        {
            string localPath = string.Empty;
            if (Attributes.ContainsKey("localPath"))
                localPath = Attributes["localPath"];
            else if (task != null && task.Attributes.ContainsKey("localPath"))
                localPath = task.Attributes["localPath"];

            localPath = ConditionHelper.GetLocalPath(task, localPath);

            if (!File.Exists(localPath))
                return true;

            if (Attributes.ContainsKey("sha256-checksum"))
            {
                string sha256 = FileChecksum.GetSHA256Checksum(localPath);
                if (!string.IsNullOrEmpty(sha256) && !sha256.Equals(Attributes["sha256-checksum"]))
                    return true;
            }

            // TODO: Support more checksum algorithms (although SHA256 has no collisions, other are more commonly used)

            return false;
        }

        #endregion
    }
}
