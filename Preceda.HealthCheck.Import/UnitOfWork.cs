using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Preceda.HealthCheck.Import.Entities;

namespace Preceda.HealthCheck.Import
{
    class UnitOfWork : IDisposable
    {
        private IDbConnection _Connection;
        private IDbTransaction _Transaction;

        public Repository<ValidationRun> RunRepository { get; private set; }
        public Repository<ValidationDatabase> DatabaseRepository { get; private set; }
        public Repository<ValidationDetail> DetailRepository { get; private set; }


        public UnitOfWork(IDbConnection connection)
        {
            _Connection = connection;

            RunRepository = new Repository<ValidationRun>(_Connection);
            DatabaseRepository = new Repository<ValidationDatabase>(_Connection);
            DetailRepository = new Repository<ValidationDetail>(_Connection);

            _Transaction = _Connection.BeginTransaction();
        }

        public void Save()
        {
            _Transaction.Commit();
            _Transaction = null;
        }
        public void Dispose()
        {
            if (_Transaction != null)
                _Transaction.Rollback();
        }
    }
}
