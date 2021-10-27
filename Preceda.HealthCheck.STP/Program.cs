using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

using Preceda.HealthCheck.STP.Entities;
using Preceda.HealthCheck.STP.Export;

namespace Preceda.HealthCheck.STP
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=srv-tpg-qa-db1;Database=PrecedaHealthChecks;User Id=StatsCollection;Password=C8Fev4!5YI;";
            var connection = new MySqlConnection(connectionString);
            connection.Open();
  
            using (var exporter = new ISeriesExporter("eb1", "craigb", "craigb"))
            {
                var import = new SingleTouchPayrollImport(connection, exporter);

                var id = Guid.NewGuid();
                import.ImportSTPHealthCheck(id);

                id = Guid.NewGuid();
                import.ImportYearEndHealthCheck(id);
            }

            connection.Close();
        }
    }
}
