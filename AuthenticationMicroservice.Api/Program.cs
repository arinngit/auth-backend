using System.Text;
using AuthenticationMicroservice.Api.Extensions;
using AuthenticationMicroservice.Infrastructure.Services.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using Prometheus;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

Env.Load(".env");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Database

string host = Env.GetString("HOST");
string port = Env.GetString("PORT");
string database = Env.GetString("DATABASE");
string username = Env.GetString("USERNAME");
string password = Env.GetString("PASSWORD");

string connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

builder.Services.AddDatabase(connectionString);

#endregion

#region Repositories

builder.Services.AddRepositories();

#endregion

#region Services

builder.Services.AddServices();

#endregion

#region Options

IConfigurationSection jwtOptions = builder.Configuration.GetSection("Jwt");
SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions["Key"]!));

const double AccessTokenLifeTime = 3;

builder.Services.Configure<JwtOptions>(options =>
{
    options.Issuer = jwtOptions["Issuer"]!;
    options.Audience = jwtOptions["Audience"]!;
    options.AccessValidFor = TimeSpan.FromHours(AccessTokenLifeTime);
    options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
});

#endregion



#region Authentication

builder.Services.AddAuthentication(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerAuth();
}

#endregion

#region Validation

builder.Services.AddValidation();

#endregion

#region Logs

builder.Services.AddLogs(builder.Host);

#endregion

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseRouting();
app.UseHttpMetrics();

app.MapMetrics();
app.MapControllers();

app.UseHttpsRedirection();
app.Run();
