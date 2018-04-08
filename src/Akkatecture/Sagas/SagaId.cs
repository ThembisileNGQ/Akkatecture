using Akkatecture.ValueObjects;

namespace Akkatecture.Sagas
{
    public abstract class SagaId<T> : SingleValueObject<string>, ISagaId
        where T : SagaId<T>
    {
        protected SagaId(string value) 
            : base(value)
        {
        }
    }
}