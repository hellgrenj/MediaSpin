using src.Comain.Commands;

namespace src.Domain.Ports.Inbound
{
    public interface IMediator
    {
        void Send(ICommand command);
        
    }
}