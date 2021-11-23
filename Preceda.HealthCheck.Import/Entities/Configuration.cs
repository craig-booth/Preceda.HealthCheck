using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preceda.HealthCheck.Import.Entities
{
    [TableName("Configuration")]
    class Configuration
    {
        [KeyField]
        public Guid Id { get; set; }

        public string ISeriesServer { get; set; }
        public string ISeriesUser { get; set; }
        public string ISeriesPassword { get; set; }

    }
}
