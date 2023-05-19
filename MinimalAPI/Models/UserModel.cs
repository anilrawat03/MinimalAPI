using FluentValidation;

namespace MinimalAPI.Models
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public class Validator : AbstractValidator<UserDto>
        {
            public Validator()
            {
                RuleFor(m => m.UserName).NotEmpty();
                RuleFor(m => m.Password).NotEmpty();
            }
        }
    };
}
