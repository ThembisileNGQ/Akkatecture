using System.Collections.Generic;
using System.Linq;

namespace Akkatecture.Aggregates.ExecutionResults
{
    public abstract class ExecutionResult : IExecutionResult
    {
        private static readonly IExecutionResult SuccessResult = new SuccessExecutionResult();
        private static readonly IExecutionResult FailedResult = new FailedExecutionResult(Enumerable.Empty<string>());

        public static IExecutionResult Success() => SuccessResult;
        public static IExecutionResult Failed() => FailedResult;
        public static IExecutionResult Failed(IEnumerable<string> errors) => new FailedExecutionResult(errors);
        public static IExecutionResult Failed(params string[] errors) => new FailedExecutionResult(errors);

        public abstract bool IsSuccess { get; }

        public override string ToString()
        {
            return $"ExecutionResult - IsSuccess:{IsSuccess}";
        }
    }
}