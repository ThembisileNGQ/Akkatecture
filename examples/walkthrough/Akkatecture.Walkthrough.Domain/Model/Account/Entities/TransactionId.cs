using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Akkatecture.Walkthrough.Domain.Model.Account.Entities
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class TransactionId : Identity<TransactionId>
    {
        public TransactionId(string value)
            : base(value)
        {
            
        }
    }
}