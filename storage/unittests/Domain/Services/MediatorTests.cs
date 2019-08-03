using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using storage.Domain.Commands;
using storage.Domain.Models;
using storage.Domain.Queries;
using storage.Domain.Services;
using Xunit;

namespace unittests.Domain.Services
{
    public class MediatorTests
    {

        [Fact]
        public async void Send_routes_command_to_correct_service()
        {
            var mockedLogger = new Mock<ILogger<Mediator>>();
            var mockedStoreAnalyzedSentenceService = new Mock<IStoreAnalyzedSentenceService>();
            var mockedGetKeywordService = new Mock<IGetKeywordsService>();
            var mockedGetYearMonthService = new Mock<IGetYearMonthsService>();
            var mockedGetSentencesService = new Mock<IGetSentencesService>();

            mockedStoreAnalyzedSentenceService.Setup(s => s.StoreAsync(It.IsAny<AnalyzedSentence>()));
            
            var mediator = new Mediator(mockedLogger.Object, mockedStoreAnalyzedSentenceService.Object,
            mockedGetKeywordService.Object, mockedGetYearMonthService.Object, mockedGetSentencesService.Object);

            var command = new StoreAnalyzedSentenceCommand();
            command.AnalyzedSentence = new AnalyzedSentence() {Sentence = "This needs to be here"};

            await mediator.SendAsync(command);

            mockedStoreAnalyzedSentenceService.Verify(s => s.StoreAsync(It.IsAny<AnalyzedSentence>()), Times.Exactly(1));
        }

        [Fact]
        public async void Send_routes_GetSentencesQuery_to_correct_service_and_gets_expected_response()
        {
            var mockedLogger = new Mock<ILogger<Mediator>>();
            var mockedStoreAnalyzedSentenceService = new Mock<IStoreAnalyzedSentenceService>();
            var mockedGetKeywordService = new Mock<IGetKeywordsService>();
            var mockedGetYearMonthService = new Mock<IGetYearMonthsService>();
            var mockedGetSentencesService = new Mock<IGetSentencesService>();

            var sentences = new List<Sentence>();
            sentences.Add(new Sentence() {Text = "example sentence"});
            mockedGetSentencesService.Setup(s => s.GetAsync(It.IsAny<GetSentencesQuery>()))
            .ReturnsAsync(sentences);
            
            var mediator = new Mediator(mockedLogger.Object, mockedStoreAnalyzedSentenceService.Object,
            mockedGetKeywordService.Object, mockedGetYearMonthService.Object, mockedGetSentencesService.Object);

            var query = new GetSentencesQuery();
            

            var listOfSentences = await mediator.SendAsync<List<Sentence>>(query);
            mockedGetSentencesService.Verify(s => s.GetAsync(It.IsAny<GetSentencesQuery>()), Times.Exactly(1));
            Assert.True(listOfSentences.Count == 1);
            Assert.True(listOfSentences[0].Text == "example sentence");
           
        }

        [Fact]
        public async void Send_routes_GetAllKeywordsQuery_to_correct_service_and_gets_expected_response()
        {
            var mockedLogger = new Mock<ILogger<Mediator>>();
            var mockedStoreAnalyzedSentenceService = new Mock<IStoreAnalyzedSentenceService>();
            var mockedGetKeywordService = new Mock<IGetKeywordsService>();
            var mockedGetYearMonthService = new Mock<IGetYearMonthsService>();
            var mockedGetSentencesService = new Mock<IGetSentencesService>();

            var keywords = new List<Keyword>();
            keywords.Add(new Keyword() {Text = "Bananglass"});
            mockedGetKeywordService.Setup(s => s.GetAsync()).ReturnsAsync(keywords);
            
            var mediator = new Mediator(mockedLogger.Object, mockedStoreAnalyzedSentenceService.Object,
            mockedGetKeywordService.Object, mockedGetYearMonthService.Object, mockedGetSentencesService.Object);

            var query = new GetAllKeywordsQuery();
            

            var listOfKeywords = await mediator.SendAsync<List<Keyword>>(query);
            mockedGetKeywordService.Verify(s => s.GetAsync(), Times.Exactly(1));
            Assert.True(listOfKeywords.Count == 1);
            Assert.True(listOfKeywords[0].Text == "Bananglass");
           
        }

        [Fact]
        public async void Send_routes_GGetAllYearMonthsQuery_to_correct_service_and_gets_expected_response()
        {
            var mockedLogger = new Mock<ILogger<Mediator>>();
            var mockedStoreAnalyzedSentenceService = new Mock<IStoreAnalyzedSentenceService>();
            var mockedGetKeywordService = new Mock<IGetKeywordsService>();
            var mockedGetYearMonthService = new Mock<IGetYearMonthsService>();
            var mockedGetSentencesService = new Mock<IGetSentencesService>();

            var allYearMonths = new List<DateTime>();
            allYearMonths.Add(new DateTime(2019,07,07));
            mockedGetYearMonthService.Setup(s => s.GetAsync()).ReturnsAsync(allYearMonths);
            
            var mediator = new Mediator(mockedLogger.Object, mockedStoreAnalyzedSentenceService.Object,
            mockedGetKeywordService.Object, mockedGetYearMonthService.Object, mockedGetSentencesService.Object);

            var query = new GetAllYearMonthsQuery();
            

            var listOfYearMonhts = await mediator.SendAsync<List<DateTime>>(query);
            mockedGetYearMonthService.Verify(s => s.GetAsync(), Times.Exactly(1));
            Assert.True(listOfYearMonhts.Count == 1);
            Assert.True(listOfYearMonhts[0].Year == 2019);
            Assert.True(listOfYearMonhts[0].Month == 07);
            Assert.True(listOfYearMonhts[0].Day == 07);
           
        }
    }
}
