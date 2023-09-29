using IEM.Application.Interfaces;
using IEM.Application.Models.Auth;
using IEM.Application.Models.Constants;
using IEM.Application.Models.Settings;
using IEM.Application.Models.Users;
using IEM.Application.Utils;
using IEM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

        #region Public
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

        public async ValueTask<LoginResponseModel> LoginAsync(LoginRequestModel model)
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
                    UserId = user.UserId,
                    AdminAccess = user.Role.RoleCode
                };

                var userConnection = await CreateUserConnectionAsync(tokenModel);
                var result = new LoginResponseModel()
                {
                    AccessToken = userConnection.AccessToken,
                    ExpiredDate = userConnection.AccessTokenExpiredDate,
                };
                return result;
            }

            throw new ApplicationException("Wrong user credential");
        }
        #endregion

        #region Private
        private async ValueTask<UserConnection> CreateUserConnectionAsync(UserTokenModel tokenModel)
        {
            var (token, expiredDate) = TokenUtils.CreateJwtAccessToken(_jwtSettingModel, tokenModel);
            var userConnection = new UserConnection()
            {
                UserId = tokenModel.UserId,
                AccessToken = token,
                AccessTokenExpiredDate = expiredDate,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
            };

            await UnitOfWork.UserConnections.CreateAsync(userConnection);
            await UnitOfWork.SaveChangesAsync();

            return userConnection;
        }
        #endregion
    }
}
