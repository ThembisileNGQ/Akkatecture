using Akka.Persistence.Journal;
using Akkatecture.Extensions;

namespace Akkatecture.Events
{
    public class AggregateEventTagger : IWriteEventAdapter
    {
        public string Manifest(object evt) => string.Empty;
        
        public object ToJournal(object evt)
        {
            try
            {
                var tag = evt
                    .GetType()
                    .GetCommittedEventAggregateRootName();

                return new Tagged(evt, new[] {tag.Value});
            }
            catch
            {
                return evt;
            }
        }
    }
}