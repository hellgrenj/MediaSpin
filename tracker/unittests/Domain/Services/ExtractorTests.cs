using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Moq;
using tracker.Domain.Models;
using tracker.Domain.Ports.Outbound;
using tracker.Domain.Services;
using Xunit;

namespace unittests.Domain.Services
{
    public class ExtractorTests
    {
        [Fact]
        public void ExtractBodyTextFromArticleDocumentusing_removes_headers_from_html_document_and_converts_to_text()
        {
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.HtmlToTextAsync(It.IsAny<string>())).ReturnsAsync("converted to text");
            spiderMock.Setup(s => s.RemoveNodesFromDocument(It.IsAny<HtmlDocument>(),
            It.IsAny<string>())).Callback((HtmlDocument document, string expression) =>
            {
                var loggerMock = new Mock<ILogger<Spider>>();
                var nodeservicesMock = new Mock<INodeServices>();
                var realSpider = new Spider(loggerMock.Object, nodeservicesMock.Object);
                realSpider.RemoveNodesFromDocument(document, expression);
            });
            var validatorMock = new Mock<IValidator>();
            validatorMock.Setup(v => v.ConsideredBodyText(It.IsAny<string>())).Returns(true);
            var extractor = new Extractor(spiderMock.Object, validatorMock.Object);
            var doc = new HtmlDocument();
            var html = HtmlNode.CreateNode(@"
            <html>
            <body>
            <h1>rubrik</h1>
            <h2>rubrik</h2>
            <h3>rubrik</h3>
            <h4>rubrik</h4>
            vanlig brödtext
            </body>
            </html>
            ");
            doc.DocumentNode.AppendChild(html);

            var mockedConvertedText = extractor.ExtractBodyTextFromArticleDocument(doc);
            var processedHtmlDoc = doc.DocumentNode.OuterHtml;
            Assert.Equal("converted to text", mockedConvertedText);
            Assert.Contains("vanlig brödtext", processedHtmlDoc);
            Assert.DoesNotContain("<h1>rubrik</h1>", processedHtmlDoc);
            Assert.DoesNotContain("<h2>rubrik</h2>", processedHtmlDoc);
            Assert.DoesNotContain("<h3>rubrik</h3>", processedHtmlDoc);
            Assert.DoesNotContain("<h4>rubrik</h4>", processedHtmlDoc);
            spiderMock.Verify(s => s.HtmlToTextAsync(It.IsAny<string>()), Times.Exactly(1));
            spiderMock.Verify(s => s.RemoveNodesFromDocument(It.IsAny<HtmlDocument>(), It.IsAny<string>()), Times.AtLeastOnce());
        }

        [Fact]
        public void ExtractBodyTextFromArticleDocumentusing_removes_lists_from_html_document_and_converts_to_text()
        {
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.HtmlToTextAsync(It.IsAny<string>())).ReturnsAsync("converted to text");
            spiderMock.Setup(s => s.RemoveNodesFromDocument(It.IsAny<HtmlDocument>(),
            It.IsAny<string>())).Callback((HtmlDocument document, string expression) =>
            {
                var loggerMock = new Mock<ILogger<Spider>>();
                var nodeservicesMock = new Mock<INodeServices>();
                var realSpider = new Spider(loggerMock.Object, nodeservicesMock.Object);
                realSpider.RemoveNodesFromDocument(document, expression);
            });
            var validatorMock = new Mock<IValidator>();
            validatorMock.Setup(v => v.ConsideredBodyText(It.IsAny<string>())).Returns(true);
            var extractor = new Extractor(spiderMock.Object, validatorMock.Object);
            var doc = new HtmlDocument();
            var html = HtmlNode.CreateNode(@"
            <html>
            <body>
           <ul>
            <li>some list</li>
           </ul>
            vanlig brödtext
            </body>
            </html>
            ");
            doc.DocumentNode.AppendChild(html);

            var mockedConvertedText = extractor.ExtractBodyTextFromArticleDocument(doc);
            var processedHtmlDoc = doc.DocumentNode.OuterHtml;
            Assert.Equal("converted to text", mockedConvertedText);
            Assert.Contains("vanlig brödtext", processedHtmlDoc);
            Assert.DoesNotContain("<ul>", processedHtmlDoc);
            Assert.DoesNotContain("</ul>", processedHtmlDoc);
            Assert.DoesNotContain("<li>some list</li>", processedHtmlDoc);
            spiderMock.Verify(s => s.HtmlToTextAsync(It.IsAny<string>()), Times.Exactly(1));
            spiderMock.Verify(s => s.RemoveNodesFromDocument(It.IsAny<HtmlDocument>(), It.IsAny<string>()), Times.AtLeastOnce());
        }

        [Fact]
        public void ExtractBodyTextFromArticleDocumentusing_removes_scripts_from_html_document_and_converts_to_text()
        {
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.HtmlToTextAsync(It.IsAny<string>())).ReturnsAsync("converted to text");
            spiderMock.Setup(s => s.RemoveNodesFromDocument(It.IsAny<HtmlDocument>(),
            It.IsAny<string>())).Callback((HtmlDocument document, string expression) =>
            {
                var loggerMock = new Mock<ILogger<Spider>>();
                var nodeservicesMock = new Mock<INodeServices>();
                var realSpider = new Spider(loggerMock.Object, nodeservicesMock.Object);
                realSpider.RemoveNodesFromDocument(document, expression);
            });
            var validatorMock = new Mock<IValidator>();
            validatorMock.Setup(v => v.ConsideredBodyText(It.IsAny<string>())).Returns(true);
            var extractor = new Extractor(spiderMock.Object, validatorMock.Object);
            var doc = new HtmlDocument();
            var html = HtmlNode.CreateNode(@"
            <html>
            <body>
            <script>some js</script>
            vanlig brödtext
            </body>
            </html>
            ");
            doc.DocumentNode.AppendChild(html);

            var mockedConvertedText = extractor.ExtractBodyTextFromArticleDocument(doc);
            var processedHtmlDoc = doc.DocumentNode.OuterHtml;
            Assert.Equal("converted to text", mockedConvertedText);
            Assert.Contains("vanlig brödtext", processedHtmlDoc);
            Assert.DoesNotContain("<script>some js</script>", processedHtmlDoc);
            spiderMock.Verify(s => s.HtmlToTextAsync(It.IsAny<string>()), Times.Exactly(1));
            spiderMock.Verify(s => s.RemoveNodesFromDocument(It.IsAny<HtmlDocument>(), It.IsAny<string>()), Times.AtLeastOnce());
        }

         [Fact]
        public void ExtractBodyTextFromArticleDocumentusing_removes_links_from_html_document_and_converts_to_text()
        {
            var spiderMock = new Mock<ISpider>();
            spiderMock.Setup(s => s.HtmlToTextAsync(It.IsAny<string>())).ReturnsAsync("converted to text");
            spiderMock.Setup(s => s.RemoveNodesFromDocument(It.IsAny<HtmlDocument>(),
            It.IsAny<string>())).Callback((HtmlDocument document, string expression) =>
            {
                var loggerMock = new Mock<ILogger<Spider>>();
                var nodeservicesMock = new Mock<INodeServices>();
                var realSpider = new Spider(loggerMock.Object, nodeservicesMock.Object);
                realSpider.RemoveNodesFromDocument(document, expression);
            });
            var validatorMock = new Mock<IValidator>();
            validatorMock.Setup(v => v.ConsideredBodyText(It.IsAny<string>())).Returns(true);
            var extractor = new Extractor(spiderMock.Object, validatorMock.Object);
            var doc = new HtmlDocument();
            var html = HtmlNode.CreateNode(@"
            <html>
            <body>
            <a href='#'>some link</a>
            vanlig brödtext
            </body>
            </html>
            ");
            doc.DocumentNode.AppendChild(html);

            var mockedConvertedText = extractor.ExtractBodyTextFromArticleDocument(doc);
            var processedHtmlDoc = doc.DocumentNode.OuterHtml;
            Assert.Equal("converted to text", mockedConvertedText);
            Assert.Contains("vanlig brödtext", processedHtmlDoc);
            Assert.DoesNotContain("<a href='#'>some link</a>", processedHtmlDoc);
            spiderMock.Verify(s => s.HtmlToTextAsync(It.IsAny<string>()), Times.Exactly(1));
            spiderMock.Verify(s => s.RemoveNodesFromDocument(It.IsAny<HtmlDocument>(), It.IsAny<string>()), Times.AtLeastOnce());
        }


    }
}
