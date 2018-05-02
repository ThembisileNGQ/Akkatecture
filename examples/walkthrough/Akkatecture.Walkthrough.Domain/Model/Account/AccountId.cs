using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Akkatecture.Walkthrough.Domain.Model.Account
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class AccountId : Identity<AccountId>
    {
        public AccountId(string value)
            : base(value)
        {
        }
    }
}
