using IEM.Domain.Core.Repositories;
using IEM.Domain.Entities;
using IEM.Infrastructure.Data;

namespace IEM.Infrastructure.Repositories
{
    internal class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}
