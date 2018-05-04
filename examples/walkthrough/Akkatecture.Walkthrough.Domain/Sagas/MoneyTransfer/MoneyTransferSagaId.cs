using Akkatecture.Sagas;

namespace Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer
{
    public class MoneyTransferSagaId : SagaId<MoneyTransferSagaId>
    {
        public MoneyTransferSagaId(string value)
            : base(value)
        {
            
        }
    }
}
