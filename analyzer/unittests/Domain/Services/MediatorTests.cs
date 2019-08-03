using Microsoft.Extensions.Logging;
using Moq;
using src.Domain.Commands;
using src.Domain.Models;
using src.Domain.Services;
using Xunit;

namespace unittests.Domain.Services
{
    public class MediatorTests
    {

        [Fact]
        public void Send_routes_command_to_correct_service()
        {
            var loggerMock = new Mock<ILogger<Mediator>>();
            var engineMock = new Mock<IEngine>();
            engineMock.Setup(e => e.Analyze(It.IsAny<Article>()));

            var mediator = new Mediator(engineMock.Object, loggerMock.Object);
            
            var command = new AnalyzeArticleCommand();
            mediator.Send(command);

            engineMock.Verify(e => e.Analyze(It.IsAny<Article>()), Times.Exactly(1));
        }
    }
}
