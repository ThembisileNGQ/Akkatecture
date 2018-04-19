using Akkatecture.Clustering.Configuration;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Clustering.Configuration
{
    public class ClusteringDefaultSettingsTests
    {
        [Fact]
        public void ClusteringDefaultSettings_IsNotEmpty()
        {
            var config = AkkatectureClusteringDefaultSettings.DefaultConfig();

            var value = config.GetString("akkatecture.test-value");

            value.Should().Be("foo bar");
        }
    }
}