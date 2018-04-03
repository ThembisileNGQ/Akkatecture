using System.Linq;
using Akkatecture.Core;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Core
{
    public class CircularBufferTests
    {
        [Theory]
        [InlineData(1)] // Below capacity
        [InlineData(1, 2)] // At capacity
        [InlineData(1, 2, 3)] // Once above capacity
        [InlineData(1, 2, 3, 4)] // Loop twice over capacity
        [InlineData(1, 2, 3, 4, 5)] // One more than of capacity
        public void CircularBuffer_Putting_ShouldContainCapacity(params int[] numbers)
        {
            const int capacity = 2;
            var sut = new CircularBuffer<int>(capacity);

            foreach (var number in numbers)
            {
                sut.Put(number);
            }
            
            var shouldContain = numbers.Reverse().Take(capacity).ToList();
            sut.Should().Contain(shouldContain);
        }
    }
}
