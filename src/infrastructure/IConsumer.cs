namespace rabbitmq_dotnet_infrastructure
{
    public interface IConsumer<T>
    {
        void Consume(T message);
    }
}
