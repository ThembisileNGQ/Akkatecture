using Akkatecture.Clustering.Configuration;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Configuration
{
    public class ClusteringDefaultSettingsTests
    {
        [Fact]
        public void ClusteringDefaultSettings_IsNotEmpty()
        {
            var config = AkkatectureClusteringDefaultSettings.DefaultConfig();

            var value = config.GetString("akka.cluster.name");

            value.Should().Be("akkatecture");
        }
    }
}