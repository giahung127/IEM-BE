using AutoMapper;
using IEM.Application.Interfaces;
using IEM.Application.Models.Commons;
using IEM.Application.Models.Users;
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
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async ValueTask<IApiResponseModel> GetAllUsers()
        {
            var result = _mapper.Map<IEnumerable<UserBaseModel>>(await _userService.GetAllUsers());
            return ResponseUtils.OkResult(result);
        }
    }
}
