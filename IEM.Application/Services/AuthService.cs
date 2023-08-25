using IEM.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace IEM.Application.Services
{
    internal class AuthService : BaseService, IAuthService
    {
        public AuthService(IServiceProvider provider,
            ILogger<UserService> logger) : base(provider, logger)
        {
        }
    }
}
