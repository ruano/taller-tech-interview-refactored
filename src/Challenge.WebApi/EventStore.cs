using Challenge.WebApi.Dtos;
using System.Collections.Concurrent;

namespace Challenge.WebApi
{
    public class EventStore
    {
        private readonly ConcurrentBag<UserEventDto> _events;

        public EventStore()
        {
            _events = [];
        }

        public IEnumerable<UserEventDto> GetAll() 
            => _events.AsEnumerable();

        public void Add(UserEventDto userEventDto) =>
            _events.Add(userEventDto);
    }
}
