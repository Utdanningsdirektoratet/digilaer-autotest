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
                if(args[0].Equals("manuellprod"))
                {
                  GlobalVariables.SetProdEnv(false, false);
                } else if(args[0].Equals("manuellstage"))
                {
                  GlobalVariables.SetStageEnv();
                } else if(args[0].Equals("chronprod"))
                {
                    GlobalVariables.SetProdEnv(true, false);
                } else if(args[0].Equals("chronstage"))
                {
                    GlobalVariables.SetStageEnv();
                } else if(args[0].Equals("chrontimingprod"))
                {
                    GlobalVariables.SetProdEnv(true, true);
                }
            }
            return new AutoRun().Execute(new string[0]);
        }
    }
}