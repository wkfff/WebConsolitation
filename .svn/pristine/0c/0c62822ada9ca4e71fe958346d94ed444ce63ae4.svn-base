using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Utils.DTSGenerator
{
    /// <summary>
    /// Переносимый пакет
    /// </summary>
    public class DtsxPackage
    {
        private string fileName;

        public DtsxPackage(string fileName)
        {
            this.fileName = fileName;
        }

        public override string ToString()
        {
            return BatWriter.PackageTask(fileName);
        }

        /// <summary>
        /// Имя пакета для переноса
        /// </summary>
        public string Name
        {
            get { return fileName.Split('.')[0]; }
        }
    }
}
