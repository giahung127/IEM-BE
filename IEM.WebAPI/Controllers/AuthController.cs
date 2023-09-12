using IEM.Application.Interfaces;
using IEM.Application.Models.Auth;
using IEM.Application.Models.Commons;
using IEM.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IEM.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    [Authorize]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("PreLogin")]
        [AllowAnonymous]
        public async ValueTask<IApiResponseModel> PreLoginAsync(PreLoginRequestModel request)
        {
            return await ResponseUtils.OkResultAsync(_userService.GetAllUsers());
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async ValueTask<IApiResponseModel> LoginAsync(LoginRequestModel request)
        {
            return await ResponseUtils.OkResultAsync(_userService.GetAllUsers());
        }

    }
}
