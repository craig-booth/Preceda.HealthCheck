using System;
using System.Data.OleDb;
using MySql.Data.MySqlClient;


using Preceda.HealthCheck.DataLayer;
using Preceda.HealthCheck.Import;
using Preceda.SystemUsage.Import;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var importer = new Importer();

            Console.WriteLine("Loading Config");
            importer.LoadConfig();
            Console.WriteLine("Config Loaded");

            Console.WriteLine("Starting Import");
            importer.Import();
            Console.WriteLine("Import Complete");
        }
    }


    class Importer
    {
        private const string _HealthCheckConnectionString = "Server=srv-tpg-qa-db1;Database=PrecedaHealthChecks;User Id=StatsCollection;Password=C8Fev4!5YI;";
        private const string _SystemUsageConnectionString = "Server=srv-tpg-qa-db1;Database=PrecedaSystemUsage;User Id=StatsCollection;Password=C8Fev4!5YI;";

        private string Server = "";
        private string UserName = "";
        private string Password = "";

        public void LoadConfig()
        {
            var connection = new MySqlConnection(_HealthCheckConnectionString);
            connection.Open();

            var importConfig = new ImportConfiguration(connection);
            var task = importConfig.LoadConfiguration();
            task.Wait();

            Server = importConfig.ISeriesServer;
            UserName = importConfig.ISeriesUser;
            Password = importConfig.ISeriesPassword;

            connection.Close();
        }

        public void Import()
        {
            var connectionStringBuilder = new OleDbConnectionStringBuilder();
            connectionStringBuilder["Provider"] = "IBMDA400";
            connectionStringBuilder["Data Source"] = Server;
            connectionStringBuilder["User Id"] = UserName;
            connectionStringBuilder["Password"] = Password;
            var iSeriesConnectionString = connectionStringBuilder.ConnectionString;

            Console.WriteLine("Importing STP...");
            var task1 = ImportData(_HealthCheckConnectionString, iSeriesConnectionString, new HealthCheckImporterFactory("STP"));
            task1.Wait();

            Console.WriteLine("Importing Year End...");
            var task2 = ImportData(_HealthCheckConnectionString, iSeriesConnectionString, new HealthCheckImporterFactory("YE"));
            task2.Wait();
         
            Console.WriteLine("Importing System Usage...");
            var task3 = ImportData(_SystemUsageConnectionString, iSeriesConnectionString, new SystemUsageImporterFactory());
            task3.Wait();

        }

        private async Task ImportData(string sqlServerConnectionString, string iSeriesConnectionString, IDataImporterFactory importerFactory)
        {

            var dataImporter = importerFactory.CreateImporter(sqlServerConnectionString, iSeriesConnectionString);

            var id = Guid.NewGuid();
            await dataImporter.Import(id);
        }

    }
}






