
using System;
using tracker.Domain.Models;

namespace tracker.Domain.Ports.Outbound
{
    public interface IPipeline
    {
        void SendForAnalysis(Article article);
        void Open(Action done);
        void Close();
    }
}