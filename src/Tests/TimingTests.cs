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
using System.Diagnostics; 
using static Utils.Navigation;

[TestFixture(DeviceConfig.Win11Edge)]
public class TimingTests {

  private IWebDriver driver;
  private BrowserStackCapabilities bsCaps;
  private string sprakUrl = "lang=nb";
  private string studentUnder18Fnr = System.Environment.GetEnvironmentVariable("DIGI_ELEV_UNDER_ATTEN");
  private string studentUnder18PW = System.Environment.GetEnvironmentVariable("DIGI_ELEV_UNDER_ATTEN_PW");
  private string resultatTekst = "";
  private int enhetIdForDB;
  private int funkTestIdForDB;
  private DateTime teststartForDB;
  private Stopwatch stopwatch;
  private int timeElapsed;
  private bool success = false;

  public TimingTests(DeviceConfig deviceConfig)
  {
    bsCaps = new BrowserStackCapabilities{os = "Windows", osVersion = "10", browser = "Edge", browserVersion = "latest"};
    // driver = SeleniumSetup.GetFirefoxDriver();  // For lokal testing
    driver = SeleniumSetup.GetBrowserstackDriver(bsCaps);
    stopwatch = new Stopwatch();
  }

  [Test]
  [TestCase(TestName = "Tidtaking hovedside digilær")]
  public void TimingDigilaerFrontPage()
  {
    if(!GlobalVariables.ErTiming()) {
      Assert.Ignore();
    }
    try
    {
      Navigation.MaksimerVindu(driver, bsCaps);

      stopwatch.Start();
      driver.Navigate().GoToUrl(GlobalVariables.DigilaerSkoleUrl + "/my/index.php?" + sprakUrl);
      
      String kildekode = driver.PageSource.ToLower();
      Assert.That(kildekode.Contains("feide"), Is.True);
      Assert.That(kildekode.Contains("digilær"), Is.True);
      stopwatch.Stop();
      TimeSpan ts = stopwatch.Elapsed;
      string elapsedTimeString = LogWriter.FormatTime(ts);
      Console.WriteLine("Tid gått laste hovedside: " + elapsedTimeString);
      timeElapsed = (int) Math.Round(ts.TotalMilliseconds);
      Console.WriteLine("Tid gått hovedside millisekunder  " + timeElapsed);
    } catch(Exception exception)
    {
      timeElapsed = 0;
      HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
    }
  }

  [Test]
  [TestCase(TestName = "Tidtaking logg inn i Digilær fra siste klikk")]
  public void TimingLoggInnFraSisteLoginKlikk()
  {
    if(!GlobalVariables.ErTiming()) {
      Assert.Ignore();
    }
    try
    {
      driver.Navigate().GoToUrl(GlobalVariables.DigilaerSkoleUrl);

      Thread.Sleep(3000);
      IWebElement feideLogin = driver.FindElement(By.ClassName("feide-login"));
      feideLogin.FindElement(By.TagName("a")).Click();

      Thread.Sleep(3000);

      ReadOnlyCollection<IWebElement> loginButtons = driver.FindElements(By.ClassName("dl-linkbutton"));
      if(loginButtons.Count > 0)
      {
          loginButtons[0].Click();
      }

      Thread.Sleep(3000);

      if(driver.FindElements(By.Id("username")).Count == 0 || !driver.FindElement(By.Id("username")).Displayed)
      {
          IWebElement orgSelector = driver.FindElement(By.Id("org_selector_filter"));
          orgSelector.SendKeys("Utdanningsdirektoratet - systemorganisasjon"); // For testing mot dataporten: Bruk "Tjenesteleverandør"
          driver.FindElement(By.XPath("//span[.='Utdanningsdirektoratet - systemorganisasjon']")).Click();
          driver.FindElement(By.Id("selectorg_button")).Click();
      }

      Thread.Sleep(3000);
      driver.FindElement(By.Id("username")).SendKeys(studentUnder18Fnr); 
      driver.FindElement(By.Id("password")).SendKeys(studentUnder18PW);
      Thread.Sleep(3000);
      
      stopwatch.Start();

      driver.FindElement(By.XPath("//button[@type='submit']")).Click(); // Sleep før og etter for android-devices */
      Assert.That(driver.PageSource.Contains("Mine kurs"));
      Assert.That(driver.PageSource.Contains("Test din maskin"));

      stopwatch.Stop();
      TimeSpan ts = stopwatch.Elapsed;
      string elapsedTimeString = LogWriter.FormatTime(stopwatch.Elapsed);
      Console.WriteLine("Tid gått siste logg inn klikk: " + elapsedTimeString);
      timeElapsed = (int) Math.Round(ts.TotalMilliseconds);
      Console.WriteLine("Tid gått siste logg inn klikk millis " + timeElapsed);

      LoggUt(driver, bsCaps);
    } catch(Exception exception)
    {
      timeElapsed = 0;
      HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
    }
  }

  [Test]
  [TestCase(TestName = "Tidtaking laste kurs og Trinket")]
  public void TimingLasteKursOgTrinket()
  {
    if(!GlobalVariables.ErTiming()) {
      Assert.Ignore();
    }
    try
    {
      driver.Navigate().GoToUrl(GlobalVariables.DigilaerSkoleUrl);
      Thread.Sleep(3000);

      LoggInnMedFeide(studentUnder18Fnr, studentUnder18PW, driver, bsCaps);
      
      Assert.That(driver.PageSource.Contains("Mine kurs"));
      Assert.That(driver.PageSource.Contains("Test din maskin"));

      stopwatch.Start();
      driver.FindElement(By.LinkText("Selenium")).Click();
      Thread.Sleep(2000);
      driver.FindElement(By.XPath("//a[.//span[starts-with(., 'Trinket')]]")).Click();
      
      Assert.That(driver.PageSource.Contains("i konsollen nedenfor"));

      stopwatch.Stop();
      TimeSpan ts = stopwatch.Elapsed;
      string elapsedTimeString = LogWriter.FormatTime(stopwatch.Elapsed);
      Console.WriteLine("Tid medgått gå til Seleniumkurs og inn på Trinket regneoppgave: " + elapsedTimeString);
      timeElapsed = (int) Math.Round(ts.TotalMilliseconds);
      Console.WriteLine("Tid medgått gå til Seleniumkurs og inn på Trinket regneoppgave millisekunder:  " + timeElapsed);

      LoggUt(driver, bsCaps);

    } catch(Exception exception)
    {
      timeElapsed = 0;
      HaandterFeiletTest(exception, System.Reflection.MethodBase.GetCurrentMethod().Name, driver, bsCaps);
    }
  }

  [OneTimeSetUp]
  public void Init()
  {
    if(GlobalVariables.ErTiming())
    {
      LogWriter.LogWrite("Starter TimingTest seleniumtest på en device i " + GlobalVariables.Miljo);
        enhetIdForDB =  MonitorApiClient.FindOrCreateEnhetOppsett(new EnhetOppsett{
            enhet = bsCaps.device, nettleserNavn = bsCaps.browser, nettleserVersjon = bsCaps.browserVersion,
            osNavn = bsCaps.os, osVersjon = bsCaps.osVersion, opplosning = bsCaps.resolution
        });
    }
    Navigation.MaksimerVindu(driver, bsCaps);
  }

  [OneTimeTearDown]
  public void AfterTestsOnOneDeviceIsFinished()
  {
    // sendSlackResultat(); // Vurder om det skal sendes hver time..
    driver.Quit();
    LogWriter.LogWrite("Test ferdig.");
  }

  [SetUp]
  public void BeforeEachTest()
  {
      LogWriter.LogToBrowserStack(driver, "Starter timingtest " + TestContext.CurrentContext.Test.MethodName);
      teststartForDB = DateTime.Now;
  }

  [TearDown]
  public void AfterEachTest()
  {
    stopwatch.Reset();
    LogWriter.LogToBrowserStack(driver, "TimingTest " + TestContext.CurrentContext.Test.MethodName + " ferdig.");
    if(GlobalVariables.ErProd()  && GlobalVariables.ErTiming() && GlobalVariables.SkalLoggeTilDatabase())
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
                starttid = teststartForDB, sluttid = DateTime.Now, tidbrukt = timeElapsed,
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