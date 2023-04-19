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
using OpenQA.Selenium.Firefox;

namespace TestSuite
{
    [TestFixture(DeviceConfig.OSXBigSurEdge)]
    [TestFixture(DeviceConfig.OSXBigSurChrome)]
    [TestFixture(DeviceConfig.OSXBigSurFirefox)]
    [TestFixture(DeviceConfig.OSXBigSurSafari)]
    [TestFixture(DeviceConfig.IOSIphone)]
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
        private string fagkodeSelenium = "Selenium";
        private string facultyEmployeeLaererFnr = System.Environment.GetEnvironmentVariable("DIGI_USER_FACULTY");
        private string facultyEmployeeLaererPW = System.Environment.GetEnvironmentVariable("DIGI_USER_FACULTY_PW");
        private string studentUnder18Fnr = System.Environment.GetEnvironmentVariable("DIGI_ELEV_UNDER_ATTEN");
        private string studentUnder18PW = System.Environment.GetEnvironmentVariable("DIGI_ELEV_UNDER_ATTEN_PW");
        private string studentOver18Fnr = System.Environment.GetEnvironmentVariable("DIGI_ELEV_OVER_ATTEN");
        private string studentOver18PW = System.Environment.GetEnvironmentVariable("DIGI_ELEV_OVER_ATTEN_PW");
        private string resultatTekst = "";
        private int enhetIdForDB;
        private int funkTestIdForDB;
        private DateTime teststartForDB;
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
            } else if(deviceConfig == DeviceConfig.IOSIphone)
            {
                bsCaps = new BrowserStackCapabilities{device = "iPhone 13", browser = "Safari", osVersion = "15", realMobile = "true"};
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

        //   driver = seleniumSetup.GetFirefoxDriver();  // For lokal testing
             driver = seleniumSetup.GetBrowserstackDriver(bsCaps);
        }

        [OneTimeSetUp]
        public void Init()
        {
            LogWriter.LogWrite("Starter seleniumtest på en device i " + GlobalVariables.Miljo);
            if(GlobalVariables.ErProd() && GlobalVariables.ErScheduled() && GlobalVariables.SkalLoggeTilDatabase())
            {
                enhetIdForDB =  MonitorApiClient.FindOrCreateEnhetOppsett(new EnhetOppsett{
                    enhet = bsCaps.device, nettleserNavn = bsCaps.browser, nettleserVersjon = bsCaps.browserVersion,
                    osNavn = bsCaps.os, osVersjon = bsCaps.osVersion, opplosning = bsCaps.resolution
                });
            }
            
            MaksimerVindu();
        }

        [SetUp]
        public void BeforeEachTest()
        {
            LogWriter.LogToBrowserStack(driver, "Starter " + TestContext.CurrentContext.Test.MethodName);
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
                if(GlobalVariables.ErProd()  && GlobalVariables.ErScheduled() && GlobalVariables.SkalLoggeTilDatabase())
                {
                    resultatTekst += "\nKanskje <@joakimbjerkheim> eller <@mathias.meier.nilsen> tar en titt?";
                }
                if (driver.GetType() != typeof(FirefoxDriver)) {((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Test feilet. \"}}");}
            } else if(TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Warning) 
            {
                resultatTekst = oppsettTekst + ":\n"
                + ":white_check_mark:" + (TestContext.CurrentContext.Result.PassCount + TestContext.CurrentContext.Result.WarningCount) + " tester kjørt ok!:ok_hand:\n"
                + resultatTekst;
                if (driver.GetType() != typeof(FirefoxDriver)) {((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \" Test OK. \"}}");}
            } else
            {
                resultatTekst = oppsettTekst + ":\n"
                + ":white_check_mark:" + "Alle " + (TestContext.CurrentContext.Result.PassCount + TestContext.CurrentContext.Result.WarningCount) + " tester kjørt ok!:ok_hand:\n"
                + resultatTekst;
                if (driver.GetType() != typeof(FirefoxDriver)) {((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \" Test OK. \"}}");}
            }

            SlackClient.CallSlack(resultatTekst);
        }

        [TearDown]
        public void AfterEachTest()
        {
            LogWriter.LogToBrowserStack(driver, TestContext.CurrentContext.Test.MethodName + " ferdig.");
            if(GlobalVariables.ErProd()  && GlobalVariables.ErScheduled() && GlobalVariables.SkalLoggeTilDatabase())
            {
                funkTestIdForDB = MonitorApiClient.FindOrCreateFunksjonellTest(TestContext.CurrentContext.Test.MethodName, TestContext.CurrentContext.Test.Name);

                string debugInfo = TestContext.CurrentContext.Result.Message + TestContext.CurrentContext.Result.StackTrace;
                if(debugInfo != null && debugInfo.Length > 1300)
                {
                    debugInfo = debugInfo.Substring(0, 300) + "[stripped]" + debugInfo.Substring(debugInfo.Length - 700, 700);
                }

                if((int)TestContext.CurrentContext.Result.Outcome.Status != 1) // Lagrer ikke skippede/ignorerte tester i DB
                {
                    MonitorApiClient.PostTestkjoring(new Testkjoring{
                        enhetOppsettId = enhetIdForDB, funksjonellTestId = funkTestIdForDB, resultatId = (int)TestContext.CurrentContext.Result.Outcome.Status,
                        starttid = teststartForDB, sluttid = DateTime.Now,
                        debugInformasjon = ""});
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

                String kildekode = driver.PageSource.ToLower();

                Assert.That(kildekode.Contains("feide"), Is.True);
                Assert.That(kildekode.Contains("digilær"), Is.True);
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Målform kan endres")]
        public void TestAtMaalFormKanEndres()
        {
            if(GlobalVariables.ErStage())
            {                
                Assert.Ignore(); // Skip stage siden målform ikke er relevant på startside der
            }
            try
            {
                driver.Navigate().GoToUrl(GlobalVariables.DigilaerUrl);
                
                if(GlobalVariables.ErProd())
                {
                    if(driver.FindElement(By.Id("language-switcher")).Displayed)
                        {
                            driver.FindElement(By.Id("language-switcher")).Click();
                        }
                        driver.FindElement(By.LinkText("Nynorsk")).Click();
                } else
                {
                    if(driver.FindElement(By.Id("lang-menu-toggle")).Displayed)
                    {
                        driver.FindElement(By.Id("lang-menu-toggle")).Click();
                    }
                    driver.FindElement(By.PartialLinkText("nynorsk")).Click();
                }
                HaandterMacSafari();

                if(GlobalVariables.ErProd())
                {
                    Assert.That(driver.PageSource.ToLower().Contains("moglegheiter"), Is.True);
                    if(driver.FindElement(By.Id("language-switcher")).Displayed)
                    {
                        driver.FindElement(By.Id("language-switcher")).Click();
                    }
                    driver.FindElement(By.LinkText("Bokmål")).Click();
                } else
                {
                    if(driver.FindElement(By.Id("lang-menu-toggle")).Displayed)
                    {
                        driver.FindElement(By.Id("lang-menu-toggle")).Click();
                    }
                    driver.FindElement(By.PartialLinkText("bokmål")).Click();
                }

                HaandterMacSafari();
                if(GlobalVariables.ErProd())
                {
                    Assert.That(driver.PageSource.ToLower().Contains("muligheter"), Is.True);
                    driver.Navigate().GoToUrl(GlobalVariables.DigilaerUrl + "/nb/om-digilaerno");
                }
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

                String kildekode = driver.PageSource.ToLower();

                Assert.That(kildekode.Contains("feide"), Is.True);
                Assert.That(kildekode.Contains("digilær"), Is.True);

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
                LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW);
                Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker") || driver.PageSource.ToLower().Contains("velkommen tilbake"), Is.True,  
                    "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
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
                LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW);
                Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker") || driver.PageSource.ToLower().Contains("velkommen tilbake"), Is.True,  
                    "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
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
                LoggInnMedFeide(facultyEmployeeLaererFnr, facultyEmployeeLaererPW);
                Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker") || driver.PageSource.ToLower().Contains("velkommen tilbake"), Is.True,  
                    "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
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
                LoggInnMedFeide(studentOver18Fnr, studentOver18PW);

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
                LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW);
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
        [TestCase(TestName = "Lærer kan redigere fag")]
        public void TestAtLaererKanRedigereFag()
        {
            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(facultyEmployeeLaererFnr, facultyEmployeeLaererPW);
                GaaTilSeleniumFag();
                driver.FindElement(By.LinkText("Kurs")).Click();
                Thread.Sleep(3000);
                driver.FindElement(By.XPath("//button[.='Slå redigering på']")).Click();
                Thread.Sleep(2000); // HaandterMacSafari(); // For element stale exception on appium iphone.
                ReadOnlyCollection<IWebElement> redigerknapper = driver.FindElements(By.XPath("//i[@aria-label='Rediger']"));
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
            try
            {
                TestAdobeConnect(facultyEmployeeLaererFnr, facultyEmployeeLaererPW);
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
        
        private void TestAdobeConnect(string fnr, string pw)
        {
            GaaTilSkoleDigilaer();
            LoggInnMedFeide(fnr, pw);
            GaaTilSeleniumFag();
            
            driver.FindElement(By.XPath("//a[.//span[starts-with(., 'SELENIUM test Adobe Connect')]]")).Click();
            int retries = 0; // For adobeconnect-hikke
            string moteUrl = null;

            HaandterMacSafari();
            while(moteUrl == null && retries < 5)
            {
                try
                {
                    moteUrl = driver.FindElement(By.XPath("//input[@value='Join Meeting']")).GetAttribute("onclick");
                } catch(Exception)
                {
                    retries++;
                    Thread.Sleep(15000);
                }
            }
            if(retries > 0)
            {
                LogWriter.LogToBrowserStack(driver, "AdobeConnect trengte " + retries + " forsøk, av 5 mulige.");
                LogWriter.LogToBrowserStack(driver, "Møteurl: " + moteUrl);
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
                Assert.True(source.Contains("meetingAreaCanvas"), "Siden inneholder ikke meetingAreaCanvas");

                try
                {
                    if(driver.FindElements(By.Id("download-app-notifier_1")).Count > 0 && driver.FindElement(By.Id("download-app-notifier_1")).Displayed)
                    {
                        driver.FindElement(By.Id("download-app-notifier_1")).Click();
                    }
                } catch(WebDriverException e)
                {
                    LogWriter.LogWrite("download-app-notifier_1 timeout " + e);
                    if(!erMacSafari()) {throw e;}
                }
                 
                if(driver.FindElements(By.XPath("//span[.='Close']")).Count > 0 && driver.FindElement(By.XPath("//span[.='Close']")).Displayed)
                {
                    driver.FindElement(By.XPath("//span[.='Close']")).FindElement(By.XPath("./..")).Click();
                }

                HaandterMacSafari();
                try
                {
                    if(driver.FindElements(By.XPath("//span[.='Display Media']")).Count > 0 && driver.FindElement(By.XPath("//span[.='Display Media']")).Displayed)
                    {
                        driver.FindElement(By.XPath("//span[.='Display Media']")).FindElement(By.XPath("./..")).Click();
                    }
                } catch(WebDriverException e)
                {
                    LogWriter.LogWrite("Display media " + e);
                    if(!erMacSafari()) {throw e;}
                }

                HaandterMacSafari();
                try
                {
                    if(driver.FindElements(By.Id("productMaintenanceNotifier_1")).Count > 0 && driver.FindElement(By.Id("productMaintenanceNotifier_1")).Displayed)
                    {
                        driver.FindElement(By.Id("productMaintenanceNotifier_1")).Click();
                    }
                } catch(WebDriverException e)
                {
                    LogWriter.LogWrite("productMaintenanceNotifier_1 timeout " + e);
                    if(!erMacSafari()) {throw e;}
                }

                Assert.True(source.Contains("meetingAreaCanvas"), "Siden inneholder ikke meetingAreaCanvas");
                
                driver.SwitchTo().ParentFrame();
            } else
            {
                // TODO: Implementer hvis støttet fra driver:
                // Thread.Sleep(10000);
                // if(driver.FindElements(By.XPath("//*[text='Open in Browser']")).Count > 0) {
                //     driver.FindElement(By.XPath("//*[text='Open in Browser']")).Click();
                //     Thread.Sleep(10000);
                // } else {
                //     Assert.True(driver.PageSource.Contains("Use the mobile app to join a room"));       
                // }
            }
            GaaTilDigilaer();
            HaandterAlert();
            HaandterMacSafari();

            LoggUt();
        }

        private bool erMacSafari()
        {
            return bsCaps.browser != null && bsCaps.browser.Equals("Safari") && bsCaps.os != null && bsCaps.os.Equals("OS X");
        }

        [Test]
        [TestCase(TestName = "AdobeConnect som elev")]
        public void TestAdobeConnectElev()
        {
            try
            {
                TestAdobeConnect(studentUnder18Fnr, studentUnder18PW);
            } catch(Exception exception)
            {
                HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        [Test]
        [TestCase(TestName = "Test Poodl MiniLesson diktat")]
        public void TestPoodlMinilessonDiktat()
        {
            if(GlobalVariables.ErProd() || (bsCaps.device != null && bsCaps.device.Contains("Tab S7")))
            {
                Assert.Ignore(); // Skal ikke testes i Prod, og S7 støttes ikke
            }

            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW);

                GaaTilSeleniumFag();
                
                driver.FindElement(By.XPath("//a[.//span[starts-with(., 'Poodll')]]")).Click();
                Thread.Sleep(3000);
                if(driver.FindElements(By.ClassName("btn_finished_attempt")).Count > 0 &&
                        driver.FindElement(By.ClassName("btn_finished_attempt")).Displayed)
                {
                    driver.FindElement(By.ClassName("btn_finished_attempt")).Click();
                    Thread.Sleep(3000);
                    
                    driver.FindElement(By.XPath("//button[@data-action='save']")).Click();
                    HaandterMacSafari();
                }
                
                Thread.Sleep(2000);
                driver.FindElement(By.ClassName("fa-play")).Click();
                Thread.Sleep(500);
                Assert.True(driver.FindElement(By.ClassName("fa-spinner")).Displayed);
                Thread.Sleep(10000);
                if((bsCaps.device != null && 
                    (bsCaps.device.Contains("iPad") || bsCaps.device.Contains("iPhone") || bsCaps.device.Contains("Pixel 4") || bsCaps.device.Contains("OnePlus 9") || bsCaps.device.Contains("Galaxy S21") || bsCaps.device.Contains("Galaxy S10")))
                    || (bsCaps.os != null && (bsCaps.os.Equals("Windows") && (bsCaps.browser.Equals("Chrome") || bsCaps.browser.Equals("Edge"))) 
                    || (bsCaps.os.Equals("OS X") && bsCaps.browser.Equals("Chrome") || bsCaps.browser.Equals("Edge"))
                    )
                )
                {
                    // Spinner forsvinner ikke ved automatisert test på disse enhetene
                } else {
                    Assert.True(driver.FindElement(By.ClassName("fa-play")).Displayed);
                }

                IWebElement inputFelt = driver.FindElement(By.XPath("//input[@maxlength='6']"));
                inputFelt.SendKeys("alw");
                Thread.Sleep(2000);
                IWebElement feedback = driver.FindElement(By.ClassName("dictate-feedback"));
                Assert.True(feedback.GetAttribute("style").Equals("color: red;"));

                inputFelt.SendKeys("ays");
                Thread.Sleep(1000);
                Assert.True(feedback.GetAttribute("style").Equals("color: green;"));
                
                driver.FindElement(By.ClassName("minilesson_nextbutton")).Click();
                Thread.Sleep(3000);

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
                Assert.Ignore(); // Skal ikke testes i Prod
            }

            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW);

                GaaTilSeleniumFag();
                
                driver.FindElement(By.XPath("//a[.//span[starts-with(., '1 Tall og siffer')]]")).Click();
                HaandterMacSafari();

                if(driver.FindElements(By.XPath("//button[.='Fortsett med forrige forsøk']")).Count > 0)
                {
                    driver.FindElement(By.XPath("//button[.='Fortsett med forrige forsøk']")).Click();
                } else if(driver.FindElements(By.XPath("//button[.='Ta quizen']")).Count > 0) {
                    driver.FindElement(By.XPath("//button[.='Ta quizen nå']")).Click();
                } else {
                    driver.FindElement(By.XPath("//button[.='Fortsett siste forhåndsvisning']")).Click();
                }

                HaandterMacSafari();
                IWebElement ordEn = driver.FindElement(By.XPath("//span[starts-with(., 'will')]"));

                IWebElement slippFeltEn = driver.FindElements(By.ClassName("droptarget"))[0]; 

                // TODO:
                // Dra ord inn i felter
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
            if(GlobalVariables.ErProd() || (bsCaps.device != null && bsCaps.device.Contains("iPhone")))
            {
                Assert.Ignore(); // Skal ikke testes i Prod
            }
            
            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW);
                
                GaaTilSeleniumFag();
                driver.FindElement(By.XPath("//a[.//span[starts-with(., 'Trinket')]]")).Click();
                HaandterMacSafari();

                if(bsCaps.device != null && bsCaps.device.Contains("iPad"))
                {
                    IWebElement iFrameTrinket = driver.FindElement(By.TagName("iframe"));

                    // iFrame fungerer ikke på automatisert iPad
                    Assert.IsTrue(iFrameTrinket.Displayed);
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

                    // TODO: Sende tall
                    IWebElement inputFelt = driver.FindElement(By.Id("honeypot"));
                    inputFelt.SendKeys("4+2");
                    Thread.Sleep(5000); 
                    
                    inputFelt.SendKeys(Keys.Enter);
                    Thread.Sleep(1000); 

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
            if(GlobalVariables.ErProd() || (bsCaps.device != null && bsCaps.device.Contains("iPhone")))
            {
                Assert.Ignore(); // Skal ikke testes i Prod, og støttes ikke på iPhone
            }
            
            try
            {
                GaaTilSkoleDigilaer();
                LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW);

                GaaTilSeleniumFag();
                
                driver.FindElement(By.XPath("//a[.//span[starts-with(., 'Vimeo')]]")).Click();
                HaandterMacSafari();
                if(bsCaps.device != null && bsCaps.device.Contains("iPad"))
                {
                    IWebElement iFrameVimeo = driver.FindElement(By.TagName("iframe"));

                    Assert.IsTrue(iFrameVimeo.Displayed);

                    Thread.Sleep(5000);
                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);
                } else
                {
                    IWebElement iFrameVimeo = driver.FindElement(By.TagName("iframe"));
                    Assert.IsTrue(iFrameVimeo.Displayed);
                    driver.SwitchTo().Frame(iFrameVimeo);
                    Thread.Sleep(1000); 
                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);

                    driver.FindElement(By.XPath("//button[@aria-label='Play']")).Click();
                    Thread.Sleep(300); // La video snurre litt
                    driver.FindElement(By.XPath("//button[@aria-label='Pause']")).Click();

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

            IWebElement feideLogin = driver.FindElement(By.ClassName("feide-login"));
            feideLogin.FindElement(By.TagName("a")).Click();

            HaandterMacSafari();

            ReadOnlyCollection<IWebElement> loginButtons = driver.FindElements(By.ClassName("dl-linkbutton"));
            if(loginButtons.Count > 0)
            {
                loginButtons[0].Click();
            }

            HaandterMacSafari();
            Thread.Sleep(2000);

            if(driver.FindElements(By.Id("username")).Count == 0 || !driver.FindElement(By.Id("username")).Displayed)
            {
                IWebElement orgSelector = driver.FindElement(By.Id("org_selector_filter"));
                orgSelector.SendKeys("Utdanningsdirektoratet - systemorganisasjon"); // For testing mot dataporten: Bruk "Tjenesteleverandør"
                driver.FindElement(By.XPath("//span[.='Utdanningsdirektoratet - systemorganisasjon']")).Click();
                driver.FindElement(By.Id("selectorg_button")).Click();
            }

            HaandterMacSafari();
            driver.FindElement(By.Id("username")).SendKeys(brukernavn);
            driver.FindElement(By.Id("password")).SendKeys(passord);

            Thread.Sleep(3000);
            driver.FindElement(By.XPath("//button[@type='submit']")).Click(); // Sleep før og etter for android-devices
            Thread.Sleep(3000);
            HaandterMacSafari();
            if(bsCaps.browser.Equals("Safari") && bsCaps.os != null && bsCaps.os.Equals("OS X")) {
                Thread.Sleep(10000);
            }

            HaandterSamtykke();

            HaandterMacSafari();
        }

        private void LoggUt()
        {
            HaandterMacSafari();
            
            AapneBrukerMeny();

            Thread.Sleep(1000);
            driver.FindElement(By.LinkText("Logg ut")).Click();
            HaandterMacSafari();
        }

        private void AapneBrukerMeny()
        {
            Thread.Sleep(1000);
            driver.FindElement(By.Id("user-menu-toggle")).Click();
            Thread.Sleep(1000);
        }

        private void GaaTilDigilaer()
        {
            if(GlobalVariables.ErProd())
            {
                driver.Navigate().GoToUrl(GlobalVariables.DigilaerSkoleUrl + "/my/index.php?" + sprakUrl);
            } else {
                driver.Navigate().GoToUrl(GlobalVariables.DigilaerUrl);
            }

            Thread.Sleep(2000);
            HaandterAlert();
            Thread.Sleep(2000);
            ReadOnlyCollection<IWebElement> agreeKnappeListe = driver.FindElements(By.ClassName("agree-button"));
            if(agreeKnappeListe.Count > 0 && agreeKnappeListe[0].Enabled)
            {
                try{
                    agreeKnappeListe[0].Click();
                } catch(Exception)
                {
                    LogWriter.LogToBrowserStack(driver, "Aksepter cookies lot se ikke klikke. Antall cookie-knapper: " + agreeKnappeListe.Count);
                }
            }
        }

        private void GaaTilSkoleDigilaer()
        {
            driver.Navigate().GoToUrl(GlobalVariables.DigilaerSkoleUrl);
            ReadOnlyCollection<IWebElement> agreeKnappeListe = driver.FindElements(By.ClassName("eupopup-button"));
            if(agreeKnappeListe.Count > 0)
            {
                agreeKnappeListe[0].Click();
            }
        }

        private void GaaTilSeleniumFag()
        {
            HaandterMacSafari();
            AapneBrukerMeny();
            driver.FindElement(By.LinkText("Profil")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.LinkText(fagkodeSelenium)).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.LinkText("Kurs")).Click();
            HaandterMacSafari();
        }

        private void MaksimerVindu()
        {
            if(bsCaps.os != null && (bsCaps.os.Equals("Windows") || bsCaps.os.Equals("OS X")))
            {
                driver.Manage().Window.Maximize();
            }
        }

        private void HaandterSamtykke()
        {
            if(driver.PageSource.ToLower().Contains("godta samtykke"))
            {
                driver.FindElement(By.XPath("//input[navn='status10']")).Click();
                driver.FindElement(By.XPath("//button[@type='submit']")).Click();
                GaaTilDigilaer();
            }
            if(driver.FindElements(By.XPath("//button[text='Avslutt veileder']")).Count > 0)
            {
                driver.FindElement(By.XPath("//button[text='Avslutt veileder']")).Click();
            }
        }

        private void HaandterMacSafari()
        {
            // Safari webdriver mac os respekterer ikke webdriver wait (!?)
            // https://developer.apple.com/forums/thread/106693
            // TODO: Vurder behov for denne metoden, eller lag bedre fiks

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
            } catch(Exception)
            {
                LogWriter.LogToBrowserStack(driver, "Alert håndtering exception catched");
            }
        }

        private void HaandterFeiletTest(Exception e, string testnavn)
        {
            LogWriter.LogWrite(testnavn + " feilet. Stacktrace:\n" + e.StackTrace);
            LogWriter.LogToBrowserStack(driver, testnavn + " feilet");
            Printscreen.TakeScreenShot(driver, testnavn);
            
            HaandterAlert();
            GaaTilSkoleDigilaer();
            try {
                LoggUt();
            } catch(Exception)
            {
                LogWriter.LogWrite("Feilet ved utlogging i håndtering av en feilet test. Var kanskje ikke innlogget?");
            }
            throw e;
        }
    }
}