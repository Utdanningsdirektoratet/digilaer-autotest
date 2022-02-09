using System;
using NUnitLite;

namespace digilaer_autotest
{
    public class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Digilær Autotest start");
            Console.WriteLine("args[0] = " + args[0]);

            return new AutoRun().Execute(args);
        }
    }
}