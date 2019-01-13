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

using Akkatecture.Aggregates;
using Akkatecture.Examples.Domain.Model.UserAccount.Commands;
using Akkatecture.Examples.Domain.Model.UserAccount.Events;

namespace Akkatecture.Examples.Domain.Model.UserAccount
{
    public class UserAccountAggregate : AggregateRoot<UserAccountAggregate,UserAccountId,UserAccountState>
    {
        public UserAccountAggregate(UserAccountId id)
            : base(id)
        {
            Command<CreateUserAccountCommand>(Execute);
            Command<UserAccountChangeNameCommand>(Execute);
        }
        
        public bool Execute(CreateUserAccountCommand command)
        {
            Create(command.Name);
            return true;
        }

        public bool Execute(UserAccountChangeNameCommand command)
        {
            ChangeName(command.Name);
            return true;
        }
        
        private void Create(string name)
        {
            if (IsNew)
            {
                Emit(new UserAccountCreatedEvent(name));
            }
            else
            {
                //signal domain error, aggregate already exists.
            }
        }

        private void ChangeName(string name)
        {
            if (!IsNew)
            {
                Emit(new UserAccountNameChangedEvent(name));   
            }
            else
            {
                //signal domain error, aggregate doesnt exist.
            }
        }
        
    }
}