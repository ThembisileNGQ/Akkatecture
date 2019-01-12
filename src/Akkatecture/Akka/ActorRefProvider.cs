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
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;

namespace Akkatecture.Akka
{
    public class ActorRefProvider<T> : ActorRefProvider
    {
        public ActorRefProvider(IActorRef actorRef)
            : base(actorRef)
        {
        }
    }

    public abstract class ActorRefProvider
    {
        public IActorRef ActorRef { get; }
        protected ActorRefProvider(IActorRef actorRef)
        {
            ActorRef = actorRef;
        }
    }

    public static class ActorRefProviderExtensions
    {
        public static void Tell(this ActorRefProvider provider, object message, IActorRef sender)
        {
            provider.ActorRef.Tell(message, sender);
        }
        
        public static void Tell(this ActorRefProvider provider, object message)
        {
            provider.ActorRef.Tell(message, ActorRefs.NoSender);
        }

        public static Task<object> Ask(this ActorRefProvider provider, object message, TimeSpan? timeout = null)
        {
            return provider.ActorRef.Ask(message, timeout);
        }

        public static Task<object> Ask(this ActorRefProvider provider, object message, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask(message, cancellationToken);
        }

        public static Task<object> Ask(this ActorRefProvider provider, object message, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask(message, timeout, cancellationToken);
        }

        public static Task<T> Ask<T>(this ActorRefProvider provider, object message, TimeSpan? timeout = null)
        {
            return provider.ActorRef.Ask<T>(message, timeout);
        }

        public static Task<T> Ask<T>(this ActorRefProvider provider, object message, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask<T>(message, cancellationToken);
        }

        public static Task<T> Ask<T>(this ActorRefProvider provider, object message, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask<T>(message, timeout, cancellationToken);
        }

        public static Task<T> Ask<T>(this ActorRefProvider provider, Func<IActorRef, object> messageFactory, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask<T>(messageFactory, timeout, cancellationToken);
        }
    }
}