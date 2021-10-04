using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace TestSuite
{
    [TestFixture]
    public class LoginTests
    {

        private IWebDriver driver;

        [OneTimeSetUp]
        public void Init() {
            Console.WriteLine("OneTimeSetUp begin");
            driver = Selenium.SeleniumSetup.GetSeleniumDriver();
            Console.WriteLine("OneTimeSetUp finished");
        }

        [OneTimeTearDown]
        public void Cleanup() {
            Console.WriteLine("OneTimeTearDown begin");
            driver.Close();
            Console.WriteLine("OneTimeTearDown finished");
        }

        [Test]
        [TestCase(TestName = "1. Åpne digilær hovedside")]
        public void goToDigiLaerFrontPage() {
            Console.WriteLine("Test åpne digilær forside kjører...");
            
            driver.Navigate().GoToUrl("https://digilaer.no");
            
        }

        [Test]
        [TestCase(TestName = "2. Klikke logg inn")]
        public void goToDigiLaerAboutUs() {
            Console.WriteLine("Test åpne digilær forside kjører...");
            
            driver.Navigate().GoToUrl("https://digilaer.no");
            driver.Navigate().GoToUrl("https://digilaer.no/nb/om-digilaerno");
        }
    }
}