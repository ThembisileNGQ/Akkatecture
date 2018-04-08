using Akkatecture.Aggregates;
using Akkatecture.ValueObjects;

namespace Akkatecture.Sagas
{
    public class SagaName : SingleValueObject<string>, IAggregateName
    {
        public SagaName(string value) : base(value)
        {
        }
    }
}