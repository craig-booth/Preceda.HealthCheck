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
        Task<int> GetDatabaseCount(ValidationRun run);
        IAsyncEnumerable<ValidationDatabase> GetDatabases(ValidationRun run);
        IAsyncEnumerable<ValidationDetail> GetDetails(ValidationDatabase database);
    }    
}
