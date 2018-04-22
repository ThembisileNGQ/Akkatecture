using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Akkatecture.Specifications;
using Akkatecture.Specifications.Provided;

namespace Akkatecture.Extensions
{
    public static class SpecificationExtensions
    {
        /*public static void ThrowDomainErrorIfNotSatisfied<T>(
            this ISpecification<T> specification,
            T obj)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            var whyIsNotStatisfiedBy = specification.WhyIsNotSatisfiedBy(obj).ToList();
            if (whyIsNotStatisfiedBy.Any())
            {
                throw DomainError.With(
                    $"'{specification.GetType().PrettyPrint()}' is not satisfied because of {string.Join(" and ", whyIsNotStatisfiedBy)}");
            }
        }

        public static IExecutionResult IsNotSatisfiedByAsExecutionResult<T>(
            this ISpecification<T> specification,
            T obj)
        {
            var whyIsNotStatisfiedBy = specification
                .WhyIsNotSatisfiedBy(obj)
                .ToList();
            
            return whyIsNotStatisfiedBy.Any()
                ? ExecutionResult.Failed(whyIsNotStatisfiedBy)
                : ExecutionResult.Success();
        }*/

        public static ISpecification<T> All<T>(
            this IEnumerable<ISpecification<T>> specifications)
        {
            return new AllSpecifications<T>(specifications);
        }

        public static ISpecification<T> AtLeast<T>(
            this IEnumerable<ISpecification<T>> specifications,
            int requiredSpecifications)
        {
            return new AtLeastSpecification<T>(requiredSpecifications, specifications);
        }

        public static ISpecification<T> And<T>(
            this ISpecification<T> specification1,
            ISpecification<T> specification2)
        {
            return new AndSpeficication<T>(specification1, specification2);
        }

        public static ISpecification<T> And<T>(
            this ISpecification<T> specification,
            Expression<Func<T, bool>> expression)
        {
            return specification.And(new ExpressionSpecification<T>(expression));
        }

        public static ISpecification<T> Or<T>(
            this ISpecification<T> specification1,
            ISpecification<T> specification2)
        {
            return new OrSpecification<T>(specification1, specification2);
        }
        public static ISpecification<T> Or<T>(
            this ISpecification<T> specification,
            Expression<Func<T, bool>> expression)
        {
            return specification.Or(new ExpressionSpecification<T>(expression));
        }

        public static ISpecification<T> Not<T>(
            this ISpecification<T> specification)
        {
            return new NotSpecification<T>(specification);
        }
}
}