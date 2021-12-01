using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.SystemUsage.Import.Entities;

namespace Preceda.SystemUsage.Import
{

    public interface ISystemUsageDataExtractor
    {
        Task<int> GetDatabaseCount();
        IAsyncEnumerable<UsageDatabase> GetDatabases();
        IAsyncEnumerable<UsageItem> GetItems();
    }    
}
