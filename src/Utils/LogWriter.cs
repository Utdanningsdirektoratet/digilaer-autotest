using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

 namespace Utils 
 {
    public class LogWriter
    {
        private static string exePath = string.Empty;
       
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }

        public static void LogToBrowserStack(IWebDriver driver, string logMessage)
        {
            if(driver.GetType() != typeof(FirefoxDriver))
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"annotate\", \"arguments\": {\"data\":\"" + logMessage + "\", \"level\": \"info\"}}");
            } else {
                Console.WriteLine(logMessage);
            }
        }

        public static void LogWrite(string logMessage)
        {
            exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            try
            {
                using (StreamWriter writer = File.AppendText(exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, writer);
                }
            }
            catch (Exception e) {}
        }

        private static void Log(string logMessage, TextWriter textWriter)
        {
            try
            {
                textWriter.Write("\r\nLogg: ");
                textWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                textWriter.WriteLine("{0}", logMessage);
                textWriter.WriteLine("------------------------------");
            }
            catch (Exception e) {}
        }
    }
}