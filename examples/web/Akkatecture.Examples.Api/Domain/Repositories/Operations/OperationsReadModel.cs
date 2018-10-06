using System;

namespace Akkatecture.Examples.Api.Domain.Repositories.Operations
{
    public class OperationsReadModel
    {
        public Guid Id { get; }
        public int Percentage { get; }
        public int Elapsed { get; }
        public string Status => Percentage < 100 ? "Running" : "Finished";

        public OperationsReadModel(
            Guid id,
            int percentage,
            int elapsed)
        {
            Id = id;
            Percentage = percentage;
            Elapsed = elapsed;
        }
    }
    
    
}