using Akkatecture.ValueObjects;

namespace Akkatecture.Sagas
{
    public abstract class SagaId<T> : SingleValueObject<string>, ISagaId
    {
        protected SagaId(string value) 
            : base(value)
        {
        }
    }
}