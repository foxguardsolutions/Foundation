using System.Linq;

namespace FGS.Primitives.Extensions
{
    public static class StringExtension
    {
        public static string TrimWhitespaceAndCharacters(this string stringToTrim, char[] charsToTrim)
        {
            var leftTrimmed = TrimLeadingWhitespaceAndCharacters(stringToTrim, charsToTrim);
            return TrimLeadingWhitespaceAndCharacters(leftTrimmed.Reverse(), charsToTrim).Reverse();
        }

        public static string TrimLeadingWhitespaceAndCharacters(this string stringToTrim, char[] charsToTrim)
        {
            return new string(stringToTrim.SkipWhile(c => IsRemovalTarget(c, charsToTrim)).ToArray());
        }

        private static bool IsRemovalTarget(char candidate, char[] targetChars)
        {
            return char.IsWhiteSpace(candidate) || targetChars.Contains(candidate);
        }

        public static string Reverse(this string stringToReverse)
        {
            return new string(stringToReverse.ToCharArray().Reverse().ToArray());
        }
    }
}
