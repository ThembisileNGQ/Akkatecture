using System;
using System.Collections.Generic;
using System.Text;
using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public class EventId : Identity<EventId>, IEventId
    {
        public EventId(string value) : base(value)
        {
        }
    }
}
