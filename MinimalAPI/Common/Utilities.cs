using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace MinimalAPI.Common
{
    public static class Utilities
    {
        public static AuthorizeuserModel LoggedUser(this HttpContext context)
        {
            //var test = context .User.Claims.Count();
            // string data = context.Request.Headers["TestingHeader"];
            var model = new AuthorizeuserModel
            {
                UserId = new Guid(context
                .User.Claims
                .FirstOrDefault(g => g.Type == ClaimTypes.NameIdentifier).Value),
                Name = context.User.Claims.FirstOrDefault(g => g.Type == ClaimTypes.Name)!.Value
            };
            return model;
        }
        public static string GenerateJwtToken(AuthorizeuserModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Appsettings.JWTSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                         new Claim(ClaimTypes.Role, "user"),
                         new Claim(ClaimTypes.NameIdentifier, model.UserId.ToString()),
                         new Claim(ClaimTypes.Name, model.Name),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static AuthorizeuserModel ValidateJwtToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            token = token.Replace("Bearer ", "");
            var returnDetails = new AuthorizeuserModel();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Appsettings.JWTSecretKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                returnDetails.Claimes = jwtToken.Claims;
                var userid = jwtToken.Claims.
                    FirstOrDefault(g => g.Type == ClaimTypes.NameIdentifier).Value;
                returnDetails.UserId = Guid.Parse(userid);
                returnDetails.Name = jwtToken.Claims.FirstOrDefault(g => g.Type == ClaimTypes.Name).Value;
                return returnDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
    public class AuthorizeuserModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public IEnumerable<Claim> Claimes { get; set; }
    }


}
