using System;
using System.Collections.ObjectModel;
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
        private string facultyEmployeeLaererFnr = "fra properties";
        // private string tittelTBD = "fra properties"; // TODO JB: Dobbeltsjekk rolle: Trengs student over 18 eller en lærer til?
        private string studentUnder18Fnr = "fra properties";

        private string feidePw = "fra properties";
        
        private string resultatTekst = "";

        [OneTimeSetUp]
        public void Init()
        {
            Console.WriteLine("OneTimeSetUp begin");
            LogWriter.LogWrite("Starter seleniumtest.");
           driver = Selenium.SeleniumSetup.GetFirefoxDriver();
           // driver = Selenium.SeleniumSetup.GetBrowserstackDriver();
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
        public void gaaTilDigilaerForside()
        {
            try
            {
                driver.Navigate().GoToUrl("https://digilaer.no");  

                ReadOnlyCollection<IWebElement> sideelementer = driver.FindElements(By.ClassName("page__content"));
                Assert.That(sideelementer.Count, Is.GreaterThan(0));
                
                sideelementer = driver.FindElements(By.ClassName("layout"));
                Assert.That(sideelementer.Count, Is.GreaterThan(0));
            } catch(Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Målform kan endres")]
        public void testAtMaalFormKanEndres()
        {
            try
            {
                driver.Navigate().GoToUrl("https://digilaer.no");

                driver.FindElement(By.Id("language-switcher")).Click();
                driver.FindElement(By.LinkText("Nynorsk")).Click();
                Assert.That(driver.PageSource.ToLower().Contains("moglegheiter"), Is.True);

                driver.FindElement(By.Id("language-switcher")).Click();
                driver.FindElement(By.LinkText("Bokmål")).Click();
                Assert.That(driver.PageSource.ToLower().Contains("muligheter"), Is.True);
                
                driver.Navigate().GoToUrl("https://digilaer.no/nb/om-digilaerno");
            } catch(Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Åpne skole.digilær hovedside")]
        public void gaaTilSkoleDigiLaerForside()
        {
            try
            {
                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                ReadOnlyCollection<IWebElement> sideelementer = driver.FindElements(By.Id("page"));
                Assert.That(sideelementer.Count, Is.GreaterThan(0));
                
                sideelementer = driver.FindElements(By.ClassName("card-body"));
                Assert.That(sideelementer.Count, Is.GreaterThan(0));
            } catch(Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Elev kan logge på med Feide via digilaer.no")]
        public void loggInnOgUtAvDigilaerMedFeide()
        {
            try
            {
                driver.Navigate().GoToUrl("https://digilaer.no");

                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                LoggUt();
            } catch(Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Elev kan logge på med Feide via skole.digilær")]
        public void loggInnOgUtAvSkoleDigilaerMedFeide()
        {
            try
			{
                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                LoggInnMedFeide(studentUnder18Fnr, feidePw);  
                LoggUt();
            } catch (Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

         [Test]
        [TestCase(TestName = "Lærer kan logge på med Feide")]
        public void loggInnOgUSomLaererMedFeide()
        {
            try
			{
                driver.Navigate().GoToUrl("https://digilaer.no");
                LoggInnMedFeide(facultyEmployeeLaererFnr, feidePw);  
                LoggUt();
            } catch (Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
    
        [Test]
        [TestCase(TestName = "Elev har tilgang til fag")]
        public void sjekkAtElevHarTilgangTilFag()
        {
            try
            {
                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                GaaTilSeleniumFag();
                string pageSource = driver.PageSource;

                Assert.That(pageSource.Contains("Oppslagstavle"), Is.True);
                Assert.That(pageSource.ToLower().Contains("emne 4"), Is.True);

                LoggUt();
            } catch (Exception exception) 
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Elev kan ikke redigere fag")]
        public void sjekkAtElevIkkeKanRedigereFag()
        {
            try
            {
                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                GaaTilSeleniumFag();

                ReadOnlyCollection<IWebElement> redigeringsknapp = driver.FindElements(By.XPath("//button[.='Slå redigering på']"));
                Assert.That(redigeringsknapp.Count, Is.Zero);
  
                LoggUt();
            } catch (Exception exception) 
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "AdobeConnect (:construction_worker: Denne testen er under arbeid... :construction:)")]
        public void testAdobeConnect()
        {            
            try
            {
                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                LoggInnMedFeide(studentUnder18Fnr, feidePw);  
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
            } catch(Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Lærer kan redigere fag")]
        public void testAtLaererKanRedigereFag()
        {            
            try
            {
                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                LoggInnMedFeide(facultyEmployeeLaererFnr, feidePw);  
                GaaTilSeleniumFag();

                driver.FindElement(By.XPath("//button[.='Slå redigering på']")).Click();

                ReadOnlyCollection<IWebElement> redigerknapper = driver.FindElements(By.XPath("//a[@aria-label='Rediger']"));
                Assert.That(redigerknapper.Count, Is.GreaterThan(6));
                //vurder å faktisk redigere noe.. obs: hvis flere tråder kjører i parallell kan dette bli krøll

                // slå redigering av 
                driver.FindElement(By.XPath("//button[.='Slå redigering av']")).Click();

                LoggUt(); 
            } catch(Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        private void LoggInnMedFeide(string brukernavn, string passord)
        {
            if(driver.PageSource.ToLower().Contains("innlogget bruker"))
            {
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

            Thread.Sleep(2000);
            Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker"), Is.True,  "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
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

        private void haandterFeiletTest(Exception e, string testnavn)
        {
                Printscreen.TakeScreenShot(driver, testnavn);
                LogWriter.LogWrite(testnavn + " feilet. Stacktrace:\n" + e.StackTrace);
                LoggUt();
        }
    }
}