using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisStatsBL.Exeptions;
using VisStatsBL.Interfaces;

namespace VisStatsBL.Model
{
    public class Jaarvangst
    {
        public double Totaal { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Gemiddelde { get; set; }
        public string Soortnaam { get; set; }

        public Jaarvangst(double totaal, double min, double max, double gemiddelde, string soortnaam)
        {
            Totaal = totaal;
            Min = min;
            Max = max;
            Gemiddelde = gemiddelde;
            Soortnaam = soortnaam;
        }
    }
}
