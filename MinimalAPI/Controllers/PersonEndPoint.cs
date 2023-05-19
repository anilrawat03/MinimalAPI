using MinimalAPI.Models;
using MinimalAPI.Common;
using MinimalAPI.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MinimalAPI.Controllers;
public static class PersonEndPoint
{
    public static void PersonsAPIs(this RouteGroupBuilder app)
    {
        app.MapGet("/product/testing/{id}/{name}", [AllowAnonymous] (HttpContext httpcontext, int id, string name) =>
        {
            var daa = httpcontext.LoggedUser().UserId;
            return Results.Ok(new APIResponse
            {
                Message = "Successfully"
            });
        }).WithTags("person");


        var consts = (IPersonManager _personmanager, HttpContext httpcontext,
             [Validate] PersonModel model) =>
          {
              var daa = httpcontext.LoggedUser().UserId;
              _personmanager.AddProduct(model);
              return Results.Ok(new APIResponse
              {
                  Message = "success",
                  Data = model
              });
          };

        app.MapPost("/person/addperson", (IPersonManager _personmanager, HttpContext httpcontext,
            [Validate] PersonModel model) =>
        {
            var daa = httpcontext.LoggedUser().UserId;
            _personmanager.AddProduct(model);
            return Results.Ok(new APIResponse
            {
                Message = "success",
                Data = model
            });
        })
          .WithTags("person");

        app.MapPost("/person/addperson2", consts)
            .RequireAuthorization("user")
            .WithTags("person");
            

    }
    public static async Task<IResult> AuthorizeAsync(HttpContext context)
    {
        await Task.CompletedTask;
        return TypedResults.Ok();
    }



}
