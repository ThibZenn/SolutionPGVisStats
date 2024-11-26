using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using VisStatsBL.Interfaces;
using VisStatsBL.Model;

namespace VisStatsDL_SQL
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class VisStatsRepository : IVisStatsRepository
    {
        private readonly string _connectionString;

        public VisStatsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool HeeftVissoort(VisSoort vis)
        {
            const string query = "SELECT COUNT(*) FROM Soort WHERE naam=@naam";
            return CheckRecordExists(query, "@naam", vis.Naam);
        }

        public void SchrijfSoort(VisSoort vis)
        {
            const string query = "INSERT INTO Soort(naam) VALUES(@naam)";
            ExecuteNonQuery(query, "@naam", vis.Naam);
        }
        
        public void SchrijfHaven(Haven haven)
        {
            string SQL = "INSERT INTO Haven(naam) VALUES(@naam)";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.Add(new SqlParameter("@naam", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters["@naam"].Value = haven.Naam;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SchrijfHaven", ex);
                }
            }
        }

        public bool HeeftHaven(Haven haven)
        {
            const string query = "SELECT COUNT(*) FROM Haven WHERE naam=@naam";
            return CheckRecordExists(query, "@naam", haven.Naam);
        }

        public bool IsOpgeladen(string fileName)
        {
            string SQL = "SELECT Count(*) FROM upload WHERE filename=@filename"; // @ is om dat we met een parameter werken.
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.Add(new SqlParameter("@filename", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters["@filename"].Value = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                    // cmd.Parameters.AddWithValue("@filename", filename.Substring(filename.LastIndexOf("\\") + 1)); kortere schrijfwijze voor vorige 2 lijnen. 
                    int n = (int)cmd.ExecuteScalar();
                    if (n > 0) return true; else return false;
                } catch (Exception ex)
                {
                    throw new Exception("IsOpgeladen", ex);
                }

            }
        }

        public List<Haven> LeesHavens() 
        {
            string SQL = "SELECT * FROM Haven";
            List<Haven > havens = new List<Haven>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    IDataReader reader = cmd.ExecuteReader(); //Om aan te duiden dat hij moet lezen en niet wegschrijven.
                    while (reader.Read()) // in een while loop want we willen ze allemaal lezen.
                    {
                        havens.Add(new Haven((int)reader["id"], (string)reader["naam"]));
                    }
                    return havens;
                }
                catch (Exception ex)
                {

                    throw new Exception("LeesHavens", ex );
                }
            }
        }

        public List<VisSoort> LeesVissoorten()
        {
            string SQL = "SELECT * FROM Soort";
            List<VisSoort> soorten = new List<VisSoort>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        soorten.Add(new VisSoort((int)reader["id"], (string)reader["naam"]));
                    }
                    return soorten;
                }
                catch (Exception ex)
                {

                    throw new Exception("LeesVissoorten", ex);
                }
            }
        }

        public void SchrijfStatistieken(List<VisStatsDataRecord> data, string fileName)
        {
            string SQLdata = "INSERT INTO VisStats(jaar,maand,haven_id,soort_id,gewicht,waarde) VALUES (@jaar,@maand,@haven_id,@soort_id,@gewicht,@waarde)";
            string SQLupload = "INSERT INTO Upload(fileName,datum,pad) VALUES (@fileName, @datum, @pad)";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQLdata;
                    cmd.Transaction = conn.BeginTransaction(); // omdat we bijde tabellen willen updaten EN ze MOETEN alletwee slagen anders willen we niet uploaden.
                    cmd.Parameters.Add(new SqlParameter("@jaar", SqlDbType.Int)); // parameters aanmaken.
                    cmd.Parameters.Add(new SqlParameter("@maand", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@haven_id", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@soort_id", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@gewicht", SqlDbType.Float)); // double in cSharp is een float in sql server
                    cmd.Parameters.Add(new SqlParameter("@waarde", SqlDbType.Float));
                    foreach (VisStatsDataRecord d in data)
                    {
                        cmd.Parameters["@jaar"].Value = d.Jaar;
                        cmd.Parameters["@maand"].Value = d.Maand;
                        cmd.Parameters["@haven_id"].Value = d.Haven.ID;
                        cmd.Parameters["@soort_id"].Value = d.Soort.ID;
                        cmd.Parameters["@gewicht"].Value = d.Gewicht;
                        cmd.Parameters["@waarde"].Value = d.Waarde;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = SQLupload;
                    cmd.Parameters.Clear(); // nu willen we andere parameters dus we moeten deze clearen.
                    cmd.Parameters.AddWithValue("@fileName", fileName.Substring(fileName.LastIndexOf("\\")+1)); //Hier gebruiken we addWithValue want we moeten het maar 1 keer doen. Bij het vorige zou hij dan elke keer een nieuwe variabele weer aanmaken wat 'zwaar' is voor het programma.
                    cmd.Parameters.AddWithValue("@pad", fileName.Substring(fileName.LastIndexOf("\\")+1));
                    cmd.Parameters.AddWithValue("@datum", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit(); // bijde wegschrijven naar de juiste DB
                }
                catch ( Exception ex)
                {
                    cmd.Transaction.Rollback(); // Als oftewel de upload of de visstats niet lukt mogen ze alletwee niet uitgevoerd worden.
                    throw new Exception("SchrijfStatistieken", ex);
                }
            }
        }

        public List<int> LeesJaartallen()
        {
            string SQL = "SELECT distinct jaar FROM VisStats";
            List<int> list = new List<int>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add((int)reader["jaar"]);
                    }
                    return list;
                }
                catch (Exception ex)
                {

                    throw new Exception ("LeesJaartallen", ex);
                }
            }
        }

        public List<Jaarvangst> LeesStatistieken(int jaar, Haven haven, List<VisSoort> vissoorten, Eenheid eenheid)
        {
            string kolom = "";
            switch (eenheid)
            {
                case Eenheid.kg: kolom = "gewicht"; break;
                case Eenheid.euro: kolom = "waarde"; break;
            }
            string paramSoorten = "";
            for (int i = 0; i < vissoorten.Count; i++) paramSoorten += $"@ps{i},";
            paramSoorten = paramSoorten.Remove(paramSoorten.Length - 1);
            string SQL = $"SELECT soort_id,t2.naam soortnaam, jaar, sum({kolom}) totaal, min ({kolom}) minimum, max ({kolom}) maximum, avg({kolom}) gemiddelde FROM VisStats t1 left join Soort t2 on t1.soort_id=t2.id WHERE jaar = @jaar and soort_id in ({paramSoorten}) and haven_id=@haven_id GROUP BY soort_id, t2.naam,jaar";
            List<Jaarvangst> vangst = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddWithValue("@haven_id", haven.ID);
                    cmd.Parameters.AddWithValue("@jaar", jaar);
                    for (int i = 0; i < vissoorten.Count; i++) cmd.Parameters.AddWithValue($"@ps{i}", vissoorten[i].ID);
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        vangst.Add(new Jaarvangst((double)reader["gemiddelde"], (double)reader["totaal"], (double)reader["minimum"], (double)reader["maximum"], (string)reader["soortnaam"]));
                    }
                    return vangst;
                }
                catch (Exception ex)
                {
                    throw new Exception("GeefJaarVangst", ex);
                }
            }
        }

        private void ExecuteNonQuery(string query, string parameterName, string parameterValue)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    cmd.Parameters.Add(new SqlParameter(parameterName, System.Data.SqlDbType.NVarChar) { Value = parameterValue });
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while executing a database operation.", ex);
                }
            }
        }

        private bool CheckRecordExists(string query, string parameterName, string parameterValue)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    cmd.Parameters.Add(new SqlParameter(parameterName, System.Data.SqlDbType.NVarChar) { Value = parameterValue });
                    var result = (int)cmd.ExecuteScalar();
                    return result > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception("An error occurred while checking for record existence.", ex);
                }
            }
        }
    }

}
