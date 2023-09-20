using IEM.Application.Interfaces;
using IEM.Application.Models.Auth;
using IEM.Application.Models.Users;
using IEM.Application.Utils;
using Microsoft.Extensions.Logging;

namespace IEM.Application.Services
{
    internal class AuthService : BaseService, IAuthService
    {
        private readonly IUserService _userService;
        public AuthService(
            IServiceProvider provider,
            IUserService userService,
            ILogger<UserService> logger) : base(provider, logger)
        {
            _userService = userService;
        }

        public async ValueTask<bool> RegisterUserAsync(UserRegistrationModel model)
        {
            var existsCheck = await _userService.CheckUserExistsAsync(model.Email!);
            if (existsCheck)
            {
                throw new ApplicationException("Email already registed");
            }

            model.Password = HashUtils.HashPassword(model.Password!);
            await _userService.CreateUserAsync(Mapper.Map<UserCreateModel>(model));

            return true;
        }

        public async ValueTask<bool> LoginAsync(LoginRequestModel model)
        {
            var user = await UnitOfWork.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (user == null)
            {
                throw new ApplicationException("User not found");
            }

            if (HashUtils.VerifyHashedPassword(user.Password!, model.Password!)) 
            {
                return true;
            }

            return false;
        }
    }
}
