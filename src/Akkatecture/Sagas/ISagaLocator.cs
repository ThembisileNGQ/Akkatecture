using Akkatecture.Aggregates;

namespace Akkatecture.Sagas
{
    public interface ISagaLocator
    {
        ISagaId LocateSagaAsync(IDomainEvent domainEvent);
    }
}