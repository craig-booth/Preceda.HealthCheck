using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

namespace Preceda.HealthCheck.STP.Entities
{
    [Table("SystemValidationDatabase")]
    class ValidationDatabase
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string Database { get; set; }
        public string ApplicationKey { get; set; }
        public string DatabaseDescription { get; set; }
        public string Partition { get; set; }
        public string Country { get; set; }
        public string Version { get; set; }

        [Write(false)]
        [Computed]
        public string RunId { get; set; }

    }
}
