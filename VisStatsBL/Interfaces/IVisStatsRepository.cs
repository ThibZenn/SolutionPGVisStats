using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisStatsBL.Model;

namespace VisStatsBL.Interfaces
{
    public interface IVisStatsRepository
    {
        bool HeeftVissoort(VisSoort vis);
        void SchrijfSoort(VisSoort vis);
        void SchrijfHaven(Haven haven);
        bool HeeftHaven(Haven haven);
        bool IsOpgeladen(string fileName);
        List<Haven> LeesHavens();
        List<VisSoort> LeesVissoorten();
        void SchrijfStatistieken(List<VisStatsDataRecord> data, string fileName);
        List<int> LeesJaartallen();
        List<Jaarvangst> LeesStatistieken(int jaar, Haven haven, List<VisSoort> vissoorten, Eenheid eenheid);
    }
}
