using Akkatecture.Aggregates;

namespace Akkatecture.Sagas
{
    //might not be needed
    public interface ISagaLocator
    {
        ISagaId LocateSagaAsync(IDomainEvent domainEvent);
    }
}