using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.Setup.GraphSetup;
public static class Setup
{
    public static IGraphImplementationConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge> For
        <TGraphInterface, TGraphImplementation, TVertex, TVertexId, TEdge>(
        this ISmartTripPlannerConfigBuilder smartTripPlannerConfigBuilder)
        where TVertex : IVertex<TVertexId>
        where TVertexId : StronglyTypedVertexId
        where TEdge : Edge<TVertex, TVertexId>
        where TGraphInterface : class, IGraph<TVertex, TVertexId, TEdge>
        where TGraphImplementation : class, TGraphInterface
    {
        smartTripPlannerConfigBuilder
            .Services
                .AddSingleton<TGraphInterface, TGraphImplementation>();

        return new GraphImplementationConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge>(smartTripPlannerConfigBuilder.Services);
    }

    public static IGraphDecoratorConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge> DecorateWith
        <TGraphInterface, TGraphImplementation, TVertex, TVertexId, TEdge>(
        this IGraphImplementationConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge> graphImplementationConfigBuilder,
        Func<TGraphInterface, IServiceProvider, TGraphImplementation> registerDecoreeFunc)
        where TVertex: IVertex<TVertexId>
        where TVertexId : StronglyTypedVertexId
        where TEdge : Edge<TVertex, TVertexId>
        where TGraphInterface : class, IGraph<TVertex, TVertexId, TEdge>
        where TGraphImplementation : class, TGraphInterface
    {
        graphImplementationConfigBuilder
            .Services
            .Decorate<TGraphInterface>((decoree, sp) => registerDecoreeFunc(decoree, sp));

        return new GraphDecoratorConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge>(graphImplementationConfigBuilder.Services);
    }

    public static IGraphDecoratorConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge> DecorateWith
        <TGraphInterface, TGraphImplementation, TVertex, TVertexId, TEdge>(
        this IGraphDecoratorConfigBuilder<TGraphInterface, TVertex, TVertexId, TEdge> graphDecoratorConfigBuilder,
        Func<TGraphInterface, IServiceProvider, TGraphImplementation> registerDecoreeFunc)
        where TVertex : IVertex<TVertexId>
        where TVertexId : StronglyTypedVertexId
        where TEdge : Edge<TVertex, TVertexId>
        where TGraphInterface : class, IGraph<TVertex, TVertexId, TEdge>
        where TGraphImplementation : class, TGraphInterface
    {
        graphDecoratorConfigBuilder
            .Services
            .Decorate<TGraphInterface>((decoree, sp) => registerDecoreeFunc(decoree, sp));

        return graphDecoratorConfigBuilder;
    }
}
