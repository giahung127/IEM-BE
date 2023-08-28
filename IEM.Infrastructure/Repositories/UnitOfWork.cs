using IEM.Domain.Core.Repositories;
using IEM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace IEM.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDatabaseContext DbContext;
        private readonly IServiceProvider ServiceProvider;

        public UnitOfWork(IServiceProvider serviceProvider, ApplicationDatabaseContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return DbContext.Database.BeginTransaction();
        }

        public async ValueTask<IDbContextTransaction> BeginTransactionAsync()
        {
            return await DbContext.Database.BeginTransactionAsync();
        }

        public void RunInTransaction(Action action)
        {
            var transaction = BeginTransaction();
            try
            {
                action();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async ValueTask RunInTransactionAsync(Action action)
        {
            var transaction = await BeginTransactionAsync();
            try
            {
                action();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async ValueTask<TResult> RunInTransactionAsync<T, TResult>(Func<T, ValueTask<TResult>> funcAsync, T arg)
        {
            var transaction = await BeginTransactionAsync();
            TResult result;
            try
            {
                result = await funcAsync(arg);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            return result;
        }

        public async ValueTask<TResult> RunInTransactionAsync<TResult>(Func<ValueTask<TResult>> funcAsync)
        {
            var transaction = await BeginTransactionAsync();
            TResult result;
            try
            {
                result = await funcAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            return result;
        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public async ValueTask<int> SaveChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        public IUserRepository Users => ServiceProvider.GetRequiredService<IUserRepository>();
    }
}
