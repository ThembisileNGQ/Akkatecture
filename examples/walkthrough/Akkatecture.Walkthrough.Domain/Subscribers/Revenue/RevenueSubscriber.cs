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

using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Subscribers;
using Akkatecture.Walkthrough.Domain.Model.Account;
using Akkatecture.Walkthrough.Domain.Model.Account.Events;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.Commands;

namespace Akkatecture.Walkthrough.Domain.Subscribers.Revenue
{
    public class RevenueSubscriber : DomainEventSubscriber,
         ISubscribeToAsync<Account,AccountId,FeesDeductedEvent>
    {
        public IActorRef RevenueRepository { get; }
        
        public RevenueSubscriber(IActorRef revenueRepository)
        {
            RevenueRepository = revenueRepository;
        }
        
        public Task HandleAsync(IDomainEvent<Account, AccountId, FeesDeductedEvent> domainEvent)
        {
            var command = new AddRevenueCommand(domainEvent.AggregateEvent.Amount);
            RevenueRepository.Tell(command);
            
            return Task.CompletedTask;
        }
    }
}