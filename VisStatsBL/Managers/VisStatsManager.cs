using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisStatsBL.Exeptions;
using VisStatsBL.Interfaces;
using VisStatsBL.Model;

namespace VisStatsBL.Managers
{
    public class VisStatsManager //public want de UI laag moet dit kunnen oproepen.
    {
        private IFileProcessor fileProcessor;
        private IVisStatsRepository visStatsRepository;

        public VisStatsManager(IFileProcessor fileProcessor, IVisStatsRepository visStatsRepository)
        {
            this.fileProcessor = fileProcessor;
            this.visStatsRepository = visStatsRepository;
        }

        public void UploadVissoorten(string fileName)
        {
            try
            {
                List<string> soorten = fileProcessor.LeesSoorten(fileName);
                List<VisSoort> vissoorten = MaakVissoorten(soorten);
                foreach (VisSoort vis in vissoorten)
                {
                    if (!visStatsRepository.HeeftVissoort(vis)) //! om te kijken of het niet dubbel is of naamgeving veranderen
                    {
                        visStatsRepository.SchrijfSoort(vis);
                    }
                }
            }
            catch (Exception ex) { }

        }

        public void UploadHaven(string fileName)
        {
            try
            {
                List<string> haven = fileProcessor.LeesHavens(fileName);
                List<Haven> havens = MaakHavens(haven);
                foreach (Haven item in havens)
                {
                    if (!visStatsRepository.HeeftHaven(item)) //! om te kijken of het niet dubbel is of naamgeving veranderen
                    {
                        visStatsRepository.SchrijfHaven(item);
                    }
                }
            }
            catch (Exception ex) { }

        }

        private List<VisSoort> MaakVissoorten(List<string> soorten)
        {
            Dictionary<string, VisSoort> visSoorten = new(); // string is de naam van de vissoort en de value van de dictionary is de lijst met vissoorten.
            // Een dictionary is sneller dan een lijst. Hier zit er nog niet veel verschil maar als je later grotere lijsten gaat gebruiken maakt dit wel uit.
            foreach (string soort in soorten)
            {
                if (!visSoorten.ContainsKey(soort))
                {
                    try
                    {
                        visSoorten.Add(soort, new VisSoort(soort));
                    }
                    catch (DomeinException) { } // Als we feedback willen geven zetten we dat tussen de haakjes.
                }
            }
            return visSoorten.Values.ToList();
        }

        private List<Haven> MaakHavens(List<string> havens)
        {
            Dictionary<string, Haven> havenDictionary = new(); // string is de naam van de vissoort en de value van de dictionary is de lijst met vissoorten.
            // Een dictionary is sneller dan een lijst. Hier zit er nog niet veel verschil maar als je later grotere lijsten gaat gebruiken maakt dit wel uit.
            foreach (string haven in havens)
            {
                if (!havenDictionary.ContainsKey(haven))
                {
                    try
                    {
                        havenDictionary.Add(haven, new Haven(haven));
                    }
                    catch (DomeinException) { } // Als we feedback willen geven zetten we dat tussen de haakjes.
                }
            }
            return havenDictionary.Values.ToList();
        }

        public void UploadStatistieken(string fileName)
        {
            try
            {
                if (!visStatsRepository.IsOpgeladen(fileName))
                {
                    List<VisSoort> soorten = visStatsRepository.LeesVissoorten();
                    List<Haven> havens = visStatsRepository.LeesHavens();
                    List<VisStatsDataRecord> data = fileProcessor.LeesStatistieken(fileName, soorten, havens);
                    visStatsRepository.SchrijfStatistieken(data,fileName);
                }
            }
            catch (Exception ex)
            {
                throw new ManagerExceptions("UploadStatistieken", ex);
            }
        }

        public List<Haven> GeefHavens()
        {
            try
            {
                return visStatsRepository.LeesHavens();
            }
            catch (Exception ex)
            {

                throw new ManagerExceptions("GeefHavens", ex);
            }
        }
        public List<int> GeefJaartallen()
        {
            try
            {
                return visStatsRepository.LeesJaartallen();
            }
            catch (Exception ex)
            {

                throw new ManagerExceptions("GeefJaartallen", ex);
            }
        }

        public List<VisSoort> GeefVissoorten()
        {
            try
            {
                return visStatsRepository.LeesVissoorten();
            }
            catch (Exception ex)
            {
                throw new Exception("GeefVissoorten", ex);
            }
        }

        public List<Jaarvangst> GeefVangst(int jaar, Haven haven, List<VisSoort> vissoorten, Eenheid eenheid)
        {
            try
            {
                return visStatsRepository.LeesStatistieken(jaar, haven, vissoorten, eenheid);
            }
            catch (Exception ex)
            {
                throw new ManagerExceptions("GeefVissoorten", ex);
            }
        }
    }
}
