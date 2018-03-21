using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Akka.Actor;
using Akka.Persistence;
using Akka.Util;

namespace Akkatecture.Akka
{
    //WIP from https://gist.github.com/Horusiath/f5c77e043ee17e02d0311315bcde9c79
    public struct TypedActorReference<TMessage> : IActorRef
    {
        public IActorRef Ref { get; }
        public ActorPath Path => Ref.Path;

        public TypedActorReference(IActorRef aref) 
            : this()
        {
            if (aref == null) throw new ArgumentNullException(nameof(aref), $"{this} has received null instead of {nameof(IActorRef)}");
            Ref = aref;
        }
        
        void ICanTell.Tell(object message, IActorRef sender) => Ref.Tell(message, sender);
        public void Tell(TMessage message, IActorRef sender) => Ref.Tell(message, sender);
        public void Tell(TMessage message) => Ref.Tell(message, ActorCell.GetCurrentSelfOrNoSender());

        public bool Equals(IActorRef other) =>
            other is TypedActorReference<TMessage> ? Ref.Equals(((TypedActorReference<TMessage>)other).Ref) : Ref.Equals(other);

        public int CompareTo(IActorRef other) =>
            other is TypedActorReference<TMessage> ? Ref.CompareTo(((TypedActorReference<TMessage>)other).Ref) : Ref.CompareTo(other);

        public ISurrogate ToSurrogate(ActorSystem system) => new TypedActorReferenceSurrogate<TMessage>(Ref.ToSurrogate(system));

        public int CompareTo(object obj)
        {
            if (obj is IActorRef) return CompareTo((IActorRef)obj);
            throw new ArgumentException($"Cannot compare {obj} to {this}");
        }
        public override int GetHashCode() => Ref.GetHashCode();
        public override bool Equals(object obj) => obj is IActorRef && Equals((IActorRef)obj);
        public override string ToString() => Ref.ToString();
    }

    public struct Props<TMessage> : ISurrogated
    {
        public readonly Props Underlying;

        public Props(Props props) : this()
        {
            if (props == null) throw new ArgumentNullException(nameof(props), $"{this} has received null instead of {nameof(Props)}");
            this.Underlying = props;
        }

        public static Props<TMessage> Create<TActor>(Expression<Func<TActor>> fac) where TActor : Actor<TMessage>
            => new Props<TMessage>(Props.Create(fac));

        public ISurrogate ToSurrogate(ActorSystem system) => new TypedPropsSurrogate<TMessage>(Underlying.ToSurrogate(system));
    }

    internal struct TypedPropsSurrogate<TMessage> : ISurrogate
    {
        public readonly ISurrogate PropsSurrogate;

        public TypedPropsSurrogate(ISurrogate propsSurrogate) : this()
        {
            PropsSurrogate = propsSurrogate;
        }

        public ISurrogated FromSurrogate(ActorSystem system) => new Props<TMessage>((Props)PropsSurrogate.FromSurrogate(system));
    }


    internal struct TypedActorReferenceSurrogate<TMessage> : ISurrogate
    {
        public readonly ISurrogate RefSurrogate;

        public TypedActorReferenceSurrogate(ISurrogate refSurrogate) : this()
        {
            RefSurrogate = refSurrogate;
        }

        public ISurrogated FromSurrogate(ActorSystem system) => new TypedActorReference<TMessage>((IActorRef)RefSurrogate.FromSurrogate(system));
    }

    public abstract class Actor<TMessage> : UntypedActor
    {
        protected abstract void OnReceive(TMessage message);

        protected override void OnReceive(object message)
        {
            if (message is TMessage)
                OnReceive((TMessage)message);
            else Unhandled(message);
        }
    }


    public abstract class PersistentActor<TMessage> : ReceivePersistentActor
    {

        
    }

    public static class TypedRefExtensions
    {
        public static TypedActorReference<TMessage> ActorOf<TMessage>(this IActorRefFactory actorReferenceFactory, Props<TMessage> props) =>
            new TypedActorReference<TMessage>(actorReferenceFactory.ActorOf(props.Underlying));

        public static TypedActorReference<TMessage> ActorOf<TMessage>(this IActorRefFactory actorReferenceFactory, Props<TMessage> props, string name) =>
            new TypedActorReference<TMessage>(actorReferenceFactory.ActorOf(props.Underlying, name));

        public static TypedActorReference<TMessage> Watch<TMessage>(this IActorContext context, TypedActorReference<TMessage> typedReference)
        {
            context.Watch(typedReference.Ref);
            return typedReference;
        }

        public static TypedActorReference<TMessage> Unwatch<TMessage>(this IActorContext context, TypedActorReference<TMessage> typedReference)
        {
            context.Unwatch(typedReference.Ref);
            return typedReference;
        }
    }
}
