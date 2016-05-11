using System.Collections.Generic;

using FGS.Pump.Extensions.ComponentModel.DataAnnotations;
using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace FGS.Pump.Extensions.Tests.ComponentModel.DataAnnotations
{
    [Unit]
    [TestFixture]
    public class CveListAttributeTests : BaseUnitTest
    {
        private CveListAttribute _subject;

        [SetUp]
        public void Setup()
        {
            _subject = Fixture.Create<CveListAttribute>();
        }

        [TestCaseSource(nameof(FailureCases))]
        public void IsValid_GivenFailureCases_ReturnsFalse(string cveList)
        {
            var results = _subject.IsValid(cveList);

            Assert.False(results);
        }

        [TestCaseSource(nameof(SuccessCases))]
        public void IsValid_GivenSuccessCases_ReturnsTrue(string cveList)
        {
            var results = _subject.IsValid(cveList);

            Assert.True(results);
        }

        public static IEnumerable<string> FailureCases()
        {
            yield return "CVE19990067";
            yield return "CVE-19990067";
            yield return "CVE1999-0067";
            yield return "1999-0067";
            yield return "CEV-1999-0067";
            yield return "1999-0067-CVE";
            yield return "CVE-1999-0067, 1999-6678";
            yield return "1999-6678, CVE-1999-0067";
            yield return "CVE-1999-0067, CEV-1999-6678";
            yield return "CVE-1999";
        }

        public static IEnumerable<string> SuccessCases()
        {
            yield return "CVE-1999-0067";
            yield return "CVE-1999-0067, CVE-1999-6678";
            yield return "CVE-1999-0067,CVE-1999-6678";
            yield return "CVE-1999-6678, CVE-1999-0067";
            yield return "CVE-1999-6678,CVE-1999-0067";
            yield return "CVE-1999-6678, CVE-1999-0067, CVE-2016-0050";
        }
    }
}
