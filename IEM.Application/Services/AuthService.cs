using IEM.Application.Interfaces;
using IEM.Application.Models.Auth;
using IEM.Application.Models.Constants;
using IEM.Application.Models.Settings;
using IEM.Application.Models.Users;
using IEM.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IEM.Application.Services
{
    internal class AuthService : BaseService, IAuthService
    {
        private readonly IUserService _userService;
        private readonly JwtSettingModel _jwtSettingModel;
        public AuthService(
            IServiceProvider provider,
            IUserService userService,
            IConfiguration configuration,
            ILogger<UserService> logger) : base(provider, logger)
        {
            _userService = userService;
            _jwtSettingModel = configuration.GetSection(AppSettingConstants.JWT).Get<JwtSettingModel>()!;
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
            var user = await UnitOfWork.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
            {
                throw new ApplicationException("User not found");
            }

            if (HashUtils.VerifyHashedPassword(user.Password!, model.Password!)) 
            {
                var tokenModel = new UserTokenModel
                {
                    Email = user.Email,
                    UserId = user.Id,
                    AdminAccess = user.Role.RoleCode
                };

                var (token, expiredDate) = TokenUtils.CreateJwtAccessToken(_jwtSettingModel, tokenModel);

                return true;
            }

            throw new ApplicationException("Wrong user credential");
        }
    }
}
