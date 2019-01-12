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

using Akka.Actor;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.Commands;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.Queries;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.ReadModels;

namespace Akkatecture.Walkthrough.Domain.Repositories.Revenue
{
    public class RevenueRepository : ReceiveActor
    {
        public Money Revenue { get; private set; }
        public int Transactions { get; private set; }

        public RevenueRepository()
        {
            Transactions = 0;
            Revenue = new Money(0.00m);
            
            Receive<AddRevenueCommand>(Handle);
            Receive<GetRevenueQuery>(Handle);
        }

        private bool Handle(AddRevenueCommand command)
        {
            Revenue = Revenue + command.AmountToAdd;
            Transactions++;
            return true;
        }

        private bool Handle(GetRevenueQuery query)
        {
            var readModel = new RevenueReadModel(Revenue, Transactions);
            Sender.Tell(readModel);
            return true;
        }
    }
}