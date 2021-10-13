using System.IO;
using System.Reflection;
using OpenQA.Selenium;

namespace Utils
{
    public class Printscreen
    {
        private static string exePath = string.Empty;
        
        public static void TakeScreenShot(IWebDriver driver, string filename) {
            exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(exePath + "//" + filename + ".png", ScreenshotImageFormat.Png);
        }
    }
}