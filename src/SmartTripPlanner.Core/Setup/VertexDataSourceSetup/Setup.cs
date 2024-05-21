using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.Core.DataSource.Interfaces;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Setup.VertexDataSourceSetup;
public static class Setup
{
    public static ISmartTripPlannerConfigBuilder WithVertexDataSource<TInterface, TImplementation, TVertex, TVertexId>(
        this ISmartTripPlannerConfigBuilder builder,
        ServiceLifetime lifetime)
        where TVertexId : StronglyTypedVertexId
        where TVertex : IVertex<TVertexId>
        where TInterface : class, IVertexDataSource<TVertex, TVertexId>
        where TImplementation : class, TInterface
    {
        builder
            .Services
                .RegisterVertexDataSource<TInterface, TImplementation, TVertex, TVertexId>(lifetime);

        return builder;
    }

    private static IServiceCollection RegisterVertexDataSource<TInterface, TImplementation, TVertex, TVertexId>(
        this IServiceCollection services,
        ServiceLifetime lifetime)
        where TVertexId : StronglyTypedVertexId
        where TVertex : IVertex<TVertexId>
        where TInterface : class, IVertexDataSource<TVertex, TVertexId>
        where TImplementation : class, TInterface
        => lifetime switch
        {
            ServiceLifetime.Singleton => services
                                            .AddSingleton<TImplementation>()
                                            .AddSingleton<IVertexDataSource<TVertex, TVertexId>, TImplementation>()
                                            .AddSingleton<TInterface, TImplementation>(),
            ServiceLifetime.Scoped => services
                                            .AddScoped<TImplementation>()
                                            .AddScoped<IVertexDataSource<TVertex, TVertexId>, TImplementation>()
                                            .AddScoped<TInterface, TImplementation>(),
            ServiceLifetime.Transient => services
                                            .AddTransient<TImplementation>()
                                            .AddTransient<IVertexDataSource<TVertex, TVertexId>, TImplementation>()
                                            .AddTransient<TInterface, TImplementation>(),
            _ => services
        };
}
