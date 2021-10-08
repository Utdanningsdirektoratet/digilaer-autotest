using System;
using OpenQA.Selenium;
using NUnitLite;

namespace digilaer_autotest
{
    public class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Digilær Autotest start");

            return new AutoRun().Execute(args);
        }
    }
}