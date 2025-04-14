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
        private static int TimeoutSekunder = 10;
        private static IWebDriver driver;

        public static IWebDriver GetBrowserstackDriver(BrowserStackCapabilities bsCaps)
        {
            DriverOptions capability = new OpenQA.Selenium.Edge.EdgeOptions();

            capability.AddAdditionalOption("os", bsCaps.os);
            capability.AddAdditionalOption("os_version", bsCaps.osVersion);
            capability.AddAdditionalOption("resolution", bsCaps.resolution);
            capability.AddAdditionalOption("browser", bsCaps.browser);
            capability.AddAdditionalOption("device", bsCaps.device);
            capability.AddAdditionalOption("browser_version", bsCaps.browserVersion);
            capability.AddAdditionalOption("name", "Test med " + GetNameString(bsCaps));
            capability.AddAdditionalOption("build", GetBuildString(bsCaps));
            capability.AddAdditionalOption("browserstack.local", bsCaps.local);
            capability.AddAdditionalOption("browserstack.video", "true");
            capability.AddAdditionalOption("browserstack.maskCommands", "setValues, getValues, setCookies, getCookies");
            capability.AddAdditionalOption("browserstack.appiumLogs", "false");
            capability.AddAdditionalOption("browserstack.debug", "false"); // FF gir feilmelding: "Invalid moz:firefoxOptions field browserstack.debug
            capability.AddAdditionalOption("browserstack.console", "disable"); // FF gir feilmelding: "Invalid moz:firefoxOptions field browserstack.debug
            capability.AddAdditionalOption("browserstack.consoleLogs", "errors"); // FF gir feilmelding: "Invalid moz:firefoxOptions field browserstack.debug
            capability.AddAdditionalOption("browserstack.seleniumLogs", "true");
            capability.AddAdditionalOption("browserstack.networkLogs", "false");
            capability.AddAdditionalOption("browserstack.appium_version", "1.22.0"); // 2.0.0

            if(bsCaps.device != null)
            {
                capability.AddAdditionalOption("browserstack.autoAcceptAlerts", "true");
            }

            IWebDriver driver = new RemoteWebDriver(new Uri($"http://{BrowserStackUsername}:{BrowserStackKey}@hub-cloud.browserstack.com/wd/hub"), capability);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);

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
            string buildString = "BStack_" + GlobalVariables.Miljo + "_";
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
        public static IWebDriver GetFirefoxDriver()
        {
            IWebDriver driver = new FirefoxDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(TimeoutSekunder);
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
        Win11Chrome,
        Win11Edge,
        OSXBigSurSafari,
        OSXMontereySafari,
        OSXVenturaSafari,
        OSXVenturaFirefox,
        OSXVenturaChrome,
        OSXVenturaEdge,
        IOSIphone,
        Ipad11Pro2020,
        Ipad10th,
        AndroidGalaxyS20,
        AndroidGalaxyS23Ultra,
        AndroidGalaxyTabS7,
        AndroidGalaxyS21,
        AndroidOnePlus9,
        GooglePixel7Pro,
        GooglePixel6,
        GooglePixel4XL,
        SamsungGalaxyS10,
    }
}