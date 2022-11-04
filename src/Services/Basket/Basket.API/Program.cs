using System.Reflection;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using MassTransit;
using static Discount.Grpc.Protos.DiscountProtoService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Redis Cache Configuration
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration["CacheSettings:ConnectionString"];
});

// General Configuration
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(Program));

// GRPC Configuration
builder.Services.AddGrpcClient<DiscountProtoServiceClient>(
    options => options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
builder.Services.AddScoped<DiscountGrpcService>();

// MassTransit - RabbitMQ Configuration
builder.Services.AddMassTransit(config => {
    config.UsingRabbitMq((ctx, configurator) => {
        configurator.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});

builder.Services.AddControllers();
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

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();;

app.Run();
