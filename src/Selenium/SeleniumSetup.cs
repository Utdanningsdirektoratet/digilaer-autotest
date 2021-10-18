using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace Selenium
{
    public class SeleniumSetup {

        private static string BrowserStackUsername = "Hent fra properties";
        private static string BrowserStackKey = "Hent fra properties";
        private static int timeoutSekunder = 10;

        public static IWebDriver GetFirefoxDriver() 
        {    
            IWebDriver driver = new FirefoxDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeoutSekunder);
            return driver;
        }

        public static IWebDriver GetBrowserstackDriver()
        {  
            OpenQA.Selenium.Firefox.FirefoxOptions capability = new OpenQA.Selenium.Firefox.FirefoxOptions();
            // Hardkodet foreløpig:
            capability.AddAdditionalCapability("os_version", "10", true);
            capability.AddAdditionalCapability("resolution", "1024x768", true);
            capability.AddAdditionalCapability("browser", "Chrome", true);
            capability.AddAdditionalCapability("browser_version", "90.0", true);
            capability.AddAdditionalCapability("os", "Windows", true);
            capability.AddAdditionalCapability("name", "Testkjøring med Chrome v90 på Windows10", true); // test name
            capability.AddAdditionalCapability("build", "BStack Build Number 1", true); // CI/CD job or build name
            capability.AddAdditionalCapability("browserstack.debug", true);
            capability.AddAdditionalCapability("browserstack.user", BrowserStackUsername, true);
            capability.AddAdditionalCapability("browserstack.key", BrowserStackKey, true);
 //           ChromeOptions options = new ChromeOptions(); //Execute Selenium Chrome WebDriver in silent mode
 //           options.AddArgument("--log-level=3");

            IWebDriver driver = new RemoteWebDriver(new Uri("http://hub-cloud.browserstack.com/wd/hub"), capability);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //Implicit wait for 10 seconds

            return driver;
        }
    }
}