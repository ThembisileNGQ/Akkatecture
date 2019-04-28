using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Core;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestExecutionResult
    {
        public IExecutionResult Result { get; set; }
        public ISourceId SourceId { get; set; }

        public static TestExecutionResult FailedWith(ISourceId sourceId)
        {
            return new TestExecutionResult
            {
                Result = ExecutionResult.Failed(),
                SourceId = sourceId
            };
        }
        public static TestExecutionResult SucceededWith(ISourceId sourceId)
        {
            return new TestExecutionResult
            {
                Result = ExecutionResult.Success(),
                SourceId = sourceId
            };
        }
    }
}