using Microsoft.Extensions.Logging;
using Moq;
using tracker.Domain.Services;
using Xunit;

namespace unittests.Domain.Services
{
    public class ValidatorTests
    {
        [Fact]
        public void ConsideredArticleHeader_fails_if_invalid()
        {
            var loggerMock = new Mock<ILogger<Validator>>();
            var validator = new Validator(loggerMock.Object);
            var result = validator.ConsideredArticleHeader("VISA FLER"); // known invalid article header
            Assert.False(result);
        }
        [Fact]
        public void ConsideredArticleHeader_passes_if_valid()
        {
            var loggerMock = new Mock<ILogger<Validator>>();
            var validator = new Validator(loggerMock.Object);
            var result = validator.ConsideredArticleHeader("This is a valid header");
            Assert.True(result);
        }
        [Fact]
        public void ConsideredBodyText_fails_if_more_than_one_star()
        {
            var loggerMock = new Mock<ILogger<Validator>>();
            var validator = new Validator(loggerMock.Object);
            var result = validator.ConsideredBodyText("This is a sentence with more than one star * *");
            Assert.False(result);
        }

        [Fact]
        public void ConsideredBodyText_fails_if_more_than_one_arrow()
        {
            var loggerMock = new Mock<ILogger<Validator>>();
            var validator = new Validator(loggerMock.Object);
            var result = validator.ConsideredBodyText("This is a sentence with more than one star ► ► ►");
            Assert.False(result);
        }

        [Fact]
        public void ConsideredBodyText_fails_if_more_than_three_dashes()
        {
            var loggerMock = new Mock<ILogger<Validator>>();
            var validator = new Validator(loggerMock.Object);
            var result = validator.ConsideredBodyText("This is a sentence with more than three -dashes ---");
            Assert.False(result);
        }

        [Fact]
        public void ConsideredBodyText_fails_if_more_than_three_pluses()
        {
            var loggerMock = new Mock<ILogger<Validator>>();
            var validator = new Validator(loggerMock.Object);
            var result = validator.ConsideredBodyText("This is a sentence with more than three pluses ++++");
            Assert.False(result);
        }

        [Fact]
        public void ConsideredBodyText_fails_if_more_than_a_third_uppercase()
        {
            var loggerMock = new Mock<ILogger<Validator>>();
            var validator = new Validator(loggerMock.Object);
            var result = validator.ConsideredBodyText("This is a SENTENCE WITH A THIRD OR MORE UPPERCASE CHARS");
            Assert.False(result);
        }

        [Fact]
        public void ConsideredBodyText_fails_if_contains_known_non_body_text()
        {
            var loggerMock = new Mock<ILogger<Validator>>();
            var validator = new Validator(loggerMock.Object);
            var knownNonBodyTextSentence = "Vi vill informera dig om vår policy som beskriver hur vi behandlar personuppgifter och cookies";
            var result = validator.ConsideredBodyText(knownNonBodyTextSentence);
            Assert.False(result);
        }

        [Fact]
        public void ConsideredBodyText_passes_if_valid_body_text_sentence()
        {
            var loggerMock = new Mock<ILogger<Validator>>();
            var validator = new Validator(loggerMock.Object);
            var result = validator.ConsideredBodyText("this is a completely valid sentence, dont you think?");
            Assert.True(result);
        }


    }
}
