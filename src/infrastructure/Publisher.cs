using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace rabbitmq_dotnet_infrastructure
{
    public class Publisher: IPublisher
    {
        private readonly MessageBrokerConfiguration _messageBrokerConfiguration;

        public Publisher(MessageBrokerConfiguration messageBrokerConfiguration)
        {
            _messageBrokerConfiguration = messageBrokerConfiguration;
        }

        public async Task Publish<T>(T message, string exchangeName = null)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
                exchangeName = typeof(T).Name;

            using (var connection = _messageBrokerConfiguration.GetConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                    CreateFanoutExchange(channel, exchangeName)
                        .BasicPublish(
                        exchange: exchangeName,
                        routingKey: "#",
                        basicProperties: null,
                        body: body);
                }
            }
        }

        private IModel CreateFanoutExchange(IModel channel, string exchangeName)
        {
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true);
            return channel;
        }
    }
}
