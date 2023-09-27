using Microsoft.EntityFrameworkCore.Storage;

namespace IEM.Domain.Core.Repositories
{
    public interface IUnitOfWork
    {
        IDbContextTransaction BeginTransaction();

        ValueTask<IDbContextTransaction> BeginTransactionAsync();

        void RunInTransaction(Action action);

        ValueTask RunInTransactionAsync(Action action);

        ValueTask<TResult> RunInTransactionAsync<TResult>(Func<ValueTask<TResult>> funcAsync);

        ValueTask<TResult> RunInTransactionAsync<T, TResult>(Func<T, ValueTask<TResult>> funcAsync, T arg);

        int SaveChanges();

        ValueTask<int> SaveChangesAsync();

        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
    }
}
