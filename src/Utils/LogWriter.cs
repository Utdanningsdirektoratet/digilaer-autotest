using System;
using System.IO;
using System.Reflection;

 namespace Utils 
 {
    public class LogWriter
    {
        private static string exePath = string.Empty;
       
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
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