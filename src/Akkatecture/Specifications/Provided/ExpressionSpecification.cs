using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Akkatecture.Extensions;

namespace Akkatecture.Specifications.Provided
{
    public class ExpressionSpecification<T> : Specification<T>
    {
        private readonly Func<T, bool> _predicate;
        private readonly Lazy<string> _string;

        public ExpressionSpecification(
            Expression<Func<T, bool>> expression)
        {
            _predicate = expression.Compile();
            _string = new Lazy<string>(() => MakeString(expression));
        }

        public override string ToString()
        {
            return _string.Value;
        }

        protected override IEnumerable<string> IsNotSatisfiedBecause(T account)
        {
            if (!_predicate(account))
            {
                yield return $"'{_string.Value}' is not satisfied";
            }
        }

        private static string MakeString(Expression<Func<T, bool>> expression)
        {
            try
            {
                var paramName = expression.Parameters[0].Name;
                var expBody = expression.Body.ToString();

                expBody = expBody
                    .Replace("AndAlso", "&&")
                    .Replace("OrElse", "||");

                return $"{paramName} => {expBody}";
            }
            catch
            {
                return typeof(ExpressionSpecification<T>).PrettyPrint();
            }
        }
    }
}