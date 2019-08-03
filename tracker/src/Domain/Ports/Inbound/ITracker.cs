using System.Collections.Generic;
using tracker.Domain.Models;

namespace tracker.Domain.Ports.Inbound
{
    public interface ITracker
    {
        void Scan(string baseUrl, List<Article> articles);
    }
}