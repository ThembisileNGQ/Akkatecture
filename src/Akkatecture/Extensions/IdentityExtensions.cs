using System.Text;
using Akkatecture.Core;

namespace Akkatecture.Extensions
{
    public static class IdentityExtensions
    {
        public static byte[] GetBytes(this IIdentity identity)
        {
            return Encoding.UTF8.GetBytes(identity.Value);
        }
    }
}
