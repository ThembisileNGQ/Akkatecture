// The MIT License (MIT)
//
// Copyright (c) 2018 Lutando Ngqakaza
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
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Util;

namespace Akkatecture.Akka
{
    public class ActorRefOfT<T> : IActorRef<T>
    {
        public RepointableActorRef ActorRef { get; }
        public ActorPath Path { get; }
        public bool IsLocal { get; }
        public IInternalActorRef Parent { get; }
        public IActorRefProvider Provider { get; }
        public bool IsTerminated { get; }
        public ActorRefOfT(IActorRef actorRef)
        {
            ActorRef = actorRef as RepointableActorRef;
            Path = actorRef.Path;
            IsLocal = ActorRef.IsLocal;
            Parent = ActorRef.Parent;
            Provider = ActorRef.Provider;
            IsTerminated = ActorRef.IsTerminated;
        }

        public void Tell(object message, IActorRef sender)
        {
            ActorRef.Tell(message, sender);
        }

        public bool Equals(IActorRef other)
        {
            return ActorRef.Equals(other);
        }

        public int CompareTo(IActorRef other)
        {
            return ActorRef.CompareTo(other);
        }

        public ISurrogate ToSurrogate(ActorSystem system)
        {
            return ActorRef.ToSurrogate(system);
        }

        public int CompareTo(object obj)
        {
            return ActorRef.CompareTo(obj);
        }

        public IActorRef GetChild(IEnumerable<string> name)
        {
            return ActorRef.GetChild(name);
        }

        public void Resume(Exception causedByFailure = null)
        {
            ActorRef.Resume(causedByFailure);
        }

        public void Start()
        {
            ActorRef.Start();
        }

        public void Stop()
        {
            ActorRef.Stop();
        }

        public void Restart(Exception cause)
        {
            ActorRef.Restart(cause);
        }

        public void Suspend()
        {
            ActorRef.Suspend();
        }

        public void SendSystemMessage(ISystemMessage message, IActorRef sender)
        {
            ActorRef.SendSystemMessage(message);
        }

        public void SendSystemMessage(ISystemMessage message)
        {
            ActorRef.SendSystemMessage(message);
        }

    }
}