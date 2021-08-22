using TrueLayer.Api.Utilities;
using Xunit;

namespace TrueLayer.Api.Tests.Utilities
{
    public class StringUtilitiesTests
    {
        [Fact]
        public void CompactWhitespace_NoWhitespaceInString_Untouched()
        {
            var candidate = "abcdefgh";
            var result = candidate.CompactWhitespace();
            Assert.Equal(candidate, result);
        }

        [Fact]
        public void CompactWhitespace_SingleSpacesInString_AllLeftUntouched()
        {
            var candidate = "a b c d ef gh";
            var result = candidate.CompactWhitespace();
            Assert.Equal(candidate, result);
        }

        [Fact]
        public void CompactWhitespace_DuplicateSpaces_AreCompactedToOne()
        {
            var candidate = "a b  c   d    e    f";
            var result = candidate.CompactWhitespace();
            Assert.Equal("a b c d e f", result);
        }

        [Theory]
        [InlineData("  ")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\t")]
        [InlineData(" \n \r\t")]
        public void CompactWhitespace_RemovesMultipleCharacters(string whitespace)
        {
            var candidate = $"a{whitespace}b";
            var result = candidate.CompactWhitespace();
            Assert.Equal("a b", result);
        }
    }
}