using System;
using NUnit.Framework;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Selenium;
using System.Collections.ObjectModel;

namespace Utils 
{
  public static class Navigation
  {

    private static string sprakUrl = "lang=nb";
    private static string fagkodeSelenium = "Selenium";

    public static void MaksimerVindu(IWebDriver driver, BrowserStackCapabilities bsCaps)
    {
        if(bsCaps.os != null && (bsCaps.os.Equals("Windows") || bsCaps.os.Equals("OS X")))
        {
            driver.Manage().Window.Maximize();
        }
    }

    public static void LoggUt(IWebDriver driver, BrowserStackCapabilities bsCaps)
    {
        HaandterMacSafari(bsCaps);
        
        AapneBrukerMeny(driver);

        Thread.Sleep(1000);
        driver.FindElement(By.LinkText("Logg ut")).Click();
        HaandterMacSafari(bsCaps);
    }

    public static void AapneBrukerMeny(IWebDriver driver)
    {
        Thread.Sleep(1000);
        driver.FindElement(By.Id("user-menu-toggle")).Click();
        Thread.Sleep(1000);
    }

    public static void HaandterMacSafari(BrowserStackCapabilities bsCaps)
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

    public static void HaandterFeiletTest(Exception e, string testnavn, IWebDriver driver, BrowserStackCapabilities bsCaps)
    {
        LogWriter.LogWrite(testnavn + " feilet. Stacktrace:\n" + e.StackTrace);
        LogWriter.LogToBrowserStack(driver, testnavn + " feilet");
        Printscreen.TakeScreenShot(driver, testnavn);
        
        HaandterAlert(driver);
        GaaTilSkoleDigilaer(driver);
        try {
            LoggUt(driver, bsCaps);
        } catch(Exception e2)
        {
            LogWriter.LogWrite("Feilet ved utlogging i håndtering av en feilet test. Var kanskje ikke innlogget?");
        }
        throw e;
    }

    public static void HaandterAlert(IWebDriver driver)
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

    public static void GaaTilSkoleDigilaer(IWebDriver driver)
    {
        driver.Navigate().GoToUrl(GlobalVariables.DigilaerSkoleUrl);
        ReadOnlyCollection<IWebElement> agreeKnappeListe = driver.FindElements(By.ClassName("eupopup-button"));
        if(agreeKnappeListe.Count > 0)
        {
            agreeKnappeListe[0].Click();
        }
    }

    public static void LoggInnMedFeide(string brukernavn, string passord, IWebDriver driver, BrowserStackCapabilities bsCaps)
    {
        HaandterMacSafari(bsCaps);
        if(driver.PageSource.ToLower().Contains("innlogget bruker"))
        {
            LogWriter.LogWrite("Var allerede logget inn, forsøker å logge ut...");
            LoggUt(driver, bsCaps);
            LogWriter.LogWrite("Logget ut ok");
        }
        HaandterMacSafari(bsCaps);

        IWebElement feideLogin = driver.FindElement(By.ClassName("feide-login"));
        feideLogin.FindElement(By.TagName("a")).Click();

        HaandterMacSafari(bsCaps);

        ReadOnlyCollection<IWebElement> loginButtons = driver.FindElements(By.ClassName("dl-linkbutton"));
        if(loginButtons.Count > 0)
        {
            loginButtons[0].Click();
        }

        HaandterMacSafari(bsCaps);
        Thread.Sleep(2000);

        if(driver.FindElements(By.Id("username")).Count == 0 || !driver.FindElement(By.Id("username")).Displayed)
        {
            IWebElement orgSelector = driver.FindElement(By.Id("org_selector_filter"));
            orgSelector.SendKeys("Utdanningsdirektoratet - systemorganisasjon"); // For testing mot dataporten: Bruk "Tjenesteleverandør"
            driver.FindElement(By.XPath("//span[.='Utdanningsdirektoratet - systemorganisasjon']")).Click();
            driver.FindElement(By.Id("selectorg_button")).Click();
        }

        HaandterMacSafari(bsCaps);
        driver.FindElement(By.Id("username")).SendKeys(brukernavn);
        driver.FindElement(By.Id("password")).SendKeys(passord);

        Thread.Sleep(3000);
        driver.FindElement(By.XPath("//button[@type='submit']")).Click(); // Sleep før og etter for android-devices
        Thread.Sleep(3000);
        HaandterMacSafari(bsCaps);
        if(bsCaps.browser.Equals("Safari") && bsCaps.os != null && bsCaps.os.Equals("OS X")) {
            Thread.Sleep(10000);
        }

        HaandterSamtykke(driver);
        HaandterVeiledning(driver);

        HaandterMacSafari(bsCaps);
    }

    private static void HaandterVeiledning(IWebDriver driver)
    {
      Thread.Sleep(3000);
      if(driver.FindElements(By.XPath("//button[@role='button' and contains(., 'Hopp over')]")).Count > 0)
        {
            driver.FindElement(By.XPath("//button[@role='button' and contains(., 'Hopp over')]")).Click();
        }
    }

    private static void HaandterSamtykke(IWebDriver driver)
    {
      // Kommentar til git: Tester å legge til sleep for å se om samtykke da håndteres skikkelig... Hør med Rune om han kan resette samtykke for å teste dette...
        Thread.Sleep(2000);
        if(driver.FindElements(By.XPath("//button[@role='button' and contains(., 'amtykke')]")).Count > 0)
        {
            driver.FindElement(By.XPath("//button[@role='button' and contains(., 'amtykke')]")).Click();
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            Thread.Sleep(2000);
            GaaTilDigilaer(driver);
        }
        Thread.Sleep(3000);
        if(driver.FindElements(By.XPath("//button[text='Avslutt veileder']")).Count > 0
            && driver.FindElements(By.XPath("//button[text='Avslutt veileder']"))[0].Displayed)
        {
            driver.FindElements(By.XPath("//button[text='Avslutt veileder']"))[0].Click();
        }
    }

    public static void GaaTilDigilaer(IWebDriver driver)
    {
      if(GlobalVariables.ErProd())
      {
          driver.Navigate().GoToUrl(GlobalVariables.DigilaerSkoleUrl + "/my/index.php?" + sprakUrl);
      } else {
          driver.Navigate().GoToUrl(GlobalVariables.DigilaerUrl);
      }

      Thread.Sleep(2000);
      HaandterAlert(driver);
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
    
    public static void GaaTilSeleniumFag(IWebDriver driver, BrowserStackCapabilities bsCaps)
    {
        HaandterMacSafari(bsCaps);
        AapneBrukerMeny(driver);
        driver.FindElement(By.LinkText("Profil")).Click();
        Thread.Sleep(2000);
        driver.FindElement(By.LinkText(fagkodeSelenium)).Click();
        Thread.Sleep(2000);
        driver.FindElement(By.LinkText("Kurs")).Click();
        HaandterMacSafari(bsCaps);
    }
  
    public static void TestAdobeConnect(string fnr, string pw, IWebDriver driver, BrowserStackCapabilities bsCaps)
    {
        GaaTilSkoleDigilaer(driver);
        LoggInnMedFeide(fnr, pw, driver, bsCaps);
        GaaTilSeleniumFag(driver, bsCaps);
        
        driver.FindElement(By.XPath("//a[.//span[starts-with(., 'SELENIUM test Adobe Connect')]]")).Click();
        int retries = 0; // For adobeconnect-hikke
        string moteUrl = null;

        HaandterMacSafari(bsCaps);
        while(moteUrl == null && retries < 5)
        {
            try
            {
                Thread.Sleep(15000);
                moteUrl = driver.FindElement(By.XPath("//input[@value='Join Meeting']")).GetAttribute("onclick");
            } catch(Exception)
            {
                retries++;
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

            HandleAdobeConnectPopups(driver, bsCaps);
            
            HaandterMacSafari(bsCaps);
            try
            {
                if(driver.FindElements(By.XPath("//span[.='Display Media']")).Count > 0 && driver.FindElement(By.XPath("//span[.='Display Media']")).Displayed)
                {
                    driver.FindElement(By.XPath("//span[.='Display Media']")).FindElement(By.XPath("./..")).Click();
                }
            } catch(WebDriverException e)
            {
                LogWriter.LogWrite("Display media " + e);
                if(!erMacSafari(bsCaps)) {throw e;}
            }

            HaandterMacSafari(bsCaps);
            try
            {
                if(driver.FindElements(By.Id("productMaintenanceNotifier_1")).Count > 0 && driver.FindElement(By.Id("productMaintenanceNotifier_1")).Displayed)
                {
                    driver.FindElement(By.Id("productMaintenanceNotifier_1")).Click();
                }
            } catch(WebDriverException e)
            {
                LogWriter.LogWrite("productMaintenanceNotifier_1 timeout " + e);
                if(!erMacSafari(bsCaps)) {throw e;}
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
        GaaTilDigilaer(driver);
        HaandterAlert(driver);
        HaandterMacSafari(bsCaps);

        LoggUt(driver, bsCaps);
    }

  	public static void HandleAdobeConnectPopups(IWebDriver driver, BrowserStackCapabilities bsCaps) {
      Thread.Sleep(10000); // Wait for popups
      try
      {
          if(driver.FindElements(By.Id("download-app-notifier_1")).Count > 0 && driver.FindElement(By.Id("download-app-notifier_1")).Displayed)
          {
              driver.FindElement(By.Id("download-app-notifier_1")).Click();
          }
      } catch(WebDriverException e)
      {
          LogWriter.LogWrite("download-app-notifier_1 timeout " + e);
          if(!erMacSafari(bsCaps)) {throw e;}
      }

      for(int i = 0; i < 3; i++) {
        if(driver.FindElements(By.XPath("//span[.='Close']")).Count > 0 && driver.FindElement(By.XPath("//span[.='Close']")).Displayed)
        {
            driver.FindElement(By.XPath("//span[.='Close']")).FindElement(By.XPath("./..")).Click();
        }
      }
    }

    public static bool erMacSafari(BrowserStackCapabilities bsCaps)
    {
        return bsCaps.browser != null && bsCaps.browser.Equals("Safari") && bsCaps.os != null && bsCaps.os.Equals("OS X");
    }
  }
}