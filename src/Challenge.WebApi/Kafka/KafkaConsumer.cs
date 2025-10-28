using Challenge.WebApi.Dtos;
using Confluent.Kafka;
using System.Text.Json;

namespace Challenge.WebApi.Kafka;

public class KafkaConsumer : BackgroundService
{
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

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(configuration["Kafka:Topic"]!);

        _eventStore = eventStore;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumerResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                if (consumerResult != null)
                {
                    _logger.LogInformation("Message consumed from topic {Topic}, partition {Partition}, offset {Offset}", consumerResult.Topic, consumerResult.Partition, consumerResult.Offset);

                    var userEventDto = JsonSerializer.Deserialize<UserEventDto>(consumerResult.Message.Value);

                    _eventStore.Add(userEventDto!);
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Error consuming message: {Reason}", ex.Error.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming message: {Reason}", ex.Message);
            }

            await Task.Delay(500, stoppingToken);
        }

        _consumer.Close();
        _logger.LogInformation("KafkaConsumer stopped");
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}