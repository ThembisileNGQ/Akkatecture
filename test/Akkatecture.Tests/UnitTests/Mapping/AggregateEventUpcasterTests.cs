using System;
using System.ComponentModel;
using Akka.Persistence.Journal;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Events;
using Akkatecture.Extensions;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Entities;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Upcasters;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Mapping
{
    public class AggregateEventUpcasterTests
    {
        private const string Category = "Mapping";

        [Fact]
        [Category(Category)]
        public void CommittedEvent_Tagged_ContainsTaggedElements()
        {
            var aggregateEventTagger = new TestAggregateEventUpcaster();
        }
    }
}