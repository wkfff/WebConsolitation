using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.Update.Framework;
using Krista.FM.Update.PatchMakerLibrary;
using NUnit.Framework;

namespace Krista.FM.Update.PatchMaker.Tests
{
    [TestFixture]
    public class CreatePatchTests
    {
        private const string info =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
              <Patch>
	             <!-- Короткое описание патча -->
	             <Description>Версия MDX Эксперт 3.13</Description>

	             <!-- Детальное описание -->
	             <DescriptionDetail>Версия MDX Эксперт 3.13</DescriptionDetail>

	             <!-- Обязательность обновления -->
	             <Use>optional</Use>

	             <!-- Региональная константа -->
	             <OKTMO>460000</OKTMO>

	             <!-- Базовая версия клиентского приложения, на которую применяем патч -->
	             <AppVersion>3.12</AppVersion>

                 <!-- Базовая версия службы обновления (заполняется если патч соделжит патч на инсталлятор) -->
                 <InstallerVersion>4.0</InstallerVersion>
	
	             <!-- Включаем в патч обновления на версию -->
	             <!-- Таких секций может бытьнесколько, если, например, одним патчем обновляем на версию и клиента, и службу обновления, и в будущем сервер.-->
	             <VersionUpdate>
		            <Names>MDXExpert</Names>
		            <DisplayName>Криста MDX Эксперт</DisplayName>
		            <DisplayVersion>3.13</DisplayVersion>
	             </VersionUpdate>	
            </Patch>";

        [Test]
        public void ReadConfigurationTest()
        {
            XDocument document = XDocument.Parse(info);
            string patchDescription = string.Empty;
            string patchDetailDescription = string.Empty;
            Use use = Use.Required;
            string OKTMO = string.Empty;
            string appVersion = string.Empty;
            string installerVersion = string.Empty;
            List<VersionUpdateClass> versionUpdateClasses = new List<VersionUpdateClass>();
            PatchMakerConsole.PatchMakerConsole.ReadFileParameters(
                document,
                ref patchDescription,
                ref patchDetailDescription,
                ref use,
                ref OKTMO,
                ref appVersion,
                ref installerVersion,
                ref versionUpdateClasses);

            Assert.AreEqual(patchDescription, "Версия MDX Эксперт 3.13");
            Assert.AreEqual(patchDetailDescription, "Версия MDX Эксперт 3.13");
            Assert.AreEqual(use, Use.Optional);
            Assert.AreEqual(OKTMO, "460000");
            Assert.AreEqual(appVersion, "3.12");
            Assert.AreEqual(installerVersion, "4.0");
            Assert.AreEqual(versionUpdateClasses.Count, 1);
        }

        [Test]
        public void CreatePatchTest()
        {
            // TODO реализовать создание патча утилитой PatchMaker, заодно провеси рефакторинг существующего кода
        }
    }
}
