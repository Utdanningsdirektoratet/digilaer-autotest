using System;
using Newtonsoft.Json.Converters;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Utils;

namespace Selenium
{
    public class SeleniumSetup
    {
        private static string BrowserStackUsername = System.Environment.GetEnvironmentVariable("DIGI_BS_USER");
        private static string BrowserStackKey = System.Environment.GetEnvironmentVariable("DIGI_BS_KEY");
        private static int timeoutSekunder = 10;
        public IWebDriver GetBrowserstackDriver(BrowserStackCapabilities bsCaps)
        {
            DriverOptions capability = new OpenQA.Selenium.Edge.EdgeOptions();

            capability.AddAdditionalCapability("os", bsCaps.os);
            capability.AddAdditionalCapability("os_version", bsCaps.osVersion);
            capability.AddAdditionalCapability("resolution", bsCaps.resolution);
            capability.AddAdditionalCapability("browser", bsCaps.browser);
            capability.AddAdditionalCapability("device", bsCaps.device);
            capability.AddAdditionalCapability("browser_version", bsCaps.browserVersion);
            capability.AddAdditionalCapability("name", "Test med " + GetNameString(bsCaps));
            capability.AddAdditionalCapability("build", GetBuildString(bsCaps));
            capability.AddAdditionalCapability("browserstack.local", bsCaps.local);

            capability.AddAdditionalCapability("browserstack.video", "true");
            capability.AddAdditionalCapability("browserstack.maskCommands", "setValues, getValues, setCookies, getCookies");
            capability.AddAdditionalCapability("browserstack.appiumLogs", "true");
            capability.AddAdditionalCapability("browserstack.debug", "false"); // FF gir feilmelding: "Invalid moz:firefoxOptions field browserstack.debug
            capability.AddAdditionalCapability("browserstack.console", "errors"); // FF gir feilmelding: "Invalid moz:firefoxOptions field browserstack.debug
            capability.AddAdditionalCapability("browserstack.consoleLogs", "errors"); // FF gir feilmelding: "Invalid moz:firefoxOptions field browserstack.debug
            capability.AddAdditionalCapability("browserstack.seleniumLogs", "false");
            capability.AddAdditionalCapability("browserstack.networkLogs", "false");

            capability.AddAdditionalCapability("browserstack.appium_version", "1.21.0"); // 2.0.0
            capability.AddAdditionalCapability("browserstack.user", BrowserStackUsername);
            capability.AddAdditionalCapability("browserstack.key", BrowserStackKey);

            if(bsCaps.device != null)
            {
                capability.AddAdditionalCapability("browserstack.autoAcceptAlerts", "true");
            }

            IWebDriver driver = new RemoteWebDriver(new Uri("http://hub-cloud.browserstack.com/wd/hub"), capability);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            return driver;
        }

        public static String GetNameString(BrowserStackCapabilities bsCaps)
        {
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
            if(nameString.Equals(""))
            {
                nameString = "Lokal Windows med Firefox";
            }
            return nameString;
        }

        private static string GetBuildString(BrowserStackCapabilities bsCaps)
        {
            string buildString = "BStack_" + GlobalVariables.miljo + "_";
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

        // For lokal testing
        public IWebDriver GetFirefoxDriver()
        {
            IWebDriver driver = new FirefoxDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeoutSekunder);
            return driver;
        }
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
        IOSIphone,
        Ipad11Pro2020,
        AndroidGalaxyS20,
        AndroidGalaxyTabS7,
        AndroidGalaxyS21,
        AndroidOnePlus9,
        GooglePixel6,
        GooglePixel4XL,
        SamsungGalaxyS10
    }
}