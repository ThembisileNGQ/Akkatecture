using Akkatecture.Core;

namespace Akkatecture.Sagas
{
    public interface ISaga
    {
        
    }

    public interface ISaga<TSagaState, TSagaId> : ISaga
        where TSagaState : ISagaState<TSagaId>
        where TSagaId : ISagaId
    {
    }
}