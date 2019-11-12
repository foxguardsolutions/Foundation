using System;
using System.Collections.Generic;

using AutoFixture;

using FGS.Tests.Support;
using FGS.Tests.Support.TestCategories;

using FluentAssertions;

using NUnit.Framework;

namespace FGS.ComponentModel.DataAnnotations.Tests
{
    [Unit]
    [TestFixture]
    public class RequiredIfTrueAttributeTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCaseSource(nameof(CommonPropertyTypeTestCases))]
        public void IsValidGivenDependentPropertyValue_GivenInputs_ReturnsExpected<T>(T targetPropertyValue, bool dependentPropertyValue, bool expectedIsValid)
        {
            CallIsValidGivenDependentPropertyValue(targetPropertyValue, dependentPropertyValue).Should().Be(expectedIsValid);
        }

        [Test]
        [TestCaseSource(nameof(TargetValueNulledTestCases))]
        public void IsValidGivenDependentPropertyValue_GivenTargetValueNulled_ReturnsExpected(bool dependentPropertyValue, bool expectedIsValid)
        {
            CallIsValidGivenDependentPropertyValue(default(object), dependentPropertyValue).Should().Be(expectedIsValid);
        }

        public static IEnumerable<TestCaseData> CommonPropertyTypeTestCases()
        {
            var fixture = AutoFixtureFactory.Create();

            yield return new TestCaseData(fixture.Create<string>(), true, true);
            yield return new TestCaseData(fixture.Create<int>(), true, true);
            yield return new TestCaseData(fixture.Create<Guid>(), true, true);
            yield return new TestCaseData(fixture.Create<int?>(), true, true);
            yield return new TestCaseData(fixture.Create<Guid?>(), true, true);

            yield return new TestCaseData(fixture.Create<string>(), false, true);
            yield return new TestCaseData(fixture.Create<int>(), false, true);
            yield return new TestCaseData(fixture.Create<Guid>(), false, true);
            yield return new TestCaseData(fixture.Create<int?>(), false, true);
            yield return new TestCaseData(fixture.Create<Guid?>(), false, true);
        }

        public static IEnumerable<TestCaseData> TargetValueNulledTestCases()
        {
            yield return new TestCaseData(true, false);
            yield return new TestCaseData(false, true);
        }

        private bool CallIsValidGivenDependentPropertyValue<T>(T targetPropertyValue, bool dependentPropertyValue)
        {
            return RequiredIfTrueAttribute.IsValidGivenDependentPropertyValue(targetPropertyValue, dependentPropertyValue);
        }

        public class TestModel<T>
        {
            public T TargetProperty { get; set; }

            public bool DependentProperty { get; set; }
        }
    }
}
