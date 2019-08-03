using src.Comain.Commands;
using src.Domain.Models;

namespace src.Domain.Commands
{
    public class AnalyzeArticleCommand : ICommand
    {   
        public Article Article { get; set; }
    }
}