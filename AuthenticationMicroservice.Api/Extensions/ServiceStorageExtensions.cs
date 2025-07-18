using AuthenticationMicroservice.Business.Service.Abstractions;
using AuthenticationMicroservice.Business.Service.Concrete;
using AuthenticationMicroservice.Domain.Abstractions.Repositories;
using AuthenticationMicroservice.DataAccess.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using AuthenticationMicroservice.Api.Validators;
using Npgsql;
using AuthenticationMicroservice.Infrastructure.Services.Concrete;
using AuthenticationMicroservice.Infrastructure.Services.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace AuthenticationMicroservice.Api.Extensions;

public static class ServiceStorageExtensions
{
    public static void AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddTransient<NpgsqlConnection>(provider => new NpgsqlConnection(connectionString));
    }

    public static void AddRedis(this IServiceCollection services, string connectionString)
    {

    }

    public static void AddLogs(this IServiceCollection services, ConfigureHostBuilder host)
    {
        host.UseSerilog();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.GrafanaLoki("http://loki:3100", labels: new[]
            {
                new LokiLabel { Key = "app", Value = "auth-service" },
                new LokiLabel { Key = "env", Value = "dev" }
            })
            .CreateLogger();

        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("AuthenticationMicroservice"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddJaegerExporter(o =>
                    {
                        o.AgentHost = "jaeger";
                        o.AgentPort = 6831;
                    });
            });
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUsersService, UsersService>();
        services.AddSingleton<ITokenGenerator, TokenGenerator>();
    }

    public static void AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining(typeof(RegisterRequestValidator));
        services.AddValidatorsFromAssemblyContaining(typeof(LoginRequestValidator));
        services.AddValidatorsFromAssemblyContaining(typeof(LogoutRequestValidator));
        services.AddValidatorsFromAssemblyContaining(typeof(RefreshTokenRequestValidator));
    }

    public static void AddSwaggerAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme
            {
                Name = "Jwt Authentication",
                Description = "Type in a valid JWT Bearer",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "Jwt",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            }
                ;
            options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
        });

    }

    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                };
            });
    }
}