using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
namespace MinimalAPI.Common
{
    public class AdminRoleRequirement : IAuthorizationRequirement
    {
        public AdminRoleRequirement(string role = "Admin") => Role = role;
        public string Role { get; set; }
    }
    public class AdminRoleRequirementHandler : AuthorizationHandler<AdminRoleRequirement>
    {
        public AdminRoleRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AdminRoleRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Value == requirement.Role))
            {
                context.Succeed(requirement);
            }
            else
            {
                _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                _httpContextAccessor.HttpContext.Response.ContentType = "application/json";
                await _httpContextAccessor.HttpContext.Response.WriteAsJsonAsync(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Unauthorized. Required role."
                });
                await _httpContextAccessor.HttpContext.Response.CompleteAsync();
                context.Fail();
            }

        }
        private readonly IHttpContextAccessor _httpContextAccessor;
    }


    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public override void Validate(string scheme)
        {

        }
    }
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        IOptionsMonitor<BasicAuthenticationOptions> _options;
        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _options = options;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // var authorizationService = Context.RequestServices.GetRequiredService<IAuthorizationService>();
            // var policy = await authorizationService.GetPolicyAsync("Policy1");

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization header");
            }

            var authHeader = Request.Headers["Authorization"].ToString();
            var authHeaderParts = authHeader.Split(' ');
            if (authHeaderParts.Length != 2 || !authHeaderParts[0].Equals("Basic"))
            {
                return AuthenticateResult.Fail("Invalid Authorization header");
            }

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderParts[1]));
            var credentialParts = credentials.Split(':');
            if (credentialParts.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid credentials");
            }

            var username = credentialParts[0];
            var password = credentialParts[1];

            var claims = new List<Claim>
        {
                new Claim(ClaimTypes.Name, username),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
