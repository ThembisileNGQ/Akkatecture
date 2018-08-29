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

using Akkatecture.Aggregates;
using Akkatecture.Extensions;
using Akkatecture.Specifications.Provided;
using Akkatecture.Walkthrough.Domain.Model.Account.Commands;
using Akkatecture.Walkthrough.Domain.Model.Account.Events;
using Akkatecture.Walkthrough.Domain.Model.Account.Specifications;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Model.Account
{
    public class Account : AggregateRoot<Account, AccountId, AccountState>
    {
        public Account(AccountId aggregateId)
            : base(aggregateId)
        {
            Command<OpenNewAccountCommand>(Execute);
            Command<TransferMoneyCommand>(Execute);
            Command<ReceiveMoneyCommand>(Execute);
        }

        private bool Execute(OpenNewAccountCommand command)
        {
            //this spec is part of Akkatecture
            var spec = new AggregateIsNewSpecification();
            if(spec.IsSatisfiedBy(this))
            {
                var aggregateEvent = new AccountOpenedEvent(command.OpeningBalance);
                Emit(aggregateEvent);
            }

            return true;
        }
        
        private bool Execute(TransferMoneyCommand command)
        {
            var balanceSpec = new EnoughBalanceAmountSpecification();
            var minimumTransferSpec = new MinimumTransferAmountSpecification();

            var andSpec = balanceSpec.And(minimumTransferSpec);
            
            if(andSpec.IsSatisfiedBy(this))
            {
                var sentEvent = new MoneySentEvent(command.Transaction);
                Emit(sentEvent);

                var feeEvent = new FeesDeductedEvent(new Money(0.25m));
                Emit(feeEvent);
            }
            
            return true;
        }
        
        private bool Execute(ReceiveMoneyCommand command)
        {
            var moneyReceived = new MoneyReceivedEvent(command.Transaction);

            Emit(moneyReceived);
            return true;
        }
    }
}