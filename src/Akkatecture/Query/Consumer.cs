using System.Linq;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Akkatecture.Aggregates;
using Akkatecture.Events;
using Akkatecture.Extensions;

namespace Akkatecture.Query
{
    public class Consumer
    {
        public ActorSystem ActorSystem { get; set; }
        protected string Name { get; set; }
        internal Consumer(
            string name,
            ActorSystem actorSystem)
        {
            ActorSystem = actorSystem;
            Name = name;
        }

        private Consumer(
            string name,
            Config config)
        {
            var actorSystem = ActorSystem.Create(name, config);
            ActorSystem = actorSystem;
            Name = name;
        }

        public static Consumer Create(string name, Config config)
        {
            return new Consumer(name,config);
        }
        
        public static Consumer Create(ActorSystem actorSystem)
        {
            return new Consumer(actorSystem.Name, actorSystem);
        }

        public Consumer<TJournal> Using<TJournal>(
            string readJournalPluginId = null)
            where TJournal : IEventsByTagQuery, ICurrentEventsByTagQuery
        {
            var readJournal = PersistenceQuery
                .Get(ActorSystem)
                .ReadJournalFor<TJournal>(readJournalPluginId);
            
            return new Consumer<TJournal>(Name, ActorSystem, readJournal);
        }
    }
    
    public class Consumer<TJournal> : Consumer
        where TJournal : IEventsByTagQuery, ICurrentEventsByTagQuery
    {
        protected TJournal Journal { get; }
        
        public Consumer(
            string name,
            ActorSystem actorSystem,
            TJournal journal) 
            : base(name, actorSystem)
        {
            Journal = journal;
        }

        public Source<EventEnvelope, NotUsed> EventsFromAggregate<TAggregate>(Offset offset = null)
            where TAggregate : IAggregateRoot
        {
            var mapper = new DomainEventReadAdapter();
            var aggregateName = typeof(TAggregate).GetAggregateName();
            
            return Journal
                .EventsByTag(aggregateName.Value, offset)
                .Select(x =>
                {
                    var domainEvent = mapper.FromJournal(x.Event, string.Empty).Events.Single();
                    return new EventEnvelope(x.Offset, x.PersistenceId, x.SequenceNr, domainEvent);
                });
        }
        
        public Source<EventEnvelope, NotUsed> CurrentEventsFromAggregate<TAggregate>(Offset offset = null)
            where TAggregate : IAggregateRoot
        {
            var mapper = new DomainEventReadAdapter();
            var aggregateName = typeof(TAggregate).GetAggregateName();
            
            return Journal
                .EventsByTag(aggregateName.Value, offset)
                .Select(x =>
                {
                    var domainEvent = mapper.FromJournal(x.Event, string.Empty).Events.Single();
                    return new EventEnvelope(x.Offset, x.PersistenceId, x.SequenceNr, domainEvent);
                });
        }
    }
    
    
}