using FGS.Pump.Configuration.Environment;
using FGS.Pump.Tests.Support.TestCategories;

using NUnit.Framework;

namespace FGS.Pump.Configuration.Tests.Environment
{
    [TestFixture]
    [Unit]
    public class EnvironmentKeySplitConnectionStringAdaptationStrategyTests
    {
        private const string Separator = EnvironmentKeySplitConnectionStringAdaptationStrategy.Separator;
        private const string Prefix = EnvironmentKeySplitConnectionStringAdaptationStrategy.Prefix;
        private const string ValueSuffix = EnvironmentKeySplitConnectionStringAdaptationStrategy.ValueSuffix;
        private const string ProviderSuffix = EnvironmentKeySplitConnectionStringAdaptationStrategy.ProviderSuffix;

        private const string SampleConnectionStringName = "SAMPLE";

        public const string SampleConnectionStringValueUnderlyingKey = Prefix + Separator + SampleConnectionStringName + Separator + ValueSuffix;
        public const string SampleConnectionStringProviderUnderlyingKey = Prefix + Separator + SampleConnectionStringName + Separator + ProviderSuffix;
        private const string SampleIrrelevantKeyWithSeparatorsButNoPrefix = "IRRELEVANT" + Separator + "KEY";

        private readonly EnvironmentKeySplitConnectionStringAdaptationStrategy _subject = new EnvironmentKeySplitConnectionStringAdaptationStrategy();

        [TestCase(SampleConnectionStringValueUnderlyingKey, ExpectedResult = "PUMP_SAMPLE_ConnectionString")]
        [TestCase(SampleConnectionStringProviderUnderlyingKey, ExpectedResult = "PUMP_SAMPLE_Provider")]
        [TestCase(SampleIrrelevantKeyWithSeparatorsButNoPrefix, ExpectedResult = "IRRELEVANT_KEY")]
        public string SampleKeyConstructionIsExpected(string key)
        {
            return key;
        }

        [TestCase(SampleConnectionStringValueUnderlyingKey, ExpectedResult = true)]
        [TestCase(SampleConnectionStringProviderUnderlyingKey, ExpectedResult = false)]
        [TestCase(SampleIrrelevantKeyWithSeparatorsButNoPrefix, ExpectedResult = false)]
        public bool IsConnectionStringValueUnderlyingKey_GivenInput_ReturnsExpected(string input)
        {
            return _subject.IsConnectionStringValueUnderlyingKey(input);
        }

        [TestCase(SampleConnectionStringValueUnderlyingKey, ExpectedResult = false)]
        [TestCase(SampleConnectionStringProviderUnderlyingKey, ExpectedResult = true)]
        [TestCase(SampleIrrelevantKeyWithSeparatorsButNoPrefix, ExpectedResult = false)]
        public bool IsConnectionStringProviderUnderlyingKey_GivenInput_ReturnsExpected(string input)
        {
            return _subject.IsConnectionStringProviderUnderlyingKey(input);
        }

        [TestCase(SampleConnectionStringValueUnderlyingKey, ExpectedResult = SampleConnectionStringName)]
        public string ConvertToConnectionStringNameFromValueUnderlyingKey_GivenInput_ReturnsExpected(string input)
        {
            return _subject.ConvertToConnectionStringNameFromValueUnderlyingKey(input);
        }

        [TestCase(SampleConnectionStringProviderUnderlyingKey, ExpectedResult = SampleConnectionStringName)]
        public string ConvertToConnectionStringNameFromProviderUnderlyingKey_GivenInput_ReturnsExpected(string input)
        {
            return _subject.ConvertToConnectionStringNameFromProviderUnderlyingKey(input);
        }
    }
}
