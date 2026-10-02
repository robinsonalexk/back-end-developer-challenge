using CharManagerAPI.Repositories;
using CharManagerAPI.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Conventions;
using CharacterManagerAPI.Hubs;
using System.Text.Json.Serialization;

var conventionPack = new ConventionPack
{
    new CamelCaseElementNameConvention()
};

ConventionRegistry.Register(
    "camelCase",
    conventionPack,
    type => true
);

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoDb"));

var allowedOrigin = builder.Configuration["Cors:AllowedOrigin"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("ui", policy =>
    {
        policy
            .WithOrigins(allowedOrigin!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(sp.GetRequiredService<IOptions<MongoSettings>>().Value.ConnectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(sp.GetRequiredService<IOptions<MongoSettings>>().Value.Database));

builder.Services.AddSingleton<ICharacterRepository, CharacterRepository>();
builder.Services.AddSingleton<ICharacterService, CharacterService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    }
);

builder.Services.AddSignalR()
    .AddJsonProtocol(o =>
        o.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.MapHub<CharacterHub>("/hubs/characters");

app.UseCors("ui");

app.Run();