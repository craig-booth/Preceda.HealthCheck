using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.DataLayer;

namespace Preceda.HealthCheck.Import.Entities
{
    [TableName("SystemValidationDatabase")]
    public class ValidationDatabase
    {
        [KeyField]
        public Guid Id { get; set; }
        public string Database { get; set; }
        public string ApplicationKey { get; set; }
        public string DatabaseDescription { get; set; }
        public string Partition { get; set; }
        public string Country { get; set; }
        public string Version { get; set; }

        [IgnoreField]
        public string RunId { get; set; }

    }
}
