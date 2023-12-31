﻿using IEM.Application.Models.Auth;

namespace IEM.Application.Interfaces
{
    public interface IAuthService
    {
        TokenValidationModel ValidateAccessToken(string accessToken);
        ValueTask<bool> RegisterUserAsync(UserRegistrationModel model);
        ValueTask<LoginResponseModel> LoginAsync(LoginRequestModel model);
    }
}
