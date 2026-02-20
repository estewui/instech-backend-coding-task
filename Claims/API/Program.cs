using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading.Channels;

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

using API.HostedServices;
using Application.Abstractions.Persistence;
using Application.Claims.CreateClaim;
using Application.Common.Auditing;
using Application.Covers.CreateCover;
using Application.Services;
using Infrastructure.Auditing;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Mongo.Repositories;
using Infrastructure.Persistence.Sql.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Start Testcontainers for SQL Server and MongoDB
var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        : new()

    ).Build();

var mongoContainer = new MongoDbBuilder()
    .WithImage("mongo:latest")
    .Build();

await sqlContainer.StartAsync();
await mongoContainer.StartAsync();

// Channel for in-memory audit buffering
var auditChannel = Channel.CreateBounded<AuditEvent>(new BoundedChannelOptions(10_000)
{
    SingleReader = true,
    SingleWriter = false,
    FullMode = BoundedChannelFullMode.Wait
});


builder.Services.AddSingleton(auditChannel);

builder.Services.AddSingleton<IAuditSink, ChannelAuditSink>();

// Register background worker
builder.Services.AddHostedService<AuditBackgroundService>();

builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ClaimRepository).Assembly);

// Add services to the container.
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<ICoverRepository, CoverRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<ICoverService, CoverService>();
builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateClaimValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCoverValidator>();

builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(sqlContainer.GetConnectionString()));

builder.Services.AddDbContext<ClaimsContext>(options =>
{
    var client = new MongoClient(mongoContainer.GetConnectionString());
    var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]); // Use a default/test database name
    options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

public partial class Program { }