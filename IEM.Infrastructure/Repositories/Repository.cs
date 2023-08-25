using EntityFrameworkExtras.EFCore;
using IEM.Domain.Core.Repositories;
using IEM.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace IEM.Infrastructure.Repositories
{
    internal class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDatabaseContext DbContext;
        protected DbSet<T> DbSet => this.DbContext.Set<T>();

        public Repository(ApplicationDatabaseContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public IQueryable<T> AsQueryable()
        {
            return this.DbSet;
        }

        public IIncludableQueryable<T, K> Include<K>(Expression<Func<T, K>> exp)
        {
            return this.DbSet.Include(exp);
        }

        public IQueryable<T> AsTracking()
        {
            return AsQueryable().AsTracking<T>();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> exp)
        {
            return this.DbSet.Where(exp);
        }

        public int Count(Expression<Func<T, bool>> exp)
        {
            return this.DbSet.Where(exp).Count();
        }

        public async ValueTask<int> CountAsync(Expression<Func<T, bool>> exp)
        {
            return await this.DbSet.Where(exp).CountAsync();
        }

        public T Single(Expression<Func<T, bool>> exp)
        {
            return this.DbSet.Single(exp);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> exp)
        {
            return this.DbSet.SingleOrDefault(exp);
        }

        public async ValueTask<T> SingleOrDefaultAsync(Expression<Func<T, bool>> exp)
        {
            return await this.DbSet.SingleOrDefaultAsync(exp);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> exp)
        {
            return this.DbSet.FirstOrDefault(exp);
        }

        public async ValueTask<T> FirstOrDefaultAsync(Expression<Func<T, bool>> exp)
        {
            return await this.DbSet.FirstOrDefaultAsync(exp);
        }

        public virtual void Delete(T entity)
        {
            this.DbSet.Remove(entity);
        }

        public void DeleteRange(params T[] entity)
        {
            this.DbSet.RemoveRange(entity);
        }

        public virtual async ValueTask DeleteAllAsync(Expression<Func<T, bool>> exp)
        {
            var objects = await this.DbSet.AsTracking().Where(exp).ToListAsync();
            foreach (var obj in objects)
            {
                this.DbSet.Remove(obj);
            }
        }

        public virtual T Create(T entity)
        {
            var dbEntry = this.DbSet.Add(entity);
            return dbEntry.Entity;
        }

        public virtual async ValueTask<T> CreateAsync(T entity)
        {
            var dbEntry = await this.DbSet.AddAsync(entity);
            return dbEntry.Entity;
        }

        public async ValueTask CreateRangeAsync(T[] entity)
        {
            await this.DbSet.AddRangeAsync(entity);
        }

        public virtual T Update(T entity)
        {
            var dbEntry = this.DbSet.Update(entity);
            return dbEntry.Entity;
        }

        public virtual void UpdateRange(T[] entities)
        {
            this.DbSet.UpdateRange(entities);
        }

        public bool Any(Expression<Func<T, bool>> exp)
        {
            return this.DbSet.Where(exp).Any();
        }

        public async ValueTask<bool> AnyAsync(Expression<Func<T, bool>> exp)
        {
            return await this.DbSet.Where(exp).AnyAsync();
        }

        private Tuple<string, string> GenerateStoredProcedure(string storedProcName, Dictionary<string, object> parameters)
        {
            var sqlParameters = parameters.Select(x => new SqlParameter(x.Key, x.Value ?? DBNull.Value)).ToArray();
            var parameterName = string.Join(",", parameters.Select(x => "@" + x.Key).ToArray());
            if (parameterName != ",")
            {
                storedProcName += " " + parameterName;
            }
            return Tuple.Create(storedProcName, parameterName);
        }

        public void ExecuteStoredProcedure(string storedProcName, Dictionary<string, object> parameters)
        {
            var (spName, parameterName) = GenerateStoredProcedure(storedProcName, parameters);
            this.DbContext.Database.ExecuteSqlRaw(spName, parameterName);
        }

        public async ValueTask ExecuteStoredProcedureAsync(string storedProcName, Dictionary<string, object> parameters)
        {
            var (spName, parameterName) = GenerateStoredProcedure(storedProcName, parameters);
            await this.DbContext.Database.ExecuteSqlRawAsync(spName, parameterName);
        }

        public IEnumerable<TResponseModel> ExecuteStoredProcedure<TRequestModel, TResponseModel>(TRequestModel model)
            where TResponseModel : class where TRequestModel : class
        {
            return this.DbContext.Database.ExecuteStoredProcedure<TResponseModel>(model);
        }

        public async ValueTask<IEnumerable<TResponseModel>> ExecuteStoredProcedureAsync<TRequestModel, TResponseModel>(TRequestModel model)
            where TResponseModel : class where TRequestModel : class
        {
            return await this.DbContext.Database.ExecuteStoredProcedureAsync<TResponseModel>(model);
        }

        public TResponseModel ExecuteStoredProcedureFirstOrDefault<TRequestModel, TResponseModel>(TRequestModel model)
            where TResponseModel : class where TRequestModel : class
        {
            return this.DbContext.Database.ExecuteStoredProcedureFirstOrDefault<TResponseModel>(model);
        }

        public async ValueTask<TResponseModel> ExecuteStoredProcedureFirstOrDefaultAsync<TRequestModel, TResponseModel>(TRequestModel model)
            where TResponseModel : class where TRequestModel : class
        {
            return await this.DbContext.Database.ExecuteStoredProcedureFirstOrDefaultAsync<TResponseModel>(model);
        }

        public void ExecuteStoredProcedure<TRequestModel>(TRequestModel model) where TRequestModel : class
        {
            this.DbContext.Database.ExecuteStoredProcedure(model);
        }

        public async ValueTask ExecuteStoredProcedureAsync<TRequestModel>(TRequestModel model) where TRequestModel : class
        {
            await this.DbContext.Database.ExecuteStoredProcedureAsync(model);
        }
    }
}
