using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;

namespace Akkatecture.Walkthrough.Domain.Repositories.Revenue.ReadModels
{
    public class RevenueReadModel
    {
        public Money Revenue { get; }
        public int Transactions { get; }

        public RevenueReadModel(Money revenue, int transactions)
        {
            Revenue = revenue;
            Transactions = transactions;
        }
    }
}