using Challenge.WebApi;
using Challenge.WebApi.Dtos;
using Challenge.WebApi.Kafka;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IKafkaProduder, KafkaProduder>();
builder.Services.AddSingleton<EventStore>();
builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/events", (IKafkaProduder KafkaProduder, IConfiguration configuration, [FromBody] UserEventDto dto) =>
{
    KafkaProduder.SendAsync(configuration["Kafka:Topic"]!, dto);

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