using System.Collections.Generic;
using Akkatecture.Aggregates;

namespace Akkatecture.Specifications.Provided
{
    public class AggregateIsNewSpecification : Specification<IAggregateRoot>
    {
        protected override IEnumerable<string> IsNotSatisfiedBecause(IAggregateRoot obj)
        {
            if (!obj.IsNew)
            {
                yield return $"'{obj.Name}' with ID '{obj.GetIdentity()}' is not new";
            }
        }
    }
}