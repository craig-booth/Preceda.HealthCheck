using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.STP.Entities;

namespace Preceda.HealthCheck.STP.Export
{
    class ISeriesExporter : IExporter, IDisposable
    {
        private OleDbConnection _Connection;

        public ISeriesExporter(string server, string userName, string password)
        {
            var connectionStringBuilder = new OleDbConnectionStringBuilder();
            connectionStringBuilder["Provider"] = "IBMDA400";
            connectionStringBuilder["Data Source"] = server;
            connectionStringBuilder["User Id"] = userName;
            connectionStringBuilder["Password"] = password;

            _Connection = new OleDbConnection(connectionStringBuilder.ConnectionString);
            _Connection.Open();
        }
        public IEnumerable<ValidationDatabase> GetDatabases(ValidationRun run)
        {
            var command = _Connection.CreateCommand();
            command.CommandText = "select SRELDBID, SRELAPPK, SRELDBDS, SRELPART, SRELCTRY, SRELVERS, SRELRNID "
                                + $" from SYSVALDTA.SYRPEXLG where SRELPROG='{run.ValidationProgram}'";

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var database = new ValidationDatabase()
                {
                    Id = Guid.Empty,
                    Database = reader.GetString(0),
                    ApplicationKey = reader.GetString(1),
                    DatabaseDescription = reader.GetString(2),
                    Partition = reader.GetString(3),
                    Country = reader.GetString(4),
                    Version = reader.GetString(5),
                    RunId = reader.GetString(6)
                };

                yield return database;
            }
        }

        public IEnumerable<ValidationDetail> GetDetails(ValidationDatabase database)
        {

            var command = _Connection.CreateCommand();
            command.CommandText = "select SRSRGRID, SRSRSESQ, SRSRITSQ, SRSRCAT1, SRSRCAT2, SRSRCAT3, SRSRITID, SRSRFUNC, SRSRCNTF, "
                                        + "SRSRCNTA, SRSRCNTT, SRSRPERF, SRSRPERA, SRSRVALC, SRSRVALN, SRSRVALD, "
                                        + "SRSRVALT, SRSRVALZ, SRSRBCLR, SRSRIMAG, SRSRMSG1, SRSRMSG2 "
                                        + $"from SYSVALDTA.SYRPSRES where SRSRDBID='{database.Database}' and SRSRRNID='{database.RunId}' and SRSRAPPK='{database.ApplicationKey}' ";


            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var detail = new ValidationDetail()
                {
                    Id = Guid.Empty,
                    Database = database.Database,
                    ApplicationKey = database.ApplicationKey,
                    GroupId = reader.GetString(0),
                    SectionSequence = Convert.ToInt32(reader.GetDecimal(1)),
                    ItemSequence = Convert.ToInt32(reader.GetDecimal(2)),
                    Category1 = reader.GetString(3),
                    Category2 = reader.GetString(4),
                    Category3 = reader.GetString(5),
                    ItemId = reader.GetString(6),
                    FunctionId = reader.GetString(7),
                    CountFor = Convert.ToInt32(reader.GetDecimal(8)),
                    CountAgainst = Convert.ToInt32(reader.GetDecimal(9)),
                    CountTotal = Convert.ToInt32(reader.GetDecimal(10)),
                    PercentFor = reader.GetDecimal(11),
                    PercentAgainst = reader.GetDecimal(12),
                    ValueChar = reader.GetString(13),
                    ValueNumeric = reader.GetDecimal(14),
                    ValueDate = reader.GetISeriesDate(15),
                    ValueTime = reader.GetISeriesTime(16),
                    ValueTimeStamp = reader.GetISeriesTimeStamp(17),
                    Color = reader.GetString(18),
                    Image = reader.GetString(19),
                    Message1 = reader.GetString(20),
                    Message2 = reader.GetString(21)
                };

                yield return detail;
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
