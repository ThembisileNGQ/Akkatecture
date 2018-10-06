using System;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource;

namespace Akkatecture.Examples.Api.Domain.Repositories.Resources
{
    public class ResourcesReadModel
    {
        public Guid Id { get; }
        public double ElapsedTimeToCreation { get; }
        public DateTime CreatedAt { get; }

        public ResourcesReadModel(
            Guid id,
            double elapsedTimeToCreation,
            DateTime createdAt)
        {
            Id = id;
            ElapsedTimeToCreation = elapsedTimeToCreation;
            CreatedAt = createdAt;
        }
    }
}