using System;
using System.Security.Cryptography.X509Certificates;

using AutoFixture;

using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.Extensions;
using FGS.Pump.Tests.Support.TestCategories;

using Moq;

using NUnit.Framework;

namespace FGS.Pump.Extensions.Tests
{
    [TestFixture]
    [Unit]
    public class WebClientFactoryTests : BaseUnitTest
    {
        private IWebClientFactory _factory;
        private Mock<X509Certificate2> _mockClientCertificate;
        private Mock<IWebClient> _mockClient;

        [SetUp]
        public void Setup()
        {
            _mockClientCertificate = Fixture.Mock<X509Certificate2>();
            _mockClient = Fixture.Mock<IWebClient>();
            Fixture.Register<Func<IWebClient>>(() => () => _mockClient.Object);

            _factory = Fixture.Create<WebClientFactory>();
        }

        [Test]
        public void Create_GivenNoValues_SetsTheClientCertificateNull()
        {
            _factory.Create();

            _mockClient.VerifySet(c => c.ClientCertificate = null, Times.Once);
        }

        [Test]
        public void Create_GivenCertificate_SetsClientCertificate()
        {
            _factory.Create(_mockClientCertificate.Object);

            _mockClient.VerifySet(c => c.ClientCertificate = _mockClientCertificate.Object, Times.Once);
        }
    }
}
