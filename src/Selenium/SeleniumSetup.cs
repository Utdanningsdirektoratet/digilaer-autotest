using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Selenium
{
    public class SeleniumSetup {

        private static int timeoutSekunder = 10;

        public static IWebDriver GetSeleniumDriver() 
        {    
            IWebDriver driver = new FirefoxDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeoutSekunder);
            return driver;
        }
    }
}