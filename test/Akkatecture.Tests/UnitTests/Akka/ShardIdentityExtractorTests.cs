using System;
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
        
        [Fact]
        public void ShardIdentityExtractor_InValidObject_ThrowsArgumentException()
        {
            var message = string.Empty;

            this.Invoking(test => ShardIdentityExtractor.IdentityExtrator(message))
                .Should().Throw<ArgumentException>()
                .WithMessage(nameof(message));
        }
        
        [Fact]
        public void ShardIdentityExtractor_InValidObject_ThrowsArgumentNullException()
        {
            ShardTestMessageId message = null;

            this.Invoking(test => ShardIdentityExtractor.IdentityExtrator(message))
                .Should().Throw<ArgumentNullException>();
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