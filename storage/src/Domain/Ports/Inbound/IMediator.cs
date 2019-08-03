using System.Threading.Tasks;
using storage.Domain.Commands;
using storage.Domain.Queries;

namespace storage.Domain.Ports.Inbound
{
    public interface IMediator
    {
        Task SendAsync(ICommand command);
        Task<TResponse> SendAsync<TResponse>(IQuery query);
    }
}