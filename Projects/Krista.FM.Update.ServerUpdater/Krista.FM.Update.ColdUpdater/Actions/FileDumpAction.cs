using System;
using System.IO;

namespace Krista.FM.Update.ColdUpdater.Actions
{
    public class FileDumpAction : IUpdateAction
    {
        private readonly string filePath;
        private readonly byte[] fileData;

        public FileDumpAction(string path, byte[] data)
        {
            this.filePath = path;
            this.fileData = data;
        }

        #region IUpdateAction Members

        public bool Do()
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fileData, 0, fileData.Length);
                }
            }
            catch { return false; }
            return true;
        }

        public void Rollback(string backupPath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
