using Akkatecture.Sagas.SagaTimeouts;

namespace Akkatecture.TestHelpers.Aggregates.Sagas.Test.SagaTimeouts
{
    public class TestSagaTimeout: ISagaTimeoutJob
    {
        public string MessageToInclude { get; set; }

        public TestSagaTimeout(string messageToInclude)
        {
            MessageToInclude = messageToInclude;
        }
        
        public TestSagaTimeout()
        {
            MessageToInclude = "Some default message.";
        }
    }
    
    public class TestSagaTimeout2: ISagaTimeoutJob
    {
        public string MessageToInclude { get; set; }

        public TestSagaTimeout2(string messageToInclude)
        {
            MessageToInclude = messageToInclude;
        }
        
        public TestSagaTimeout2()
        {
            MessageToInclude = "Some default message from timeout 2!!.";
        }
    }
}