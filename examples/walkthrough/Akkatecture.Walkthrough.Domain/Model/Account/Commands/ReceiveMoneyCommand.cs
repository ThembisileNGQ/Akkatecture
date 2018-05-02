using System;
using Akkatecture.Commands;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Commands
{
    public class ReceiveMoneyCommand : Command<Account,AccountId>
    {
        public AccountId SenderId { get; }
        public Money Amount { get; }
        
        public ReceiveMoneyCommand(
            AccountId aggregateId, 
            AccountId senderId,
            Money amount) 
            : base(aggregateId) 
        {
            if(amount == null) throw new ArgumentNullException(nameof(amount));

            Amount = amount;
            SenderId = senderId;
        }
    }
}