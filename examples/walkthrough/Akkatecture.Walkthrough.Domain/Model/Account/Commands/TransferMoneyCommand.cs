using System;
using Akkatecture.Commands;
using Akkatecture.Walkthrough.Domain.Model.Account.Entities;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Commands
{
    public class TransferMoneyCommand : Command<Account, AccountId>
    {
        public Transaction Transaction { get; }
        public TransferMoneyCommand(
            AccountId aggregateId,
            Transaction transaction)
            : base(aggregateId)
        {
            if(transaction == null) throw new ArgumentNullException(nameof(transaction));

            Transaction = transaction;
        }
    }
}
