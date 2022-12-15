namespace rabbitmq_dotnet_infrastructure
{
    public interface IPublisher
    {
        Task Publish<T>(T message, string exchangeName = null);
    }
}
