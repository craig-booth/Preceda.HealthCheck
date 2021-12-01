using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.DataLayer;
using Preceda.HealthCheck.Import.Entities;

namespace Preceda.HealthCheck.Import
{
    class HealthCheckUnitOfWork : UnitOfWork
    {

        public Repository<ValidationRun> RunRepository { get; private set; }
        public Repository<ValidationDatabase> DatabaseRepository { get; private set; }
        public Repository<ValidationDetail> DetailRepository { get; private set; }


        public HealthCheckUnitOfWork(IDbConnection connection)
             : base(connection)
        {
            RunRepository = new Repository<ValidationRun>(connection);
            DatabaseRepository = new Repository<ValidationDatabase>(connection);
            DetailRepository = new Repository<ValidationDetail>(connection);
        }
    }
}
