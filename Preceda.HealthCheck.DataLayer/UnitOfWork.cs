using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace Preceda.HealthCheck.DataLayer
{
    public class UnitOfWork : IDisposable
    {
        protected IDbConnection _Connection;
        protected IDbTransaction? _Transaction;


        public UnitOfWork(string connectionString)
        {
            _Connection = new MySqlConnection(connectionString);
            _Connection.Open();


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

            if (_Connection != null)
                _Connection.Close();
        }
    }
}
