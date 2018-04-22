using System;
using System.Collections.Generic;
using System.Linq;

namespace Akkatecture.Specifications.Provided
{
    public class OrSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _specification1;
        private readonly ISpecification<T> _specification2;

        public OrSpecification(
            ISpecification<T> specification1,
            ISpecification<T> specification2)
        {
            _specification1 = specification1 ?? throw new ArgumentNullException(nameof(specification1));
            _specification2 = specification2 ?? throw new ArgumentNullException(nameof(specification2));
        }

        protected override IEnumerable<string> IsNotSatisfiedBecause(T obj)
        {
            var reasons1 = _specification1.WhyIsNotSatisfiedBy(obj).ToList();
            var reasons2 = _specification2.WhyIsNotSatisfiedBy(obj).ToList();

            if (!reasons1.Any() || !reasons2.Any())
            {
                return Enumerable.Empty<string>();
            }

            return reasons1.Concat(reasons2);
        }
    }
}