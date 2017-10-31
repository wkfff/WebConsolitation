using System;
using System.Diagnostics;
using System.IO;

namespace Krista.FM.Update.Framework.Conditions
{
    [Serializable]
    public class FileVersionCondition : UpdateCondition
    {
        #region IUpdateCondition Members

        public override bool IsMet(IUpdateTask task)
        {
            if (!Attributes.ContainsKey("version"))
                return true;

            string localPath = string.Empty;
            if (Attributes.ContainsKey("localPath"))
                localPath = Attributes["localPath"];
            else if (task != null && task.Attributes.ContainsKey("localPath"))
                localPath = task.Attributes["localPath"];

            localPath = ConditionHelper.GetLocalPath(task, localPath);
            
            if (!File.Exists(localPath))
                return true;

            string versionString = String.Format("{0}.{1}.{2}.{3}",
                                        FileVersionInfo.GetVersionInfo(localPath).FileMajorPart,
                                        FileVersionInfo.GetVersionInfo(localPath).FileMinorPart,
                                        FileVersionInfo.GetVersionInfo(localPath).FileBuildPart,
                                        FileVersionInfo.GetVersionInfo(localPath).FilePrivatePart);

            Version localVersion = new Version(versionString);
            Version updateVersion = new Version(Attributes["version"]);

            /*Trace.TraceVerbose(string.Format(
                "localPath = {0}; Версия локального файла {1};Версия файла в патче {2}", localPath, localVersion,
                updateVersion));*/

            if (Attributes.ContainsKey("what"))
            {
                switch (Attributes["what"])
                {
                    case "above":
                        return updateVersion < localVersion;
                    case "is":
                        return updateVersion == localVersion;
                    case "is-above":
                        return updateVersion <= localVersion;
                    case "is-below":
                        return updateVersion >= localVersion;
                }
            }

            return updateVersion > localVersion;
        }

        #endregion
    }
}
