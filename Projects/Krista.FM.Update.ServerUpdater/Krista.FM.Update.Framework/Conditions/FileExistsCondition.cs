using System;

namespace Krista.FM.Update.Framework.Conditions
{
    [Serializable]
    public sealed class FileExistsCondition : UpdateCondition
    {
        #region IUpdateCondition Members

        public override bool IsMet(IUpdateTask task)
        {
            string localPath = string.Empty;
            if (Attributes.ContainsKey("localPath"))
            {
                localPath = Attributes["localPath"];
            }
            else if (task != null && task.Attributes.ContainsKey("localPath"))
            {
                localPath = task.Attributes["localPath"];
            }

            localPath = ConditionHelper.GetLocalPath(task, localPath);

            return System.IO.File.Exists(localPath);
        }

        #endregion
    }
}
