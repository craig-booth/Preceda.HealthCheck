using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

using Preceda.HealthCheck.STP.Entities;


namespace Preceda.HealthCheck.STP
{
    class Repository<T> where T :class
    {
        private IDbConnection _Connection;

        public Repository(IDbConnection connection)
        {
            _Connection = connection;
        }

        public T Get(Guid id)
        {
            var entity = _Connection.Get<T>(id);

            return entity;
        }
        public void Add(T entity)
        {
            _Connection.Insert<T>(entity);       
        }

        public void Update(T entity)
        {

        }
        public void Delete(Guid id)
        {
            var attribute = (TableAttribute)typeof(T).GetCustomAttributes(typeof(TableAttribute), true)[0];

            _Connection.Execute($"Delete from {attribute.Name} where Id = '{id}'");
        }
            
    }
}
