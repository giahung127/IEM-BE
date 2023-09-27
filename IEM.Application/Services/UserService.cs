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

        public async ValueTask<User> CreateUserAsync(UserCreateModel model)
        {
            var user = Mapper.Map<User>(model);

            user.RoleId = (await UnitOfWork.Roles.FirstOrDefaultAsync(x => x.RoleCode == 1))!.Id;

            await UnitOfWork.Users.CreateAsync(user);
            await UnitOfWork.SaveChangesAsync();

            return user;
        }

        public async ValueTask<UserSingleModel> GetSingleUserAysnc(int id)
        {
            var user = await this.UnitOfWork.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new ApplicationException("User not found");
            }

            return Mapper.Map<UserSingleModel>(user);
        }

        public async ValueTask<UserSingleModel> GetSingleUserAysnc(string mail)
        {
            var user = await this.UnitOfWork.Users.FirstOrDefaultAsync(x => x.Email == mail);

            if (user == null)
            {
                throw new ApplicationException("User not found");
            }

            return Mapper.Map<UserSingleModel>(user);
        }

        public async ValueTask<bool> CheckUserExistsAsync(string Email)
        {
            if (await UnitOfWork.Users.AnyAsync(x => x.Email == Email))
            {
                return true;
            }

            return false;    
        }
    }
}
