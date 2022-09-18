using System;
using NUnitLite;
using Utils;

namespace digilaer_autotest
{
    public class Program
    {
        static int Main(string[] args)
        {
            if(args != null && args.Length > 0)
            {
                if(args[0].Equals("manuell"))
                {
                    // Manuell kjøring styres med miljøvariabel
                    if(System.Environment.GetEnvironmentVariable("DIGI_MANUAL_RUN_ENV").Equals("stage"))
                    {
                        GlobalVariables.SetStageEnv();
                    } else if(System.Environment.GetEnvironmentVariable("DIGI_MANUAL_RUN_ENV").Equals("prod"))
                    {
                        GlobalVariables.SetProdEnv();
                    } else if(System.Environment.GetEnvironmentVariable("DIGI_MANUAL_RUN_ENV").Equals("test"))
                    {
                        GlobalVariables.SetTestEnv();
                    }
                } else if(args[0].Equals("chron"))
                {
                    // Nattlig jobb er alltid mot produksjon
                    GlobalVariables.SetProdEnv();
                }
            }
            return new AutoRun().Execute(new string[0]);
        }
    }
}