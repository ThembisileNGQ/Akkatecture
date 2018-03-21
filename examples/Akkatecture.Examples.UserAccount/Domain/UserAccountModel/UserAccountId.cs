using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class UserAccountId : Identity<UserAccountId>
    {
        public UserAccountId(string value)
            : base(value)
        {
            
        }
    }
}