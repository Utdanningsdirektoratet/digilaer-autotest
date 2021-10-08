using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;

namespace TestSuite
{
    [TestFixture]
    public class LoginTests
    {

        private IWebDriver driver;
        private string fagkodeSelenium = "SEL";
        private string elevFeideFnr = "fra properties";
        private string feidePw = "fra properties";

        [OneTimeSetUp]
        public void Init() {
            Console.WriteLine("OneTimeSetUp begin");
            driver = Selenium.SeleniumSetup.GetSeleniumDriver();
            Console.WriteLine("OneTimeSetUp finished");
        }

        [OneTimeTearDown]
        public void Cleanup() {
            Console.WriteLine("OneTimeTearDown begin");
            driver.Close();
            Console.WriteLine("OneTimeTearDown finished");
        }

        [Test]
        [TestCase(TestName = "1. Åpne digilær hovedside")]
        public void goToDigiLaerFrontPage() {
          driver.Navigate().GoToUrl("https://digilaer.no");  
            Assert.That(driver.PageSource.ToLower().Contains("nasjonal"));
        }

        [Test]
        [TestCase(TestName = "2. Test at målform kan endres")]
        public void testAtMaalFormKanEndres() {
            driver.Navigate().GoToUrl("https://digilaer.no");

            driver.FindElement(By.Id("language-switcher")).Click();
            driver.FindElement(By.LinkText("Nynorsk")).Click();
            Assert.That(driver.PageSource.ToLower().Contains("moglegheiter"), Is.True);

            driver.FindElement(By.Id("language-switcher")).Click();
            driver.FindElement(By.LinkText("Bokmål")).Click();
            Assert.That(driver.PageSource.ToLower().Contains("muligheter"), Is.True);
            
            driver.Navigate().GoToUrl("https://digilaer.no/nb/om-digilaerno");
        }

        [Test]
        [TestCase(TestName = "3. Åpne skole.digilær hovedside")]
        public void goToSkoleDigiLaerFrontPage() 
        {    
            driver.Navigate().GoToUrl("https://skole.digilaer.no");
            Assert.That(driver.PageSource.ToLower().Contains("velkommen"), Is.True);
        } 

        [Test]
        [TestCase(TestName = "4. Logg inn og ut av digilær med Feide")]
        public void logInAndOutDigilaerWithFeide() 
        {
            driver.Navigate().GoToUrl("https://digilaer.no");

            LoggInnMedFeide("Logg inn", elevFeideFnr, feidePw);

            LoggUt();
        }   

        [Test]
        [TestCase(TestName = "5. Logg inn og ut av skole.digilær med Feide")]
        public void logInAndOutSkoleDigilaerWithFeide() {
            driver.Navigate().GoToUrl("https://skole.digilaer.no");
            LoggInnMedFeide("Logg inn her", elevFeideFnr, feidePw);  
            LoggUt();
        }
    
        [Test]
        [TestCase(TestName = "6. Sjekk tilgang til fag")]
        public void sjekkTilgangTilFag() {
            driver.Navigate().GoToUrl("https://skole.digilaer.no");
            LoggInnMedFeide("Logg inn her", elevFeideFnr, feidePw);

            driver.FindElement(By.XPath("//span[.='" + fagkodeSelenium + "']")).Click();
            string pageSource = driver.PageSource;

            Assert.That(pageSource.Contains("Oppslagstavle"), Is.True);
            Assert.That(pageSource.ToLower().Contains("emne 4"), Is.True);

            LoggUt();
        }
    
        private void LoggInnMedFeide(string loggInnLinkTekst, string brukernavn, string passord)
        {
            Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker"), Is.False);
            
            driver.FindElement(By.LinkText(loggInnLinkTekst)).Click();
            
            if(driver.FindElements(By.Id("username")).Count == 0 || !driver.FindElement(By.Id("username")).Displayed) 
            {
                IWebElement orgSelector = driver.FindElement(By.Id("org_selector-selectized"));
                orgSelector.SendKeys("Feide"); // For testing mot dataporten bruk "Tjenesteleverandør"
                orgSelector.SendKeys(Keys.Enter);
            }

            driver.FindElement(By.Id("username")).SendKeys(brukernavn);
            driver.FindElement(By.Id("password")).SendKeys(passord);
            driver.FindElement(By.Id("password")).SendKeys(Keys.Enter);

            Assert.That(driver.PageSource.Contains("usermenu"), Is.True);
        }
 
        private void LoggUt() 
        {
            driver.FindElement(By.ClassName("usermenu")).Click();
            Thread.Sleep(1000);            
            driver.FindElement(By.XPath("//span[.='Logg ut']")).Click();
        }
    }
}