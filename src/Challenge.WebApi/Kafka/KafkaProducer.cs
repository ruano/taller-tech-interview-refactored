using Challenge.WebApi.Dtos;
using Confluent.Kafka;
using System.Text.Json;

namespace Challenge.WebApi.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
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
        try
        {
            _logger.LogInformation("Producing message to topic {Topic}: {@UserEventDto}", topic, userEventDto);

            var serialized = JsonSerializer.Serialize(userEventDto);

            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = serialized });
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, "Delivery failed: {Error}", ex.Error.Reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing message: {Reason}", ex.Message);
        }
    }
}
