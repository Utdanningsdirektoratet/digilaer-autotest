using System;
using System.Globalization;
using System.Net.Http.Headers;
using NUnit.Framework.Constraints;

namespace Utils 
 {
    public static class GlobalVariables
    {
        public static Miljo Miljo {get; private set;}
        public static string DigilaerUrl {get; private set;}
        public static string DigilaerSkoleUrl {get; private set;}
        private static bool LoggTilDatabase;
        private static bool Scheduled = false;
        private static bool Timing = false;
        private static string SlackChannel {get;}

        public static bool ErProd()
        {
            return Miljo == Miljo.Prod;
        }

        public static bool ErStage()
        {
            return Miljo == Miljo.Stage;
        }
        
        public static bool ErTest()
        {
            return Miljo == Miljo.Test;
        }

        public static bool ErScheduled()
        {
            return Scheduled;
        }

        public static bool ErTiming()
        {
            return Timing;
        }

        public static bool SkalLoggeTilDatabase()
        {
            return LoggTilDatabase;
        }

        public static void SetStageEnv()
        {
            Miljo = Miljo.Stage;
            DigilaerUrl = "https://moodle-stage.udir.c.bitbit.net/my"; 
            DigilaerSkoleUrl = "https://moodle-stage.udir.c.bitbit.net/my"; 
            LoggTilDatabase = false;
        }

        public static void SetProdEnv(bool erScheduled, bool erTiming)
        {
            Miljo = Miljo.Prod;
            DigilaerUrl = "https://digilaer.no";
            DigilaerSkoleUrl = "https://skole.digilaer.no";
            LoggTilDatabase = System.Environment.GetEnvironmentVariable("DIGI_LOGG_TIL_DB").Equals("true");
            Scheduled = erScheduled;
            Timing = erTiming;
        }

        public static void SetTestEnv()
        {
            Miljo = Miljo.Test;
            DigilaerUrl = "https://moodle-test.udir.c.bitbit.net/my";
            DigilaerSkoleUrl = "https://moodle-test.udir.c.bitbit.net/my";
            LoggTilDatabase =   false;
        }
    }

    public enum Miljo
    {
        Test,
        Stage,
        Prod
    }
}