using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using NzbDrone.Core.MediaFiles;
using RadarrPusherApi.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

IConfiguration configuration = builder.Configuration;

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    AutofacConfig.Configure(configuration, containerBuilder);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = $"https://{configuration.GetValue<string>("Auth0:Domain")}/";
    options.Audience = configuration.GetValue<string>("Auth0:Audience");
});

if (builder.Environment.IsDevelopment())
{
    //added this to stop Swagger from crashing when trying to build the json. The dependency FFMpegCore in Radarr.Core was not being referenced for some reason.
    builder.Services.AddSwaggerGen(options =>
    {
        options.MapType(typeof(MovieFile), () => new OpenApiSchema());

        var securitySchema = new OpenApiSecurityScheme
        {
            Description = "Using the Authorization header with the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        options.AddSecurityDefinition("Bearer", securitySchema);

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { securitySchema, new[] { "Bearer" } }
        });
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
