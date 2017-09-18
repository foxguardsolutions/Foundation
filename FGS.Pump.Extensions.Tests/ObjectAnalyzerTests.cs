using System.Dynamic;

using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.TestCategories;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace FGS.Pump.Extensions.Tests
{
    [Unit]
    [TestFixture]
    public class ObjectAnalyzerTests
    {
        private string _propertyName;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = AutoFixtureFactory.Create();
            _propertyName = _fixture.Create<string>();
        }

        [Test]
        public void HasProperty_GivenNameOfFunctionOnObject_ReturnsFalse()
        {
            var obj = new TestObject();

            var actual = ObjectAnalyzer.HasProperty(obj, nameof(obj.TestFunction));

            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasProperty_GivenNameOfPropertyOnAnonymousObject_ReturnsTrue()
        {
            var aObj = new { Property = true };

            var actual = ObjectAnalyzer.HasProperty(aObj, nameof(aObj.Property));

            Assert.That(actual, Is.True);
        }

        [Test]
        public void HasProperty_GivenNameOfPropertyOnExpandoObject_ReturnsTrue()
        {
            dynamic eObj = new ExpandoObject();
            eObj.Property = true;

            var actual = ObjectAnalyzer.HasProperty(eObj, nameof(eObj.Property));

            Assert.That(actual, Is.True);
        }

        [Test]
        public void HasProperty_GivenNameOfPropertyOnJObject_ReturnsTrue()
        {
            var jObj = new JObject
            {
                [_propertyName] = true
            };

            var actual = ObjectAnalyzer.HasProperty(jObj, _propertyName);

            Assert.That(actual, Is.True);
        }

        [Test]
        public void HasProperty_GivenNameOfPropertyOnObject_ReturnsTrue()
        {
            var obj = new TestObject();

            var actual = ObjectAnalyzer.HasProperty(obj, nameof(obj.TestProperty));

            Assert.That(actual, Is.True);
        }

        [Test]
        public void HasProperty_GivenNameOfPropertyNotOnAnonymousObject_ReturnsFalse()
        {
            var aObj = new { Property = true };

            var actual = ObjectAnalyzer.HasProperty(aObj, _propertyName);

            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasProperty_GivenNameOfPropertyOnExpandoObject_ReturnsFalse()
        {
            dynamic eObj = new ExpandoObject();

            var actual = ObjectAnalyzer.HasProperty(eObj, _propertyName);

            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasProperty_GivenNameOfPropertyNotOnJObject_ReturnsFalse()
        {
            var jObj = new JObject();

            var actual = ObjectAnalyzer.HasProperty(jObj, _propertyName);

            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasProperty_GivenNameOfPropertyNotOnObject_ReturnsFalse()
        {
            var obj = new TestObject();

            var actual = ObjectAnalyzer.HasProperty(obj, _propertyName);

            Assert.That(actual, Is.False);
        }

        private class TestObject
        {
            public string TestProperty { get; set; }

            public void TestFunction()
            {
            }
        }
    }
}