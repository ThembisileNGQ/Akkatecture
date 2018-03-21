using Akkatecture.ValueObjects;

namespace Akkatecture.Aggregates
{
    public class AggregateName : SingleValueObject<string>, IAggregateName
    {
        public AggregateName(string value) : base(value)
        {
        }
    }
}