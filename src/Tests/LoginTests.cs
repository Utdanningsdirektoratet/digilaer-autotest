using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using monitor.api;
using monitor.api.dto;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Selenium;
using Slack;
using Utils;
//using WindowsInput;

namespace TestSuite
{
//   [TestFixture(DeviceConfig.AndroidGalaxyS20)]  // Utgår egentlig pga device issues på bstack iflg support der:       
//   [TestFixture(DeviceConfig.GooglePixel6)] // Samme som S20 har denne gitt appium/chrome error, så forsøker uten denne også
//  [Parallelizable(ParallelScope.Fixtures)] // Har et maks antall, trolig 5 stk samtidige. Dersom dette skal benyttes: Sørg for at det er trådsikkert mtp skriving til DB-api
    [TestFixture(DeviceConfig.OSXBigSurEdge)]
    [TestFixture(DeviceConfig.OSXBigSurChrome)]
    [TestFixture(DeviceConfig.OSXBigSurFirefox)]
    [TestFixture(DeviceConfig.OSXBigSurSafari)]
    [TestFixture(DeviceConfig.IOSIphoneXS)]
    [TestFixture(DeviceConfig.Win10Chrome)]
    [TestFixture(DeviceConfig.Win10Firefox)]
    [TestFixture(DeviceConfig.Win10Edge)] 
    [TestFixture(DeviceConfig.AndroidGalaxyTabS7)]
    [TestFixture(DeviceConfig.Ipad11Pro2020)]
    [TestFixture(DeviceConfig.AndroidGalaxyS21)]
    [TestFixture(DeviceConfig.AndroidOnePlus9)]
    [TestFixture(DeviceConfig.SamsungGalaxyS10)]
    [TestFixture(DeviceConfig.GooglePixel4XL)]
    public class LoginTests
    {
        private IWebDriver driver;
        private BrowserStackCapabilities bsCaps;
        private string fagkodeSelenium = "SEL";
        private string facultyEmployeeLaererFnr = System.Environment.GetEnvironmentVariable("DIGI_USER_FACULTY");
        // private string tittelTBD = "fra properties"; // TODO: Dobbeltsjekk rolle: Trengs student over 18 eller en lærer til?
        private string studentUnder18Fnr = System.Environment.GetEnvironmentVariable("DIGI_ELEV_UNDER_ATTEN");
        private string feidePw = System.Environment.GetEnvironmentVariable("DIGI_FEIDE_PW");
        private string resultatTekst = "";
        private int enhetIdForDB;
        private int funkTestIdForDB;
        private DateTime teststartForDB;
        private DateTime testsluttForDB;
        private string sprakUrl = "lang=nb";

        public LoginTests(DeviceConfig deviceConfig)
        {
            SeleniumSetup seleniumSetup = new SeleniumSetup();

            if(deviceConfig == DeviceConfig.Win10Edge)
            {
                bsCaps = new BrowserStackCapabilities{os = "Windows", osVersion = "10", browser = "Edge", browserVersion = "latest"};
            } else if(deviceConfig == DeviceConfig.Win10Chrome)
            {
                bsCaps = new BrowserStackCapabilities{os = "Windows", osVersion = "10", browser = "Chrome", browserVersion = "latest"};
            } else if(deviceConfig == DeviceConfig.Win10Firefox)
            {
                bsCaps = new BrowserStackCapabilities{os = "Windows", osVersion = "10", browser = "Firefox", browserVersion = "latest"};
            } else if(deviceConfig == DeviceConfig.OSXBigSurSafari)
            {
                bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Big Sur", browser = "Safari", browserVersion = "14.1", resolution = "1920x1080"};
            } else if(deviceConfig == DeviceConfig.OSXBigSurFirefox)
            {
                bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Big Sur", browser = "Firefox", browserVersion = "99.0", resolution = "1920x1080"};
            } else if(deviceConfig == DeviceConfig.OSXBigSurChrome)
            {
                bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Big Sur", browser = "Chrome", browserVersion = "latest", resolution = "1920x1080"};
            } else if(deviceConfig == DeviceConfig.OSXBigSurEdge)
            {
                bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Big Sur", browser = "Edge", browserVersion = "latest", resolution = "1920x1080"};
            } else if(deviceConfig == DeviceConfig.Ipad11Pro2020)
            {
                bsCaps = new BrowserStackCapabilities{device = "iPad Pro 11 2020", browser = "Safari", osVersion = "13", realMobile = "true", local = "false"};
            } else if(deviceConfig == DeviceConfig.IOSIphoneXS)
            {
                bsCaps = new BrowserStackCapabilities{device = "iPhone XS", browser = "Safari", osVersion = "15", realMobile = "true"};
            } else if(deviceConfig == DeviceConfig.AndroidGalaxyTabS7)
            {
                bsCaps = new BrowserStackCapabilities{device = "Samsung Galaxy Tab S7", browser = "Chrome", osVersion = "10.0", realMobile = "true", local = "false"};
            }
             else if(deviceConfig == DeviceConfig.AndroidGalaxyS20)
            {
                // OBS: Frarådet å bruke denne fra bstack support da den i følge dem er ustabil.
                bsCaps = new BrowserStackCapabilities{device = "Samsung Galaxy S20", browser = "Chrome", osVersion = "10.0", realMobile = "true", local = "false"};
            } else if(deviceConfig == DeviceConfig.AndroidGalaxyS21)
            {
                bsCaps = new BrowserStackCapabilities{device = "Samsung Galaxy S21", browser = "Chrome", osVersion = "11.0", realMobile = "true", local = "false"};
            } else if(deviceConfig == DeviceConfig.AndroidOnePlus9)
            {
                bsCaps = new BrowserStackCapabilities{device = "OnePlus 9", browser = "Chrome", osVersion = "11.0", realMobile = "true", local = "false"};
            } else if(deviceConfig == DeviceConfig.GooglePixel6)
            {
                bsCaps = new BrowserStackCapabilities{device = "Google Pixel 6", browser = "Chrome", osVersion = "12.0", realMobile = "true", local = "false"};
            } else if(deviceConfig == DeviceConfig.SamsungGalaxyS10)
            {
                bsCaps = new BrowserStackCapabilities{device = "Samsung Galaxy S10", browser = "Chrome", osVersion = "9.0", realMobile = "true", local = "false"};
            } else if(deviceConfig == DeviceConfig.GooglePixel4XL)
            {
                bsCaps = new BrowserStackCapabilities{device = "Google Pixel 4 XL", browser = "Chrome", osVersion = "10.0", realMobile = "true", local = "false"};
            }

         //  driver = seleniumSetup.GetFirefoxDriver(); GlobalVariables.setStage(); /  GlobalVariables.setProd(); // For lokal debug evt
             driver = seleniumSetup.GetBrowserstackDriver(bsCaps);
        }

        [OneTimeSetUp]
        public void Init()
        {
            LogWriter.LogWrite("Starter seleniumtest på en device i " + GlobalVariables.miljo);
            if(GlobalVariables.ErProd() && GlobalVariables.loggTilDatabase)
            {
                enhetIdForDB =  MonitorApiClient.FindOrCreateEnhetOppsett(new EnhetOppsett{
                    enhet = bsCaps.device, nettleserNavn = bsCaps.browser, nettleserVersjon = bsCaps.browserVersion,
                    osNavn = bsCaps.os, osVersjon = bsCaps.osVersion, opplosning = bsCaps.resolution
                });
                Console.WriteLine("Id mottatt er :  " + enhetIdForDB);
            }
            
            MaksimerVindu();
        }

        [SetUp]
        public void BeforeEachTest()
        {
            teststartForDB = DateTime.Now;
        }

        [OneTimeTearDown]
        public void AfterTestsOnOneDeviceIsFinished()
        {
            sendSlackResultat();
            driver.Quit();
            LogWriter.LogWrite("Test ferdig.");
        }

        private void sendSlackResultat()
        {
            string oppsettTekst = ":gear:" + SeleniumSetup.GetNameString(bsCaps);

            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                resultatTekst = oppsettTekst + ":\n"
                + TestContext.CurrentContext.Result.FailCount + " test fail, " + TestContext.CurrentContext.Result.PassCount + " test ok\n"
                + resultatTekst;
                if(GlobalVariables.ErProd()  && GlobalVariables.loggTilDatabase) // Hvis vanlig monitoreringskjøring
                {
                    resultatTekst += "\nKanskje <@joakimbjerkheim> eller <@mathias.meier.nilsen> tar en titt?";
                }
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Test feilet. \"}}");
            } else if(TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Warning) 
            {
                resultatTekst = oppsettTekst + ":\n"
                + ":white_check_mark:" + TestContext.CurrentContext.Result.PassCount + " tester kjørt ok!:ok_hand:\n"
                + resultatTekst;
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \" Test OK. \"}}");
            } else
            {
                resultatTekst = oppsettTekst + ":\n"
                + ":white_check_mark:" + "Alle " + TestContext.CurrentContext.Result.PassCount + " tester kjørt ok!:ok_hand:\n"
                + resultatTekst;
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \" Test OK. \"}}");
            }

            SlackClient.CallSlack(resultatTekst);
        }

        [TearDown]
        public void AfterEachTest()
        {
            if(GlobalVariables.ErProd()  && GlobalVariables.loggTilDatabase)
            {
                funkTestIdForDB = MonitorApiClient.FindOrCreateFunksjonellTest(TestContext.CurrentContext.Test.MethodName, TestContext.CurrentContext.Test.Name);

                string debugInfo = TestContext.CurrentContext.Result.Message + TestContext.CurrentContext.Result.StackTrace;
                if(debugInfo != null && debugInfo.Length > 1300)
                {
                    Console.WriteLine("Kutter debuginfo:\n " + debugInfo);
                    debugInfo = debugInfo.Substring(0, 300) + "[stripped]" + debugInfo.Substring(debugInfo.Length - 700, 700);
                    Console.WriteLine("Debuginfo kuttet:\n " + debugInfo);
                }

                if((int)TestContext.CurrentContext.Result.Outcome.Status != 1) // Lagrer ikke skippede/ignorerte tester i DB
                {
                    MonitorApiClient.PostTestkjoring(new Testkjoring{
                        enhetOppsettId = enhetIdForDB, funksjonellTestId = funkTestIdForDB, resultatId = (int)TestContext.CurrentContext.Result.Outcome.Status,
                        starttid = teststartForDB, sluttid = DateTime.Now,
                        debugInformasjon = ""});
                }
                 
                if((int)TestContext.CurrentContext.Result.Outcome.Status == 4)
                {
                    Console.WriteLine("Sendt feilet test til db: " + "Enhet-id: " + enhetIdForDB + 
                    " funktestid: " +  funkTestIdForDB +
                    " teststartForDB: " +  teststartForDB +
                    "debuginfo: " + debugInfo);
                }
            }

            if((int)TestContext.CurrentContext.Result.Outcome.Status == 3)
            {
                resultatTekst += ":warning:'"  + TestContext.CurrentContext.Test.Name +  "' fungerte etter retry.\n";
            } else if (TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Passed))
            {
                // Utkommentert for å ikke overfylle slack kanalen etter kjøring. Fokus på evt feilede tester heller.
                // resultatTekst += ":white_check_mark:" + TestContext.CurrentContext.Test.Name + "\n";
            } else if (TestContext.CurrentContext.Result.Outcome.Equals(TestStatus.Failed) ||
                TestContext.CurrentContext.Result.Outcome.Equals(ResultState.Failure) || TestContext.CurrentContext.Result.Outcome.Equals(ResultState.Error))
            {
                resultatTekst += ":x:" + TestContext.CurrentContext.Test.Name + "\n";
            }
        }

        [Test]
        [TestCase(TestName = "Åpne digilær hovedside")]
        public void GaaTilDigilaerForside()
        {
            try
            {
                GaaTilDigilaer();
                ReadOnlyCollection<IWebElement> sideelementer = driver.FindElements(By.ClassName("page__content"));
                Assert.That(sideelementer.Count, Is.GreaterThan(0));

                sideelementer = driver.FindElements(By.ClassName("layout"));
                Assert.That(sideelementer.Count, Is.GreaterThan(0));
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Målform kan endres")]
        public void TestAtMaalFormKanEndres()
        {
            try
            {
                GaaTilDigilaer();
                if(driver.FindElement(By.Id("language-switcher")).Displayed)
                {
                    driver.FindElement(By.Id("language-switcher")).Click();
                }

                driver.FindElement(By.LinkText("Nynorsk")).Click();
                HaandterMacSafari();
                Assert.That(driver.PageSource.ToLower().Contains("moglegheiter"), Is.True);

                if(driver.FindElement(By.Id("language-switcher")).Displayed)
                {
                    driver.FindElement(By.Id("language-switcher")).Click();
                }

                driver.FindElement(By.LinkText("Bokmål")).Click();
                HaandterMacSafari();
                Assert.That(driver.PageSource.ToLower().Contains("muligheter"), Is.True);

                driver.Navigate().GoToUrl(GlobalVariables.digilaerUrl + "/nb/om-digilaerno");
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Åpne skole.digilær hovedside")]
        public void GaaTilSkoleDigiLaerForside()
        {
            try
            {
                GaaTilSkoleDigilaer();

                ReadOnlyCollection<IWebElement> sideelementer = driver.FindElements(By.Id("page"));
                Assert.That(sideelementer.Count, Is.GreaterThan(0));

                sideelementer = driver.FindElements(By.ClassName("card-body"));
                Assert.That(sideelementer.Count, Is.GreaterThan(0));
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
 
        [Test]
        [TestCase(TestName = "Elev kan logge på med Feide via digilaer.no")]
        public void LoggInnOgUtAvDigilaerMedFeide()
        {
            if(GlobalVariables.ErStage())
            {                
                Assert.Ignore(); // Skip test siden det ikke er stage-innlogging med feide fra annet sted enn hovedsiden
            }
            
            try
            {
                GaaTilDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                LoggUt();
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Elev kan logge på med Feide via skole.digilær")]
        public void LoggInnOgUtAvSkoleDigilaerMedFeide()
        {
            try
			{
                GaaTilSkoleDigilaer();
                HaandterMacSafari();
                LoggInnMedFeide(studentUnder18Fnr, feidePw);
                LoggUt();
            } catch (Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Lærer kan logge på med Feide")]
        public void LoggInnOgUSomLaererMedFeide()
        {
            try
			{
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(facultyEmployeeLaererFnr, feidePw);
                LoggUt();
            } catch (Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Elev har tilgang til fag")]
        public void SjekkAtElevHarTilgangTilFag()
        {
            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                GaaTilSeleniumFag();
                string pageSource = driver.PageSource;

                Assert.That(pageSource.Contains("Oppslagstavle"), Is.True);

                LoggUt();
            } catch (Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Elev kan ikke redigere fag")]
        public void SjekkAtElevIkkeKanRedigereFag()
        {
            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                GaaTilSeleniumFag();

                ReadOnlyCollection<IWebElement> redigeringsknapp = driver.FindElements(By.XPath("//button[.='Slå redigering på']"));
                Assert.That(redigeringsknapp.Count, Is.Zero);
  
                LoggUt();
            } catch (Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Last Zoom-side")]
        public void TestZoom()
        {
            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, feidePw);
                GaaTilSeleniumFag();

                driver.FindElement(By.XPath("//span[contains(text(), 'SELENIUM test av Zoom')]")).Click();

                Thread.Sleep(5000); // Load spinner i zoom...

                if(bsCaps.device != null && bsCaps.device.Contains("iPad"))
                {
                    IWebElement iFrameZoom = driver.FindElement(By.Id("contentframe"));

                    Assert.IsTrue(iFrameZoom.Displayed);
                    // Gå inn på iFrame fungerer ikke på automatisert iPad

                    Thread.Sleep(5000); // spinner i zoom...

                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed); // TODO: Forsøk å teste noe mer rundt zoom... denne treffer trolig kun toppen av html...
                } else if(driver.Url.Contains(GlobalVariables.digilaerSkoleUrl))
                {
                    IWebElement iFrameZoom = driver.FindElement(By.Id("contentframe"));

                    Assert.IsTrue(iFrameZoom.Displayed);

                    driver.SwitchTo().Frame(iFrameZoom);

                    Thread.Sleep(5000); // spinner i zoom...

                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);
                    // TODO: Test evt noe mer her:
                    // Assert.IsTrue(driver.FindElement(By.Id("@integration-meeting-list")).Displayed);
                    // Assert.Greater(driver.FindElements(By.XPath("//a[text(), 'Join']")).Count, 0, "Forventet at det skulle være minst 1 Join-knapp");

                    driver.SwitchTo().ParentFrame();
                } else
                { // Mobil device har selve zoom lastet i stedet for en iFrame
                    Thread.Sleep(5000); // spinner i Zoom
                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);
                }
                driver.Navigate().GoToUrl(GlobalVariables.digilaerSkoleUrl + "/my/index.php?" + sprakUrl);
                HaandterMacSafari();
                LoggUt();
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Lærer kan redigere fag")]
        public void TestAtLaererKanRedigereFag()
        {
            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(facultyEmployeeLaererFnr, feidePw);
                GaaTilSeleniumFag();

                driver.FindElement(By.XPath("//button[.='Slå redigering på']")).Click();
                HaandterMacSafari();
                ReadOnlyCollection<IWebElement> redigerknapper = driver.FindElements(By.XPath("//a[@aria-label='Rediger']"));
                Assert.That(redigerknapper.Count, Is.GreaterThan(6));

                driver.FindElement(By.XPath("//button[.='Slå redigering av']")).Click();

                LoggUt();
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "AdobeConnect som lærer")]
        public void TestAdobeConnectLaerer()
        {
            if(GlobalVariables.ErStage())  // TODO JB: Midlertidig inntil testbruker fikses på stage
            {
                Assert.Ignore();
            }

            try
            {
                TestAdobeConnect(facultyEmployeeLaererFnr);
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
        
        private void TestAdobeConnect(string fnr)
        {
            GaaTilSkoleDigilaer();
            LoggInnMedFeide(fnr, feidePw);
            GaaTilSeleniumFag();
            int retries = 0; // For adobeconnect-hikke
            string moteUrl = null;

            while(moteUrl == null && retries < 5)
            {                
                string adobeConnectUrl = driver.FindElement(By.XPath("//span[.='SELENIUM test Adobe Connect']/ancestor::a")).GetAttribute("href");
                driver.Navigate().GoToUrl(adobeConnectUrl);

                try
                {
                    moteUrl = driver.FindElement(By.XPath("//input[@value='Join Meeting']")).GetAttribute("onclick");
                } catch(Exception e)
                {
                    retries++;
                    Thread.Sleep(15000);
                }
            }
            if(retries > 0)
            {
                Assert.Warn("Adobeconnect test gikk videre etter retry");
            }

            int moteUrlLengde = moteUrl.IndexOf("'", (moteUrl.IndexOf("'")) + 1) - moteUrl.IndexOf("'") - 1;
            moteUrl = moteUrl.Substring(moteUrl.IndexOf("'") + 1, moteUrlLengde);
            driver.Navigate().GoToUrl(moteUrl);
            Thread.Sleep(15000); // Lang lastetid og flere redirects

            if(bsCaps.realMobile == null)
            {
                IWebElement iFrameAdobe = driver.FindElement(By.Id("html-meeting-frame"));

                Assert.IsTrue(iFrameAdobe.Displayed);

                driver.SwitchTo().Frame(iFrameAdobe);
                String source = driver.PageSource;
                Assert.True(source.Contains("attendeePodContainerDiv"), "Siden inneholder ikke attendeePodContainerDiv");

                if(driver.FindElements(By.Id("download-app-notifier_1")).Count > 0 && driver.FindElement(By.Id("download-app-notifier_1")).Displayed)
                {
                    driver.FindElement(By.Id("download-app-notifier_1")).Click();
                }

                if(driver.FindElements(By.XPath("//span[.='Close']")).Count > 0 && driver.FindElement(By.XPath("//span[.='Close']")).Displayed)
                {
                    driver.FindElement(By.XPath("//span[.='Close']")).FindElement(By.XPath("./..")).Click();
                }

                if(driver.FindElements(By.XPath("//span[.='Display Media']")).Count > 0 && driver.FindElement(By.XPath("//span[.='Display Media']")).Displayed)
                {
                    driver.FindElement(By.XPath("//span[.='Display Media']")).FindElement(By.XPath("./..")).Click();
                }

                Assert.True(driver.FindElement(By.Id("attendeePodContainerDiv")).Displayed, "Div for deltakere ikke funnet (attendeePodContainerDiv)");

                driver.SwitchTo().ParentFrame();
            } else // Mobiler/tablets krever egen app. Gjør kun en assert:
            {
                Assert.True(driver.PageSource.Contains("Use the mobile app to join a room"));
            }
            driver.Navigate().GoToUrl(GlobalVariables.digilaerSkoleUrl + "/my/index.php?" + sprakUrl);
            HaandterAlert();
            HaandterMacSafari();

            LoggUt();
        }

        [Test]
        [TestCase(TestName = "AdobeConnect som elev")]
        public void TestAdobeConnectElev()
        {
            try
            {
                TestAdobeConnect(studentUnder18Fnr);
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Test Poodl MiniLesson diktat")]
        public void TestPoodlMinilessonDiktat()
        {
            if(GlobalVariables.ErProd())
            {
                Assert.Ignore(); // Skal kun testes på Stage
            }

            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                GaaTilSeleniumFag();
                
                // Gå inn på poodl minilesson
                driver.FindElement(By.XPath("//span[starts-with(., 'Poodll')]")).Click();
                Thread.Sleep(3000);
                if(driver.FindElements(By.ClassName("btn_finished_attempt")).Count > 0 &&
                        driver.FindElement(By.ClassName("btn_finished_attempt")).Displayed)
                {
                    driver.FindElement(By.ClassName("btn_finished_attempt")).Click();
                    Thread.Sleep(3000); // Trengs i FF hvertfall.
                    
                    driver.FindElement(By.XPath("//button[@data-action='save']")).Click();
                    HaandterMacSafari();
                }
                
                driver.FindElement(By.ClassName("fa-play")).Click();
                Thread.Sleep(500);
                Assert.True(driver.FindElement(By.ClassName("fa-spinner")).Displayed);
                Thread.Sleep(10000);
                Assert.True(driver.FindElement(By.ClassName("fa-play")).Displayed);

                IWebElement inputFelt = driver.FindElement(By.XPath("//input[@maxlength='6']"));
                inputFelt.SendKeys("alw");
                Thread.Sleep(1000);
                Assert.True(driver.FindElement(By.ClassName("fa-times")).Displayed);
                Assert.False(driver.FindElement(By.ClassName("fa-check")).Displayed);

                inputFelt.SendKeys("ays");
                Thread.Sleep(1000);
                Assert.True(driver.FindElements(By.ClassName("fa-check"))[1].Displayed);
                
                driver.FindElement(By.ClassName("minilesson_nextbutton")).Click();
                Thread.Sleep(3000);
    
                //driver.FindElement(By.ClassName("btn_finished_attempt")).Click();
                    //Thread.Sleep(3000);
                //driver.FindElement(By.ClassName("btn-primary")).Click();

                LoggUt();
            } catch (Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Test Moodle Quiz med gap fill dra ord")]
        public void TestMoodleQuizMedGapFill()
        {
            if(GlobalVariables.ErProd())
            {
                Assert.Ignore(); // Skal kun testes på Stage
            }

            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                GaaTilSeleniumFag();
                
                driver.FindElement(By.XPath("//span[starts-with(., '1 Tall og siffer')]")).Click();
                HaandterMacSafari();

                if(driver.FindElements(By.XPath("//button[.='Fortsett med forrige forsøk']")).Count > 0)
                {
                    driver.FindElement(By.XPath("//button[.='Fortsett med forrige forsøk']")).Click();
                } else if(driver.FindElements(By.XPath("//button[.='Ta quizen nå']")).Count > 0) {
                    driver.FindElement(By.XPath("//button[.='Ta quizen nå']")).Click();
                } else {
                    driver.FindElement(By.XPath("//button[.='Fortsett siste forhåndsvisning']")).Click();
                }

                HaandterMacSafari();
                IWebElement ordEn = driver.FindElement(By.XPath("//span[.='will']"));

                IWebElement slippFeltEn = driver.FindElements(By.ClassName("droptarget"))[0]; 

                // TODO:
                // dra ord inn i felter
                // Actions drag: Fungerer ikke med safari-driver
                // Assert Resultat
                // Klikk begynn på nytt
                // Håndter lukking av popup for å avslutte
                // Assert.That(pageSource.Contains("Oppslagstavle"), Is.True);

                LoggUt();
            } catch (Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Test Trinket regnestykke")]
        public void TestTrinket()
        {
            if(GlobalVariables.ErProd())
            {
                Assert.Ignore(); // Skal kun testes på Stage
            }
            
            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                GaaTilSeleniumFag();
                driver.FindElement(By.XPath("//span[starts-with(., 'Trinket')]")).Click();
                HaandterMacSafari();

                if(bsCaps.device != null && bsCaps.device.Contains("iPad"))
                {
                    IWebElement iFrameTrinket = driver.FindElement(By.TagName("iframe"));

                    Assert.IsTrue(iFrameTrinket.Displayed);

                    // driver.SwitchTo().Frame(iFrameVimeo); // Fungerer ikke på automatisert iPad
                    Thread.Sleep(5000);
                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);
                } else
                {
                    IWebElement iFrameTrinket = driver.FindElement(By.TagName("iframe"));
                    Assert.IsTrue(iFrameTrinket.Displayed);
                    driver.SwitchTo().Frame(iFrameTrinket);
                    Thread.Sleep(5000); 
                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);

                    driver.FindElement(By.ClassName("run-it")).Click();
                    Thread.Sleep(4000); 

                // TODO Sende tall: InputSimulator funker ikke
                //  InputSimulator sim = new InputSimulator();
                //  Thread.Sleep(5000); 
                //  sim.Keyboard.TextEntry("4");
                //  Thread.Sleep(5000); 
                //  sim.Keyboard.TextEntry("4");

                    IWebElement inputFelt = driver.FindElement(By.Id("honeypot"));
                    inputFelt.SendKeys("4+2");
                    Thread.Sleep(5000); 

                    // MouseHook m;
                    //  Actions action = new Actions(driver); 
                    // action.SendKeys("4").Perform();
                    //action.KeyDown("4").sendKeys(String.valueOf('\u0061')).perform();
                    
                    Thread.Sleep(1000); 
                    inputFelt.SendKeys(Keys.Enter);
                    Thread.Sleep(1000); 
                    // Console.WriteLine(driver.PageSource.ToLower());

                    driver.SwitchTo().ParentFrame();
                    Thread.Sleep(1000);
                }

                LoggUt();
            } catch (Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Test Vimeo avspilling")]
        public void TestVimeoAvspilling()
        {
            if(GlobalVariables.ErProd())
            {
                Assert.Ignore(); // Skal kun testes på Stage
            }
            
            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, feidePw);

                GaaTilSeleniumFag();
                
                driver.FindElement(By.XPath("//span[starts-with(., 'Vimeo')]")).Click();
                HaandterMacSafari();
                if(bsCaps.device != null && bsCaps.device.Contains("iPad"))
                {
                    IWebElement iFrameVimeo = driver.FindElement(By.TagName("iframe"));

                    Assert.IsTrue(iFrameVimeo.Displayed);

                    // driver.SwitchTo().Frame(iFrameVimeo); // Fungerer ikke på automatisert iPad
                    Thread.Sleep(5000);
                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);
                } else
                {
                    IWebElement iFrameVimeo = driver.FindElement(By.TagName("iframe"));
                    Assert.IsTrue(iFrameVimeo.Displayed);
                    driver.SwitchTo().Frame(iFrameVimeo);
                    Thread.Sleep(1000); 
                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);

                    driver.FindElement(By.ClassName("state-paused")).Click();
                    Thread.Sleep(10000); // La video snurre litt
                    driver.FindElement(By.ClassName("state-playing")).Click();

                    driver.SwitchTo().ParentFrame();
                    Thread.Sleep(1000);
                }

                LoggUt();
            } catch (Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        private void LoggInnMedFeide(string brukernavn, string passord)
        {
            HaandterMacSafari();
            if(driver.PageSource.ToLower().Contains("innlogget bruker"))
            {
                LogWriter.LogWrite("Var allerede logget inn, forsøker å logge ut...");
                LoggUt();
                LogWriter.LogWrite("Logget ut ok");
            }
            HaandterMacSafari();
            driver.FindElement(By.LinkText("Logg inn")).Click();

            HaandterMacSafari();

            if(driver.FindElements(By.ClassName("dl-linkbutton")).Count > 0)
            { // Flere login-knapper: Klikker den første for å komme videre
                driver.FindElements(By.ClassName("dl-linkbutton"))[0].Click();
            }

            HaandterMacSafari();
            Thread.Sleep(2000);

            if(GlobalVariables.ErStage())
            {
                driver.FindElement(By.Id("dropdownlist")).Click();
                driver.FindElements(By.TagName("option"))[1].Click();
                driver.FindElement(By.ClassName("btn")).Click();
                HaandterMacSafari();
            }

            if(driver.FindElements(By.Id("username")).Count == 0 || !driver.FindElement(By.Id("username")).Displayed)
            {
                IWebElement orgSelector = driver.FindElement(By.Id("org_selector_filter"));
                orgSelector.SendKeys("Feide"); // For testing mot dataporten bruk "Tjenesteleverandør"
  
                driver.FindElement(By.XPath("//span[.='Feide']")).Click();

                driver.FindElement(By.Id("selectorg_button")).Click();
            }

            HaandterMacSafari();
            driver.FindElement(By.Id("username")).SendKeys(brukernavn);
            driver.FindElement(By.Id("password")).SendKeys(passord);

            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            HaandterMacSafari();

            if(GlobalVariables.ErStage())
            {
               driver.FindElement(By.XPath("//button[@type='submit']")).Click(); 
               HaandterMacSafari();
            }
            Thread.Sleep(5000); // Lar systemet få logge bruker inn
            driver.Navigate().GoToUrl(GlobalVariables.digilaerSkoleUrl + "/my/index.php?" + sprakUrl);
            
            HaandterMacSafari();
            Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker"), Is.True,  "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
        }

        private void LoggUt()
        {
            HaandterMacSafari();
            
            AapneBrukerMeny();

            HaandterMacSafari();
            driver.FindElement(By.XPath("//span[.='Logg ut']")).Click();
            HaandterMacSafari();
        }

        private void AapneBrukerMeny()
        {
            try
            {
                driver.FindElement(By.ClassName("avatars")).Click();
            } catch(Exception e)
            {
                driver.FindElement(By.ClassName("usermenu")).Click();
            }
        }

        private void GaaTilDigilaer()
        {
            driver.Navigate().GoToUrl(GlobalVariables.digilaerUrl);
            Thread.Sleep(2000); // Kan ta litt tid før cookie-knapp plutselig kommer
            ReadOnlyCollection<IWebElement> agreeKnappeListe = driver.FindElements(By.ClassName("agree-button"));
            if(agreeKnappeListe.Count > 0 && agreeKnappeListe[0].Enabled)
            {
                try{
                    agreeKnappeListe[0].Click();
                } catch(Exception e)
                {
                    Console.WriteLine("Aksepter cookies lot se ikke klikke. Antall cookie-knapper: " + agreeKnappeListe.Count);
                }
            }
        }

        private void GaaTilSkoleDigilaer()
        {
            driver.Navigate().GoToUrl(GlobalVariables.digilaerSkoleUrl + "/?" + sprakUrl);
            ReadOnlyCollection<IWebElement> agreeKnappeListe = driver.FindElements(By.ClassName("eupopup-button"));
            if(agreeKnappeListe.Count > 0)
            {
                agreeKnappeListe[0].Click();
            }
        }

        private void GaaTilSeleniumFag()
        {
            HaandterMacSafari();

            IWebElement hamburgerButton = driver.FindElement(By.XPath("//button[@data-action='toggle-drawer']"));

            if(hamburgerButton.GetAttribute("aria-expanded").Equals("false"))
            {
                hamburgerButton.Click();
            }

            ReadOnlyCollection<IWebElement> merKnapper = driver.FindElements(By.XPath("//a[@data-key='courseindexpage']"));
            if(merKnapper.Count > 0)
            {
                merKnapper[0].Click();
            }

            driver.FindElement(By.XPath("//span[.='" + fagkodeSelenium + "']")).Click();
            HaandterMacSafari();
        }

        private void MaksimerVindu()
        {
            if(bsCaps.os != null && (bsCaps.os.Equals("Windows") || bsCaps.os.Equals("OS X")))
            {
                driver.Manage().Window.Maximize();
            }
        }

        private void HaandterMacSafari()
        {
            // Safari webdriver mac os respekterer ikke webdriver wait (!?)
            // https://developer.apple.com/forums/thread/106693
            // TODO: Vurder behov for denne metoden, eller se etter bedre fiks

            if((bsCaps.browser != null && bsCaps.browser.Equals("Safari") &&
                bsCaps.os != null && bsCaps.os.Equals("OS X"))
                || (bsCaps.browser != null && bsCaps.browser.Equals("Firefox"))
                || (bsCaps.browser != null && bsCaps.browser.Equals("Safari") && bsCaps.device != null && bsCaps.device.Contains("iPhone"))
                || (bsCaps.device != null && bsCaps.device.Contains("iPad"))
                || (bsCaps.device != null && bsCaps.device.Contains("S10")) // Feilet ved login ifbm adobeconnect
                || (bsCaps.device != null && bsCaps.device.Contains("OnePlus 9")) // Feilet ifbm klikke på lenke for å gå inn på zoom
                )
            {
                Thread.Sleep(3000);
            }
        }

        private void HaandterAlert()
        {
            Thread.Sleep(2000);
            try
            {
                driver.SwitchTo().Alert().Accept();
            } catch(Exception e)
            {
                Console.WriteLine("Alert håndtering exception catched");
            }
        }

        private void HaandterFeiletTest(Exception e, string testnavn)
        {
            Printscreen.TakeScreenShot(driver, testnavn);
            LogWriter.LogWrite(testnavn + " feilet. Stacktrace:\n" + e.StackTrace);
            
            HaandterAlert();
            GaaTilSkoleDigilaer();
            try {
                LoggUt();
            } catch(Exception ex)
            {
                LogWriter.LogWrite("Feilet ved utlogging i håndtering av en feilet test. Var kanskje ikke innlogget?");
            }
            throw e;
        }
    }
}