using System.Collections.Generic;

using NUnit.Framework;

namespace FGS.Pump.Extensions.Tests
{
    [TestFixture]
    public class CollectionExtensionsTests
    {
        [TestCaseSource(nameof(TakeLastTestCases))]
        public void TakeLast_ReturnsExpectedResults(ICollection<object> enumerable, int count, ICollection<object> expected)
        {
            var actual = enumerable.TakeLast(count);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private static IEnumerable<TestCaseData> TakeLastTestCases()
        {
            yield return new TestCaseData(new List<object> { "a", "b", "c", "d", "e", "f" }, 3, new List<object> { "d", "e", "f" });
            yield return new TestCaseData(new[] { "f", "e", "d", "c", "b", "a" }, 2, new[] { "b", "a" });
            yield return new TestCaseData(new List<object> { "abc", "def", "456", "123", "ghi", "789" }, 4, new List<object> { "456", "123", "ghi", "789" });
            yield return new TestCaseData(new List<object> { 1, 9, 2, 8, 3, 7, 4, 6, 5 }, 5, new List<object> { 3, 7, 4, 6, 5 });
            yield return new TestCaseData(new List<object> { 1, "a", 2, "b", 3, "c", 4, "d", 5 }, 6, new List<object> { "b", 3, "c", 4, "d", 5 });
        }
    }
}