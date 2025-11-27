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
using static Utils.Navigation;

namespace TestSuite
{
    // [TestFixture(DeviceConfig.OSXBigSurSafari)]
    // [TestFixture(DeviceConfig.OSXMontereySafari)]
    // [TestFixture(DeviceConfig.OSXVenturaSafari)]
    [TestFixture(DeviceConfig.OSXVenturaChrome)]
    [TestFixture(DeviceConfig.OSXVenturaFirefox)]
    [TestFixture(DeviceConfig.OSXVenturaEdge)]
    [TestFixture(DeviceConfig.Win10Chrome)]
    [TestFixture(DeviceConfig.Win10Firefox)]
    [TestFixture(DeviceConfig.Win10Edge)]
    [TestFixture(DeviceConfig.Win11Chrome)]
    [TestFixture(DeviceConfig.Win11Edge)]
    [TestFixture(DeviceConfig.IOSIphone)]
    [TestFixture(DeviceConfig.Ipad11Pro2020)]
    [TestFixture(DeviceConfig.Ipad10th)]
    // [TestFixture(DeviceConfig.AndroidGalaxyS23Ultra)]
    // [TestFixture(DeviceConfig.GooglePixel7Pro)]
    // [TestFixture(DeviceConfig.AndroidGalaxyTabS7)]
    // [TestFixture(DeviceConfig.AndroidGalaxyS21)]
    // [TestFixture(DeviceConfig.AndroidOnePlus9)]
    // [TestFixture(DeviceConfig.SamsungGalaxyS10)]
    // [TestFixture(DeviceConfig.GooglePixel4XL)]
    public class LoginTests
    {
        private IWebDriver driver;
        private BrowserStackCapabilities bsCaps;
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

        public LoginTests(DeviceConfig deviceConfig)
        {
              if(deviceConfig == DeviceConfig.Win10Edge)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Windows", osVersion = "10", browser = "Edge", browserVersion = "latest"};
              } else if(deviceConfig == DeviceConfig.Win10Chrome)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Windows", osVersion = "10", browser = "Chrome", browserVersion = "latest"};
              } else if(deviceConfig == DeviceConfig.Win10Firefox)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Windows", osVersion = "10", browser = "Firefox", browserVersion = "latest"};
              } else if(deviceConfig == DeviceConfig.Win11Chrome)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Windows", osVersion = "11", browser = "Chrome", browserVersion = "latest"};
              } else if(deviceConfig == DeviceConfig.Win11Edge)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Windows", osVersion = "11", browser = "Edge", browserVersion = "latest"};
              } else if(deviceConfig == DeviceConfig.OSXBigSurSafari)
              {
                  bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Big Sur", browser = "Safari", browserVersion = "latest", resolution = "1920x1080"};
              } else if(deviceConfig == DeviceConfig.OSXMontereySafari)
              {
                  bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Monterey", browser = "Safari", browserVersion = "latest", resolution = "1920x1080"};
              } else if(deviceConfig == DeviceConfig.OSXVenturaSafari)
              {
                  bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Ventura", browser = "Safari", browserVersion = "latest", resolution = "1920x1080"};
              } else if(deviceConfig == DeviceConfig.OSXVenturaFirefox)
              {
                  bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Ventura", browser = "Firefox", browserVersion = "latest", resolution = "1920x1080"};
              } else if(deviceConfig == DeviceConfig.OSXVenturaChrome)
              {
                  bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Ventura", browser = "Chrome", browserVersion = "latest", resolution = "1920x1080"};
              } else if(deviceConfig == DeviceConfig.OSXVenturaEdge)
              {
                  bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Ventura", browser = "Edge", browserVersion = "latest", resolution = "1920x1080"};
              } else if(deviceConfig == DeviceConfig.Ipad11Pro2020)
              {
                  bsCaps = new BrowserStackCapabilities{os = "IOS", device = "iPad Pro 11 2020", browser = "Safari", osVersion = "13", realMobile = "true", local = "false"};
              } else if(deviceConfig == DeviceConfig.Ipad10th){
                  bsCaps = new BrowserStackCapabilities{os = "IOS", device = "iPad 10th", browser = "Safari", osVersion = "16", realMobile = "true", local = "false"};
              } else if(deviceConfig == DeviceConfig.IOSIphone)
              {
                  bsCaps = new BrowserStackCapabilities{os = "IOS", device = "iPhone 14", browser = "Safari", osVersion = "16", realMobile = "true"};
              } else if(deviceConfig == DeviceConfig.AndroidGalaxyS23Ultra)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Android", device = "Samsung Galaxy S23 Ultra", browser = "Chrome", osVersion = "13.0", realMobile = "true", local = "false"};
              } else if(deviceConfig == DeviceConfig.GooglePixel7Pro)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Android", device = "Pixel 7 Pro", browser = "Chrome", osVersion = "12.0", realMobile = "true", local = "false"};
              } else if(deviceConfig == DeviceConfig.AndroidGalaxyTabS7)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Android", device = "Samsung Galaxy Tab S7", browser = "Chrome", osVersion = "10.0", realMobile = "true", local = "false"};
              }
              else if(deviceConfig == DeviceConfig.AndroidGalaxyS20)
              {
                  // OBS: Frarådet å bruke denne fra bstack support da den i følge dem er ustabil.
                  bsCaps = new BrowserStackCapabilities{os = "Android", device = "Samsung Galaxy S20", browser = "Chrome", osVersion = "10.0", realMobile = "true", local = "false"};
              } else if(deviceConfig == DeviceConfig.AndroidGalaxyS21)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Android", device = "Samsung Galaxy S21", browser = "Chrome", osVersion = "11.0", realMobile = "true", local = "false"};
              } else if(deviceConfig == DeviceConfig.AndroidOnePlus9)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Android", device = "OnePlus 9", browser = "Chrome", osVersion = "11.0", realMobile = "true", local = "false"};
              } else if(deviceConfig == DeviceConfig.GooglePixel6)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Android", device = "Google Pixel 6", browser = "Chrome", osVersion = "12.0", realMobile = "true", local = "false"};
              } else if(deviceConfig == DeviceConfig.SamsungGalaxyS10)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Android", device = "Samsung Galaxy S10", browser = "Chrome", osVersion = "9.0", realMobile = "true", local = "false"};
              } else if(deviceConfig == DeviceConfig.GooglePixel4XL)
              {
                  bsCaps = new BrowserStackCapabilities{os = "Android", device = "Google Pixel 4 XL", browser = "Chrome", osVersion = "10.0", realMobile = "true", local = "false"};
              }

            // driver = SeleniumSetup.GetFirefoxDriver();  // For lokal testing
              driver = SeleniumSetup.GetBrowserstackDriver(bsCaps);
        }

        [OneTimeSetUp]
        public void Init()
        {
          if(!GlobalVariables.ErTiming())
          {
            LogWriter.LogWrite("Starter seleniumtest på en device i " + GlobalVariables.Miljo);
            if(GlobalVariables.ErProd() && GlobalVariables.SkalLoggeTilDatabase())
            {
                enhetIdForDB =  MonitorApiClient.FindOrCreateEnhetOppsett(new EnhetOppsett{
                    enhet = bsCaps.device, nettleserNavn = bsCaps.browser, nettleserVersjon = bsCaps.browserVersion,
                    osNavn = bsCaps.os, osVersjon = bsCaps.osVersion, opplosning = bsCaps.resolution
                });
            }
            
            Navigation.MaksimerVindu(driver, bsCaps);
          }
        }

        [SetUp]
        public void BeforeEachTest()
        {
          if(!GlobalVariables.ErTiming())
          {
            LogWriter.LogToBrowserStack(driver, "Starter " + TestContext.CurrentContext.Test.MethodName);
            teststartForDB = DateTime.Now;
          }
        }

        [OneTimeTearDown]
        public void AfterTestsOnOneDeviceIsFinished()
        {
          if(!GlobalVariables.ErTiming())
          {
            sendSlackResultat();
          LogWriter.LogWrite("Test ferdig.");
          }
          driver.Quit();
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
                    resultatTekst += "\nKanskje <@joakimbjerkheim> tar en titt?";
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
          if(!GlobalVariables.ErTiming())
          {
            LogWriter.LogToBrowserStack(driver, TestContext.CurrentContext.Test.MethodName + " ferdig.");
            if(GlobalVariables.ErProd() && GlobalVariables.SkalLoggeTilDatabase())
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
        }

        [Test]
        [TestCase(TestName = "Åpne digilær hovedside")]
        public void GaaTilDigilaerForside()
        {
          if(GlobalVariables.ErTiming()) {
            Assert.Ignore();
          }
          try
          {
            LogWriter.LogWrite("er stage " + GlobalVariables.ErStage());
            GaaTilDigilaer(driver);

            String kildekode = driver.PageSource.ToLower();

            Assert.That(kildekode.Contains("feide"), Is.True);
            Assert.That(kildekode.Contains("digilær"), Is.True);
          } catch(Exception exception)
          {
            HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        // [Test]
        // [TestCase(TestName = "Målform kan endres")]
        public void TestAtMaalFormKanEndres()
        {
          if(GlobalVariables.ErTiming() || GlobalVariables.ErStage())
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
              HaandterMacSafari(bsCaps);

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

              HaandterMacSafari(bsCaps);
              if(GlobalVariables.ErProd())
              {
                  Assert.That(driver.PageSource.ToLower().Contains("muligheter"), Is.True);
                  driver.Navigate().GoToUrl(GlobalVariables.DigilaerUrl + "/nb/om-digilaerno");
              }
          } catch(Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Åpne skole.digilær hovedside")]
        public void GaaTilSkoleDigiLaerForside()
        {
          if(GlobalVariables.ErTiming()) {
            Assert.Ignore();
          }
          try
          {
              GaaTilSkoleDigilaer(driver);

              String kildekode = driver.PageSource.ToLower();

              Assert.That(kildekode.Contains("feide"), Is.True);
              Assert.That(kildekode.Contains("digilær"), Is.True);

          } catch(Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Elev kan logge på med Feide via digilaer.no")]
        public void LoggInnOgUtAvDigilaerMedFeide()
        {
          if(GlobalVariables.ErTiming() || GlobalVariables.ErStage())
          {                
              Assert.Ignore(); // Skip test siden det ikke er stage-innlogging med feide fra annet sted enn hovedsiden
          }
          
          try
          {
              GaaTilDigilaer(driver);
              LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW, driver, bsCaps);
              Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker") || driver.PageSource.ToLower().Contains("velkommen tilbake"), Is.True,  
                  "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
              LoggUt(driver, bsCaps);
          } catch(Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Elev kan logge på med Feide via skole.digilær")]
        public void LoggInnOgUtAvSkoleDigilaerMedFeide()
        {
          if(GlobalVariables.ErTiming()) {
            Assert.Ignore();
          }
          try
          {
              GaaTilSkoleDigilaer(driver);
              HaandterMacSafari(bsCaps);
              LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW, driver, bsCaps);
              Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker") || driver.PageSource.ToLower().Contains("velkommen tilbake"), Is.True,  
                  "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
              LoggUt(driver, bsCaps);
          } catch (Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Lærer kan logge på med Feide")]
        public void LoggInnOgUSomLaererMedFeide()
        {
          if(GlobalVariables.ErTiming()) {
            Assert.Ignore();
          }
          try
          {
              GaaTilSkoleDigilaer(driver);
              LoggInnMedFeide(facultyEmployeeLaererFnr, facultyEmployeeLaererPW, driver, bsCaps);
              Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker") || driver.PageSource.ToLower().Contains("velkommen tilbake"), Is.True,  
                  "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
              LoggUt(driver, bsCaps);
          } catch (Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Elev har tilgang til fag")]
        public void SjekkAtElevHarTilgangTilFag()
        {
          if(GlobalVariables.ErTiming()) {
            Assert.Ignore();
          }
          try
          {
              GaaTilSkoleDigilaer(driver);
              LoggInnMedFeide(studentOver18Fnr, studentOver18PW, driver, bsCaps);

              GaaTilSeleniumFag(driver, bsCaps);
              string pageSource = driver.PageSource;

              Assert.That(pageSource.Contains("Oppslagstavle"), Is.True);
              LoggUt(driver, bsCaps);
          } catch (Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Elev kan ikke redigere fag")]
        public void SjekkAtElevIkkeKanRedigereFag()
        {
          if(GlobalVariables.ErTiming()) {
            Assert.Ignore();
          }
          try
          {
              GaaTilSkoleDigilaer(driver);
              LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW, driver, bsCaps);
              GaaTilSeleniumFag(driver, bsCaps);

              ReadOnlyCollection<IWebElement> redigeringsknapp = driver.FindElements(By.XPath("//button[.='Slå redigering på']"));
              Assert.That(redigeringsknapp.Count, Is.Zero);

              LoggUt(driver, bsCaps);
          } catch (Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Lærer kan redigere fag")]
        public void TestAtLaererKanRedigereFag()
        {
          if(GlobalVariables.ErTiming()) {
            Assert.Ignore();
          }
          try
          {
              GaaTilSkoleDigilaer(driver);
              LoggInnMedFeide(facultyEmployeeLaererFnr, facultyEmployeeLaererPW, driver, bsCaps);
              GaaTilSeleniumFag(driver, bsCaps);
              driver.FindElement(By.LinkText("Kurs")).Click();
              Thread.Sleep(3000);
              driver.FindElement(By.XPath("//button[.='Slå redigering på']")).Click();
              Thread.Sleep(2000); // HaandterMacSafari(); // For element stale exception on appium iphone.
              ReadOnlyCollection<IWebElement> redigerknapper = driver.FindElements(By.XPath("//*[@aria-label='Rediger']"));
              Assert.That(redigerknapper.Count, Is.GreaterThan(6));
              driver.FindElement(By.XPath("//button[.='Slå redigering av']")).Click();

              LoggUt(driver, bsCaps);
          } catch(Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "AdobeConnect som lærer")]
        public void TestAdobeConnectLaerer()
        {
          if(GlobalVariables.ErTiming()) {
            Assert.Ignore();
          }
          try
          {
              TestAdobeConnect(facultyEmployeeLaererFnr, facultyEmployeeLaererPW, driver, bsCaps);
          } catch(Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "AdobeConnect som elev")]
        public void TestAdobeConnectElev()
        {
          if(GlobalVariables.ErTiming())
          {
            Assert.Ignore();
          }
          try
          {
              TestAdobeConnect(studentUnder18Fnr, studentUnder18PW, driver, bsCaps);
          } catch(Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Test Poodl MiniLesson diktat")]
        public void TestPoodlMinilessonDiktat()
        {
          if(GlobalVariables.ErTiming() || GlobalVariables.ErProd() || (bsCaps.device != null && bsCaps.device.Contains("Tab S7")))
          {
              Assert.Ignore(); // Skal ikke testes i Prod, og S7 støttes ikke
          }

          try
          {
            GaaTilSkoleDigilaer(driver);
            LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW, driver, bsCaps);

            GaaTilSeleniumFag(driver, bsCaps);
            
            driver.FindElement(By.XPath("//a[.//span[starts-with(., 'Poodll')]]")).Click();
            Thread.Sleep(3000);
            if(driver.FindElements(By.ClassName("btn_finished_attempt")).Count > 0 &&
                    driver.FindElement(By.ClassName("btn_finished_attempt")).Displayed)
            {
              driver.FindElement(By.ClassName("btn_finished_attempt")).Click();
              Thread.Sleep(3000);
              
              driver.FindElement(By.XPath("//button[@data-action='save']")).Click();
              HaandterMacSafari(bsCaps);
            }
            
            Thread.Sleep(2000);
            driver.FindElement(By.ClassName("fa-play")).Click();
            Thread.Sleep(500);
/*             Assert.True(driver.FindElement(By.ClassName("fa-spinner")).Displayed);
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
            } */

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

            LoggUt(driver, bsCaps);
          } catch (Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Test Moodle Quiz med gap fill dra ord")]
        public void TestMoodleQuizMedGapFill()
        {
          if(GlobalVariables.ErTiming() || GlobalVariables.ErProd())
          {
              Assert.Ignore(); // Skal ikke testes i Prod
          }

          try
          {
              GaaTilSkoleDigilaer(driver);
              LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW, driver, bsCaps);

              GaaTilSeleniumFag(driver, bsCaps);
              
              driver.FindElement(By.XPath("//a[.//span[starts-with(., '1 Tall og siffer')]]")).Click();
              HaandterMacSafari(bsCaps);

              if(driver.FindElements(By.XPath("//button[.='Fortsett med forrige forsøk']")).Count > 0)
              {
                  driver.FindElement(By.XPath("//button[.='Fortsett med forrige forsøk']")).Click();
              } else if(driver.FindElements(By.XPath("//button[.='Ta quizen']")).Count > 0) {
                  driver.FindElement(By.XPath("//button[.='Ta quizen']")).Click();
              } else {
                  driver.FindElement(By.XPath("//button[.='Fortsett siste forhåndsvisning']")).Click();
              }

              HaandterMacSafari(bsCaps);
              IWebElement ordEn = driver.FindElement(By.XPath("//span[starts-with(., 'will')]"));

              IWebElement slippFeltEn = driver.FindElements(By.ClassName("droptarget"))[0]; 

              // TODO:
              // Dra ord inn i felter
              // Actions drag: Fungerer ikke med safari-driver
              // Assert Resultat
              // Klikk begynn på nytt
              // Håndter lukking av popup for å avslutte
              // Assert.That(pageSource.Contains("Oppslagstavle"), Is.True);

              LoggUt(driver, bsCaps);
          } catch (Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Test Trinket regnestykke")]
        public void TestTrinket()
        {
          if(GlobalVariables.ErTiming() || GlobalVariables.ErProd() || (bsCaps.device != null && bsCaps.device.Contains("iPhone")))
          {
              Assert.Ignore(); // Skal ikke testes i Prod
          }
          
          try
          {
            GaaTilSkoleDigilaer(driver);
            LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW, driver, bsCaps);
            
            GaaTilSeleniumFag(driver, bsCaps);
            driver.FindElement(By.XPath("//a[.//span[starts-with(., 'Trinket')]]")).Click();
            HaandterMacSafari(bsCaps);

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

            LoggUt(driver, bsCaps);
          } catch (Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }

        [Test]
        [TestCase(TestName = "Test Vimeo avspilling")]
        public void TestVimeoAvspilling()
        {
          if(GlobalVariables.ErTiming() || GlobalVariables.ErProd() || (bsCaps.device != null && bsCaps.device.Contains("iPhone")))
          {
              Assert.Ignore(); // Skal ikke testes i Prod, og støttes ikke på iPhone
          }
          
          try
          {
              GaaTilSkoleDigilaer(driver);
              LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW, driver, bsCaps);

              GaaTilSeleniumFag(driver, bsCaps);
              
              driver.FindElement(By.XPath("//a[.//span[starts-with(., 'Vimeo')]]")).Click();
              HaandterMacSafari(bsCaps);
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
                  
                  try
                  {
                    driver.FindElement(By.XPath("//button[@aria-label='Play']")).Click();
                    Thread.Sleep(300); // La video snurre litt
                    driver.FindElement(By.XPath("//button[@aria-label='Pause']")).Click();
                  } catch(Exception e)
                  {
                    driver.FindElement(By.XPath("//span[.='Play']/..")).Click();
                    Thread.Sleep(300); // La video snurre litt
                    driver.FindElement(By.XPath("//span[.='Pause']/..")).Click();
                  }

                  driver.SwitchTo().ParentFrame();
                  Thread.Sleep(1000);
              }
              LoggUt(driver, bsCaps);
          } catch (Exception exception)
          {
              HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
          }
        }
    }
}
