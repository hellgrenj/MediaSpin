using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Moq;
using tracker.Domain.Models;
using tracker.Domain.Ports.Outbound;
using tracker.Domain.Services;
using Xunit;

namespace unittests.Domain.Services
{
    public class TrackerTests
    {
        [Fact]
        public void Scan_searches_by_baseurl_for_4_header_levels()
        {
            var pipelineMock = new Mock<IPipeline>();
            var loggerMock = new Mock<ILogger<Tracker>>();
            var validatorMock = new Mock<IValidator>();
            var extractorMock = new Mock<IExtractor>();
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.LoadPage(It.IsAny<string>()));
            spiderMock.Setup(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>())).Returns(new HtmlNodeCollection(null));
            
            var tracker = new Tracker(pipelineMock.Object, spiderMock.Object,
            loggerMock.Object, validatorMock.Object, extractorMock.Object);

            var baseUrlInTest = "http://madeupnews.com";
            tracker.Scan(baseUrlInTest, new List<Article>());

            var numberOfHeaderLevels = 4; // all levels will return the one header in this test...
            spiderMock.Verify(s => s.LoadPage(baseUrlInTest), Times.Exactly(numberOfHeaderLevels));
        }

        [Fact]
        public void Scan_doesnt_download_article_with_invalid_header()
        {
            var pipelineMock = new Mock<IPipeline>();
            var loggerMock = new Mock<ILogger<Tracker>>();
            var extractorMock = new Mock<IExtractor>();
            
            var validatorMock = new Mock<IValidator>();
            validatorMock.Setup(v => v.ConsideredArticleHeader(It.IsAny<string>())).Returns(false);
            
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.LoadPage(It.IsAny<string>()));
            
            var headerNode = new HtmlNode(HtmlNodeType.Element, new HtmlDocument(), 0);
            headerNode.InnerHtml = "<h1>doesnt matter - mocked</h1>"; //known invalid article header

            var headers = new HtmlNodeCollection(null);
            headers.Add(headerNode);

            spiderMock.Setup(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>())).Returns(headers);
            spiderMock.Setup(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>())).Returns((null, null));
            
            var tracker = new Tracker(pipelineMock.Object, spiderMock.Object, loggerMock.Object,
            validatorMock.Object, extractorMock.Object);
          
            var baseUrlInTest = "http://madeupnews.com";
            tracker.Scan(baseUrlInTest, new List<Article>());

            var numberOfHeaderLevels = 4; // all levels will return the one header in this test...            
            spiderMock.Verify(s => s.LoadPage(baseUrlInTest), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>()), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>()), Times.Exactly(0));
        }

        [Fact]
        public void Scan_doesnt_send_article_for_analysis_if_download_fails()
        {
            var extractorMock = new Mock<IExtractor>();
            var loggerMock = new Mock<ILogger<Tracker>>();
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.LoadPage(It.IsAny<string>()));

            var headerNode = new HtmlNode(HtmlNodeType.Element, new HtmlDocument(), 0);
            headerNode.InnerHtml = "Some valid article header";
            var headers = new HtmlNodeCollection(null);
            headers.Add(headerNode);
            spiderMock.Setup(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>())).Returns(headers);
            spiderMock.Setup(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>())).Returns((null, null));
            
            var pipelineMock = new Mock<IPipeline>();
            pipelineMock.Setup(p => p.SendForAnalysis(It.IsAny<Article>()));
            
            var validatorMock = new Mock<IValidator>();
            validatorMock.Setup(v => v.ConsideredArticleHeader(It.IsAny<string>())).Returns(true);

            var tracker = new Tracker(pipelineMock.Object, spiderMock.Object, loggerMock.Object,
            validatorMock.Object, extractorMock.Object);
           
            var baseUrlInTest = "http://madeupnews.com";
            tracker.Scan(baseUrlInTest, new List<Article>());

            var numberOfHeaderLevels = 4; // recursive for four header levels h1, h2, h3 and h4
            // all levels will return the one header in this test...
            spiderMock.Verify(s => s.LoadPage(baseUrlInTest), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>()), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>()), Times.Exactly(numberOfHeaderLevels));
            pipelineMock.Verify(p => p.SendForAnalysis(It.IsAny<Article>()), Times.Exactly(0));
        }

        [Fact]
        public void Scan_doesnt_send_article_for_analysis_if_text_extraction_fails()
        {
            var loggerMock = new Mock<ILogger<Tracker>>();
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.LoadPage(It.IsAny<string>()));
            var headerNode = new HtmlNode(HtmlNodeType.Element, new HtmlDocument(), 0);
            headerNode.InnerHtml = "Some valid article header";

            var headers = new HtmlNodeCollection(null);
            headers.Add(headerNode);

            spiderMock.Setup(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>())).Returns(headers);
            spiderMock.Setup(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>())).Returns(("", new HtmlDocument()));
            var pipelineMock = new Mock<IPipeline>();
            pipelineMock.Setup(p => p.SendForAnalysis(It.IsAny<Article>()));
            
            var validatorMock = new Mock<IValidator>();
            validatorMock.Setup(v => v.ConsideredArticleHeader(It.IsAny<string>())).Returns(true);
            var extractorMock = new Mock<IExtractor>();
            extractorMock.Setup(e => e.ExtractBodyTextFromArticleDocument(It.IsAny<HtmlDocument>())).Throws(new Exception("something bad happend"));
           
            var tracker = new Tracker(pipelineMock.Object, spiderMock.Object, loggerMock.Object,
            validatorMock.Object, extractorMock.Object);
           
            var baseUrlInTest = "http://madeupnews.com";
            tracker.Scan(baseUrlInTest, new List<Article>());

            
            var numberOfHeaderLevels = 4; // recursive for four header levels h1, h2, h3 and h4
            // all levels will return the one header in this test...

            spiderMock.Verify(s => s.LoadPage(baseUrlInTest), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>()), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>()), Times.Exactly(numberOfHeaderLevels));
            extractorMock.Verify(e => e.ExtractBodyTextFromArticleDocument(It.IsAny<HtmlDocument>()), Times.Exactly(numberOfHeaderLevels));
            pipelineMock.Verify(p => p.SendForAnalysis(It.IsAny<Article>()), Times.Exactly(0));
        }

        
        [Fact]
        public void Scan_doesnt_send_article_for_analysis_if_no_keyword_found()
        {
            var loggerMock = new Mock<ILogger<Tracker>>();
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.LoadPage(It.IsAny<string>()));
            var headerNode = new HtmlNode(HtmlNodeType.Element, new HtmlDocument(), 0);
            headerNode.InnerHtml = "Some valid article header";

            var headers = new HtmlNodeCollection(null);
            headers.Add(headerNode);

            spiderMock.Setup(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>())).Returns(headers);
            spiderMock.Setup(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>())).Returns(("", new HtmlDocument()));
            var pipelineMock = new Mock<IPipeline>();
            pipelineMock.Setup(p => p.SendForAnalysis(It.IsAny<Article>()));
            
            var validatorMock = new Mock<IValidator>();
            validatorMock.Setup(v => v.ConsideredArticleHeader(It.IsAny<string>())).Returns(true);
            var extractorMock = new Mock<IExtractor>();

            var extractedBodyText = @"this body text was extracted from an html document 
            but it doesnt contains any keywords we are serching for";

            extractorMock.Setup(e => e.ExtractBodyTextFromArticleDocument(It.IsAny<HtmlDocument>())).Returns(extractedBodyText);
           
            var tracker = new Tracker(pipelineMock.Object, spiderMock.Object, loggerMock.Object,
            validatorMock.Object, extractorMock.Object);
           
            var baseUrlInTest = "http://madeupnews.com";
            tracker.Scan(baseUrlInTest, new List<Article>());

            
            var numberOfHeaderLevels = 4; // recursive for four header levels h1, h2, h3 and h4
            // all levels will return the one header in this test...

            spiderMock.Verify(s => s.LoadPage(baseUrlInTest), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>()), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>()), Times.Exactly(numberOfHeaderLevels));
            extractorMock.Verify(e => e.ExtractBodyTextFromArticleDocument(It.IsAny<HtmlDocument>()), Times.Exactly(numberOfHeaderLevels));
            pipelineMock.Verify(p => p.SendForAnalysis(It.IsAny<Article>()), Times.Exactly(0));
        }

        [Fact]
        public void Scan_sends_article_for_analysis_if_keyword_found()
        {
            var loggerMock = new Mock<ILogger<Tracker>>();
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.LoadPage(It.IsAny<string>()));
            var headerNode = new HtmlNode(HtmlNodeType.Element, new HtmlDocument(), 0);
            headerNode.InnerHtml = "Some valid article header";

            var headers = new HtmlNodeCollection(null);
            headers.Add(headerNode);

            spiderMock.Setup(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>())).Returns(headers);
            spiderMock.Setup(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>())).Returns(("", new HtmlDocument()));
            var pipelineMock = new Mock<IPipeline>();
            pipelineMock.Setup(p => p.SendForAnalysis(It.IsAny<Article>()));
            
            var validatorMock = new Mock<IValidator>();
            validatorMock.Setup(v => v.ConsideredArticleHeader(It.IsAny<string>())).Returns(true);
            var extractorMock = new Mock<IExtractor>();

            var extractedBodyText = @"this body text was extracted from an html document 
            and it contains the default keyword Sverige which we know is a keyword since no env var KEYWORDS was set";

            extractorMock.Setup(e => e.ExtractBodyTextFromArticleDocument(It.IsAny<HtmlDocument>())).Returns(extractedBodyText);
           
            var tracker = new Tracker(pipelineMock.Object, spiderMock.Object, loggerMock.Object,
            validatorMock.Object, extractorMock.Object);

            var baseUrlInTest = "http://madeupnews.com";
            tracker.Scan(baseUrlInTest, new List<Article>());

            
            var numberOfHeaderLevels = 4; // recursive for four header levels h1, h2, h3 and h4
            // all levels will return the one header in this test...

            spiderMock.Verify(s => s.LoadPage(baseUrlInTest), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.GetHeadersOfSize(It.IsAny<HtmlDocument>(), It.IsAny<int>()), Times.Exactly(numberOfHeaderLevels));
            spiderMock.Verify(s => s.DownloadArticleByHeader(It.IsAny<string>(), It.IsAny<HtmlNode>()), Times.Exactly(numberOfHeaderLevels));
            extractorMock.Verify(e => e.ExtractBodyTextFromArticleDocument(It.IsAny<HtmlDocument>()), Times.Exactly(numberOfHeaderLevels));
            pipelineMock.Verify(p => p.SendForAnalysis(It.IsAny<Article>()), Times.Exactly(numberOfHeaderLevels));
        }
    }
}
