using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

namespace Preceda.HealthCheck.STP.Entities
{
    [Table("SystemValidation")]
    class ValidationRun
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime RunDate { get; set; }
        public string ValidationProgram { get; set; }
    }
}
