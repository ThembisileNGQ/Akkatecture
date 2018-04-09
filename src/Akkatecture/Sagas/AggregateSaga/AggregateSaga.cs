namespace Akkatecture.Sagas.AggregateSaga
{
    public class AggregateSaga<TSaga, TIdentity, TSagaState> : Saga<TIdentity, TSagaState>
        where TSaga : AggregateSaga<TSaga, TIdentity,TSagaState>
        where TIdentity : ISagaId
        where TSagaState : ISagaState<TIdentity>
    {
        
    }
}