// The MIT License (MIT)
// Copyright (c) 2018 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture
// See LICENSE in the project root for license information.

using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
    public abstract class AggregateEvent<TAggregate, TIdentity> : IAggregateEvent<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        public override string ToString()
        {
            return $"{typeof(TAggregate).PrettyPrint()}/{GetType().PrettyPrint()}";
        }
    }
}