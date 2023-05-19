using Microsoft.AspNetCore.Authorization;
using MinimalAPI.Common;
using MinimalAPI.Models;
namespace MinimalAPI.Controllers;
public static class UserEndPoint
{
    public static void UserAPIs(this RouteGroupBuilder app)
    {
        app.MapPost("/security/getToken", [AllowAnonymous] ([Validate] UserDto user) =>
        {
            if (user.UserName == "admin@anil.com" && user.Password == "P@ssword")
            {
                var token = Utilities.GenerateJwtToken(new AuthorizeuserModel
                {
                    Name = user.UserName,
                    UserId = Guid.NewGuid()
                });
                return Results.Ok(new APIResponse
                {
                    Data = new
                    {
                        Token = token
                    },
                    Message = "logged in successfully"
                });
            }

            return Results.BadRequest(new APIResponse
            {
                Message = "email and password didn't match"
            });
        });
    }
}
