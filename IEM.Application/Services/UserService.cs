using IEM.Application.Interfaces;
using IEM.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IEM.Application.Services
{
    internal class UserService : BaseService, IUserService
    {
        public UserService(IServiceProvider provider,
            ILogger<UserService> logger) : base(provider, logger)
        {
        }
        public async ValueTask<IEnumerable<User>> GetAllUsers()
        {
            var users = UnitOfWork.Users.AsQueryable();

            return users;
        }
    }
}
