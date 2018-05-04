using System;
using Akkatecture.Entities;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Entities
{
    public class Transaction : Entity<TransactionId>
    {
        public AccountId Sender { get; }
        public AccountId Receiver { get; }
        public Money Amount { get; }
        
        public Transaction(TransactionId entityId, AccountId sender, AccountId receiver, Money amount)
            : base(entityId)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            if(amount == null) throw new ArgumentNullException(nameof(amount));

            Sender = sender;
            Receiver = receiver;
            Amount = amount;
        }

        public Transaction(AccountId sender, AccountId receiver, Money amount)
            :this(TransactionId.New,sender,receiver,amount)
        {  
        }
    }
}