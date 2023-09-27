using IEM.Domain.Core.Repositories;
using IEM.Domain.Entities;
using IEM.Infrastructure.Data;

namespace IEM.Infrastructure.Repositories
{
    internal class UserConnectionRepository : Repository<UserConnection>, IUserConnectionRepository
    {
        public UserConnectionRepository(ApplicationDatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}
