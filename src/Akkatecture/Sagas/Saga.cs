using Akka.Actor;
using Akka.Persistence;

namespace Akkatecture.Sagas
{
    public class Saga : ReceivePersistentActor
    {
        public override string PersistenceId { get; } = Context.Self.Path.Name;
    }
}
 