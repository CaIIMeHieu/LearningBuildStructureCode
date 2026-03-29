using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Application.Mapper;
using AutoMapper;

namespace Application.DependencyInjection.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(
            conf => conf.RegisterServicesFromAssembly(AssemblyReference.Assembly)
        ).AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
         .AddTransient(typeof(IPipelineBehavior<,>),typeof(TransactionPipelineBehavior<,>))
         .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformancePipelineBehavior<,>))
        .AddValidatorsFromAssembly(AssemblyReference.Assembly);

        return services;
    }

    public static IServiceCollection AddConfigurationAutoMapper( this IServiceCollection services )
    {
        services.AddAutoMapper( typeof(ServiceProfile) );
        return services;
    }
}

