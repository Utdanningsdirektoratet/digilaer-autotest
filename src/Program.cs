using System;
using NUnitLite;

namespace digilaer_autotest
{
    public class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Digilær Autotest start");
            Console.WriteLine("0 = " + args[0] + ", 1 = " + args[1]);

            return new AutoRun().Execute(new string[0]);
        }
    }
}