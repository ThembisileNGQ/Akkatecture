using Akka.Actor;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.Commands;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.Queries;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.ReadModels;

namespace Akkatecture.Walkthrough.Domain.Repositories.Revenue
{
    public class RevenueRepository : ReceiveActor
    {
        public Money Revenue { get; private set; } = new Money(0.00m);
        public int Transactions { get; private set; } = 0;

        public RevenueRepository()
        {
            Receive<AddRevenueCommand>(Handle);
            Receive<GetRevenueQuery>(Handle);
        }

        private bool Handle(AddRevenueCommand command)
        {
            Revenue = Revenue + command.AmountToAdd;
            Transactions++;
            return true;
        }

        private bool Handle(GetRevenueQuery query)
        {
            var readModel = new RevenueReadModel(Revenue, Transactions);
            Sender.Tell(readModel);
            return true;
        }
    }
}