using System;
using System.Globalization;
using NUnit.Framework.Constraints;

namespace Utils 
 {
    public static class GlobalVariables
    {
        public static Miljo miljo;
        public static string digilaerUrl;
        public static string digilaerSkoleUrl;

        public static string slackChannel;

        public static Boolean ErProd()
        {
            return miljo == Miljo.Prod;
        }

        public static Boolean ErStage()
        {
            return miljo == Miljo.Stage;
        }

        public static void SetStage()
        {
            GlobalVariables.miljo = Miljo.Stage;
            GlobalVariables.digilaerUrl = "https://digilaer-cms-stage.udir.c.bitbit.net"; 
            GlobalVariables.digilaerSkoleUrl = "https://moodle-stage.udir.c.bitbit.net";
        }

        public static void SetProd()
        {
            GlobalVariables.miljo = Miljo.Prod;
            GlobalVariables.digilaerUrl = "https://digilaer.no";
            GlobalVariables.digilaerSkoleUrl = "https://skole.digilaer.no";
        }
    }

    public enum Miljo
    {
        Stage,
        Prod
    }
}