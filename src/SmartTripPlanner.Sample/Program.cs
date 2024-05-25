using SmartTripPlanner.Sample.Endpoints;
using SmartTripPlanner.Sample.Services;
using SmartTripPlanner.Core.Setup;
using SmartTripPlanner.GoogleRoutes.Setup;
using SmartTripPlanner.ChargePoints.Graphs;
using SmartTripPlanner.ChargePoints.Models;
using SmartTripPlanner.Core.Cache;
using SmartTripPlanner.ChargePoints.Cache;
using SmartTripPlanner.ChargePoints.Setup;
using SmartTripPlanner.Core.Graph;
using SmartTripPlanner.Sample.Repositories;
using SmartTripPlanner.Core.Setup.VertexDataSourceSetup;
using SmartTripPlanner.Core.JsonConverters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services
        .AddServices(builder.Configuration)
        .AddRepositories();

builder
    .Services
        .Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(x =>
        {
            x.SerializerOptions.Converters.Add(new StronglyTypedVertexIdJsonConverter<ChargePointBarcode>());
        })
        .AddSmartTripPlanner(x => x.ForChargePoints()
                                   .WithVertexDataSource<IChargePointVertexDataSource, ChargePointVertexDataSource, ChargePoint, ChargePointBarcode>(ServiceLifetime.Singleton)
                                   .WithGoogleRoutes(builder.Configuration));

var app = builder.Build();

GoogleRoutesEndpoints.MapEndpoints(app);
ChargePointEndpoints.MapEndpoints(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var cpGraph = app.Services.GetRequiredService<IChargePointGraph>();

await cpGraph.EnsureInitializedAsync();

#pragma warning disable CA1849 // Call async methods when in an async method
#pragma warning disable S6966 // Awaitable method should be used
app.Run();
#pragma warning restore S6966 // Awaitable method should be used
#pragma warning restore CA1849 // Call async methods when in an async method
