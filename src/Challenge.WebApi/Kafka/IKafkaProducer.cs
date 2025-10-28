using Challenge.WebApi.Dtos;

namespace Challenge.WebApi.Kafka;
public interface IKafkaProducer
{
    Task SendAsync(string topic, UserEventDto userEventDto);
}