using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NzbDrone.Core.MediaFiles;
using RadarrPusherApi.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IConfiguration configuration = builder.Configuration;

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    AutofacConfig.Configure(configuration, containerBuilder);

});

if (builder.Environment.IsDevelopment())
{
    //added this to stop Swagger from crashing when trying to build the json. The dependency FFMpegCore in Radarr.Core was not being referenced for some reason.
    builder.Services.AddSwaggerGen(options =>
    {
        options.MapType(typeof(MovieFile), () => new OpenApiSchema());
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
