using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Akkatecture.Aggregates;
using Akkatecture.Sagas;

namespace Akkatecture.Clustering.Extentions
{
    public static class TypeExtensions
    {
        internal static IReadOnlyList<Type> GetSagaEventSubscriptionTypes(this Type type)
        {
            //TODO
            //add checks for iaggregateroot
            //add checks for iidentity
            //add checks for iaggregatevent

            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();

            var handleEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISagaHandles<,,>))
                .Select(t => typeof(IDomainEvent<,,>).MakeGenericType(t.GetGenericArguments()[0],
                    t.GetGenericArguments()[1], t.GetGenericArguments()[2]))
                .ToList();

            var startedByEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISagaIsStartedBy<,,>))
                .Select(t => typeof(IDomainEvent<,,>).MakeGenericType(t.GetGenericArguments()[0],
                    t.GetGenericArguments()[1], t.GetGenericArguments()[2]))
                .ToList();

            startedByEventTypes.AddRange(handleEventTypes);

            return startedByEventTypes;
        }
    }
}