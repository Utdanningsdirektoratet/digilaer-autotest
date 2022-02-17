using System;
using System.Collections.ObjectModel;
using System.Threading;
using monitor.api;
using monitor.api.dto;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using Selenium;
using Slack;
using Utils;

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
        private string UrlDigilaer = "https://digilaer.no";
        private string UrlSkoleDigilaer = "https://skole.digilaer.no";

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
                bsCaps = new BrowserStackCapabilities{os = "OS X", osVersion = "Big Sur", browser = "Firefox", browserVersion = "92.0", resolution = "1920x1080"};
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

            // driver = seleniumSetup.GetFirefoxDriver(); // For lokal testing evt
             driver = seleniumSetup.GetBrowserstackDriver(bsCaps);
        }

        [OneTimeSetUp]
        public void Init()
        {
            LogWriter.LogWrite("Starter seleniumtest på en device.");
            enhetIdForDB =  MonitorApiClient.FindOrCreateEnhetOppsett(new EnhetOppsett{
                enhet = bsCaps.device, nettleserNavn = bsCaps.browser, nettleserVersjon = bsCaps.browserVersion,
                osNavn = bsCaps.os, osVersjon = bsCaps.osVersion, opplosning = bsCaps.resolution
            });
            Console.WriteLine("Id mottatt er :  " + enhetIdForDB);
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
                + resultatTekst
                + "\nKanskje <@joakimbjerkheim> eller <@mathias.meier.nilsen> tar en titt?";
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Test feilet. \"}}");
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
            funkTestIdForDB = MonitorApiClient.FindOrCreateFunksjonellTest(TestContext.CurrentContext.Test.MethodName, TestContext.CurrentContext.Test.Name);

             MonitorApiClient.PostTestkjoring(new Testkjoring{
                enhetOppsettId = enhetIdForDB, funksjonellTestId = funkTestIdForDB, resultatId = (int)TestContext.CurrentContext.Result.Outcome.Status,
                starttid = teststartForDB, sluttid = DateTime.Now,
                debugInformasjon = TestContext.CurrentContext.Result.Message + TestContext.CurrentContext.Result.StackTrace});

            if (TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Passed))
            {
                // Utkommentert for å ikke overfylle slack kanalen etter kjøring. Fokus på evt feilede tester heller.
                // resultatTekst += ":white_check_mark:" + TestContext.CurrentContext.Test.Name + "\n"; 
            }
            else if (TestContext.CurrentContext.Result.Outcome.Equals(TestStatus.Failed) ||
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
                driver.Navigate().GoToUrl(UrlDigilaer);
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
                driver.Navigate().GoToUrl(UrlDigilaer);
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

                driver.Navigate().GoToUrl(UrlDigilaer + "/nb/om-digilaerno");
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
                driver.Navigate().GoToUrl(UrlSkoleDigilaer);

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
            try
            {
                driver.Navigate().GoToUrl(UrlDigilaer);
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
                driver.Navigate().GoToUrl(UrlSkoleDigilaer);
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
                driver.Navigate().GoToUrl(UrlDigilaer);
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
                driver.Navigate().GoToUrl(UrlSkoleDigilaer);
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
                driver.Navigate().GoToUrl(UrlSkoleDigilaer);
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
        [TestCase(TestName = "Last AdobeConnect-side")]
        public void TestAdobeConnect()
        {
            try
            {
                driver.Navigate().GoToUrl(UrlSkoleDigilaer);
                LoggInnMedFeide(studentUnder18Fnr, feidePw);
                GaaTilSeleniumFag();

                string adobeConnectUrl = driver.FindElement(By.XPath("//span[.='SELENIUM test Adobe Connect']/ancestor::a")).GetAttribute("href");
                driver.Navigate().GoToUrl(adobeConnectUrl);

                string moteUrl = driver.FindElement(By.XPath("//input[@value='Join Meeting']")).GetAttribute("onclick");
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

                    if(driver.FindElement(By.Id("download-app-notifier_1")) != null && driver.FindElement(By.Id("download-app-notifier_1")).Displayed)
                    {
                        driver.FindElement(By.Id("download-app-notifier_1")).Click();
                    }

                    if(driver.FindElement(By.XPath("//span[.='Close']")) != null && driver.FindElement(By.XPath("//span[.='Close']")).Displayed)
                    {
                        driver.FindElement(By.XPath("//span[.='Close']")).FindElement(By.XPath("./..")).Click();
                    }

                    if(driver.FindElement(By.XPath("//span[.='Display Media']")) != null && driver.FindElement(By.XPath("//span[.='Display Media']")).Displayed)
                    {
                        driver.FindElement(By.XPath("//span[.='Display Media']")).FindElement(By.XPath("./..")).Click();
                    }

                    Assert.True(driver.FindElement(By.Id("attendeePodContainerDiv")).Displayed, "Div for deltakere ikke funnet (attendeePodContainerDiv)");

                    driver.SwitchTo().ParentFrame();
                } else // Mobiler/tablets krever egen app. Gjør kun en assert:
                {
                    Assert.True(driver.PageSource.Contains("Use the mobile app to join a room"));
                }
                driver.Navigate().GoToUrl(UrlSkoleDigilaer);
                HaandterAlert();
                HaandterMacSafari();

                LoggUt();
            } catch(Exception exception)
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
                driver.Navigate().GoToUrl(UrlSkoleDigilaer);
                LoggInnMedFeide(studentUnder18Fnr, feidePw);
                GaaTilSeleniumFag();

                driver.FindElement(By.XPath("//span[contains(text(), 'SELENIUM test av Zoom')]")).Click();

                Thread.Sleep(5000); // Load spinner i zoom...

                if(bsCaps.device != null && bsCaps.device.Contains("iPad"))
                {
                    IWebElement iFrameZoom = driver.FindElement(By.Id("contentframe"));

                    Assert.IsTrue(iFrameZoom.Displayed);

                    // driver.SwitchTo().Frame(iFrameZoom); // Fungerer ikke på automatisert iPad

                    Thread.Sleep(5000); // spinner i zoom...

                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed); // TODO: Forsøk å teste noe mer rundt zoom... denne treffer trolig kun toppen av html...
                } else if(driver.Url.Contains(UrlSkoleDigilaer))
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
                driver.Navigate().GoToUrl(UrlSkoleDigilaer);
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
                driver.Navigate().GoToUrl(UrlSkoleDigilaer);
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
            driver.Navigate().GoToUrl(UrlSkoleDigilaer + "/my/index.php?lang=nb");
            
            HaandterMacSafari();
            Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker"), Is.True,  "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
        }

        private void LoggUt()
        {
            HaandterMacSafari();
            try
            {
                driver.FindElement(By.ClassName("avatars")).Click();
            } catch(Exception e)
            {
                driver.FindElement(By.ClassName("usermenu")).Click();
            }

            HaandterMacSafari();
            driver.FindElement(By.XPath("//span[.='Logg ut']")).Click();
            HaandterMacSafari();
        }

        private void GaaTilSeleniumFag()
        {
            HaandterMacSafari();

            IWebElement hamburgerButton = driver.FindElement(By.XPath("//button[@data-action='toggle-drawer']"));

            if(hamburgerButton.GetAttribute("aria-expanded").Equals("false"))
            {
                hamburgerButton.Click();
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
            try
            {
                driver.SwitchTo().Alert().Accept();
            } catch(Exception e) 
            {
                // Alert not present
            } 
        }

        private void HaandterFeiletTest(Exception e, string testnavn)
        {
            Printscreen.TakeScreenShot(driver, testnavn);
            LogWriter.LogWrite(testnavn + " feilet. Stacktrace:\n" + e.StackTrace);
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