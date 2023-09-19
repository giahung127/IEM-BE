using IEM.Application.Interfaces;
using IEM.Application.Models.Users;
using IEM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("IEM.UnitTest")]
namespace IEM.Application.Services
{
    internal class UserService : BaseService, IUserService
    {
        public UserService(IServiceProvider provider,
            ILogger<UserService> logger) : base(provider, logger)
        {
        }
        public async ValueTask<IEnumerable<UserBaseModel>> GetAllUsers()
        {
            var users = await UnitOfWork.Users.ToListAsync();

            return Mapper.Map<IEnumerable<UserBaseModel>>(users);
        }
    }
}
