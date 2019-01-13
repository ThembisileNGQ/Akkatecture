// The MIT License (MIT)
//
// Copyright (c) 2015-2019 Rasmus Mikkelsen
// Copyright (c) 2015-2019 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
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

using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Commands
{
    public abstract class CommandHandler<TAggregate, TIdentity,TResult,TCommand> :
        ICommandHandler<TAggregate, TIdentity,TResult, TCommand>
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate, TIdentity>
    {
        public abstract TResult HandleCommand(
            TAggregate aggregate,
            IActorContext context,
            TCommand command);
    }

    public abstract class CommandHandler<TAggregate, TIdentity, TCommand> :
        CommandHandler<TAggregate, TIdentity,bool,TCommand>
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate, TIdentity>
    {
        public override bool HandleCommand(
            TAggregate aggregate,
            IActorContext context,
            TCommand command)
        {
            var logger = context.GetLogger();
            Handle(aggregate, context, command);
            logger.Info($"{command.GetType().PrettyPrint()} handled in {GetType().PrettyPrint()}");
            return true;
        }

        public abstract void Handle(
            TAggregate aggregate,
            IActorContext context,
            TCommand command);
    }
}