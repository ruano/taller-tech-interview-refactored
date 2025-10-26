using Challenge.WebApi.Dtos;
using Confluent.Kafka;
using System.Text.Json;

namespace Challenge.WebApi.Kafka;

public class KafkaConsumer : BackgroundService
{
    private readonly string _topic;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly EventStore _eventStore;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(IConfiguration configuration, EventStore eventStore, ILogger<KafkaConsumer> logger)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _topic = configuration["Kafka:Topic"]!;
        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _eventStore = eventStore;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = _consumer.Consume(stoppingToken);

                _logger.LogInformation("Message consumed from topic {Topic}, partition {Partition}, offset {Offset}", cr.Topic, cr.Partition, cr.Offset);

                var userEventDto = JsonSerializer.Deserialize<UserEventDto>(cr.Message.Value);

                _eventStore.Add(userEventDto!);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError("Error consuming message: {Reason}", ex.Error.Reason);
            }

            await Task.Delay(500, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}