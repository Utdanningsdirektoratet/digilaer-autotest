using System;
using NUnitLite;
using Utils;

namespace digilaer_autotest
{
    public class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Digilær Autotest start");

            if(args != null && args.Length > 0)
            {
                Console.WriteLine("0 = " + args[0]);
                if(args[0].Equals("manuell"))
                {
                    // Manuell kjøring styres med miljøvariabel
                    if(System.Environment.GetEnvironmentVariable("DIGI_MANUAL_RUN_ENV").Equals("stage"))
                    {
                        GlobalVariables.SetStage();
                    } else if(System.Environment.GetEnvironmentVariable("DIGI_MANUAL_RUN_ENV").Equals("prod"))
                    {
                        GlobalVariables.SetProd();
                    }
                } else if(args[0].Equals("chron"))
                {
                    // Nattlig jobb er alltid mot produksjon
                    GlobalVariables.SetProd();
                }
            }
            return new AutoRun().Execute(new string[0]);
        }
    }
}