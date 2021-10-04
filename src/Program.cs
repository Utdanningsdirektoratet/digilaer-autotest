using System;
using OpenQA.Selenium;

namespace digilaer_autotest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IWebDriver driver = Selenium.SeleniumSetup.GetSeleniumDriver();
            
            driver.Navigate().GoToUrl("https://digilaer.no");
            driver.Close();

        }
    }
}
