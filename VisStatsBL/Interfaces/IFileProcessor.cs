using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisStatsBL.Model;

namespace VisStatsBL.Interfaces
{
    public interface IFileProcessor
    {
        List<string> LeesSoorten(string fileName);
        List<string> LeesHavens(string fileName);
        List<VisStatsDataRecord> LeesStatistieken(string fileName, List<VisSoort> soorten, List<Haven> havens);
    }
}
