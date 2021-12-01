using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.DataLayer;


namespace Preceda.HealthCheck.Import.Entities
{
    [TableName("SystemValidationDetail")]
    public class ValidationDetail
    {
        [KeyField]
        public Guid Id { get; set; }
        public string Database { get; set; }
        public string ApplicationKey { get; set; }
        public string GroupId { get; set; }
        public int  SectionSequence { get; set; }
        public int ItemSequence{ get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public string ItemId { get; set; }
        public string FunctionId { get; set; }
        public int CountFor { get; set; }
        public int CountAgainst { get; set; }
        public int CountTotal { get; set; }
        public decimal PercentFor { get; set; }
        public decimal PercentAgainst { get; set; }
        public string ValueChar { get; set; }
        public decimal ValueNumeric { get; set; }
        public DateTime ValueDate { get; set; }
        public TimeSpan ValueTime { get; set; }
        public DateTime ValueTimeStamp { get; set; }
        public string Color { get; set; }
        public string Image { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }
    }
}
