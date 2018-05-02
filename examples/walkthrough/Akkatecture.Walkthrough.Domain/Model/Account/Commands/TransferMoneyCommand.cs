using System;
using Akkatecture.Commands;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Commands
{
    public class TransferMoneyCommand : Command<Account, AccountId>
    {
        public AccountId ReceiverId { get; }
        public Money Amount { get; }
        public TransferMoneyCommand(
            AccountId aggregateId, 
            AccountId receiverId,
            Money amount) 
            : base(aggregateId) 
        {
            if(amount == null) throw new ArgumentNullException(nameof(amount));

            Amount = amount;
            ReceiverId = receiverId;
        }
    }
}
