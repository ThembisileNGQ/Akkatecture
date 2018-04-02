namespace Akkatecture.Sagas
{
    public enum SagaStatus
    {
        NotStarted = 0,
        Running = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
        PartiallySucceeded = 5,
    }
}