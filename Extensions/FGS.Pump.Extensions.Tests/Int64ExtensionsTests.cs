using FGS.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Extensions.Tests
{
    [Unit]
    [TestFixture]
    public class Int64ExtensionsTests
    {
        [Test]
        public void WhenSizeIsLessThan1024_SizeIsInBytes()
        {
            string expected = "1,000 bytes";
            var actual = Int64Extensions.SizeSuffix(1000);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WhenSizeIs1024_SizeIs1KB()
        {
            long kilobyte = 1024;
            long inputSize = 1;
            string expected = "1.0 KB";
            var actual = Int64Extensions.SizeSuffix(inputSize * kilobyte);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WhenSizeIsGreaterThan1024_SizeIsInKilobytes()
        {
            long kilobyte = 1024;
            double inputSize = 1.5;
            string expected = "1.5 KB";
            var actual = Int64Extensions.SizeSuffix((long)(kilobyte * inputSize));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WhenSizeIs1048576_SizeIsIs1MB()
        {
            long megabyte = 1024 * 1024;
            double inputSize = 1.0;
            string expected = "1.0 MB";
            var actual = Int64Extensions.SizeSuffix((long)(megabyte * inputSize));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WhenSizeIsGreaterThan1048576_SizeIsInMBs()
        {
            long megabyte = 1024 * 1024;
            double inputSize = 1.5;
            string expected = "1.5 MB";
            var actual = Int64Extensions.SizeSuffix((long)(megabyte * inputSize));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WhenSizeIs1073741824_SizeIsIs1GB()
        {
            long gigabyte = 1024 * 1024 * 1024;
            double inputSize = 1.0;
            string expected = "1.0 GB";
            var actual = Int64Extensions.SizeSuffix((long)(gigabyte * inputSize));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WhenSizeIsGreaterThan1073741824_SizeIsInGBs()
        {
            long gigabyte = 1024 * 1024 * 1024;
            double inputSize = 1.5;
            string expected = "1.5 GB";
            var actual = Int64Extensions.SizeSuffix((long)(gigabyte * inputSize));
            Assert.AreEqual(expected, actual);
        }
    }
}