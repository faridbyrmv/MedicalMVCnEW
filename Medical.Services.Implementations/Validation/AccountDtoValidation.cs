using FluentValidation;
using Medical.Service.Interfaces.Dtos.Account;

namespace Medical.Services.Implementations.Validation
{
    public class AccountDtoValidation : AbstractValidator<AccountDto>
    {
        public AccountDtoValidation()
        {
            RuleFor(x => x.UserEmail).NotEmpty().WithMessage("Please enter your name.");
            RuleFor(x => x.UserPassword).NotEmpty().WithMessage("Please enter your password.");
        }
    }
}
