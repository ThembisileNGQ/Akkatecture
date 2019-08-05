namespace Akkatecture.TestHelpers.Jobs
{
    public class TestJobAck
    {
        public static TestJobAck Instance => new TestJobAck();
    }
    
    public class TestJobNack
    {
        public static TestJobNack Instance => new TestJobNack();
    }
}