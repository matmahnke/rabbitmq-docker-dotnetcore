using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace rabbitmq_dotnet_infrastructure
{
    public class ConsumerManager
    {
        public void RegisterQueueAndExchange<TType>(IModel? channel, string queueName, string exchangeName = null)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
                exchangeName = typeof(TType).Name;

            CreateQueueAndExchange(channel, exchangeName, queueName);
        }

        public void RegisterConsumer<TType, TConsumer>(IModel? channel, TConsumer consumer, string queueName) where TConsumer : IConsumer<TType>
        {
            var basicConsumer = new EventingBasicConsumer(channel);
            basicConsumer.Received += (sender, e) =>
            {
                ConsumerReceived<TType, TConsumer>(sender, e, consumer);
            };

            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: basicConsumer);
        }

        public void RegisterAndConsumeOneMessage<TType>(IModel? channel, Action<TType?> consumer, string queueName)
        {
            var response = channel.BasicGet(queue: queueName,
                autoAck: true);
            var @event = GetEventFromBody<TType>(response?.Body.ToArray());
            consumer.Invoke(@event);
        }

        private void CreateQueueAndExchange(IModel? channel, string exchangeName, string queueName)
        {
            CreateFanoutExchange(channel, exchangeName);
            CreateQueue(channel, queueName);
            BindQueueToExchange(channel, queueName, exchangeName);
        }

        private TType? GetEventFromBody<TType>(byte[] body)
        {
            var json = Encoding.UTF8.GetString(body);
            var @event = JsonSerializer.Deserialize<TType>(json);
            return @event;
        }

        private void ConsumerReceived<TEvent, TConsumer>(
            object? sender, BasicDeliverEventArgs e, TConsumer consumer) where TConsumer : IConsumer<TEvent>
        {
            var @event = GetEventFromBody<TEvent>(e.Body.ToArray());
            consumer.Consume(@event);
        }

        private IModel CreateQueue(IModel? channel, string queueName)
        {
            channel?.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            return channel;
        }

        private IModel? BindQueueToExchange(IModel? channel, string queue, string exchange)
        {
            channel?.QueueBind(queue, exchange, "#");
            return channel;
        }
        private IModel CreateFanoutExchange(IModel channel, string exchangeName)
        {
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true);
            return channel;
        }
    }
}
