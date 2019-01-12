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
using Akka.Actor;
using Akkatecture.Examples.Domain.Model.UserAccount;
using Akkatecture.Examples.Domain.Model.UserAccount.Commands;

namespace Akkatecture.Examples.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Create actor system
            var system = ActorSystem.Create("useraccount-example");

            //Create supervising aggregate manager for UserAccount aggregate root actors
            var aggregateManager = system.ActorOf(Props.Create(() => new UserAccountAggregateManager()));

            //Build create user account aggregate command
            var aggregateId = UserAccountId.New;
            var createUserAccountCommand = new CreateUserAccountCommand(aggregateId, "foo bar");
            
            //Send command, this is equivalent to command.publish() in other cqrs frameworks
            aggregateManager.Tell(createUserAccountCommand);

            var changeNameCommand = new UserAccountChangeNameCommand(aggregateId, "foo bar baz");
            aggregateManager.Tell(changeNameCommand);
                        
            //block end of program
            Console.ReadLine();
        }
    }
}
