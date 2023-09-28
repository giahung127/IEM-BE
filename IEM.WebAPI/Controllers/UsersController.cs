using Hangfire;
using IEM.Application.Interfaces;
using IEM.Application.Models.Commons;
using IEM.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        [Authorize(Policy = "Admin")]
        public async ValueTask<IApiResponseModel> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();
            return ResponseUtils.OkResult(result);
        }

        [HttpGet("JobTest")]
        public async ValueTask<IApiResponseModel> TestJob()
        {
            var jobId = BackgroundJob.Enqueue(() => Debug.WriteLine("FromJob: Welcome"));
            return ResponseUtils.OkResult(jobId);
        }
    }
}
