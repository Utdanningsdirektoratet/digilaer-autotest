using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Selenium
{

    public class SeleniumSetup {

        public static IWebDriver GetSeleniumDriver() {
            
            Console.WriteLine("Selenium todo setup");
            IWebDriver driver = new FirefoxDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            return driver;

        }
    }
}