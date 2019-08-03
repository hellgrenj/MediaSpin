using System.Collections.Generic;
using tracker.Domain.Models;

namespace tracker.Domain.Ports.Outbound
{
    public interface IArticlesFileWriter
    {
        void WriteToJSON(List<Article> articles);
    }
}