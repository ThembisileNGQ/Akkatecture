using System;

namespace Akkatecture.Examples.Api.Domain.Repositories.Operations
{
    public class OperationsReadModel
    {
        public Guid Id { get; }
        public int Percentage { get; }
        public int Elapsed { get; }
        public string Status => Percentage < 100 ? "Running" : "Finished";
        public DateTime StartedAt { get; }

        public OperationsReadModel(
            Guid id,
            int percentage,
            int elapsed,
            DateTime startedAt)
        {
            Id = id;
            Percentage = percentage;
            Elapsed = elapsed;
            StartedAt = startedAt;
        }
    }
    
    
}