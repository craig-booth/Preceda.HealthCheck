using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.STP.Entities;

namespace Preceda.HealthCheck.STP.Export
{

    interface IExporter
    {
        IEnumerable<ValidationDatabase> GetDatabases(ValidationRun run);
        IEnumerable<ValidationDetail> GetDetails(ValidationDatabase database);
    }    
}
