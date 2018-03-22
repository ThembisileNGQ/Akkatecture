using Akkatecture.Akka;
using Akkatecture.Core;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Akka
{
    public class ShardIdentityExtractorTests
    {
        [Fact]
        public void ShardIdentityExtractor_ValidMessage_ExtractsIdentity()
        {
            var message = ShardTestMessageId.New;

            var extractedIdentity = ShardIdentityExtractor.IdentityExtrator(message).Item1;

            extractedIdentity.Should().BeEquivalentTo(message.Value);
        }
        
        [Fact]
        public void ShardIdentityExtractor_ValidObject_ExtractsObject()
        {
            var message = ShardTestMessageId.New;

            var extractedObject = ShardIdentityExtractor.IdentityExtrator(message).Item2;

            extractedObject.GetHashCode().Should().Be(message.GetHashCode());
        }
    }

    public class ShardTestMessageId : Identity<ShardTestMessageId>
    {
        public ShardTestMessageId(string value)
            : base(value)
        {
            
        }
    }
    
}