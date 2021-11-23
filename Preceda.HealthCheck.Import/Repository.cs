using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Dapper;


using Preceda.HealthCheck.Import.Entities;

namespace Preceda.HealthCheck.Import
{
    class Repository<T> where T :class
    {
        private IDbConnection _Connection;

        public Repository(IDbConnection connection)
        {
            _Connection = connection;
        }

        public async Task<T> Get(Guid id)
        {
            var sql = SqlBuilder.SelectByIdCommand<T>(id);
            var entity = await _Connection.QuerySingleOrDefaultAsync<T>(sql); 

            return entity;
        }
        public async Task Add(T entity)
        {
            var sql = SqlBuilder.InsertCommand<T>(entity);

            await _Connection.ExecuteAsync(sql);
        }

        public async Task Add(IEnumerable<T> entities)
        {
            var stringBuilder = new StringBuilder();
            foreach (var entity in entities)
            {
                var sql = SqlBuilder.InsertCommand<T>(entity);
                stringBuilder.Append(sql);
            }

            var allSql = stringBuilder.ToString();
            if (allSql != "")
                await _Connection.ExecuteAsync(allSql);
        }

        public async Task Update(T entity)
        {
            var sql = SqlBuilder.UpdateCommand<T>(entity);
            await _Connection.ExecuteAsync(sql);
        }

        public async Task Delete(Guid id)
        {
            var sql = SqlBuilder.DeleteByIdCommand<T>(id);
            await _Connection.ExecuteAsync(sql);
        }
            
    }
}
