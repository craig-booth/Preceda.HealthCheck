using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preceda.HealthCheck.DataLayer
{
    public class UnitOfWork : IDisposable
    {
        private IDbConnection _Connection;
        private IDbTransaction? _Transaction;


        public UnitOfWork(IDbConnection connection)
        {
            _Connection = connection;

            _Transaction = _Connection.BeginTransaction();
        }

        public void Save()
        {
            if (_Transaction != null)
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
