using RabbitMQ.Client;

namespace rabbitmq_dotnet_infrastructure
{
    public class MessageBrokerConfiguration
    {
        public string ConnectionString { get; set; } = "amqp://guest:guest@localhost:5672/";

        public IConnection GetConnection()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(ConnectionString)
            };
            return factory.CreateConnection();
        }
    }
}
