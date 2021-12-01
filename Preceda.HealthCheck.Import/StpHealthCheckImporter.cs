using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

//using Preceda.HealthCheck.DataLayer;
using Preceda.HealthCheck.Import.Entities;

namespace Preceda.HealthCheck.Import
{
    public class StpHealthCheckImporter : HealthCheckImporter
    {

        public StpHealthCheckImporter(IDbConnection dBConnection, IHealthCheckDataExtractor extractor)
            : base(dBConnection, extractor)
        {

        }

        public override async Task Import(Guid id)
        {
            var validation = new ValidationRun()
            {
                Id = id,
                Description = "Single Touch Payroll",
                RunDate = DateTime.Today,
                ValidationProgram = "STPHC001C"
            };

            await ImportValidation(validation);
        }
    }
}
