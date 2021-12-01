using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.DataLayer;

namespace Preceda.SystemUsage.Import.Entities
{ 

    [TableName("Databases")]
    public class UsageDatabase
    {
        public string DatabaseName { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Version { get; set; }
        public string Partition { get; set; }
        public DateTime MostRecentSurvey { get; set; }

        [IgnoreField]
        public List<UsageDetail> Details { get; set; } = new List<UsageDetail>();
    }
}
