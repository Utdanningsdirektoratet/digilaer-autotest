using System;

namespace monitor.api.dto
{
    public class Testkjoring
    {
        public int enhetOppsettId {get; set;}
        public int funksjonellTestId {get; set;}
        public int resultatId {get; set;}
        public DateTime starttid {get; set;}
        public DateTime sluttid {get; set;}
        public int tidbrukt {get; set;}
        public string debugInformasjon {get; set;}
    }

    public class EnhetOppsett
    {
        public string enhet {get; set;}
        public string nettleserNavn {get; set;}
        public string nettleserVersjon {get; set;}
        public string osNavn {get; set;}
        public string osVersjon {get; set;}
        public string opplosning {get; set;}
    }
}