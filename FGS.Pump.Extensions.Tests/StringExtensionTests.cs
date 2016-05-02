using System.Collections.Generic;
using System.Linq;

using FGS.Pump.Tests.Support;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace FGS.Pump.Extensions.Tests
{
    [TestFixture]
    public class StringExtensionTests : BaseUnitTest
    {
        [Test]
        public void TrimLeadingWhitespaceAndChars_RemovesLeadingCharacters()
        {
            var characters = Fixture.Create<char[]>();
            var baseString = Fixture.Create<string>();
            var stringWithCharacterToBeRemoved = new string(characters) + baseString;

            var trimmedString = stringWithCharacterToBeRemoved.TrimWhitespaceAndCharacters(characters);

            AssertStringIsTrimmed(trimmedString, characters);
        }

        [Test]
        [TestCaseSource(nameof(WhitespaceCharacters))]
        public void TrimLeadingWhitespaceAndChars_RemovesWhitespace(char whitespaceCharacter)
        {
            var baseString = Fixture.Create<string>();
            var stringWithCharacterToBeRemoved = whitespaceCharacter + baseString;

            var trimmedString = stringWithCharacterToBeRemoved.TrimWhitespaceAndCharacters(new[] { whitespaceCharacter });

            AssertStringIsTrimmed(trimmedString, new[] { whitespaceCharacter });
        }

        [Test]
        public void TrimLeadingWhitespaceAndChars_RemovesTrailingCharacters()
        {
            var characters = Fixture.Create<char[]>();
            var baseString = Fixture.Create<string>();
            var stringWithCharacterToBeRemoved = baseString + new string(characters);

            var trimmedString = stringWithCharacterToBeRemoved.TrimWhitespaceAndCharacters(characters);

            AssertStringIsTrimmed(trimmedString, characters);
        }

        [Test]
        [TestCaseSource(nameof(WhitespaceCharacters))]
        public void TrimWhitespaceAndChars_RemovesTrailingWhitespace(char whitespaceCharacter)
        {
            var baseString = Fixture.Create<string>();
            var stringWithCharacterToBeRemoved = baseString + whitespaceCharacter;

            var trimmedString = stringWithCharacterToBeRemoved.TrimWhitespaceAndCharacters(new[] { whitespaceCharacter });

            AssertStringIsTrimmed(trimmedString, new[] { whitespaceCharacter });
        }

        private void AssertStringIsTrimmed(string stringToTest, char[] characters)
        {
            characters.ToList().ForEach(c => Assert.AreNotSame(c, stringToTest.First()));
            characters.ToList().ForEach(c => Assert.AreNotSame(c, stringToTest.Last()));
        }

        public static IEnumerable<char> WhitespaceCharacters()
        {
            yield return ' ';
            yield return '\u0004'; //// line feed
            yield return '\u000D'; //// carriage return
            yield return '\u0009'; //// character tabulation
            yield return '\u2028'; //// line separator
        }

        [Test]
        public void Reverse_ReversesCharactersInString()
        {
            var stringToReverse = Fixture.Create<string>();

            var reversedString = stringToReverse.Reverse();

            Assert.True(reversedString.SequenceEqual(stringToReverse.ToCharArray().Reverse()));
        }
    }
}
