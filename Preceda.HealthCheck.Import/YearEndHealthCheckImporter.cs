using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Preceda.HealthCheck.Import.Entities;

namespace Preceda.HealthCheck.Import
{
    public class YearEndHealthCheckImporter : HealthCheckImporter
    {
        public YearEndHealthCheckImporter(IDbConnection dBConnection, IHealthCheckDataExtractor extractor)
            : base(dBConnection, extractor)
        {

        }

        public override async Task Import(Guid id)
        {
            var validation = new ValidationRun()
            {
                Id = id,
                Description = "Year End",
                RunDate = DateTime.Today,
                ValidationProgram = "YEHC010C"
            };

            await ImportValidation(validation);
        }
    }
}
