using System.Configuration;
using System.Reflection;
using System.Text.Json.Serialization;
using Claims.Application;
using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Auditing;
using Claims.Controllers;
using Claims.Infrastructure;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

builder.Services.AddSingleton(
    InitializeCosmosClientInstanceAsync(builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

RegisterHandlers(builder.Services);

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

static async Task<IClaimsService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
{
    var databaseName = configurationSection.GetSection("DatabaseName").Value;
    var containerName = configurationSection.GetSection("ContainerName").Value;
    var account = configurationSection.GetSection("Account").Value;
    var key = configurationSection.GetSection("Key").Value;
    var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
    var cosmosContainer = client.GetContainer(databaseName, containerName);
    var cosmosDbService = new ClaimsService(cosmosContainer);
    var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
    return cosmosDbService;
}

public partial class Program
{
}