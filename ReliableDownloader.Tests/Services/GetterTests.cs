using Moq;
using NUnit.Framework;
using ReliableDownloader.Contracts.Services;
using ReliableDownloader.Logic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace ReliableDownloader.Tests
{
    [TestFixture]
    public class GetterTests
    {
        private Mock<IWebSystemCalls> _webSystemCallsmock;

        [SetUp]
        public void Setup()
        {
            _webSystemCallsmock = new Mock<IWebSystemCalls>();
        }
        
        [Test]
        public async Task TestError()
        {
            // Arrange
            _webSystemCallsmock.Setup(x => x.GetHeadersAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(HttpResponseMessage));
            var getter = new Getter(_webSystemCallsmock.Object);

            // Act
            var result = await getter.GetHeadersAsync("", new CancellationToken());

            // Assert
            result.HasError.Should().BeTrue();
        }

        [Test]
        public async Task TestHappyPath()
        {
            // Arrange
            _webSystemCallsmock.Setup(x => x.GetHeadersAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage());
            var getter = new Getter(_webSystemCallsmock.Object);

            // Act
            var result = await getter.GetHeadersAsync("", new CancellationToken());

            // Assert
            result.HasError.Should().BeFalse();
            result.ContentLength.Should().Be(0);
            result.ContentMD5.Should().Equal(null);
        }
    }
}