using System;
using System.Security.Cryptography.X509Certificates;
using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.Extensions;
using FGS.Pump.Tests.Support.TestCategories;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RestSharp;

namespace FGS.Pump.Extensions.Tests
{
    [TestFixture]
    [Unit]
    public class RestClientFactoryTests : BaseUnitTest
    {
        private RestClientFactory _subject;
        private Mock<IRestClient> _mockClient;
        private X509Certificate2Collection _certificateCollection;

        [SetUp]
        public void Setup()
        {
            _mockClient = Fixture.Mock<IRestClient>();
            _certificateCollection = new X509Certificate2Collection();
            Fixture.Register<Func<IRestClient>>(() => () => _mockClient.Object);
            Fixture.Register<Func<X509Certificate2, X509Certificate2Collection>>(() => (cert) => _certificateCollection);
            _subject = Fixture.Create<RestClientFactory>();
        }

        [Test]
        public void Create_GivenAddress_SetsClientsBaseUrlUsingAddress()
        {
            var address = Fixture.Create<Uri>();
            var expected = new Uri($"{address.Scheme}://{address.Authority}");

            _subject.Create(address);

            _mockClient.VerifySet(c => c.BaseUrl = expected, Times.Once);
        }

        [Test]
        public void Create_GivenNullCertificate_DoesNotSetCertificateOnClient()
        {
            var address = Fixture.Create<Uri>();

            _subject.Create(address, null);

            _mockClient.VerifySet(c => c.ClientCertificates = _certificateCollection, Times.Never);
        }

        [Test]
        public void Create_GivenValidCertificate_SetsCertificateOnClient()
        {
            var address = Fixture.Create<Uri>();
            var certificate = Fixture.Mock<X509Certificate2>();

            _subject.Create(address, certificate.Object);

            _mockClient.VerifySet(c => c.ClientCertificates = _certificateCollection, Times.Once);
        }
    }
}
