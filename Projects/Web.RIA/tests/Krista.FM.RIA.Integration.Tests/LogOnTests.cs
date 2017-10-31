using NUnit.Framework;
using WatiN.Core;
using Button = Krista.FM.RIA.Integration.Tests.Controls.Button;

namespace Krista.FM.RIA.Integration.Tests
{
    [TestFixture]
    public class LogOnTests
    {
        [SetUp]
        public void SetUp()
        {
#if !DEBUG
            Settings.MakeNewIeInstanceVisible = false;
#endif
            Settings.WaitForCompleteTimeOut = 60 * 2;
        }

        [Test]
        public void LogOnLogOffIETest()
        {
            using (var browser = new IE("http://fm5-9.krista.ru:8080/"))
            {
                LogOn(browser);

                LogOff(browser);
            }
        }

        private void LogOn(IE browser)
        {
            browser.TextField(Find.ById("userName")).TypeText("weblogin");
            browser.TextField(Find.ById("password")).TypeText("1");

            browser.Control<Button>("logonButton").Click();

            browser.WaitUntilContainsText("Выйти", 2*60);
        }

        private static void LogOff(IE browser)
        {
            browser.Control<Button>("btnLogout").Click();

            browser.WaitUntilContainsText("Вы действительно хотите выйти?");

            browser.Button(Find.ByText("Да")).Click();

            browser.WaitForComplete();
            
            browser.WaitUntilContainsText("Вход в систему");
            Assert.True(browser.ContainsText("Вход в систему"));
        }
    }
}
