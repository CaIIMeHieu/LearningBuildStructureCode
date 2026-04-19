using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DependencyInjection.Options;
using Application.Interface;
using Infrastructure.Authentication;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Serilog;
using StackExchange.Redis;

namespace Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var jwtSettings = configuration
            .GetSection(nameof(JwtSettings))
            .Get<JwtSettings>()!;

        services.Configure<JwtSettings>(
            configuration.GetSection(nameof(JwtSettings)));

        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero // Không cho phép trễ giờ
                };
            });

        return services;
    }

    public static IServiceCollection AddRedisConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConfiguration = new RedisOptions();
        configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);
        services.AddSingleton(redisConfiguration);
        if (!redisConfiguration.Enable)
            return services;

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));
        services.AddStackExchangeRedisCache(option => option.Configuration = redisConfiguration.ConnectionString);
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }

    //public static IServiceCollection AddResilienceConfiguration(
    //this IServiceCollection services)
    //{
    //    // Định nghĩa 1 pipeline dùng chung
    //    services.AddResiliencePipeline("global-pipeline", builder =>
    //    {
    //        // 1. Timeout — mỗi request không quá 10s
    //        builder.AddTimeout(TimeSpan.FromSeconds(10));

    //        // 2. Retry — thử lại 3 lần với exponential backoff
    //        builder.AddRetry(new RetryStrategyOptions
    //        {
    //            MaxRetryAttempts = 3,
    //            Delay = TimeSpan.FromSeconds(2),
    //            BackoffType = DelayBackoffType.Exponential,
    //            OnRetry = args =>
    //            {
    //                Log.Warning("Retry {Attempt} after {Delay}s — {Exception}",
    //                    args.AttemptNumber,
    //                    args.RetryDelay.TotalSeconds,
    //                    args.Outcome.Exception?.Message);
    //                return ValueTask.CompletedTask;
    //            }
    //        });

    //        // 3. Circuit Breaker — 5 fails → open 30s
    //        builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
    //        {
    //            FailureRatio = 0.5,           // 50% request fail → open
    //            MinimumThroughput = 5,        // Cần ít nhất 5 request để tính
    //            BreakDuration = TimeSpan.FromSeconds(30),
    //            OnOpened = args =>
    //            {
    //                Log.Warning("Circuit OPEN — {Exception}", args.Outcome.Exception?.Message);
    //                return ValueTask.CompletedTask;
    //            },
    //            OnClosed = args =>
    //            {
    //                Log.Information("Circuit CLOSED — service recovered");
    //                return ValueTask.CompletedTask;
    //            }
    //        });
    //    });

    //    return services;
    //}
}
