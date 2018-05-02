using System.Collections.Generic;
using Akkatecture.Specifications;

namespace Akkatecture.TestHelpers.Specifications
{
    public static class TestSpecifications
    {
        public class IsAboveSpecification : Specification<int>
        {
            private readonly int _limit;

            public IsAboveSpecification(
                int limit)
            {
                _limit = limit;
            }

            protected override IEnumerable<string> IsNotSatisfiedBecause(int account)
            {
                if (account <= _limit)
                {
                    yield return $"{account} is less or equal than {_limit}";
                }
            }
        }

        public class IsTrueSpecification : Specification<bool>
        {
            protected override IEnumerable<string> IsNotSatisfiedBecause(bool account)
            {
                if (!account)
                {
                    yield return "Its false!";
                }
            }
        }
    }
}