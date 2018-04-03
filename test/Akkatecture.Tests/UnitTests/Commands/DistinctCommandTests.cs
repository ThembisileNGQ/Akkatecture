namespace Akkatecture.Tests.UnitTests.Commands
{
    public class DistinctCommandTests
    {
        /*[Theory]
        [InlineData("test-4b1e7b48-18f1-4215-91d9-903cffdab3d8", 1, "command-40867fe4-94da-59e4-9bb5-e532f6565751")]
        [InlineData("test-4b1e7b48-18f1-4215-91d9-903cffdab3d8", 2, "command-df0e238e-676b-500e-b962-76fcef97768a")]
        [InlineData("test-6a2a04bd-bbc8-44ac-80ac-b0ca56897bc0", 2, "command-0455a861-bc9e-56c5-b7b9-5c14671db8b2")]
        public void Arguments(string aggregateId, int magicNumber, string expectedSouceId)
        {
            var testId = TestId.With(aggregateId);
            var command = new MyDistinctCommand(testId, magicNumber);

            var sourceId = command.SourceId;
            
            sourceId.Value.Should().Be(expectedSouceId);
        }

        public class MyDistinctCommand : DistinctCommand<TestAggregate, TestId>
        {
            public int MagicNumber { get; }

            public MyDistinctCommand(
                TestId aggregateId,
                int magicNumber) : base(aggregateId)
            {
                MagicNumber = magicNumber;
            }

            protected override IEnumerable<byte[]> GetSourceIdComponents()
            {
                yield return BitConverter.GetBytes(MagicNumber);
                yield return AggregateId.GetBytes();
            }
        }*/
    }
}