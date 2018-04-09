using Akka.Persistence;

namespace Akkatecture.Sagas
{
    public abstract class Saga<TIdentity, TSagaState> : ReceivePersistentActor, ISaga<TIdentity, TSagaState>
        where TIdentity : ISagaId
        where TSagaState : ISagaState<TIdentity>
    {
        public override string PersistenceId { get; } = Context.Self.Path.Name;
    }
}
 