using Challenge.WebApi.Dtos;

namespace Challenge.WebApi.Kafka;
public interface IKafkaProduder
{
    Task SendAsync(string topic, UserEventDto userEventDto);
}