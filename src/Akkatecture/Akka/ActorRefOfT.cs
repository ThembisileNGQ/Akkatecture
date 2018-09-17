using System;
using Akka.Actor;
using Akka.Util;

namespace Akkatecture.Akka
{
    public class ActorRefOfT<T> : IActorRef<T>
    {
        public IActorRef ActorRef { get; }
        public ActorPath Path { get; }
        
        public ActorRefOfT(IActorRef actorRef)
        {
            ActorRef = actorRef;
            Path = actorRef.Path;
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

    }
}