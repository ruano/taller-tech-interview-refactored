using Challenge.WebApi.Dtos;
using Confluent.Kafka;

namespace Challenge.WebApi.Kafka;

public class KafkaProduder : IKafkaProduder
{
    private readonly IProducer<Null, UserEventDto> _producer;
    private readonly ILogger<KafkaProduder> _logger;

    public KafkaProduder(IConfiguration configuration, ILogger<KafkaProduder> logger)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, UserEventDto>(config).Build();
        _logger = logger;
    }

    public async Task SendAsync(string topic, UserEventDto userEventDto)
    {
        _logger.LogInformation("Producing message to topic {Topic}: {@UserEventDto}", topic, userEventDto);

        await _producer.ProduceAsync(topic, new Message<Null, UserEventDto> { Value = userEventDto });
    }
}
