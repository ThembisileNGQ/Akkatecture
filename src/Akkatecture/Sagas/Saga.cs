using Akka.Persistence;

namespace Akkatecture.Sagas
{
    public abstract class Saga<TSagaId, TSagaState> : ReceivePersistentActor, ISaga<TSagaId, TSagaState>
        where TSagaId : ISagaId
        where TSagaState : ISagaState<TSagaId>
    {
        public override string PersistenceId { get; } = Context.Self.Path.Name;
        public TSagaId Id { get; } 
        public string Name { get; }
    }
}
 