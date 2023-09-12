using FluentValidation;
using IEM.Application.Models.Auth;

namespace IEM.Application.Validators
{
    public class PreLoginRequestValidator : AbstractValidator<PreLoginRequestModel>
    {
        public PreLoginRequestValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}
