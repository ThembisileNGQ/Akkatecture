using System;
using System.Linq.Expressions;
using Akkatecture.Aggregates;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer
{
    public class MoneyTransferSagaManager : AggregateSagaManager<MoneyTransferSaga,MoneyTransferSagaId,MoneyTransferSagaLocator>
    {
        public MoneyTransferSagaManager(Expression<Func<MoneyTransferSaga>> factory)
            : base(factory)
        {
            
        }
    }
}