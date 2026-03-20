using Eccomerce_Web.Modules.Auth.Dtos.Request;
using FluentValidation;

namespace Eccomerce_Web.Modules.Auth.Validators
  
{
    public class AuthValidator : AbstractValidator<RegisterDto>
    {
        public AuthValidator()
        {
            RuleFor(x => x.Email)
              .NotEmpty().WithMessage("Email is required")
              .EmailAddress().WithMessage("Invalid email format")
              .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6) 
                .MaximumLength(50)
                .Matches(@"[A-Z]").WithMessage("Must contain uppercase letter")
                .Matches(@"[a-z]").WithMessage("Must contain lowercase letter")
                .Matches(@"\d").WithMessage("Must contain number");

            RuleFor(x => x.FullName).NotEmpty().WithMessage("Name is required").MaximumLength(100); ;
        }




    }
    
}
