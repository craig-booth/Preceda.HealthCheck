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

        public SystemUsageUnitOfWork(IDbConnection connection)
            : base(connection)
        {

            UsageDatabaseRepository = new Repository<UsageDatabase>(connection);
            UsageDetailRepository = new Repository<UsageDetail>(connection);
            UsageItemRepository = new Repository<UsageItem>(connection);
        }
    }
}
