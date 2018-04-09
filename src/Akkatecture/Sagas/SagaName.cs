using Akkatecture.ValueObjects;

namespace Akkatecture.Sagas
{
    public class SagaName : SingleValueObject<string>, ISagaName
    {
        public SagaName(string value) : base(value)
        {
        }
    }
}