using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace IEM.Domain.Core.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();

        public IIncludableQueryable<T, K> Include<K>(Expression<Func<T, K>> exp);

        bool Any(Expression<Func<T, bool>> exp);

        ValueTask<bool> AnyAsync(Expression<Func<T, bool>> exp);

        IQueryable<T> Where(Expression<Func<T, bool>> exp);

        int Count(Expression<Func<T, bool>> exp);

        ValueTask<int> CountAsync(Expression<Func<T, bool>> exp);

        T Single(Expression<Func<T, bool>> exp);

        T SingleOrDefault(Expression<Func<T, bool>> exp);

        ValueTask<T> SingleOrDefaultAsync(Expression<Func<T, bool>> exp);

        T FirstOrDefault(Expression<Func<T, bool>> exp);

        ValueTask<T> FirstOrDefaultAsync(Expression<Func<T, bool>> exp);

        IQueryable<T> AsTracking();

        void Delete(T entity);

        void DeleteRange(params T[] entity);

        ValueTask DeleteAllAsync(Expression<Func<T, bool>> exp);

        T Create(T entity);

        ValueTask<T> CreateAsync(T entity);

        ValueTask CreateRangeAsync(T[] entity);

        void UpdateRange(T[] entity);

        T Update(T entity);

        void ExecuteStoredProcedure(string storedProcName, Dictionary<string, object> parameters);

        ValueTask ExecuteStoredProcedureAsync(string storedProcName, Dictionary<string, object> parameters);

        IEnumerable<TResponseModel> ExecuteStoredProcedure<TRequestModel, TResponseModel>(TRequestModel model)
            where TResponseModel : class where TRequestModel : class;

        ValueTask<IEnumerable<TResponseModel>> ExecuteStoredProcedureAsync<TRequestModel, TResponseModel>(TRequestModel model)
            where TResponseModel : class where TRequestModel : class;

        TResponseModel ExecuteStoredProcedureFirstOrDefault<TRequestModel, TResponseModel>(TRequestModel model)
            where TResponseModel : class where TRequestModel : class;

        ValueTask<TResponseModel> ExecuteStoredProcedureFirstOrDefaultAsync<TRequestModel, TResponseModel>(TRequestModel model)
            where TResponseModel : class where TRequestModel : class;

        void ExecuteStoredProcedure<TRequestModel>(TRequestModel model) where TRequestModel : class;

        ValueTask ExecuteStoredProcedureAsync<TRequestModel>(TRequestModel model) where TRequestModel : class;
    }
}
