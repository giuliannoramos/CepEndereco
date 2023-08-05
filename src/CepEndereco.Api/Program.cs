using CepEndereco.Api.Interfaces;
using CepEndereco.Api.Services;
using StackExchange.Redis;
using MassTransit;
using System.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IEnderecoService, EnderecoService>();
builder.Services.AddScoped<IViacepService, ViacepService>();
builder.Services.AddScoped<IRedisService, RedisService>();

// Add HttpClient
builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro do IConnectionMultiplexer usando provedor de serviços personalizado
builder.Services.AddSingleton(sp =>
{
    var redisConnectionString = "redis-15590.c308.sa-east-1-1.ec2.cloud.redislabs.com:15590,password=xE0zSuE4M9C2fBo1bWps1Ikzxa6lwwAc";
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
    {
        busFactoryConfigurator.Host("jackal-01.rmq.cloudamqp.com", 5671, "smxidzbm", h =>
        {
            h.Username("smxidzbm");
            h.Password("r0NqiOSMhkhEriskosEUM6wHIE9SmIdE");

            h.UseSsl(s =>
            {
                s.Protocol = SslProtocols.Tls12;
            });
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();