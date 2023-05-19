using MinimalAPI.Common;
using FluentValidation;
using MinimalAPI.Controllers;
using MinimalAPI.Manager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);
Appsettings.Configure(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token * *_only_ * *",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
{securityScheme, new string[] { }}
});
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
builder.Services.AddScoped<IPersonManager, PersonManager>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(Appsettings.JWTSecretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "admin",
        policy => policy.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, "admin")
    );
    options.AddPolicy(
        "user",
        policy => policy.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, "user")
    );
});

var app = builder.Build();
var root = app.MapGroup("/api/").AddEndpointFilter<AEndpointFilter>();
root.AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory);
root.InitAPIEndPoints();
//root.MapGet("anil", () => "hi");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.AddMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
