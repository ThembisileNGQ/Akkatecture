using Akkatecture.Aggregates;

namespace Akkatecture.Sagas
{
    public interface ISagaLocator
    {
        ISagaId LocateSaga(IDomainEvent domainEvent);
    }
}