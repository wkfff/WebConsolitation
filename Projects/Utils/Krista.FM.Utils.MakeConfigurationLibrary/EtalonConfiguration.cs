using System;
using System.IO;

namespace Krista.FM.Utils.MakeConfigurationLibrary
{
    /// <summary>
    /// Эталонный список пакетов для создания полной FMMD_All
    /// </summary>
    public class EtalonConfiguration
    {
        public EtalonConfiguration(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Путь к папке OLAPPackages
        /// </summary>
        public string Path { get; set; }

        public bool CreateEtalonConfiguration(PackageConfig rootPackage)
        {
            try
            {
                if (!Directory.Exists(string.Format("{0}OLAPPackages\\", Path)))
                {
                    Console.WriteLine("По указанному в параметрах пути {0}не найдено директории OLAPPackages", Path);
                    return false;
                }

                if (Path != null)
                {
                    var dir = new DirectoryInfo(string.Format("{0}OLAPPackages\\", Path));
                    ReadDirectory(dir.GetDirectories(), rootPackage);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private static void ReadDirectory(DirectoryInfo[] getDirectories, PackageConfig rootPackage)
        {
            foreach (var directory in getDirectories)
            {
                if (directory != null)
                {
                    var dir = new DirectoryInfo(directory.FullName);

                    var package = new PackageConfig(dir.Name);
                    rootPackage.Packages.Add(package);

                    ReadDirectory(dir.GetDirectories(), package);

                    FileInfo[] files = dir.GetFiles("*.xml");

                    foreach (var fileInfo in files)
                    {
                        var item = new ItemConfig(fileInfo.Name);
                        package.Items.Add(item);
                    }
                }
            }
        }
    }
}
