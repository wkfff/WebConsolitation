using System.Collections.Generic;
using Krista.FM.Update.Framework;
using Krista.FM.Update.Framework.FeedReaders;
using NUnit.Framework;

namespace Krirta.FM.Update.Tests
{
    [TestFixture]
    public class XmlFeedReaderTest
    {
        const string testFeed =
               @"<?xml version=""1.0"" encoding=""utf-8""?>
                <Patches>
                    <Patch objectKey=""da55b42c-279f-4364-b7a2-0702d94d40e2"" name=""Патчик"" baseUrl=""http://tsvetkov:86/UI/Client/SchemeDesigner/2/"" description=""Описание патча"" use=""optional"">
		                <Tasks>
			                <FileUpdateTask updateTo=""Krista.FM.Client.SchemeEditor.dll"" localPath=""Krista.FM.Client.SchemeEditor.dll"" apply=""app-restart"">
				                <Description>Krista.FM.Client.SchemeEditor.dll</Description>
				                <Conditions>
					                <FileVersionCondition what=""below"" version=""2.8.0.10""/>
				                </Conditions>
			                </FileUpdateTask>
			                <FileUpdateTask updateTo=""Krista.FM.Client.SchemeEditor.pdb"" localPath=""Krista.FM.Client.SchemeEditor.pdb"" apply=""app-restart"">
				                <Description>Krista.FM.Client.SchemeEditor.pdb</Description>
				                <Conditions>
					                <FileVersionCondition what=""below"" version=""2.8.0.10"" localPath=""Krista.FM.Client.SchemeEditor.dll""/>
				                </Conditions>
			                </FileUpdateTask>
		                </Tasks>
	                </Patch>
                </Patches>";

        [Test]
        public void TestCanReadFeed()
        {
            var reader = new XmlFeedReader();
            IList<IUpdatePatch> updates = reader.Read(testFeed, null);

            Assert.IsTrue(updates.Count > 0);
            Assert.IsTrue(updates[0].Tasks.Count > 0);
        }
    }
}
