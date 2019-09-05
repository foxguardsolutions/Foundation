using System.Collections.Generic;

using AutoFixture;

using FGS.Pump.Tests.Support;
using FGS.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.ComponentModel.DataAnnotations.Tests
{
    [Unit]
    [TestFixture]
    public class CveAttributeTests : BaseUnitTest
    {
        private CveAttribute _subject;

        [SetUp]
        public void Setup()
        {
            _subject = Fixture.Create<CveAttribute>();
        }

        [TestCaseSource(nameof(FailureCases))]
        public void IsValid_GivenFailureCases_ReturnsFalse(string cve)
        {
            var results = _subject.IsValid(cve);

            Assert.False(results);
        }

        [TestCaseSource(nameof(SuccessCases))]
        public void IsValid_GivenSuccessCases_ReturnsTrue(string cve)
        {
            var results = _subject.IsValid(cve);

            Assert.True(results);
        }

        private static IEnumerable<string> FailureCases()
        {
            yield return "CVE19990067";
            yield return "CVE-19990067";
            yield return "CVE1999-0067";
            yield return "1999-0067";
            yield return "CEV-1999-0067";
            yield return "1999-0067-CVE";
            yield return "1999-6678";
            yield return "CVE-1999";
            yield return "CVE-1234-01111";
        }

        private static IEnumerable<string> SuccessCases()
        {
            yield return "CVE-1999-0067";
            yield return "CVE-1999-6678";
            yield return "CVE-1234-11111";
        }
    }
}
