using Challenge.WebApi;
using Challenge.WebApi.Dtos;
using Challenge.WebApi.Kafka;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddSingleton<EventStore>();
builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/events", async (IKafkaProducer kafkaProduder, IConfiguration configuration, [FromBody] UserEventDto dto) =>
{
    await kafkaProduder.SendAsync(configuration["Kafka:Topic"]!, dto);

    return Results.Accepted();
})
.WithName("Creates an event")
.WithOpenApi();

app.MapGet("/events", (EventStore eventStore) =>
{
    var events = eventStore.GetAll();
    return Results.Ok(events);
})
.WithName("Get all created events")
.WithOpenApi();

app.Run();