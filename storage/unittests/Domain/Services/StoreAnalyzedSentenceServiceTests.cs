using Microsoft.Extensions.Logging;
using Moq;
using storage.Domain.Models;
using storage.Domain.Ports.Outbound;
using storage.Domain.Services;
using Xunit;

namespace unittests.Domain.Services
{
    public class StoreAnalyzedSentenceServiceTests
    {
        [Fact]
        public async void StoreAsync_makes_sure_keyword_and_sources_exists_before_saving_sentence()
        {
            var mockedDatabaseAccess = new Mock<IDatabaseAccess>();
            var mockedLogger = new Mock<ILogger<StoreAnalyzedSentenceService>>();
            
            mockedDatabaseAccess.Setup(db => db.EnsureExistAsync(It.IsAny<Source>())).ReturnsAsync(0);
            mockedDatabaseAccess.Setup(db => db.EnsureExistAsync(It.IsAny<Keyword>())).ReturnsAsync(0);
            mockedDatabaseAccess.Setup(db => db.SaveSentenceAsync(It.IsAny<Sentence>())).ReturnsAsync((sentenceId: 0, saved: true));

            var service = new StoreAnalyzedSentenceService(mockedDatabaseAccess.Object, mockedLogger.Object);
            var sentence = new AnalyzedSentence();
            await service.StoreAsync(sentence);

            mockedDatabaseAccess.Verify(db => db.EnsureExistAsync(It.IsAny<Source>()), Times.Exactly(1));
            mockedDatabaseAccess.Verify(db => db.EnsureExistAsync(It.IsAny<Keyword>()), Times.Exactly(1));
            mockedDatabaseAccess.Verify(db => db.SaveSentenceAsync(It.IsAny<Sentence>()), Times.Exactly(1));

        }
    }
}