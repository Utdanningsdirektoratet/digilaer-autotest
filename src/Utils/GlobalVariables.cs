using System;
using System.Globalization;
using System.Net.Http.Headers;
using NUnit.Framework.Constraints;

namespace Utils 
 {
    public static class GlobalVariables
    {
        public static Miljo miljo;
        public static string digilaerUrl;
        public static string digilaerSkoleUrl;
        public static bool loggTilDatabase;

        public static string slackChannel;

        public static bool ErProd()
        {
            return miljo == Miljo.Prod;
        }

        public static bool ErStage()
        {
            return miljo == Miljo.Stage;
        }
        
        public static bool ErTest()
        {
            return miljo == Miljo.Test;
        }

        public static void SetStageEnv()
        {
            GlobalVariables.miljo = Miljo.Stage;
            GlobalVariables.digilaerUrl = "https://digilaer-cms-stage.udir.c.bitbit.net"; 
            GlobalVariables.digilaerSkoleUrl = "https://moodle-stage.udir.c.bitbit.net";
            GlobalVariables.loggTilDatabase = false;
        }

        public static void SetProdEnv()
        {
            GlobalVariables.miljo = Miljo.Prod;
            GlobalVariables.digilaerUrl = "https://digilaer.no";
            GlobalVariables.digilaerSkoleUrl = "https://skole.digilaer.no";
            GlobalVariables.loggTilDatabase = System.Environment.GetEnvironmentVariable("DIGI_LOGG_TIL_DB").Equals("true");
        }

        public static void SetTestEnv()
        {
            GlobalVariables.miljo = Miljo.Test;
            GlobalVariables.digilaerUrl = "https://moodle-test.udir.c.bitbit.net";
            GlobalVariables.loggTilDatabase =   false;
        }
    }

    public enum Miljo
    {
        Test,
        Stage,
        Prod
    }
}