using System.Reflection;
using System.Text.Json.Serialization;
using Claims.Application.Auditing;
using Claims.Application.Covers.Rates;
using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Infrastructure;
using Claims.Model;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);


RegisterCosmosServices(builder.Configuration.GetSection("CosmosDb"), builder.Services).GetAwaiter().GetResult();

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuditerService, Auditer>();
builder.Services.AddScoped<IRateService, RateService>();
RegisterHandlers(builder.Services);
RegisterMasstransit(builder.Services);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.EnableAnnotations());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();
return;

void RegisterMasstransit(IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<AuditConsumer>();
        x.UsingInMemory((context, cfg) =>
        {
            cfg.ConcurrentMessageLimit = 100;
            cfg.ConfigureEndpoints(context);
        });
    });
}

void RegisterHandlers(IServiceCollection services)
{
    var assembly = Assembly.GetExecutingAssembly();

    foreach (var type in assembly.GetTypes())
    {
        if (type is { IsClass: true, IsAbstract: false })
        {
            foreach (var i in type.GetInterfaces())
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                {
                    services.AddScoped(i, type);
                }

                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                {
                    services.AddScoped(i, type);
                }
            }
        }
    }
}

static async Task RegisterCosmosServices(IConfigurationSection configurationSection,
    IServiceCollection services)
{
    var databaseName = configurationSection.GetSection("DatabaseName").Value;
    var claimsContainer = configurationSection.GetSection("ClaimContainerName").Value;
    var coversContainer = configurationSection.GetSection("CoverContainerName").Value;
    var account = configurationSection.GetSection("Account").Value;
    var key = configurationSection.GetSection("Key").Value;
    var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);

    services.AddScoped<IClaimsService>(_ => new ClaimsService(client.GetContainer(databaseName, claimsContainer)));
    services.AddScoped<ICoversService>(_ => new CoversService(client.GetContainer(databaseName, coversContainer)));

    var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(claimsContainer, "/id");
    await database.Database.CreateContainerIfNotExistsAsync(coversContainer, "/id");
}

public partial class Program
{
}