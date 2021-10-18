using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Slack;
using Utils;

namespace TestSuite
{
    [TestFixture]
    public class LoginTests
    {

        private IWebDriver driver;
        private string fagkodeSelenium = "SEL";
        private string elevFeideFnr = "fra properties";
        private string feidePw = "fra properties";
        
        private string resultatTekst = "";

        [OneTimeSetUp]
        public void Init()
        {
            Console.WriteLine("OneTimeSetUp begin");
            LogWriter.LogWrite("Starter seleniumtest.");
            driver = Selenium.SeleniumSetup.GetFirefoxDriver();
            //driver = Selenium.SeleniumSetup.GetBrowserstackDriver();
            Console.WriteLine("OneTimeSetUp finished");
        }

        [OneTimeTearDown]
        public void AfterTestsFinished()
        {
            Console.WriteLine("OneTimeTearDown begin");
            
            driver.Quit();

            sendSlackResultat();

            Console.WriteLine("OneTimeTearDown finished");
            LogWriter.LogWrite("Test ferdig.");
        }

        private void sendSlackResultat() 
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                resultatTekst = TestContext.CurrentContext.Result.FailCount + " test fail, " + TestContext.CurrentContext.Result.PassCount + " test ok\n" + resultatTekst;
                resultatTekst += "\n Kanskje <@joakimbjerkheim> tar en titt?";
            } else 
            {
                Console.WriteLine("Digitester ferdig uten feil");
                resultatTekst = "Alle " + TestContext.CurrentContext.Result.PassCount + " tester kjørt ok!:ok_hand:\n" + resultatTekst;
            }
            SlackClient.CallSlack(resultatTekst);
        }

        [TearDown]
        public void AfterEachTest() 
        {
            if (TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Passed))
            {
                resultatTekst += ":white_check_mark:" + TestContext.CurrentContext.Test.Name + "\n";
            }
            else if (TestContext.CurrentContext.Result.Outcome.Equals(TestStatus.Failed) ||  
            TestContext.CurrentContext.Result.Outcome.Equals(ResultState.Failure) || TestContext.CurrentContext.Result.Outcome.Equals(ResultState.Error))
            {
                resultatTekst += ":x:" + TestContext.CurrentContext.Test.Name + "\n";
            }
        }

        [Test]
        [TestCase(TestName = "Åpne digilær hovedside")]
        public void goToDigiLaerFrontPage()
        {
          driver.Navigate().GoToUrl("https://digilaer.no");  
            Assert.That(driver.PageSource.ToLower().Contains("nasjonal"));
        }

        [Test]
        [TestCase(TestName = "Test at målform kan endres")]
        public void testAtMaalFormKanEndres()
        {
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
        [TestCase(TestName = "Åpne skole.digilær hovedside")]
        public void goToSkoleDigiLaerFrontPage()
        {    
            driver.Navigate().GoToUrl("https://skole.digilaer.no");
            Assert.That(driver.PageSource.ToLower().Contains("velkommen"), Is.True);
        } 

        [Test]
        [TestCase(TestName = "Logg inn med Feide via digilaer.no")]
        public void logInAndOutDigilaerWithFeide()
        {
            driver.Navigate().GoToUrl("https://digilaer.no");

            LoggInnMedFeide(elevFeideFnr, feidePw);

            LoggUt();
        }

        [Test]
        [TestCase(TestName = "Logg inn og ut av skole.digilær med Feide")]
        public void logInAndOutSkoleDigilaerWithFeide()
        {
            try
			{
                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                LoggInnMedFeide(elevFeideFnr, feidePw);  
                LoggUt();
            } catch (Exception e)
            {
                Printscreen.TakeScreenShot(driver, "innUtSkoleDig");
                LogWriter.LogWrite("logInAndOutSkoleDigilaerWithFeide failed.. StackTrace:");
                LogWriter.LogWrite(e.StackTrace);
                LoggUt();
            }
        } 
    
        [Test]
        [TestCase(TestName = "Sjekk tilgang til fag")]
        public void sjekkTilgangTilFag()
        {
            try
            {
                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                LoggInnMedFeide(elevFeideFnr, feidePw);

                GaaTilSeleniumFag();
                string pageSource = driver.PageSource;

                Assert.That(pageSource.Contains("Oppslagstavle"), Is.True);
                Assert.That(pageSource.ToLower().Contains("emne 4"), Is.True);

                LoggUt();
            } catch (Exception e) 
            {
                Printscreen.TakeScreenShot(driver, "sjekkFagtilgang");
                LogWriter.LogWrite("sjekkTilgangTilFag failed.. StackTrace:");
                LogWriter.LogWrite(e.StackTrace);
                LoggUt();
            }
        }

        [Test]
        [TestCase(TestName = "Test av AdobeConnect (:construction_worker: Denne testen er under arbeid... :construction:)")]
        public void testAdobeConnect()
        {            
            driver.Navigate().GoToUrl("https://skole.digilaer.no");
            LoggInnMedFeide(elevFeideFnr, feidePw);  
            GaaTilSeleniumFag();

            string adobeConnectUrl = driver.FindElement(By.XPath("//span[.='SELENIUM test Adobe Connect']/ancestor::a")).GetAttribute("href");
            driver.Navigate().GoToUrl(adobeConnectUrl);
            Assert.True(driver.FindElement(By.XPath("//label[@for='lblmeetingnametitle']")).Displayed, "Felt for møtetittel ikke funnet");

            string moteUrl = driver.FindElement(By.XPath("//input[@value='Join Meeting']")).GetAttribute("onclick");
            int moteUrlLengde = moteUrl.IndexOf("'", (moteUrl.IndexOf("'")) + 1) - moteUrl.IndexOf("'") - 1;
            moteUrl = moteUrl.Substring(moteUrl.IndexOf("'") + 1, moteUrlLengde);
            driver.Navigate().GoToUrl(moteUrl);

            // TODO: Når testdata er på plass: Implementer mer test her hvis mulig (Nå gis kun "Unable to retrieve meeting details")
            LoggUt(); // Funker logout hefra direkte når inne i AdobeConnect? 
        }

        private void LoggInnMedFeide(string brukernavn, string passord)
        {
            if(driver.PageSource.ToLower().Contains("innlogget bruker")){
                LogWriter.LogWrite("Var allerede logget inn, forsøker å logge ut...");
                LoggUt();
                LogWriter.LogWrite("Logget ut ok");
            }

            driver.FindElement(By.LinkText("Logg inn")).Click();            
            if(driver.FindElements(By.ClassName("dl-linkbutton")).Count > 0) 
            {
                driver.FindElements(By.ClassName("dl-linkbutton"))[0].Click();
            }

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

        private void GaaTilSeleniumFag()
        {
            driver.FindElement(By.XPath("//span[.='" + fagkodeSelenium + "']")).Click();
        }
    }
}