using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Repositories.Revenue.Commands
{
    public class AddRevenueCommand
    {
        public Money AmountToAdd { get; }

        public AddRevenueCommand(Money amountToAdd)
        {
            AmountToAdd = amountToAdd;
        }
    }
}