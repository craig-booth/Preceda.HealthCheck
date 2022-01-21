using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.DataLayer;
using Preceda.SystemUsage.Import;
using Preceda.SystemUsage.Import.Entities;

namespace Preceda.SystemUsage.Import
{
    public class SystemUsageDataExtractor : ISystemUsageDataExtractor, IDisposable
    {
        private OleDbConnection _Connection;

        public SystemUsageDataExtractor(string iSeriesConnectionString)
        {
            _Connection = new OleDbConnection(iSeriesConnectionString);
            _Connection.Open();
        }

        public async Task<int> GetDatabaseCount()
        {
              var command = _Connection.CreateCommand();
              command.CommandText = $"select count(distinct SRPRDBID) from SYSVALDTA.SYRPPSRS";

              var count =  (int) await command.ExecuteScalarAsync();

              return count;
        }

        public async IAsyncEnumerable<UsageDatabase> GetDatabases()
        {
            var command = _Connection.CreateCommand();
            command.CommandText = "select SRPRDBID, SRPRLNAM, SRPRCTRY, SRPRVERS, SRPRPART, SRPRCRDT, "
                                + "SRPRITID, SRPRAPK1, SRPRAPK2, SRPRVALC, SRPRVALN, SRPRVALD, SRPRVALT, SRPRVALZ "
                                + "from SYSVALDTA.SYRPPSRS ORDER BY SRPRDBID, SRPRITID";

            var reader = await command.ExecuteReaderAsync();

            UsageDatabase? database = null;
            while (await reader.ReadAsync())
            {
                var databaseName = reader.GetString(0);

                if ((database != null) && (database.DatabaseName != databaseName))
                {
                    yield return database;
                    database = null;
                }

                if (database == null)
                {
                    database = new UsageDatabase()
                    {
                        DatabaseName = databaseName,
                        Description = reader.GetString(1),
                        Country = reader.GetString(2),
                        Version = reader.GetString(3),
                        Partition = reader.GetString(4),
                        MostRecentSurvey = reader.GetISeriesDate(5)
                    };
                }

                var detail = new UsageDetail()
                {
                    DatabaseName = databaseName,
                    ItemId = reader.GetString(6),
                    ApplicationKey1 = reader.GetString(7),
                    ApplicationKey2 = reader.GetString(8),
                    CharacterValue = reader.GetString(9),
                    NumericValue = reader.GetDecimal(10),
                    DateValue = reader.GetISeriesDate(11),
                    TimeValue = reader.GetISeriesTime(12),
                    DateTimeValue = reader.GetISeriesTimeStamp(13)
                };
                database.Details.Add(detail);
            } 

            if (database != null)
                yield return database;
        }

        public async IAsyncEnumerable<UsageItem> GetItems()
        {
            var command = _Connection.CreateCommand();
            command.CommandText = "select SRPIITID, SRPIDESC, SRPIPRCT, SRPIPRSC, SRPIPREP, SRPITYPE, "
                                + "SRPICTRY, SRPIRELL, SRPIRELU, SRPIENAB "
                                + "from SYSVALCTL.SYRPPSIT";


            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var item = new UsageItem()
                {
                    ItemId = reader.GetString(0),
                    ItemDescription = reader.GetString(1),
                    ProductCategory = reader.GetString(2),
                    ProductSubCategory = reader.GetString(3),
                    ProductEpic = reader.GetString(4),
                    ItemType = reader.GetString(5),
                    CountryCodes = reader.GetString(6),
                    LowerReleaseLevel = reader.GetString(7),
                    UpperReleaseLevel = reader.GetString(8),
                    Enabled = reader.GetString(9)
                };

                yield return item;
            }

            reader.Close();
        }

        public void Dispose()
        {
            if (_Connection != null)
                _Connection.Close();
        }
    }
}
