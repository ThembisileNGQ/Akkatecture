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

using System.Collections.Generic;
using Akka.Actor;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Commands;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource.Events;

namespace Akkatecture.Examples.Api.Domain.Aggregates.Resource.Commands
{
    public class CreateResourceCommand : Command<Resource, ResourceId>
    {
        public CreateResourceCommand(ResourceId aggregateId)
            : base(aggregateId)
        {
            
        }
    }

    public class CreateResourceCommandHandler : CommandHandler<Resource, ResourceId, CreateResourceCommand>
    {
        public override void Handle(
            Resource aggregate,
            IActorContext context,
            CreateResourceCommand command)
        {
            if (aggregate.IsNew)
            {
                var aggregateEvent = new ResourceCreatedEvent();
                aggregate.Emit(aggregateEvent);


                var executionResult = new SuccessExecutionResult();
                context.Sender.Tell(executionResult);
            }
            else
            {
                var executionResult = new FailedExecutionResult(new List<string> {"aggregate is already created"});
                context.Sender.Tell(executionResult);
            }
        }
    }
}