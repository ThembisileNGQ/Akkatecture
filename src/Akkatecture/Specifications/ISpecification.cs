using System.Collections.Generic;

namespace Akkatecture.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T obj);

        IEnumerable<string> WhyIsNotSatisfiedBy(T obj);
    }
}