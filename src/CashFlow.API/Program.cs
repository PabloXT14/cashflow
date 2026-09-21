using System.Text;
using CashFlow.API.Filters;
using CashFlow.API.Middlewares;
using CashFlow.Application;
using CashFlow.Infrastructure;
using CashFlow.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ExceptionFilter));
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// JWT Authentication configuration

var signingKey = builder.Configuration.GetValue<string>("Settings:Jwt:SigningKey");

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(config =>
{
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // If true, add ou app name (a string) to the "iss" claim in the token, which is optional. 
        ValidateAudience = false, // If true, add the audience (a string of the others app that will can consume the token) to the "aud" claim in the token, which is optional.
                                  // (both issue and audience are optional, and if true the jwt token will check them on the authentication process)

        ClockSkew = new TimeSpan(0), // To prevent errors on check the token expiration time, we set the clock skew to zero. By default, the clock skew is 5 minutes, which means that if the token expires in 5 minutes, it will still be considered valid for an additional 5 minutes. Setting it to zero ensures that the token is considered expired immediately after its expiration time.


        // Our security key (as configured in the infrastructure project)
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware to handle culture based on Accept-Language header
app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await MigrateDatabase();

app.Run();

async Task MigrateDatabase()
{
    // Create a scope to get the required services for database migration and work the dependency injection (obs: in api controller/request context the scope is created automatically, but here we are outside of the request context, so we need to create a scope manually)
    await using var scope = app.Services.CreateAsyncScope();

    await DatabaseMigration.MigrateDatabase(scope.ServiceProvider);
}