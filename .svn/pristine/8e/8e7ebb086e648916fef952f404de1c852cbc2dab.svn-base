using System;
using Krista.FM.Update.Framework.Utils;
using NUnit.Framework;

namespace Krirta.FM.Update.Tests
{
    [TestFixture]
    public class FileDownloaderTests
    {
        [Test]
        public void Should_be_able_to_download_a_small_file_from_the_internet()
        {
            FileDownloader fileDownloader = new FileDownloader("", "", "", "");
            Uri _uri = new Uri(@"http://www.krista.ru/sites/all/themes/kristageneral/img/headerUp.png");
            byte[] fileData = fileDownloader.Download(_uri);
            Assert.IsTrue(fileData.Length > 0);
        }

        [Test]
        public void Should_be_able_to_download_a_small_file_from_the_internet_to_file()
        {
            FileDownloader fileDownloader = new FileDownloader("", "", "", "");
            Uri _uri = new Uri(@"http://www.krista.ru/sites/all/themes/kristageneral/img/headerUp.png");
            bool downloadToFile = fileDownloader.DownloadToFile(@"c:\1.png",_uri, true);
            Assert.IsTrue(downloadToFile);
        }
    }
}
