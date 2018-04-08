using Akkatecture.Core;

namespace Akkatecture.Sagas
{
    public interface ISaga
    {
        
    }

    public interface ISaga<TSagaId, TSagaState> : ISaga
        where TSagaState : ISagaState<TSagaId>
        where TSagaId : ISagaId
    {
        TSagaId Id { get; }
        string Name { get; }
    }
}