using Akkatecture.Aggregates;

namespace Akkatecture.Sagas
{
    /*public interface ISagaLocator
    {
        ISagaId LocateSaga(IDomainEvent domainEvent);
    }*/

    public interface ISagaLocator<out TIdentity>
        where TIdentity : ISagaId
    {
        TIdentity LocateSaga(IDomainEvent domainEvent);
    }

}