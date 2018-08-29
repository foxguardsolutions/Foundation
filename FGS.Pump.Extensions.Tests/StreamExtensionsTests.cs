using System.IO;
using System.Linq;

using AutoFixture;

using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Extensions.Tests
{
    [TestFixture]
    [Unit]
    public class StreamExtensionsTests : BaseUnitTest
    {
        [Test]
        public void CopyToMemory_GivenBytes_ReturnsStreamWithSameBytes()
        {
            var expected = Fixture.CreateMany<byte>().ToArray();
            var inputStream = new MemoryStream(expected);

            var resultStream = inputStream.CopyToMemory();
            var actual = resultStream.ToArray();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CopyToMemory_GivenBytes_ReturnsMemoryStream()
        {
            var inputBytes = Fixture.CreateMany<byte>().ToArray();
            var inputStream = new MemoryStream(inputBytes);

            var actual = inputStream.CopyToMemory();

            Assert.That(actual, Is.AssignableTo<MemoryStream>());
        }
    }
}
