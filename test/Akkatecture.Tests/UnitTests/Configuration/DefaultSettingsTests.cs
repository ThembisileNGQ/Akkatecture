using Akkatecture.Configuration;
using FluentAssertions;
using Xunit;

namespace Akkatecture.Tests.UnitTests.Configuration
{
    public class DefaultSettingsTests
    {
        [Fact]
        public void ClusteringDefaultSettings_IsNotEmpty()
        {
            var config = AkkatectureDefaultSettings.DefaultConfig();

            var value = config.GetBoolean("akkatecture.aggregate-manager.handle-deadletters");

            value.Should().BeTrue();
        }
    }
}