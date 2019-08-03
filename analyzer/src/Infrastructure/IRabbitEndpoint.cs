namespace src.Infrastructure 
{
    public interface IRabbitEndpoint
    {
        void StartListening();
        void StopListening();
        bool IsConnectionsOpen();
    }
}