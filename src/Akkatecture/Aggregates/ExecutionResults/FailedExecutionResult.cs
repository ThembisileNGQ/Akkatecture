using System.Collections.Generic;
using System.Linq;

namespace Akkatecture.Aggregates.ExecutionResults
{
    public class FailedExecutionResult : ExecutionResult
    {
        public IReadOnlyCollection<string> Errors { get; }

        public FailedExecutionResult(
            IEnumerable<string> errors)
        {
            Errors = (errors ?? Enumerable.Empty<string>()).ToList();
        }
            
        public override bool IsSuccess { get; } = false;

        public override string ToString()
        {
            return Errors.Any()
                ? $"Failed execution due to: {string.Join(", ", Errors)}"
                : "Failed execution";
        }
    }
}