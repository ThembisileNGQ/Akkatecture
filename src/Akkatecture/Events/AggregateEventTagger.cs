using Akka.Persistence.Journal;
using Akkatecture.Aggregates;

namespace Akkatecture.Events
{
    public class AggregateEventTagger : IWriteEventAdapter
    {
        public string Manifest(object evt) => string.Empty;

        public object ToJournal(object evt)
        {
            if (evt is IAggregateEvent)
            {
                return new Tagged(evt, new[] {"AggregateEvent"});
            }

            return new Tagged(evt, new[] {"OtherEvent"});
        }
    }
}