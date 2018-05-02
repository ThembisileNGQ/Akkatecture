using System.Collections.Generic;
using Akkatecture.Aggregates;

namespace Akkatecture.Specifications.Provided
{
    public class AggregateIsNewSpecification : Specification<IAggregateRoot>
    {
        protected override IEnumerable<string> IsNotSatisfiedBecause(IAggregateRoot account)
        {
            if (!account.IsNew)
            {
                yield return $"'{account.Name}' with ID '{account.GetIdentity()}' is not new";
            }
        }
    }
}