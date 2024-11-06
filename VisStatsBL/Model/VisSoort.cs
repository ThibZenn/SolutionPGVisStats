using System.Runtime.ExceptionServices;
using VisStatsBL.Exeptions;

namespace VisStatsBL.Model
{
    public class VisSoort
    {
        public int? ID; // Het vraagteken wil zeggen dat de ID wel nullable is. Hij kan leeg zijn.
        private string naam;
        public string Naam
        {
            get { return naam; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) // Mag niet leeg zijn.
                    throw new DomeinException("Vissoort_naam");
                naam = value;
            }
        }
        // Verplicht een constructor schrijven want anders (met de standaard constructor) kan je een vissoort aanmaken zonder naam. Dit mag niet, een vissoort moet altijd een naam hebben!
        public VisSoort(int iD, string naam)
        {
            ID = iD;
            Naam = naam;
        }

        public VisSoort(string naam)
        {
            Naam = naam;
        }

        public override string ToString()
        {
            return Naam;
        }

    }
}
