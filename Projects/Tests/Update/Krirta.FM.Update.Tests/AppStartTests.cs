using System;
using System.IO;
using NUnit.Framework;

namespace Krirta.FM.Update.Tests
{
    [TestFixture]
    public class AppStartTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BacupLocalFile_test_null_bacup_folder()
        {
            string backupFolder = null;
            string localName = "localName";
            string appFolder = "appFolder";

            Krista.FM.Update.ColdUpdater.AppStart.BacupLocalFile(backupFolder, localName, appFolder);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BacupLocalFile_test_null_localName()
        {
            string backupFolder = "backupFolder";
            string localName = null;
            string appFolder = "appFolder";

            Krista.FM.Update.ColdUpdater.AppStart.BacupLocalFile(backupFolder, localName, appFolder);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BacupLocalFile_test_null_app_folder()
        {
            string backupFolder = "bacupFolder";
            string localName = "localName";
            string appFolder = null;

            Krista.FM.Update.ColdUpdater.AppStart.BacupLocalFile(backupFolder, localName, appFolder);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BackupLocalFile_test_long_backup_folder()
        {
            string backupFolder =
                @"c:\Program Files\Krista\FM\Krista.FM.Client\Workplace\Program Files\Krista\FM\Krista.FM.Client\Workplace\Program Files\Krista\FM\Krista.FM.Client\Workplace\Program Files\Krista\FM\Krista.FM.Client\Workplace\Program Files\Krista\FM\Krista.FM.Client\Workplace\111\";
            string localName = "localName";
            string appFolder = "appFolder";

            Krista.FM.Update.ColdUpdater.AppStart.BacupLocalFile(backupFolder, localName, appFolder);
        }

        [Test]
        public void BackupLocalFile_test_invalid_bacup_folder_name()
        {
            string backupFolder =
                @"c:\Program Files\<";
            string localName = "localName";
            string appFolder = "appFolder";

            bool i = Krista.FM.Update.ColdUpdater.AppStart.BacupLocalFile(backupFolder, localName, appFolder);

            Assert.AreEqual(i, false);
        }

        [Test]
        public void BackupLocalFile_test_invalid_app_folder_name()
        {
            string backupFolder ="backup";
            string localName = "localName";
            string appFolder = @"c:\Program Files\<";

            bool i = Krista.FM.Update.ColdUpdater.AppStart.BacupLocalFile(backupFolder, localName, appFolder);

            Assert.AreEqual(i, false);
        }

        [Test]
        public void BackupLocalFile_test_copy_file()
        {
            DirectoryInfo tempFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "Tsvetkov"));
            string fileName = Path.Combine(tempFolder.FullName, "1.txt");
            if (!File.Exists(fileName))
            {
                FileStream fileStream = File.Create(fileName);
                fileStream.Dispose();
            }

            string backupFolder = Path.Combine(Path.GetTempPath(), "Tsvetkov2");
            string localName = "1.txt";
            string appFolder = tempFolder.FullName;

            bool isCopy = Krista.FM.Update.ColdUpdater.AppStart.BacupLocalFile(backupFolder, localName, appFolder);

            Assert.AreEqual(isCopy, true);
            Assert.AreEqual(File.Exists(Path.Combine(backupFolder, localName)), true);

            Directory.Delete(tempFolder.FullName, true);
            Directory.Delete(backupFolder, true);
        }
    }
}
