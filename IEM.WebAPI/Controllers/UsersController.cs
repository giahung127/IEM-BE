using IEM.Application.Interfaces;
using IEM.Application.Models.Commons;
using IEM.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IEM.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("")]
        public async ValueTask<IApiResponseModel> GetAllUsers()
        {
            return await ResponseUtils.OkResultAsync(_userService.GetAllUsers());
        }
    }
}
