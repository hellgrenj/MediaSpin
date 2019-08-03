using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using src.Comain.Commands;
using src.Domain.Commands;
using src.Domain.Models;
using src.Domain.Ports.Inbound;

namespace src.Domain.Services
{
    public class Mediator : IMediator
    {
        private readonly IEngine _engine;
        private readonly ILogger _logger;
        public Mediator(IEngine engine, ILogger<Mediator> logger)
        {
            _engine = engine;
            _logger = logger;
        }
        public void Send(ICommand command)
        {
            switch (command)
            {
                case AnalyzeArticleCommand analyzeArticleCommand:
                    _engine.Analyze(analyzeArticleCommand.Article);
                    break;
                default:
                    _logger.LogError("Could not find a handler for the command");
                    break;
            }
        }
    }
}