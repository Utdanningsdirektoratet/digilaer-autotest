using System;
using System.Collections.ObjectModel;
using System.Threading;
using api.monitor.db;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using Selenium;
using Slack;
using Utils;

namespace TestSuite
{
    //  [Parallelizable(ParallelScope.Fixtures)] // Gjør ingen forskjell i browserstack virker som.. 
    [TestFixture(DeviceConfig.OSXBigSurEdge)]
    [TestFixture(DeviceConfig.Win10Firefox)]
    [TestFixture(DeviceConfig.OSXBigSurChrome)]
    [TestFixture(DeviceConfig.Win10Edge)]
    [TestFixture(DeviceConfig.OSXBigSurFirefox)]
    [TestFixture(DeviceConfig.OSXBigSurSafari)]
    [TestFixture(DeviceConfig.Ipad11Pro2020)]
    [TestFixture(DeviceConfig.AndroidGalaxyS20)]
    [TestFixture(DeviceConfig.AndroidGalaxyTabS7)] 
    [TestFixture(DeviceConfig.IOSIphoneXS)]
    [TestFixture(DeviceConfig.Win10Chrome)]
    public class LoginTests
    {
        private IWebDriver driver;
        private BrowserStackCapabilities bsCaps;
        private string fagkodeSelenium = "SEL";
        private string facultyEmployeeLaererFnr = "fra properties";
        // private string tittelTBD = "fra properties"; // TODO JB: Dobbeltsjekk rolle: Trengs student over 18 eller en lærer til?
        private string studentUnder18Fnr = "fra properties";
        private string feidePw = "fra properties";
        
        private string resultatTekst = "";

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
                bsCaps = new BrowserStackCapabilities{device = "iPad Pro 11 2020", osVersion = "13", realMobile = "true", local = "false"};
            } else if(deviceConfig == DeviceConfig.IOSIphoneXS)
            {
                bsCaps = new BrowserStackCapabilities{device = "iPhone XS", osVersion = "15", browser = "iPhone", realMobile = "true"};
            } else if(deviceConfig == DeviceConfig.AndroidGalaxyTabS7)
            {
                bsCaps = new BrowserStackCapabilities{device = "Samsung Galaxy Tab S7", osVersion = "10.0", realMobile = "true", local = "false"};
            }
             else if(deviceConfig == DeviceConfig.AndroidGalaxyS20)
            {
                bsCaps = new BrowserStackCapabilities{device = "Samsung Galaxy S20", osVersion = "10.0", realMobile = "true", local = "false"};
            }

            driver = seleniumSetup.GetBrowserstackDriver(bsCaps);
        }

        [OneTimeSetUp]
        public void Init()
        {
            LogWriter.LogWrite("Starter seleniumtest.");
            maksimerVindu();
        }

        [OneTimeTearDown]
        public void AfterTestsOnOneDeviceIsFinished()
        { 
            
            sendSlackResultat();
            // TODO: Send til API  
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
                + "\n Kanskje <@joakimbjerkheim> tar en titt?";
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Test feilet. \"}}");
            } else 
            {
                resultatTekst = oppsettTekst + ":\n"
                + "Alle " + TestContext.CurrentContext.Result.PassCount + " tester kjørt ok!:ok_hand:\n"
                + resultatTekst;
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \" Test OK. \"}}");
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
                if(driver.FindElement(By.Id("language-switcher")).Displayed)
                {
                    driver.FindElement(By.Id("language-switcher")).Click();
                }
                
                driver.FindElement(By.LinkText("Nynorsk")).Click();
                haandterMacSafari();
                Assert.That(driver.PageSource.ToLower().Contains("moglegheiter"), Is.True);

                if(driver.FindElement(By.Id("language-switcher")).Displayed)
                {
                    driver.FindElement(By.Id("language-switcher")).Click();
                }

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
                haandterMacSafari();
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
        [TestCase(TestName = "Last AdobeConnect-side")]
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
                // TODO JB: Det er trolig flere popups...
                string moteUrl = driver.FindElement(By.XPath("//input[@value='Join Meeting']")).GetAttribute("onclick");
                int moteUrlLengde = moteUrl.IndexOf("'", (moteUrl.IndexOf("'")) + 1) - moteUrl.IndexOf("'") - 1;
                moteUrl = moteUrl.Substring(moteUrl.IndexOf("'") + 1, moteUrlLengde);
                driver.Navigate().GoToUrl(moteUrl);
                // TODO: Implementer mer test her om ønskelig

                LoggUt();
            } catch(Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        } 

     [Test]
        [TestCase(TestName = "Last Zoom-side")]
        public void testZoom()
        {            
            try
            {
                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                LoggInnMedFeide(studentUnder18Fnr, feidePw);  
                GaaTilSeleniumFag();

                driver.FindElement(By.XPath("//span[contains(text(), 'SELENIUM test av Zoom')]")).Click();       
                
                Thread.Sleep(5000); // spinner i zoom...
                if(driver.Url.Contains("https://skole.digilaer.no"))
                {
                    IWebElement iFrameZoom = driver.FindElement(By.Id("contentframe"));
                    
                    Assert.IsTrue(iFrameZoom.Displayed);

                    driver.SwitchTo().Frame(iFrameZoom);

                    Thread.Sleep(5000); // spinner i zoom...

                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);
                    // TODO: Test evt noe mer her:
                    // Assert.IsTrue(driver.FindElement(By.Id("@integration-meeting-list")).Displayed);
                    //Assert.True(driver.FindElement(By.XPath("//label[@for='lblmeetingnametitle']")).Displayed, "Felt for møtetittel ikke funnet");
                    // Assert.Greater(driver.FindElements(By.XPath("//a[text(), 'Join']")).Count, 0, "Forventet at det skulle være minst 1 Join-knapp");

                    driver.SwitchTo().ParentFrame();

                } else { // Mobil device har selve zoom lastet i stedet for en iFrame
                    Thread.Sleep(5000); // spinner i Zoom

                    Assert.IsTrue(driver.FindElement(By.XPath("/html/body")).Displayed);
                }

                driver.Navigate().GoToUrl("https://skole.digilaer.no");
                haandterMacSafari();
                LoggUt();
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
                haandterMacSafari();
                ReadOnlyCollection<IWebElement> redigerknapper = driver.FindElements(By.XPath("//a[@aria-label='Rediger']"));
                Assert.That(redigerknapper.Count, Is.GreaterThan(6));

                driver.FindElement(By.XPath("//button[.='Slå redigering av']")).Click();

                LoggUt(); 
            } catch(Exception exception)
            {
                haandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        private void LoggInnMedFeide(string brukernavn, string passord)
        {
            haandterMacSafari(); 
            if(driver.PageSource.ToLower().Contains("innlogget bruker"))
            {
                LogWriter.LogWrite("Var allerede logget inn, forsøker å logge ut...");
                LoggUt();
                LogWriter.LogWrite("Logget ut ok");
            }
            haandterMacSafari();
            driver.FindElement(By.LinkText("Logg inn")).Click();            
           
            haandterMacSafari();

            if(driver.FindElements(By.ClassName("dl-linkbutton")).Count > 0) 
            {
                driver.FindElements(By.ClassName("dl-linkbutton"))[0].Click(); // Flere login-knapper: Klikker den første for å komme videre.. 
            }
         s   
            haandterMacSafari();
            if(driver.FindElements(By.Id("username")).Count == 0 || !driver.FindElement(By.Id("username")).Displayed) 
            {
                IWebElement orgSelector = driver.FindElement(By.Id("org_selector-selectized"));
                orgSelector.SendKeys("Feide"); // For testing mot dataporten bruk "Tjenesteleverandør"
                orgSelector.SendKeys(Keys.Enter);
            }

            haandterMacSafari();
            driver.FindElement(By.Id("username")).SendKeys(brukernavn);
            driver.FindElement(By.Id("password")).SendKeys(passord);

            // driver.FindElement(By.Id("password")).SendKeys(Keys.Enter); //  forsøker å bytte sendkeys ENTER med klikk på button submt...
             driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            haandterMacSafari();
            driver.Navigate().GoToUrl("https://skole.digilaer.no/my/index.php?lang=nb");
            
            haandterMacSafari();
            Assert.That(driver.PageSource.ToLower().Contains("innlogget bruker"), Is.True,  "Brukermeny ble ikke vist, selv om bruker skulle vært innlogget");
        }

        private void LoggUt()
        {
            haandterMacSafari(); 
            try
            {
                driver.FindElement(By.ClassName("avatars")).Click();
            } catch(Exception e) {
                driver.FindElement(By.ClassName("usermenu")).Click();
            }

            haandterMacSafari();
            driver.FindElement(By.XPath("//span[.='Logg ut']")).Click();
            haandterMacSafari(); 
        }

        private void GaaTilSeleniumFag()
        {
            haandterMacSafari();

            IWebElement hamburgerButton = driver.FindElement(By.XPath("//button[@data-action='toggle-drawer']"));

            if(hamburgerButton.GetAttribute("aria-expanded").Equals("false")) // is closed aria-expanded="false"
            { 
                hamburgerButton.Click();
            }
            
            driver.FindElement(By.XPath("//span[.='" + fagkodeSelenium + "']")).Click();
            haandterMacSafari();
        }

        private void maksimerVindu()
        {
            if(bsCaps.os != null && (bsCaps.os.Equals("Windows") || bsCaps.os.Equals("OS X")))
            {
                driver.Manage().Window.Maximize();
            }
        }

        private void haandterMacSafari()
        {
            // Safari webdriver mac os respekterer ikke webdriver wait (!?)
            // https://developer.apple.com/forums/thread/106693 
            // TODO: Vurder behov for denne metoden

            if((bsCaps.browser != null && bsCaps.browser.Equals("Safari") && 
                bsCaps.os != null && bsCaps.os.Equals("OS X")) 
                || (bsCaps.browser != null && bsCaps.browser.Equals("Firefox"))
                || (bsCaps.browser != null && bsCaps.browser.Equals("iPhone"))
                )
            {
                Thread.Sleep(3000);
            }
        }

        private void haandterFeiletTest(Exception e, string testnavn)
        {
                Printscreen.TakeScreenShot(driver, testnavn);
                LogWriter.LogWrite(testnavn + " feilet. Stacktrace:\n" + e.StackTrace);
                LoggUt();
                throw e;
        }
    }
}