using Moq;
using NUnit.Framework;
using ReliableDownloader.Contracts.Services;
using ReliableDownloader.Contracts.Validations;
using ReliableDownloader.Logic;
using ReliableDownloader.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace ReliableDownloader.Tests.Services
{
    [TestFixture]
    public class FileDownloaderTests
    {
        private Mock<IGetter> _getter;
        private Mock<IWriter> _writer;
        private Mock<IValidate> _validate;

        [SetUp]
        public void Setup()
        {
            _getter = new Mock<IGetter>();
            _writer = new Mock<IWriter>();
            _validate = new Mock<IValidate>();
        }

        [Test]
        public async Task TestHappyPath()
        {
            // Arrange
            _getter.Setup(x => x.GetHeadersAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FileHeader());
            _writer.Setup(x => x.SaveContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            _validate.Setup(x => x.IsValid(It.IsAny<string>(), It.IsAny<FileHeader>())).Returns(true);
            var fileDownloader = new FileDownloader(_getter.Object, _writer.Object, _validate.Object, new CancellationTokenSource());

            // Act
            var result = await fileDownloader.DownloadFile("","",x => { }, new TimeSpan());

            // Assert
            result.Should().BeTrue();
        }
    }
}