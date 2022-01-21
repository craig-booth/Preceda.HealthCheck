using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.DataLayer;
using Preceda.SystemUsage.Import.Entities;

namespace Preceda.SystemUsage.Import
{
    class SystemUsageUnitOfWork : UnitOfWork
    {
        public Repository<UsageDatabase> UsageDatabaseRepository { get; private set; }
        public Repository<UsageDetail> UsageDetailRepository { get; private set; }
        public Repository<UsageItem> UsageItemRepository { get; private set; }

        public SystemUsageUnitOfWork(string connectionString)
            : base(connectionString)
        {
            UsageDatabaseRepository = new Repository<UsageDatabase>(_Connection);
            UsageDetailRepository = new Repository<UsageDetail>(_Connection);
            UsageItemRepository = new Repository<UsageItem>(_Connection);
        }
    }
}
