using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Sagas
{
    public interface ISagaLocator
    {
        ISagaId LocateSaga(IDomainEvent domainEvent);
    }
    
}