using VisStatsBL.Interfaces;
using VisStatsBL.Managers;
using VisStatsDL_File;
using VisStatsDL_SQL;

namespace Program
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string conn = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=PGVisStats;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
            string filePath = @"C:\Users\thiba\Documents\HOGENT\Semester2\ProgGevorderd1\Vis\vissoorten.txt";
            IFileProcessor fp = new FileProcessor();
            IVisStatsRepository repo = new VisStatsRepository(conn);

            VisStatsManager vm = new VisStatsManager(fp, repo);
            vm.UploadVissoorten(filePath);
        }
    }
}
