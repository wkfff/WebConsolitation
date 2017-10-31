using System.IO;

namespace Krista.FM.Update.Framework
{
    public static class Extensions
    {
        /// <summary>
        /// Рекурсивное создание каталога
        /// </summary>
        /// <param name="dirInfo">Каталог, который требуется создать</param>
        public static void CreateDirectory(this DirectoryInfo dirInfo)
        {
            if (dirInfo.Parent != null)
            {
                CreateDirectory(dirInfo.Parent);
            }

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }
    }
}
