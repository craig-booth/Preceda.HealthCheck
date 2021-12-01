using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.DataLayer;

namespace Preceda.SystemUsage.Import.Entities
{
    [TableName("ItemDefinitions")]
    public class UsageItem
    {
        public string ItemId { get; set; }
        public string ItemDescription { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSubCategory { get; set; }
        public string ProductEpic { get; set; }
        public string ItemType { get; set; }
        public string CountryCodes { get; set; }
        public string LowerReleaseLevel { get; set; }
        public string UpperReleaseLevel { get; set; }
        public string Enabled { get; set; }
    }
}
