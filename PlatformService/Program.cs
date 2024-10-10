using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//Add services to the container.
if(builder.Environment.IsProduction())
{ 
    Console.WriteLine("--> Using SQL DB...");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("PSConn")));
}
else 
{
    Console.WriteLine("--> Using in mem  DB...");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
Console.WriteLine($"INFO --> CommandService Endpoint --> {config["CommandSrv"]}");

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

app.UseRouting();

app.UseAuthorization();

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.MapControllers();

app.Run();
