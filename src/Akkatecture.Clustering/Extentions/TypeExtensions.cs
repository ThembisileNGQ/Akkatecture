// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISagaHandlesAsync<,,>))
                .Select(t => typeof(IDomainEvent<,,>).MakeGenericType(t.GetGenericArguments()[0],
                    t.GetGenericArguments()[1], t.GetGenericArguments()[2]))
                .ToList();

            var startedByEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISagaIsStartedByAsync<,,>))
                .Select(t => typeof(IDomainEvent<,,>).MakeGenericType(t.GetGenericArguments()[0],
                    t.GetGenericArguments()[1], t.GetGenericArguments()[2]))
                .ToList();

            startedByEventTypes.AddRange(handleEventTypes);

            return startedByEventTypes;
        }
    }
}