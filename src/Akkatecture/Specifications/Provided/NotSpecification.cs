using System;
using System.Collections.Generic;
using Akkatecture.Extensions;

namespace Akkatecture.Specifications.Provided
{
    public class NotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _specification;

        public NotSpecification(
            ISpecification<T> specification)
        {
            _specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }

        protected override IEnumerable<string> IsNotSatisfiedBecause(T account)
        {
            if (_specification.IsSatisfiedBy(account))
            {
                yield return $"Specification '{_specification.GetType().PrettyPrint()}' should not be satisfied";
            }
        }
    }
}