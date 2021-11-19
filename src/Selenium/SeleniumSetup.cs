using System;
using Newtonsoft.Json.Converters;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace Selenium
{
    public class SeleniumSetup {

        private static string BrowserStackUsername = "Hent fra properties";
        private static string BrowserStackKey = "Hent fra properties";
        private static int timeoutSekunder = 10;
        public IWebDriver GetBrowserstackDriver(BrowserStackCapabilities bsCaps)
        {  
             // capability = new OpenQA.Selenium.Chrome.ChromeOptions(); // Gir authentication error mot BrowserStack.
            
            /* if(bsCaps.browser.Contains("Safari")  || bsCaps.browser.Contains("iPhone")) {
                capability = new OpenQA.Selenium.Safari.SafariOptions();
            } else if(bsCaps.browser.Contains("Firefox")) {
                //capability = new OpenQA.Selenium.Firefox.FirefoxOptions();
                capability = new OpenQA.Selenium.Edge.EdgeOptions();
             } else if(bsCaps.browser.Contains("Chrome") || bsCaps.browser.Contains("Android")) {
                capability = new OpenQA.Selenium.Chrome.ChromeOptions();
            } else {
                capability = new OpenQA.Selenium.Edge.EdgeOptions();
            } */
            
            DriverOptions capability = new OpenQA.Selenium.Edge.EdgeOptions();
             
            capability.AddAdditionalCapability("os", bsCaps.os);
            capability.AddAdditionalCapability("os_version", bsCaps.osVersion);
            capability.AddAdditionalCapability("resolution", bsCaps.resolution);
            capability.AddAdditionalCapability("browser", bsCaps.browser);
            capability.AddAdditionalCapability("device", bsCaps.device);
            capability.AddAdditionalCapability("browser_version", bsCaps.browserVersion);
            capability.AddAdditionalCapability("name", "Test med " + GetNameString(bsCaps)); 
            capability.AddAdditionalCapability("build", "BStack_Build" + GetBuildString(bsCaps));
            capability.AddAdditionalCapability("browserstack.local", bsCaps.local);
            capability.AddAdditionalCapability("browserstack.maskCommands", "setValues, getValues, setCookies, getCookies");
            capability.AddAdditionalCapability("browserstack.seleniumLogs", "false");
            capability.AddAdditionalCapability("browserstack.appiumLogs", "false");
            capability.AddAdditionalCapability("browserstack.video", "false");
            capability.AddAdditionalCapability("browserstack.debug", "false"); // FF gir feilmelding: "Invalid moz:firefoxOptions field browserstack.debug
            capability.AddAdditionalCapability("browserstack.networkLogs", "false");

            capability.AddAdditionalCapability("browserstack.user", BrowserStackUsername);
            capability.AddAdditionalCapability("browserstack.key", BrowserStackKey);
            
//          ChromeOptions options = new ChromeOptions(); //Execute Selenium Chrome WebDriver in silent mode
//          options.AddArgument("--log-level=3");

            IWebDriver driver = new RemoteWebDriver(new Uri("http://hub-cloud.browserstack.com/wd/hub"), capability);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); // Implisitt venting 10 sekunder
            
            return driver;
        }

        public static String GetNameString(BrowserStackCapabilities bsCaps) {
            string nameString = "";
            if(bsCaps.device != null)
            {
                nameString += bsCaps.device + " " + bsCaps.osVersion;
            }
            if(bsCaps.os != null)
            {
                nameString += bsCaps.os + " " + bsCaps.osVersion;
            }
            if(bsCaps.browser != null)
            {
                nameString += ", " + bsCaps.browser + " " + bsCaps.browserVersion;
            }
            if(bsCaps.resolution != null)
            {
                nameString += ", " + bsCaps.resolution;
            }
            return nameString;
        }

        private static string GetBuildString(BrowserStackCapabilities bsCaps)
        {
            string buildString = "";
            if(bsCaps.os != null)
            {
                buildString += bsCaps.os + "_" + bsCaps.osVersion;
            }
            if(bsCaps.device != null)
            {
                buildString += bsCaps.device + "_" + bsCaps.osVersion;
            }
            return buildString;
        }

        /* public IWebDriver GetFirefoxDriver() 
        {    
		// For lokal testing evt
            IWebDriver driver = new FirefoxDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeoutSekunder);
            return driver;
        } */

    }

    public class BrowserStackCapabilities
    {
        public string device {get; set;}
        public string realMobile {get; set;}
        public string local {get; set;}
        public string os {get; set;} 
        public string osVersion {get; set;} 
        public string browser {get; set;} 
        public string browserVersion {get; set;} 
        public string resolution {get; set;} 
    }

    public enum DeviceConfig
    {
        Win10Chrome,
        Win10Firefox,
        Win10Edge,
        OSXBigSurSafari,
        OSXBigSurFirefox,
        OSXBigSurChrome,
        OSXBigSurEdge,
        IOSIphoneXS,
        Ipad11Pro2020, 
        AndroidGalaxyS20,
        AndroidGalaxyTabS7
    }
}