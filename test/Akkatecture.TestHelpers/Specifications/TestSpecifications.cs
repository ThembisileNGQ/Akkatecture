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

            protected override IEnumerable<string> IsNotSatisfiedBecause(int obj)
            {
                if (obj <= _limit)
                {
                    yield return $"{obj} is less or equal than {_limit}";
                }
            }
        }

        public class IsTrueSpecification : Specification<bool>
        {
            protected override IEnumerable<string> IsNotSatisfiedBecause(bool obj)
            {
                if (!obj)
                {
                    yield return "Its false!";
                }
            }
        }
    }
}