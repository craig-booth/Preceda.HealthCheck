using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.Import.Entities;

namespace Preceda.HealthCheck.Import
{

    public interface IHealthCheckDataExtractor
    {
        int GetDatabaseCount(ValidationRun run);
        IEnumerable<ValidationDatabase> GetDatabases(ValidationRun run);
        IEnumerable<ValidationDetail> GetDetails(ValidationDatabase database);
    }    
}
