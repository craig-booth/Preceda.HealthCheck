using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.DataLayer;

namespace Preceda.SystemUsage.Import.Entities
{
    [TableName("SystemUsage")]
    public class UsageDetail
    {
        public string DatabaseName { get; set; }
        public string ItemId { get; set; }
        public string ApplicationKey1 { get; set; }
        public string ApplicationKey2 { get; set; }
        public string CharacterValue { get; set; }
        public decimal NumericValue{ get; set; }
        public DateTime DateValue { get; set; }
        public TimeSpan TimeValue { get; set; }
        public DateTime DateTimeValue { get; set; }
    }
}
