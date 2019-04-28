using Akka.Persistence.Journal;

namespace Akkatecture.Events
{
    public class DomainEventReadAdapter : IReadEventAdapter
    {
        public IEventSequence FromJournal(object evt, string manifest)
        {
            var newEvent = DomainEventMapper.FromCommittedEvent(evt);
            
            return new SingleEventSequence(newEvent);
        }
    }
}