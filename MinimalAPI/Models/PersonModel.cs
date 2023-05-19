using FluentValidation;
using MinimalAPI.Common;
using System.Reflection;
namespace MinimalAPI.Models
{
    public class PersonModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        //  public IFormFile File { get; set; }
        public class Validator : AbstractValidator<PersonModel>
        {
            public Validator()
            {
                RuleFor(m => m.Name).NotEmpty();
                RuleFor(m => m.Email).EmailAddress().WithMessage("Invalid email address");
                RuleFor(m => m.Age).NotEmpty().GreaterThan(0);
            }
        }

        public PersonModel() { }
        //public async static Task<PersonModel> BindAsync(HttpContext context) => new PersonModel();
        //public static async ValueTask<PersonModel> BindAsync(HttpContext httpContext, ParameterInfo parameter)
        //{
        //    return await httpContext.BindFromForm<PersonModel>();
        //}
    };
}
