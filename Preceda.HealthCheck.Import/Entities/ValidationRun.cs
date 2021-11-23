using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Preceda.HealthCheck.Import.Entities
{
    [TableName("SystemValidation")]
    public class ValidationRun
    {
        [KeyField]
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime RunDate { get; set; }
        public string ValidationProgram { get; set; }
    }
}
