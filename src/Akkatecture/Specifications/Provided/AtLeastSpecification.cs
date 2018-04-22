using System;
using System.Collections.Generic;
using System.Linq;
using Akkatecture.Extensions;

namespace Akkatecture.Specifications.Provided
{
    public class AtLeastSpecification<T> : Specification<T>
    {
        private readonly int _requiredSpecifications;
        private readonly IReadOnlyList<ISpecification<T>> _specifications;

        public AtLeastSpecification(
            int requiredSpecifications,
            IEnumerable<ISpecification<T>> specifications)
        {
            var specificationList = (specifications ?? Enumerable.Empty<ISpecification<T>>()).ToList();

            if (requiredSpecifications <= 0)
                throw new ArgumentOutOfRangeException(nameof(requiredSpecifications));
            if (!specificationList.Any())
                throw new ArgumentException("Please provide some specifications", nameof(specifications));
            if (requiredSpecifications > specificationList.Count)
                throw new ArgumentOutOfRangeException($"You required '{requiredSpecifications}' to be met, but only '{specificationList.Count}' was supplied");

            _requiredSpecifications = requiredSpecifications;
            _specifications = specificationList;
        }

        protected override IEnumerable<string> IsNotSatisfiedBecause(T obj)
        {
            var notStatisfiedReasons = _specifications
                .Select(s => new
                {
                    Specification = s,
                    WhyIsNotStatisfied = s.WhyIsNotSatisfiedBy(obj).ToList()
                })
                .Where(a => a.WhyIsNotStatisfied.Any())
                .Select(a => $"{a.Specification.GetType().PrettyPrint()}: {string.Join(", ", a.WhyIsNotStatisfied)}")
                .ToList();

            return (_specifications.Count - notStatisfiedReasons.Count) >= _requiredSpecifications
                ? Enumerable.Empty<string>()
                : notStatisfiedReasons;
        }
    }
}