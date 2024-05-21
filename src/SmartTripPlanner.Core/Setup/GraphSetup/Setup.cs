using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Setup.GraphSetup;
public static class Setup
{
    public static IGraphImplementationConfigBuilder<TGraphInterface, TVertexId, TEdge> For
        <TGraphInterface, TGraphImplementation, TVertexId, TEdge>(
        this ISmartTripPlannerConfigBuilder smartTripPlannerConfigBuilder)
        where TVertexId : StronglyTypedVertexId
        where TEdge : Edge
        where TGraphInterface : class, IGraph<TVertexId, TEdge>
        where TGraphImplementation : class, TGraphInterface
    {
        smartTripPlannerConfigBuilder
            .Services
                .AddSingleton<TGraphInterface, TGraphImplementation>();

        return new GraphImplementationConfigBuilder<TGraphInterface, TVertexId, TEdge>(smartTripPlannerConfigBuilder.Services);
    }

    public static IGraphDecoratorConfigBuilder<TGraphInterface, TVertexId, TEdge> DecorateWith
        <TGraphInterface, TGraphImplementation, TVertexId, TEdge>(
        this IGraphImplementationConfigBuilder<TGraphInterface, TVertexId, TEdge> graphImplementationConfigBuilder,
        Func<TGraphInterface, IServiceProvider, TGraphImplementation> registerDecoreeFunc)
        where TVertexId : StronglyTypedVertexId
        where TEdge : Edge
        where TGraphInterface : class, IGraph<TVertexId, TEdge>
        where TGraphImplementation : class, TGraphInterface
    {
        graphImplementationConfigBuilder
            .Services
            .Decorate<TGraphInterface>((decoree, sp) => registerDecoreeFunc(decoree, sp));

        return new GraphDecoratorConfigBuilder<TGraphInterface, TVertexId, TEdge>(graphImplementationConfigBuilder.Services);
    }

    public static IGraphDecoratorConfigBuilder<TGraphInterface, TVertexId, TEdge> DecorateWith
        <TGraphInterface, TGraphImplementation, TVertexId, TEdge>(
        this IGraphDecoratorConfigBuilder<TGraphInterface, TVertexId, TEdge> graphDecoratorConfigBuilder,
        Func<TGraphInterface, IServiceProvider, TGraphImplementation> registerDecoreeFunc)
        where TVertexId : StronglyTypedVertexId
        where TEdge : Edge
        where TGraphInterface : class, IGraph<TVertexId, TEdge>
        where TGraphImplementation : class, TGraphInterface
    {
        graphDecoratorConfigBuilder
            .Services
            .Decorate<TGraphInterface>((decoree, sp) => registerDecoreeFunc(decoree, sp));

        return graphDecoratorConfigBuilder;
    }
}
