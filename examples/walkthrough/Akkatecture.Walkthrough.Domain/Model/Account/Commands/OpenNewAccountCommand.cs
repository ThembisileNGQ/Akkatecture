using System;
using Akkatecture.Commands;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Commands
{
    public class OpenNewAccountCommand : Command<Account,AccountId> 
    {
        public Money OpeningBalance { get; }
        
        public OpenNewAccountCommand(AccountId aggregateId, Money openingBalance)
            : base(aggregateId)
        {
            if(openingBalance == null) throw new ArgumentNullException(nameof(openingBalance));

            OpeningBalance = openingBalance;
        }
    }
}