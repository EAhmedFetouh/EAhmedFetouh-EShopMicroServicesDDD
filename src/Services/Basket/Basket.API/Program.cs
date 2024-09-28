using BuildingBlocks.Behavior;
using BuildingBlocks.Exceptions.Handler;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
//builder.Services.AddScoped <IBasketRepository>(provider =>
//{
//    var basketRepo = provider.GetRequiredService<BasketRepository>();
//    return new CachedBasketRepository(basketRepo,provider.GetRequiredService<IDistributedCache>());
//});

builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();
app.UseExceptionHandler(option => { });
app.MapGet("/", () => "Hello World!");
app.MapCarter();
// configure the HTTP request pipeline
app.Run();
