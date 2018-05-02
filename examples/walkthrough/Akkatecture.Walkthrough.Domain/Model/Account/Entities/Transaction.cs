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
            Sender = sender;
            Receiver = receiver;
            Amount = amount;
        }
    }
}