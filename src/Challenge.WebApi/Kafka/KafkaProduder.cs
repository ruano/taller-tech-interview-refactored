using Challenge.WebApi.Dtos;
using Confluent.Kafka;
using System.Text.Json;

namespace Challenge.WebApi.Kafka;

public class KafkaProduder : IKafkaProduder
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProduder> _logger;

    public KafkaProduder(IConfiguration configuration, ILogger<KafkaProduder> logger)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
        _logger = logger;
    }

    public async Task SendAsync(string topic, UserEventDto userEventDto)
    {
        _logger.LogInformation("Producing message to topic {Topic}: {@UserEventDto}", topic, userEventDto);

        string serialized = JsonSerializer.Serialize(userEventDto);

        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = serialized });
    }
}
